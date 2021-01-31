do
  local smokeIntensity = briefingRoom.mission.parameters.fireAndSmokeOnTargetIntensity or math.random(1, 2)

  for _,o in ipairs(briefingRoom.mission.objectives) do
    local spawnPoint = { x = o.coordinates.x, y = land.getHeight(o.coordinates), z = o.coordinates.y }
    trigger.action.effectSmokeBig(spawnPoint, smokeIntensity, math.random(7, 10) * 0.1)
  end
end
