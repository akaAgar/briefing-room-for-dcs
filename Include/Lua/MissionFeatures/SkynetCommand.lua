local skynetCommandCenter = StaticObject.getByName(briefingRoom.mission.missionFeatures.unitNames.skynetIADSCommand[1])
redIADS:addCommandCenter(skynetCommandCenter)
redIADS:activate()

local skynetCommandCenterPoint = skynetCommandCenter:getPoint()
local size = 3000
local circleCenterPoint = {x = skynetCommandCenterPoint.x + math.random(-size, size), y = 0, z = skynetCommandCenterPoint.z + math.random(-size, size)}
trigger.action.circleToAll(-1 , 1,  circleCenterPoint,  size, {.8, .8, .8, 1}, {0, 0, 0, 0}, 1, true)
local msg = "IADS Command Center"
local offset = {x = circleCenterPoint.x + 50, y = 0, z = circleCenterPoint.z - (string.len(msg)/2)}
trigger.action.textToAll(-1 , 2 , circleCenterPoint  , {1, 1, 1, 1} , {0, 0, 0, 0} , 10, true , msg )
