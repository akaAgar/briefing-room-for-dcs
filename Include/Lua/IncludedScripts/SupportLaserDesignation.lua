briefingRoom.mission.features.supportLaserDesignation = { }
briefingRoom.mission.features.supportLaserDesignation.LASER_CODE = 1688 -- laser code to use
briefingRoom.mission.features.supportLaserDesignation.laserSpot = { } -- laser spot for each JTAC
briefingRoom.mission.features.supportLaserDesignation.laserTarget = { } -- unit currently laser by the JTAC

-- Update active lasers every second
function briefingRoom.mission.features.supportLaserDesignation.updateAllLasers(args, time)
  for i, v in pairs(briefingRoom.mission.features.supportLaserDesignation.laserTarget) do -- for each JTAC...
    if briefingRoom.mission.features.supportLaserDesignation.laserTarget[i] ~= nil then -- if lasing target is set...   

      local targetDestroyed = false
      if not briefingRoom.mission.features.supportLaserDesignation.laserTarget[i]:isExist() then
        targetDestroyed = true
      elseif briefingRoom.mission.features.supportLaserDesignation.laserTarget[i]:getLife() < 1 then
        targetDestroyed = true
      end

      if targetDestroyed then -- if target is dead...
        briefingRoom.debugPrint("Target is dead, destroyed JTAC #"..tostring(i).."'s laser")

        if briefingRoom.mission.features.supportLaserDesignation.laserSpot[i] ~= nil then -- destroy laser spot, if any
          Spot.destroy(briefingRoom.mission.features.supportLaserDesignation.laserSpot[i])
          briefingRoom.mission.features.supportLaserDesignation.laserSpot[i] = nil
        end
        briefingRoom.mission.features.supportLaserDesignation.laserTarget[i] = nil -- unset target
      else -- if target is still alive...
        local targetPos = briefingRoom.mission.features.supportLaserDesignation.laserTarget[i]:getPoint()
        local targetSpeed = briefingRoom.mission.features.supportLaserDesignation.laserTarget[i]:getVelocity()
        -- adds a small offset so that the laser is always where the (moving) target will be, not where it is
        targetPos.x = targetPos.x + targetSpeed.x
        targetPos.y = targetPos.y + 2.0
        targetPos.z = targetPos.z + targetSpeed.z

        briefingRoom.debugPrint("Updating JTAC #"..tostring(i).." laser: "..tostring(targetPos.x)..","..tostring(targetPos.y)..","..tostring(targetPos.z), 1)

        if briefingRoom.mission.features.supportLaserDesignation.laserSpot[i] == nil then -- spot doesn't exist, create it
          briefingRoom.mission.features.supportLaserDesignation.laserSpot[i] = Spot.createLaser(briefingRoom.mission.features.supportLaserDesignation.laserTarget[i], { x = 0, y = 2.0, z = 0 }, targetPos, briefingRoom.mission.features.supportLaserDesignation.LASER_CODE)
        else -- spot already exists, update its position
          briefingRoom.mission.features.supportLaserDesignation.laserSpot[i]:setPoint(targetPos)
        end
      end
    end
  end

  return time + 1 -- next update in one second
end

-- Begins lasing the target
function briefingRoom.mission.features.supportLaserDesignation.turnLaserOn(index)
  briefingRoom.radioManager.play("Can you paint the target for me?", "RadioPilotLaseTarget")

  if briefingRoom.mission.features.supportLaserDesignation.laserTarget[index] ~= nil then -- already lasing something
    briefingRoom.radioManager.play("Already painting the target. Check laser code. Laser code is "..tostring(briefingRoom.mission.features.supportLaserDesignation.LASER_CODE)..".", "RadioSupportTargetLasingAlready", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  if #briefingRoom.mission.objectives[index].unitsID == 0 then -- no target units left
    briefingRoom.radioManager.play("Negative, no visual on any target.", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  local unit = dcsExtensions.getUnitByID(briefingRoom.mission.objectives[index].unitsID[1])
  if unit == nil then -- no unit found with the ID
    briefingRoom.radioManager.play("Negative, no visual on any target.", "RadioSupportNoTarget", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  briefingRoom.radioManager.play("Affirm. Laser on, painting the target now. Laser code is "..tostring(briefingRoom.mission.features.supportLaserDesignation.LASER_CODE)..".", "RadioSupportLasingOk", briefingRoom.radioManager.getAnswerDelay())
  briefingRoom.mission.features.supportLaserDesignation.laserTarget[index] = unit
end

-- Stops lasing the target
function briefingRoom.mission.features.supportLaserDesignation.turnLaserOff(index)
  briefingRoom.radioManager.play("Terminate. Laser off.", "RadioPilotLaseTargetStop")

  if briefingRoom.mission.features.supportLaserDesignation.laserTarget[index] == nil then -- not lasing anything
    briefingRoom.radioManager.play("Cannot comply, not lasing anything.", "RadioSupportLasingNotLasing", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  if briefingRoom.mission.features.supportLaserDesignation.laserSpot[i] ~= nil then -- destroy laser spot, if any
    Spot.destroy(briefingRoom.mission.features.supportLaserDesignation.laserSpot[i])
    briefingRoom.mission.features.supportLaserDesignation.laserSpot[i] = nil
  end
  briefingRoom.mission.features.supportLaserDesignation.laserTarget[index] = nil -- unset target

  briefingRoom.radioManager.play("Copy. Terminate, laser is off.", "RadioSupportLasingStopped", briefingRoom.radioManager.getAnswerDelay())
end

-- Create F10 menu options
do
  for i,o in ipairs(briefingRoom.mission.objectives) do
    missionCommands.addCommandForCoalition($PLAYERCOALITION$, "Designate target with laser", briefingRoom.f10Menu.objectives[i], briefingRoom.mission.features.supportLaserDesignation.turnLaserOn, i)
    missionCommands.addCommandForCoalition($PLAYERCOALITION$, "Stop lasing target", briefingRoom.f10Menu.objectives[i], briefingRoom.mission.features.supportLaserDesignation.turnLaserOff, i)
  end
end

-- Begin updating laser designation
timer.scheduleFunction(briefingRoom.mission.features.supportLaserDesignation.updateAllLasers, nil, timer.getTime() + 1)

do
  for i=1,#briefingRoom.mission.objectives do
    briefingRoom.mission.features.supportLaserDesignation.laserSpot[i] = nil
    briefingRoom.mission.features.supportLaserDesignation.laserTarget[i] = nil
  end
end
