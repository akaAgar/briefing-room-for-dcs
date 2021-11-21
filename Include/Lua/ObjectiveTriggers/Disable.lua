briefingRoom.mission.objectiveTriggers[$OBJECTIVEINDEX$] = function(event)
    -- Mission complete, nothing to do
    if briefingRoom.mission.complete then return false end

    -- Objective complete, nothing to do
    if briefingRoom.mission.objectives[$OBJECTIVEINDEX$].complete then return false end
    local unitID = nil
    if event.id == world.event.S_EVENT_HIT then -- unit was hit but not destroyed, check anyway because destroying a parked aircraft in DCS is HARD, and any aircraft with less than 90% hp left is not airworthy
        if event.target == nil then return end -- no target (should never happen)
        if event.target:getCategory() ~= Object.Category.UNIT then return end -- target was not a unit
        local life = event.target:getLife() / event.target:getLife0()
        if life > .9 then return end -- not damaged enough
        unitID = tonumber(event.target:getID())
    elseif event.id == world.event.S_EVENT_DEAD or event.id == world.event.S_EVENT_CRASH then -- unit destroyed
        if event.initiator == nil then return end -- no target (should never happen)
        if event.initiator:getCategory() ~= Object.Category.UNIT then return end -- target was not a unit
        unitID = tonumber(event.initiator:getID())
    else return end

    if not table.contains(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID, unitID) then return false end

    -- Remove the unit from the list of targets
    table.removeValue(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID, unitID)
  
    -- Play "target destroyed" radio message
    local soundName = "TargetDestroyed"
    local messages = { "Target destroyed.", "Good hit on target.", "Target splashed.", "Target shot down!" }
    local targetType = "Ground"
    local messageIndex = math.random(1, 2)
    local messageIndexOffset = 0
    briefingRoom.radioManager.play(messages[messageIndex + messageIndexOffset], "RadioHQ"..soundName..targetType..tostring(messageIndex), math.random(1, 3))

    -- Mark the objective as complete if all targets have been destroyed
    if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID < 1 then -- all target units destroyed, objective complete
      briefingRoom.mission.coreFunctions.completeObjective($OBJECTIVEINDEX$)
    end

    return true
end
