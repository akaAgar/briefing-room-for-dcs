-- Triggers the completion of objective $OBJECTIVEINDEX$ when all targets are destroyed
table.insert(briefingRoom.mission.objectiveTriggers, briefingRoom.mission.getDestroyFunction($OBJECTIVEINDEX$))
