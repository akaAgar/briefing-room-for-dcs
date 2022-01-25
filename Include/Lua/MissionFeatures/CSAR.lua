-- CSAR Script for DCS Ciribob - 2015
-- Version 1.9.2 - 23/04/2018
-- DCS 1.5 Compatible - Needs Mist 4.0.55 or higher!
--
-- 4 Options:
--      0 - No Limit - NO Aircraft disabling or pilot lives
--      1 - Disable Aircraft when its down - Timeout to reenable aircraft
--      2 - Disable Aircraft for Pilot when he's shot down -- timeout to reenable pilot for aircraft
--      3 - Pilot Life Limit - No Aircraft Disabling 



csar = {}

-- SETTINGS FOR MISSION DESIGNER vvvvvvvvvvvvvvvvvv
csar.enableAllslots = true  -- Doesn't require to set the Unit name check Aircraft Type and Limit below
csar.useprefix    = false  -- Use the Prefixed defined below, Requires Unit have the Prefix defined below
-- Prefix settting
csar.csarPrefix = { "helicargo", "MEDEVAC"}

-- All slot / Limit settings
csar.aircraftType = {} -- Type and limit
csar.aircraftType["SA342Mistral"] = 2
csar.aircraftType["SA342Minigun"] = 2
csar.aircraftType["SA342L"] = 2
csar.aircraftType["SA342M"] = 2
csar.aircraftType["UH-1H"] = 8
csar.aircraftType["Mi-8MT"] = 16
csar.aircraftType["UH-60L"] = 11

csar.autosmoke = false -- Automatically Smoke when CSAR helicopter is at 5 km
                  
                  
csar.csarUnits = {} -- DO NOT CHANGE

csar.bluemash = {
    "BlueMASH #1",
    "BlueMASH #2",
    "BlueMASH #3",
    "BlueMASH #4",
    "BlueMASH #5",
    "BlueMASH #6",
    "BlueMASH #7",
    "BlueMASH #8",
    "BlueMASH #9",
    "BlueMASH #10"
} -- The unit that serves as MASH for the blue side

csar.redmash = {
    "RedMASH #1",
    "RedMASH #2",
    "RedMASH #3",
    "RedMASH #4",
    "RedMASH #5",
    "RedMASH #6",
    "RedMASH #7",
    "RedMASH #8",
    "RedMASH #9",
    "RedMASH #10"
} -- The unit that serves as MASH for the red side


csar.csarMode = 0

--      0 - No Limit - NO Aircraft disabling
--      1 - Disable Aircraft when its down - Timeout to reenable aircraft
--      2 - Disable Aircraft for Pilot when he's shot down -- timeout to reenable pilot for aircraft
--      3 - Pilot Life Limit - No Aircraft Disabling -- timeout to reset lives?

csar.maxLives = 8 -- Maximum pilot lives

csar.countCSARCrash = false -- If you set to true, pilot lives count for CSAR and CSAR aircraft will count.

csar.csarOncrash = true -- If set to true, will generate a csar when crash as well.

csar.allowDownedPilotCAcontrol = true -- Set to false if you don't want to allow control by Combined arms

csar.reenableIfCSARCrashes = true -- If a CSAR heli crashes, the pilots are counted as rescued anyway. Set to false to Stop this

-- - I recommend you leave the option on below IF USING MODE 1 otherwise the
-- aircraft will be disabled for the duration of the mission
csar.disableAircraftTimeout = true -- Allow aircraft to be used after 20 minutes if the pilot isnt rescued
csar.disableTimeoutTime = 20 -- Time in minutes for TIMEOUT

csar.destructionHeight = 150 -- height in meters an aircraft will be destroyed at if the aircraft is disabled

csar.enableForAI = true -- set to false to disable AI units from being rescued.

csar.enableForRED = true -- enable for red side

csar.enableForBLUE = true -- enable for blue side

csar.enableSlotBlocking = true -- if set to true, you need to put the csarSlotBlockGameGUI.lua
-- in C:/Users/<YOUR USERNAME>/DCS/Scripts for 1.5 or C:/Users/<YOUR USERNAME>/DCS.openalpha/Scripts for 2.0
-- For missions using FLAGS and this script, the CSAR flags will NOT interfere with your mission :)

csar.bluesmokecolor = 4 -- Color of smokemarker for blue side, 0 is green, 1 is red, 2 is white, 3 is orange and 4 is blue
csar.redsmokecolor = 1 -- Color of smokemarker for red side, 0 is green, 1 is red, 2 is white, 3 is orange and 4 is blue

csar.requestdelay = 2 -- Time in seconds before the survivors will request Medevac

csar.coordtype = 3 -- Use Lat/Long DDM (0), Lat/Long DMS (1), MGRS (2), Bullseye imperial (3) or Bullseye metric (4) for coordinates.
csar.coordaccuracy = 1 -- Precision of the reported coordinates, see MIST-docs at http://wiki.hoggit.us/view/GetMGRSString
-- only applies to _non_ bullseye coords

csar.immortalcrew = true -- Set to true to make wounded crew immortal
csar.invisiblecrew = true -- Set to true to make wounded crew insvisible

csar.messageTime = 30 -- Time to show the intial wounded message for in seconds

csar.weight = 100

csar.loadDistance = 60 -- configure distance for pilot to get in helicopter in meters.

csar.radioSound = "beacon.ogg" -- the name of the sound file to use for the Pilot radio beacons. If this isnt added to the mission BEACONS WONT WORK!

csar.allowFARPRescue = true --allows pilot to be rescued by landing at a FARP or Airbase

-- SETTINGS FOR MISSION DESIGNER ^^^^^^^^^^^^^^^^^^^*

-- ***************************************************************
-- **************** Mission Editor Functions *********************
-- ***************************************************************

-----------------------------------------------------------------
-- Resets all life limits so everyone can spawn again. Usage:
-- csar.resetAllPilotLives()
--
function csar.resetAllPilotLives()

    for x, _pilot in pairs(csar.pilotLives) do

        trigger.action.setUserFlag("CSAR_PILOT" .. _pilot:gsub('%W', ''), csar.maxLives + 1)
    end

    csar.pilotLives = {}
    env.info("Pilot Lives Reset!")
end

-----------------------------------------------------------------
-- Resets all life limits so everyone can spawn again. Usage:
-- csar.resetAllPilotLives()
--
function csar.resetPilotLife(_playerName)

    csar.pilotLives[_playerName] = nil

    trigger.action.setUserFlag("CSAR_PILOT" .. _playerName:gsub('%W', ''), csar.maxLives + 1)

    env.info("Pilot life Reset!")
end


-- ***************************************************************
-- **************** BE CAREFUL BELOW HERE ************************
-- ***************************************************************

-- Sanity checks of mission designer
assert(mist ~= nil, "\n\n** HEY MISSION-DESIGNER! **\n\nMiST has not been loaded!\n\nMake sure MiST 4.0.57 or higher is running\n*before* running this script!\n")

csar.addedTo = {}

csar.downedPilotCounterRed = 0
csar.downedPilotCounterBlue = 0

csar.woundedGroups = {} -- contains the new group of units
csar.inTransitGroups = {} -- contain a table for each SAR with all units he has with the
-- original name of the killed group

csar.radioBeacons = {}

csar.smokeMarkers = {} -- tracks smoke markers for groups
csar.heliVisibleMessage = {} -- tracks if the first message has been sent of the heli being visible

csar.heliCloseMessage = {} -- tracks heli close message  ie heli < 500m distance

csar.radioBeacons = {} -- all current beacons

csar.max_units = 6 --number of pilots that can be carried

csar.currentlyDisabled = {} --stored disabled aircraft

