import os
from shutil import copyfile

path = '/'.join(os.path.realpath(__file__).split('\\')[:-1])
print(path)
fileToDistribute='FindBestSplitsWOSF.py'
folders = [f for f in os.listdir(path) if 'config' in f and not os.path.isfile(os.path.join(path, f))]

for folder in folders:
    singles = [f for f in os.listdir(path+'/'+folder) if not os.path.isfile(os.path.join(path+'/'+folder, f))]
    for single in singles:
        target = path+'/'+folder+'/'+single+'/'
        check = False
        try:
            f = open(target+fileToDistribute)
                # Do something with the file
        except IOError:
                check = True                
                print("File not accessible")
        finally:
            f.close()
        if check:
            continue
        os.system('python '+target+fileToDistribute)
        

