briefingRoom.mission.missionFeatures.friendlyTaskableSEAD = {}
briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.MARKER_NAME = "sead"
briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.LANG_UNIT = "$LANG_SEAD$"
briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.LANG_REQEUEST = "$LANG_SEADREQUEST$"
briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.RADIO_REQEUEST = "RadioPilotSEADSupport"
briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.LANG_AFFIRM = "$LANG_SEADAFFIRM$"
briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.RADIO_AFFRIM = "RadioHQSEADSupport"
briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.LANG_NO_COORDS = "$LANG_SEADNOCOORDINATES$"
briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.markID = nil -- ID of the mark on the map
briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.disableCooRemovedRadioMessage = false


-- Radio command to launch bombing run (called from F10 menu)
function briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.launchBombingRun()
  briefingRoom.taskables.launchCurry(briefingRoom.mission.missionFeatures.friendlyTaskableSEAD,
    briefingRoom.mission.missionFeatures.groupNames.friendlyTaskableSEAD, function(group, mark)
    local currPos = mist.getLeadPos(group)
    return {
      id = 'Mission',
      airborne = true,
      params = {
        route = {
          points = {
            [1] = {
              speed = 200,
              x = dcsExtensions.lerp(currPos.x, mark.pos.x, 0.9),
              y = dcsExtensions.lerp(currPos.z, mark.pos.z, 0.9),
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
                        y = mark.pos.z,
                        x = mark.pos.x,
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
                        pattern = "Circle",
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
  end)
end

-- Add F10 menu command
missionCommands.addCommandForCoalition(briefingRoom.playerCoalition, "$LANG_SEADMENU$", briefingRoom.f10Menu.missionMenu
  , briefingRoom.mission.missionFeatures.friendlyTaskableSEAD.launchBombingRun, nil)

-- Enable event handler
world.addEventHandler(briefingRoom.taskables.markerManager(briefingRoom.mission.missionFeatures.friendlyTaskableSEAD))