csar.hoverStatus = {} -- tracks status of a helis hover above a downed pilot

csar.pilotDisabled = {} -- tracks what aircraft a pilot is disabled for

csar.pilotLives = {} -- tracks how many lives a pilot has

csar.takenOff = {}

function csar.tableLength(T)

    if T == nil then
        return 0
    end


    local count = 0
    for _ in pairs(T) do count = count + 1 end
    return count
end

function csar.pilotsOnboard(_heliName)
    local count = 0
    if csar.inTransitGroups[_heliName] then
        for _, _group in pairs(csar.inTransitGroups[_heliName]) do
            count = count + 1
        end
    end
    return count
end

-- Handles all world events
csar.eventHandler = {}
function csar.eventHandler:onEvent(_event)
    local status, err = pcall(function(_event)

        if _event == nil or _event.initiator == nil then
            return false

        elseif _event.id == 3 then -- taken offf

        if _event.initiator:getName() then
            csar.takenOff[_event.initiator:getName()] = true
        end

        return true
        elseif _event.id == 15 then --player entered unit

        if _event.initiator:getName() then
            csar.takenOff[_event.initiator:getName()] = nil
        end

        -- if its a sar heli, re-add check status script
        for _, _heliName in pairs(csar.csarUnits) do

            if _heliName == _event.initiator:getName() then
                -- add back the status script
                for _woundedName, _groupInfo in pairs(csar.woundedGroups) do

                    if _groupInfo.side == _event.initiator:getCoalition() then

                        --env.info(string.format("Schedule Respawn %s %s",_heliName,_woundedName))
                        -- queue up script
                        -- Schedule timer to check when to pop smoke
                 --       timer.scheduleFunction(csar.checkWoundedGroupStatus, { _heliName, _woundedName }, timer.getTime() + 5)
                    end
                end
            end
        end

        if _event.initiator:getName() and _event.initiator:getPlayerName() then

            env.info("Checking Unit - " .. _event.initiator:getName())
            csar.checkDisabledAircraftStatus({ _event.initiator:getName(), _event.initiator:getPlayerName() })
        end

        return true

        elseif (_event.id == 9 and csar.csarOncrash == false) then
            -- Pilot dead

            env.info("Event unit - Pilot Dead")

            local _unit = _event.initiator

            if _unit == nil then
                return -- error!
            end

            local _coalition = _unit:getCoalition()

            if _coalition == 1 and not csar.enableForRED then
                return --ignore!
            end

            if _coalition == 2 and not csar.enableForBLUE then
                return --ignore!
            end

            -- Catch multiple events here?
            if csar.takenOff[_event.initiator:getName()] == true or _unit:inAir() then

                if csar.doubleEjection(_unit) then
                    return
                end

                trigger.action.outTextForCoalition(_unit:getCoalition(), "MAYDAY MAYDAY! " .. _unit:getTypeName() .. " shot down. No Chute!", 10)
                csar.handleEjectOrCrash(_unit, true)
            else
                env.info("Pilot Hasnt taken off, ignore")
            end

            return

        elseif _event.id == 9 or world.event.S_EVENT_EJECTION == _event.id then
            if _event.id == 9 and csar.csarOncrash == false then 
                return     
            end
            env.info("Event unit - Pilot Ejected")

            local _unit = _event.initiator

            if _unit == nil then
                return -- error!
            end

            local _coalition = _unit:getCoalition()

            if _coalition == 1 and not csar.enableForRED then
                return --ignore!
            end

            if _coalition == 2 and not csar.enableForBLUE then
                return --ignore!
            end

            -- TODO catch ejection on runway?

            if csar.enableForAI == false and _unit:getPlayerName() == nil then

                return
            end

            if csar.takenOff[_event.initiator:getName()] ~= true and not _unit:inAir() then
                env.info("Pilot Hasnt taken off, ignore")
                return -- give up, pilot hasnt taken off
            end

            if csar.doubleEjection(_unit) then
                return
            end



            local _spawnedGroup = csar.spawnGroup(_unit)
            csar.addSpecialParametersToGroup(_spawnedGroup)

            trigger.action.outTextForCoalition(_unit:getCoalition(), "MAYDAY MAYDAY! " .. _unit:getTypeName() .. " shot down. Chute Spotted!", 10)

            local _freq = csar.generateADFFrequency()

            csar.addBeaconToGroup(_spawnedGroup:getName(), _freq)

            --handle lives and plane disabling
            csar.handleEjectOrCrash(_unit, false)

            -- Generate DESCRIPTION text
            local _text = " "
            if _unit:getPlayerName() ~= nil then
                _text = "Pilot " .. _unit:getPlayerName() .. " of " .. _unit:getName() .. " - " .. _unit:getTypeName()
            else
                _text = "AI Pilot of " .. _unit:getName() .. " - " .. _unit:getTypeName()
            end

            csar.woundedGroups[_spawnedGroup:getName()] = { side = _spawnedGroup:getCoalition(), originalUnit = _unit:getName(), frequency = _freq, desc = _text, player = _unit:getPlayerName() }

            csar.initSARForPilot(_spawnedGroup, _freq)

            return true

        elseif world.event.S_EVENT_LAND == _event.id then

            if _event.initiator:getName() then
                csar.takenOff[_event.initiator:getName()] = nil
            end

            if csar.allowFARPRescue then

                --env.info("Landing")

                local _unit = _event.initiator

                if _unit == nil then
                    env.info("Unit Nil on Landing")
                    return -- error!
                end

                csar.takenOff[_event.initiator:getName()] = nil

                local _place = _event.place

                if _place == nil then
                    env.info("Landing Place Nil")
                    return -- error!
                end
                -- Coalition == 3 seems to be a bug... unless it means contested?!
                if _place:getCoalition() == _unit:getCoalition() or _place:getCoalition() == 0 or _place:getCoalition() == 3 then
                    csar.rescuePilots(_unit)
                    --env.info("Rescued")
                    --   env.info("Rescued by Landing")

                else
                    --    env.info("Cant Rescue ")

                    env.info(string.format("airfield %d, unit %d", _place:getCoalition(), _unit:getCoalition()))
                end
            end

            return true
        end
    end, _event)
    if (not status) then
        env.error(string.format("Error while handling event %s", err), false)
    end
end

csar.lastCrash = {}

function csar.doubleEjection(_unit)

    if csar.lastCrash[_unit:getName()] then
        local _time = csar.lastCrash[_unit:getName()]

        if timer.getTime() - _time < 10 then
            env.info("Caught double ejection!")
            return true
        end
    end

    csar.lastCrash[_unit:getName()] = timer.getTime()

    return false
end

