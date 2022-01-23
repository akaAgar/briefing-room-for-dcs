briefingRoom.mission.objectiveTimers[$OBJECTIVEINDEX$] = function()
  if briefingRoom.mission.objectives[$OBJECTIVEINDEX$].complete then return false end -- Objective complete, nothing to do
  for __,u in ipairs(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID) do
    local unit = dcsExtensions.getUnitByID(u)
    if unit == nil then
      unit = dcsExtensions.getStaticByID(u)
    end
    if unit ~= nil then
      local vec2p = briefingRoom.mission.objectives[$OBJECTIVEINDEX$].waypoint
      local vec2u = dcsExtensions.toVec2(unit:getPoint())
      local distance = dcsExtensions.getDistance(vec2p, vec2u);
      if distance < 200 and not unit:inAir() then -- less than 5nm away on the X/Z axis, less than 8000 feet of altitude difference
          briefingRoom.radioManager.play("Pilot: Command, Cargo Delivered.", "RadioPilotCargoDelivered")
        table.removeValue(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID, u)
        if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID < 1 then -- all target units destroyed, objective complete
          briefingRoom.mission.coreFunctions.completeObjective($OBJECTIVEINDEX$)
        end
      end
    end
  end
end
