briefingRoom.mission.objectiveTimers[$OBJECTIVEINDEX$] = function()
  if briefingRoom.mission.objectives[$OBJECTIVEINDEX$].complete then return false end -- Objective complete, nothing to do

  local players = dcsExtensions.getAllPlayers()

  for _,p in ipairs(players) do
    for __,u in ipairs(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames) do
      local unit = Unit.getByName(u)
      if unit ~= nil then
        local vec2p = dcsExtensions.toVec2(p:getPoint())
        local vec2u = dcsExtensions.toVec2(unit:getPoint())
        local distance = dcsExtensions.getDistance(vec2p, vec2u);

        if distance < 3704 and math.abs(vec2p.y - vec2u.y) < 609.6 and p:inAir() then -- less than 2nm away on the X/Z axis, less than 2000 feet of altitude difference
          if math.random(1, 2) == 1 then
            briefingRoom.radioManager.play("$LANG_PILOT$: $LANG_FLYNEAR1$", "RadioPilotTargetReconned1")
          else
            briefingRoom.radioManager.play("$LANG_PILOT$: $LANG_FLYNEAR2$", "RadioPilotTargetReconned2")
          end
          briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames = { }
          briefingRoom.mission.coreFunctions.completeObjective($OBJECTIVEINDEX$)
          return nil
        end
      end
    end
  end
end

briefingRoom.mission.objectives[$OBJECTIVEINDEX$].hideTargetCount = true
