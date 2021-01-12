import sys
import csv
import os
import math
import numpy as np
from sklearn.linear_model import LinearRegression
from pathlib import Path

number = 1

#enum:
AIRPRESSURE = 0
AIRSPEED = 1
AIRTEMPOUT = 2
FLUIDPRESSURE = 3
FLUIDSPEED = 4
FLUIDTEMPOUT = 5
PRICE = 6
SAFETYFACTOR = 7

# problem: the matrix was computed with safetyfactor in mind
# affectionMatrix = [[True , True , True , False, True , False, False, True ],
#                    [True , True , False, False, False, False, False, True ],
#                    [False, False, False, False, False, False, False, True ],
#                    [True , True , True , True , False, False, True , True ],
#                    [False, False, False, True , False, False, False, True ],
#                    [False, False, False, False, False, False, True , True ],
#                    [True , True , True , True , True , True , False, True ],
#                    [True , True , True , True , True , True , True , True ]]

affectionMatrix = [[True , True , True , True , True , True , True , True ],
                   [True , True , True , True , True , True , True , True ],
                   [True , True , True , True , True , True , True , True ],
                   [True , True , True , True , True , True , True , True ],
                   [True , True , True , True , True , True , True , True ],
                   [True , True , True , True , True , True , True , True ],
                   [True , True , True , True , True , True , True , True ]]

lpNamesColum = ['apd', 'as', 'aTempO', 'fpd', 'fs', 'fTempO', 'p']
lpNamesRow = ['h','l','r','c','s','t','fTempI','am']

filenames = []
filenames.append('/'.join(os.path.realpath(__file__).split('\\')[:-1]) + '/h_l_nRows_nCircuits_fSpac_fThick_fTempIn_aMass_APresDrop.csv')
filenames.append('/'.join(os.path.realpath(__file__).split('\\')[:-1]) + '/h_l_nRows_nCircuits_fSpac_fThick_fTempIn_aMass_ASpeed.csv')
filenames.append('/'.join(os.path.realpath(__file__).split('\\')[:-1]) + '/h_l_nRows_nCircuits_fSpac_fThick_fTempIn_aMass_ATempOut.csv')
filenames.append('/'.join(os.path.realpath(__file__).split('\\')[:-1]) + '/h_l_nRows_nCircuits_fSpac_fThick_fTempIn_aMass_FPresDrop.csv')
filenames.append('/'.join(os.path.realpath(__file__).split('\\')[:-1]) + '/h_l_nRows_nCircuits_fSpac_fThick_fTempIn_aMass_FSpeed.csv')
filenames.append('/'.join(os.path.realpath(__file__).split('\\')[:-1]) + '/h_l_nRows_nCircuits_fSpac_fThick_fTempIn_aMass_FTempOut.csv')
filenames.append('/'.join(os.path.realpath(__file__).split('\\')[:-1]) + '/h_l_nRows_nCircuits_fSpac_fThick_fTempIn_aMass_P.csv')
filenames.append('/'.join(os.path.realpath(__file__).split('\\')[:-1]) + '/h_l_nRows_nCircuits_fSpac_fThick_fTempIn_aMass_SF.csv')

regressionpath = '/'.join(os.path.realpath(__file__).split('\\')[:-6]) + '/Regression/RegressionParams/'
systemVar = (os.path.realpath(__file__).split('\\')[-3]).split('_')
systemConf = systemVar[1]+'_'+systemVar[2]+'--'+systemVar[8]
regressionpath = regressionpath + systemConf+'/'
try:
    os.mkdir(regressionpath)
except:
    _=0


manualConfig = {}
automaticConfig = [[2 for _ in lpNamesRow]]

inputs = []
indicesNoError = []