function csar.handleEjectOrCrash(_unit, _crashed)

    -- disable aircraft for ALL pilots
    if csar.csarMode == 1 then

        if csar.currentlyDisabled[_unit:getName()] ~= nil then
            return --already ejected once!
        end

        --                --mark plane as broken and unflyable
        if _unit:getPlayerName() ~= nil and csar.currentlyDisabled[_unit:getName()] == nil then

            if csar.countCSARCrash == false then
                for _, _heliName in pairs(csar.csarUnits) do

                    if _unit:getName() == _heliName then
                        -- IGNORE Crashed CSAR
                        return
                    end
                end
            end

            csar.currentlyDisabled[_unit:getName()] = { timeout = (csar.disableTimeoutTime * 60) + timer.getTime(), desc = "", noPilot = _crashed, unitId = _unit:getID(), name = _unit:getName() }

            -- disable aircraft

            trigger.action.setUserFlag("CSAR_AIRCRAFT" .. _unit:getID(), 100)

            env.info("Unit Disabled: " .. _unit:getName() .. " ID:" .. _unit:getID())
        end

    elseif csar.csarMode == 2 then -- disable aircraft for pilot

    --csar.pilotDisabled
    if _unit:getPlayerName() ~= nil and csar.pilotDisabled[_unit:getPlayerName() .. "_" .. _unit:getName()] == nil then

        if csar.countCSARCrash == false then
            for _, _heliName in pairs(csar.csarUnits) do

                if _unit:getName() == _heliName then
                    -- IGNORE Crashed CSAR
                    return
                end
            end
        end

        csar.pilotDisabled[_unit:getPlayerName() .. "_" .. _unit:getName()] = { timeout = (csar.disableTimeoutTime * 60) + timer.getTime(), desc = "", noPilot = true, unitId = _unit:getID(), player = _unit:getPlayerName(), name = _unit:getName() }

        -- disable aircraft

        -- strip special characters from name gsub('%W','')
        trigger.action.setUserFlag("CSAR_AIRCRAFT" .. _unit:getPlayerName():gsub('%W', '') .. "_" .. _unit:getID(), 100)

        env.info("Unit Disabled for player : " .. _unit:getName())
    end

    elseif csar.csarMode == 3 then -- No Disable - Just reduce player lives

    --csar.pilotDisabled
    if _unit:getPlayerName() ~= nil then

        if csar.countCSARCrash == false then
            for _, _heliName in pairs(csar.csarUnits) do

                if _unit:getName() == _heliName then
                    -- IGNORE Crashed CSAR
                    return
                end
            end
        end

        local _lives = csar.pilotLives[_unit:getPlayerName()]

        if _lives == nil then
            _lives = csar.maxLives + 1 --plus 1 because we'll use flag set to 1 to indicate NO MORE LIVES
        end

        csar.pilotLives[_unit:getPlayerName()] = _lives - 1

        trigger.action.setUserFlag("CSAR_PILOT" .. _unit:getPlayerName():gsub('%W', ''), _lives - 1)
    end
    end
end

function csar.enableAircraft(_name, _playerName)


    -- enable aircraft for ALL pilots
    if csar.csarMode == 1 then

        local _details = csar.currentlyDisabled[_name]

        if _details ~= nil then
            csar.currentlyDisabled[_name] = nil -- {timeout =  (csar.disableTimeoutTime*60) + timer.getTime(),desc="",noPilot = _crashed,unitId=_unit:getID() }

            --use flag to reenable
            trigger.action.setUserFlag("CSAR_AIRCRAFT" .. _details.unitId, 0)
        end

    elseif csar.csarMode == 2 and _playerName ~= nil then -- enable aircraft for pilot

    local _details = csar.pilotDisabled[_playerName .. "_" .. _name]

    if _details ~= nil then
        csar.pilotDisabled[_playerName .. "_" .. _name] = nil

        trigger.action.setUserFlag("CSAR_AIRCRAFT" .. _playerName:gsub('%W', '') .. "_" .. _details.unitId, 0)
    end

    elseif csar.csarMode == 3 and _playerName ~= nil then -- No Disable - Just reduce player lives

    -- give back life

    local _lives = csar.pilotLives[_playerName]

    if _lives == nil then
        _lives = csar.maxLives + 1 --plus 1 because we'll use flag set to 1 to indicate NO MORE LIVES
    else
        _lives = _lives + 1 -- give back live!

        if csar.maxLives + 1 <= _lives then
            _lives = csar.maxLives + 1 --plus 1 because we'll use flag set to 1 to indicate NO MORE LIVES
        end
    end

    csar.pilotLives[_playerName] = _lives

    trigger.action.setUserFlag("CSAR_PILOT" .. _playerName:gsub('%W', ''), _lives)
    end
end



function csar.reactivateAircraft()

    timer.scheduleFunction(csar.reactivateAircraft, nil, timer.getTime() + 5)

    -- disable aircraft for ALL pilots
    if csar.csarMode == 1 then

        for _unitName, _details in pairs(csar.currentlyDisabled) do

            if timer.getTime() >= _details.timeout then

                csar.enableAircraft(_unitName)
            end
        end

    elseif csar.csarMode == 2 then -- disable aircraft for pilot

    for _key, _details in pairs(csar.pilotDisabled) do

        if timer.getTime() >= _details.timeout then

            csar.enableAircraft(_details.name, _details.player)
        end
    end

    elseif csar.csarMode == 3 then -- No Disable - Just reduce player lives
    end
end

function csar.checkDisabledAircraftStatus(_args)

    local _name = _args[1]
    local _playerName = _args[2]

    local _unit = Unit.getByName(_name)

    --if its not the same user anymore, stop checking
    if _unit ~= nil and _unit:getPlayerName() ~= nil and _playerName == _unit:getPlayerName() then
        -- disable aircraft for ALL pilots
        if csar.csarMode == 1 then

            local _details = csar.currentlyDisabled[_unit:getName()]

            if _details ~= nil then

                local _time = _details.timeout - timer.getTime()

                if _details.noPilot then

                    if csar.disableAircraftTimeout then

                        local _text = string.format("This aircraft cannot be flow as the pilot was killed in a crash. Reinforcements in %.2dM,%.2dS\n\nIt will be DESTROYED on takeoff!", (_time / 60), _time % 60)

                        --display message,
                        csar.displayMessageToSAR(_unit, _text, 10, true)
                    else
                        --display message,
                        csar.displayMessageToSAR(_unit, "This aircraft cannot be flown again as the pilot was killed in a crash\n\nIt will be DESTROYED on takeoff!", 10, true)
                    end
                else
                    if csar.disableAircraftTimeout then
                        --display message,
                        csar.displayMessageToSAR(_unit, _details.desc .. " needs to be rescued or reinforcements arrive before this aircraft can be flown again! Reinforcements in " .. string.format("%.2dM,%.2d", (_time / 60), _time % 60) .. "\n\nIt will be DESTROYED on takeoff!", 10, true)
                    else
                        --display message,
                        csar.displayMessageToSAR(_unit, _details.desc .. " needs to be rescued before this aircraft can be flown again!\n\nIt will be DESTROYED on takeoff!", 10, true)
                    end
                end

                if csar.destroyUnit(_unit) then
                    return --plane destroyed
                else
                    --check again in 10 seconds
                    timer.scheduleFunction(csar.checkDisabledAircraftStatus, _args, timer.getTime() + 10)
                end
            end



        elseif csar.csarMode == 2 then -- disable aircraft for pilot

        local _details = csar.pilotDisabled[_unit:getPlayerName() .. "_" .. _unit:getName()]

        if _details ~= nil then

            local _time = _details.timeout - timer.getTime()

            if _details.noPilot then

                if csar.disableAircraftTimeout then

                    local _text = string.format("This aircraft cannot be flow as the pilot was killed in a crash. Reinforcements in %.2dM,%.2dS\n\nIt will be DESTROYED on takeoff!", (_time / 60), _time % 60)

                    --display message,
                    csar.displayMessageToSAR(_unit, _text, 10, true)
                else
                    --display message,
                    csar.displayMessageToSAR(_unit, "This aircraft cannot be flown again as the pilot was killed in a crash\n\nIt will be DESTROYED on takeoff!", 10, true)
                end
            else
                if csar.disableAircraftTimeout then
                    --display message,
                    csar.displayMessageToSAR(_unit, _details.desc .. " needs to be rescued or reinforcements arrive before this aircraft can be flown again! Reinforcements in " .. string.format("%.2dM,%.2d", (_time / 60), _time % 60) .. "\n\nIt will be DESTROYED on takeoff!", 10, true)
                else
                    --display message,
                    csar.displayMessageToSAR(_unit, _details.desc .. " needs to be rescued before this aircraft can be flown again!\n\nIt will be DESTROYED on takeoff!", 10, true)
                end
            end

            if csar.destroyUnit(_unit) then
                return --plane destroyed
            else
                --check again in 10 seconds
                timer.scheduleFunction(csar.checkDisabledAircraftStatus, _args, timer.getTime() + 10)
            end
        end


        elseif csar.csarMode == 3 then -- No Disable - Just reduce player lives

        local _lives = csar.pilotLives[_unit:getPlayerName()]

        if _lives == nil or _lives > 1 then

            if _lives == nil then
                _lives = csar.maxLives + 1
            end

            -- -1 for lives as we use 1 to indicate out of lives!
            local _text = string.format("CSAR ACTIVE! \n\nYou have " .. (_lives - 1) .. " lives remaining. Make sure you eject!")

            csar.displayMessageToSAR(_unit, _text, 20, true)

            return

        else

            local _text = string.format("You have run out of LIVES! Lives will be reset on mission restart or when your pilot is rescued.\n\nThis aircraft will be DESTROYED on takeoff!")

            --display message,
            csar.displayMessageToSAR(_unit, _text, 10, true)

            if csar.destroyUnit(_unit) then
                return --plane destroyed
            else
                --check again in 10 seconds
                timer.scheduleFunction(csar.checkDisabledAircraftStatus, _args, timer.getTime() + 10)
            end
        end
        end
    end
