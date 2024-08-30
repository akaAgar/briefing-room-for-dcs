-- Triggers the completion of objective $OBJECTIVEINDEX$ when all targets are destroyed
table.insert(briefingRoom.mission.objectiveTriggers, briefingRoom.mission.getDestroyFunction($OBJECTIVEINDEX$))

-- Remove all non-tracking radar units from the targets list (unless there's no tracking radar at all)
do
  local newTargetTable = { }
  
  for _,i in ipairs(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames) do
    local unit = Unit.getByName(i)
    if unit ~= nil and unit:hasAttribute("SAM TR") then
      table.insert(newTargetTable, i)
    end
  end

  if #newTargetTable > 0 then
    briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames = newTargetTable
    briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsCount = #newTargetTable
  end
end
