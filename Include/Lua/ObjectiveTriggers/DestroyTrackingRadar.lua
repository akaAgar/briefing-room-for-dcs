-- Triggers the completion of objective $OBJECTIVEINDEX$ when a tracking radar has been destroyed
briefingRoom.mission.objectiveTriggers[$OBJECTIVEINDEX$] = function(event)
    -- Mission complete, nothing to do
    if briefingRoom.mission.complete then return false end
  
    -- Objective complete, nothing to do
    if briefingRoom.mission.objectives[$OBJECTIVEINDEX$].complete then return false end
  
    -- Check if event is a "destruction" event
    if event.id ~= world.event.S_EVENT_DEAD then return false end
  
    -- Initiator was nil
    if event.initiator == nil then return false end
    if Object.getCategory(event.initiator) ~= Object.Category.UNIT then return false end
  
    local unitID = tonumber(event.initiator:getID())
    -- Destroyed unit wasn't a target
    if not table.contains(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID, unitID) then return false end
    local unit = dcsExtensions.getUnitByID(unitID)
    if unit == nil then return false end
    local unitDescription = unit:getDesc()
    if unitDescription.attributes["SAM TR"] ~= true then return false end -- Not a tracking radar

    -- Play "target destroyed" radio message
    local soundName = "TargetDestroyed"
    local messages = { "Target destroyed.", "Good hit on target." }
    briefingRoom.radioManager.play(messages[math.random(1, 2)], "RadioHQ"..soundName.."Ground"..tostring(messageIndex), math.random(1, 3))
  
    briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID = { }
    briefingRoom.mission.coreFunctions.completeObjective($OBJECTIVEINDEX$)
  
    return true
  end
  