end

function csar.destroyUnit(_unit)

    --destroy if the SAME player is still in the aircraft
    -- if a new player got in it'll be destroyed in a bit anyways
    if _unit ~= nil and _unit:getPlayerName() ~= nil then

        if csar.heightDiff(_unit) > csar.destructionHeight then

            csar.displayMessageToSAR(_unit, "**** Aircraft Destroyed as the pilot needs to be rescued or you have no lives! ****", 10, true)
            --if we're off the ground then explode
            trigger.action.explosion(_unit:getPoint(), 100);

            return true
        end
        --_unit:destroy() destroy doesnt work for playes who arent the host in multiplayer
    end

    return false
end

function csar.heightDiff(_unit)

    local _point = _unit:getPoint()

    return _point.y - land.getHeight({ x = _point.x, y = _point.z })
end

csar.addBeaconToGroup = function(_woundedGroupName, _freq)

    local _group = Group.getByName(_woundedGroupName)

    if _group == nil then

        --return frequency to pool of available
        for _i, _current in ipairs(csar.usedVHFFrequencies) do
            if _current == _freq then
                table.insert(csar.freeVHFFrequencies, _freq)
                table.remove(csar.usedVHFFrequencies, _i)
            end
        end

        return
    end

    local _sound = "l10n/DEFAULT/" .. csar.radioSound

    trigger.action.radioTransmission(_sound, _group:getUnit(1):getPoint(), 0, false, _freq, 1000)

    timer.scheduleFunction(csar.refreshRadioBeacon, { _woundedGroupName, _freq }, timer.getTime() + 30)
end

csar.refreshRadioBeacon = function(_args)

    csar.addBeaconToGroup(_args[1], _args[2])
end

csar.addSpecialParametersToGroup = function(_spawnedGroup)

    -- Immortal code for alexej21
    local _setImmortal = {
        id = 'SetImmortal',
        params = {
            value = true
        }
    }
    -- invisible to AI, Shagrat
    local _setInvisible = {
        id = 'SetInvisible',
        params = {
            value = true
        }
    }

    local _controller = _spawnedGroup:getController()

    if (csar.immortalcrew) then
        Controller.setCommand(_controller, _setImmortal)
    end

    if (csar.invisiblecrew) then
        Controller.setCommand(_controller, _setInvisible)
    end
end

function csar.spawnGroup(_deadUnit)

    local _id = mist.getNextGroupId()

    local _groupName = "Downed Pilot #" .. _id

    local _side = _deadUnit:getCoalition()

    local _pos = _deadUnit:getPoint()

    local _group = {
        ["visible"] = false,
        ["groupId"] = _id,
        ["hidden"] = false,
        ["units"] = {},
        ["name"] = _groupName,
        ["task"] = {},
    }

    if _side == 2 then
        _group.units[1] = csar.createUnit(_pos.x + 50, _pos.z + 50, 120, "Soldier M4")
    else
        _group.units[1] = csar.createUnit(_pos.x + 50, _pos.z + 50, 120, "Infantry AK")
    end

    _group.category = Group.Category.GROUND;
    _group.country = _deadUnit:getCountry();

    local _spawnedGroup = Group.getByName(mist.dynAdd(_group).name)

    -- Turn off AI
    if csar.allowDownedPilotCAcontrol == false then
      trigger.action.setGroupAIOff(_spawnedGroup)
    end
    return _spawnedGroup
end


function csar.createUnit(_x, _y, _heading, _type)

    local _id = mist.getNextUnitId();

    local _name = string.format("Wounded Pilot #%s", _id)

    local _newUnit = {
        ["y"] = _y,
        ["type"] = _type,
        ["name"] = _name,
        ["unitId"] = _id,
        ["heading"] = _heading,
        ["playerCanDrive"] = false,
        ["skill"] = "Excellent",
        ["x"] = _x,
    }

    return _newUnit
end

function csar.initSARForPilot(_downedGroup, _freq)

    local _leader = _downedGroup:getUnit(1)

    local _coordinatesText = csar.getPositionOfWounded(_downedGroup)

    local
    _text = string.format("%s requests SAR at %s, beacon at %.2f KHz",
        _leader:getName(), _coordinatesText, _freq / 1000)

    local _randPercent = math.random(1, 100)

    -- Loop through all the medevac units
    for x, _heliName in pairs(csar.csarUnits) do
        local _status, _err = pcall(function(_args)
            local _unitName = _args[1]
            local _woundedSide = _args[2]
            local _medevacText = _args[3]
            local _leaderPos = _args[4]
            local _groupName = _args[5]
            local _group = _args[6]

            local _heli = csar.getSARHeli(_unitName)

            -- queue up for all SAR, alive or dead, we dont know the side if they're dead or not spawned so check
            --coalition in scheduled smoke

            if _heli ~= nil then

                -- Check coalition side
                if (_woundedSide == _heli:getCoalition()) then
                    -- Display a delayed message
                    timer.scheduleFunction(csar.delayedHelpMessage, { _unitName, _medevacText, _groupName }, timer.getTime() + csar.requestdelay)

                    -- Schedule timer to check when to pop smoke
                    timer.scheduleFunction(csar.checkWoundedGroupStatus, { _unitName, _groupName }, timer.getTime() + 1)
                end
            else
                --env.warning(string.format("Medevac unit %s not active", _heliName), false)

                -- Schedule timer for Dead unit so when the unit respawns he can still pickup units
                --timer.scheduleFunction(medevac.checkStatus, {_unitName,_groupName}, timer.getTime() + 5)
            end
        end, { _heliName, _leader:getCoalition(), _text, _leader:getPoint(), _downedGroup:getName(), _downedGroup })

        if (not _status) then
            env.warning(string.format("Error while checking with medevac-units %s", _err))
        end
    end
end

