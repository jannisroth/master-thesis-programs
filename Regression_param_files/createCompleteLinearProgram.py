import sys
import os
from os import listdir
from os.path import isfile, join

# mypath = 'C:/Users/Roth.J/Documents/Master/EcoConfBasic/LinearProgram/ParamFiles/'
# hc='HC100'

# folders = [f for f in listdir(mypath) if (not isfile(join(mypath, f)) and '=' in f)]
# print('folders:')
# print(folders)

outputs = ['apd', 'as', 'aTempO', 'fpd', 'fs', 'fTempO', 'p']
variables = ['h','l','r','c','s','t','fTempI','am']

class Config:
    def __init__(self):
        #hold per output a list of variables
        self.matrix = []
        # hold per variable and output a tuple of upper and lower bound
        self.bounds = [(100,300),(0,3),(5,25),(100,300),(0,1.25),(5,25),(0,None),(None,None),(None,None)] #bounds for the outputs
        # self.bounds = [(0,300),(0,3),(5,25),(0,300),(0,1.3),(5,25),(0,None),(None,None),(None,None)] #bounds for the outputs
        self.t = ''

# sclale the parameters for the objective function
#   fpd: /300
#   apd: /300
#   p: /20000
#   fs: /2
#other possible parameters for the objective function: weight

# objectiveFunction = 'min: - 0.00333fpdi - 0.00333fpdo + 0.00333apdi + 0.00333apdo + 0.00005pi + 0.00005po - 0.5fsi - 0.5fso;' +.01666*si+.01666*so 
objectiveFunction = 'min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;'
# hold the different variable types
finSpacingsSmall = ['s25','s30','s40']
finSpacingsBig = ['s30','s40','s50','s60']
finSpacings = ['s25','s30','s40','s50','s60']
numberRows = ['r6','r8','r10','r12','r14']
variablesTypeBin = finSpacings+numberRows
variableTypeInt = ['h','l','s','t','c','r','am']

intakeList = []
outtakeList = []

def createFolders(folders, mypath):
    print('IntakeFolders:')
    for intakefolder in folders:
        if intakefolder.split('=')[1] == '1':
            continue
        print(intakefolder)
        files = [f for f in listdir(mypath+intakefolder+'/') if isfile(join(mypath+intakefolder+'/', f))]
        intakeList.append([])
        for f in files:
            with open(mypath+intakefolder+'/'+f, 'r') as paramFile:
                isBound = False
                currentBound = ('',0)
                intakeList[-1].append(Config())
                intakeList[-1][-1].t = intakefolder.split('=')[2]
                for line in paramFile:
                    line=line[:-1]
                    if('<' in line or '>' in line):
                        isBound = True
                    if(isBound and '>' in line and currentBound[0] != line.split('>')[0]):
                        currentBound = (line.split('>')[0],line.split('>')[1])
                    elif(isBound and '<' in line and currentBound[0] == line.split('<')[0]):
                        intakeList[-1][-1].bounds.append((currentBound[1],line.split('<')[1]))
                    if(not isBound and line in outputs):
                        continue
                    elif(not isBound and not line in outputs):
                        varList = line.split(' ')
                        intakeList[-1][-1].matrix.append([])
                        for var in varList:
                            intakeList[-1][-1].matrix[-1].append(var.split(':')[1])
                intakeList[-1][-1].bounds.append((None,None))       

    print('OuttakeFolders:')
    for outtakefolder in folders:
        if outtakefolder.split('=')[1] == '0':
            continue
        print(outtakefolder)
        files = [f for f in listdir(mypath+outtakefolder+'/') if isfile(join(mypath+outtakefolder+'/', f))]
        outtakeList.append([])
        for f in files:
            with open(mypath+outtakefolder+'/'+f, 'r') as paramFile:
                isBound = False
                currentBound = ('',0)
                outtakeList[-1].append(Config())
                outtakeList[-1][-1].t = outtakefolder.split('=')[2]
                for line in paramFile:
                    line=line[:-1]
                    if('<' in line or '>' in line):
                        isBound = True
                    if(isBound and '>' in line and not currentBound[0] == line.split('>')[0]):
                        currentBound = (line.split('>')[0],line.split('>')[1])
                    elif(isBound and '<' in line and currentBound[0] == line.split('<')[0]):
                        outtakeList[-1][-1].bounds.append((currentBound[1],line.split('<')[1]))

                    if(not isBound and line in outputs):
                        continue
                    elif(not isBound and not line in outputs):
                        varList = line.split(' ')
                        outtakeList[-1][-1].matrix.append([])
                        for var in varList:
                            outtakeList[-1][-1].matrix[-1].append(var.split(':')[1])
                outtakeList[-1][-1].bounds.append((None,None))


