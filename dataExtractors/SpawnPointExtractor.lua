require "mission" -- Mission lua file

function __genOrderedIndex( t )
    local orderedIndex = {}
    for key in pairs(t) do
        table.insert( orderedIndex, key )
    end
    table.sort( orderedIndex )
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
        t.__orderedIndex = __genOrderedIndex( t )
        key = t.__orderedIndex[1]
    else
        -- fetch the next value
        for i = 1,table.getn(t.__orderedIndex) do
            if t.__orderedIndex[i] == state then
                key = t.__orderedIndex[i+1]
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


file = io.open ("SpawnPoints.out", "w")
io.output(file)

-- Waypoints
io.write("\n[SpawnPoints]\n")
local index = 0
for _,value in orderedPairs(mission.coalition.red.country[1].vehicle.group[1].route.points) do --actualcode
    io.write("Point"..string.format("%04d", index).."="..value.x..","..value.y..",LandSmall\n")
    index = index + 1
end

for _,value in orderedPairs(mission.coalition.neutrals.country[1].vehicle.group[1].route.points) do --actualcode
    io.write("Point"..string.format("%04d", index).."="..value.x..","..value.y..",LandMedium\n")
    index = index + 1
end

for _,value in orderedPairs(mission.coalition.blue.country[1].vehicle.group[1].route.points) do --actualcode
    io.write("Point"..string.format("%04d", index).."="..value.x..","..value.y..",LandLarge\n")
    index = index + 1
end


io.close(file)