function csar.checkWoundedGroupStatus(_argument)

    local _status, _err = pcall(function(_args)
        local _heliName = _args[1]
        local _woundedGroupName = _args[2]

        local _woundedGroup = csar.getWoundedGroup(_woundedGroupName)
        local _heliUnit = csar.getSARHeli(_heliName)

        -- if wounded group is not here then message alread been sent to SARs
        -- stop processing any further
        if csar.woundedGroups[_woundedGroupName] == nil then
            return
        end

        if _heliUnit == nil then
            -- stop wounded moving, head back to smoke as target heli is DEAD

            -- in transit cleanup
            --  csar.inTransitGroups[_heliName] = nil
            return
        end

        -- double check that this function hasnt been queued for the wrong side

        if csar.woundedGroups[_woundedGroupName].side ~= _heliUnit:getCoalition() then
            return --wrong side!
        end

        if csar.checkGroupNotKIA(_woundedGroup, _woundedGroupName, _heliUnit, _heliName) then

            local _woundedLeader = _woundedGroup[1]
            local _lookupKeyHeli = _heliUnit:getID() .. "_" .. _woundedLeader:getID() --lookup key for message state tracking

            local _distance = csar.getDistance(_heliUnit:getPoint(), _woundedLeader:getPoint())

            if _distance < 3000 then

                if csar.checkCloseWoundedGroup(_distance, _heliUnit, _heliName, _woundedGroup, _woundedGroupName) == true then
                    -- we're close, reschedule
                    timer.scheduleFunction(csar.checkWoundedGroupStatus, _args, timer.getTime() + 1)
                end

            else
                csar.heliVisibleMessage[_lookupKeyHeli] = nil

                --reschedule as units arent dead yet , schedule for a bit slower though as we're far away
                timer.scheduleFunction(csar.checkWoundedGroupStatus, _args, timer.getTime() + 5)
            end
        end
    end, _argument)

    if not _status then

        env.error(string.format("error checkWoundedGroupStatus %s", _err))
    end
end

function csar.popSmokeForGroup(_woundedGroupName, _woundedLeader)
    -- have we popped smoke already in the last 5 mins
    local _lastSmoke = csar.smokeMarkers[_woundedGroupName]
    if _lastSmoke == nil or timer.getTime() > _lastSmoke then

        local _smokecolor
        if (_woundedLeader:getCoalition() == 2) then
            _smokecolor = csar.bluesmokecolor
        else
            _smokecolor = csar.redsmokecolor
        end
        trigger.action.smoke(_woundedLeader:getPoint(), _smokecolor)

        csar.smokeMarkers[_woundedGroupName] = timer.getTime() + 300 -- next smoke time
    end
end

function csar.pickupUnit(_heliUnit, _pilotName, _woundedGroup, _woundedGroupName)

    local _woundedLeader = _woundedGroup[1]

    -- GET IN!
    local _heliName = _heliUnit:getName()
    local _groups = csar.inTransitGroups[_heliName]
    local _unitsInHelicopter = csar.pilotsOnboard(_heliName)

    -- init table if there is none for this helicopter
    if not _groups then
        csar.inTransitGroups[_heliName] = {}
        _groups = csar.inTransitGroups[_heliName]
    end

    -- if the heli can't pick them up, show a message and return
    local _maxUnits = csar.aircraftType[_heliUnit:getTypeName()]
    if _maxUnits == nil then
      _maxUnits = csar.max_units
    end
    if _unitsInHelicopter + 1 > _maxUnits then
        csar.displayMessageToSAR(_heliUnit, string.format("%s, %s. We're already crammed with %d guys! Sorry!",
            _pilotName, _heliName, _unitsInHelicopter, _unitsInHelicopter), 10)
        return true
    end

    csar.inTransitGroups[_heliName][_woundedGroupName] =
    {
        originalUnit = csar.woundedGroups[_woundedGroupName].originalUnit,
        woundedGroup = _woundedGroupName,
        side = _heliUnit:getCoalition(),
        desc = csar.woundedGroups[_woundedGroupName].desc,
        player = csar.woundedGroups[_woundedGroupName].player,
    }

    Group.destroy(_woundedLeader:getGroup())

    csar.displayMessageToSAR(_heliUnit, string.format("%s: %s I'm in! Get to the MASH ASAP! ", _heliName, _pilotName), 10)

    timer.scheduleFunction(csar.scheduledSARFlight,
        {
            heliName = _heliUnit:getName(),
            groupName = _woundedGroupName
        },
        timer.getTime() + 1)

    return true
end


-- Helicopter is within 3km
function csar.checkCloseWoundedGroup(_distance, _heliUnit, _heliName, _woundedGroup, _woundedGroupName)

    local _woundedLeader = _woundedGroup[1]
    local _lookupKeyHeli = _heliUnit:getID() .. "_" .. _woundedLeader:getID() --lookup key for message state tracking

    local _pilotName = csar.woundedGroups[_woundedGroupName].desc

    local _woundedCount = 1

    local _reset = true
    if csar.autosmoke == true then
    csar.popSmokeForGroup(_woundedGroupName, _woundedLeader)
    end
    if csar.heliVisibleMessage[_lookupKeyHeli] == nil then

        csar.displayMessageToSAR(_heliUnit, string.format("%s: %s. I hear you! Damn that thing is loud! Land or hover by the smoke.", _heliName, _pilotName), 30)

        --mark as shown for THIS heli and THIS group
        csar.heliVisibleMessage[_lookupKeyHeli] = true
    end

    if (_distance < 500) then

        if csar.heliCloseMessage[_lookupKeyHeli] == nil then

            csar.displayMessageToSAR(_heliUnit, string.format("%s: %s. You're close now! Land or hover at the smoke.", _heliName, _pilotName), 10)

            --mark as shown for THIS heli and THIS group
            csar.heliCloseMessage[_lookupKeyHeli] = true
        end

        -- have we landed close enough?
        if csar.inAir(_heliUnit) == false then

            -- if you land on them, doesnt matter if they were heading to someone else as you're closer, you win! :)
            if (_distance < csar.loadDistance) then

                return csar.pickupUnit(_heliUnit, _pilotName, _woundedGroup, _woundedGroupName)
            end

        else

            local _unitsInHelicopter = csar.pilotsOnboard(_heliName)
            local _maxUnits = csar.aircraftType[_heliUnit:getTypeName()]
            if _maxUnits == nil then
              _maxUnits = csar.max_units
            end
            
            if csar.inAir(_heliUnit) and _unitsInHelicopter + 1 <= _maxUnits then

                if _distance < 8.0 then

                    --check height!
                    local _height = _heliUnit:getPoint().y - _woundedLeader:getPoint().y

                    if _height <= 20.0 then

                        local _time = csar.hoverStatus[_lookupKeyHeli]

                        if _time == nil then
                            csar.hoverStatus[_lookupKeyHeli] = 10
                            _time = 10
                        else
                            _time = csar.hoverStatus[_lookupKeyHeli] - 1
                            csar.hoverStatus[_lookupKeyHeli] = _time
                        end

                        if _time > 0 then
                            csar.displayMessageToSAR(_heliUnit, "Hovering above " .. _pilotName .. ". \n\nHold hover for " .. _time .. " seconds to winch them up. \n\nIf the countdown stops you're too far away!", 10, true)
                        else
                            csar.hoverStatus[_lookupKeyHeli] = nil
                            return csar.pickupUnit(_heliUnit, _pilotName, _woundedGroup, _woundedGroupName)
                        end
                        _reset = false
                    else
                        csar.displayMessageToSAR(_heliUnit, "Too high to winch " .. _pilotName .. " \nReduce height and hover for 10 seconds!", 5, true)
                    end
                end
            
            end
        end
    end

    if _reset then
        csar.hoverStatus[_lookupKeyHeli] = nil
    end

    return true
end



