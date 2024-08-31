briefingRoom.mission.objectives[$OBJECTIVEINDEX$].completeCommand = nil
briefingRoom.mission.objectives[$OBJECTIVEINDEX$].reportComplete = function ()
    briefingRoom.radioManager.play("$LANG_PILOT$: $LANG_PILOTREPORTCOMPLETE$", "RadioPilotReportComplete", math.random(1, 3))
    briefingRoom.mission.coreFunctions.completeObjective($OBJECTIVEINDEX$)
    missionCommands.removeItemForCoalition(briefingRoom.playerCoalition, briefingRoom.mission.objectives[$OBJECTIVEINDEX$].completeCommand)
end

briefingRoom.mission.objectiveTimers[$OBJECTIVEINDEX$] = function()
    if briefingRoom.mission.objectives[$OBJECTIVEINDEX$].flownOver then return false end -- Objective complete, nothing to do
  
    local players = dcsExtensions.getAllPlayers()
  
    for _,p in ipairs(players) do
      for __,u in ipairs(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames) do
        local unit = Unit.getByName(u)
        if unit ~= nil then
          local vec2p = dcsExtensions.toVec2(p:getPoint())
          local vec2u = dcsExtensions.toVec2(unit:getPoint())
          local distance = dcsExtensions.getDistance(vec2p, vec2u);
  
          if distance < 9260 and math.abs(vec2p.y - vec2u.y) < 2438 and briefingRoom.mission.objectives[$OBJECTIVEINDEX$].completeCommand == nil then
            local playername = p.getPlayerName and p:getPlayerName() or nil
            if math.random(1, 2) == 1 then
                briefingRoom.radioManager.play((playername or"$LANG_PILOT$").." $LANG_FLYNEAR1$", "RadioPilotTargetReconned1")
            else
                briefingRoom.radioManager.play((playername or"$LANG_PILOT$").." $LANG_FLYNEAR2$", "RadioPilotTargetReconned2")
            end
            briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames = { }
            briefingRoom.mission.objectives[$OBJECTIVEINDEX$].completeCommand = missionCommands.addCommandForCoalition(briefingRoom.playerCoalition, "$LANG_REPORTCOMPLETE$", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectives[$OBJECTIVEINDEX$].reportComplete)
          end
        end
      end
    end
  end


