require "Marianas_out" -- Mission lua file
file = io.open ("Waypoints.out", "w")
io.output(file)
local first = true
local groupIndex = 1;
for _,group in pairs(mission.coalition.blue.country[1].ship.group) do --actualcode
    local index = 0
    if first then
        io.write("[WaterCoordinates]\n") 
        for _,value in pairs(group.route.points) do --actualcode
            io.write("Waypoint"..string.format("%04d", index).."="..value.x..","..value.y.."\n")
            index = index + 1
        end
        io.write("[WaterExclusionCoordinates]\n")
        first = false
    else
        for _,value in pairs(group.route.points) do --actualcode
            io.write("Islands"..string.format("%02d", groupIndex)..".Waypoint"..string.format("%04d", index).."="..value.x..","..value.y.."\n")
            index = index + 1
        end
        groupIndex = groupIndex + 1
    end

end




io.close(file)

