briefingRoom.mission.missionFeatures.respawnAircraft = {}
briefingRoom.mission.missionFeatures.respawnAircraft.totalGroups = table.merge(dcsExtensions.getGroupNamesContaining("-RQ-")
    , dcsExtensions.getGroupNamesContaining("-IQ-"))
briefingRoom.mission.missionFeatures.respawnAircraft.spawnChance = 3 -- out of 10 each check when suitable
briefingRoom.mission.missionFeatures.respawnAircraft.interval = { 300, 600 } -- min/max interval (in seconds) between two updates


function briefingRoom.mission.missionFeatures.respawnAircraft.getRandomInterval()
    return math.random(briefingRoom.mission.missionFeatures.respawnAircraft.interval[1],
        briefingRoom.aircraftActivator.INTERVAL[2])
end

function briefingRoom.mission.missionFeatures.respawnAircraft.checkDeadGroups(args, time)
    if not briefingRoom.mission.hasStarted then
        return time + briefingRoom.mission.missionFeatures.respawnAircraft.getRandomInterval()
    end
    briefingRoom.mission.missionFeatures.respawnAircraft.trySpawnQueue(briefingRoom.mission.missionFeatures.respawnAircraft
        .totalGroups)
    return time + briefingRoom.mission.missionFeatures.respawnAircraft.getRandomInterval()
end

function briefingRoom.mission.missionFeatures.respawnAircraft.trySpawnQueue(queueTable)
    local suitableAircraft = table.filter(queueTable, function(o, k, i)
        return not string.find(o, "-TGT-") and mist.groupIsDead(o)
    end)
    briefingRoom.debugPrint(#suitableAircraft .. " Suitable for respawn")
    for k, name in pairs(suitableAircraft) do
        if mist.random(10) < briefingRoom.mission.missionFeatures.respawnAircraft.spawnChance then
            mist.respawnGroup(name, true)
            local acGroup = Group.getByName(name) -- get the group
            if acGroup ~= nil then -- activate the group, if it exists
                acGroup:activate()
                local Start = {
                    id = 'Start',
                    params = {
                    }
                }
                acGroup:getController():setCommand(Start)
                briefingRoom.debugPrint("Respawning aircraft group " .. name)
            end
        end
    end
end

timer.scheduleFunction(briefingRoom.mission.missionFeatures.respawnAircraft.checkDeadGroups, {},
    timer.getTime() + briefingRoom.mission.missionFeatures.respawnAircraft.getRandomInterval())
