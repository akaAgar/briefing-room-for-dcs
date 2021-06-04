briefingRoom.f10MenuCommands.debug = { } -- Create F10 debug sub-menu
briefingRoom.printDebugMessages = true -- Enable debug messages

-- briefingRoom.extensions.debug = { }

-- function briefingRoom.extensions.debug.activateAllAircraft()
  -- local activatedOk = { }
  -- local activatedFailed = { }

  -- Dump all extra queues into the current queue
  -- if #briefingRoom.aircraftActivator.extraQueues > 0 then
    -- for _,queue in ipairs(briefingRoom.aircraftActivator.extraQueues) do
      -- for __,g in ipairs(queue) do
        -- table.insert(briefingRoom.aircraftActivator.currentQueue, g)
      -- end
    -- end

    -- briefingRoom.aircraftActivator.extraQueues = { }
  -- end

  -- activate all groups in the current queue
  -- for _,g in ipairs(briefingRoom.aircraftActivator.currentQueue) do
    -- local acGroup = dcsExtensions.getGroupByID(g) -- get the group
    -- if acGroup ~= nil then -- activate the group, if it exists
      -- acGroup:activate()
      -- table.insert(activatedOk, tostring(g).." ("..acGroup:getName()..")")
    -- else
      -- table.insert(activatedFailed, tostring(g))
    -- end
  -- end

  -- briefingRoom.aircraftActivator.currentQueue = { }

  -- local activationSummary = "ACTIVATED:\n"
  -- for _,s in ipairs(activatedOk) do activationSummary = activationSummary..s.." " end
  -- activationSummary = activationSummary.."\n\nFAILED:\n"
  -- for _,s in ipairs(activatedFailed) do activationSummary = activationSummary..s.." " end
  -- trigger.action.outText(activationSummary, 15)
-- end

-- Destroys the next active enemy structure
-- function briefingRoom.extensions.debug.destroyEnemyStructure()
  -- for _,s in ipairs(coalition.getStaticObjects($ENEMYCOALITION$)) do
    -- if s:getLife() > 0 then
      -- trigger.action.outText("Destroyed "..s:getName(), 2)
      -- for i=1,32 do trigger.action.explosion(s:getPoint(), 100) end -- structures are tough, multiple explosions required
      -- return
    -- end
  -- end
  -- trigger.action.outText("No enemy structures found", 2)
-- end

-- Destroys the next active target unit
function briefingRoom.f10MenuCommands.debug.destroyTargetUnit()
  for index,objective in ipairs(briefingRoom.mission.objectives) do
    local u = dcsExtensions.getAliveUnitInGroup(objective.groupID)
    if u ~= nil then
      trigger.action.outText("Destroyed "..u:getName(), 2)
      trigger.action.explosion(u:getPoint(), 100)
      return
    end
  end

  trigger.action.outText("No target units found", 2)
end

-- Destroys the next active enemy unit
-- function briefingRoom.extensions.debug.destroyEnemyUnit()
  -- for _,g in ipairs(coalition.getGroups($ENEMYCOALITION$)) do
    -- for __,u in ipairs(g:getUnits()) do
      -- if u:getLife() > 0 then
        -- trigger.action.outText("Destroyed "..u:getName(), 2)
        -- trigger.action.explosion(u:getPoint(), 100)
        -- return
      -- end
    -- end
  -- end
  -- trigger.action.outText("No enemy units found", 2)
-- end

-- Create the debug menu
do
  briefingRoom.f10Menu.debug = missionCommands.addSubMenu("(DEBUG MENU)", nil)
  missionCommands.addCommand("Destroy target unit", briefingRoom.f10Menu.debug, briefingRoom.f10MenuCommands.debug.destroyTargetUnit)
  missionCommands.addCommand("Simulate player takeoff (start mission)", briefingRoom.f10Menu.debug, briefingRoom.mission.coreFunctions.beginMission)

  -- missionCommands.addCommand("Destroy enemy unit", briefingRoom.extensions.debug.menu, briefingRoom.extensions.debug.destroyEnemyUnit)
  -- missionCommands.addCommand("Destroy enemy structure", briefingRoom.extensions.debug.menu, briefingRoom.extensions.debug.destroyEnemyStructure)
  -- missionCommands.addCommand("Activate all aircraft groups", briefingRoom.extensions.debug.menu, briefingRoom.extensions.debug.activateAllAircraft)
end
