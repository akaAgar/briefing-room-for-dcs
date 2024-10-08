-- Lase anything script - group edition
-- by Carsten Gurk aka Don Rudi

local version = "1.3"

-- user variables

local targetGroupName = 'TARGET'
local myGroup = Group.getByName(targetGroupName)
local myGroupType = "group"
local validGroupName = true

if not myGroup or not myGroup:isExist() then
    validGroupName = false
end

local myLaserCode = 1516                        -- change to any lasercode between 1111 and 1788, can be changed in game

-- default beam is laser, IR can set via F10 option or map marker text field

local BEAM = "laser"

-- variables

local JTAC_GO = false
local JTAC_Menu_Root = nil
local JTAC_Menu_ID = nil
local JTACoptions = false
local JTACmenu = false
local currentTargetIndex = 0
local skippedUnits = {}
local targetPoint
local myStaticGroup
local staticUnit

-- Function to determine which unit is controlled by the player
local function getPlayerControlledUnit()
	
  local playerUnit = nil
  
  -- Iterate through all coalitions and their respective player units
  
  for coalitionID = 1, 2 do  -- 1 = Red, 2 = Blue
    local playerUnits = coalition.getPlayers(coalitionID)
    for _, unit in ipairs(playerUnits) do
      if unit and unit:getPlayerName() then
        playerUnit = unit
        break
      end
    end
    if playerUnit then
      break
    end
  end
  
  return playerUnit
end

-- create invisible unit to circumvent static objects

local function createInvisibleReflectiveUnit(staticObject)
    if staticObject and staticObject:isExist() then
        local position = staticObject:getPoint()
        local unitData = {
            ["type"] = "TACAN_beacon", 
            ["unitId"] = 1001, 
            ["name"] = staticObject:getName() .. "_reflector",
            ["heading"] = 0,
            ["x"] = position.x,
            ["y"] = position.z,
            ["alt"] = position.y,
            ["category"] = "Fortification", 
            ["country"] = staticObject:getCountry(),
            ["coalition"] = staticObject:getCoalition(),
            ["groupId"] = 2001, 
            ["groupName"] = staticObject:getName() .. "_reflector_group",
            ["units"] = {
                [1] = {
                    ["type"] = "TACAN_beacon", 
                    ["unitId"] = 1001,
                    ["name"] = staticObject:getName() .. "_reflector",
                    ["heading"] = 0,
                    ["x"] = position.x,
                    ["y"] = position.z,
                    ["alt"] = position.y,
                },
            },
        }
        coalition.addGroup(staticObject:getCountry(), Group.Category.GROUND, unitData)
		
		return Group.getByName(staticObject:getName() .. "_reflector")
    else
        return nil
    end
end

-- select next available unit

local function getNextTarget(group, currentIndex, skipped, myGroupType)
    
	if not group or not group:isExist() then
        return nil, nil
    end
		
	local units = group:getUnits()
	-- First, check for the next available unit after the current index
	
	for i = currentIndex + 1, #units do
		local unit = units[i]
		if unit and unit:isExist() then
			return unit, i
		end
	end
	-- If no units are found after the current index, check from the beginning
	for i = 1, currentIndex do
		local unit = units[i]
		if unit and unit:isExist() then
			return unit, i
		end
	end
	-- If no units are found in the normal order, check the skipped units
	for i, unitIndex in ipairs(skipped) do
		local unit = units[unitIndex]
		if unit and unit:isExist() then
			table.remove(skipped, i)
			return unit, unitIndex
		end
	end
	return nil, nil
		
end

-- get unit type name and number of units

local function getUnitType(unit)
    return unit:getTypeName()
end

local function countUnits(group)
    local units = group:getUnits()
    local count = 0
    for _, unit in ipairs(units) do
        if unit and unit:isExist() then
            count = count + 1
        end
    end
    return count
end

-- construct coordinates as MGRS, LAT/LONG standard and LAT/LONG decimal

local function correctMGRS (tmpMGRS)

	if tmpMGRS < 10 then
		tmpMGRS = "0000" .. tostring(tmpMGRS)
	elseif tmpMGRS < 100 then
		tmpMGRS = "000" .. tostring(tmpMGRS)
	elseif tmpMGRS < 1000 then
		tmpMGRS = "00" .. tostring(tmpMGRS)
	elseif tmpMGRS < 10000 then
		tmpMGRS = "0" .. tostring(tmpMGRS)
	end
	
	return string.sub(tmpMGRS, 1, 4)
