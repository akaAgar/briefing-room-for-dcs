briefingRoom.mission.features.targetGetCoordinates = { }

function briefingRoom.mission.features.targetGetCoordinates.getCoordinates(index)
  local unit = dcsExtensions.getAliveUnitInGroup(briefingRoom.mission.objectives[index].groupID)
  
  if unit == nil then
    briefingRoom.radioManager.play("Negative, no visual on any target", "RadioSupportNoTarget")
  else
    local unitVec3 = unit:getPoint()
    local unitVec2 = { x = unitVec3.x, y = unitVec3.z }
    local cooMessage = dcsExtensions.vec2ToStringCoordinates(unitVec2)
    briefingRoom.radioManager.play("Affirm, transmitting target coordinates\n"..cooMessage, "RadioSupportCoordinates")
  end
end
  
-- Create F10 menu options
do
  for i,o in ipairs(briefingRoom.mission.objectives) do
    missionCommands.addCommandForCoalition($PLAYERCOALITION$, "Require target coordinates", briefingRoom.f10Menu.objectives[i], briefingRoom.mission.features.targetGetCoordinates.getCoordinates, { index = i })
  end
end
