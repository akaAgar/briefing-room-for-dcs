briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage = { }
briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage.INTERVAL = 5 -- check damage status every 5 seconds
briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage.SUPPRESSION_DURATION = { 30, 60 } -- how long a ground unit stays suppressed after being hit
briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage.suppressedGroups = { } -- list of currently suppressed units

	--Function to end suppression and let group open fire again
function briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage.endSuppression(group)
  group:getController():setOption(AI.Option.Ground.id.AC_ENGAGEMENT_RANGE_RESTRICTION, 100)
  briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage.suppressedGroups[tonumber(group:getID())] = nil
  briefing.debugPrint("Group "..tostring(group:getID()).." is not suppressed anymore.")
end

-- function briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage(unit)
--   if unit:getLife() < 1 then return end -- Unit is already dead
--   if u:getCategory() ~= Unit.Category.GROUND_UNIT and u:getCategory() ~= Unit.Category.SHIP then return end -- Unit is not a ship or ground vehicle

--   local lifePercent = unit:getLife() / unit:getLife0() -- compute percentage of HP left

--   if u:getCategory() == Unit.Category.GROUND_UNIT then
--     if lifePercent < 0.5 then unit:getController():setOnOff(false) -- less than 50%, unit is completely uneffective
--     elseif 
--   end
-- end

-- -- Every INTERVAL seconds, check all ground and naval units to check their damage status
-- function briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage.update(args, time)
--   -- check all units in all groups in all coalitions
--   for i=1,2 do
--     for _,g in pairs(coalition.getGroups(i)) do
--       for __,u in pairs(g:getUnits()) do
--         briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage.checkUnit(u)
--       end
--     end
--   end

--   return time + briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage.INTERVAL -- schedule next update and return
-- end

-- -- Schedule damage check function
-- timer.scheduleFunction(briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage.update, nil, timer.getTime() + briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage.INTERVAL)

--------------------------------
briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage.eventHandler = { }
function briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage.eventHandler:onEvent(event)
  if event.id ~= world.event.S_EVENT_HIT then return end -- Make sure this is a hit event

  local unit = event.target
  if unit == nil then return end -- no target
  if Object.getCategory(unit) ~= Object.Category.UNIT then return false end -- target is not an unit
  if unit:getCategory() ~= Unit.Category.GROUND_UNIT and unit:getCategory() ~= Unit.Category.SHIP then return end -- target is not a ship or ground vehicle

  local lifePercent = unit:getLife() / unit:getLife0() -- compute percentage of HP left

  if unit:getCategory() == Unit.Category.GROUND_UNIT then -- Ground unit
    local groupID = unit:getGroup():getID()
    if briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage.suppressedGroups[groupID] == nil then
    end
    briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage.SUPPRESSION_DURATION

    if event.weapon ~= nil then
      if event.weapon:getCategory() ~= Weapon.Category.SHELL then
        if random(1, 2) then
          trigger.action.explosion(table vec3 , math.random(10, 20))
        end
      end
    end

    if lifePercent < 0.35 then -- less than 35% HP, unit is completely uneffective
      unit:getGroup():getController():setOnOff(false)
    elseif lifePercent < 0.7 then -- less than 70% HP, unit cannot attack anymore
      unit:getGroup():getController():setOption(AI.Option.Ground.id.AC_ENGAGEMENT_RANGE_RESTRICTION, 0)
    else -- less than 100% HP, unit attack capabilities halved
      unit:getGroup():getController():setOption(AI.Option.Ground.id.AC_ENGAGEMENT_RANGE_RESTRICTION, 50)
    end
  end
end

-- Enable event handler
world.addEventHandler(briefingRoom.mission.missionFeatures.improvementsGroundUnitsDamage.eventHandler)

