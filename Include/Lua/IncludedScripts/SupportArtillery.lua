briefingRoom.mission.features.artillery = { }
briefingRoom.mission.features.artillery.FIRE_MISSIONS_PER_OBJECTIVE = 3
briefingRoom.mission.features.artillery.INACCURACY = 500 -- in meters
briefingRoom.mission.features.artillery.MARKER_NAME = "artillery"
briefingRoom.mission.features.artillery.SHELLS_PER_FIRE_MISSION = 10
briefingRoom.mission.features.artillery.fireMissionsLeft = 0
briefingRoom.mission.features.artillery.markID = nil -- ID of the mark on the map
briefingRoom.mission.features.artillery.shellsLeftInFireMission = 0

function briefingRoom.mission.features.artillery:onEvent(event)
  if event.id == world.event.S_EVENT_MARK_REMOVE then
    if briefingRoom.mission.features.artillery.markID ~= nil and event.idx == briefingRoom.mission.features.artillery.markID then
      briefingRoom.radioManager.play("Affirm, coordinates discarded. Awaiting new coordinates", "RadioCoordinatesDiscardedM")
      briefingRoom.mission.features.artillery.markID = nil
    end
  elseif event.id == world.event.S_EVENT_MARK_ADDED then
    local markText = string.lower(tostring(event.text or ""))
    if markText == briefingRoom.mission.features.artillery.MARKER_NAME then
      if briefingRoom.mission.features.artillery.markID ~= nil then
        trigger.action.removeMark(briefingRoom.mission.features.artillery.markID)
      end
      briefingRoom.mission.features.artillery.markID = event.idx
      briefingRoom.radioManager.play("Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      return
    end
  elseif event.id == world.event.S_EVENT_MARK_CHANGE then
    if briefingRoom.mission.features.artillery.markID ~= nil and event.idx == briefingRoom.mission.features.artillery.markID then
      local markText = string.lower(tostring(event.text or ""))
      if markText ~= briefingRoom.mission.features.artillery.MARKER_NAME then
        briefingRoom.radioManager.play("Affirm, coordinates discarded. Awaiting new coordinates", "RadioCoordinatesDiscardedM")
        briefingRoom.mission.features.artillery.markID = nil
      end
    end
  end
end

function briefingRoom.mission.features.artillery.doShell(args, time)
  briefingRoom.mission.features.artillery.shellsLeftInFireMission = briefingRoom.mission.features.artillery.shellsLeftInFireMission - 1
  
  for i=1,3 do
    local impactPoint = args.position
    local inaccuracy = math.randomPointInCircle({ ["x"] = 0, ["y"] = 0 }, briefingRoom.mission.features.artillery.INACCURACY)
    impactPoint.x = impactPoint.x + inaccuracy.x
    impactPoint.z = impactPoint.z + inaccuracy.y
    trigger.action.explosion(impactPoint, 100)
  end

  if briefingRoom.mission.features.artillery.shellsLeftInFireMission <= 0 then -- no shells life in this fire mission
    return nil
  else
    return time + 1
  end
end

function briefingRoom.mission.features.artillery.doFireMission(args)
  briefingRoom.mission.features.artillery.fireMissionsLeft = briefingRoom.mission.features.artillery.fireMissionsLeft - 1
  briefingRoom.mission.features.artillery.shellsLeftInFireMission = briefingRoom.mission.features.artillery.SHELLS_PER_FIRE_MISSION
  timer.scheduleFunction(briefingRoom.mission.features.artillery.doShell, args, timer.getTime() + math.random(2,3))
end

function briefingRoom.mission.features.artillery.launchFireMission()
  briefingRoom.radioManager.play("Fire support, begin fire mission on provided coordinates.", "RadioPilotArtillery")
 
  if briefingRoom.mission.features.artillery.fireMissionsLeft <= 0 then
    briefingRoom.radioManager.play("Negative, no fire mission available.", "RadioArtilleryNoAmmo", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    local markText = string.lower(tostring(m.text or ""))
    if markText == briefingRoom.mission.features.artillery.MARKER_NAME then
      local args = { ["position"] = m.pos }
      briefingRoom.radioManager.play("Copy, firing for effect on provided coordinates.", "RadioArtilleryFiring", briefingRoom.radioManager.getAnswerDelay(), briefingRoom.mission.features.artillery.doFireMission, args)
      return
    end
  end

  briefingRoom.radioManager.play("Cannot comply. No coordinates provided for fire mission.\n(add a marker named \""..string.upper(briefingRoom.mission.features.artillery.MARKER_NAME).."\" on the F10 to designate a target)", "RadioArtilleryNoCoordinates", briefingRoom.radioManager.getAnswerDelay())
end

world.addEventHandler(briefingRoom.mission.features.artillery)

-- set the correct number of fire missions
briefingRoom.mission.features.artillery.fireMissionsLeft = briefingRoom.mission.features.artillery.FIRE_MISSIONS_PER_OBJECTIVE * #briefingRoom.mission.objectives

if briefingRoom.f10Menu.supportMenu == nil then
  briefingRoom.f10Menu.supportMenu = missionCommands.addSubMenuForCoalition($PLAYERCOALITION$, "Support", nil)
end
missionCommands.addCommandForCoalition($PLAYERCOALITION$, "Begin fire mission on provided coordinates", briefingRoom.f10Menu.supportMenu, briefingRoom.mission.features.supportLaserDesignation.turnLaserOff, nil)
