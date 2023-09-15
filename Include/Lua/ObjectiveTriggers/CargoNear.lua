briefingRoom.mission.objectiveTimers[$OBJECTIVEINDEX$] = function()
  if briefingRoom.mission.objectives[$OBJECTIVEINDEX$].complete then return false end -- Objective complete, nothing to do
  for __,u in ipairs(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames) do
    local unit = dcsExtensions.getUnitOrStatic(u)
    if unit ~= nil then
      local vec2p = briefingRoom.mission.objectives[$OBJECTIVEINDEX$].waypoint
      local vec2u = dcsExtensions.toVec2(unit:getPoint())
      local distance = dcsExtensions.getDistance(vec2p, vec2u);
      if distance < 500 and not unit:inAir() then
        briefingRoom.radioManager.play("$LANG_PILOT$: $LANG_CARGODELIVERED$", "RadioPilotCargoDelivered")
        table.removeValue(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames, u)
        if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames < 1 then -- all target units destroyed, objective complete
          briefingRoom.mission.coreFunctions.completeObjective($OBJECTIVEINDEX$)
        end
      end
    end
  end
end