# pathLinearProgram = 'C:/Users/Roth.J/Documents/EcoCond/Programm/EcoConf/EcoConf/bin/Debug/'+hc+'/'
linearProgramName = 'GeneratedLP'

def writeLinearProgram(folderIndexIntake, folderIndexOuttake, intakeIndex, outtakeIndex,pathLinearProgram):
    with open(pathLinearProgram+linearProgramName+'-'+str(folderIndexIntake)+'-'+str(folderIndexOuttake)+'-'+str(intakeIndex)+'-'+str(outtakeIndex)+'.lp', 'w') as linearProgram:
        linearProgram.write('/* Objective function */\n')
        linearProgram.write(objectiveFunction+'\n\n')
        #intake matrix
        counter = 0
        for output in [out+'i' for out in outputs]:
            linearProgram.write(output+' = '+' + '.join([str(intakeList[folderIndexIntake][intakeIndex].matrix[counter][i])+' '+variables[i]+'i' for i in range(0,len(variables))]) + ' + ' +  intakeList[folderIndexIntake][intakeIndex].matrix[counter][-1]+';\n')
            counter += 1
        linearProgram.write('\n')

        #outtake matrix
        counter = 0
        for output in [out+'o' for out in outputs]:
            linearProgram.write(output+' = '+' + '.join([str(outtakeList[folderIndexOuttake][outtakeIndex].matrix[counter][i])+' '+variables[i]+'o' for i in range(0,len(variables))]) + ' + ' + outtakeList[folderIndexOuttake][outtakeIndex].matrix[counter][-1]+';\n')
            counter += 1
        linearProgram.write('\n')

        linearProgram.write('/* Variable bounds */\n')

        #intake bounds
        linearProgram.write('aTempOi > 18.6;\n')
        linearProgram.write('1 =' + ' + '.join([r+'i' for r in numberRows])+';\n')
        linearProgram.write('ri =' + ' + '.join([r.split('r')[1]+r+'i' for r in numberRows])+';\n')
        if intakeList[folderIndexIntake][intakeIndex].t == '3':
            linearProgram.write('1 =' + ' + '.join([s+'i' for s in finSpacingsBig])+';\n')
            linearProgram.write('si =' + ' + '.join([s.split('s')[1]+s+'i' for s in finSpacingsBig])+';\n')
        else:
            linearProgram.write('1 =' + ' + '.join([s+'i' for s in finSpacingsSmall])+';\n')
            linearProgram.write('si =' + ' + '.join([s.split('s')[1]+s+'i' for s in finSpacingsSmall])+';\n')
        linearProgram.write('ti = '+ intakeList[folderIndexIntake][intakeIndex].t +';\n')
        counter = 0
        for output in [out+'i' for out in outputs]:
            if(intakeList[folderIndexIntake][intakeIndex].bounds[counter][0] != None):
                linearProgram.write(output + ' > ' + str(intakeList[folderIndexIntake][intakeIndex].bounds[counter][0]) + ';\n')
            if(intakeList[folderIndexIntake][intakeIndex].bounds[counter][1] != None):
                linearProgram.write(output + ' < ' + str(intakeList[folderIndexIntake][intakeIndex].bounds[counter][1]) + ';\n')
            counter +=1
        for variable in [var+'i' for var in variables]:
            if(intakeList[folderIndexIntake][intakeIndex].bounds[counter][0] != None):
                linearProgram.write(variable + ' > ' + str(intakeList[folderIndexIntake][intakeIndex].bounds[counter][0]) + ';\n')
            if(intakeList[folderIndexIntake][intakeIndex].bounds[counter][1] != None):
                linearProgram.write(variable + ' < ' + str(intakeList[folderIndexIntake][intakeIndex].bounds[counter][1]) + ';\n')
            counter +=1

        linearProgram.write('\n')
        #outtake bounds
        linearProgram.write('/* aTempOo < 11.4; */\n')
        linearProgram.write('1 =' + ' + '.join([r+'o' for r in numberRows])+';\n')
        linearProgram.write('ro =' + ' + '.join([r.split('r')[1]+r+'o' for r in numberRows])+';\n')
        if outtakeList[folderIndexOuttake][outtakeIndex].t == '3':
            linearProgram.write('1 =' + ' + '.join([s+'o' for s in finSpacingsBig])+';\n')
            linearProgram.write('so =' + ' + '.join([s.split('s')[1]+s+'o' for s in finSpacingsBig])+';\n')    
        else:
            linearProgram.write('1 =' + ' + '.join([s+'o' for s in finSpacingsSmall])+';\n')
            linearProgram.write('so =' + ' + '.join([s.split('s')[1]+s+'o' for s in finSpacingsSmall])+';\n')
        linearProgram.write('to = '+ outtakeList[folderIndexOuttake][outtakeIndex].t +';\n')
        counter = 0
        for output in [out+'o' for out in outputs]:
            if(outtakeList[folderIndexOuttake][outtakeIndex].bounds[counter][0] != None):
                linearProgram.write(output + ' > ' + str(outtakeList[folderIndexOuttake][outtakeIndex].bounds[counter][0]) + ';\n')
            if(outtakeList[folderIndexOuttake][outtakeIndex].bounds[counter][1] != None):
                linearProgram.write(output + ' < ' + str(outtakeList[folderIndexOuttake][outtakeIndex].bounds[counter][1]) + ';\n')
            counter +=1
        for variable in [var+'o' for var in variables]:
            if(outtakeList[folderIndexOuttake][outtakeIndex].bounds[counter][0] != None):
                linearProgram.write(variable + ' > ' + str(outtakeList[folderIndexOuttake][outtakeIndex].bounds[counter][0]) + ';\n')
            if(outtakeList[folderIndexOuttake][outtakeIndex].bounds[counter][1] != None):
                linearProgram.write(variable + ' < ' + str(outtakeList[folderIndexOuttake][outtakeIndex].bounds[counter][1]) + ';\n')
            counter +=1

        linearProgram.write('int ' + ','.join([var+'i' for var in variableTypeInt])  + ';\n')
        linearProgram.write('int ' + ','.join([var+'o' for var in variableTypeInt])  + ';\n')
        linearProgram.write('bin ' + ','.join([var+'i' for var in variablesTypeBin]) + ';\n')
        linearProgram.write('bin ' + ','.join([var+'o' for var in variablesTypeBin]) + ';\n')

