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
      if distance < 500 then
        table.removeValue(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames, u)
        if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames < 1 then -- all target units destroyed, objective complete
          briefingRoom.radioManager.play("$LANG_PILOT$: $LANG_ESCORTCOMPLETE$", "RadioPilotEscortComplete")
          briefingRoom.mission.coreFunctions.completeObjective($OBJECTIVEINDEX$)
        end
      end
    end
  end
end

briefingRoom.mission.objectives[$OBJECTIVEINDEX$].launchMission = function ()
  local objective = briefingRoom.mission.objectives[$OBJECTIVEINDEX$]
  briefingRoom.radioManager.play("$LANG_PILOT$: $LANG_ESCORTSTARTREQUEST$", "RadioPilotBeginEscort")
  local unit = Unit.getByName(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
  if unit == nil then
    unit = StaticObject.getByName(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
  end
  if unit ~= nil then
    local group = unit:getGroup()
    if group ~= nil then
      group:activate()
      briefingRoom.radioManager.play("$LANG_ESCORT$ "..objective.name..": $LANG_ESCORTAFFIRM$", "RadioEscortMoving", briefingRoom.radioManager.getAnswerDelay(), nil, nil)
    end
  end
end

local unit = Unit.getByName(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
if unit == nil then
  unit = StaticObject.getByName(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
end
if unit ~= nil and not unit:isActive() then
  table.insert(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].f10Commands, {text = "$LANG_ESCORTMENU$", func = briefingRoom.mission.objectives[$OBJECTIVEINDEX$].launchMission, args =  nil})
end
