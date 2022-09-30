briefingRoom.mission.missionFeatures.respawnAircraft = {}
briefingRoom.mission.missionFeatures.respawnAircraft.totalGroups = table.merge(dcsExtensions.getGroupNamesContaining("-RQ-")
    , dcsExtensions.getGroupNamesContaining("-IQ-"))
briefingRoom.mission.missionFeatures.respawnAircraft.spawnChance = 3 -- out of 10 each check when suitable
briefingRoom.mission.missionFeatures.respawnAircraft.intervalMultiplier = 2 -- how much longer is the random check interval vs normal aircraftActivator

function briefingRoom.mission.missionFeatures.respawnAircraft.checkDeadGroups(args, time)
    if not briefingRoom.mission.hasStarted then
        return time + briefingRoom.aircraftActivator.getRandomInterval() * briefingRoom.mission.missionFeatures.respawnAircraft.intervalMultiplier
    end
    briefingRoom.mission.missionFeatures.respawnAircraft.trySpawnQueue(briefingRoom.mission.missionFeatures.respawnAircraft
        .totalGroups)
    return time + briefingRoom.aircraftActivator.getRandomInterval() * briefingRoom.mission.missionFeatures.respawnAircraft.intervalMultiplier
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
    timer.getTime() + briefingRoom.aircraftActivator.getRandomInterval())
