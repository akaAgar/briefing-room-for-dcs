briefingRoom.mission.missionFeatures.friendlyTaskableSEAD = { }
briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.AUTO_AIM_RADIUS = 1000 -- in meters
briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.MARKER_NAME = "sead"
briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID = nil -- ID of the mark on the map
briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.disableCooRemovedRadioMessage = false

briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.eventHandler = { }
function briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.eventHandler:onEvent(event)
  if event.id == world.event.S_EVENT_MARK_REMOVED then
    if briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID then
      if not briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.disableCooRemovedRadioMessage then
        briefingRoom.radioManager.play("SEAD: Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID = nil
    end
  elseif event.id == world.event.S_EVENT_MARK_ADDED then
    local markText = string.lower(tostring(event.text or ""))
    if markText == briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.MARKER_NAME then
      if briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID ~= nil then
        briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID)
        briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID = event.idx
      briefingRoom.radioManager.play("SEAD: Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      return
    end
  elseif event.id == world.event.S_EVENT_MARK_CHANGE then
    local markText = string.lower(tostring(event.text or ""))

    if markText == briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.MARKER_NAME then
      briefingRoom.radioManager.play("SEAD: Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      if briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID ~= nil then
        briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID)
        briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID = event.idx
    elseif briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID then
      briefingRoom.radioManager.play("SEAD: Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID = nil
    end
  end
end

-- Radio command to launch bombing run (called from F10 menu)
function briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.launchBombingRun()
  briefingRoom.radioManager.play("Pilot: Command, reqesting SEAD support.", "RadioPilotSEADSupport")

  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID then
      local group = dcsExtensions.getGroupByID(briefingRoom.mission.missionFeatures.groupsID.friendlyTaskableSEAD)
      if group ~= nil then
        group:activate()
        timer.scheduleFunction(briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.setTask, {}, timer.getTime() + 10) --just re-run after 10 s
        briefingRoom.radioManager.play("Command: Affirm, SEAD support is on its way.", "RadioHQSEADSupport", briefingRoom.radioManager.getAnswerDelay(), nil, nil)
      end
      return
    end
  end

  briefingRoom.radioManager.play("SEAD: Cannot comply. No coordinates provided for patrol run (add a marker named \""..string.upper(briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.MARKER_NAME).."\" on the F10 map to designate a target).", "RadioArtilleryNoCoordinates", briefingRoom.radioManager.getAnswerDelay())
end

function briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.setTask()
  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID then
      local group = dcsExtensions.getGroupByID(briefingRoom.mission.missionFeatures.groupsID.friendlyTaskableSEAD)
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
        wp.task = { id = 'EngageTargets', params = { targetTypes = { [1] = "Air Defence"} }} 
        
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
        group:getController():pushTask({ id = 'EngageTargetsInZone', params = { targetTypes = { [1] = "Air Defence"}, point = dcsExtensions.toVec2(m.pos), zoneRadius = 15000 }})
      end
      return
    end
  end
end

-- Add F10 menu command
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Begin SEAD on provided coordinates", briefingRoom.f10Menu.missionMenu, briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.launchBombingRun, nil)

-- Enable event handler
world.addEventHandler(briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.eventHandler)
