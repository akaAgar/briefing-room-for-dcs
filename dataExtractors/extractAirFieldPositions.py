# Fill all positions on chosen airfield with aircraft (preferably same type)
# Extract Missions file to temp.txt
# Edit extracted data so that parking information is formatted as so ["parking"] = "17",-508.40157664646,233.57108844941, (VS code helps here)
# Run Script

f = open("temp.txt", "r")
data = f.read();
positions = [f.strip().replace('["parking"] = ', "").replace('"', "") for f in data.split("\n") if 'parking"' in f]
for pos in positions:
    posBits = pos.split(",")
    Id = int(posBits[0])
    print(f"Spot{Id}.Coordinates={posBits[1]},{posBits[2]}\nSpot{Id}.DCSID={Id}\nSpot{Id}.Type=OpenAirSpawn")
