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




-- Areas of Influence
local ids = "DCSID="
local coordinates = "Offset.Coordinates="
local headings = "Offset.Heading="
local shape = "Shape="
local originX = mission.coalition.red.country[1].static.group[1].x
local originY = mission.coalition.red.country[1].static.group[1].y
for _,value in orderedPairs(mission.coalition.red.country[1].static.group) do --actualcode
    ids = ids..value.units[1].type..","
    coordinates = coordinates..(originX - value.units[1].x)..","..(originY - value.units[1].y)..";"
    headings = headings..value.units[1].heading..","
    if value.units[1].shape_name == nil  then
        shape = shape..","
    else
        shape = shape..value.units[1].shape_name..","
    end
end

print(ids)
print(coordinates)
print(headings)
print(shape)


