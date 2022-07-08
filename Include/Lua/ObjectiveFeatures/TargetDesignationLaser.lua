briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser = { }
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserSpot = nil -- current laser spot
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserTarget = nil -- current lased target
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserCode = $LASERCODE$ -- current lased target

-- Update active lasers every second
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserWatch = function(args, time)
  local objFeature = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$]
  local objective = briefingRoom.mission.objectives[$OBJECTIVEINDEX$]
  -- if lasing target is set...
  if objFeature.targetDesignationLaser.laserTarget == nil then
    return time + 1 -- next update in one second
  end

  if not objFeature.targetDesignationLaser.laserTarget:isExist() or not table.contains(objective.unitNames, objFeature.targetDesignationLaser.laserTarget:getName()) then -- target is considered complete
    briefingRoom.debugPrint("JTAC $OBJECTIVEINDEX$: $LANGLASERTARGETDESTROYED$", 1)
    local unit = objFeature.targetDesignationLaser.setRandomTarget()
    if unit == nil then
      objFeature.targetDesignationLaser.deleteLaser()
      briefingRoom.radioManager.play(objective.name.." $LANGJTAC$: $LANGLASERNOTARGET$", "RadioSupportLasingNoMoreTargets", briefingRoom.radioManager.getAnswerDelay())
      return
    end
    briefingRoom.radioManager.play(objective.name.." $LANGJTAC$: $LANGLASERNEXTTARGET$", "RadioSupportLasingNextTarget", briefingRoom.radioManager.getAnswerDelay())
  end

  objFeature.targetDesignationLaser.updateLaserPos()
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
    objFeature.targetDesignationLaser.laserSpot = Spot.createLaser(objFeature.targetDesignationLaser.laserTarget, { x = 0, y = 2.0, z = 0 }, targetPos, objFeature.targetDesignationLaser.laserCode)
    briefingRoom.debugPrint("JTAC $OBJECTIVEINDEX$: Created Laser "..objFeature.targetDesignationLaser.laserSpot:getCode()..":"..tostring(targetPos.x)..","..tostring(targetPos.y)..","..tostring(targetPos.z), 1)
  else -- spot already exists, update its position
    objFeature.targetDesignationLaser.laserSpot:setPoint(targetPos)
    briefingRoom.debugPrint("JTAC $OBJECTIVEINDEX$: Update Laser Pos "..objFeature.targetDesignationLaser.laserSpot:getCode()..":"..tostring(targetPos.x)..","..tostring(targetPos.y)..","..tostring(targetPos.z), 1)
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
  local randomUnitName = math.randomFromHashTable(objective.unitNames)
  local unit = Unit.getByName(randomUnitName)
  if unit == nil then -- no unit found with the ID, try searching for a static
    unit = StaticObject.getByName(randomUnitName)
    if unit == nil then -- no unit nor static found with the ID
      return nil
    end
  end
  briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserTarget = unit
  briefingRoom.debugPrint("JTAC $OBJECTIVEINDEX$: Assigned Laser Target:"..randomUnitName, 1)
  return unit
end

-- # UI Commands

-- Begins lasing the target
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.turnOn = function()
  local objFeature = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$]
  local objective = briefingRoom.mission.objectives[$OBJECTIVEINDEX$]
  briefingRoom.radioManager.play("$LANGPILOT$: $LANGLASERREQUEST$", "RadioPilotLaseTarget")

  -- already lasing something
  if objFeature.targetDesignationLaser.laserTarget ~= nil then
    briefingRoom.radioManager.play(objective.name.." $LANGJTAC$: $LANGLASERALREADYPAINTING$"..tostring(objFeature.targetDesignationLaser.laserCode)..".", "RadioSupportTargetLasingAlready", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  -- no target units left
  local unit = objFeature.targetDesignationLaser.setRandomTarget()
  if unit == nil then
    briefingRoom.radioManager.play(objective.name.." $LANGJTAC$: $LANGNOTARGETREMAINING$", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end
  briefingRoom.radioManager.play(objective.name.." $LANGJTAC$: $LANGLASERAFFIRM$"..tostring(objFeature.targetDesignationLaser.laserCode)..".", "RadioSupportLasingOk", briefingRoom.radioManager.getAnswerDelay())
  missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Lase diffrent target", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.newTarget)
  missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Stop lasing target", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.turnOff)
end

-- Stops lasing the target
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.turnOff = function()
  local objFeature = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$]
  local objective = briefingRoom.mission.objectives[$OBJECTIVEINDEX$]
  briefingRoom.radioManager.play("$LANGPILOT$: Terminate. Laser off.", "RadioPilotLaseTargetStop")
  -- not lasing anything
  if objFeature.targetDesignationLaser.laserTarget == nil then
    briefingRoom.radioManager.play(objective.name.." $LANGJTAC$: $LANGLASERALREADYOFF", "RadioSupportLasingNotLasing", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  objFeature.targetDesignationLaser.deleteLaser()
  briefingRoom.radioManager.play(objective.name.." $LANGJTAC$: $LANGLASEROFF$", "RadioSupportLasingStopped", briefingRoom.radioManager.getAnswerDelay())
end

-- Get new target
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.newTarget = function()
  local objective = briefingRoom.mission.objectives[$OBJECTIVEINDEX$]
  briefingRoom.radioManager.play("$LANGPILOT$: $LANGLASERNEWTARGET$", "RadioPilotLaseDiffrentTarget")

  -- no target units left
  local unit = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.setRandomTarget()
  if unit == nil then
    briefingRoom.radioManager.play(objective.name.." $LANGJTAC$:$LANGNOTARGET$", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end
  briefingRoom.radioManager.play(objective.name.." $LANGJTAC$: $LANGLASERNEXTTARGET$", "RadioSupportLasingNextTarget", briefingRoom.radioManager.getAnswerDelay())
end


-- Begin updating laser designation
timer.scheduleFunction(briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.laserWatch, nil, timer.getTime() + 1)

missionCommands.addCommandForCoalition($LUAPLAYERCOALITION$, "Designate target with laser", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].targetDesignationLaser.turnOn)