def initFiles():
    global indicesNoError
    global inputs
    inputs.clear()
    indicesNoError.clear()
    for output in range(0,len(lpNamesColum)):
        inputs.append([])
        indicesNoError.append([])
        inputs[-1].append([]) # height = []
        inputs[-1].append([]) # length = []
        inputs[-1].append([]) # numberRows = []
        inputs[-1].append([]) # numberCircuits = []
        inputs[-1].append([]) # finSpacings = []
        inputs[-1].append([]) # finThicknesses = []
        inputs[-1].append([]) # fluidTempIns = []
        inputs[-1].append([]) # airMasses = []
        inputs[-1].append([]) # values = []
        inputs[-1].append([]) # error = []
        switch = {
            0 : 'AIR PRESSURE',
            1 : 'AIR SPEED',
            2 : 'AIR TEMP OUT',
            3 : 'FLUID PRESSURE',
            4 : 'FLUID SPEED',
            5 : 'FLUID TEMP OUT',
            6 : 'PRICE',
            7 : 'SAFETYFACTOR'
        }
        print(switch.get(output))
        with open(filenames[output], 'r') as csvfile:
            csvreader = csv.reader(csvfile, delimiter=';', quotechar='|')
            _ = csvreader.__next__()    #skip first line with names
            for row in csvreader:
                inputs[-1][0].append(row[0])
                inputs[-1][1].append(row[1])
                inputs[-1][2].append(row[2])
                inputs[-1][3].append(row[3])
                inputs[-1][4].append(row[4])
                inputs[-1][5].append(row[5])
                inputs[-1][6].append(row[6].replace(',','.'))
                inputs[-1][7].append(row[7])
                inputs[-1][8].append(row[8].replace(',','.'))
                inputs[-1][9].append(False if row[9] == '' else True)


            csvfile.close()
        indicesNoError[-1] = [i for i, x in enumerate(inputs[-1][9]) if not x]
    print('')

def regression(inputs,inputIndexFirst,inputIndexSecond,currentConfigs,middleNumbers):
    x_values = []
    y_values = []
    for i in inputIndexSecond:
        helper = []
        isContinue = True
        for c in inputIndexFirst:
            if currentConfigs[c] == 0:
                if int(inputs[c][i]) >= middleNumbers[c]:
                    isContinue = False
            elif currentConfigs[c] == 1:
                if int(inputs[c][i]) < middleNumbers[c]:
                    isContinue = False
            #else if currentConfigs[c] == '2':
                #isContinue = True #do not actually set, since it should check the other 2
        if not isContinue:
            continue

        for k in inputIndexFirst:
                helper.append(inputs[k][i])
        x_values.append(helper)
        y_values.append(inputs[8][i])

    if len(x_values) < 1:
        # print('no x-values')
        return -10000,[],-10000,[]

    if len(x_values[0]) == 1:
        x = np.array(x_values,  dtype=float).reshape((-1,1))
    else:
        x = np.array(x_values,  dtype=float)
    y = np.array(y_values,  dtype=float)

    model = LinearRegression().fit(x, y)
    r_sq = model.score(x, y)
    interception = round(model.intercept_,5)
    slope = list(np.around(np.array(model.coef_),5))
    determination = round(r_sq,2)
    # print('Config :')
    # print('coefficient of determination:', determination)
    # print('intercept:', interception)
    # print('slope:', slope)
    # print('')
    x_values.clear()
    y_values.clear()
    breakList = [(middleNumbers[j],j,currentConfigs[j]) for j in range(0, len(lpNamesRow))]
    return interception, slope, determination, breakList

class Config:
    def __init__(self):
        self.interceptions = []
        self.slopes = []
        self.determinations = []
        self.breaks = []
        self.config = []
    def clear(self):
        self.interceptions.clear()
        self.slopes.clear()
        self.breaks.clear()
        self.determinations.clear()
        self.config.clear()

def compute(ratio, currentConfig):
    variableLists = []
    for output in range(0,len(lpNamesColum)):
        inputIndexes = []
        for variable in range(0, len(lpNamesRow)):
            if affectionMatrix[output][variable]:
                inputIndexes.append(variable)
        variableLists.append(inputIndexes)

    middleNumbers = []
    for i in range(0,len(lpNamesRow)):
        middleNumber = [int(x) for x in list(set(inputs[0][i]))]
        middleNumber.sort()
        if i in manualConfig:
            middleNumbers.append(middleNumber[math.ceil(len(middleNumber)*manualConfig[i][0])-1])
        else:
            middleNumbers.append(middleNumber[math.ceil(len(middleNumber)*ratio)-1])

    config = Config()
    for output in range(0,len(lpNamesColum)):
        test = set(indicesNoError[output])
        # print(lpNamesColum[output]+':')
        interception, slope, determination, abreak = regression(inputs[output],variableLists[output],test,currentConfig,middleNumbers)
        config.interceptions.append(interception)
        config.slopes.append(slope)
        config.determinations.append(determination)
        config.breaks.append(abreak)
        config.config = currentConfig

    return config

