briefingRoom.mission.missionFeatures.supportArtillery = { }
briefingRoom.mission.missionFeatures.supportArtillery.FIRE_MISSIONS_PER_OBJECTIVE = 3
briefingRoom.mission.missionFeatures.supportArtillery.AUTO_AIM_RADIUS = 1000 -- in meters
briefingRoom.mission.missionFeatures.supportArtillery.INACCURACY = 500 -- in meters
briefingRoom.mission.missionFeatures.supportArtillery.MARKER_NAME = "arty"
briefingRoom.mission.missionFeatures.supportArtillery.SHELLS_PER_FIRE_MISSION = 10
briefingRoom.mission.missionFeatures.supportArtillery.fireMissionsLeft = 0
briefingRoom.mission.missionFeatures.supportArtillery.markID = nil -- ID of the mark on the map
briefingRoom.mission.missionFeatures.supportArtillery.shellsLeftInFireMission = 0
briefingRoom.mission.missionFeatures.supportArtillery.disableCooRemovedRadioMessage = false

briefingRoom.mission.missionFeatures.supportArtillery.eventHandler = { }
function briefingRoom.mission.missionFeatures.supportArtillery.eventHandler:onEvent(event)
  if event.id == world.event.S_EVENT_MARK_REMOVED then
    if briefingRoom.mission.missionFeatures.supportArtillery.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.supportArtillery.markID then
      if not briefingRoom.mission.missionFeatures.supportArtillery.disableCooRemovedRadioMessage then
        briefingRoom.radioManager.play("Fire Support: Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      end
      briefingRoom.mission.missionFeatures.supportArtillery.markID = nil
    end
  elseif event.id == world.event.S_EVENT_MARK_ADDED then
    local markText = string.lower(tostring(event.text or ""))
    if markText == briefingRoom.mission.missionFeatures.supportArtillery.MARKER_NAME then
      if briefingRoom.mission.missionFeatures.supportArtillery.markID ~= nil then
        briefingRoom.mission.missionFeatures.supportArtillery.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.supportArtillery.markID)
        briefingRoom.mission.missionFeatures.supportArtillery.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.supportArtillery.markID = event.idx
      briefingRoom.radioManager.play("Fire Support: Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      return
    end
  elseif event.id == world.event.S_EVENT_MARK_CHANGE then
    local markText = string.lower(tostring(event.text or ""))

    if markText == briefingRoom.mission.missionFeatures.supportArtillery.MARKER_NAME then
      briefingRoom.radioManager.play("Fire Support: Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      if briefingRoom.mission.missionFeatures.supportArtillery.markID ~= nil then
        briefingRoom.mission.missionFeatures.supportArtillery.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.supportArtillery.markID)
        briefingRoom.mission.missionFeatures.supportArtillery.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.supportArtillery.markID = event.idx
    elseif briefingRoom.mission.missionFeatures.supportArtillery.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.supportArtillery.markID then
      briefingRoom.radioManager.play("Fire Support: Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      briefingRoom.mission.missionFeatures.supportArtillery.markID = nil
    end
  end
end

function briefingRoom.mission.missionFeatures.supportArtillery.doShell(args, time)
  briefingRoom.mission.missionFeatures.supportArtillery.shellsLeftInFireMission = briefingRoom.mission.missionFeatures.supportArtillery.shellsLeftInFireMission - 1
  
  for i=1,3 do
    local impactPoint = args.position
    local impactPointV2 = dcsExtensions.toVec2(impactPoint)
    local inaccuracy = math.randomPointInCircle({ ["x"] = 0, ["y"] = 0 }, briefingRoom.mission.missionFeatures.supportArtillery.INACCURACY)
    impactPoint.x = impactPoint.x + inaccuracy.x
    impactPoint.y = impactPoint.y + 0.5
    impactPoint.z = impactPoint.z + inaccuracy.y

    if i == 1 then
      local enemyUnits = dcsExtensions.getCoalitionUnits($LUAENEMYCOALITION$)
      for _,u in ipairs(enemyUnits) do
        if dcsExtensions.getDistance(dcsExtensions.toVec2(u:getPoint()), impactPointV2) < briefingRoom.mission.missionFeatures.supportArtillery.AUTO_AIM_RADIUS then
          if not u:inAir() then
            impactPoint = u:getPoint()
          end
        end
      end
    end

    trigger.action.explosion(impactPoint, 100)
  end

  if briefingRoom.mission.missionFeatures.supportArtillery.shellsLeftInFireMission <= 0 then -- no shells left in this fire mission
    return nil
  else
    return time + 1
  end
end

-- Internal function to begin executing fire mission (called when radio message is complete)
function briefingRoom.mission.missionFeatures.supportArtillery.doFireMission(args)
  briefingRoom.mission.missionFeatures.supportArtillery.shellsLeftInFireMission = briefingRoom.mission.missionFeatures.supportArtillery.SHELLS_PER_FIRE_MISSION
  timer.scheduleFunction(briefingRoom.mission.missionFeatures.supportArtillery.doShell, args, timer.getTime() + math.random(2,3))
end

-- Radio command to launch fire mission (called from F10 menu)
function briefingRoom.mission.missionFeatures.supportArtillery.launchFireMission()
  briefingRoom.radioManager.play("Pilot: Fire support, begin fire mission on provided coordinates.", "RadioPilotArtillery")
 
  if briefingRoom.mission.missionFeatures.supportArtillery.fireMissionsLeft <= 0 then
    briefingRoom.radioManager.play("Fire Support: Negative, no fire missions available.", "RadioArtilleryNoAmmo", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.supportArtillery.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.supportArtillery.markID then
      local args = { ["position"] = m.pos }
      briefingRoom.mission.missionFeatures.supportArtillery.fireMissionsLeft = briefingRoom.mission.missionFeatures.supportArtillery.fireMissionsLeft - 1
      briefingRoom.radioManager.play("Fire Support: Copy, firing for effect on provided coordinates ("..tostring(briefingRoom.mission.missionFeatures.supportArtillery.fireMissionsLeft).." fire mission(s) left).", "RadioArtilleryFiring", briefingRoom.radioManager.getAnswerDelay(), briefingRoom.mission.missionFeatures.supportArtillery.doFireMission, args)
      return
    end
  end

  briefingRoom.radioManager.play("Fire Support: Cannot comply. No coordinates provided for fire mission (add a marker named \""..string.upper(briefingRoom.mission.missionFeatures.supportArtillery.MARKER_NAME).."\" on the F10 map to designate a target).", "RadioArtilleryNoCoordinates", briefingRoom.radioManager.getAnswerDelay())
end

-- Set the correct number of fire missions
briefingRoom.mission.missionFeatures.supportArtillery.fireMissionsLeft = briefingRoom.mission.missionFeatures.supportArtillery.FIRE_MISSIONS_PER_OBJECTIVE * math.max(1, #briefingRoom.mission.objectives)

-- Add F10 menu command
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Begin fire mission on provided coordinates", briefingRoom.f10Menu.missionMenu, briefingRoom.mission.missionFeatures.supportArtillery.launchFireMission, nil)

-- Enable event handler
world.addEventHandler(briefingRoom.mission.missionFeatures.supportArtillery.eventHandler)
