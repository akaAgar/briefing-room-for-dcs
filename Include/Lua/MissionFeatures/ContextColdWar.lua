-- Makes units hold their fire until attacked, turning the mission into a cold war situation.
for i = 1, 2 do
  for _, g in pairs(coalition.getGroups(i)) do
    local controller = g:getController()

    if g:getCategory() == Group.Category.AIRPLANE or g:getCategory() == Group.Category.HELICOPTER then -- AIRCRAFT
      controller:setOption(AI.Option.Air.id.ROE, AI.Option.Air.val.ROE.RETURN_FIRE) -- never attack or return fire
    elseif g:getCategory() == Group.Category.GROUND then -- GROUND VEHICLES
      controller:setOption(AI.Option.Ground.id.ALARM_STATE, AI.Option.Ground.val.ALARM_STATE.RED) -- still search for enemies with radar even if not attacking
      controller:setOption(AI.Option.Ground.id.ROE, AI.Option.Ground.val.ROE.RETURN_FIRE) -- never attack or return fire
    elseif g:getCategory() == Group.Category.SHIP then -- SHIPS
      controller:setOption(AI.Option.Naval.id.ROE, AI.Option.Naval.val.ROE.RETURN_FIRE) -- never attack or return fire
    end
  end
end

briefingRoom.mission.missionFeatures.contextColdWar = {}
briefingRoom.mission.missionFeatures.contextColdWar.wasHasStarted = false -- has the enemy been attacked yet?

briefingRoom.mission.missionFeatures.contextColdWar.eventHandler = {}
function briefingRoom.mission.missionFeatures.contextColdWar.eventHandler:onEvent(event)

  if briefingRoom.mission.missionFeatures.contextColdWar.wasHasStarted then return end -- War has already started

  -- Make sure this is a hit/death/weapon shot event
  if event.id ~= world.event.S_EVENT_DEAD and event.id ~= world.event.S_EVENT_CRASH and
      event.id ~= world.event.S_EVENT_HIT and event.id ~= world.event.S_EVENT_SHOT then return end

  for i = 1, 2 do
    for _, g in pairs(coalition.getGroups(i)) do
      local controller = g:getController()

      if g:getCategory() == Group.Category.AIRPLANE or g:getCategory() == Group.Category.HELICOPTER then -- AIRCRAFT
        controller:setOption(AI.Option.Air.id.ROE, AI.Option.Air.val.ROE.OPEN_FIRE) -- never attack or return fire
      elseif g:getCategory() == Group.Category.GROUND then -- GROUND VEHICLES
        controller:setOption(AI.Option.Ground.id.ROE, AI.Option.Ground.val.ROE.OPEN_FIRE) -- never attack or return fire
      elseif g:getCategory() == Group.Category.SHIP then -- SHIPS
        controller:setOption(AI.Option.Naval.id.ROE, AI.Option.Naval.val.ROE.OPEN_FIRE) -- never attack or return fire
      end
    end
  end

  briefingRoom.radioManager.play("$LANG_COLDWARGONEHOTTRIGGER$"
    , "RadioHQColdWarGoneHot")
  briefingRoom.mission.missionFeatures.contextColdWar.wasHasStarted = true -- War has begun
end

-- Enable event handler
world.addEventHandler(briefingRoom.mission.missionFeatures.contextColdWar.eventHandler)
