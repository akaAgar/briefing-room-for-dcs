-- Time (in seconds) until next smoke is available
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationSmokeMarkerNextSmoke = -999999

-- Spawn smoke marker
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationSmokeMarkerDoSmoke = function(args)
  trigger.action.smoke(args.position, args.color)
  return nil
end

-- "Mark target with smoke" F10 radio command
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationSmokeMarker = function()
  local objective = briefingRoom.mission.objectives[$OBJECTIVEINDEX$]
  briefingRoom.radioManager.play("$LANG_PILOT$: $LANG_SMOKEREQUEST$", "RadioPilotMarkTargetWithSmoke")

  if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames == 0 then -- no target units left
    briefingRoom.radioManager.play(objective.name.." $LANG_JTAC$:$LANG_NOTARGET$", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end
  
  local unit = dcsExtensions.getUnitOrStatic(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
  if unit == nil then -- no unit nor static found with the ID
      briefingRoom.radioManager.play(objective.name.." $LANG_JTAC$:$LANG_NOTARGET$", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
      return
  end
  
  local timeNow = timer.getAbsTime()
  
  if timeNow < briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationSmokeMarkerNextSmoke then -- Smoke not ready
    briefingRoom.radioManager.play(objective.name.." $LANG_JTAC$: $LANG_SMOKEALREADY$", "RadioSupportTargetAlreadyMarkedWithSmoke", briefingRoom.radioManager.getAnswerDelay())
    return
  end
  
  -- Set cooldown for next smoke
  briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationSmokeMarkerNextSmoke = timeNow + SMOKE_DURATION
  
  -- Play radio message and setup smoke creating function
  local args = { position = unit:getPoint(), color = trigger.smokeColor.Red }
  if unit:getCoalition() == briefingRoom.playerCoalition then args.color = trigger.smokeColor.Green end
  briefingRoom.radioManager.play(objective.name.." $LANG_JTAC$: $LANG_SMOKEAFFIRM$", "RadioSupportTargetMarkedWithSmoke", briefingRoom.radioManager.getAnswerDelay(), briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationSmokeMarkerDoSmoke, args)
end
      
-- Add the command to the F10 menu
missionCommands.addCommandForCoalition(briefingRoom.playerCoalition, "$LANG_SMOKEMENU$", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationSmokeMarker)
