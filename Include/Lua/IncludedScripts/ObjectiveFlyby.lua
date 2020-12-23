briefingRoom.mission.extensions.objectiveFlyby = {}
briefingRoom.mission.extensions.objectiveFlyby.MAX_ALTITUDE_GROUND_RECON = 1524 -- in meters (5000 feet)
briefingRoom.mission.extensions.objectiveFlyby.MAX_ALT_DISTANCE_AIR_RECON = 152 -- in meters (500 feet)
briefingRoom.mission.extensions.objectiveFlyby.MAX_DISTANCE = 500 -- in meters

function briefingRoom.mission.extensions.objectiveFlyby.checkFlyby(player, unit)
  if player == nil or unit == nil then
    return false
  end

  local vec2p = dcsExtensions.toVec2(player:getPoint())
  local vec2u = dcsExtensions.toVec2(unit:getPoint())
  local distance = dcsExtensions.getDistance(vec2p, vec2u);

  if distance > briefingRoom.mission.extensions.objectiveFlyby.MAX_DISTANCE then
    return false
  end

  local unitCategory = unit:getCategory()

  if unitCategory == Unit.Category.AIRPLANE or unitCategory == Unit.Category.HELICOPTER then
    if math.abs(player:getPoint().y - unit:getPoint().y) < briefingRoom.mission.extensions.objectiveFlyby.MAX_ALT_DISTANCE_AIR_RECON then
      return true
    end
  else
    if player:getPoint().y - land.getHeight <= briefingRoom.mission.extensions.objectiveFlyby.MAX_ALTITUDE_GROUND_RECON then
      return true
    end
  end

  return false
end

function briefingRoom.mission.extensions.objectiveFlyby.update(args, time)
   -- mission already complete/failed, nothing to do, return nil to stop updating
  if briefingRoom.mission.status ~= brMissionStatus.IN_PROGRESS then
    return nil
  end

  local players = dcsExtensions.getAllPlayers()

  for index,objective in ipairs(briefingRoom.mission.objectives) do
    if objective.status == brMissionStatus.IN_PROGRESS then
      if table.contains(objective.unitsID, unitID) then
        for _,p in ipairs(players) do
          for __,u in ipairs(objective.unitsID) do
            if briefingRoom.mission.extensions.objectiveFlyby.checkFlyby(p, u) then
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
    end
  end

  return time + 1
end

-- Begin updating flyby check
timer.scheduleFunction(briefingRoom.mission.extensions.objectiveFlyby.update, nil, timer.getTime() + 1)
