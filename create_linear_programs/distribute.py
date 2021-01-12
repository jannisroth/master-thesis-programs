import os
from shutil import copyfile

path = '/'.join(os.path.realpath(__file__).split('\\')[:-1])
print(path)
fileToDistribute='FindBestSplitsWOSF.py'
folders = [f for f in os.listdir(path) if 'config' in f and not os.path.isfile(os.path.join(path, f))]

source = path+'/'+fileToDistribute
for folder in folders:
    singles = [f for f in os.listdir(path+'/'+folder) if not os.path.isfile(os.path.join(path+'/'+folder, f))]
    for single in singles:
        target = path+'/'+folder+'/'+single+'/'
        try:
            copyfile(source, target+fileToDistribute)
        except:
            _=0

