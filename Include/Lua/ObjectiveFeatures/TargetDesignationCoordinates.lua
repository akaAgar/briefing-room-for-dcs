briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationCoordinates = function()
  briefingRoom.radioManager.play("Pilot: Require update on target coordinates.", "RadioPilotTargetCoordinates")
  local objective = briefingRoom.mission.objectives[$OBJECTIVEINDEX$]
    
  local unit = dcsExtensions.getAliveUnitInGroup(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].groupID)
    
  if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID == 0 then -- no target units left
    briefingRoom.radioManager.play(objective.name.." JTAC: Negative, no visual on any target.", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end
    
  local unit = dcsExtensions.getUnitByID(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID[1])
  if unit == nil then -- no unit found with the ID, try searching for a static
    unit = dcsExtensions.getStaticByID(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID[1])
    if unit == nil then -- no unit nor static found with the ID
      briefingRoom.radioManager.play(objective.name.." JTAC: Negative, no visual on any target.", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
      return
    end
  end
    
  local unitVec3 = unit:getPoint()
  local unitVec2 = { x = unitVec3.x, y = unitVec3.z }
  local cooMessage = dcsExtensions.vec2ToStringCoordinates(unitVec2)
  briefingRoom.radioManager.play(objective.name.." JTAC: Affirm, transmitting updated target coordinates\n"..cooMessage, "RadioSupportTargetCoordinates", briefingRoom.radioManager.getAnswerDelay())
end
    
-- Add the command to the F10 menu
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Require target coordinates", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationCoordinates)
