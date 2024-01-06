-- Number of bombs available
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBombBombsLeft = 4

-- Spawn bomb
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBombDoBomb = function(args)
  args.position.y = args.position.y + 1250 + math.random(0, 500)
  trigger.action.illuminationBomb(args.position, 100000)
end

-- "Signal position with bomb" F10 radio command
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBomb = function()
  briefingRoom.radioManager.play("$LANG_PILOT$: $LANG_ILLUMINATIONREQUEST$", "RadioPilotDropIlluminationBomb")
  local objective = briefingRoom.mission.objectives[$OBJECTIVEINDEX$]
  local objectiveFeature = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$]
  -- out of bombs
  if objectiveFeature.targetDesignationIlluminationBombBombsLeft <= 0 then
    briefingRoom.radioManager.play(objective.name.." $LANG_RECON$: $LANG_ILLUMINATIONREJECT$", "RadioSupportIlluminationBombOut", briefingRoom.radioManager.getAnswerDelay())
    return
  end
  
  if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames == 0 then -- no target units left
    briefingRoom.radioManager.play(objective.name.." $LANG_RECON$:$LANG_NOTARGET$", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  local unit = dcsExtensions.getUnitOrStatic(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
  if unit == nil then -- no unit nor static found with the ID
    briefingRoom.radioManager.play(objective.name.." $LANG_RECON$:$LANG_NOTARGET$", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  objectiveFeature.targetDesignationIlluminationBombBombsLeft = objectiveFeature.targetDesignationIlluminationBombBombsLeft - 1

  local args = { ["position"] = unit:getPoint() }

  briefingRoom.radioManager.play(objective.name.." $LANG_RECON$: $LANG_ILLUMINATIONAFFIRM$", "RadioSupportIlluminationBomb", briefingRoom.radioManager.getAnswerDelay(), objectiveFeature.targetDesignationIlluminationBombDoBomb, args)
end
      
-- Add the command to the F10 menu
table.insert(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].f10Commands, {text = "$LANG_ILLUMINATIONMENU$", func = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBomb, args =  nil})
