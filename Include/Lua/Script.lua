-- ==========================================================================
-- This file is part of Briefing Room for DCS World, a mission
-- generator for DCS World, by @akaAgar (https://github.com/akaAgar/briefing-room-for-dcs)

-- Briefing Room for DCS World is free software: you can redistribute it
-- and/or modify it under the terms of the GNU General Public License
-- as published by the Free Software Foundation, either version 3 of
-- the License, or (at your option) any later version.

-- Briefing Room for DCS World is distributed in the hope that it will
-- be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
-- of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
-- GNU General Public License for more details.

-- You should have received a copy of the GNU General Public License
-- along with Briefing Room for DCS World. If not, see https://www.gnu.org/licenses/
-- ==========================================================================

-- ===================================================================================
-- SUMMARY
-- ===================================================================================

-- 1 - Initialization and extensions
--   1.1 - Constants and enumerations
--   1.2 - Initialization
--   1.3 - Lua extensions
--   1.4 - DCS World extensions
-- 2 - "Pseudo-classes" tables
--   2.1 - Radio manager
--   2.2 - Aircraft activator
-- 3 - Mission
--   3.1 - Core mission script
--   3.2 - Mission functions
--   3.3 - Mission f10 menu
--   3.4 - Extension and mission features scripts
--   3.5 - Target unit lists initialization
--   3.6 - Mission event handler

-- ===================================================================================
-- 1.1 - CONSTANTS AND ENUMERATIONS
-- ===================================================================================

METERS_TO_NM = 0.000539957 -- number of nautical miles in a meter
NM_TO_METERS = 1852.0 -- number of meters in a nautical mile
TWO_PI = math.pi * 2 -- two times Pi

brMissionStatus =
{
  IN_PROGRESS = 1,
  COMPLETE = 2,
  FAILED = 3
}

brMissionType =
{
  SINGLE_PLAYER = 1,
  COOPERATIVE = 2,
  VERSUS = 3
}

-- ===================================================================================
-- 1.2 - INITIALIZATION
-- ===================================================================================

briefingRoom = { } -- main BriefingRoom table
briefingRoom.printDebugMessages = false -- is the debug mode enabled?

function briefingRoom.debugPrint(message, duration)
  if not briefingRoom.printDebugMessages then return end -- do not print debug messages if not in debug mode

  message = message or ""
  message = "BRIEFINGROOM: "..tostring(message)
  duration = duration or 3

  trigger.action.outText(message, duration, false)
  env.info(message, false)
end

-- ===================================================================================
-- 1.3 - LUA EXTENSIONS
-- Provides additional functions to Lua
-- ===================================================================================

function math.clamp(val, min, max) -- makes sure the value is between min and max
  return math.min(math.max(val, min), max)
end

function math.randomFloat(min, max) -- returns a random floating-point number between min and max
  if min >= max then return a end
  return min + math.random() * (max - min)
end

function math.randomFloatTable(t) -- returns a random floating point number between t[1] and t[2]
  return math.randomFloat(t[1], t[2])
end

function math.randomFromTable(t) -- returns a random value from table t
  return t[math.random(#t)]
end

function math.randomPointInCircle(center, radius) -- returns a random point in circle of center center and of radius radius
  local dist = math.random() * radius
  local angle = math.random() * TWO_PI

  local x = center.x + math.cos(angle) * dist
  local y = center.y + math.sin(angle) * dist

  return { ["x"] = x, ["y"] = y }
end

function string.endsWith(str, needle) -- returns true if string str ends with needle
  return needle == "" or str:sub(-#needle) == needle
end

function string.replace(str, repTable) -- search a string for all keys in a table and replace them with the matching value
  for k,v in pairs(repTable) do
    str = string.gsub(str, k, v)
  end
  return str
end

function string.split(str, separator)
  separator = separator or "%s"

  local t = { }
  for s in string.gmatch(str, "([^"..separator.."]+)") do
    table.insert(t, s)
  end

  return t
end

-- returns true if string str starts with needle
function string.startsWith(str, needle)
  return str:sub(1, #needle) == needle
end

-- returns the value matching the case-insensitive key in enumTable
function string.toEnum(str, enumTable, defaultVal)
  local cleanStr = string.trim(string.lower(str))

  for key,val in pairs(enumTable) do
    if key:lower() == cleanStr then return val end
  end

  return defaultVal
end

-- returns string str withtout leading and closing spaces
function string.trim(str)
  return str:match "^%s*(.-)%s*$"
end

-- converts a value to a boolean
function toboolean(val)
  if val == nil or val == 0 or val == false then return false end
  if type(val) == "string" and string.lower(val) == "false" then return false end
  return true
end

-- like the built-in "tonumber" functions, but returns 0 instead of nil in case of an error
function tonumber0(val)
  local numVal = tonumber(val)
  if numVal == nil then return 0 end
  return numVal
end

-- returns true if table t contains value val
function table.contains(t, val)
  for _,v in pairs(t) do
    if v == val then return true end
  end
  return false
end

-- returns true if table t contains key key
function table.containsKey(t, key)
  for k,_v in pairs(t) do
    if k == key then return true end
  end
  return false
end

function table.createFromRandomElements(valTable, count) -- creates a new table which countains count elements from table valTable
  local t = { }
  for i=1,count do table.insert(t, math.randomFromTable(valTable)) end
  return t
end

-- creates a new table which countains count times the value val
function table.createFromSameElement(val, count)
  local t = { }
  for i=1,count do table.insert(t, val) end
  return t
end

-- Returns a deep copy of the table, doesn't work with recursive tables
-- Code from http://lua-users.org/wiki/CopyTable
function table.deepCopy(orig)
  if type(orig) ~= 'table' then return orig end

  local copy = {}
  for orig_key, orig_value in next, orig, nil do
    copy[deepcopy(orig_key)] = deepcopy(orig_value)
  end
  setmetatable(copy, deepcopy(getmetatable(orig)))

  return copy
end

-- Returns the key associated to a value in a table, or nil if not found
function table.getKeyFromValue(t, val)
  for k,v in pairs(t) do
    if v == val then return k end
  end
  return nil
end

-- Removes one instance of a value from a table
function table.removeValue(t, val)
  for k,v in pairs(t)do
    if v == val then
      table.remove(t, k)
      return
    end
  end
end

-- Shuffles a table
function table.shuffle(t)
  local len, random = #t, math.random
  for i = len, 2, -1 do
    local j = random( 1, i )
    t[i], t[j] = t[j], t[i]
  end
  return t
end

-- ===================================================================================
-- 1.4 - DCS WORLD EXTENSIONS
-- Provides additional functions to DCS World scripting
-- ===================================================================================

dcsExtensions = { }

-- Is an unit alive?
function dcsExtensions.isUnitAlive(name)
  if name == nil then return false end
  local unit = Unit.getByName(name)
  if unit == nil then return false end
  if unit:isActive() == false then return false end
  if unit:getLife() < 1 then return false end

  return true
end

-- Returns a table with all units controlled by a player
function dcsExtensions.getAllPlayers()
  local players = { }
  
  for i=1,2 do
    for _,g in pairs(coalition.getGroups(i)) do
      for __,u in pairs(g:getUnits()) do
        if u:getPlayerName() ~= nil then
          table.insert(players, u)
        end
      end
    end
  end

  return players
end

-- Returns the distance between two vec2s
function dcsExtensions.getDistance(vec2a, vec2b)
  return math.sqrt(math.pow(vec2a.x - vec2b.x, 2) + math.pow(vec2a.y - vec2b.y, 2))
end

-- Returns the group with ID id, or nil if no group with this ID is found
function dcsExtensions.getGroupByID(id)
  for i=1,2 do
    for _,g in pairs(coalition.getGroups(i)) do
      if g:getID() == id then return g end
    end
  end

  return nil
end

-- Returns the first unit alive in group with ID groupID, or nil if group doesn't exist or is completely destroyed
function dcsExtensions.getAliveUnitInGroup(groupID)
  local g = dcsExtensions.getGroupByID(groupID)
  if g == nil then return nil end

  for __,u in ipairs(g:getUnits()) do
    if u:getLife() >= 1 and u:isActive() then
      return u
    end
  end

  return nil
end

-- Returns all units belonging to the given coalition
function dcsExtensions.getCoalitionUnits(coalID)
  local units = { }
  for _,g in pairs(coalition.getGroups(coalID)) do
    for __,u in pairs(g:getUnits()) do
      if u:isActive() then
        if u:getLife() >= 1 then
          table.insert(units, u)
        end
      end
    end
  end

  return units
end

-- Returns the vec3 position of the first unit alive in group with ID id
function dcsExtensions.getGroupLocationByID(id)
  local g = dcsExtensions.getGroupByID(id)
  if g == nil then
    return nil
  end

  for _,unit in pairs(g:getUnits()) do
    if unit:getLife() >= 1 then return unit:getPoint() end
  end

  return nil
end

-- Returns the unit with ID id, or nil if no unit with this ID is found
function dcsExtensions.getUnitByID(id)
  for i=1,2 do
    for _,g in pairs(coalition.getGroups(i)) do
      for __,u in pairs(g:getUnits()) do
        if u:getID() == id then
          return u
        end
      end
    end
  end

  return nil
end

-- function dcsExtensions.getValidPointInRegion(center, radius, validSurfaceTypes)
--   for i=1,50 do -- only look for a valid point 50 times to avoid endless loops
--     local position = math.randomPointInCircle(center, radius)

--     if validSurfaceTypes == nil then return position -- no valid surface types provided, so any surface is valid
--     else
--       local surfType = land.getSurfaceType(center)
--       if table.contains(validSurfaceTypes, surfType) then return position end
--     end
--   end

--   return nil -- .no valid point found, return nil
-- end

-- Converts a timecode (in seconds since midnight) in a hh:mm:ss string
function dcsExtensions.timeToHMS(timecode)
  local h = math.floor(timecode / 3600)
  timecode = timecode - h * 3600
  local m = math.floor(timecode / 60)
  timecode = timecode - m * 60
  local s = timecode

  return string.format("%.2i:%.2i:%.2i", h, m, s)
end

-- Converts a pair of x, y coordinates or a vec3 to a vec2
function dcsExtensions.toVec2(xOrVector, y)
  if y == nil then
    if xOrVector.z then return { ["x"] = xOrVector.x, ["y"] = xOrVector.z } end
    return { ["x"] = pxOrVector1.x, ["y"] = xOrVector.y } -- return xOrVector if it was already a vec2
  else
    return { ["x"] = xOrVector, ["y"] = y }
  end
end

-- Converts a triplet of x, y, z coordinates or a vec2 to a vec3
function dcsExtensions.toVec3(xOrVector, y, z)
  if y == nil or z == nil then
    if xOrVector.z then return { ["x"] = xOrVector.x, ["y"] = xOrVector.y, ["z"] = xOrVector.z } end  -- return xOrVector if it was already a vec3
    return { ["x"] = pxOrVector1.x, ["y"] = 0, ["z"] = xOrVector.y }
  else
    return { ["x"] = xOrVector, ["y"] = y, ["z"] = z }
  end
end

-- Converts a vec2 or ver3 into a human-readable string
function dcsExtensions.vectorToString(vec)
  if vec.z == nil then -- no Z coordinate, vec is a Vec2
    return tostring(vec.x)..","..tostring(vec.y)
  else
    return tostring(vec.x)..","..tostring(vec.y)..","..tostring(vec.z)
  end
end

-- Turns a vec2 to a string with LL/MGRS coordinates
-- Based on code by Bushmanni - https://forums.eagle.ru/showthread.php?t=99480
function dcsExtensions.vec2ToStringCoordinates(vec2)
  local pos = { x = vec2.x, y = 0, z = vec2.y }
  local cooString = ""

  local LLposN, LLposE = coord.LOtoLL(pos)
  local LLposfixN, LLposdegN = math.modf(LLposN)
  LLposdegN = LLposdegN * 60
  local LLposdegN2, LLposdegN3 = math.modf(LLposdegN)
  local LLposdegN3Decimal = LLposdegN3 * 1000
  LLposdegN3 = LLposdegN3 * 60

  local LLposfixE, LLposdegE = math.modf(LLposE)
  LLposdegE = LLposdegE * 60
  local LLposdegE2, LLposdegE3 = math.modf(LLposdegE)
  local LLposdegE3Decimal = LLposdegE3 * 1000
  LLposdegE3 = LLposdegE3 * 60

  local LLns = "N"
  if LLposfixN < 0 then LLns = "S" end
  local LLew = "E"
  if LLposfixE < 0 then LLew = "W" end

  local LLposNstring = LLns.." "..string.format("%.2i°%.2i'%.2i''", LLposfixN, LLposdegN2, LLposdegN3)
  local LLposEstring = LLew.." "..string.format("%.3i°%.2i'%.2i''", LLposfixE, LLposdegE2, LLposdegE3)
  cooString = "L/L: "..LLposNstring.." "..LLposEstring

  local LLposNstring = LLns.." "..string.format("%.2i°%.2i.%.3i", LLposfixN, LLposdegN2, LLposdegN3Decimal)
  local LLposEstring = LLew.." "..string.format("%.3i°%.2i.%.3i", LLposfixE, LLposdegE2, LLposdegE3Decimal)
  cooString = cooString.."\nL/L: "..LLposNstring.." "..LLposEstring

  local mgrs = coord.LLtoMGRS(LLposN, LLposE)
  local mgrsString = mgrs.MGRSDigraph.." "..mgrs.UTMZone.." "..tostring(mgrs.Easting).." "..tostring(mgrs.Northing)
  cooString = cooString.."\nMGRS: "..mgrsString

  return cooString
end

-- ===================================================================================
-- 2.1 - RADIO MANAGER
-- Plays radio messages (text and audio)
-- ===================================================================================

briefingRoom.radioManager = { }
briefingRoom.radioManager.ANSWER_DELAY = { 4, 6 } -- min/max time to get a answer to a radio message, in seconds
briefingRoom.radioManager.sound = $RADIOSOUNDS$ -- can ogg files be played

function briefingRoom.radioManager.getAnswerDelay()
  return math.randomFloat(briefingRoom.radioManager.ANSWER_DELAY[1], briefingRoom.radioManager.ANSWER_DELAY[2])
end

-- Estimates the time (in seconds) required for the player to read a message
function briefingRoom.radioManager.getReadingTime(message)
  message = message or ""
  messsage = tostring(message)

  return math.max(3.0, #message / 10.7) -- 10.7 letters per second, minimum length 3.0 seconds
end

function briefingRoom.radioManager.play(message, oggFile, delay, functionToRun, functionParameters)
  delay = delay or 0
  local argsTable = { ["message"] = message, ["oggFile"] = oggFile, ["functionToRun"] = functionToRun, ["functionParameters"] = functionParameters }

  if delay > 0 then -- a delay was provided, schedule the radio message
    timer.scheduleFunction(briefingRoom.radioManager.doRadioMessage, argsTable, timer.getTime() + delay)
  else -- no delay, play the message at once
    briefingRoom.radioManager.doRadioMessage(argsTable, nil)
  end
end

function briefingRoom.radioManager.doRadioMessage(args, time)
  if args.message ~= nil then -- a message was provided, print it
    args.message = tostring(args.message)
    local duration = briefingRoom.radioManager.getReadingTime(args.message)
    trigger.action.outTextForCoalition($PLAYERCOALITION$, args.message, duration, false)
  end

  if args.oggFile ~= nil and briefingRoom.radioManager.sound then -- a sound was provided and radio sounds are enabled, play it
    trigger.action.outSoundForCoalition($PLAYERCOALITION$, args.oggFile..".ogg")
  else -- else play the default sound
    trigger.action.outSoundForCoalition($PLAYERCOALITION$, "Radio0.ogg")
  end

  if args.functionToRun ~= nil then -- a function was provided, run it
    args.functionToRun(args.functionParameters)
  end

  return nil -- disable scheduling, if any
end

-- ===================================================================================
-- 2.2 - AIRCRAFT ACTIVATOR
-- Activator aircraft flight groups gradually during the mission
-- ===================================================================================

briefingRoom.aircraftActivator = { }
briefingRoom.aircraftActivator.INTERVAL = { 10, 20 } -- min/max interval (in seconds) between two updates
briefingRoom.aircraftActivator.currentQueue = { } -- current queue of aircraft to spawn every INTERVAL seconds
briefingRoom.aircraftActivator.extraQueues = { } -- additional queues of aircraft

function briefingRoom.aircraftActivator.getRandomInterval()
  return math.random(briefingRoom.aircraftActivator.INTERVAL[1], briefingRoom.aircraftActivator.INTERVAL[2])
end

function briefingRoom.aircraftActivator.pushNextQueue()
  if #briefingRoom.aircraftActivator.extraQueues == 0 then -- no extra queues available
    briefingRoom.debugPrint("Tried to push extra aircraft to the activation queue, but found none")
    return
  end

  -- add aircraft in the next extra queue to the current queue
  for _,g in ipairs(briefingRoom.aircraftActivator.extraQueues[1]) do
    briefingRoom.debugPrint("Pushed aircraft group #"..tostring(g).." into the activation queue")
    table.insert(briefingRoom.aircraftActivator.currentQueue, g)
  end
  table.remove(briefingRoom.aircraftActivator.extraQueues, 1) -- remove the added extra queue
end

-- Every INTERVAL seconds, check for aircraft groups to activate in the queue
function briefingRoom.aircraftActivator.update(args, time)
  briefingRoom.debugPrint("Looking for aircraft groups to activate, found "..tostring(#briefingRoom.aircraftActivator.currentQueue), 1)
  if #briefingRoom.aircraftActivator.currentQueue == 0 then -- no aircraft in the queue at the moment
    return time + briefingRoom.aircraftActivator.getRandomInterval() -- schedule next update and return
  end

  local acGroup = dcsExtensions.getGroupByID(briefingRoom.aircraftActivator.currentQueue[1]) -- get the group
  if acGroup ~= nil then -- activate the group, if it exists
    acGroup:activate()
    briefingRoom.debugPrint("Activating aircraft group "..acGroup:getName())
  else
    briefingRoom.debugPrint("Failed to activate aircraft group "..tostring(briefingRoom.aircraftActivator.currentQueue[1]))
  end
  table.remove(briefingRoom.aircraftActivator.currentQueue, 1) -- remove the ID from the queue

  return time + briefingRoom.aircraftActivator.getRandomInterval() -- schedule next update
end

-- ===================================================================================
-- 3.1 - CORE MISSION SCRIPT
-- Main mission table
-- ===================================================================================

briefingRoom.mission = { } -- main Briefing room mission table
briefingRoom.mission.hasStarted = false -- has at least one player taken off?
briefingRoom.mission.objectives = { } -- table which will hold the mission objectives
briefingRoom.mission.objectivesAreStatic = $STATICOBJECTIVE$ -- true if the objective is static object, false if it isn't (which means it's an unit)
briefingRoom.mission.status = brMissionStatus.IN_PROGRESS -- current status of the mission
briefingRoom.mission.parameters = { } -- various mission parameters
briefingRoom.mission.parameters.targetUnitsAttributesRequired = { } -- units in target groups must have AT LEAST ONE of these attributes to be a valid target
briefingRoom.mission.parameters.targetUnitsAttributesIgnored = { } -- units in target groups must have NONE of these attributes to be a valid target

-- === Mission script generated by BriefingRoom begins here ===
$CORELUA$
-- === Mission script generated by BriefingRoom ends here ===

briefingRoom.mission.objectivesLeft = #briefingRoom.mission.objectives -- number of objectives left to complete

-- ===================================================================================
-- 3.2 - MISSION FUNCTIONS
-- Script ran when the mission starts to create the F10 menu
-- ===================================================================================

briefingRoom.mission.functions = { }
briefingRoom.mission.functions.onUnitDestroyed = nil -- defined by objectives

function briefingRoom.mission.functions.beginMission()
  if briefingRoom.mission.hasStarted then return end -- mission has already started, do nothing

  briefingRoom.debugPrint("Mission has started")

  briefingRoom.mission.hasStarted = true
  -- enable the aircraft activator and start spawning aircraft
  timer.scheduleFunction(briefingRoom.aircraftActivator.update, nil, timer.getTime() + briefingRoom.aircraftActivator.getRandomInterval())
end

function briefingRoom.mission.functions.completeObjective(index)
  -- mission already completed or failed
  if briefingRoom.mission.status ~= brMissionStatus.IN_PROGRESS then return end
  -- objective already completed or failed
  if briefingRoom.mission.objectives[index].status ~= brMissionStatus.IN_PROGRESS then return end

  briefingRoom.debugPrint("Objective "..tostring(index).." marked as complete")
  briefingRoom.mission.objectives[index].status = brMissionStatus.COMPLETE
  briefingRoom.mission.objectivesLeft = briefingRoom.mission.objectivesLeft - 1
  briefingRoom.aircraftActivator.pushNextQueue() -- activate next batch of aircraft (so more enemy CAP will pop up)

  -- add a little delay before playing the "mission/objective complete" sounds to make sure all "target destroyed", "target photographed" sounds are done playing
  if briefingRoom.mission.objectivesLeft <= 0 then
    briefingRoom.debugPrint("Mission marked as complete")
    briefingRoom.mission.status = brMissionStatus.COMPLETE
    briefingRoom.radioManager.play("Excellent work! Mission complete, you may return to base.", "RadioHQMissionComplete", math.random(6, 8))
  else
    briefingRoom.radioManager.play("Good job! Objective complete, proceed to next objective.", "RadioHQObjectiveComplete", math.random(6, 8))
  end
end

function briefingRoom.mission.functions.getMissionStatus()
  local msnStatus = ""
  local msnSound = ""

  if briefingRoom.mission.status == brMissionStatus.COMPLETE then
    msnStatus = "Mission complete, you may return to base.\n\n"
    msnSound = "RadioHQMissionStatusComplete"
  elseif briefingRoom.mission.status == brMissionStatus.FAILED then
    msnStatus = "Mission failed, return to base for debriefing.\n\n"
    msnSound = "RadioHQMissionStatusFailed"
  else
    msnStatus = "Mission is still in progress.\n\n"
    msnSound = "RadioHQMissionStatusInProgress"
  end

  for i,o in ipairs(briefingRoom.mission.objectives) do
    if o.status == brMissionStatus.COMPLETE then
      msnStatus = msnStatus.."[DONE]"
    elseif o.status == brMissionStatus.FAILED then
      msnStatus = msnStatus.."[FAIL]"
    else
      msnStatus = msnStatus.."[    ]"
    end

    local objectiveProgress = ""
    if o.unitsCount > 0 then
      local targetsDone = math.max(0, briefingRoom.mission.objectives[i].unitsCount - #briefingRoom.mission.objectives[i].unitsID)
      objectiveProgress = " ("..tostring(targetsDone).."/"..tostring(briefingRoom.mission.objectives[i].unitsCount)..")"
    end

    msnStatus = msnStatus.." "..o.task..objectiveProgress.."\n"
  end

  briefingRoom.radioManager.play("Command, require update on mission status.", "RadioPilotMissionStatus")
  briefingRoom.radioManager.play(msnStatus, msnSound, briefingRoom.radioManager.getAnswerDelay())
end

function briefingRoom.mission.functions.getWaypointCoordinates(index)
  local cooMessage = dcsExtensions.vec2ToStringCoordinates(briefingRoom.mission.objectives[index].waypoint)
  briefingRoom.radioManager.play("Command, request confirmation of waypoint "..briefingRoom.mission.objectives[index].name.." coordinates.", "RadioPilotWaypointCoordinates")
  briefingRoom.radioManager.play("Acknowledged, transmitting waypoint "..briefingRoom.mission.objectives[index].name.." coordinates.\n\n"..cooMessage, "RadioHQWaypointCoordinates", briefingRoom.radioManager.getAnswerDelay())
end

-- ===================================================================================
-- 3.3 - MISSION F10 MENU
-- Script ran when the mission starts to create the F10 menu
-- ===================================================================================

briefingRoom.f10Menu = { }
briefingRoom.f10Menu.objectives = { }

do
  missionCommands.addCommandForCoalition($PLAYERCOALITION$, "Mission status", nil, briefingRoom.mission.functions.getMissionStatus, nil)

  for i,o in ipairs(briefingRoom.mission.objectives) do
    briefingRoom.f10Menu.objectives[i] = missionCommands.addSubMenuForCoalition($PLAYERCOALITION$, "Objective "..o.name, nil)
    missionCommands.addCommandForCoalition($PLAYERCOALITION$, "Require waypoint coordinates", briefingRoom.f10Menu.objectives[i], briefingRoom.mission.functions.getWaypointCoordinates, i)
  end
end

-- ===================================================================================
-- 3.4 - EXTENSION AND MISSION FEATURES SCRIPTS
-- Lua included from the mission features
-- ===================================================================================

briefingRoom.extensions = { } -- extensions table
briefingRoom.mission.features = { } -- mission features table

-- === Mission script generated by BriefingRoom begins here ===
$LUASETTINGS$
$INCLUDEDLUA$
-- === Mission script generated by BriefingRoom ends here ===

-- ===================================================================================
-- 3.5 - TARGET UNIT LISTS INITIALIZATION
-- Lua included from the mission features
-- ===================================================================================

do
  for index,objective in ipairs(briefingRoom.mission.objectives) do
    briefingRoom.mission.objectives[index].unitsID = { }

    local group = dcsExtensions.getGroupByID(briefingRoom.mission.objectives[index].groupID)
    if group ~= nil then
      for _,unit in ipairs(group:getUnits()) do
        local validUnit = true

        if #briefingRoom.mission.parameters.targetUnitsAttributesRequired > 0 then
          validUnit = false
          for __,a in ipairs(briefingRoom.mission.parameters.targetUnitsAttributesRequired) do
            if unit:hasAttribute(a) then
              validUnit = true
            end
          end
        end

        if #briefingRoom.mission.parameters.targetUnitsAttributesIgnored > 0 then
          for __,a in ipairs(briefingRoom.mission.parameters.targetUnitsAttributesIgnored) do
            if unit:hasAttribute(a) then
              validUnit = false
            end
          end
        end

        if validUnit then
          table.insert(briefingRoom.mission.objectives[index].unitsID, unit:getID())
        end
      end

      if #briefingRoom.mission.objectives[index].unitsID == 0 then -- no valid unit found, add all units so at least we have a target
        for _,unit in ipairs(group:getUnits()) do
          table.insert(briefingRoom.mission.objectives[index].unitsID, unit:getID())
        end
      end
    end

    briefingRoom.mission.objectives[index].unitsCount = #briefingRoom.mission.objectives[index].unitsID
  end
end

-- ===================================================================================
-- 3.6 - MISSION EVENT HANDLER
-- Common event handler used by the mission
-- ===================================================================================

briefingRoom.mission.eventHandler = {}

function briefingRoom.mission.eventHandler:onEvent(event)

  if event.id == world.event.S_EVENT_TAKEOFF then -- unit took off
    if event.initiator:getPlayerName() ~= nil then
      briefingRoom.mission.functions.beginMission() -- first player to take off triggers the mission start
    end

  elseif event.id == world.event.S_EVENT_DEAD or event.id == world.event.S_EVENT_CRASH then -- unit destroyed
    if event.initiator == nil then return end -- no initiator
    --if Unit.getGroup(event.initiator) == nil then return end -- initiator was not an unit
    if event.initiator:getCategory() ~= Object.Category.UNIT then return end -- initiator was not an unit
    local unitID = event.initiator:getID()

    if event.initiator:getCoalition() == $ENEMYCOALITION$ then -- unit is an enemy, radio some variation of a "enemy destroyed" message
      local unitWasAMissionTarget = false

      -- was the destroyed unit a mission target?
      for _,objective in ipairs(briefingRoom.mission.objectives) do
        if table.contains(objective.unitsID, unitID) then
          unitWasAMissionTarget = true
        end
      end

      local soundName = "UnitDestroyed"
      local messages = { "Weapon was effective.", "Good hit! Good hit!", "They're going down.", "Splashed one!" }
      local messageIndex = math.random(1, 2)
      local messageIndexOffset = 0

      if unitWasAMissionTarget then
        soundName = "TargetDestroyed"
        messages = { "Target destroyed.", "Good hit on target.", "Target splashed.", "Target shot down!" }
      end

      local targetType = "Ground"
      if event.id == world.event.S_EVENT_CRASH then
        targetType = "Air"
        messageIndexOffset = 2
      end

      briefingRoom.radioManager.play(messages[messageIndex + messageIndexOffset], "RadioHQ"..soundName..targetType..tostring(messageIndex), math.random(1, 3))
    end

    if briefingRoom.mission.functions.onUnitDestroyed ~= nil then
      briefingRoom.mission.functions.onUnitDestroyed(unitID)
    end

  elseif event.id == world.event.S_EVENT_EJECTION then -- unit ejected
    if event.initiator:getPlayerName() ~= nil and briefingRoom.mission.missionType == brMissionType.SINGLE_PLAYER then
      briefingRoom.radioManager.play("We have confirmation. Our pilot has ejected. To all units, initiate immediate combat search and rescue operation.", "RadioHQPlayerEjected", math.random(5, 6))
    end
  end
end

world.addEventHandler(briefingRoom.mission.eventHandler)
