require "mission" -- Mission lua file
json = require "json"


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


file = io.open ("SpawnPoints.json", "w")
io.output(file)

-- Waypoints

local index = 1
local ouput = {}
for _,group in orderedPairs(mission.coalition.red.country[1].vehicle.group) do --actualcode
    for _,point in orderedPairs(group.route.points) do --actualcode
        ouput[index] = {coords = {point.x,point.y}, BRtype ="LandSmall"}
        index = index + 1
    end
end

for _,group in orderedPairs(mission.coalition.neutrals.country[1].vehicle.group) do --actualcode
    for _,point in orderedPairs(group.route.points) do --actualcode
        ouput[index] = {coords = {point.x,point.y}, BRtype ="LandMedium"}
        index = index + 1
    end
end

for _,group in orderedPairs(mission.coalition.blue.country[1].vehicle.group) do --actualcode
    for _,point in orderedPairs(group.route.points) do --actualcode
        ouput[index] = {coords = {point.x,point.y}, BRtype ="LandLarge"}
        index = index + 1
    end
end

io.write(json.encode(ouput))

io.close(file)