function csar.checkGroupNotKIA(_woundedGroup, _woundedGroupName, _heliUnit, _heliName)

    -- check if unit has died or been picked up
    if #_woundedGroup == 0 and _heliUnit ~= nil then

        local inTransit = false

        for _currentHeli, _groups in pairs(csar.inTransitGroups) do

            if _groups[_woundedGroupName] then
                local _group = _groups[_woundedGroupName]
                if _group.side == _heliUnit:getCoalition() then
                    inTransit = true

                    csar.displayToAllSAR(string.format("%s has been picked up by %s", _woundedGroupName, _currentHeli), _heliUnit:getCoalition(), _heliName)

                    break
                end
            end
        end


        --display to all sar
        if inTransit == false then
            --DEAD

            csar.displayToAllSAR(string.format("%s is KIA ", _woundedGroupName), _heliUnit:getCoalition(), _heliName)
        end

        --     medevac.displayMessageToSAR(_heliUnit, string.format("%s: %s is dead", _heliName,_woundedGroupName ),10)

        --stops the message being displayed again
        csar.woundedGroups[_woundedGroupName] = nil

        return false
    end

    --continue
    return true
end


function csar.scheduledSARFlight(_args)

    local _status, _err = pcall(function(_args)

        local _heliUnit = csar.getSARHeli(_args.heliName)
        local _woundedGroupName = _args.groupName

        if (_heliUnit == nil) then

            --helicopter crashed?
            -- Put intransit pilots back
            --TODO possibly respawn the guys
            if csar.reenableIfCSARCrashes then
                local _rescuedGroups = csar.inTransitGroups[_args.heliName]

                if _rescuedGroups ~= nil then

                    -- enable pilots again
                    for _, _rescueGroup in pairs(_rescuedGroups) do

                        csar.enableAircraft(_rescueGroup.originalUnit, _rescuedGroups.player)
                    end
                end
            end

            csar.inTransitGroups[_args.heliName] = nil

            return
        end

        if csar.inTransitGroups[_heliUnit:getName()] == nil or csar.inTransitGroups[_heliUnit:getName()][_woundedGroupName] == nil then
            -- Groups already rescued
            return
        end


        local _dist = csar.getClosetMASH(_heliUnit)

        if _dist == -1 then
            -- Can now rescue to FARP
            -- Mash Dead
            --  csar.inTransitGroups[_heliUnit:getName()][_woundedGroupName] = nil

            --  csar.displayMessageToSAR(_heliUnit, string.format("%s: NO MASH! The pilot died of despair!", _heliUnit:getName()), 10)

            return
        end

        if _dist < 200 and _heliUnit:inAir() == false then

            csar.rescuePilots(_heliUnit)

            return
        end

        -- end
        --queue up
        timer.scheduleFunction(csar.scheduledSARFlight,
            {
                heliName = _heliUnit:getName(),
                groupName = _woundedGroupName
            },
            timer.getTime() + 1)
    end, _args)
    if (not _status) then
        env.error(string.format("Error in scheduledSARFlight\n\n%s", _err))
    end
end

function csar.rescuePilots(_heliUnit)
    local _rescuedGroups = csar.inTransitGroups[_heliUnit:getName()]

    if _rescuedGroups == nil then
        -- Groups already rescued
        return
    end

    csar.inTransitGroups[_heliUnit:getName()] = nil

    local _txt = string.format("%s: The pilots have been taken to the\nmedical clinic. Good job!", _heliUnit:getName())

    -- enable pilots again
    for _, _rescueGroup in pairs(_rescuedGroups) do

        csar.enableAircraft(_rescueGroup.originalUnit, _rescueGroup.player)
    end

    csar.displayMessageToSAR(_heliUnit, _txt, 10)

    -- env.info("Rescued")
end


function csar.getSARHeli(_unitName)

    local _heli = Unit.getByName(_unitName)

    if _heli ~= nil and _heli:isActive() and _heli:getLife() > 0 then

        return _heli
    end

    return nil
end


-- Displays a request for medivac
function csar.delayedHelpMessage(_args)
    local status, err = pcall(function(_args)
        local _heliName = _args[1]
        local _text = _args[2]
        local _injuredGroupName = _args[3]

        local _heli = csar.getSARHeli(_heliName)

        if _heli ~= nil and #csar.getWoundedGroup(_injuredGroupName) > 0 then
            csar.displayMessageToSAR(_heli, _text, csar.messageTime)


            local _groupId = csar.getGroupId(_heli)

            if _groupId then
                trigger.action.outSoundForGroup(_groupId, "l10n/DEFAULT/CSAR.ogg")
            end

        else
            env.info("No Active Heli or Group DEAD")
        end
    end, _args)

    if (not status) then
        env.error(string.format("Error in delayedHelpMessage "))
    end

    return nil
end

function csar.displayMessageToSAR(_unit, _text, _time, _clear)

    local _groupId = csar.getGroupId(_unit)

    if _groupId then
        if _clear == true then
            trigger.action.outTextForGroup(_groupId, _text, _time, _clear)
        else
            trigger.action.outTextForGroup(_groupId, _text, _time)
        end
    end
end

function csar.getWoundedGroup(_groupName)
    local _status, _result = pcall(function(_groupName)

        local _woundedGroup = {}
        local _units = Group.getByName(_groupName):getUnits()

        for _, _unit in pairs(_units) do

            if _unit ~= nil and _unit:isActive() and _unit:getLife() > 0 then
                table.insert(_woundedGroup, _unit)
            end
        end

        return _woundedGroup
    end, _groupName)

    if (_status) then
        return _result
    else
        --env.warning(string.format("getWoundedGroup failed! Returning 0.%s",_result), false)
        return {} --return empty table
    end
end


function csar.convertGroupToTable(_group)

    local _unitTable = {}

    for _, _unit in pairs(_group:getUnits()) do

        if _unit ~= nil and _unit:getLife() > 0 then
            table.insert(_unitTable, _unit:getName())
        end
    end

    return _unitTable
end

function csar.getPositionOfWounded(_woundedGroup)

    local _woundedTable = csar.convertGroupToTable(_woundedGroup)

    local _coordinatesText = ""
    if csar.coordtype == 0 then -- Lat/Long DMTM
    _coordinatesText = string.format("%s", mist.getLLString({ units = _woundedTable, acc = csar.coordaccuracy, DMS = 0 }))

    elseif csar.coordtype == 1 then -- Lat/Long DMS
    _coordinatesText = string.format("%s", mist.getLLString({ units = _woundedTable, acc = csar.coordaccuracy, DMS = 1 }))

    elseif csar.coordtype == 2 then -- MGRS
    _coordinatesText = string.format("%s", mist.getMGRSString({ units = _woundedTable, acc = csar.coordaccuracy }))

    elseif csar.coordtype == 3 then -- Bullseye Imperial
    _coordinatesText = string.format("bullseye %s", mist.getBRString({ units = _woundedTable, ref = coalition.getMainRefPoint(_woundedGroup:getCoalition()), alt = 0 }))

    else -- Bullseye Metric --(medevac.coordtype == 4)
    _coordinatesText = string.format("bullseye %s", mist.getBRString({ units = _woundedTable, ref = coalition.getMainRefPoint(_woundedGroup:getCoalition()), alt = 0, metric = 1 }))
    end

    return _coordinatesText
end

