-- Number of bombs available
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBombBombsLeft = 4

-- Spawn bomb
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBombDoBomb = function(args)
  args.position.y = args.position.y + 1250 + math.random(0, 500)
  trigger.action.illuminationBomb(args.position, 100000)
end

-- "Signal position with bomb" F10 radio command
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBomb = function()
  briefingRoom.radioManager.play("Pilot: Recon, can you drop an illumination bomb on the objective?", "RadioPilotDropIlluminationBomb")
  local objective = briefingRoom.mission.objectives[$OBJECTIVEINDEX$]

  if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID == 0 then -- no target units left
    briefingRoom.radioManager.play(objective.name.." Recon: Negative, no visual on any target.", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  local unit = dcsExtensions.getUnitByID(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID[1])
  if unit == nil then -- no unit found with the ID, try searching for a static
    unit = dcsExtensions.getStaticByID(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID[1])
    if unit == nil then -- no unit nor static found with the ID
      briefingRoom.radioManager.play(objective.name.." Recon: Negative, no visual on any target.", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
      return
    end
  end

  -- out of bombs
  if briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBombBombsLeft <= 0 then
    briefingRoom.radioManager.play(objective.name.." Recon: Negative, I'm Winchester. No bombs left.", "RadioSupportIlluminationBombOut", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBombBombsLeft = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBombBombsLeft - 1

  local args = { ["position"] = unit:getPoint() }

  briefingRoom.radioManager.play(objective.name.." Recon: Affirm, bomb away! (bomb(s) left: "..tostring(briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBombBombsLeft)..")", "RadioSupportIlluminationBomb", briefingRoom.radioManager.getAnswerDelay(), briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBombDoBomb, args)
end
      
-- Add the command to the F10 menu
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Drop illumination bomb", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationIlluminationBomb)