def computeNewConfigs(computedConfigs):
        test = True
        for variable in range(0,len(lpNamesRow)):
            print('variable: '+str(variable))
            computedConfigs.append([])

            if variable in manualConfig:
                computedConfigs[-1].append([])
                for autoConf in automaticConfig:
                    print(' conf: '+str(autoConf))
                    # helper = [x for x in autoConf]
                    # helper[variable] = 0
                    computedConfigs[-1][-1].append(compute(manualConfig[variable][0], autoConf))
                    # helper = [x for x in autoConf]
                    # helper[variable] = 1
                    computedConfigs[-1][-1].append(compute(manualConfig[variable][0], autoConf))
                continue

            for ratio in ratios:
                computedConfigs[-1].append([])
                for autoConf in automaticConfig:
                    print(' conf: '+str(autoConf))
                    helper = [x for x in autoConf]
                    helper[variable] = 0
                    computedConfigs[-1][-1].append(compute(ratio, helper))
                    helper = [x for x in autoConf]
                    helper[variable] = 1
                    computedConfigs[-1][-1].append(compute(ratio, helper))

            # if test:
            #     test = False
            #     continue
            # break #TEST


initFiles()

Path(regressionpath).mkdir(parents=True, exist_ok=True)
# filelist = [ f for f in os.listdir(regressionpath)]
# for f in filelist:
#     os.remove(os.path.join(regressionpath, f))
ratios = [0.15, 0.35, 0.5, 0.65, 0.85]
ratiosDic = {0.15:0, 0.35:1, 0.5:2, 0.65:3, 0.85:4}


computedConfigs = []
print('step: '+str(len(computedConfigs)))

# compute the first regressions without any splits
computedConfigs.append([[[compute(1,automaticConfig[0])]]])
determinationAVG = [x for x in computedConfigs[-1][0][0][0].determinations]
firstValue = 0
for x in determinationAVG:
    firstValue += x
firstValue = firstValue / len(determinationAVG)
print('determinationAVG = '+str(firstValue))



with open(regressionpath+'RegressionParams.txt', 'w') as f:
    f.write('avg: '+str(firstValue)+'  with len='+str(len(determinationAVG))+'\n')
    for i in range(0, len(determinationAVG)):
        f.write(lpNamesColum[i]+ ': ' + str(determinationAVG[i])+'\n')
    f.write('\n')

# compute one split for every vairable we have
# only use the split if it is more than 1 percent points better
if(True):
    test = True
    for i in range(0,len(lpNamesRow)):
        # if(v=='l' or v=='h' or v=='t'):
        #     continue
        print('step: '+str(len(computedConfigs)))
        computedConfigs.append([])
        computeNewConfigs(computedConfigs[-1])

        computedDeterminations = []
        bestRatio = 0
        bestRatioIdx = 0
        bestValue = firstValue + 0.01
        bestVariable = -1
        bestVariableIndex = -1
        repressionsplitpath = regressionpath + "split"+str(i)+'/'
        try:
            os.mkdir(repressionsplitpath)
        except:
            _=0
        for variable in range(0,len(computedConfigs[-1])):
            computedDeterminations.append([])
            for ratio in range(0,len(computedConfigs[-1][variable])):
                determinationAVG = [0 for _ in computedConfigs[-1][variable][ratio][0].determinations]
                for config in computedConfigs[-1][variable][ratio]:
                    for i in range(0, len(config.determinations)):
                        determinationAVG[i] += config.determinations[i]
                determinationAVG = [x/len(computedConfigs[-1][variable][ratio]) for x in determinationAVG]
                avg = 0
                for x in determinationAVG:
                    avg += x
                avg = avg/len(determinationAVG)
                if avg > bestValue:
                    bestValue = avg
                    bestRatio = ratios[ratio]
                    bestRatioIdx = ratio
                    bestVariable = variable % len(lpNamesRow)
                    bestVariableIndex = variable
                computedDeterminations[-1].append(determinationAVG)
                # print('avg = '+str(avg))

                with open(repressionsplitpath+'Candidates '+lpNamesRow[variable % len(lpNamesRow)]+'-'+str(ratios[ratio])+'.txt', 'w') as f:
                    f.write('ratio: '+str(ratios[ratio])+'\n')
                    f.write('avg: '+str(avg)+'  with len='+str(len(determinationAVG))+'\n')
                    for i in range(0, len(determinationAVG)):
                        f.write(lpNamesColum[i]+ ': ' + str(determinationAVG[i])+'\n')
                    f.write('\n')

        print('determinationAVG = '+str(bestValue))
        print('bestRatio = '+str(bestRatio))
        print('bestVariable = '+str(bestVariable))
        # break #TEST
        if(bestVariable in manualConfig or bestVariable == -1):
            break

        with open(repressionsplitpath+'RegressionSplit '+lpNamesRow[bestVariable]+'-'+str(bestRatio)+'.txt', 'w') as f:
            f.write('avg: '+str(bestValue)+'\n')
            for i in range(0, len(computedDeterminations[bestVariable][bestRatioIdx])):
                f.write(lpNamesColum[i]+ ': ' + str(computedDeterminations[bestVariable][bestRatioIdx][i])+'\n')
            f.write('\n')

        manualConfig[bestVariable] = (bestRatio,bestVariableIndex)
        autoConfLength = len(automaticConfig)
        firstValue = bestValue

        newAutoConf = []

        for automaticConf in range(0,autoConfLength):
            helper = [x for x in automaticConfig[automaticConf]]
            helper[bestVariable] = 0
            newAutoConf.append(helper)
            helper = [x for x in automaticConfig[automaticConf]]
            helper[bestVariable] = 1
            newAutoConf.append(helper)
        automaticConfig = [x for x in newAutoConf]
        newAutoConf.clear()

        print(automaticConfig)
        print(manualConfig)


