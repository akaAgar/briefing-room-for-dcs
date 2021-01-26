briefingRoom.mission.features.supportSmokeMarker = { }
briefingRoom.mission.features.supportSmokeMarker.SMOKE_DURATION = 300 -- smoke markers last for 5 minutes (300 seconds) in DCS World
briefingRoom.mission.features.supportSmokeMarker.nextSmoke = { } -- time when next smoke marker will be available for each objective

-- Spawn smoke marker
function briefingRoom.mission.features.supportSmokeMarker.doSmoke(args)
  local smokeColor = briefingRoom.mission.parameters.targetSmokeMarkerColor or trigger.smokeColor.Red

  trigger.action.smoke(args.position, smokeColor)
  return nil
end

function briefingRoom.mission.features.supportSmokeMarker.markWithSmoke(index)
  briefingRoom.radioManager.play("I have no visual. Pop a smoke grenade on target.", "RadioPilotMarkTargetWithSmoke")

  if #briefingRoom.mission.objectives[index].unitsID == 0 then -- no target units left
    briefingRoom.radioManager.play("Negative, no visual on any target.", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  local unit = dcsExtensions.getUnitByID(briefingRoom.mission.objectives[index].unitsID[1])
  if unit == nil then -- no unit found with the ID, try searching for a static
    unit = dcsExtensions.getStaticByID(briefingRoom.mission.objectives[index].unitsID[1])
    if unit == nil then -- no unit nor static found with the ID
      briefingRoom.radioManager.play("Negative, no visual on any target.", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
      return
    end
  end

  -- Set the next smoke time to an arbitrary negative value if this JTAC was never used before
  if briefingRoom.mission.features.supportSmokeMarker.nextSmoke[index] == nil then
    briefingRoom.mission.features.supportSmokeMarker.nextSmoke[index] = -999999
  end

  local timeNow = timer.getAbsTime()

  if timeNow < briefingRoom.mission.features.supportSmokeMarker.nextSmoke[index] then -- Smoke not ready
    briefingRoom.radioManager.play("Target is already marked with smoke. Check your position, you should see it.", "RadioSupportTargetAlreadyMarkedWithSmoke", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  briefingRoom.mission.features.supportSmokeMarker.nextSmoke[index] = timeNow + briefingRoom.mission.features.supportSmokeMarker.SMOKE_DURATION -- set time for next smoke

  local args = { ["position"] = unit:getPoint() }
  briefingRoom.radioManager.play("Affirm, target marked with smoke.", "RadioSupportTargetMarkedWithSmoke", briefingRoom.radioManager.getAnswerDelay(), briefingRoom.mission.features.supportSmokeMarker.doSmoke, args)
end

-- Create F10 menu options
do
  for i,o in ipairs(briefingRoom.mission.objectives) do
    missionCommands.addCommandForCoalition($PLAYERCOALITION$, "Mark target with smoke", briefingRoom.f10Menu.objectives[i], briefingRoom.mission.features.supportSmokeMarker.markWithSmoke, i)
  end
end
