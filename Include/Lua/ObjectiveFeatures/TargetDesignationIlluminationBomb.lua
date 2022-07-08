-- Number of bombs available
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBombBombsLeft = 4

-- Spawn bomb
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBombDoBomb = function(args)
  args.position.y = args.position.y + 1250 + math.random(0, 500)
  trigger.action.illuminationBomb(args.position, 100000)
end

-- "Signal position with bomb" F10 radio command
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBomb = function()
  briefingRoom.radioManager.play("$LANGPILOT$: $LANGILLUMINATIONREQUEST$", "RadioPilotDropIlluminationBomb")
  local objective = briefingRoom.mission.objectives[$OBJECTIVEINDEX$]

  if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames == 0 then -- no target units left
    briefingRoom.radioManager.play(objective.name.." $LANGRECON$:$LANGNOTARGET$", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  local unit = Unit.getByName(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
  if unit == nil then -- no unit found with the ID, try searching for a static
    unit = StaticObject.getByName(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
    if unit == nil then -- no unit nor static found with the ID
      briefingRoom.radioManager.play(objective.name.." $LANGRECON$:$LANGNOTARGET$", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
      return
    end
  end

  -- out of bombs
  if briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBombBombsLeft <= 0 then
    briefingRoom.radioManager.play(objective.name.." $LANGRECON$: $LANGILLUMINATIONREJECT$", "RadioSupportIlluminationBombOut", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBombBombsLeft = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBombBombsLeft - 1

  local args = { ["position"] = unit:getPoint() }

  briefingRoom.radioManager.play(objective.name.." $LANGRECON$: $LANGILLUMINATIONAFFIRM$", "RadioSupportIlluminationBomb", briefingRoom.radioManager.getAnswerDelay(), briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBombDoBomb, args)
end
      
-- Add the command to the F10 menu
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Drop illumination bomb", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBomb)
