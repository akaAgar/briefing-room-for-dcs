-- Triggers the completion of objective $OBJECTIVEINDEX$ when player flies near/other the target
briefingRoom.mission.objectiveTriggers[$OBJECTIVEINDEX$] = function(event)
  return false
end

function briefingRoom.mission.objectiveTriggersCommon.flyByCheck(player, unit)
  if player == nil or unit == nil then
    return false
  end

  local vec2p = dcsExtensions.toVec2(player:getPoint())
  local vec2u = dcsExtensions.toVec2(unit:getPoint())
  local distance = dcsExtensions.getDistance(vec2p, vec2u);

  if distance > briefingRoom.mission.features.objectiveFlyby.MAX_DISTANCE then
    return false
  end
end

function briefingRoom.mission.objectiveTriggersCommon.flyBy(args, time)
  -- mission already complete, nothing to do, return nil to stop updating
  if briefingRoom.mission.complete then return nil end

  local players = dcsExtensions.getAllPlayers()

  for index,objective in ipairs(briefingRoom.mission.objectives) do
    if not objective.complete then
      if #objective.unitsID > 0 then
        for _,p in ipairs(players) do
          for __,u in ipairs(objective.unitsID) do
            if briefingRoom.mission.objectiveTriggersCommon.flyByCheck(p, dcsExtensions.getUnitByID(u)) then
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
if briefingRoom.mission.objectiveTriggersCommon.flyByScheduled == nil then
  timer.scheduleFunction(briefingRoom.mission.objectiveTriggersCommon.flyBy, nil, timer.getTime() + 1)
  briefingRoom.mission.objectiveTriggersCommon.flyByScheduled = true
end
