briefingRoom.mission.missionFeatures.friendlyTaskableCAP = { }
briefingRoom.mission.missionFeatures.friendlyTaskableCAP.AUTO_AIM_RADIUS = 1000 -- in meters
briefingRoom.mission.missionFeatures.friendlyTaskableCAP.MARKER_NAME = "cap"
briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID = nil -- ID of the mark on the map
briefingRoom.mission.missionFeatures.friendlyTaskableCAP.disableCooRemovedRadioMessage = false

briefingRoom.mission.missionFeatures.friendlyTaskableCAP.eventHandler = { }
function briefingRoom.mission.missionFeatures.friendlyTaskableCAP.eventHandler:onEvent(event)
  if event.id == world.event.S_EVENT_MARK_REMOVED then
    if briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID then
      if not briefingRoom.mission.missionFeatures.friendlyTaskableCAP.disableCooRemovedRadioMessage then
        briefingRoom.radioManager.play("CAP: Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID = nil
    end
  elseif event.id == world.event.S_EVENT_MARK_ADDED then
    local markText = string.lower(tostring(event.text or ""))
    if markText == briefingRoom.mission.missionFeatures.friendlyTaskableCAP.MARKER_NAME then
      if briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID ~= nil then
        briefingRoom.mission.missionFeatures.friendlyTaskableCAP.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID)
        briefingRoom.mission.missionFeatures.friendlyTaskableCAP.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID = event.idx
      briefingRoom.radioManager.play("CAP: Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      return
    end
  elseif event.id == world.event.S_EVENT_MARK_CHANGE then
    local markText = string.lower(tostring(event.text or ""))

    if markText == briefingRoom.mission.missionFeatures.friendlyTaskableCAP.MARKER_NAME then
      briefingRoom.radioManager.play("CAP: Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      if briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID ~= nil then
        briefingRoom.mission.missionFeatures.friendlyTaskableCAP.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID)
        briefingRoom.mission.missionFeatures.friendlyTaskableCAP.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID = event.idx
    elseif briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID then
      briefingRoom.radioManager.play("CAP: Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID = nil
    end
  end
end

-- Radio command to launch bombing run (called from F10 menu)
function briefingRoom.mission.missionFeatures.friendlyTaskableCAP.launchBombingRun()
  briefingRoom.radioManager.play("Pilot: Command, requesting CAP support.", "RadioPilotCAPSupport")
  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID then
      local group = dcsExtensions.getGroupByID(briefingRoom.mission.missionFeatures.groupsID.friendlyTaskableCAP)
      if group ~= nil then
        group:activate()
        timer.scheduleFunction(briefingRoom.mission.missionFeatures.friendlyTaskableCAP.setTask, {}, timer.getTime() + 10) --just re-run after 10 s
        briefingRoom.radioManager.play("Command: Affirm, CAP support is on its way.", "RadioHQCAPSupport", briefingRoom.radioManager.getAnswerDelay(), nil, nil)
      end
      return
    end
  end

  briefingRoom.radioManager.play("CAP: Cannot comply. No coordinates provided for patrol run (add a marker named \""..string.upper(briefingRoom.mission.missionFeatures.friendlyTaskableCAP.MARKER_NAME).."\" on the F10 map to designate a target).", "RadioArtilleryNoCoordinates", briefingRoom.radioManager.getAnswerDelay())
end

function briefingRoom.mission.missionFeatures.friendlyTaskableCAP.setTask()
  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.friendlyTaskableCAP.markID then
      local group = dcsExtensions.getGroupByID(briefingRoom.mission.missionFeatures.groupsID.friendlyTaskableCAP)
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
        wp.name = "CAP"
        wp.task = { id = 'EngageTargets', params = { targetTypes = { [1] = "Air"} }} 
        
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
        group:getController():pushTask({ id = 'Orbit', params = { point = dcsExtensions.toVec2(m.pos), pattern  = "Circle", speed = 200, altitude = 7620}})
      end
      return
    end
  end
end




-- Add F10 menu command
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Begin CAP on provided coordinates", briefingRoom.f10Menu.missionMenu, briefingRoom.mission.missionFeatures.friendlyTaskableCAP.launchBombingRun, nil)


-- Enable event handler
world.addEventHandler(briefingRoom.mission.missionFeatures.friendlyTaskableCAP.eventHandler)
