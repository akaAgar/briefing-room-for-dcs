require "mission" -- Mission lua file

function __genOrderedIndex(t)
    local orderedIndex = {}
    for key in pairs(t) do
        table.insert(orderedIndex, key)
    end
    table.sort(orderedIndex)
    return orderedIndex
end

function orderedNext(t, state)
    -- Equivalent of the next function, but returns the keys in the alphabetic
    -- order. We use a temporary ordered key table that is stored in the
    -- table being iterated.

    local key = nil
    --print("orderedNext: state = "..tostring(state) )
    if state == nil then
        -- the first time, generate the index
        t.__orderedIndex = __genOrderedIndex(t)
        key = t.__orderedIndex[1]
    else
        -- fetch the next value
        for i = 1, table.getn(t.__orderedIndex) do
            if t.__orderedIndex[i] == state then
                key = t.__orderedIndex[i + 1]
            end
        end
    end

    if key then
        return key, t[key]
    end

    -- no more value to return, cleanup
    t.__orderedIndex = nil
    return
end

function orderedPairs(t)
    -- Equivalent of the pairs() function on tables. Allows to iterate
    -- in order
    return orderedNext, t, nil
end

file = io.open("Zones.out", "w")
io.output(file)

-- WATER
local first = true
local groupIndex = 1;
for _, group in orderedPairs(mission.drawings.layers[3].objects) do -- Neutral Markers
    if first then
        io.write("\n[WaterCoordinates]\n")
        local originX = group.mapX
        local originY = group.mapY
        io.write("Waypoint" .. string.format("%04d", 0) .. "=" .. originX .. "," .. originY .. "\n")
        for index, value in orderedPairs(group.points) do --actualcode
            io.write("Waypoint" .. string.format("%04d", index) ..
                "=" .. (originX + value.x) .. "," .. (originY + value.y) .. "\n")
        end
        io.write("\n[WaterExclusionCoordinates]\n")
        first = false
    else
        local originX = group.mapX
        local originY = group.mapY
        io.write("Islands" ..
            string.format("%02d", groupIndex) .. ".Waypoint" ..
            string.format("%04d", 0) .. "=" .. originX .. "," .. originY .. "\n")
        for index, value in orderedPairs(group.points) do --actualcode
            io.write("Islands" ..
                string.format("%02d", groupIndex) ..
                ".Waypoint" .. string.format("%04d", index) .. "=" .. (originX + value.x) ..
                "," .. (originY + value.y) .. "\n")
        end
        groupIndex = groupIndex + 1
    end
end


-- Areas of Influence
io.write("\n[RedCoordinates]\n") -- RED Markers
local originX = mission.drawings.layers[1].objects[1].mapX
local originY = mission.drawings.layers[1].objects[1].mapY
io.write("Waypoint" .. string.format("%04d", 0) .. "=" .. originX .. "," .. originY .. "\n")
for key, value in orderedPairs(mission.drawings.layers[1].objects[1].points) do --actualcode
    io.write("Waypoint" .. string.format("%04d", key) .. "=" .. (originX + value.x) .. "," .. (originY + value.y) .. "\n")
end
io.write("\n[BlueCoordinates]\n") -- BLUE Markers
local originX = mission.drawings.layers[2].objects[1].mapX
local originY = mission.drawings.layers[2].objects[1].mapY
io.write("Waypoint" .. string.format("%04d", 0) .. "=" .. originX .. "," .. originY .. "\n")
for key, value in orderedPairs(mission.drawings.layers[2].objects[1].points) do --actualcode
    io.write("Waypoint" .. string.format("%04d", key) .. "=" .. (originX + value.x) .. "," .. (originY + value.y) .. "\n")
end

io.write("\n[NoSpawnCoordinates]\n") -- Common Markers
local originX = mission.drawings.layers[4].objects[1].mapX
local originY = mission.drawings.layers[4].objects[1].mapY
io.write("Waypoint" .. string.format("%04d", 0) .. "=" .. originX .. "," .. originY .. "\n")
for key, value in orderedPairs(mission.drawings.layers[4].objects[1].points) do --actualcode
    io.write("Waypoint" .. string.format("%04d", key) .. "=" .. (originX + value.x) .. "," .. (originY + value.y) .. "\n")
end


io.close(file)
