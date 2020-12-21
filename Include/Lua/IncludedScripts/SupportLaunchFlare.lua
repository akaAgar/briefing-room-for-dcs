briefingRoom.mission.features.supportLaunchFlare = { }
briefingRoom.mission.features.supportLaunchFlare.FLARES_COUNT = 5
briefingRoom.mission.features.supportLaunchFlare.flaresLeft = { }

-- Spawn flare
function briefingRoom.mission.features.supportLaunchFlare.doFlare(args)
  trigger.action.signalFlare(args.position, trigger.flareColor.Yellow, 0)
  return nil
end

function briefingRoom.mission.features.supportLaunchFlare.launchFlare(index)
  briefingRoom.radioManager.play("I have no visual on you. Can you shoot a flare?", "RadioPilotMarkSelfWithFlare")

  if #briefingRoom.mission.objectives[index].unitsID == 0 then -- no target units left
    return
  end

  local unit = dcsExtensions.getUnitByID(briefingRoom.mission.objectives[index].unitsID[1])
  if unit == nil then -- no unit found with the ID
    return
  end

  if briefingRoom.mission.features.supportLaunchFlare.flaresLeft[index] == nil then
    briefingRoom.mission.features.supportLaunchFlare.flaresLeft[index] = briefingRoom.mission.features.supportLaunchFlare.FLARES_COUNT
  elseif briefingRoom.mission.features.supportLaunchFlare.flaresLeft[index] <= 0 then
    briefingRoom.radioManager.play("Negative, I'm out of flares.", "RadioSupportShootingFlareOut", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  briefingRoom.mission.features.supportLaunchFlare.flaresLeft[index] = briefingRoom.mission.features.supportLaunchFlare.flaresLeft[index] - 1

  local args = { ["position"] = unit:getPoint() }
  briefingRoom.radioManager.play("Affirm, shooting a flare now. Flare left: "..tostring(briefingRoom.mission.features.supportLaunchFlare.flaresLeft[index]), "RadioSupportShootingFlare", briefingRoom.radioManager.getAnswerDelay(), briefingRoom.mission.features.supportLaunchFlare.doFlare, args)
end

-- Create F10 menu options
do
  for i,o in ipairs(briefingRoom.mission.objectives) do
    missionCommands.addCommandForCoalition($PLAYERCOALITION$, "Launch a distress flare", briefingRoom.f10Menu.objectives[i], briefingRoom.mission.features.supportLaunchFlare.launchFlare, i)
  end
end

