briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser = { }
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserSpot = nil -- current laser spot
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserTarget = nil -- current lased target

-- Update active lasers every second
function briefingRoom.mission.objectiveFeaturesCommon.targetDesignationLaserUpdateAll(args, time)
  for i,_ in ipairs(briefingRoom.mission.objectives) do
    if briefingRoom.mission.objectiveFeatures[i].targetDesignationLaser ~= nil then
      -- if lasing target is set...
      if briefingRoom.mission.objectiveFeatures[i].targetDesignationLaser.laserTarget ~= nil then
        local targetDestroyed = false
        if not briefingRoom.mission.objectiveFeatures[i].targetDesignationLaser.laserTarget:isExist() then
          targetDestroyed = true
        elseif briefingRoom.mission.objectiveFeatures[i].targetDesignationLaser.laserTarget:getLife() < 1 then
          targetDestroyed = true
        end

        if targetDestroyed then -- target is dead
          briefingRoom.debugPrint("Target is dead, destroyed JTAC #"..tostring(i).."'s laser")

          -- destroy laser spot, if any
          if briefingRoom.mission.objectiveFeatures[i].targetDesignationLaser.laserSpot ~= nil then
            Spot.destroy(briefingRoom.mission.objectiveFeatures[i].targetDesignationLaser.laserSpot)
            briefingRoom.mission.objectiveFeatures[i].targetDesignationLaser.laserSpot = nil
          end
          briefingRoom.mission.objectiveFeatures[i].targetDesignationLaser.laserSpot = nil -- unset target
        else -- if target is still alive...
          local targetPos = briefingRoom.mission.objectiveFeatures[i].targetDesignationLaser.laserTarget:getPoint()
          local targetSpeed = briefingRoom.mission.objectiveFeatures[i].targetDesignationLaser.laserTarget:getVelocity()
          -- adds a small offset so that the laser is always where the (moving) target will be, not where it is
          targetPos.x = targetPos.x + targetSpeed.x
          targetPos.y = targetPos.y + 2.0
          targetPos.z = targetPos.z + targetSpeed.z

          briefingRoom.debugPrint("Updating JTAC #"..tostring(i).." laser: "..tostring(targetPos.x)..","..tostring(targetPos.y)..","..tostring(targetPos.z), 1)

          -- spot doesn't exist, create it
          if briefingRoom.mission.objectiveFeatures[i].targetDesignationLaser.laserSpot == nil then
            briefingRoom.mission.objectiveFeatures[i].targetDesignationLaser.laserSpot = Spot.createLaser(briefingRoom.mission.features.supportLaserDesignation.laserTarget[i], { x = 0, y = 2.0, z = 0 }, targetPos, briefingRoom.mission.features.supportLaserDesignation.LASER_CODE)
          else -- spot already exists, update its position
            briefingRoom.mission.objectiveFeatures[i].targetDesignationLaser.laserSpot:setPoint(targetPos)
          end
        end
      end
    end

    return time + 1 -- next update in one second
  end

  -- Begins lasing the target
  briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.turnOn = function()
  briefingRoom.radioManager.play("Can you paint the target for me?", "RadioPilotLaseTarget")

  -- already lasing something
  if briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserTarget ~= nil then
    briefingRoom.radioManager.play("Already painting the target. Check laser code. Laser code is "..tostring(LASER_CODE)..".", "RadioSupportTargetLasingAlready", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  -- no target units left
  if #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID == 0 then
    briefingRoom.radioManager.play("Negative, no visual on any target.", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  local unit = dcsExtensions.getUnitByID(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID[1])
  if unit == nil then -- no unit found with the ID, try searching for a static
    unit = dcsExtensions.getStaticByID(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID[1])
    if unit == nil then -- no unit nor static found with the ID
      briefingRoom.radioManager.play("Negative, no visual on any target.", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
      return
    end
  end

  briefingRoom.radioManager.play("Affirm. Laser on, painting the target now. Laser code is "..tostring(LASER_CODE)..".", "RadioSupportLasingOk", briefingRoom.radioManager.getAnswerDelay())
  briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserTarget = unit
end

-- Stops lasing the target
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.turnOff = function()
  briefingRoom.radioManager.play("Terminate. Laser off.", "RadioPilotLaseTargetStop")
  -- not lasing anything
  if briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserTarget == nil then
    briefingRoom.radioManager.play("Cannot comply, not lasing anything.", "RadioSupportLasingNotLasing", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  -- destroy laser spot, if any
  if briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserSpot ~= nil then
    Spot.destroy(briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserSpot)
    briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserSpot = nil
  end

  -- unset target and play radio message
  briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserTarget = nil
  briefingRoom.radioManager.play("Copy. Terminate, laser is off.", "RadioSupportLasingStopped", briefingRoom.radioManager.getAnswerDelay())
end

-- Begin updating laser designation
if not briefingRoom.mission.objectiveFeaturesCommon.targetDesignationLaserUpdateScheduled then
  timer.scheduleFunction(briefingRoom.mission.objectiveFeaturesCommon.targetDesignationLaserUpdateAll, nil, timer.getTime() + 1)
  briefingRoom.mission.objectiveFeaturesCommon.targetDesignationLaserUpdateScheduled = true
end

-- Add the commands to the F10 menu
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Designate target with laser", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.turnOn)
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Stop lasing target", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.turnOff)
