briefingRoom.mission.features.supportTargetCoordinates = { }

function briefingRoom.mission.features.supportTargetCoordinates.getCoordinates(index)
  briefingRoom.radioManager.play("Required update on target coordinates.", "RadioPilotTargetCoordinates")

  local unit = dcsExtensions.getAliveUnitInGroup(briefingRoom.mission.objectives[index].groupID)
  
  if #briefingRoom.mission.objectives[index].unitsID == 0 then -- no target units left
    briefingRoom.radioManager.play("Negative, no visual on any target.", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  local unit = dcsExtensions.getUnitByID(briefingRoom.mission.objectives[index].unitsID[1])
  if unit == nil then -- no unit found with the ID
    briefingRoom.radioManager.play("Negative, no visual on any target.", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  local unitVec3 = unit:getPoint()
  local unitVec2 = { x = unitVec3.x, y = unitVec3.z }
  local cooMessage = dcsExtensions.vec2ToStringCoordinates(unitVec2)
  briefingRoom.radioManager.play("Affirm, transmitting updated target coordinates\n"..cooMessage, "RadioSupportTargetCoordinates", briefingRoom.radioManager.getAnswerDelay())
end
  
-- Create F10 menu commands
do
  for i,o in ipairs(briefingRoom.mission.objectives) do
    missionCommands.addCommandForCoalition($PLAYERCOALITION$, "Require target coordinates", briefingRoom.f10Menu.objectives[i], briefingRoom.mission.features.supportTargetCoordinates.getCoordinates, i)
  end
end
