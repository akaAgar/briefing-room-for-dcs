briefingRoom.mission.missionFeatures.friendlyTaskableGround = { }
briefingRoom.mission.missionFeatures.friendlyTaskableGround.AUTO_AIM_RADIUS = 1000 -- in meters
briefingRoom.mission.missionFeatures.friendlyTaskableGround.SELECT_MARKER_NAME = "select"
briefingRoom.mission.missionFeatures.friendlyTaskableGround.MARKER_NAME = "move"
briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID = nil -- ID of the mark on the map
briefingRoom.mission.missionFeatures.friendlyTaskableGround.selectMarkID = nil -- ID of the mark on the map
briefingRoom.mission.missionFeatures.friendlyTaskableGround.disableCooRemovedRadioMessage = false

briefingRoom.mission.missionFeatures.friendlyTaskableGround.eventHandler = { }
function briefingRoom.mission.missionFeatures.friendlyTaskableGround.eventHandler:onEvent(event)
  if event.id == world.event.S_EVENT_MARK_REMOVED then
    if briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID then
      if not briefingRoom.mission.missionFeatures.friendlyTaskableGround.disableCooRemovedRadioMessage then
        briefingRoom.radioManager.play("Ground: Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID = nil
    end
    if briefingRoom.mission.missionFeatures.friendlyTaskableGround.selectMarkID ~= nil and event.idx == briefingRoom.mission.missionFeatures.friendlyTaskableGround.selectMarkID then
      if not briefingRoom.mission.missionFeatures.friendlyTaskableGround.disableCooRemovedRadioMessage then
        briefingRoom.radioManager.play("Ground: Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableGround.selectMarkID = nil
    end
  elseif event.id == world.event.S_EVENT_MARK_ADDED then
    local markText = string.lower(tostring(event.text or ""))
    if markText == briefingRoom.mission.missionFeatures.friendlyTaskableGround.MARKER_NAME then
      if briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID ~= nil then
        briefingRoom.mission.missionFeatures.friendlyTaskableGround.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID)
        briefingRoom.mission.missionFeatures.friendlyTaskableGround.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID = event.idx
      briefingRoom.radioManager.play("Ground: Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      return
    end
    if markText == briefingRoom.mission.missionFeatures.friendlyTaskableGround.SELECT_MARKER_NAME then
      if briefingRoom.mission.missionFeatures.friendlyTaskableGround.selectMarkID ~= nil then
        briefingRoom.mission.missionFeatures.friendlyTaskableGround.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.friendlyTaskableGround.selectMarkID)
        briefingRoom.mission.missionFeatures.friendlyTaskableGround.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableGround.selectMarkID = event.idx
      briefingRoom.radioManager.play("Ground: Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      return
    end
  elseif event.id == world.event.S_EVENT_MARK_CHANGE then
    local markText = string.lower(tostring(event.text or ""))

    if markText == briefingRoom.mission.missionFeatures.friendlyTaskableGround.MARKER_NAME then
      briefingRoom.radioManager.play("Ground: Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      if briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID ~= nil then
        briefingRoom.mission.missionFeatures.friendlyTaskableGround.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID)
        briefingRoom.mission.missionFeatures.friendlyTaskableGround.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID = event.idx
    elseif briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID then
      briefingRoom.radioManager.play("Ground: Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID = nil
    end
    if markText == briefingRoom.mission.missionFeatures.friendlyTaskableGround.SELECT_MARKER_NAME then
      briefingRoom.radioManager.play("Ground: Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      if briefingRoom.mission.missionFeatures.friendlyTaskableGround.selectMarkID ~= nil then
        briefingRoom.mission.missionFeatures.friendlyTaskableGround.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.friendlyTaskableGround.selectMarkID)
        briefingRoom.mission.missionFeatures.friendlyTaskableGround.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableGround.selectMarkID = event.idx
    elseif briefingRoom.mission.missionFeatures.friendlyTaskableGround.selectMarkID ~= nil and event.idx == briefingRoom.mission.missionFeatures.friendlyTaskableGround.selectMarkID then
      briefingRoom.radioManager.play("Ground: Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      briefingRoom.mission.missionFeatures.friendlyTaskableGround.selectMarkID = nil
    end
  end
end

function briefingRoom.mission.missionFeatures.friendlyTaskableGround.getNearestGroup()
  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if m.idx == briefingRoom.mission.missionFeatures.friendlyTaskableGround.selectMarkID then
      local selectGroup = 0
      local minDist = 999999999999999999999999999999999999999999
      for _,g in pairs(coalition.getGroups($LUAPLAYERCOALITION$)) do
        local dist = mist.utils.get2DDist(mist.getLeadPos(g), m.pos)
        if dist < minDist then
          selectGroup = g
          minDist = dist
        end
      end
      return selectGroup
    end
  end
end


-- Radio command to launch bombing run (called from F10 menu)
function briefingRoom.mission.missionFeatures.friendlyTaskableGround.launchBombingRun()
  briefingRoom.radioManager.play("Pilot: Command, reqesting Ground support.", "RadioPilotGroundSupport")

  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID then
      local group = briefingRoom.mission.missionFeatures.friendlyTaskableGround.getNearestGroup()
      if group ~= nil then
        group:activate()
        local Start = {
          id = 'Start',
          params = {
          }
        }
        group:getController():setCommand(Start)
        timer.scheduleFunction(briefingRoom.mission.missionFeatures.friendlyTaskableGround.setTask, {onRoad = false}, timer.getTime() + 10) --just re-run after 10 s
        briefingRoom.radioManager.play("Command: Affirm, Ground support is on its way.", "RadioHQGroundSupport", briefingRoom.radioManager.getAnswerDelay(), nil, nil)
      end
      return
    end
  end

  briefingRoom.radioManager.play("Ground: Cannot comply. No coordinates provided for patrol run (add a marker named \""..string.upper(briefingRoom.mission.missionFeatures.friendlyTaskableGround.MARKER_NAME).."\" on the F10 map to designate a target).", "RadioArtilleryNoCoordinates", briefingRoom.radioManager.getAnswerDelay())
end

function briefingRoom.mission.missionFeatures.friendlyTaskableGround.setTask(onRoad)
  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.friendlyTaskableGround.markID then
      local group = dcsExtensions.getGroupByID(briefingRoom.mission.missionFeatures.groupsID.friendlyTaskableGround)
      if group ~= nil then
        local newTask = {
          id = 'Mission',
          airborne = false,
          params = {
            route = {
              points = {
                [1] = {
                  speed = 5,
                  x = m.pos.x,
                  y = m.pos.z,
                  type = 'Turning Point',
                  ETA_locked = false,
                  ETA = 100,
                  alt = 14,
                  alt_type = "BARO",
                  speed_locked = false,
                  action = "Off Road",
                  name = "Ground",
                  task = {
                    id = "ComboTask",
                    params = {
                      tasks = {
                        }
                      }
                    }
                  }
                }
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
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Begin Ground on provided coordinates", briefingRoom.f10Menu.missionMenu, briefingRoom.mission.missionFeatures.friendlyTaskableGround.launchBombingRun, nil)

-- Enable event handler
world.addEventHandler(briefingRoom.mission.missionFeatures.friendlyTaskableGround.eventHandler)
