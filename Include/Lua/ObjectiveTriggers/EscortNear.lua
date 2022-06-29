briefingRoom.mission.objectiveTimers[$OBJECTIVEINDEX$] = function()
  if briefingRoom.mission.objectives[$OBJECTIVEINDEX$].complete then return false end -- Objective complete, nothing to do
  for __,u in ipairs(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames) do
    local unit = Unit.getByName(u)
    if unit == nil then
      unit = StaticObject.getByName(u)
    end
    if unit ~= nil then
      local vec2p = briefingRoom.mission.objectives[$OBJECTIVEINDEX$].waypoint
      local vec2u = dcsExtensions.toVec2(unit:getPoint())
      local distance = dcsExtensions.getDistance(vec2p, vec2u);
      if distance < 500 then -- less than 5nm away on the X/Z axis, less than 8000 feet of altitude difference
        table.removeValue(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames, u)
        if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames < 1 then -- all target units destroyed, objective complete
          briefingRoom.radioManager.play("Pilot: Command, friendly forces have reached their objective.", "RadioPilotEscortComplete")
          briefingRoom.mission.coreFunctions.completeObjective($OBJECTIVEINDEX$)
        end
      end
    end
  end
end

briefingRoom.mission.objectives[$OBJECTIVEINDEX$].launchMission = function ()
  local objective = briefingRoom.mission.objectives[$OBJECTIVEINDEX$]
  briefingRoom.radioManager.play("Pilot: Escort "..objective.name..", you are clear to begin.", "RadioPilotBeginEscort")
  local unit = Unit.getByName(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
  if unit == nil then
    unit = StaticObject.getByName(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
  end
  if unit ~= nil then
    local group = unit:getGroup()
    if group ~= nil then
      group:activate()
      briefingRoom.radioManager.play("Escort "..objective.name..": Copy, moving out.", "RadioEscortMoving", briefingRoom.radioManager.getAnswerDelay(), nil, nil)
    end
  end
end

local unit = Unit.getByName(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
if unit == nil then
  unit = StaticObject.getByName(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
end
if unit ~= nil and not unit:isActive() then
  missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Launch Mission", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectives[$OBJECTIVEINDEX$].launchMission)
end
