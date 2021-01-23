briefingRoom.mission.features.objectiveDestroyParkedAircraft = {}

function briefingRoom.mission.features.objectiveDestroyParkedAircraft:onEvent(event)
  if briefingRoom.mission.status ~= brMissionStatus.IN_PROGRESS then return end -- mission already complete/failed, nothing to do
  if event.id ~= world.event.S_EVENT_HIT then return end -- only "hit" events interest us
  if event.target == nil then return end -- no target (should never happen)
  if event.target:getCategory() ~= Object.Category.UNIT then return end -- target was not a unit
  local unitID = event.target:getID()

  for index,objective in ipairs(briefingRoom.mission.objectives) do
    if objective.status == brMissionStatus.IN_PROGRESS then
      if table.contains(briefingRoom.mission.objectives[index].unitsID, unitID) then
        local life = event.target:getLife() / event.target:getLife0()

        if life < .9 then
          table.removeValue(briefingRoom.mission.objectives[index].unitsID, unitID)
          local messageIndex = math.random(1, 2)
          messages = { "Target destroyed.", "Good hit on target." }
          briefingRoom.radioManager.play(messages[messageIndex], "RadioHQTargetDestroyedGround"..tostring(messageIndex), math.random(1, 3))

          if #briefingRoom.mission.objectives[index].unitsID < 1 then
            briefingRoom.mission.functions.completeObjective(index)
          end
        end
      end
    end
  end
end

world.addEventHandler(briefingRoom.mission.features.objectiveDestroyParkedAircraft)
