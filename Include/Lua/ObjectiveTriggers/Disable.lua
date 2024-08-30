table.insert(briefingRoom.mission.objectiveTriggers, function(event)
    local objectiveIndex = $OBJECTIVEINDEX$
    -- Mission complete, nothing to do
    if briefingRoom.mission.complete then return false end

    -- Objective complete, nothing to do
    if briefingRoom.mission.objectives[objectiveIndex].complete then return false end
    local killedUnit = event.initiator
    if event.id == world.event.S_EVENT_KILL then
        if event.target == nil then return end
        killedUnit = event.target
    elseif  event.id == world.event.S_EVENT_HIT then -- unit was hit but not destroyed, check anyway because destroying a parked aircraft in DCS is HARD, and any aircraft with less than 90% hp left is not airworthy
        if event.target == nil then return end
        if event.target:getCategory() ~= Object.Category.UNIT then return end -- target was not a unit
        local life = event.target:getLife() / event.target:getLife0()
        if life > .9 then return end -- not damaged enough
        killedUnit = event.target
    elseif briefingRoom.mission.isSoftKillEvent(event.id) then -- unit destroyed
        if event.initiator == nil then return end
    else return end
    return  briefingRoom.mission.destroyCallout(objectiveIndex, killedUnit, event.id)
end)
