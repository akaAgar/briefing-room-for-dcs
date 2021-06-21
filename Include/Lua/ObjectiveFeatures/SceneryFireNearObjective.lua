-- Add fire and smoke near objective
do
  local wpCoos = table.deepCopy(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].waypoint)
  wpCoos.x = wpCoos.x + math.random(-3000, 3000)
  wpCoos.y = wpCoos.y + math.random(-3000, 3000)
  local spawnPoint = { x = wpCoos.x, y = land.getHeight(wpCoos), z = wpCoos.y }
  trigger.action.effectSmokeBig(spawnPoint, math.random(2, 3), math.random(7, 10) * 0.1)
end

