briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser = { }
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserSpot = nil -- current laser spot
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserTarget = nil -- current lased target

-- Update active lasers every second
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserWatch = function(args, time)
  local obj = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$]
  -- if lasing target is set...
  if obj.targetDesignationLaser.laserTarget == nil then
    return time + 1 -- next update in one second
  end

  if briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID[tonumber(obj.targetDesignationLaser.laserTarget:getID())] == nil then -- target is considered complete
    briefingRoom.debugPrint("JTAC $OBJECTIVEINDEX$: Target Complete finding new target", 1)
    local unit = obj.targetDesignationLaser.setRandomTarget()
    if unit == nil then
      obj.targetDesignationLaser.deleteLaser()
      briefingRoom.radioManager.play("No visual on any target, laser is off.", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay()) -- TODO new sound
      return
    end
    briefingRoom.radioManager.play("Painting next target", "RadioSupportLasingOk", briefingRoom.radioManager.getAnswerDelay()) -- TODO new Sound
  end

  obj.targetDesignationLaser.updateLaserPos()
  return time + 1
end

-- # Operation Functions

-- Updtae Laser pos
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.updateLaserPos = function()
  local objFeature = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$]
  local targetPos = objFeature.targetDesignationLaser.laserTarget:getPoint()
  local targetSpeed = objFeature.targetDesignationLaser.laserTarget:getVelocity()
  -- adds a small offset so that the laser is always where the (moving) target will be, not where it is
  targetPos.x = targetPos.x + targetSpeed.x
  targetPos.y = targetPos.y + 2.0
  targetPos.z = targetPos.z + targetSpeed.z
  if objFeature.targetDesignationLaser.laserSpot == nil then
    objFeature.targetDesignationLaser.laserSpot = Spot.createLaser(objFeature.targetDesignationLaser.laserTarget, { x = 0, y = 2.0, z = 0 }, targetPos, LASER_CODE)
    briefingRoom.debugPrint("JTAC $OBJECTIVEINDEX$: Created Laser"..tostring(targetPos.x)..","..tostring(targetPos.y)..","..tostring(targetPos.z), 1)
  else -- spot already exists, update its position
    objFeature.targetDesignationLaser.laserSpot:setPoint(targetPos)
    briefingRoom.debugPrint("JTAC $OBJECTIVEINDEX$: Update Laser Pos"..tostring(targetPos.x)..","..tostring(targetPos.y)..","..tostring(targetPos.z), 1)
  end
end

-- Delete Laser
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.deleteLaser = function()
  local objFeature = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$]
  if objFeature.targetDesignationLaser.laserSpot ~= nil then
    Spot.destroy(objFeature.targetDesignationLaser.laserSpot)
    objFeature.targetDesignationLaser.laserSpot = nil
  end

  -- unset target and play radio message
  objFeature.targetDesignationLaser.laserTarget = nil
  briefingRoom.debugPrint("JTAC $OBJECTIVEINDEX$: Deleted Laser", 1)
end

-- Get Random Target
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.setRandomTarget = function()
  local objective = briefingRoom.mission.objectives[$OBJECTIVEINDEX$]
  local randomId = math.random(#objective.unitsID)
  briefingRoom.debugPrint("JTAC $OBJECTIVEINDEX$: DEBUG GOT RANDOM ID:"..randomId, 1)
  local unit = dcsExtensions.getUnitByID(objective.unitsID[randomId])
  if unit == nil then -- no unit found with the ID, try searching for a static
    unit = dcsExtensions.getStaticByID(objective.unitsID[randomId])
    if unit == nil then -- no unit nor static found with the ID
      briefingRoom.debugPrint("JTAC $OBJECTIVEINDEX$: FOUND NOTHING", 1)
      return
    end
  end
  briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserTarget = unit
  briefingRoom.debugPrint("JTAC $OBJECTIVEINDEX$: Assigned Laser Target:"..randomId, 1)
  return unit
end

-- # UI Commands

-- Begins lasing the target
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.turnOn = function()
  local objFeature = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$]
  briefingRoom.radioManager.play("Can you paint the target for me?", "RadioPilotLaseTarget")

  -- already lasing something
  if objFeature.targetDesignationLaser.laserTarget ~= nil then
    briefingRoom.radioManager.play("Already painting the target. Check laser code. Laser code is "..tostring(LASER_CODE)..".", "RadioSupportTargetLasingAlready", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  -- no target units left
  local unit = objFeature.targetDesignationLaser.setRandomTarget()
  if unit == nil then
    briefingRoom.radioManager.play("Negative, no visual on any target.", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end
  briefingRoom.radioManager.play("Affirm. Laser on, painting the target now. Laser code is "..tostring(LASER_CODE)..".", "RadioSupportLasingOk", briefingRoom.radioManager.getAnswerDelay())
end

-- Stops lasing the target
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.turnOff = function()
  local objFeature = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$]
  briefingRoom.radioManager.play("Terminate. Laser off.", "RadioPilotLaseTargetStop")
  -- not lasing anything
  if objFeature.targetDesignationLaser.laserTarget == nil then
    briefingRoom.radioManager.play("Cannot comply, not lasing anything.", "RadioSupportLasingNotLasing", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  objFeature.targetDesignationLaser.deleteLaser()
  briefingRoom.radioManager.play("Copy. Terminate, laser is off.", "RadioSupportLasingStopped", briefingRoom.radioManager.getAnswerDelay())
end

-- Begins lasing the target
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.newTarget = function()
  briefingRoom.radioManager.play("Can you paint a diffrent target for me?", "RadioPilotLaseTarget")

  -- no target units left
  local unit = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.setRandomTarget()
  if unit == nil then
    briefingRoom.radioManager.play("Negative, no visual on any target.", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end
  briefingRoom.radioManager.play("Painting next target", "RadioSupportLasingOk", briefingRoom.radioManager.getAnswerDelay()) -- TODO new Sound
end


-- Begin updating laser designation
timer.scheduleFunction(briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserWatch, nil, timer.getTime() + 1)


missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Designate target with laser", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.turnOn)
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Stop lasing target", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.turnOff)
missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Get a diffrent lasing target", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.newTarget)
