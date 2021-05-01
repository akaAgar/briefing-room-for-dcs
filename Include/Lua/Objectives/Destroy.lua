function briefingRoom.mission.functions.onUnitDestroyed(unitID)
  if briefingRoom.mission.status ~= brMissionStatus.IN_PROGRESS then return end -- mission already complete/failed, nothing to do
  
  for index,objective in ipairs(briefingRoom.mission.objectives) do
    if objective.status == brMissionStatus.IN_PROGRESS then
      if table.contains(briefingRoom.mission.objectives[index].unitsID, unitID) then
        table.removeValue(briefingRoom.mission.objectives[index].unitsID, unitID)

        if #briefingRoom.mission.objectives[index].unitsID < 1 then
          briefingRoom.mission.functions.completeObjective(index)
        end
      end
    end
  end
end
