briefingRoom.mission.missionFeatures.supportFriendlySEAD = { }
briefingRoom.mission.missionFeatures.supportFriendlySEAD.AUTO_AIM_RADIUS = 1000 -- in meters
briefingRoom.mission.missionFeatures.supportFriendlySEAD.MARKER_NAME = "sead"
briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID = nil -- ID of the mark on the map
briefingRoom.mission.missionFeatures.supportFriendlySEAD.disableCooRemovedRadioMessage = false

briefingRoom.mission.missionFeatures.supportFriendlySEAD.eventHandler = { }
function briefingRoom.mission.missionFeatures.supportFriendlySEAD.eventHandler:onEvent(event)
  if event.id == world.event.S_EVENT_MARK_REMOVED then
    if briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID then
      if not briefingRoom.mission.missionFeatures.supportFriendlySEAD.disableCooRemovedRadioMessage then
        briefingRoom.radioManager.play("SEAD, Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      end
      briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID = nil
    end
  elseif event.id == world.event.S_EVENT_MARK_ADDED then
    local markText = string.lower(tostring(event.text or ""))
    if markText == briefingRoom.mission.missionFeatures.supportFriendlySEAD.MARKER_NAME then
      if briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID ~= nil then
        briefingRoom.mission.missionFeatures.supportFriendlySEAD.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID)
        briefingRoom.mission.missionFeatures.supportFriendlySEAD.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID = event.idx
      briefingRoom.radioManager.play("SEAD, Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      return
    end
  elseif event.id == world.event.S_EVENT_MARK_CHANGE then
    local markText = string.lower(tostring(event.text or ""))

    if markText == briefingRoom.mission.missionFeatures.supportFriendlySEAD.MARKER_NAME then
      briefingRoom.radioManager.play("SEAD, Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      if briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID ~= nil then
        briefingRoom.mission.missionFeatures.supportFriendlySEAD.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID)
        briefingRoom.mission.missionFeatures.supportFriendlySEAD.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID = event.idx
    elseif briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID then
      briefingRoom.radioManager.play("SEAD, Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID = nil
    end
  end
end

-- Radio command to launch bombing run (called from F10 menu)
function briefingRoom.mission.missionFeatures.supportFriendlySEAD.launchBombingRun()
  briefingRoom.radioManager.play("SEAD, begin your patrol.", "RadioPilotBeginYourBombingRun")

  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID then
      local group = dcsExtensions.getGroupByID(briefingRoom.mission.missionFeatures.groupsID.supportFriendlySEAD)
      if group ~= nil then
        group:activate()
        timer.scheduleFunction(briefingRoom.mission.missionFeatures.supportFriendlySEAD.setTask, {}, timer.getTime() + 10) --just re-run after 10 s
        briefingRoom.radioManager.play("Copy, beginning patrol run on coordinates.", "RadioArtilleryFiring", briefingRoom.radioManager.getAnswerDelay(), nil, nil)
      end
      return
    end
  end

  briefingRoom.radioManager.play("Cannot comply. No coordinates provided for patrol run (add a marker named \""..string.upper(briefingRoom.mission.missionFeatures.supportFriendlySEAD.MARKER_NAME).."\" on the F10 map to designate a target).", "RadioArtilleryNoCoordinates", briefingRoom.radioManager.getAnswerDelay())
end

function briefingRoom.mission.missionFeatures.supportFriendlySEAD.setTask()
  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.supportFriendlySEAD.markID then
      local group = dcsExtensions.getGroupByID(briefingRoom.mission.missionFeatures.groupsID.supportFriendlySEAD)
      if group ~= nil then
        local wp = {}
        wp.speed = 200
        wp.x = m.pos.x
        wp.y = m.pos.z                
        wp.type = 'Turning Point'
        wp.ETA_locked = true
        wp.ETA = 100
        wp.alt = 7620
        wp.alt_type = "BARO"
        wp.speed_locked = true
        wp.action = "Fly Over Point"
        wp.airdromeId = nil
        wp.helipadId = nil
        wp.name = "SEAD"
        wp.task = { id = 'Orbit', params = { point = dcsExtensions.toVec2(m.pos), pattern  = "Circle", speed = 200, altitude = 7620}} 
        
        local newRoute = {}
        newRoute[1]=wp
        
        local newTask = {
            id = 'Mission',
            airborne = true,
            params = {
                route = {
                    points = newRoute,
                },
            },
        }
        group:getController():setTask(newTask)
      end
      return
    end
  end
end

-- Add F10 menu command
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Begin SEAD on provided coordinates", briefingRoom.f10Menu.missionMenu, briefingRoom.mission.missionFeatures.supportFriendlySEAD.launchBombingRun, nil)

-- Enable event handler
world.addEventHandler(briefingRoom.mission.missionFeatures.supportFriendlySEAD.eventHandler)
