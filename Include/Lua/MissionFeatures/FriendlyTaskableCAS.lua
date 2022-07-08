briefingRoom.mission.missionFeatures.friendlyTaskableCAS = { }
briefingRoom.mission.missionFeatures.friendlyTaskableCAS.AUTO_AIM_RADIUS = 1000 -- in meters
briefingRoom.mission.missionFeatures.friendlyTaskableCAS.MARKER_NAME = "cas"
briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID = nil -- ID of the mark on the map
briefingRoom.mission.missionFeatures.friendlyTaskableCAS.disableCooRemovedRadioMessage = false

briefingRoom.mission.missionFeatures.friendlyTaskableCAS.eventHandler = { }
function briefingRoom.mission.missionFeatures.friendlyTaskableCAS.eventHandler:onEvent(event)
  if event.id == world.event.S_EVENT_MARK_REMOVED then
    if briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID then
      if not briefingRoom.mission.missionFeatures.friendlyTaskableCAS.disableCooRemovedRadioMessage then
        briefingRoom.radioManager.play("$LANGCAS$: $LANGDISCARDCOORDINATES$", "RadioCoordinatesDiscardedM")
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID = nil
    end
  elseif event.id == world.event.S_EVENT_MARK_ADDED then
    local markText = string.lower(tostring(event.text or ""))
    if markText == briefingRoom.mission.missionFeatures.friendlyTaskableCAS.MARKER_NAME then
      if briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID ~= nil then
        briefingRoom.mission.missionFeatures.friendlyTaskableCAS.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID)
        briefingRoom.mission.missionFeatures.friendlyTaskableCAS.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID = event.idx
      briefingRoom.radioManager.play("$LANGCAS$: $LANGUPDATECOORDINATES$", "RadioCoordinatesUpdatedM")
      return
    end
  elseif event.id == world.event.S_EVENT_MARK_CHANGE then
    local markText = string.lower(tostring(event.text or ""))

    if markText == briefingRoom.mission.missionFeatures.friendlyTaskableCAS.MARKER_NAME then
      briefingRoom.radioManager.play("$LANGCAS$: $LANGUPDATECOORDINATES$", "RadioCoordinatesUpdatedM")
      if briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID ~= nil then
        briefingRoom.mission.missionFeatures.friendlyTaskableCAS.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID)
        briefingRoom.mission.missionFeatures.friendlyTaskableCAS.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID = event.idx
    elseif briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID then
      briefingRoom.radioManager.play("$LANGCAS$: $LANGDISCARDCOORDINATES$", "RadioCoordinatesDiscardedM")
      briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID = nil
    end
  end
end

-- Radio command to launch bombing run (called from F10 menu)
function briefingRoom.mission.missionFeatures.friendlyTaskableCAS.launchBombingRun()
  briefingRoom.radioManager.play("$LANGPILOT$: $LANGCASREQUEST$", "RadioPilotBeginYourAttack")
  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID then
      local group = Group.getByName(briefingRoom.mission.missionFeatures.groupNames.friendlyTaskableCAS)
      if group ~= nil then
        group:activate()
        local Start = {
          id = 'Start',
          params = {
          }
        }
        group:getController():setCommand(Start)
        timer.scheduleFunction(briefingRoom.mission.missionFeatures.friendlyTaskableCAS.setTask, {}, timer.getTime() + 10) --just re-run after 10 s
        briefingRoom.radioManager.play("$LANGCAS$: $LANGBEGINATTACK$", "RadioOtherPilotBeginAttack", briefingRoom.radioManager.getAnswerDelay(), nil, nil)
      end
      return
    end
  end

  briefingRoom.radioManager.play("$LANGCAS$: $LANGCASNOCOORDINATES$", "RadioArtilleryNoCoordinates", briefingRoom.radioManager.getAnswerDelay())
end

function briefingRoom.mission.missionFeatures.friendlyTaskableCAS.setTask()
  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID then
      local group = Group.getByName(briefingRoom.mission.missionFeatures.groupNames.friendlyTaskableCAS)
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
                  x = dcsExtensions.lerp(currPos.x, m.pos.x,0.85),
                  y = dcsExtensions.lerp(currPos.z, m.pos.z,0.85),
                  type = 'Turning Point',
                  ETA_locked = false,
                  ETA = 100,
                  alt = 1524,
                  alt_type = "BARO",
                  speed_locked = false,
                  action = "Fly Over Point",
                  name = "CAS",
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
                              [1] = "All",
                            },
                            value = "All;",
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
                            altitude = 2000,
                            pattern = "Circle",
                            speed = 100,
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
        group:getController():setTask(newTask)
      end
      return
    end
  end
end




-- Add F10 menu command
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Begin CAS on provided coordinates", briefingRoom.f10Menu.missionMenu, briefingRoom.mission.missionFeatures.friendlyTaskableCAS.launchBombingRun, nil)


-- Enable event handler
world.addEventHandler(briefingRoom.mission.missionFeatures.friendlyTaskableCAS.eventHandler)
