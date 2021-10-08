require "Syria_out" -- Mission lua file
file = io.open ("Waypoints.out", "w")
io.output(file)
local index = 0
io.write("[RedCoordinates]\n")
for key,value in pairs(mission.coalition.red.country[1].plane.group[1].route.points) do --actualcode
    io.write("Waypoint"..string.format("%04d", index).."="..value.x..","..value.y.."\n")
    index = index + 1
end
io.write("[BlueCoordinates]\n")
for key,value in pairs(mission.coalition.blue.country[1].plane.group[1].route.points) do --actualcode
    io.write("Waypoint"..string.format("%04d", index).."="..value.x..","..value.y.."\n")
    index = index + 1
end



io.close(file)

