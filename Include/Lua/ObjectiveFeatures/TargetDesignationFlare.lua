-- Number of flares available
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationFlareFlaresLeft = 5

-- Spawn flare
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationFlareDoFlare = function(args)
  trigger.action.signalFlare(args.position, trigger.flareColor.Yellow, 0)
  return nil
end

-- "Signal position with flare" F10 radio command
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationFlare = function()
  briefingRoom.radioManager.play("I have no visual on you. Can you shoot a flare?", "RadioPilotMarkSelfWithFlare")

  if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID == 0 then -- no target units left
    return
  end

  local unit = dcsExtensions.getUnitByID(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID[1])
  if unit == nil then -- no unit found with the ID, try searching for a static
    unit = dcsExtensions.getStaticByID(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID[1])
    if unit == nil then -- no unit nor static found with the ID
      return
    end
  end

  -- out of flares
  if briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationFlareFlaresLeft <= 0 then
    briefingRoom.radioManager.play("Negative, I'm out of flares.", "RadioSupportShootingFlareOut", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationFlareFlaresLeft = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationFlareFlaresLeft - 1

  local args = { ["position"] = unit:getPoint() }
  briefingRoom.radioManager.play("Affirm, shooting a flare now (flare(s) left: "..tostring(briefingRoom.mission.features.supportLaunchFlare.flaresLeft[index])..")", "RadioSupportShootingFlare", briefingRoom.radioManager.getAnswerDelay(), briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationFlareDoFlare, args)
end
      
-- Add the command to the F10 menu
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Launch a flare", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationFlare)
