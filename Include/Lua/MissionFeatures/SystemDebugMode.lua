
briefingRoom.f10MenuCommands.debug = {} -- Create F10 debug sub-menu
briefingRoom.printDebugMessages = true -- Enable debug messages

function briefingRoom.f10MenuCommands.debug.activateAllAircraft()
  if #briefingRoom.aircraftActivator.reserveQueue == 0 then
    briefingRoom.debugPrint("No groups in the reserve queue")
    return
  end

  while #briefingRoom.aircraftActivator.reserveQueue > 0 do
    briefingRoom.aircraftActivator.pushFromReserveQueue()
  end
end

-- Destroys the next active target unit
function briefingRoom.f10MenuCommands.debug.destroyTargetUnit()
  for index, objective in ipairs(briefingRoom.mission.objectives) do
    if (#objective.unitNames > 0) then
      local u = Unit.getByName(objective.unitNames[1])
      if u == nil then
        u = StaticObject.getByName(objective.unitNames[1])
      end
      if u ~= nil then
        trigger.action.outText("Destroyed " .. u:getName(), 2)
        trigger.action.explosion(u:getPoint(), 100)
        return
      end
    end
  end

  trigger.action.outText("No target units found", 2)
end

function briefingRoom.f10MenuCommands.debug.destroySpecificTargetUnit(i)
  local objective = briefingRoom.mission.objectives[i]
  if (#objective.unitNames > 0) then
    local u = Unit.getByName(objective.unitNames[1])
    if u == nil then
      u = StaticObject.getByName(objective.unitNames[1])
    end
    if u ~= nil then
      trigger.action.outText("Destroyed " .. u:getName(), 2)
      trigger.action.explosion(u:getPoint(), 100)
      return
    end
  end

  trigger.action.outText("No target units found", 2)
end

function briefingRoom.f10MenuCommands.debug.destroyRandomEnemyUnit()
  local u = math.randomFromTable(
    table.merge(
      dcsExtensions.getCoalitionUnits(briefingRoom.enemyCoalition),
      dcsExtensions.getCoalitionStaticObjects(briefingRoom.enemyCoalition)
    )
  )
  if u ~= nil then
    trigger.action.outText("Destroyed " .. u:getName(), 2)
    trigger.action.explosion(u:getPoint(), 100)
  else
    trigger.action.outText("No target units found", 2)
  end

end

function briefingRoom.f10MenuCommands.debug.destroyRandomAllyUnit()
  local u = math.randomFromTable(
    table.merge(
      dcsExtensions.getCoalitionUnits(briefingRoom.playerCoalition),
      dcsExtensions.getCoalitionStaticObjects(briefingRoom.playerCoalition)
    )
  )
  if u ~= nil then
    trigger.action.outText("Destroyed " .. u:getName(), 2)
    trigger.action.explosion(u:getPoint(), 100)
  else
    trigger.action.outText("No target units found", 2)
  end

end




function briefingRoom.f10MenuCommands.debug.dumpAirbaseDataType(o)
  if o == 16 then return "Runway" end
  if o == 40 then return "HelicopterOnly" end
  if o == 68 then return "HardenedAirShelter" end
  if o == 72 then return "AirplaneOnly" end
  if o == 104 then return "OpenAirSpawn" end
  return "UNKNOWN"
end

function briefingRoom.f10MenuCommands.debug.dumpAirbaseParkingData()
  briefingRoom.debugPrint("STARTING AIRBASE PARKING DUMP");
  local base = world.getAirbases()
  for i = 1, #base do
    local parkingData = base[i]:getParking()
    local parkingString = [[
[Airbase]
ATC=
Coalition=
Coordinates=
DCSID=]]..base[i]:getID()..[[

Elevation=
Flags=
ILS=
Name=]]..base[i]:getDesc()['displayName']..[[

Runways=
RunwayLengthFt=
TACAN=
Theater=]]..env.mission.theatre..[[


[Parking]
]]
    for j = 1, #parkingData do
      if parkingData[j].Term_Type ~= 16 then
        parkingString = parkingString ..
            "\nSpot" .. j .. ".Coordinates=" .. parkingData[j].vTerminalPos.x .. "," .. parkingData[j].vTerminalPos.z
        parkingString = parkingString .. "\nSpot" .. j .. ".DCSID=" .. parkingData[j].Term_Index
        parkingString = parkingString ..
            "\nSpot" .. j .. ".Type=" .. briefingRoom.f10MenuCommands.debug.dumpAirbaseDataType(parkingData[j].Term_Type)
      end
    end
    local fp = io.open(lfs.writedir() .. "\\"..env.mission.theatre..base[i]:getDesc()['displayName']..".ini", 'w')
    fp:write(parkingString)
    fp:close()
    briefingRoom.debugPrint("Done"..lfs.writedir() .. "\\"..env.mission.theatre..base[i]:getDesc()['displayName']..".ini");
  end
  briefingRoom.debugPrint("DONE AIRBASE PARKING DUMP");
end


function briefingRoom.f10MenuCommands.debug.map_zero_coords()
  local zero = {
    ["x"] = 0,
    ["y"] = 0,
    ["z"] = 0,
  }
  lat, lon, alt = coord.LOtoLL(zero)
  briefingRoom.debugPrint("lat:" .. tostring(lat) .. " lon:" .. tostring(lon) .. " alt:" .. alt);

end

function briefingRoom.f10MenuCommands.debug.map_coords()
  local minMax ={
    ["minX"] = $THEATERMINX$,
    ["minY"] = $THEATERMINY$,
    ["maxX"] = $THEATERMAXX$,
    ["maxY"] = $THEATERMAXY$,
  }
  local spot = {
    ["x"] = minMax["minX"],
    ["y"] = 0,
    ["z"] = minMax["minY"],
  }
  local coordinates = {}
  briefingRoom.debugPrint("Getting Data");
  while spot["x"] <= minMax["maxX"] do
    while spot["z"] <= minMax["maxY"] do
      lat, lon, alt = coord.LOtoLL(spot)
      coordinates["\"x:" .. spot["x"] .. ",z:" .. spot["z"]] = {
        ["x"] = lat,
        ["y"] = lon
      }
      spot["z"] = spot["z"] + 1000
    end
    spot["z"] = minMax["minY"]
    spot["x"] = spot["x"] + 1000
  end
  local fp = io.open(lfs.writedir() .. "\\coords.json", 'w')
  fp:write(json:encode(coordinates))
  fp:close()
  briefingRoom.debugPrint("Done"..lfs.writedir() .. "\\coords.json");

end

-- Create the debug menu
do
  briefingRoom.f10Menu.debug = missionCommands.addSubMenu("(DEBUG MENU)", nil)
  missionCommands.addCommand("Simulate player takeoff (start mission)", briefingRoom.f10Menu.debug,
    briefingRoom.mission.coreFunctions.beginMission)
  missionCommands.addCommand("Destroy target unit", briefingRoom.f10Menu.debug,
    briefingRoom.f10MenuCommands.debug.destroyTargetUnit)
  missionCommands.addCommand("Destroy random enemy unit", briefingRoom.f10Menu.debug,
    briefingRoom.f10MenuCommands.debug.destroyRandomEnemyUnit)
    missionCommands.addCommand("Destroy random ally unit", briefingRoom.f10Menu.debug,
    briefingRoom.f10MenuCommands.debug.destroyRandomAllyUnit)
  missionCommands.addCommand("Activate all aircraft groups", briefingRoom.f10Menu.debug,
    briefingRoom.f10MenuCommands.debug.activateAllAircraft)
  missionCommands.addCommand("Dump All Airbase parking Data", briefingRoom.f10Menu.debug,
    briefingRoom.f10MenuCommands.debug.dumpAirbaseParkingData)
  missionCommands.addCommand("Dump Map coords", briefingRoom.f10Menu.debug,
    briefingRoom.f10MenuCommands.debug.map_coords)
end
