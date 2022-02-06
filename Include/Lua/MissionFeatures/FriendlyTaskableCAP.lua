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
        local Start = {
          id = 'Start',
          params = {
          }
        }
        group:getController():setCommand(Start)
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
        local newTask = {
          id = 'Mission',
          airborne = true,
          params = {
            route = {
              points = {
                [1] = {
                  speed = 200,
                  x = m.pos.x,
                  y = m.pos.z,
                  type = 'Turning Point',
                  ETA_locked = false,
                  ETA = 100,
                  alt = 7620,
                  alt_type = "BARO",
                  speed_locked = false,
                  action = "Fly Over Point",
                  name = "CAP",
                  task = {
                    id = "ComboTask",
                    params = {
                      tasks = {
                        [1] = {
                          enabled = true,
                          auto = false,
                          id = "EngageTargets",
                          number = 1,
                          params = {
                            targetTypes = {
                              [1] = "Air",
                            },
                            value = "Air;",
                            noTargetTypes = {
                            },
                            priority = 0,
                          },
                        },
                        [2] = {
                          enabled = true,
                          auto = false,
                          id = "Orbit",
                          number = 2,
                          params = {
                            altitude = 7620,
                            pattern = "Circle",
                            speed = 100,
                          }
                        }
                      }
                    }
                  }
                },
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
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Begin CAP on provided coordinates", briefingRoom.f10Menu.missionMenu, briefingRoom.mission.missionFeatures.friendlyTaskableCAP.launchBombingRun, nil)


-- Enable event handler
world.addEventHandler(briefingRoom.mission.missionFeatures.friendlyTaskableCAP.eventHandler)