end

local function getMGRS(unit)
    local lat, lon, alt = coord.LOtoLL(unit:getPoint())
    local mgrs = coord.LLtoMGRS(lat, lon)
	local altFT = math.floor(alt * 3,28084)
	mgrs.Easting = correctMGRS(mgrs.Easting)
	mgrs.Northing = correctMGRS(mgrs.Northing)
	return mgrs.UTMZone .. " " .. mgrs.MGRSDigraph .. " " .. mgrs.Easting .. " " .. mgrs.Northing, altFT
end

local function getLatLon(unit)
    local lat, lon, alt = coord.LOtoLL(unit:getPoint())
    local lat_deg = math.floor(lat)
	local altFT = math.floor(alt * 3,28084)
    local lat_min = (lat - lat_deg) * 60
    local lat_sec = (lat_min - math.floor(lat_min)) * 60
    local lon_deg = math.floor(lon)
    local lon_min = (lon - lon_deg) * 60
    local lon_sec = (lon_min - math.floor(lon_min)) * 60
    return string.format("N %d°%d'%.2f\" E %d°%d'%.2f\"", lat_deg, math.floor(lat_min), lat_sec, lon_deg, math.floor(lon_min), lon_sec), altFT
end

local function getLatLonDecimal(unit)
    local lat, lon, alt = coord.LOtoLL(unit:getPoint())
    local lat_deg = math.floor(lat)
	local altFT = math.floor(alt * 3,28084)
    local lat_min = (lat - lat_deg) * 60
    local lon_deg = math.floor(lon)
    local lon_min = (lon - lon_deg) * 60
    return string.format("N %d°%.3f' E %d°%.3f'", lat_deg, lat_min, lon_deg, lon_min), altFT
end

-- initial message to player

local function displayMessage(coord_format)

	myTarget, currentTargetIndex = getNextTarget(myGroup, 0, skippedUnits, myGroupType)
    local unitType = getUnitType(myTarget)
	
	local Coords
	local elevation
		
	if coord_format == "MGRS" then
	
		Coords, elevation = getMGRS(myTarget)
				
	elseif coord_format == "LatLon" then
	
		Coords, elevation = getLatLon(myTarget)
		
	elseif coord_format == "LatLonDecimal" then
	
		Coords, elevation = getLatLonDecimal(myTarget)
		
	end
	
	if myGroupType == "group" then
	
		local unitCount = countUnits(myGroup)
		--local unitType = getUnitType(myTarget)
		
		if BEAM == "laser" then
			trigger.action.outText("Current target: " .. Coords .. "\nElevation: " .. elevation .. " ft\nTarget: group of " .. unitCount .. " units (" .. unitType .. ")\nMarked by laser " .. myLaserCode, 30)
		elseif BEAM == "IR" then
			trigger.action.outText("Current target: " .. Coords .. "\nElevation: " .. elevation .. " ft\nTarget: group of " .. unitCount .. " units (" .. unitType .. ")\nMarked by IR", 30)
		end
	elseif myGroupType == "static" then

		if BEAM == "laser" then
			trigger.action.outText("Current target: " .. Coords .. "\nElevation: " .. elevation .. " ft\nTarget: " .. staticUnit .. "\nMarked by laser " .. myLaserCode, 30)
		elseif BEAM == "IR" then
			trigger.action.outText("Current target: " .. Coords .. "\nElevation: " .. elevation .. " ft\nTarget: " .. staticUnit .. "\nMarked by IR", 30)
		end
		
	end
end

-- remove F10 JTAC Options

local function removeJTACOptions()
    if JTACoptions then
        missionCommands.removeItem(JTAC_Menu_ID)
    end
    JTACoptions = false
	JTACmenu = false
end

-- preparing the laser...

local myTarget, currentTargetIndex = getNextTarget(myGroup, 0, skippedUnits, myGroupType)
local myRay

local function createSpot()

	if JTAC_GO then
	
		local spotPosition = myTarget
		
		if BEAM == "laser" then
			myRay = Spot.createLaser(spotPosition, { x = math.random(-1000,1000), y = 2000, z = math.random(-1000,1000) }, myTarget:getPoint(), myLaserCode)
		elseif BEAM == "IR" then
			myRay = Spot.createInfraRed(spotPosition, { x = math.random(-1000,1000), y = 2000, z = math.random(-1000,1000) }, myTarget:getPoint() )
		end
		
	end
end

-- Laser

