briefingRoom.mission.missionFeatures.supportFriendlyCAS = { }
briefingRoom.mission.missionFeatures.supportFriendlyCAS.AUTO_AIM_RADIUS = 1000 -- in meters
briefingRoom.mission.missionFeatures.supportFriendlyCAS.MARKER_NAME = "cas"
briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID = nil -- ID of the mark on the map
briefingRoom.mission.missionFeatures.supportFriendlyCAS.disableCooRemovedRadioMessage = false

briefingRoom.mission.missionFeatures.supportFriendlyCAS.eventHandler = { }
function briefingRoom.mission.missionFeatures.supportFriendlyCAS.eventHandler:onEvent(event)
  if event.id == world.event.S_EVENT_MARK_REMOVED then
    if briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID then
      if not briefingRoom.mission.missionFeatures.supportFriendlyCAS.disableCooRemovedRadioMessage then
        briefingRoom.radioManager.play("CAS: Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      end
      briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID = nil
    end
  elseif event.id == world.event.S_EVENT_MARK_ADDED then
    local markText = string.lower(tostring(event.text or ""))
    if markText == briefingRoom.mission.missionFeatures.supportFriendlyCAS.MARKER_NAME then
      if briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID ~= nil then
        briefingRoom.mission.missionFeatures.supportFriendlyCAS.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID)
        briefingRoom.mission.missionFeatures.supportFriendlyCAS.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID = event.idx
      briefingRoom.radioManager.play("CAS: Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      return
    end
  elseif event.id == world.event.S_EVENT_MARK_CHANGE then
    local markText = string.lower(tostring(event.text or ""))

    if markText == briefingRoom.mission.missionFeatures.supportFriendlyCAS.MARKER_NAME then
      briefingRoom.radioManager.play("CAS: Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      if briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID ~= nil then
        briefingRoom.mission.missionFeatures.supportFriendlyCAS.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID)
        briefingRoom.mission.missionFeatures.supportFriendlyCAS.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID = event.idx
    elseif briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID then
      briefingRoom.radioManager.play("CAS: Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID = nil
    end
  end
end

-- Radio command to launch bombing run (called from F10 menu)
function briefingRoom.mission.missionFeatures.supportFriendlyCAS.launchBombingRun()
  briefingRoom.radioManager.play("Pilot: CAS, begin your attack.", "RadioPilotBeginYourAttack")
  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID then
      local group = dcsExtensions.getGroupByID(briefingRoom.mission.missionFeatures.groupsID.supportFriendlyCAS)
      if group ~= nil then
        group:activate()
        timer.scheduleFunction(briefingRoom.mission.missionFeatures.supportFriendlyCAS.setTask, {}, timer.getTime() + 10) --just re-run after 10 s
        briefingRoom.radioManager.play("CAS: Copy, beginning my attack.", "RadioOtherPilotBeginAttack", briefingRoom.radioManager.getAnswerDelay(), nil, nil)
      end
      return
    end
  end

  briefingRoom.radioManager.play("CAS: Cannot comply. No coordinates provided for patrol run (add a marker named \""..string.upper(briefingRoom.mission.missionFeatures.supportFriendlyCAS.MARKER_NAME).."\" on the F10 map to designate a target).", "RadioArtilleryNoCoordinates", briefingRoom.radioManager.getAnswerDelay())
end

function briefingRoom.mission.missionFeatures.supportFriendlyCAS.setTask()
  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.supportFriendlyCAS.markID then
      local group = dcsExtensions.getGroupByID(briefingRoom.mission.missionFeatures.groupsID.supportFriendlyCAS)
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
        wp.name = "CAS"
        wp.task ={ id = 'EngageTargets', params = { targetTypes = { [1] = "All"} }} 
        
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
        group:getController():pushTask({ id = 'EngageTargetsInZone', params = { targetTypes = { [1] = "All"}, point = dcsExtensions.toVec2(m.pos), zoneRadius = 15000 }})
      end
      return
    end
  end
end




-- Add F10 menu command
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Begin CAS on provided coordinates", briefingRoom.f10Menu.missionMenu, briefingRoom.mission.missionFeatures.supportFriendlyCAS.launchBombingRun, nil)


-- Enable event handler
world.addEventHandler(briefingRoom.mission.missionFeatures.supportFriendlyCAS.eventHandler)
