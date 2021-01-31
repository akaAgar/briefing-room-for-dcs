briefingRoom.mission.features.objectiveOverflyWaypoint = {}
briefingRoom.mission.features.objectiveOverflyWaypoint.MAX_ALTITUDE = 1524 -- in meters (5000 feet)
briefingRoom.mission.features.objectiveOverflyWaypoint.MAX_DISTANCE = 1852 -- in meters (1 nautical mile)

function briefingRoom.mission.features.objectiveOverflyWaypoint.checkOverfly(player, wpCoordinates)
  if player == nil or wpCoordinates == nil then
    return false
  end

  local vec2p = dcsExtensions.toVec2(player:getPoint())
  local distance = dcsExtensions.getDistance(vec2p, wpCoordinates);

  if distance > briefingRoom.mission.features.objectiveOverflyWaypoint.MAX_DISTANCE then
    return false
  end
  
  return true
end

function briefingRoom.mission.features.objectiveOverflyWaypoint.update(args, time)
   -- mission already complete/failed, nothing to do, return nil to stop updating
  if briefingRoom.mission.status ~= brMissionStatus.IN_PROGRESS then
    return nil
  end

  local players = dcsExtensions.getAllPlayers()

  for index,objective in ipairs(briefingRoom.mission.objectives) do
    if objective.status == brMissionStatus.IN_PROGRESS then
        for _,p in ipairs(players) do
            if briefingRoom.mission.features.objectiveOverflyWaypoint.checkOverfly(p, objective.coordinates) then
              if math.random(1, 2) == 1 then
                briefingRoom.radioManager.play("Command, I have a good visual on target.", "RadioPilotTargetReconned1")
              else
                briefingRoom.radioManager.play("Command, positive visual on target.", "RadioPilotTargetReconned2")
              end
              briefingRoom.mission.functions.completeObjective(index)
              briefingRoom.mission.objectives[index].unitsID = { }
              return time + 1
          end
      end
    end
  end

  return time + 1
end

-- Begin updating WP overfly check
timer.scheduleFunction(briefingRoom.mission.features.objectiveOverflyWaypoint.update, nil, timer.getTime() + 1)
