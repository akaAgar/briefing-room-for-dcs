briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationCoordinates = function()
  briefingRoom.radioManager.play("$LANG_PILOT$: $LANG_TARGETCOORDSREQUEST$", "RadioPilotTargetCoordinates")
  local objective = briefingRoom.mission.objectives[$OBJECTIVEINDEX$]
    
  if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames == 0 then -- no target units left
    briefingRoom.radioManager.play(objective.name.." $LANG_JTAC$: $LANG_NOTARGET$", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end
    
  local unit = dcsExtensions.getUnitOrStatic(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
  if unit == nil then -- no unit nor static found with the ID
    briefingRoom.radioManager.play(objective.name.." $LANG_JTAC$: $LANG_NOTARGET$", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end
    
  local unitVec3 = unit:getPoint()
  local unitVec2 = { x = unitVec3.x, y = unitVec3.z }
  local cooMessage = dcsExtensions.vec2ToStringCoordinates(unitVec2)
  briefingRoom.radioManager.play(objective.name.." $LANG_JTAC$: $LANG_TARGETCOORDSAFFIRM$\n"..cooMessage, "RadioSupportTargetCoordinates", briefingRoom.radioManager.getAnswerDelay())

  local idx = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].objRadioCommandIndex

  missionCommands.removeItemForCoalition(briefingRoom.playerCoalition, briefingRoom.mission.objectives[$OBJECTIVEINDEX$].f10Commands[idx].commandPath)
  briefingRoom.mission.objectives[$OBJECTIVEINDEX$].f10Commands[idx].commandPath = missionCommands.addCommandForCoalition(briefingRoom.playerCoalition, "$LANG_TARGETCOORDSMENU$.\n$LANG_TARGETCOORDSMENULAST$:\n"..cooMessage, briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationCoordinates)
end
    
-- Add the command to the F10 menu

table.insert(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].f10Commands, {text = "$LANG_TARGETCOORDSMENU$", func = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationCoordinates, args =  nil})
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].objRadioCommandIndex = #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].f10Commands