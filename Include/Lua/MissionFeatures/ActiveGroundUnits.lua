briefingRoom.mission.missionFeatures.activeGroundUnits = {}
briefingRoom.mission.missionFeatures.activeGroundUnits.moveChance = 7 -- out of 10 each check when
briefingRoom.mission.missionFeatures.activeGroundUnits.formationOptions = {
    "Off Road",
    "On Road",
    "Rank",
    "Cone",
    "Vee",
    "Diamond",
    "EchelonL",
    "EchelonR",
}

function briefingRoom.mission.missionFeatures.activeGroundUnits.directUnit(group, oppositeSide)
    local path = {}
    local groupPoint = group:getUnit(1):getPoint()
    local _sort = function(a, b)
        return mist.utils.get2DDist(groupPoint, a:getUnit(1):getPoint()) <
            mist.utils.get2DDist(groupPoint, b:getUnit(1):getPoint())
    end
    table.sort(oppositeSide, _sort)
    local randomGroup = oppositeSide[1]
    if randomGroup == nil then
        return nil
    end
    local formation = math.randomFromTable(briefingRoom.mission.missionFeatures.activeGroundUnits.formationOptions)
    path[#path + 1] = mist.ground.buildWP(group:getUnit(1):getPoint(), formation, 5)
    path[#path + 1] = mist.ground.buildWP(randomGroup:getUnit(1):getPoint(), formation, 5)
    mist.goRoute(group, path)
    briefingRoom.debugPrint("Assigned Group: " .. group:getName() .. "to attack: " .. randomGroup:getName())
end

function briefingRoom.mission.missionFeatures.activeGroundUnits.activateGroundUnits(args, time)
    if not briefingRoom.mission.hasStarted then
        return time + briefingRoom.aircraftActivator.getRandomInterval()
    end
    briefingRoom.debugPrint("Assigning missions to active ground units")
    local suitableGroups = {}
    for i = 1, 2 do
        local coalitionGroups = coalition.getGroups(i)
        suitableGroups[i] = table.removeNils(table.filter(coalitionGroups, function(o, k, i)
            return not string.find(o:getName(), "-TGT-") and not string.find(o:getName(), "-STGT-") and
            #table.filter(o:getUnits(), function(j, k, i)
                return not j:hasAttribute("Ground Units Non Airdefence") or j:hasAttribute("Unarmed vehicles") or
                    j:hasAttribute("Indirect fire")
            end) == 0
        end))
        suitableGroups[i] = table.filter(suitableGroups[i], function(o, k, i) return o ~= nil end)
    end
    for i = 1, 2 do
        local opposite = (i == 1 and 2 or 1)
        for _, g in pairs(suitableGroups[i]) do
            if mist.random(10) < briefingRoom.mission.missionFeatures.activeGroundUnits.moveChance then
                briefingRoom.mission.missionFeatures.activeGroundUnits.directUnit(g, suitableGroups[opposite])
            end
        end
    end
    return nil
end

timer.scheduleFunction(briefingRoom.mission.missionFeatures.activeGroundUnits.activateGroundUnits, {},
    timer.getTime() + briefingRoom.aircraftActivator.getRandomInterval())
