-- Triggers the completion of objective $OBJECTIVEINDEX$ when a friendly helicopter lands near the target
briefingRoom.mission.objectiveTriggers[$OBJECTIVEINDEX$] = function(event)
    if briefingRoom.mission.complete then return false end -- Mission complete, nothing to do
    if briefingRoom.mission.objectives[$OBJECTIVEINDEX$].complete then return false end -- Objective complete, nothing to do
    if event.id ~= world.event.S_EVENT_LAND then return false end -- Not a "land" event, nothing to do
  
    if event.initiator == nil then return false end -- Initiator was nil
    if Object.getCategory(event.initiator) ~= Object.Category.UNIT then return false end -- Initiator was not an unit
    if event.initiator:getCoalition() ~= $LUAPLAYERCOALITION$ then return false end -- Initiator was not a friendy unit

    local position = dcsExtensions.toVec2(event.initiator:getPoint()) -- get the landing unit position

    -- check if any target unit is close enough from the landing unit
    -- if so, clean the target unit ID table and mark the objective as completed
    for _,id in ipairs(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames) do
      local targetUnit = Unit.getByName(id)
      if targetUnit ~= nil then
        local targetPosition = dcsExtensions.toVec2(targetUnit:getPoint())
        if dcsExtensions.getDistance(position, targetPosition) < 650 then
          briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames = { }
          briefingRoom.mission.coreFunctions.completeObjective($OBJECTIVEINDEX$)
          return true
        end
      end
    end
end

briefingRoom.mission.objectives[$OBJECTIVEINDEX$].hideTargetCount = true
