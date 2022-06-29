briefingRoom.mission.objectiveTriggers[$OBJECTIVEINDEX$] = function(event)
  if briefingRoom.mission.complete then return false end -- Mission complete, nothing to do
  if briefingRoom.mission.objectives[$OBJECTIVEINDEX$].complete then return false end -- Objective complete, nothing to do
  if event.id ~= world.event.S_EVENT_LAND then return false end -- Not a "land" event, nothing to do

  if event.initiator == nil then return false end -- Initiator was nil
  if Object.getCategory(event.initiator) ~= Object.Category.UNIT then return false end -- Initiator was not an unit
  if event.initiator:getCoalition() ~= $LUAPLAYERCOALITION$ then return false end -- Initiator was not a friendy unit

  local position = dcsExtensions.toVec2(event.initiator:getPoint()) -- get the landing unit position
 
  -- Drop off
  local distanceToObjective = dcsExtensions.getDistance(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].waypoint, position);
  if distanceToObjective < 500 then
    local removed = briefingRoom.transportManager.removeTroopCargo(event.initiator:getName(), briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames)
    for index, value in ipairs(removed) do
      table.removeValue(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames, value)
    end
    if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames < 1 then -- all target units destroyed, objective complete
      briefingRoom.radioManager.play("Pilot: Command, Troops Delivered.", "RadioPilotTroopsDelivered")
      briefingRoom.mission.coreFunctions.completeObjective($OBJECTIVEINDEX$)
    end
    return true
  end

  local collect = {}
  -- Pickup
  for _,id in ipairs(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames) do
    local targetUnit = Unit.getByName(id)
    if targetUnit ~= nil then
      local targetPosition = dcsExtensions.toVec2(targetUnit:getPoint())
      if dcsExtensions.getDistance(position, targetPosition) < 500 then
        table.insert(collect, id)
      end
    end
  end
  if #collect > 0 then
    briefingRoom.transportManager.addTroopCargo(event.initiator:getName(), collect)
  end
end