local function updateMyRay()

	if myTarget and myTarget:isExist() and JTAC_GO then
		
		myRay:setPoint( myTarget:getPoint() )
			
        timer.scheduleFunction(updateMyRay, {}, timer.getTime() + 0.5)
    else
        if myRay then
            myRay:destroy()
			myRay = nil
        end
		
        myTarget, currentTargetIndex = getNextTarget(myGroup, currentTargetIndex, skippedUnits, myGroupType)
		
        if myTarget and JTAC_GO then
            local unitType = getUnitType(myTarget)
            trigger.action.outText("Target destroyed! \nSHIFT - new target: " .. unitType, 10)  
            createSpot()
            updateMyRay()
        elseif not myTarget and JTAC_GO then
            trigger.action.outText("All targets destroyed, you are cleared to leave.", 10)
            removeJTACOptions()
			JTAC_GO = false
        end
    end
end

-- Smoke marker

local function markWithSmoke()

    if myGroup and myGroup:isExist() then
	
        if myGroupType == "static" then
			
			local groupPosition = myTarget:getPoint()
			local smokePosition = {
            x = groupPosition.x + math.random(-50, 50),
            y = groupPosition.y,
            z = groupPosition.z + math.random(-50, 50)
			}
			trigger.action.smoke(smokePosition, trigger.smokeColor.White)
			
		else
		
			local groupPosition = myGroup:getUnit(1):getPoint()
			
			local smokePosition = {
            x = groupPosition.x + math.random(-50, 50),
            y = groupPosition.y,
            z = groupPosition.z + math.random(-50, 50)
			}
			trigger.action.smoke(smokePosition, trigger.smokeColor.White)
			
		end
		
        
        trigger.action.outText("Target marked with smoke.", 10)
    else
        trigger.action.outText("Error: Target group does not exist.", 10)
    end
end

-- IR marker

local function markWithIR()

	if myGroup and myGroup:isExist() then
		
		BEAM = "IR"
		if myRay then
			myRay:destroy()
			myRay = nil
		end
		createSpot()
		updateMyRay()
		
		displayMessage("MGRS")

	else
        trigger.action.outText("Error: Target group does not exist.", 10)
    end
	
end


-- skip target

local function skipTarget()
   
	if myGroupType == "static" then
	
		trigger.action.outText("No targets available to skip to.", 10)
		
	else	
	
   		if myTarget then
			table.insert(skippedUnits, currentTargetIndex)
			if myRay then
				myRay:destroy()
				myRay = nil
			end
			myTarget, currentTargetIndex = getNextTarget(myGroup, currentTargetIndex, skippedUnits, myGroupType)
			if myTarget then
				local unitType = getUnitType(myTarget)
				trigger.action.outText("SHIFT - new target: " .. unitType, 10) 
				createSpot()
				updateMyRay()
			else
				trigger.action.outText("No more targets available to skip to.", 10)
			end
		end
	
	end
end

-- F10 menu handling


-- end JTAC service before all units are destroyed

local function terminateService()

    if myRay then
        myRay:destroy()
		myRay = nil
    end
	
    JTAC_GO = false
    trigger.action.outText("JTAC service terminated. Goodbye!", 10)
    removeJTACOptions()
end

-- add Options

local function addJTACOptions()

    JTAC_Menu_ID = missionCommands.addSubMenu("JTAC Options", JTAC_Menu_Root)
    missionCommands.addCommand("Repeat message (MGRS)", JTAC_Menu_ID, function () displayMessage("MGRS") end)
    missionCommands.addCommand("Repeat message (LAT/LONG)", JTAC_Menu_ID, function () displayMessage("LatLon") end)
    missionCommands.addCommand("Repeat message (LAT/LONG decimal)", JTAC_Menu_ID, function () displayMessage("LatLonDecimal") end)
	missionCommands.addCommand("Mark with smoke", JTAC_Menu_ID, markWithSmoke)
	missionCommands.addCommand("Mark with IR", JTAC_Menu_ID, markWithIR)
	missionCommands.addCommand("Skip target", JTAC_Menu_ID, skipTarget)
	missionCommands.addCommand("Terminate service", JTAC_Menu_ID, terminateService)
	
	missionCommands.removeItem(JTAC_initial)
	JTACmenu = false
	JTACoptions = true
end

-- Initial call from F10 menu

