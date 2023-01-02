table.insert(briefingRoom.mission.objectiveTriggers,  function(event)
    -- Mission complete, nothing to do
    if briefingRoom.mission.complete then return false end

    -- Objective complete, nothing to do
    if briefingRoom.mission.objectives[$OBJECTIVEINDEX$].complete then return false end
    local unitName = nil
    if event.id == world.event.S_EVENT_HIT then -- unit was hit but not destroyed, check anyway because destroying a parked aircraft in DCS is HARD, and any aircraft with less than 90% hp left is not airworthy
        if event.target == nil then return end -- no target (should never happen)
        if event.target:getCategory() ~= Object.Category.UNIT then return end -- target was not a unit
        local life = event.target:getLife() / event.target:getLife0()
        if life > .9 then return end -- not damaged enough
        unitName = event.target:getName()
    elseif event.id == world.event.S_EVENT_DEAD or event.id == world.event.S_EVENT_CRASH then -- unit destroyed
        if event.initiator == nil then return end -- no target (should never happen)
        if event.initiator:getCategory() ~= Object.Category.UNIT then return end -- target was not a unit
        unitName = event.initiator:getName()
    else return end

    if not table.contains(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames, unitName) then return false end

    -- Remove the unit from the list of targets
    table.removeValue(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames, unitName)
  
    -- Play "target destroyed" radio message
    local soundName = "TargetDestroyed"
    local messages = { "$LANG_COMMAND$: $LANG_TARGETDESTROY1$", "$LANG_COMMAND$: $LANG_TARGETDESTROY2$", "$LANG_COMMAND$: $LANG_TARGETSHOOTDOWN1$", "$LANG_COMMAND$: $LANG_TARGETSHOOTDOWN2" }
    local targetType = "Ground"
    local messageIndex = math.random(1, 2)
    local messageIndexOffset = 0
    if briefingRoom.eventHandler.BDASetting == "ALL" or briefingRoom.eventHandler.BDASetting == "TARGETONLY" then
        briefingRoom.radioManager.play(messages[messageIndex + messageIndexOffset], "RadioHQ"..soundName..targetType..tostring(messageIndex), math.random(1, 3))
    end

    -- Mark the objective as complete if all targets have been destroyed
    if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames < 1 then -- all target units destroyed, objective complete
        briefingRoom.mission.coreFunctions.completeObjective($OBJECTIVEINDEX$)
    else
        briefingRoom.aircraftActivator.possibleResponsiveSpawn()
    end

    return true
end)