def makeCombinations(folderIntakeIndex, folderOuttakeIndex,pathLinearProgram):
    intakeConfCounter = 0
    for intakeConf in intakeList[folderIntakeIndex]:
        outtakeConfCounter = 0
        for outtakeConf in outtakeList[folderOuttakeIndex]:
            writeLinearProgram(folderIntakeIndex,folderOuttakeIndex,intakeConfCounter,outtakeConfCounter,pathLinearProgram)
            outtakeConfCounter += 1
        intakeConfCounter +=1

# mypath = 'C:/Users/Roth.J/Documents/Master/EcoConfBasic/LinearProgram/ParamFiles/'
path = '/'.join(os.path.realpath(__file__).split('\\')[:-1])+'/'
pathEtawin = 'C:/EtaWin/'
print(path)
hcFolders = [f for f in os.listdir(path) if '--' in f and not os.path.isfile(os.path.join(path, f))]
print(hcFolders)
hcDict = {
    "h642_l606":"HC4",
    "h642_l912":"HC6",
    "h948_l912":"HC9",
    "h948_l1218":"HC12",
    "h1254_l1218":"HC16",
    "h1254_l1524":"HC20",
    "h1560_l1524":"HC25",
    "h1560_l1830":"HC30",
    "h1866_l1830":"HC36",
    "h1866_l2136":"HC42",
    "h2172_l2136":"HC49",
    "h2172_l2442":"HC56",
    "h2478_l2442":"HC64",
    "h2478_l2748":"HC72",
    "h2750_l2748":"HC80",
    "h2750_l3054":"HC90",
    "h2750_l3360":"HC100",
    "h2750_l3666":"HC110"
}
for f in hcFolders:
    folders = [f for f in listdir(path+f) if (not isfile(join(path+f, f)) and '=' in f)]
    print('folders:')
    print(folders)
    intakeList.clear()
    outtakeList.clear()
    createFolders(folders,path+f+'/')
    pathLinearProgram = ''
    if f.split('--')[0] in hcDict:
        pathLinearProgram = pathEtawin + hcDict[f.split('--')[0]]+'/'
    else:
        continue
    # make every possible combination
    intakeCounter = 0
    try:
        os.mkdir(pathLinearProgram)
    except:
        _=0
    for intake in intakeList:
        outtakeCounter = 0
        for outtake in outtakeList:
            makeCombinations(intakeCounter, outtakeCounter,pathLinearProgram)
            outtakeCounter += 1
        intakeCounter += 1
