briefingRoom.mission.missionFeatures.supportFriendlyBomber = { }
briefingRoom.mission.missionFeatures.supportFriendlyBomber.AUTO_AIM_RADIUS = 1000 -- in meters
briefingRoom.mission.missionFeatures.supportFriendlyBomber.MARKER_NAME = "bomber"
briefingRoom.mission.missionFeatures.supportFriendlyBomber.markID = nil -- ID of the mark on the map
briefingRoom.mission.missionFeatures.supportFriendlyBomber.disableCooRemovedRadioMessage = false

briefingRoom.mission.missionFeatures.supportFriendlyBomber.eventHandler = { }
function briefingRoom.mission.missionFeatures.supportFriendlyBomber.eventHandler:onEvent(event)
  if event.id == world.event.S_EVENT_MARK_REMOVED then
    if briefingRoom.mission.missionFeatures.supportFriendlyBomber.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.supportFriendlyBomber.markID then
      if not briefingRoom.mission.missionFeatures.supportFriendlyBomber.disableCooRemovedRadioMessage then
        briefingRoom.radioManager.play("Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      end
      briefingRoom.mission.missionFeatures.supportFriendlyBomber.markID = nil
    end
  elseif event.id == world.event.S_EVENT_MARK_ADDED then
    local markText = string.lower(tostring(event.text or ""))
    if markText == briefingRoom.mission.missionFeatures.supportFriendlyBomber.MARKER_NAME then
      if briefingRoom.mission.missionFeatures.supportFriendlyBomber.markID ~= nil then
        briefingRoom.mission.missionFeatures.supportFriendlyBomber.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.supportFriendlyBomber.markID)
        briefingRoom.mission.missionFeatures.supportFriendlyBomber.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.supportFriendlyBomber.markID = event.idx
      briefingRoom.radioManager.play("Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      return
    end
  elseif event.id == world.event.S_EVENT_MARK_CHANGE then
    local markText = string.lower(tostring(event.text or ""))

    if markText == briefingRoom.mission.missionFeatures.supportFriendlyBomber.MARKER_NAME then
      briefingRoom.radioManager.play("Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      if briefingRoom.mission.missionFeatures.supportFriendlyBomber.markID ~= nil then
        briefingRoom.mission.missionFeatures.supportFriendlyBomber.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.supportFriendlyBomber.markID)
        briefingRoom.mission.missionFeatures.supportFriendlyBomber.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.supportFriendlyBomber.markID = event.idx
    elseif briefingRoom.mission.missionFeatures.supportFriendlyBomber.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.supportFriendlyBomber.markID then
      briefingRoom.radioManager.play("Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      briefingRoom.mission.missionFeatures.supportFriendlyBomber.markID = nil
    end
  end
end

-- Radio command to launch bombing run (called from F10 menu)
function briefingRoom.mission.missionFeatures.supportFriendlyBomber.launchBombingRun()
  briefingRoom.radioManager.play("Threats .", "RadioPilotBeginYourBombingRun")
 
  if briefingRoom.mission.missionFeatures.supportFriendlyBomber.fireMissionsLeft <= 0 then
    briefingRoom.radioManager.play("Negative, no fire missions available.", "RadioArtilleryNoAmmo", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.supportFriendlyBomber.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.supportFriendlyBomber.markID then
      --local args = { ["position"] = m.pos }
      local group = dcsExtensions.getGroupByID(briefingRoom.mission.missionFeatures.groupsID.supportFriendlyBomber)
      if group ~= nil then
        group:activate()
        group:getController():setTask({ id = 'Bombing', params = { point = { x = m.pos.x, y = m.pos.z }, weaponType = 2147485694, expend = AI.Task.WeaponExpend.FOUR, attackQty = 1, groupAttack = true } })
        briefingRoom.radioManager.play("Copy, beginning bombing run on coordinates.", "RadioArtilleryFiring", briefingRoom.radioManager.getAnswerDelay(), nil, nil)
      end
      return
    end
  end

  briefingRoom.radioManager.play("Cannot comply. No coordinates provided for bombing run (add a marker named \""..string.upper(briefingRoom.mission.missionFeatures.supportFriendlyBomber.MARKER_NAME).."\" on the F10 map to designate a target).", "RadioArtilleryNoCoordinates", briefingRoom.radioManager.getAnswerDelay())
end

-- Add F10 menu command
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Begin bombing run on provided coordinates", briefingRoom.f10Menu.missionMenu, briefingRoom.mission.missionFeatures.supportFriendlyBomber.launchBombingRun, nil)

-- Enable event handler
world.addEventHandler(briefingRoom.mission.missionFeatures.supportFriendlyBomber.eventHandler)
