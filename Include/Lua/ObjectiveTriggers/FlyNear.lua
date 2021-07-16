briefingRoom.mission.objectiveTimers[$OBJECTIVEINDEX$] = function()
  if briefingRoom.mission.objectives[$OBJECTIVEINDEX$].complete then return false end -- Objective complete, nothing to do

  local players = dcsExtensions.getAllPlayers()

  for _,p in ipairs(players) do
    for __,u in ipairs(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID) do
      local unit = dcsExtensions.getUnitByID(u)
      if unit ~= nil then
        local vec2p = dcsExtensions.toVec2(p:getPoint())
        local vec2u = dcsExtensions.toVec2(unit:getPoint())
        local distance = dcsExtensions.getDistance(vec2p, vec2u);

        if distance < 9260 and math.abs(vec2p.y - vec2u.y) < 2438 then -- less than 5nm away on the X/Z axis, less than 8000 feet of altitude difference
          if math.random(1, 2) == 1 then
            briefingRoom.radioManager.play("Command, I have a good visual on target.", "RadioPilotTargetReconned1")
          else
            briefingRoom.radioManager.play("Command, positive visual on target.", "RadioPilotTargetReconned2")
          end
          briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID = { }
          briefingRoom.mission.coreFunctions.completeObjective($OBJECTIVEINDEX$)
        end
      end
    end
  end
end

briefingRoom.mission.objectives[$OBJECTIVEINDEX$].hideTargetCount = true
