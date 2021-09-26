from glob import glob
from pprint import pprint

folders = glob("D:/Other Games/DCS World OpenBeta/CoreMods/aircraft/*/Liveries/*/*/description.lua")
zips = glob("D:/Other Games/DCS World OpenBeta/CoreMods/aircraft/*/Liveries/*/*.zip")
zips = [x.replace(".zip", "") for x in zips]
orgs = [x.split("\\")[-3:-1] for x in folders] + [x.split("\\")[-2:] for x in zips]
liveriesDict = {}

for livery in orgs:
    if livery[0] in liveriesDict.keys():
        liveriesDict[livery[0]] = liveriesDict[livery[0]] + f", {livery[1]}"
    else: 
       liveriesDict[livery[0]] = livery[1];

for key in liveriesDict.keys():
    print(f"{key}: Liveries={liveriesDict[key]}\n")
  