local function callJTAC()
	
	if myRay then
        myRay:destroy()
		myRay = nil
    end
	
    if not JTAC_GO then
	
		if validGroupName then
		
			JTAC_GO = true
						
			if myGroup and myGroup:isExist() and myGroupType == "group" then
			
				displayMessage("MGRS")
				addJTACOptions()
				
				myTarget, currentTargetIndex = getNextTarget(myGroup, currentTargetIndex, skippedUnits, myGroupType)
				createSpot()
				updateMyRay()
				
			elseif myGroup and myGroupType == "static" then
			
				displayMessage("MGRS")
				addJTACOptions()
				
				myTarget, currentTargetIndex = getNextTarget(myGroup, currentTargetIndex, skippedUnits, myGroupType)
				createSpot()
				updateMyRay()
			
			end
		else
            trigger.action.outText("Error: Initial target group does not exist.", 10)
        end
    end
end

-- Set the traget group

local function setTargetGroup(groupName)

    local newGroup = Group.getByName(groupName)
    local newStatic = StaticObject.getByName(groupName)
	
    if newGroup and newGroup:isExist() then
	
		validGroupName = true
        targetGroupName = groupName
        myGroup = newGroup
		myGroupType = "group"
        --myStatic = nil
        skippedUnits = {} -- reset skipped units
        currentTargetIndex = 0 -- reset index
		
		myTarget, currentTargetIndex = getNextTarget(myGroup, currentTargetIndex, skippedUnits, myGroupType)
        trigger.action.outText("Target group set to: " .. groupName, 10)
		
	end
	
    if newStatic and newStatic:isExist() then
	
		validGroupName = true
        targetGroupName = groupName
		myStaticGroup = newStatic
		myGroup = createInvisibleReflectiveUnit(newStatic)
		myGroupType = "static"
        --myGroup = nil
        --myStatic = newStatic
        skippedUnits = {} -- reset skipped units
        currentTargetIndex = 0 -- reset index
		
		myTarget, currentTargetIndex = getNextTarget(myGroup, currentTargetIndex, skippedUnits, myGroupType)
		staticUnit = getUnitType(myStaticGroup)
        trigger.action.outText("Target group set to: " .. groupName .. " (Static Object)", 10)

    end
	
	if validGroupName then
	
		if JTAC_GO then
			JTAC_GO = false
			removeJTACOptions()
			callJTAC()
		else
			if not JTACmenu then
				JTAC_initial = missionCommands.addCommand("Call JTAC", nil, callJTAC)
				JTACmenu = true
			end
		end
		
	else
	
        trigger.action.outText("Error: Group " .. groupName .. " does not exist.", 10)
    end
end

-- player communicates with the script via map markers. Input of "codexxxx", xxxx will be checked for plausibility and intrepreted as new laser code

local function onPlayerChangeMarker(event)

    if event.id == world.event.S_EVENT_MARK_CHANGE then
	
        local markText = string.upper(event.text)
		
        if markText:find("CODE:%d%d%d%d") then
			
            local code = tonumber(markText:match("CODE:(%d%d%d%d)"))
			
            if code and code >= 1111 and code <= 1788 and not tostring(code):find("[09]") then
                myLaserCode = code
                trigger.action.outText("Laser code updated to " .. myLaserCode, 10)
				
                -- Recreate the spot with the new laser code if JTAC_GO is true
				
                if JTAC_GO then
                    createSpot()
                end
            else
                trigger.action.outText("Invalid laser code. Code must be between 1111 and 1788, and cannot contain 0 or 9.", 10)
            end
			
		elseif markText:find("TARGET:") then
            
			local newGroupName = event.text:sub(8)
            setTargetGroup(newGroupName)
		
		elseif markText:find("SPOT:LASER") then
            
			BEAM = "laser"
			if JTAC_GO then
				createSpot()
				displayMessage("MGRS")
			else
				trigger.action.outText("Roger, marking by laser selected.", 10)
			end
		
		elseif markText:find("SPOT:IR") then
            
			BEAM = "IR"
			if JTAC_GO then
				createSpot()
				displayMessage("MGRS")
			else
				trigger.action.outText("Roger, marking by IR pointer selected.", 10)
			end
			
        end
    end
end



-- Add the F10 radio item

JTAC_initial = missionCommands.addCommand("Call JTAC", nil, callJTAC)
JTACmenu = true


-- Register the event handler for map marker interaction

local eventHandler = { f = onPlayerChangeMarker }
function eventHandler:onEvent(e)
  self.f(e)
end
world.addEventHandler(eventHandler)

trigger.action.outText("Lase anything script " .. version .. " loaded", 10)

