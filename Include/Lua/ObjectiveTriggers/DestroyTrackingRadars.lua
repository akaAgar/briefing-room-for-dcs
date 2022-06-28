-- Triggers the completion of objective $OBJECTIVEINDEX$ when all targets are destroyed
briefingRoom.mission.objectiveTriggers[$OBJECTIVEINDEX$] = function(event)
  -- Mission complete, nothing to do
  if briefingRoom.mission.complete then return false end

  -- Objective complete, nothing to do
  if briefingRoom.mission.objectives[$OBJECTIVEINDEX$].complete then return false end

  -- Check if event is a "destruction" event
  local destructionEvent = false
  if event.id == world.event.S_EVENT_DEAD or event.id == world.event.S_EVENT_CRASH then destructionEvent = true end
  -- "Landing" events are considered kills for helicopter targets
  if briefingRoom.mission.objectives[$OBJECTIVEINDEX$].targetCategory == Unit.Category.HELICOPTER and event.id == world.event.S_EVENT_LAND then destructionEvent = true end
  if not destructionEvent then return false end

  -- Initiator was nil
  if event.initiator == nil then return false end
  if Object.getCategory(event.initiator) ~= Object.Category.UNIT and Object.getCategory(event.initiator) ~= Object.Category.STATIC then return false end

  local unitID = tonumber(event.initiator:getID())
  -- Destroyed unit wasn't a target
  if not table.contains(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames, unitID) then return false end

  -- Remove the unit from the list of targets
  table.removeValue(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames, unitID)

  -- Play "target destroyed" radio message
  local soundName = "TargetDestroyed"
  local messages = { "Command: Target destroyed.", "Command: Good hit on target.", "Command: Target splashed.", "Command: Target shot down!" }
  local targetType = "Ground"
  local messageIndex = math.random(1, 2)
  local messageIndexOffset = 0
  if event.id == world.event.S_EVENT_CRASH and event.initiator:inAir() then
    targetType = "Air"
    messageIndexOffset = 2
  end
  briefingRoom.radioManager.play(messages[messageIndex + messageIndexOffset], "RadioHQ"..soundName..targetType..tostring(messageIndex), math.random(1, 3))

  -- Mark the objective as complete if all targets have been destroyed
  if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames < 1 then -- all target units destroyed, objective complete
    briefingRoom.mission.coreFunctions.completeObjective($OBJECTIVEINDEX$)
  else
    briefingRoom.aircraftActivator.possibleResponsiveSpawn()
  end

  return true
end

-- Remove all non-tracking radar units from the targets list (unless there's no tracking radar at all)
do
  local newTargetTable = { }
  
  for _,i in ipairs(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames) do
    local unit = Unit.getByName(i)
    if unit ~= nil and unit:hasAttribute("SAM TR") then
      table.insert(newTargetTable, i)
    end
  end

  if #newTargetTable > 0 then
    briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames = newTargetTable
    briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsCount = #newTargetTable
  end
end
