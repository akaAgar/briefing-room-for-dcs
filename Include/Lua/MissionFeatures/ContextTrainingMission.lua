-- Makes enemy units hold their fire, turning the mission into a training mission
for _, g in pairs(coalition.getGroups(briefingRoom.enemyCoalition)) do
  local controller = g:getController()

  if g:getCategory() == Group.Category.AIRPLANE or g:getCategory() == Group.Category.HELICOPTER then            -- AIRCRAFT
    controller:setOption(AI.Option.Air.id.FLARE_USING, AI.Option.Air.val.FLARE_USING.NEVER)                     -- don't use flares
    controller:setOption(AI.Option.Air.id.REACTION_ON_THREAT, AI.Option.Air.val.REACTION_ON_THREAT.NO_REACTION) -- don't react when attacked
    controller:setOption(AI.Option.Air.id.ROE, AI.Option.Air.val.ROE.WEAPON_HOLD)                               -- never attack or return fire
  elseif g:getCategory() == Group.Category.GROUND then                                                          -- GROUND VEHICLES
    controller:setOption(AI.Option.Ground.id.ALARM_STATE, AI.Option.Ground.val.ALARM_STATE.GREEN)               -- don't use radar or search of enemies
    controller:setOption(AI.Option.Ground.id.DISPERSE_ON_ATTACK, false)                                         -- don't flee when attacked
    controller:setOption(AI.Option.Ground.id.ROE, AI.Option.Ground.val.ROE.WEAPON_HOLD)                         -- never attack or return fire
    controller:setCommand({ id = "SMOKE_ON_OFF", params = { value = false } })                                  -- no smoke screen defense
  elseif g:getCategory() == Group.Category.SHIP then                                                            -- SHIPS
    controller:setOption(AI.Option.Naval.id.ROE, AI.Option.Naval.val.ROE.WEAPON_HOLD)                           -- never attack or return fire
  end
end
