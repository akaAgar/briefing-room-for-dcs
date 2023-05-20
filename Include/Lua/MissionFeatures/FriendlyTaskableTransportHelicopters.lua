briefingRoom.mission.missionFeatures.friendlyTaskableTransportHelicopters = {}
briefingRoom.mission.missionFeatures.friendlyTaskableTransportHelicopters.MARKER_NAME = "transport"
briefingRoom.mission.missionFeatures.friendlyTaskableTransportHelicopters.LANG_UNIT = "$LANG_TRANSPORTCHOPPERS$"
briefingRoom.mission.missionFeatures.friendlyTaskableTransportHelicopters.LANG_REQEUEST =
"$LANG_TRANSPORTCHOPPERSREQUEST$"
briefingRoom.mission.missionFeatures.friendlyTaskableTransportHelicopters.RADIO_REQEUEST = "RadioPilotToLZ"
briefingRoom.mission.missionFeatures.friendlyTaskableTransportHelicopters.LANG_AFFIRM = "$LANG_TRANSPORTCHOPPERSAFFIRM$"
briefingRoom.mission.missionFeatures.friendlyTaskableTransportHelicopters.RADIO_AFFIRM = "RadioOtherPilotToLZ"
briefingRoom.mission.missionFeatures.friendlyTaskableTransportHelicopters.LANG_NO_COORDS =
"$LANG_TRANSPORTCHOPPERSNOCOORDINATES$"
briefingRoom.mission.missionFeatures.friendlyTaskableTransportHelicopters.markID = nil -- ID of the mark on the map
briefingRoom.mission.missionFeatures.friendlyTaskableTransportHelicopters.disableCooRemovedRadioMessage = false


-- Radio command to launch bombing run (called from F10 menu)
function briefingRoom.mission.missionFeatures.friendlyTaskableTransportHelicopters.launchBombingRun()
  briefingRoom.taskables.launchCurry(briefingRoom.mission.missionFeatures.friendlyTaskableTransportHelicopters,
    briefingRoom.mission.missionFeatures.groupNames.friendlyTaskableTransportHelicopters, function(group, mark)
      local currPos = mist.getLeadPos(group)
      return {
        id = 'Mission',
        airborne = true,
        params = {
          route = {
            points = {
              [1] = {
                speed = 100,
                x = mark.pos.x,
                y = mark.pos.z,
                type = 'Turning Point',
                ETA_locked = false,
                ETA = 100,
                alt = 152.4,
                alt_type = "RADIO",
                speed_locked = false,
                action = "Turning Point",
                name = "CAS",
                task = {
                  id = "ComboTask",
                  params = {
                    tasks = {
                      [1] = {
                        enabled = true,
                        auto = false,
                        id = "Land",
                        number = 1,
                        params = {
                          y = mark.pos.z,
                          x = mark.pos.x,
                          duration = 300,
                          durationFlag = false,
                        },
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
missionCommands.addCommandForCoalition(briefingRoom.playerCoalition, "$LANG_TRANSPORTCHOPPERMENU$",
  briefingRoom.f10Menu.missionMenu,
  briefingRoom.mission.missionFeatures.friendlyTaskableTransportHelicopters.launchBombingRun,
  nil)


-- Enable event handler
world.addEventHandler(briefingRoom.taskables.markerManager(briefingRoom.mission.missionFeatures
.friendlyTaskableTransportHelicopters))