-- Displays all active MEDEVACS/SAR
function csar.displayActiveSAR(_unitName)
    local _msg = "Active MEDEVAC/SAR:"

    local _heli = csar.getSARHeli(_unitName)

    if _heli == nil then
        return
    end

    local _heliSide = _heli:getCoalition()

    local _csarList = {}

    for _groupName, _value in pairs(csar.woundedGroups) do

        local _woundedGroup = csar.getWoundedGroup(_groupName)

        if #_woundedGroup > 0 and (_woundedGroup[1]:getCoalition() == _heliSide) then

            local _coordinatesText = csar.getPositionOfWounded(_woundedGroup[1]:getGroup())

            local _distance = csar.getDistance(_heli:getPoint(), _woundedGroup[1]:getPoint())

            table.insert(_csarList, { dist = _distance, msg = string.format("%s at %s - %.2f KHz ADF - %.3fKM ", _value.desc, _coordinatesText, _value.frequency / 1000, _distance / 1000.0) })
        end
    end

    local function sortDistance(a, b)
        return a.dist < b.dist
    end

    table.sort(_csarList, sortDistance)

    for _, _line in pairs(_csarList) do
        _msg = _msg .. "\n" .. _line.msg
    end

    csar.displayMessageToSAR(_heli, _msg, 20)
end


function csar.getClosetDownedPilot(_heli)

    local _side = _heli:getCoalition()

    local _closetGroup = nil
    local _shortestDistance = -1
    local _distance = 0
    local _closetGroupInfo = nil

    for _woundedName, _groupInfo in pairs(csar.woundedGroups) do

        local _tempWounded = csar.getWoundedGroup(_woundedName)

        -- check group exists and not moving to someone else
        if #_tempWounded > 0 and (_tempWounded[1]:getCoalition() == _side) then

            _distance = csar.getDistance(_heli:getPoint(), _tempWounded[1]:getPoint())

            if _distance ~= nil and (_shortestDistance == -1 or _distance < _shortestDistance) then


                _shortestDistance = _distance
                _closetGroup = _tempWounded[1]
                _closetGroupInfo = _groupInfo
            end
        end
    end

    return { pilot = _closetGroup, distance = _shortestDistance, groupInfo = _closetGroupInfo }
end

function csar.signalFlare(_unitName)

    local _heli = csar.getSARHeli(_unitName)

    if _heli == nil then
        return
    end

    local _closet = csar.getClosetDownedPilot(_heli)

    if _closet ~= nil and _closet.pilot ~= nil and _closet.distance < 8000.0 then

        local _clockDir = csar.getClockDirection(_heli, _closet.pilot)

        local _msg = string.format("%s - %.2f KHz ADF - %.3fM - Popping Signal Flare at your %s ", _closet.groupInfo.desc, _closet.groupInfo.frequency / 1000, _closet.distance, _clockDir)
        csar.displayMessageToSAR(_heli, _msg, 20)

        trigger.action.signalFlare(_closet.pilot:getPoint(), 1, 0)
    else
        csar.displayMessageToSAR(_heli, "No Pilots within 8KM", 20)
    end
end

function csar.displayToAllSAR(_message, _side, _ignore)

    for _, _unitName in pairs(csar.csarUnits) do

        local _unit = csar.getSARHeli(_unitName)

        if _unit ~= nil and _unit:getCoalition() == _side then

            if _ignore == nil or _ignore ~= _unitName then
                csar.displayMessageToSAR(_unit, _message, 10)
            end
        else
            -- env.info(string.format("unit nil %s",_unitName))
        end
    end
end
function csar.reqsmoke( _unitName )

    local _heli = csar.getSARHeli(_unitName)
    if _heli == nil then
        return
    end

    local _closet = csar.getClosetDownedPilot(_heli)

    if _closet ~= nil and _closet.pilot ~= nil and _closet.distance < 8000.0 then

        local _clockDir = csar.getClockDirection(_heli, _closet.pilot)

        local _msg = string.format("%s - %.2f KHz ADF - %.3fM - Popping Blue smoke at your %s ", _closet.groupInfo.desc, _closet.groupInfo.frequency / 1000, _closet.distance, _clockDir)
        csar.displayMessageToSAR(_heli, _msg, 20)
        
       local _smokecolor
        if (_closet.pilot:getCoalition() == 2) then
            _smokecolor = csar.bluesmokecolor
        else
            _smokecolor = csar.redsmokecolor
        end

         trigger.action.smoke(_closet.pilot:getPoint(), _smokecolor)
  
    else
        csar.displayMessageToSAR(_heli, "No Pilots within 8KM", 20)
    end

end

function csar.getClosetMASH(_heli)

    local _mashes = csar.bluemash

    if (_heli:getCoalition() == 1) then
        _mashes = csar.redmash
    end

    local _shortestDistance = -1
    local _distance = 0

    for _, _mashName in pairs(_mashes) do

        local _mashUnit = Unit.getByName(_mashName)

        if _mashUnit ~= nil and _mashUnit:isActive() and _mashUnit:getLife() > 0 then

            _distance = csar.getDistance(_heli:getPoint(), _mashUnit:getPoint())

            if _distance ~= nil and (_shortestDistance == -1 or _distance < _shortestDistance) then

                _shortestDistance = _distance
            end
        end
    end

    if _shortestDistance ~= -1 then
        return _shortestDistance
    else
        return -1
    end
end

function csar.checkOnboard(_unitName)
    local _unit = csar.getSARHeli(_unitName)

    if _unit == nil then
        return
    end

    --list onboard pilots

    local _inTransit = csar.inTransitGroups[_unitName]

    if _inTransit == nil or csar.tableLength(_inTransit) == 0 then
        csar.displayMessageToSAR(_unit, "No Rescued Pilots onboard", 30)
    else

        local _text = "Onboard - RTB to FARP/Airfield or MASH: "

        for _, _onboard in pairs(csar.inTransitGroups[_unitName]) do
            _text = _text .. "\n" .. _onboard.desc
        end

        csar.displayMessageToSAR(_unit, _text, 30)
    end
end

function csar.addweight( _heli )
  local cargoWeight = 0
  
  local _heliName =  _heli:getName()
  if ctld ~= nil and ctld.troopWeight ~= nil then
      -- TODO Count CTLD troops
          
  end
  ctld.troopWeight = 100
  if csar.inTransitGroups[_heliName] then
    local csarcount = 0
    for _, _group in pairs(csar.inTransitGroups[_heliName]) do
        csarcount = csarcount + 1
    end
    cargoWeight = cargoWeight + csar.weight * csarcount
  end
  
  trigger.action.setUnitInternalCargo(_heli:getName(),0 ) -- Set To  to recalculate 
  trigger.action.setUnitInternalCargo(_heli:getName(), cargoWeight)
  

