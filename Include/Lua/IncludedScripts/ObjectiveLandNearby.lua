briefingRoom.mission.extensions.objectiveLandNearby = {}
briefingRoom.mission.extensions.objectiveLandNearby.MAX_DISTANCE = 500 -- max distance from the target the helo must land, in meters

function briefingRoom.mission.extensions.objectiveLandNearby:onEvent(event)
  if briefingRoom.mission.status ~= brMissionStatus.IN_PROGRESS then return end -- mission already complete/failed, nothing to do
  if event.id ~= world.event.S_EVENT_LAND then return end -- only "land" event interest us
  if event.initiator == nil then return end -- no unit (should never happen)
  if event.initiator:getCoalition() == $ENEMYCOALITION$ then return end -- unit is an enemy

  local landPos = dcsExtensions.toVec2(event.initiator:getPoint())

  for index,objective in ipairs(briefingRoom.mission.objectives) do
    if objective.status == brMissionStatus.IN_PROGRESS then
      for _,unitID in ipairs(briefingRoom.mission.objectives.unitsID) do
        local unit = dcsExtensions.getUnitByID(unitID)

        if unit ~= nil then
          local unitPos = dcsExtensions.toVec2(unit:getPoint())
          local distance = dcsExtensions.getDistance(landPos, unitPos)
          if distance <= briefingRoom.mission.extensions.objectiveLand.MAX_DISTANCE then
            briefingRoom.mission.functions.completeObjective(index)

            if event.initiator:getPlayerName() ~= nil then
              if briefingRoom.mission.parameters.objectiveLandNearbyPickupUnit == true then
                briefingRoom.radioManager.play("Command, packaged was picked up.", "RadioPilotPackageUp", math.random(1, 2))
              else
                briefingRoom.radioManager.play("Command, packaged was delivered.", "RadioPilotPackageDown", math.random(1, 2))
              end
            end

            if briefingRoom.mission.parameters.objectiveLandNearbyPickupUnit == true then
              unit:getGroup():destroy()
            end
          end
        end
      end
    end
  end
end

world.addEventHandler(briefingRoom.mission.extensions.objectiveLandNearby)
