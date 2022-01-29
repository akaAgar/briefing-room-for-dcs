briefingRoom.mission.missionFeatures.friendlyTaskableBomber = { }
briefingRoom.mission.missionFeatures.friendlyTaskableBomber.AUTO_AIM_RADIUS = 1000 -- in meters
briefingRoom.mission.missionFeatures.friendlyTaskableBomber.MARKER_NAME = "bomber"
briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID = nil -- ID of the mark on the map
briefingRoom.mission.missionFeatures.friendlyTaskableBomber.disableCooRemovedRadioMessage = false

briefingRoom.mission.missionFeatures.friendlyTaskableBomber.eventHandler = { }
function briefingRoom.mission.missionFeatures.friendlyTaskableBomber.eventHandler:onEvent(event)
  if event.id == world.event.S_EVENT_MARK_REMOVED then
    if briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID then
      if not briefingRoom.mission.missionFeatures.friendlyTaskableBomber.disableCooRemovedRadioMessage then
        briefingRoom.radioManager.play("Bomber: Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID = nil
    end
  elseif event.id == world.event.S_EVENT_MARK_ADDED then
    local markText = string.lower(tostring(event.text or ""))
    if markText == briefingRoom.mission.missionFeatures.friendlyTaskableBomber.MARKER_NAME then
      if briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID ~= nil then
        briefingRoom.mission.missionFeatures.friendlyTaskableBomber.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID)
        briefingRoom.mission.missionFeatures.friendlyTaskableBomber.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID = event.idx
      briefingRoom.radioManager.play("Bomber: Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      return
    end
  elseif event.id == world.event.S_EVENT_MARK_CHANGE then
    local markText = string.lower(tostring(event.text or ""))

    if markText == briefingRoom.mission.missionFeatures.friendlyTaskableBomber.MARKER_NAME then
      briefingRoom.radioManager.play("Bomber: Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      if briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID ~= nil then
        briefingRoom.mission.missionFeatures.friendlyTaskableBomber.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID)
        briefingRoom.mission.missionFeatures.friendlyTaskableBomber.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID = event.idx
    elseif briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID then
      briefingRoom.radioManager.play("Bomber: Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID = nil
    end
  end
end

-- Radio command to launch bombing run (called from F10 menu)
function briefingRoom.mission.missionFeatures.friendlyTaskableBomber.launchBombingRun()
  briefingRoom.radioManager.play("Pilot: Bomber, begin your run.", "RadioPilotBeginYourBombingRun")
 
  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID then
      local group = dcsExtensions.getGroupByID(briefingRoom.mission.missionFeatures.groupsID.friendlyTaskableBomber)
      if group ~= nil then
        group:activate()
        timer.scheduleFunction(briefingRoom.mission.missionFeatures.friendlyTaskableBomber.setTask, {}, timer.getTime() + 10)
        briefingRoom.radioManager.play("Bomber: Copy, beginning bombing run on coordinates.", "RadioOtherPilotBeginBombing", briefingRoom.radioManager.getAnswerDelay(), nil, nil)
      end
      return
    end
  end

  briefingRoom.radioManager.play("Bomber: Cannot comply. No coordinates provided for bombing run (add a marker named \""..string.upper(briefingRoom.mission.missionFeatures.friendlyTaskableBomber.MARKER_NAME).."\" on the F10 map to designate a target).", "RadioArtilleryNoCoordinates", briefingRoom.radioManager.getAnswerDelay())
end

function briefingRoom.mission.missionFeatures.friendlyTaskableBomber.setTask()
  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.friendlyTaskableBomber.markID then
      local group = dcsExtensions.getGroupByID(briefingRoom.mission.missionFeatures.groupsID.friendlyTaskableBomber)
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
        wp.name = "BOMB"
        wp.task = { id = 'Bombing', params = { point = dcsExtensions.toVec2(m.pos), weaponType = 2956984318, expend = AI.Task.WeaponExpend.FOUR, attackQty = 1, groupAttack = true } }
        
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
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Begin bombing run on provided coordinates", briefingRoom.f10Menu.missionMenu, briefingRoom.mission.missionFeatures.friendlyTaskableBomber.launchBombingRun, nil)

-- Enable event handler
world.addEventHandler(briefingRoom.mission.missionFeatures.friendlyTaskableBomber.eventHandler)
