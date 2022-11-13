briefingRoom.mission.missionFeatures.friendlyTaskableCAS = {}
briefingRoom.mission.missionFeatures.friendlyTaskableCAS.AUTO_AIM_RADIUS = 1000 -- in meters
briefingRoom.mission.missionFeatures.friendlyTaskableCAS.MARKER_NAME = "cas"
briefingRoom.mission.missionFeatures.friendlyTaskableCAS.LANG_UNIT = "$LANG_CAS$"
briefingRoom.mission.missionFeatures.friendlyTaskableCAS.LANG_REQEUEST = "$LANG_CASREQUEST$"
briefingRoom.mission.missionFeatures.friendlyTaskableCAS.RADIO_REQEUEST = "RadioPilotBeginYourAttack"
briefingRoom.mission.missionFeatures.friendlyTaskableCAS.LANG_AFFIRM = "$LANG_BEGINATTACK$"
briefingRoom.mission.missionFeatures.friendlyTaskableCAS.RADIO_AFFRIM = "RadioOtherPilotBeginAttack"
briefingRoom.mission.missionFeatures.friendlyTaskableCAS.LANG_NO_COORDS = "$LANG_CASNOCOORDINATES$"
briefingRoom.mission.missionFeatures.friendlyTaskableCAS.markID = nil -- ID of the mark on the map
briefingRoom.mission.missionFeatures.friendlyTaskableCAS.disableCooRemovedRadioMessage = false


-- Radio command to launch bombing run (called from F10 menu)
function briefingRoom.mission.missionFeatures.friendlyTaskableCAS.launchBombingRun()
  briefingRoom.taskables.launchCurry(briefingRoom.mission.missionFeatures.friendlyTaskableCAS,
    briefingRoom.mission.missionFeatures.groupNames.friendlyTaskableCAS, function(group, mark)
    local currPos = mist.getLeadPos(group)
    return {
      id = 'Mission',
      airborne = true,
      params = {
        route = {
          points = {
            [1] = {
              speed = 200,
              x = dcsExtensions.lerp(currPos.x, mark.pos.x, 0.85),
              y = dcsExtensions.lerp(currPos.z, mark.pos.z, 0.85),
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
                        y = mark.pos.z,
                        x = mark.pos.x,
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
                        pattern = "Circle",
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
  end)
end

-- Add F10 menu command
missionCommands.addCommandForCoalition(briefingRoom.playerCoalition, "$LANG_CASMENU$", briefingRoom.f10Menu.missionMenu,
  briefingRoom.mission.missionFeatures.friendlyTaskableCAS.launchBombingRun, nil)


-- Enable event handler
world.addEventHandler(briefingRoom.taskables.markerManager(briefingRoom.mission.missionFeatures.friendlyTaskableCAS))