pathLinearProgram = 'C:/Users/Roth.J/Documents/Master/EcoConfBasic/LinearProgram/LPFamily/'
linearProgramName = 'GeneratedLP'

Path(pathLinearProgram).mkdir(parents=True, exist_ok=True)
filelist = [ f for f in os.listdir(pathLinearProgram)]
for f in filelist:
    os.remove(os.path.join(pathLinearProgram, f))

def writeLinearProgram(config, index):
    with open(pathLinearProgram+linearProgramName+str(config.config)+'.lp', 'w') as linearProgram:
        linearProgram.write('/* Objective function */\n')
        linearProgram.write('min: fpd + 4apd + fpd1 + 4apd1 + 0.1p + 0.1p1 - 200fs - 200fs1;\n\n')
        for i in range(0, len(config.interceptions)):
            helper = ''
            for j in range(0,len(config.slopes[i])):
                helper += str(config.slopes[i][j])+lpNamesRow[config.breaks[i][j][1]]+' + '
            linearProgram.write(lpNamesColum[i]+ ' = '+ helper + str(config.interceptions[i])+';\n')

        linearProgram.write('\n')
        linearProgram.write('aTempO > 18.6;\n')
            
        linearProgram.write('transR = 0.05aTempO - 0.25;\n\n')

        linearProgram.write('/* Variable bounds */\n')
        #linearProgram.write('am = 60000;\n')

        lowerBound = ['150', '150', '6', '1', '25','1','5','1000']
        upperBound = ['2750', '4200', '14', '60', '60', '3', '25', '150000']

        # in this stage we do not use height, length and airmass as vairable, so we can skip these
        for var in range(2,len(lpNamesRow) - 1):
            breakIndex = -1
            for i in range(0,len(config.breaks[0])):
                if var == config.breaks[0][i][1]:
                    breakIndex = i
                    break
            if(config.config[var] == 1):
                if breakIndex == -1:
                    print('BreakIndex should be part of the Breaks!!')
                linearProgram.write(lpNamesRow[var] + ' > ' + str(config.breaks[0][breakIndex][0]) + ';\n')
            else:
                linearProgram.write(lpNamesRow[var] + ' > '+ lowerBound[var] + ';\n')
            if(config.config[var] == 0):
                if breakIndex == -1:
                    print('BreakIndex should be part of the Breaks!!')
                linearProgram.write(lpNamesRow[var] + ' < ' + str(config.breaks[0][breakIndex][0]) + ';\n')
            else:
                linearProgram.write(lpNamesRow[var] + ' < '+ upperBound[var] + ';\n')

        linearProgram.write('s = 25s25 + 30s30 + 40s40 + 50s50 + 60s60;\n')
        linearProgram.write('1 = s25 + s30 + s40 + s50 + s60;\n')
        linearProgram.write('fs > 0;\n')
        linearProgram.write('fs < 2;\n')
        linearProgram.write('as > 0;\n')
        linearProgram.write('as < 2;\n')
        linearProgram.write('r = 6r6 + 8r8 + 10r10 + 12r12 + 14r14;\n')
        linearProgram.write('1 = r6 + r8 + r10 + r12 + r14;\n')
        # linearProgram.write('sf > 100;\n')
        # linearProgram.write('sf < 110;\n')
        linearProgram.write('fTempO > 5;\n')
        linearProgram.write('fTempO < 25;\n')
        linearProgram.write('apd > 0;\n')
        linearProgram.write('apd < 500;\n')
        linearProgram.write('fpd > 0;\n')
        linearProgram.write('fpd < 500;\n')
        linearProgram.write('p > 0;\n')
        linearProgram.write('aTempO > 5;\n')
        linearProgram.write('aTempO < 25;\n')
        linearProgram.write('\n /*Variables:*/\n')

        linearProgram.write('int h')
        integers = ['l','s','t','c','r','sf','p','am']
        for i in integers:
                linearProgram.write(', ' + i)
        linearProgram.write(';\n')
        
        linearProgram.write('bin s25,s30,s40,s50,s60,r6,r8,r10,r12,r14;\n')

