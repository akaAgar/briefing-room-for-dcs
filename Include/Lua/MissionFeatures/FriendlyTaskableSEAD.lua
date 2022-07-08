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
        briefingRoom.radioManager.play("SEAD: $LANGDISCARDCOORDINATES$", "RadioCoordinatesDiscardedM")
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
      briefingRoom.radioManager.play("SEAD: $LANGUPDATECOORDINATES$", "RadioCoordinatesUpdatedM")
      return
    end
  elseif event.id == world.event.S_EVENT_MARK_CHANGE then
    local markText = string.lower(tostring(event.text or ""))

    if markText == briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.MARKER_NAME then
      briefingRoom.radioManager.play("SEAD: $LANGUPDATECOORDINATES$", "RadioCoordinatesUpdatedM")
      if briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID ~= nil then
        briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID)
        briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID = event.idx
    elseif briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID then
      briefingRoom.radioManager.play("SEAD: $LANGDISCARDCOORDINATES$", "RadioCoordinatesDiscardedM")
      briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID = nil
    end
  end
end

-- Radio command to launch bombing run (called from F10 menu)
function briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.launchBombingRun()
  briefingRoom.radioManager.play("$LANGPILOT$: $LANGSEADREQUEST$", "RadioPilotSEADSupport")

  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID then
      local group = Group.getByName(briefingRoom.mission.missionFeatures.groupNames.friendlyTaskableSEAD)
      if group ~= nil then
        group:activate()
        local Start = {
          id = 'Start',
          params = {
          }
        }
        group:getController():setCommand(Start)
        timer.scheduleFunction(briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.setTask, {}, timer.getTime() + 10) --just re-run after 10 s
        briefingRoom.radioManager.play("$LANGCOMMAND$: $LANGSEADAFFIRM$", "RadioHQSEADSupport", briefingRoom.radioManager.getAnswerDelay(), nil, nil)
      end
      return
    end
  end

  briefingRoom.radioManager.play("SEAD: $LANGSEADNOCOORDINATES$", "RadioArtilleryNoCoordinates", briefingRoom.radioManager.getAnswerDelay())
end

function briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.setTask()
  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID then
      local group = Group.getByName(briefingRoom.mission.missionFeatures.groupNames.friendlyTaskableSEAD)
      if group ~= nil then
        local currPos = mist.getLeadPos(group)
        local newTask = {
          id = 'Mission',
          airborne = true,
          params = {
            route = {
              points = {
                [1] = {
                  speed = 200,
                  x = dcsExtensions.lerp(currPos.x, m.pos.x,0.9),
                  y = dcsExtensions.lerp(currPos.z, m.pos.z,0.9),
                  type = 'Turning Point',
                  ETA_locked = false,
                  ETA = 100,
                  alt = 7620,
                  alt_type = "BARO",
                  speed_locked = false,
                  action = "Fly Over Point",
                  name = "SEAD",
                  task = {
                    id = "ComboTask",
                    params = {
                      tasks = {
                        [1] = {
                          enabled = true,
                          auto = false,
                          id = "EngageTargetsInZone",
                          number = 1,
                          params = {
                            y = m.pos.z,
                            x = m.pos.x,
                            targetTypes = {
                              [1] = "Air Defence",
                            },
                            value = "Air Defence;",
                            noTargetTypes = {
                            },
                            priority = 0,
                            zoneRadius = 5000,
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
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Begin SEAD on provided coordinates", briefingRoom.f10Menu.missionMenu, briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.launchBombingRun, nil)

-- Enable event handler
world.addEventHandler(briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.eventHandler)
