require "Caracus_out" -- Mission lua file
file = io.open ("Waypoints.out", "w")
io.output(file)
local index = 0
for key,value in pairs(mission.coalition.red.country[1].vehicle.group[1].route.points) do --actualcode
    io.write("Point"..string.format("%04d", index).."="..value.x..","..value.y..",LandSmall\n")
    index = index + 1
end

for key,value in pairs(mission.coalition.neutrals.country[1].vehicle.group[1].route.points) do --actualcode
    io.write("Point"..string.format("%04d", index).."="..value.x..","..value.y..",LandMedium\n")
    index = index + 1
end

for key,value in pairs(mission.coalition.red.country[1].vehicle.group[1].route.points) do --actualcode
    io.write("Point"..string.format("%04d", index).."="..value.x..","..value.y..",LandLarge\n")
    index = index + 1
end


io.close(file)

