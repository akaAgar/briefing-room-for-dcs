briefingRoom.mission.missionFeatures.supportArtillery = { }
briefingRoom.mission.missionFeatures.supportArtillery.FIRE_MISSIONS_PER_OBJECTIVE = 3
briefingRoom.mission.missionFeatures.supportArtillery.AUTO_AIM_RADIUS = 1000 -- in meters
briefingRoom.mission.missionFeatures.supportArtillery.INACCURACY = 500 -- in meters
briefingRoom.mission.missionFeatures.supportArtillery.MARKER_NAME = "arty"
briefingRoom.mission.missionFeatures.supportArtillery.LANG_UNIT = "$LANG_FIRESUPPORT$"
briefingRoom.mission.missionFeatures.supportArtillery.SHELLS_PER_FIRE_MISSION = 10
briefingRoom.mission.missionFeatures.supportArtillery.fireMissionsLeft = 0
briefingRoom.mission.missionFeatures.supportArtillery.markID = nil -- ID of the mark on the map
briefingRoom.mission.missionFeatures.supportArtillery.shellsLeftInFireMission = 0
briefingRoom.mission.missionFeatures.supportArtillery.disableCooRemovedRadioMessage = false


function briefingRoom.mission.missionFeatures.supportArtillery.doShell(args, time)
  briefingRoom.mission.missionFeatures.supportArtillery.shellsLeftInFireMission = briefingRoom.mission.missionFeatures.supportArtillery.shellsLeftInFireMission - 1
  
  for i=1,3 do
    local impactPoint = args.position
    local impactPointV2 = dcsExtensions.toVec2(impactPoint)
    local inaccuracy = math.randomPointInCircle({ ["x"] = 0, ["y"] = 0 }, briefingRoom.mission.missionFeatures.supportArtillery.INACCURACY)
    impactPoint.x = impactPoint.x + inaccuracy.x
    impactPoint.y = impactPoint.y + 0.5
    impactPoint.z = impactPoint.z + inaccuracy.y

    if i == 1 then
      local enemyUnits = dcsExtensions.getCoalitionUnits(briefingRoom.enemyCoalition)
      for _,u in ipairs(enemyUnits) do
        if dcsExtensions.getDistance(dcsExtensions.toVec2(u:getPoint()), impactPointV2) < briefingRoom.mission.missionFeatures.supportArtillery.AUTO_AIM_RADIUS then
          if not u:inAir() then
            impactPoint = u:getPoint()
          end
        end
      end
    end

    trigger.action.explosion(impactPoint, 100)
  end

  if briefingRoom.mission.missionFeatures.supportArtillery.shellsLeftInFireMission <= 0 then -- no shells left in this fire mission
    return nil
  else
    return time + 1
  end
end

-- Internal function to begin executing fire mission (called when radio message is complete)
function briefingRoom.mission.missionFeatures.supportArtillery.doFireMission(args)
  briefingRoom.mission.missionFeatures.supportArtillery.shellsLeftInFireMission = briefingRoom.mission.missionFeatures.supportArtillery.SHELLS_PER_FIRE_MISSION
  timer.scheduleFunction(briefingRoom.mission.missionFeatures.supportArtillery.doShell, args, timer.getTime() + math.random(2,3))
end

-- Radio command to launch fire mission (called from F10 menu)
function briefingRoom.mission.missionFeatures.supportArtillery.launchFireMission()
  briefingRoom.radioManager.play("$LANG_PILOT$: $LANG_FIREREQUEST$", "RadioPilotArtillery")
 
  if briefingRoom.mission.missionFeatures.supportArtillery.fireMissionsLeft <= 0 then
    briefingRoom.radioManager.play("$LANG_FIRESUPPORT$: $LANG_FIREREJECT$", "RadioArtilleryNoAmmo", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  local marks = world.getMarkPanels()
  for _,m in ipairs(marks) do
    if briefingRoom.mission.missionFeatures.supportArtillery.markID ~= nil and m.idx == briefingRoom.mission.missionFeatures.supportArtillery.markID then
      local args = { ["position"] = m.pos }
      briefingRoom.mission.missionFeatures.supportArtillery.fireMissionsLeft = briefingRoom.mission.missionFeatures.supportArtillery.fireMissionsLeft - 1
      briefingRoom.radioManager.play("$LANG_FIRESUPPORT$: $LANG_FIREAFFIRM$", "RadioArtilleryFiring", briefingRoom.radioManager.getAnswerDelay(), briefingRoom.mission.missionFeatures.supportArtillery.doFireMission, args)
      return
    end
  end

  briefingRoom.radioManager.play("$LANG_FIRESUPPORT$: $LANG_FIRENOCOORDINATES$", "RadioArtilleryNoCoordinates", briefingRoom.radioManager.getAnswerDelay())
end

-- Set the correct number of fire missions
briefingRoom.mission.missionFeatures.supportArtillery.fireMissionsLeft = briefingRoom.mission.missionFeatures.supportArtillery.FIRE_MISSIONS_PER_OBJECTIVE * math.max(1, #briefingRoom.mission.objectives)

-- Add F10 menu command
missionCommands.addCommandForCoalition(briefingRoom.playerCoalition, "$LANG_FIREMENU$", briefingRoom.f10Menu.missionMenu, briefingRoom.mission.missionFeatures.supportArtillery.launchFireMission, nil)

-- Enable event handler
world.addEventHandler(briefingRoom.taskables.markerManager(briefingRoom.mission.missionFeatures.supportArtillery))
