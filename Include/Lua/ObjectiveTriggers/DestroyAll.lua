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

  local unitName = event.initiator:getName()
  -- Destroyed unit wasn't a target
  if not table.contains(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames, unitName) then return false end

  -- Remove the unit from the list of targets
  table.removeValue(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames, unitName)

  -- Play "target destroyed" radio message
  local soundName = "TargetDestroyed"
  local messages = { "$LANG_COMMAND$: $LANG_TARGETDESTROY1$", "$LANG_COMMAND$: $LANG_TARGETDESTROY2$", "$LANG_COMMAND$: $LANG_TARGETSHOOTDOWN1$", "$LANG_COMMAND$: $LANG_TARGETSHOOTDOWN2$" }
  local targetType = "Ground"
  local messageIndex = math.random(1, 2)
  local messageIndexOffset = 0
  if event.id == world.event.S_EVENT_CRASH and event.initiator:inAir() then
    targetType = "Air"
    messageIndexOffset = 2
  end
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
end
