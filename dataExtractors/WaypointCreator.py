import configparser
config = configparser.ConfigParser()
config.read('Database\Theaters\Caucasus.ini')
template = """
[$INDEX$] = 
{
    ["alt"] = 0,
    ["action"] = "Turning Point",
    ["alt_type"] = "BARO",
    ["speed"] = 0,
    ["task"] = 
    {
        ["id"] = "ComboTask",
        ["params"] = 
        {
            ["tasks"] = 
            {
            }, -- end of ["tasks"]
        }, -- end of ["params"]
    }, -- end of ["task"]
    ["type"] = "Turning Point",
    ["ETA"] = 0,
    ["ETA_locked"] = false,
    ["y"] = $Y$,
    ["x"] = $X$,
    ["name"] = "",
    ["formation_template"] = "",
    ["speed_locked"] = true,
}, -- end of [$INDEX$]
"""
LandSmall = ""
LandSmallIndex = 1
LandMedium = ""
LandMediumIndex = 1
LandLarge = ""
LandLargeIndex = 1

for key in config["SpawnPoints"]:
    data = config["SpawnPoints"][key].split(",")
    datValue = template.replace("$X$", data[0])
    datValue = datValue.replace("$Y$", data[1])
    if data[2] == "LandSmall":
        datValue = datValue.replace("$INDEX$", str(LandSmallIndex))
        LandSmallIndex += 1
        LandSmall = LandSmall + datValue
    if data[2] == "LandMedium":
        datValue = datValue.replace("$INDEX$", str(LandMediumIndex))
        LandMediumIndex += 1
        LandMedium = LandMedium + datValue
    if data[2] == "LandLarge":
        datValue = datValue.replace("$INDEX$", str(LandLargeIndex))
        LandLargeIndex += 1
        LandLarge = LandLarge + datValue

file1 = open("dataExtractors\mapTemplates\Caracus.txt", "r")
text = file1.read()
file1.close()
text = text.replace("$SMALL$", LandSmall)
text = text.replace("$MEDIUM$", LandMedium)
text = text.replace("$LARGE$", LandLarge)

f = open("Caracus_out.txt", "a")
f.write(text)
f.close()