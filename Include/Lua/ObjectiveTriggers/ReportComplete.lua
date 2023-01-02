briefingRoom.mission.objectives[$OBJECTIVEINDEX$].reportComplete = function ()
    briefingRoom.radioManager.play("$LANG_PILOT$: $LANG_PILOTREPORTCOMPLETE$", "RadioPilotReportComplete", math.random(1, 3))
    briefingRoom.mission.coreFunctions.completeObjective($OBJECTIVEINDEX$)
end
missionCommands.addCommandForCoalition(briefingRoom.playerCoalition, "$LANG_REPORTCOMPLETE$", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectives[$OBJECTIVEINDEX$].reportComplete)
