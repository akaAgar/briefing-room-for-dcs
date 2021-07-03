# Fill all positions on chosen airfield with aircraft (preferably same type)
# Extract Missions file to temp.txt
# Edit extracted data so that parking information is formatted as so ["formation_template"] = 87028.301886792,-669575.47169811, (VS code helps here)
# Run Script

type = "LandLarge"
side = "Blue"
f = open("temp.txt", "r")
data = f.read();
positions = [f.strip().replace('["formation_template"] = ', "") for f in data.split("\n") if "formation_template" in f]
count = 145
for pos in positions:
    posBits = pos.split(",")
    print(f"Point{count:04d}={posBits[0]},{posBits[1]},{type},{side}")
    count = count + 1
