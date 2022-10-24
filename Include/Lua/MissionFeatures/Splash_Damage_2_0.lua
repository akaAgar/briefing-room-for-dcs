-- BR SINGLETON FLAG
--[[
2 October 2020
FrozenDroid:
- Added error handling to all event handler and scheduled functions. Lua script errors can no longer bring the server down.
- Added some extra checks to which weapons to handle, make sure they actually have a warhead (how come S-8KOM's don't have a warhead field...?)
28 October 2020
FrozenDroid: 
- Uncommented error logging, actually made it an error log which shows a message box on error.
- Fixed the too restrictive weapon filter (took out the HE warhead requirement)
21 December 2021
spencershepard (GRIMM):
 SPLASH DAMAGE 2.0:
 -Added blast wave effect to add timed and scaled secondary explosions on top of game objects
 -object geometry within blast wave changes damage intensity
 -damage boost for structures since they are hard to kill, even if very close to large explosions
 -increased some rocket values in explTable
 -missing weapons from explTable will display message to user and log to DCS.log so that we can add what's missing
 -damage model for ground units that will disable their weapons and ability to move with partial damage before they are killed
 -added options table to allow easy adjustments before release
 -general refactoring and restructure
--]]

----[[ ##### SCRIPT CONFIGURATION ##### ]]----

splash_damage_options = {
  ["static_damage_boost"] = 2000, --apply extra damage to Unit.Category.STRUCTUREs with wave explosions
  ["wave_explosions"] = true, --secondary explosions on top of game objects, radiating outward from the impact point and scaled based on size of object and distance from weapon impact point
  ["larger_explosions"] = true, --secondary explosions on top of weapon impact points, dictated by the values in the explTable
  ["damage_model"] = false, --allow blast wave to affect ground unit movement and weapons
  ["blast_search_radius"] = 100, --this is the max size of any blast wave radius, since we will only find objects within this zone
  ["cascade_damage_threshold"] = 0.1, --if the calculated blast damage doesn't exeed this value, there will be no secondary explosion damage on the unit.  If this value is too small, the appearance of explosions far outside of an expected radius looks incorrect.
  ["game_messages"] = true, --enable some messages on screen
  ["blast_stun"] = false, --not implemented
  ["unit_disabled_health"] = 30, --if health is below this value after our explosions, disable its movement
  ["unit_cant_fire_health"] = 50, --if health is below this value after our explosions, set ROE to HOLD to simulate damage weapon systems
  ["infantry_cant_fire_health"] = 90, --if health is below this value after our explosions, set ROE to HOLD to simulate severe injury
  ["debug"] = false, --enable debugging messages
  ["weapon_missing_message"] = false, --false disables messages alerting you to weapons missing from the explTable
}

local script_enable = 1
refreshRate = 0.1

----[[ ##### End of SCRIPT CONFIGURATION ##### ]]----

explTable = {
  ["FAB_100"]                         = 45,
  ["FAB_250"]                         = 100,
  ["FAB_250M54TU"]                    = 100,
  ["FAB_500"]                         = 213,
  ["FAB_1500"]                        = 675,
  ["BetAB_500"]                       = 98,
  ["BetAB_500ShP"]                    = 107,
  ["KH-66_Grom"]                      = 108,
  ["M_117"]                           = 201,
  ["Mk_81"]                           = 60,
  ["Mk_82"]                           = 118,
  ["AN_M64"]                          = 121,
  ["Mk_83"]                           = 274,
  ["Mk_84"]                           = 582,
  ["MK_82AIR"]                        = 118,
  ["MK_82SNAKEYE"]                    = 118,
  ["GBU_10"]                          = 582,
  ["GBU_12"]                          = 118,
  ["GBU_16"]                          = 274,
  ["KAB_1500Kr"]                      = 675,
  ["KAB_500Kr"]                       = 213,
  ["KAB_500"]                         = 213,
  ["GBU_31"]                          = 582,
  ["GBU_31_V_3B"]                     = 582,
  ["GBU_31_V_2B"]                     = 582,
  ["GBU_31_V_4B"]                     = 582,
  ["GBU_32_V_2B"]                     = 202,
  ["GBU_38"]                          = 118,
  ["AGM_62"]                          = 400,
  ["GBU_24"]                          = 582,
  ["X_23"]                            = 111,
  ["X_23L"]                           = 111,
  ["X_28"]                            = 160,
  ["X_25ML"]                          = 89,
  ["X_25MP"]                          = 89,
  ["X_25MR"]                          = 140,
  ["X_58"]                            = 140,
  ["X_29L"]                           = 320,
  ["X_29T"]                           = 320,
  ["X_29TE"]                          = 320,
  ["AGM_84E"]                         = 488,
  ["AGM_88C"]                         = 89,
  ["AGM_122"]                         = 15,
  ["AGM_123"]                         = 274,
  ["AGM_130"]                         = 582,
  ["AGM_119"]                         = 176,
  ["AGM_154C"]                        = 305,
  ["S-24A"]                           = 24,
  --["S-24B"] = 123,
  ["S-25OF"]                          = 194,
  ["S-25OFM"]                         = 150,
  ["S-25O"]                           = 150,
  ["S_25L"]                           = 190,
  ["S-5M"]                            = 1,
  ["C_8"]                             = 4,
  ["C_8OFP2"]                         = 3,
  ["C_13"]                            = 21,
  ["C_24"]                            = 123,
  ["C_25"]                            = 151,
  ["HYDRA_70M15"]                     = 3,
  ["Zuni_127"]                        = 5,
  ["ARAKM70BHE"]                      = 4,
  ["BR_500"]                          = 118,
  ["Rb 05A"]                          = 217,
  ["HEBOMB"]                          = 40,
  ["HEBOMBD"]                         = 40,
  ["MK-81SE"]                         = 60,
  ["AN-M57"]                          = 56,
  ["AN-M64"]                          = 180,
  ["AN-M65"]                          = 295,
  ["AN-M66A2"]                        = 536,
  ["HYDRA_70_M151"]                   = 4,
  ["HYDRA_70_MK5"]                    = 4,
  ["Vikhr_M"]                         = 11,
  ["British_GP_250LB_Bomb_Mk1"]       = 100, --("250 lb GP Mk.I")
  ["British_GP_250LB_Bomb_Mk4"]       = 100, --("250 lb GP Mk.IV")
  ["British_GP_250LB_Bomb_Mk5"]       = 100, --("250 lb GP Mk.V")
  ["British_GP_500LB_Bomb_Mk1"]       = 213, --("500 lb GP Mk.I")
  ["British_GP_500LB_Bomb_Mk4"]       = 213, --("500 lb GP Mk.IV")
  ["British_GP_500LB_Bomb_Mk4_Short"] = 213, --("500 lb GP Short tail")
  ["British_GP_500LB_Bomb_Mk5"]       = 213, --("500 lb GP Mk.V")
  ["British_MC_250LB_Bomb_Mk1"]       = 100, --("250 lb MC Mk.I")
  ["British_MC_250LB_Bomb_Mk2"]       = 100, --("250 lb MC Mk.II")
  ["British_MC_500LB_Bomb_Mk1_Short"] = 213, --("500 lb MC Short tail")
  ["British_MC_500LB_Bomb_Mk2"]       = 213, --("500 lb MC Mk.II")
  ["British_SAP_250LB_Bomb_Mk5"]      = 100, --("250 lb S.A.P.")
  ["British_SAP_500LB_Bomb_Mk5"]      = 213, --("500 lb S.A.P.")
  ["British_AP_25LBNo1_3INCHNo1"]     = 4, --("RP-3 25lb AP Mk.I")
  ["British_HE_60LBSAPNo2_3INCHNo1"]  = 4, --("RP-3 60lb SAP No2 Mk.I")
  ["British_HE_60LBFNo1_3INCHNo1"]    = 4, --("RP-3 60lb F No1 Mk.I")
  ["WGr21"]                           = 4, --("Werfer-Granate 21 - 21 cm UnGd air-to-air rocket")
  ["3xM8_ROCKETS_IN_TUBES"]           = 4, --("4.5 inch M8 UnGd Rocket")
  ["AN_M30A1"]                        = 45, --("AN-M30A1 - 100lb GP Bomb LD")
  ["AN_M57"]                          = 100, --("AN-M57 - 250lb GP Bomb LD")
  ["AN_M65"]                          = 400, --("AN-M65 - 1000lb GP Bomb LD")
  ["AN_M66"]                          = 800, --("AN-M66 - 2000lb GP Bomb LD")
  ["SC_50"]                           = 20, --("SC 50 - 50kg GP Bomb LD")
  ["ER_4_SC50"]                       = 20, --("4 x SC 50 - 50kg GP Bomb LD")
  ["SC_250_T1_L2"]                    = 100, --("SC 250 Type 1 L2 - 250kg GP Bomb LD")
  ["SC_501_SC250"]                    = 100, --("SC 250 Type 3 J - 250kg GP Bomb LD")
  ["Schloss500XIIC1_SC_250_T3_J"]     = 100, --("SC 250 Type 3 J - 250kg GP Bomb LD")
  ["SC_501_SC500"]                    = 213, --("SC 500 J - 500kg GP Bomb LD")
  ["SC_500_L2"]                       = 213, --("SC 500 L2 - 500kg GP Bomb LD")
  ["SD_250_Stg"]                      = 100, --("SD 250 Stg - 250kg GP Bomb LD")
  ["SD_500_A"]                        = 213, --("SD 500 A - 500kg GP Bomb LD")
  ["AB_250_2_SD_2"]                   = 100, --("AB 250-2 - 144 x SD-2, 250kg CBU with HE submunitions")
  ["AB_250_2_SD_10A"]                 = 100, --("AB 250-2 - 17 x SD-10A, 250kg CBU with 10kg Frag/HE submunitions")
  ["AB_500_1_SD_10A"]                 = 213, --("AB 500-1 - 34 x SD-10A, 500kg CBU with 10kg Frag/HE submunitions")
  --["LTF_5B"] = 100,                                   --("LTF 5b Aerial Torpedo")
  --agm-65??

}


----[[ ##### HELPER/UTILITY FUNCTIONS ##### ]]----

local function tableHasKey(table, key)
  return table[key] ~= nil
end

local function debugMsg(str)
  if splash_damage_options.debug == true then
    trigger.action.outText(str, 5)
  end
end

local function gameMsg(str)
  if splash_damage_options.game_messages == true then
    trigger.action.outText(str, 5)
  end
end

local function getDistance(point1, point2)
  local x1 = point1.x
  local y1 = point1.y
  local z1 = point1.z
  local x2 = point2.x
  local y2 = point2.y
  local z2 = point2.z
  local dX = math.abs(x1 - x2)
  local dZ = math.abs(z1 - z2)
  local distance = math.sqrt(dX * dX + dZ * dZ)
  return distance
end

local function getDistance3D(point1, point2)
  local x1 = point1.x
  local y1 = point1.y
  local z1 = point1.z
  local x2 = point2.x
  local y2 = point2.y
  local z2 = point2.z
  local dX = math.abs(x1 - x2)
  local dY = math.abs(y1 - y2)
  local dZ = math.abs(z1 - z2)
  local distance = math.sqrt(dX * dX + dZ * dZ + dY * dY)
  return distance
end

local function vec3Mag(speedVec)
  local mag = speedVec.x * speedVec.x + speedVec.y * speedVec.y + speedVec.z * speedVec.z
  mag = math.sqrt(mag)
  --trigger.action.outText("X = " .. speedVec.x ..", y = " .. speedVec.y .. ", z = "..speedVec.z, 10)
  --trigger.action.outText("Speed = " .. mag, 1)
  return mag
end

local function lookahead(speedVec)
  local speed = vec3Mag(speedVec)
  local dist = speed * refreshRate * 1.5
  return dist
end

----[[ ##### End of HELPER/UTILITY FUNCTIONS ##### ]]----


WpnHandler = {}
tracked_weapons = {}

function track_wpns()
  --  env.info("Weapon Track Start")
  for wpn_id_, wpnData in pairs(tracked_weapons) do
    if wpnData.wpn:isExist() then -- just update speed, position and direction.
      wpnData.pos = wpnData.wpn:getPosition().p
      wpnData.dir = wpnData.wpn:getPosition().x
      wpnData.speed = wpnData.wpn:getVelocity()
      --wpnData.lastIP = land.getIP(wpnData.pos, wpnData.dir, 50)
    else -- wpn no longer exists, must be dead.
      --      trigger.action.outText("Weapon impacted, mass of weapon warhead is " .. wpnData.exMass, 2)
      local ip = land.getIP(wpnData.pos, wpnData.dir, lookahead(wpnData.speed)) -- terrain intersection point with weapon's nose.  Only search out 20 meters though.
      local impactPoint
      if not ip then -- use last calculated IP
        impactPoint = wpnData.pos
        --        trigger.action.outText("Impact Point:\nPos X: " .. impactPoint.x .. "\nPos Z: " .. impactPoint.z, 2)
      else -- use intersection point
        impactPoint = ip
        --        trigger.action.outText("Impact Point:\nPos X: " .. impactPoint.x .. "\nPos Z: " .. impactPoint.z, 2)
      end
      --env.info("Weapon is gone") -- Got to here --
      --trigger.action.outText("Weapon Type was: ".. wpnData.name, 20)
      if splash_damage_options.larger_explosions == true then
        --env.info("triggered explosion size: "..getWeaponExplosive(wpnData.name))
        trigger.action.explosion(impactPoint, getWeaponExplosive(wpnData.name))
        --trigger.action.smoke(impactPoint, 0)
      end
      --if wpnData.cat == Weapon.Category.ROCKET then
      blastWave(impactPoint, splash_damage_options.blast_search_radius, wpnData.ordnance,
        getWeaponExplosive(wpnData.name))
      --end
      tracked_weapons[wpn_id_] = nil -- remove from tracked weapons first.
    end
  end
  --  env.info("Weapon Track End")
end

function onWpnEvent(event)
  if event.id == world.event.S_EVENT_SHOT then
    if event.weapon then
      local ordnance = event.weapon
      local weapon_desc = ordnance:getDesc()
      if string.find(ordnance:getTypeName(), "weapons.shells") then
        debugMsg("event shot, but not tracking: " .. ordnance:getTypeName())
        return --we wont track these types of weapons, so exit here
      end

      if explTable[ordnance:getTypeName()] then
        --trigger.action.outText(ordnance:getTypeName().." found.", 10)
      else
        env.info(ordnance:getTypeName() .. " missing from Splash Damage script")
        if splash_damage_options.weapon_missing_message == true then
          trigger.action.outText(ordnance:getTypeName() .. " missing from Splash Damage script", 10)
          debugMsg("desc: " .. mist.utils.tableShow(weapon_desc))
        end
      end
      if (weapon_desc.category ~= 0) and event.initiator then
        if (weapon_desc.category == 1) then
          if (weapon_desc.MissileCategory ~= 1 and weapon_desc.MissileCategory ~= 2) then
            tracked_weapons[event.weapon.id_] = { wpn = ordnance, init = event.initiator:getName(),
              pos = ordnance:getPoint(), dir = ordnance:getPosition().x, name = ordnance:getTypeName(),
              speed = ordnance:getVelocity(), cat = ordnance:getCategory() }
          end
        else
          tracked_weapons[event.weapon.id_] = { wpn = ordnance, init = event.initiator:getName(),
            pos = ordnance:getPoint(), dir = ordnance:getPosition().x, name = ordnance:getTypeName(),
            speed = ordnance:getVelocity(), cat = ordnance:getCategory() }
        end
      end
    end
  end

end

local function protectedCall(...)
  local status, retval = pcall(...)
  if not status then
    env.warning("Splash damage script error... gracefully caught! " .. retval, true)
  end
end

function WpnHandler:onEvent(event)
  protectedCall(onWpnEvent, event)
end

function explodeObject(table)
  local point = table[1]
  local distance = table[2]
  local power = table[3]
  trigger.action.explosion(point, power)
end

function getWeaponExplosive(name)
  if explTable[name] then
    return explTable[name]
  else
    return 0
  end
end

--controller is only at group level for ground units.  we should itterate over the group and only apply effects if health thresholds are met by all units in the group
function modelUnitDamage(units)
  --debugMsg("units table: "..mist.utils.tableShow(units))
  for i, unit in ipairs(units) do
    --debugMsg("unit table: "..mist.utils.tableShow(unit))
    if unit:isExist() then --if units are not already dead
      local health = (unit:getLife() / unit:getDesc().life) * 100
      --debugMsg(unit:getTypeName().." health %"..health)
      if unit:hasAttribute("Infantry") == true and health > 0 then --if infantry
        if health <= splash_damage_options.infantry_cant_fire_health then
          ---disable unit's ability to fire---
          unit:getController():setOption(AI.Option.Ground.id.ROE, AI.Option.Ground.val.ROE.WEAPON_HOLD)
        end
      end
      if unit:getDesc().category == Unit.Category.GROUND_UNIT == true and unit:hasAttribute("Infantry") == false and
          health > 0 then --if ground unit but not infantry
        if health <= splash_damage_options.unit_cant_fire_health then
          ---disable unit's ability to fire---
          unit:getController():setOption(AI.Option.Ground.id.ROE, AI.Option.Ground.val.ROE.WEAPON_HOLD)
          gameMsg(unit:getTypeName() .. " weapons disabled")
        end
        if health <= splash_damage_options.unit_disabled_health and health > 0 then
          ---disable unit's ability to move---
          unit:getController():setTask({ id = 'Hold', params = {} })
          unit:getController():setOnOff(false)
          gameMsg(unit:getTypeName() .. " disabled")
        end
      end

    else
      --debugMsg("unit no longer exists")
    end
  end
end

function blastWave(_point, _radius, weapon, power)
  local foundUnits = {}
  local volS = {
    id = world.VolumeType.SPHERE,
    params = {
      point = _point,
      radius = _radius
    }
  }

  local ifFound = function(foundObject, val)
    if foundObject:getDesc().category == Unit.Category.GROUND_UNIT and foundObject:getCategory() == Object.Category.UNIT then
      foundUnits[#foundUnits + 1] = foundObject
    end
    if foundObject:getDesc().category == Unit.Category.GROUND_UNIT then --if ground unit
      if splash_damage_options.blast_stun == true then
        --suppressUnit(foundObject, 2, weapon)
      end
    end
    if splash_damage_options.wave_explosions == true then
      local obj = foundObject
      local obj_location = obj:getPoint()
      local distance = getDistance(_point, obj_location)
      local timing = distance / 500
      if obj:isExist() then

        if tableHasKey(obj:getDesc(), "box") then
          local length = (obj:getDesc().box.max.x + math.abs(obj:getDesc().box.min.x))
          local height = (obj:getDesc().box.max.y + math.abs(obj:getDesc().box.min.y))
          local depth = (obj:getDesc().box.max.z + math.abs(obj:getDesc().box.min.z))
          local _length = length
          local _depth = depth
          if depth > length then
            _length = depth
            _depth = length
          end
          local surface_distance = distance - _depth / 2
          local scaled_power_factor = 0.006 * power + 1 --this could be reduced into the calc on the next line
          local intensity = (power * scaled_power_factor) / (4 * 3.14 * surface_distance * surface_distance)
          local surface_area = _length * height --Ideally we should roughly calculate the surface area facing the blast point, but we'll just find the largest side of the object for now
          local damage_for_surface = intensity * surface_area
          --debugMsg(obj:getTypeName().." sa:"..surface_area.." distance:"..surface_distance.." dfs:"..damage_for_surface)
          if damage_for_surface > splash_damage_options.cascade_damage_threshold then
            local explosion_size = damage_for_surface
            if obj:getDesc().category == Unit.Category.STRUCTURE then
              explosion_size = intensity * splash_damage_options.static_damage_boost --apply an extra damage boost for static objects. should we factor in surface_area?
              --debugMsg("static obj :"..obj:getTypeName())
            end
            if explosion_size > power then explosion_size = power end --secondary explosions should not be larger than the explosion that created it
            local id = timer.scheduleFunction(explodeObject, { obj_location, distance, explosion_size },
              timer.getTime() + timing) --create the explosion on the object location
          end


        else --debugMsg(obj:getTypeName().." object does not have box property")
        end

      end

    end

    return true
  end

  world.searchObjects(Object.Category.UNIT, volS, ifFound)
  world.searchObjects(Object.Category.STATIC, volS, ifFound)
  world.searchObjects(Object.Category.SCENERY, volS, ifFound)
  world.searchObjects(Object.Category.CARGO, volS, ifFound)
  --world.searchObjects(Object.Category.BASE, volS, ifFound)

  if splash_damage_options.damage_model == true then
    local id = timer.scheduleFunction(modelUnitDamage, foundUnits, timer.getTime() + 1.5) --allow some time for the game to adjust health levels before running our function
  end
end

if (script_enable == 1) then
  gameMsg("SPLASH DAMAGE 2 SCRIPT RUNNING")
  timer.scheduleFunction(function()
    protectedCall(track_wpns)
    return timer.getTime() + refreshRate
  end,
    {},
    timer.getTime() + refreshRate
  )
  world.addEventHandler(WpnHandler)
end