systemVar = (os.path.realpath(__file__).split('\\')[-3]).split('_')
systemConf = systemVar[1]+'_'+systemVar[2]+'--'+systemVar[8]
paramConf = 'ParamsT='+str(0 if systemVar[-1] == 'I' else 1)+'='+systemVar[6][-1:]
paramFilename = 'ParamFile'
def writeParamFile(config, index):
    paramFilePath = 'C:/Users/Roth.J/Documents/Master/EcoConfBasic/LinearProgram/ParamFiles/'
    confFolder = paramFilePath+systemConf+'/'
    try:
        os.mkdir(confFolder)
    except:
        _=0
    paramFolder = confFolder+paramConf+'/'
    try:
        os.mkdir(paramFolder)
    except:
        _=0
    paramFilePath = paramFolder
    with open(paramFilePath+paramFilename+str(config.config)+'.txt', 'w') as paramFile:
        for i in range(0, len(config.interceptions)):
            helper = ''
            for j in range(0,len(config.slopes[i])):
                helper += lpNamesRow[config.breaks[i][j][1]]+':'+str(config.slopes[i][j])+' '
            paramFile.write(lpNamesColum[i]+ '\n'+ helper + ':'+str(config.interceptions[i])+'\n')
            
        lowerBound = ['150', '150', '6', '1', '25','1','5','1000']
        upperBound = ['2750', '4200', '14', '60', '60', '3', '25', '150000']

        # in this stage we do not use height, length and airmass as vairable, so we can skip these
        for var in range(2,len(lpNamesRow) - 1):
            breakIndex = -1
            for i in range(0,len(config.breaks[0])):
                if var == config.breaks[0][i][1]:
                    breakIndex = i
                    break
            if(config.config[var] == 1):
                if breakIndex == -1:
                    print('BreakIndex should be part of the Breaks!!')
                paramFile.write(lpNamesRow[var] + '>' + str(config.breaks[0][breakIndex][0]) + '\n')
            else:
                paramFile.write(lpNamesRow[var] + '>'+ lowerBound[var] + '\n')
            if(config.config[var] == 0):
                if breakIndex == -1:
                    print('BreakIndex should be part of the Breaks!!')
                paramFile.write(lpNamesRow[var] + '<' + str(config.breaks[0][breakIndex][0]) + '\n')
            else:
                paramFile.write(lpNamesRow[var] + '<'+ upperBound[var] + '\n')


def generation():
    # print('index:')
    # for index in range(0,len(computedConfigs)):
    #     print(' variable:')
    #     for variable in range(0, len(computedConfigs[index])):
    #         print('     ratio:')
    #         for ratio in range(0, len(computedConfigs[index][variable])):
    #             print('         config:')
    #             for config in computedConfigs[index][variable][ratio]:
    #                 print('             '+str(config.config))
    #                 print('             '+str(config.determinations))
    #                 print('             '+str(config.interceptions))
    #                 print('             '+str(config.breaks[0]))
    #                 print('')
    # print('')


    print('variable:')
    for variable in range(0, len(computedConfigs[-1])):
        print('    ratio:')
        for ratio in range(0, len(computedConfigs[-1][variable])):
            print('        config:')
            for config in computedConfigs[-1][variable][ratio]:
                print('             '+str(config.config))
    print('')

    bestConfigs = []
    if len(manualConfig) == 0:
        bestConfigs.append(computedConfigs[0][0][0][0])
    else:
        # for config in range(0,len(computedConfigs[-1])):
        variable,ratio = list(manualConfig.items())[-1]
        print(' len computedconfig[-1][variable]: '+str(len(computedConfigs[-1][ratio[1]])))
        print(' ratio: '+str(ratio[0]))
        if len(computedConfigs[-1][ratio[1]]) == 1:
            print(' len computedconfig[-1][variable][0]: '+str(len(computedConfigs[-1][ratio[1]][0])))
            for c in computedConfigs[-1][ratio[1]][0]:
                bestConfigs.append(c)
            # continue    
        else:
            print(' len computedconfig[-1][variable][ratio[0]]: '+str(len(computedConfigs[-1][ratio[1]][ratiosDic[ratio[0]]])))
            for c in computedConfigs[-1][ratio[1]][ratiosDic[ratio[0]]]:
                bestConfigs.append(c)

    count = 0
    for bconfig in bestConfigs:
        writeLinearProgram(bconfig, count)
        writeParamFile(bconfig, count)
        count += 1


print('Generate Linear Programs')
generation()