end
-- Adds menuitem to all medevac units that are active
function csar.addMedevacMenuItem()
    -- Loop through all Medevac units

    timer.scheduleFunction(csar.addMedevacMenuItem, nil, timer.getTime() + 5)

    local _allHeliGroups = coalition.getGroups(coalition.side.BLUE, Group.Category.HELICOPTER)
    
    for key, val in pairs (coalition.getGroups(coalition.side.RED, Group.Category.HELICOPTER)) do
      table.insert(_allHeliGroups, val)    
    end
    
    for _key, _group in pairs (_allHeliGroups) do
      
      local _unit = _group:getUnit(1) -- Asume that there is only one unit in the flight for players
      if _unit == nil then return end
      if _unit:isExist() ~= true then return false end         
      local unitName = _unit:getName()
      
      if csar.enableAllslots == true then
      -- Enable all helicopters
        local _type = _unit:getTypeName()
        if csar.aircraftType[_type] ~= nil then
          if csar.csarUnits[_unit:getName()] == nil then
            csar.csarUnits[_unit:getName()] = _unit:getName()
            
              for _woundedName, _groupInfo in pairs(csar.woundedGroups) do
                if _groupInfo.side == _group:getCoalition() then
                
                  -- Schedule timer to check when to pop smoke
                  timer.scheduleFunction(csar.checkWoundedGroupStatus, { _unit:getName() , _woundedName }, timer.getTime() + 5)
                end
              end
          end
        end
        
      elseif csar.useprefix == true then
      --use prefix 
        local upperCaseUnitname = string.upper(unitName)
        
        for key, prefix in pairs (csar.csarPrefix) do
          local upperCasePrefix = string.upper(prefix)
          
          if string.match(upperCaseUnitname, upperCasePrefix) then
            
            if csar.csarUnits[_unit:getName()] == nil then
              csar.csarUnits[_unit:getName()] = _unit:getName() 
                for _woundedName, _groupInfo in pairs(csar.woundedGroups) do
                  if _groupInfo.side == _group:getCoalition() then
                    -- Schedule timer to check when to pop smoke
                    timer.scheduleFunction(csar.checkWoundedGroupStatus, { _unit:getName() , _woundedName }, timer.getTime() + 5)
                  end
                end
            end            
            break
          end
        end
              
      end
    
    
    end

    for _, _unitName in pairs(csar.csarUnits) do

        local _unit = csar.getSARHeli(_unitName)

        if _unit ~= nil then

            local _groupId = csar.getGroupId(_unit)

            if _groupId then

                if csar.addedTo[tostring(_groupId)] == nil then

                    csar.addedTo[tostring(_groupId)] = true

                    local _rootPath = missionCommands.addSubMenuForGroup(_groupId, "CSAR")

                    missionCommands.addCommandForGroup(_groupId, "List Active CSAR", _rootPath, csar.displayActiveSAR,
                        _unitName)

                    missionCommands.addCommandForGroup(_groupId, "Check Onboard", _rootPath, csar.checkOnboard, _unitName)

                    missionCommands.addCommandForGroup(_groupId, "Request Signal Flare", _rootPath, csar.signalFlare, _unitName)
                    missionCommands.addCommandForGroup(_groupId, "Request Smoke", _rootPath, csar.reqsmoke, _unitName)
                    
                end
            end
        else
            -- env.info(string.format("unit nil %s",_unitName))
        end
    end

    return
end

--get distance in meters assuming a Flat world
function csar.getDistance(_point1, _point2)

    local xUnit = _point1.x
    local yUnit = _point1.z
    local xZone = _point2.x
    local yZone = _point2.z

    local xDiff = xUnit - xZone
    local yDiff = yUnit - yZone

    return math.sqrt(xDiff * xDiff + yDiff * yDiff)
end

-- 200 - 400 in 10KHz
-- 400 - 850 in 10 KHz
-- 850 - 1250 in 50 KHz
function csar.generateVHFrequencies()

    --ignore list
    --list of all frequencies in KHZ that could conflict with
    -- 191 - 1290 KHz, beacon range
    local _skipFrequencies = {
        745, --Astrahan
        381,
        384,
        300.50,
        312.5,
        1175,
        342,
        735,
        300.50,
        353.00,
        440,
        795,
        525,
        520,
        690,
        625,
        291.5,
        300.50,
        435,
        309.50,
        920,
        1065,
        274,
        312.50,
        580,
        602,
        297.50,
        750,
        485,
        950,
        214,
        1025, 730, 995, 455, 307, 670, 329, 395, 770,
        380, 705, 300.5, 507, 740, 1030, 515,
        330, 309.5,
        348, 462, 905, 352, 1210, 942, 435,
        324,
        320, 420, 311, 389, 396, 862, 680, 297.5,
        920, 662,
        866, 907, 309.5, 822, 515, 470, 342, 1182, 309.5, 720, 528,
        337, 312.5, 830, 740, 309.5, 641, 312, 722, 682, 1050,
        1116, 935, 1000, 430, 577
    }

    csar.freeVHFFrequencies = {}
    csar.usedVHFFrequencies = {}

    local _start = 200000

    -- first range
    while _start < 400000 do

        -- skip existing NDB frequencies
        local _found = false
        for _, value in pairs(_skipFrequencies) do
            if value * 1000 == _start then
                _found = true
                break
            end
        end


        if _found == false then
            table.insert(csar.freeVHFFrequencies, _start)
        end

        _start = _start + 10000
    end

    _start = 400000
    -- second range
    while _start < 850000 do

        -- skip existing NDB frequencies
        local _found = false
        for _, value in pairs(_skipFrequencies) do
            if value * 1000 == _start then
                _found = true
                break
            end
        end

        if _found == false then
            table.insert(csar.freeVHFFrequencies, _start)
        end

        _start = _start + 10000
    end

    _start = 850000
    -- third range
    while _start <= 1250000 do

        -- skip existing NDB frequencies
        local _found = false
        for _, value in pairs(_skipFrequencies) do
            if value * 1000 == _start then
                _found = true
                break
            end
        end

        if _found == false then
            table.insert(csar.freeVHFFrequencies, _start)
        end

        _start = _start + 50000
    end
end

function csar.generateADFFrequency()

    if #csar.freeVHFFrequencies <= 3 then
        csar.freeVHFFrequencies = csar.usedVHFFrequencies
        csar.usedVHFFrequencies = {}
    end

    local _vhf = table.remove(csar.freeVHFFrequencies, math.random(#csar.freeVHFFrequencies))

    return _vhf
    --- return {uhf=_uhf,vhf=_vhf}
end

function csar.inAir(_heli)

    if _heli:inAir() == false then
        return false
    end

    -- less than 5 cm/s a second so landed
    -- BUT AI can hold a perfect hover so ignore AI
    if mist.vec.mag(_heli:getVelocity()) < 0.05 and _heli:getPlayerName() ~= nil then
        return false
    end
    return true
end

function csar.getClockDirection(_heli, _crate)

    -- Source: Helicopter Script - Thanks!

    local _position = _crate:getPosition().p -- get position of crate
    local _playerPosition = _heli:getPosition().p -- get position of helicopter
    local _relativePosition = mist.vec.sub(_position, _playerPosition)

    local _playerHeading = mist.getHeading(_heli) -- the rest of the code determines the 'o'clock' bearing of the missile relative to the helicopter

    local _headingVector = { x = math.cos(_playerHeading), y = 0, z = math.sin(_playerHeading) }

    local _headingVectorPerpendicular = { x = math.cos(_playerHeading + math.pi / 2), y = 0, z = math.sin(_playerHeading + math.pi / 2) }

    local _forwardDistance = mist.vec.dp(_relativePosition, _headingVector)

    local _rightDistance = mist.vec.dp(_relativePosition, _headingVectorPerpendicular)

    local _angle = math.atan2(_rightDistance, _forwardDistance) * 180 / math.pi

    if _angle < 0 then
        _angle = 360 + _angle
    end
    _angle = math.floor(_angle * 12 / 360 + 0.5)
    if _angle == 0 then
        _angle = 12
    end

    return _angle
end

function csar.getGroupId(_unit)

    local _unitDB = mist.DBs.unitsById[tonumber(_unit:getID())]
    if _unitDB ~= nil and _unitDB.groupId then
        return _unitDB.groupId
    end

    return nil
end

csar.generateVHFrequencies()

-- Schedule timer to add radio item
timer.scheduleFunction(csar.addMedevacMenuItem, nil, timer.getTime() + 5)

if csar.disableAircraftTimeout then
    -- Schedule timer to reactivate things
    timer.scheduleFunction(csar.reactivateAircraft, nil, timer.getTime() + 5)
end

world.addEventHandler(csar.eventHandler)

env.info("CSAR event handler added")

--save CSAR MODE
trigger.action.setUserFlag("CSAR_MODE", csar.csarMode)

-- disable aircraft
if csar.enableSlotBlocking then

    trigger.action.setUserFlag("CSAR_SLOTBLOCK", 100)

    env.info("CSAR Slot block enabled")
end
