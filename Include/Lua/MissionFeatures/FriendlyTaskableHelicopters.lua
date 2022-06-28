briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters = { }
briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.AUTO_AIM_RADIUS = 1000 -- in meters
briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.MARKER_NAME = "helo"
briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID = nil -- ID of the mark on the map
briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.disableCooRemovedRadioMessage = false

briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.eventHandler = { }
function briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.eventHandler:onEvent(event)
  if event.id == world.event.S_EVENT_MARK_REMOVED then
    if briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID then
      if not briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.disableCooRemovedRadioMessage then
        briefingRoom.radioManager.play("Attack Choppers: Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID = nil
    end
  elseif event.id == world.event.S_EVENT_MARK_ADDED then
    local markText = string.lower(tostring(event.text or ""))
    if markText == briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.MARKER_NAME then
      if briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID ~= nil then
        briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID)
        briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID = event.idx
      briefingRoom.radioManager.play("Attack Choppers: Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      return
    end
  elseif event.id == world.event.S_EVENT_MARK_CHANGE then
    local markText = string.lower(tostring(event.text or ""))

    if markText == briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.MARKER_NAME then
      briefingRoom.radioManager.play("Attack Choppers: Copy, coordinates updated.", "RadioCoordinatesUpdatedM")
      if briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID ~= nil then
        briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.disableCooRemovedRadioMessage = true
        trigger.action.removeMark(briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID)
        briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.disableCooRemovedRadioMessage = false
      end
      briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID = event.idx
    elseif briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID ~= nil and event.idx == briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID then
      briefingRoom.radioManager.play("Attack Choppers: Affirm, coordinates discarded. Awaiting new coordinates.", "RadioCoordinatesDiscardedM")
      briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID = nil
    end
  end
end

-- Radio command to launch bombing run (called from F10 menu)
function briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.launchBombingRun()
  briefingRoom.radioManager.play("Pilot: Attack Choppers, begin your attack.", "RadioPilotBeginYourAttack")
  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID then
      local group = Group.getByName(briefingRoom.mission.missionFeatures.groupNames.friendlyTaskableHelicopters)
      if group ~= nil then
        group:activate()
        local Start = {
          id = 'Start',
          params = {
          }
        }
        group:getController():setCommand(Start)
        timer.scheduleFunction(briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.setTask, {}, timer.getTime() + 10) --just re-run after 10 s
        briefingRoom.radioManager.play("Attack Choppers: Copy, beginning my attack.", "RadioOtherPilotBeginAttack", briefingRoom.radioManager.getAnswerDelay(), nil, nil)
      end
      return
    end
  end

  briefingRoom.radioManager.play("Attack Choppers: Cannot comply. No coordinates provided for patrol run (add a marker named \""..string.upper(briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.MARKER_NAME).."\" on the F10 map to designate a target).", "RadioArtilleryNoCoordinates", briefingRoom.radioManager.getAnswerDelay())
end

function briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.setTask()
  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.markID then
      local group = Group.getByName(briefingRoom.mission.missionFeatures.groupNames.friendlyTaskableHelicopters)
      if group ~= nil then
        local currPos = mist.getLeadPos(group)
        local newTask = {
          id = 'Mission',
          airborne = true,
          params = {
            route = {
              points = {
                [1] = {
                  speed = 100,
                  x = dcsExtensions.lerp(currPos.x, m.pos.x,0.9),
                  y = dcsExtensions.lerp(currPos.z, m.pos.z,0.9),
                  type = 'Turning Point',
                  ETA_locked = false,
                  ETA = 100,
                  alt = 152.4,
                  alt_type = "RADIO",
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
                            altitude = 152.4,
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
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Begin Helo attack on provided coordinates", briefingRoom.f10Menu.missionMenu, briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.launchBombingRun, nil)


-- Enable event handler
world.addEventHandler(briefingRoom.mission.missionFeatures.friendlyTaskableHelicopters.eventHandler)
