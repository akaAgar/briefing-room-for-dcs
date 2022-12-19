briefingRoom.aircraftActivator.responsiveMode = true
briefingRoom.mission.missionFeatures.ImprovementResponsiveAircraftActivator = {}
briefingRoom.mission.missionFeatures.ImprovementResponsiveAircraftActivator.interval = 60

function briefingRoom.mission.missionFeatures.ImprovementResponsiveAircraftActivator.detectedSpawner(args, time)
    -- Don't spawn extra aircraft via this method if CAP is already in the air.
    if briefingRoom.mission.hasStarted and
        briefingRoom.mission.missionFeatures.ImprovementResponsiveAircraftActivator.isPlayerDetected() and
        not briefingRoom.mission.missionFeatures.ImprovementResponsiveAircraftActivator.isCASInAir() then
        briefingRoom.aircraftActivator.possibleResponsiveSpawn()
    end
    return time + briefingRoom.mission.missionFeatures.ImprovementResponsiveAircraftActivator.interval
end

function briefingRoom.mission.missionFeatures.ImprovementResponsiveAircraftActivator.isCASInAir()
    briefingRoom.debugPrint("Checking if enemy CAP is in the air")
    for _, g in pairs(coalition.getGroups(briefingRoom.enemyCoalition), Group.Category.AIRPLANE) do
        local groupRouteData = mist.getGroupRoute(g:getName(), true)
        if dcsExtensions.getAliveUnitInGroup(g:getName()) and #groupRouteData[1].task.params.tasks > 0 and
            groupRouteData[1].task.params.tasks[1].key == "CAP" then
            briefingRoom.debugPrint("Enemy CAP is in the air")
            briefingRoom.debugPrint(g:getName())
            return true
        end
    end
    return false
end

function briefingRoom.mission.missionFeatures.ImprovementResponsiveAircraftActivator.isPlayerDetected()
    briefingRoom.debugPrint("Checking is player detected")
    for _, player in ipairs(dcsExtensions.getAllPlayers()) do
        for _, g in pairs(coalition.getGroups(briefingRoom.enemyCoalition)) do
            if dcsExtensions.getAliveUnitInGroup(g:getName()) then
                for __, u in pairs(g:getUnits()) do
                    local con = u:getController()
                    if con then
                        for det, enum in pairs(Controller.Detection) do
                            if con:isTargetDetected(player, enum) and det ~= "RWR" and det ~= "DLINK" then
                                briefingRoom.debugPrint("Player seen on " .. det)
                                return true
                            end
                        end
                    end
                end
            end
        end
    end
    return false
end

timer.scheduleFunction(briefingRoom.mission.missionFeatures.ImprovementResponsiveAircraftActivator.detectedSpawner, nil,
    timer.getTime() + briefingRoom.mission.missionFeatures.ImprovementResponsiveAircraftActivator.interval)
