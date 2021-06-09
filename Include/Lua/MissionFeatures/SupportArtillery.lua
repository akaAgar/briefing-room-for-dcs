briefingRoom.mission.features.artillery = { }
briefingRoom.mission.features.artillery.FIRE_MISSIONS_PER_OBJECTIVE = 3
briefingRoom.mission.features.artillery.AUTO_AIM_RADIUS = 1000 -- in meters
briefingRoom.mission.features.artillery.INACCURACY = 500 -- in meters
briefingRoom.mission.features.artillery.MARKER_NAME = "artillery"
briefingRoom.mission.features.artillery.SHELLS_PER_FIRE_MISSION = 10
briefingRoom.mission.features.artillery.fireMissionsLeft = 0
briefingRoom.mission.features.artillery.markID = nil -- ID of the mark on the map
briefingRoom.mission.features.artillery.shellsLeftInFireMission = 0
briefingRoom.mission.features.artillery.disableCooRemovedRadioMessage = false

function briefingRoom.mission.features.artillery:onEvent(event)
  if event.id == world.event.S_EVENT_MARK_REMOVED then
    if briefingRoom.mission.features.artillery.markID ~= nil and event.idx == briefingRoom.mission.features.artillery.markID then
      if not briefingRoom.mission.features.artillery.disableCooRemovedRadioMessage then
        briefingRoom.radioManager.play("Affirm, coordinates discarded. Awaiting new coordinates", "RadioCoordinatesDiscardedM")
      end
      briefingRoom.mission.features.artillery.markID = nil
    end
  elseif event.id == world.event.S_EVENT_MARK_ADDED then
    local markText = string.lower(tostring(event.text or ""))
    if markText == briefingRoom.mission.features.artillery.MARKER_NAME then
      if briefingRoom.mission.features.artillery.markID ~= nil then
        briefingRoom.mission.features.artillery.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.features.artillery.markID)
        briefingRoom.mission.features.artillery.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.features.artillery.markID = event.idx
      briefingRoom.radioManager.play("Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      return
    end
  elseif event.id == world.event.S_EVENT_MARK_CHANGE then
    local markText = string.lower(tostring(event.text or ""))

    if markText == briefingRoom.mission.features.artillery.MARKER_NAME then
      briefingRoom.radioManager.play("Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      if briefingRoom.mission.features.artillery.markID ~= nil then
        briefingRoom.mission.features.artillery.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.features.artillery.markID)
        briefingRoom.mission.features.artillery.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.features.artillery.markID = event.idx
    elseif briefingRoom.mission.features.artillery.markID ~= nil and event.idx == briefingRoom.mission.features.artillery.markID then
      briefingRoom.radioManager.play("Affirm, coordinates discarded. Awaiting new coordinates", "RadioCoordinatesDiscardedM")
      briefingRoom.mission.features.artillery.markID = nil
    end
  end
end

function briefingRoom.mission.features.artillery.doShell(args, time)
  briefingRoom.mission.features.artillery.shellsLeftInFireMission = briefingRoom.mission.features.artillery.shellsLeftInFireMission - 1
  
  for i=1,3 do
    local impactPoint = args.position
    local impactPointV2 = dcsExtensions.toVec2(impactPoint)
    local inaccuracy = math.randomPointInCircle({ ["x"] = 0, ["y"] = 0 }, briefingRoom.mission.features.artillery.INACCURACY)
    impactPoint.x = impactPoint.x + inaccuracy.x
    impactPoint.y = impactPoint.y + 0.5
    impactPoint.z = impactPoint.z + inaccuracy.y

    if i == 1 then
      local enemyUnits = dcsExtensions.getCoalitionUnits($ENEMYCOALITION$)
      for _,u in ipairs(enemyUnits) do
        if dcsExtensions.getDistance(dcsExtensions.toVec2(u:getPoint()), impactPointV2) < briefingRoom.mission.features.artillery.AUTO_AIM_RADIUS then
          if not u:inAir() then
            impactPoint = u:getPoint()
          end
        end
      end
    end

    trigger.action.explosion(impactPoint, 100)
  end

  if briefingRoom.mission.features.artillery.shellsLeftInFireMission <= 0 then -- no shells left in this fire mission
    return nil
  else
    return time + 1
  end
end

function briefingRoom.mission.features.artillery.doFireMission(args)
  briefingRoom.mission.features.artillery.shellsLeftInFireMission = briefingRoom.mission.features.artillery.SHELLS_PER_FIRE_MISSION
  timer.scheduleFunction(briefingRoom.mission.features.artillery.doShell, args, timer.getTime() + math.random(2,3))
end

function briefingRoom.mission.features.artillery.launchFireMission()
  briefingRoom.radioManager.play("Fire support, begin fire mission on provided coordinates", "RadioPilotArtillery")
 
  if briefingRoom.mission.features.artillery.fireMissionsLeft <= 0 then
    briefingRoom.radioManager.play("Negative, no fire missions available.", "RadioArtilleryNoAmmo", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.features.artillery.markID ~= nil and m.idx == briefingRoom.mission.features.artillery.markID then
      local args = { ["position"] = m.pos }
      briefingRoom.mission.features.artillery.fireMissionsLeft = briefingRoom.mission.features.artillery.fireMissionsLeft - 1
      briefingRoom.radioManager.play("Copy, firing for effect on provided coordinates ("..tostring(briefingRoom.mission.features.artillery.fireMissionsLeft).." fire mission(s) left).", "RadioArtilleryFiring", briefingRoom.radioManager.getAnswerDelay(), briefingRoom.mission.features.artillery.doFireMission, args)
      return
    end
  end

  briefingRoom.radioManager.play("Cannot comply. No coordinates provided for fire mission (add a marker named \""..string.upper(briefingRoom.mission.features.artillery.MARKER_NAME).."\" on the F10 map to designate a target).", "RadioArtilleryNoCoordinates", briefingRoom.radioManager.getAnswerDelay())
end

world.addEventHandler(briefingRoom.mission.features.artillery)

-- set the correct number of fire missions
briefingRoom.mission.features.artillery.fireMissionsLeft = briefingRoom.mission.features.artillery.FIRE_MISSIONS_PER_OBJECTIVE * #briefingRoom.mission.objectives

if briefingRoom.f10Menu.supportMenu == nil then
end
missionCommands.addCommandForCoalition($PLAYERCOALITION$, "Begin fire mission on provided coordinates", briefingRoom.f10Menu.supportMenu, briefingRoom.mission.features.artillery.launchFireMission, nil)
