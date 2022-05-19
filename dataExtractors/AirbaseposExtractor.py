# Extracts based on a dump from the debug
import configparser
from glob import glob

mapPrefix = "PersianGulf"
f = open("dump.txt", "r")
data = f.read()
airbaseMap = {}
for x in data.split(";"):
    temp = x.split(":")
    airbaseMap[temp[0]] = temp[1]

# print(airbaseMap)

files = glob(f"Database/TheatersAirbases/{mapPrefix}*.ini")

config = configparser.ConfigParser()
config.optionxform=str
for file in files:
    config.read(file)
    config.remove_section('RunwaySpawns')
    if config["Airbase"]["DCSID"] in airbaseMap :
        tempData = airbaseMap[config["Airbase"]["DCSID"]]
        config["Parking"] = {}
        for line in tempData.strip().split("\n"):
            parts = line.split("=")
            key = parts[0]
            value = parts[1]
            config["Parking"][key] = value
            with open(file, 'w') as configfile:
              config.write(configfile, space_around_delimiters=False)

