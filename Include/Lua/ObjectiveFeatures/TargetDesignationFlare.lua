-- Number of flares available
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationFlareFlaresLeft = 5

-- Spawn flare
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationFlareDoFlare = function(args)
  trigger.action.signalFlare(args.position, trigger.flareColor.Yellow, 0)
end

-- "Signal position with flare" F10 radio command
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationFlare = function()
  briefingRoom.radioManager.play("$LANG_PILOT$: $LANG_FLAIRREQUEST$", "RadioPilotMarkSelfWithFlare")
  local objective = briefingRoom.mission.objectives[$OBJECTIVEINDEX$]
  local objectiveFeature = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$]

  if objectiveFeature.targetDesignationFlareFlaresLeft <= 0 then
    briefingRoom.radioManager.play(objective.name.." $LANG_JTAC$: $LANG_FLAIRNOFLAIRS$", "RadioSupportShootingFlareOut", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames == 0 then -- no target units left
    briefingRoom.radioManager.play(objective.name.." $LANG_JTAC$:$LANG_NOTARGET$", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  local unit = dcsExtensions.getUnitOrStatic(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
  if unit == nil then -- no unit nor static found with the ID
    briefingRoom.radioManager.play(objective.name.." $LANG_JTAC$:$LANG_NOTARGET$", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end
  
  objectiveFeature.targetDesignationFlareFlaresLeft = objectiveFeature.targetDesignationFlareFlaresLeft - 1

  local args = { ["position"] = unit:getPoint() }

  briefingRoom.radioManager.play(objective.name.." $LANG_JTAC$: $LANG_FLAIRAFFIRM$", "RadioSupportShootingFlare", briefingRoom.radioManager.getAnswerDelay(), objectiveFeature.targetDesignationFlareDoFlare, args)
end
      
-- Add the command to the F10 menu
missionCommands.addCommandForCoalition(briefingRoom.playerCoalition, "$LANG_FLAIRMENU$", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationFlare)
