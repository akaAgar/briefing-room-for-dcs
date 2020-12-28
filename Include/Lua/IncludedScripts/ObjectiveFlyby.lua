briefingRoom.mission.features.objectiveFlyby = {}
briefingRoom.mission.features.objectiveFlyby.MAX_ALTITUDE_GROUND_RECON = 1524 -- in meters (5000 feet)
briefingRoom.mission.features.objectiveFlyby.MAX_ALT_DISTANCE_AIR_RECON = 152 -- in meters (500 feet)
briefingRoom.mission.features.objectiveFlyby.MAX_DISTANCE = 1000 -- in meters

function briefingRoom.mission.features.objectiveFlyby.checkFlyby(player, unit)
  if player == nil or unit == nil then
    return false
  end

  local vec2p = dcsExtensions.toVec2(player:getPoint())
  local vec2u = dcsExtensions.toVec2(unit:getPoint())
  local distance = dcsExtensions.getDistance(vec2p, vec2u);

  if distance > briefingRoom.mission.features.objectiveFlyby.MAX_DISTANCE then
    return false
  end

  -- local unitCategory = unit:getCategory()

  -- if unitCategory == Unit.Category.AIRPLANE or unitCategory == Unit.Category.HELICOPTER then
  --   if math.abs(player:getPoint().y - unit:getPoint().y) < briefingRoom.mission.features.objectiveFlyby.MAX_ALT_DISTANCE_AIR_RECON then
  --     return true
  --   end
  -- else
  --   if player:getPoint().y - land.getHeight(vec2p) <= briefingRoom.mission.features.objectiveFlyby.MAX_ALTITUDE_GROUND_RECON then
  --     return true
  --   end
  -- end

  -- local altitudeDifference = briefingRoom.mission.features.objectiveFlyby.MAX_ALTITUDE_GROUND_RECON
  -- if unitCategory == Unit.Category.AIRPLANE or unitCategory == Unit.Category.HELICOPTER then
  --   altitudeDifference = briefingRoom.mission.features.objectiveFlyby.MAX_ALT_DISTANCE_AIR_RECON
  -- end

  -- if math.abs(player:getPoint().y - unit:getPoint().y) < altitudeDifference then
  --   return true
  -- end

  return true
  -- return false
end

function briefingRoom.mission.features.objectiveFlyby.update(args, time)
   -- mission already complete/failed, nothing to do, return nil to stop updating
  if briefingRoom.mission.status ~= brMissionStatus.IN_PROGRESS then
    return nil
  end

  local players = dcsExtensions.getAllPlayers()

  for index,objective in ipairs(briefingRoom.mission.objectives) do
    if objective.status == brMissionStatus.IN_PROGRESS then
      if #objective.unitsID > 0 then
        for _,p in ipairs(players) do
          for __,u in ipairs(objective.unitsID) do
            if briefingRoom.mission.features.objectiveFlyby.checkFlyby(p, dcsExtensions.getUnitByID(u)) then
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
timer.scheduleFunction(briefingRoom.mission.features.objectiveFlyby.update, nil, timer.getTime() + 1)
