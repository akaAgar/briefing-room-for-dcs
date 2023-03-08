
function dumpAirbaseDataType(o)
    if o == 16 then return "Runway" end
    if o == 40 then return "HelicopterOnly" end
    if o == 68 then return "HardenedAirShelter" end
    if o == 72 then return "AirplaneOnly" end
    if o == 104 then return "OpenAirSpawn" end
    return "UNKNOWN"
  end
  
  function dumpAirbaseParkingData(parkingObj)
      local output = {}
      for i = 1, #parkingObj do
          lat, lon, alt = coord.LOtoLL(parkingObj[i].vTerminalPos)
          output[parkingObj[i]["Term_Index"]] = {
              ["id"] = parkingObj[i]["Term_Index"],
              ["pos"] = {
                  ['DCS'] = parkingObj[i].vTerminalPos,
                  ['World'] = {
                      ['lat'] = lat,
                      ['lon'] = lon,
                      ['alt'] = alt
                  }
              },
              ["typeEnum"] = parkingObj[i].Term_Type,
              ["typeStr"] = dumpAirbaseDataType(parkingObj[i].Term_Type)
  
          }
      end
      return output
  end
  
  function dumpAirbaseData()
    local baseObj = {}
    local base = world.getAirbases()
    for i = 1, #base do
      local parkingData = base[i]:getParking()
      baseObj[base[i]:getID()] = {
         ["id"] = base[i]:getID(),
         ["name"] = base[i]:getDesc()['displayName'],
         ["desc"] = base[i]:getDesc(),
         ["parking"] = dumpAirbaseParkingData(base[i]:getParking()),
         ["runways"] = base[i]:getRunways()
      }
      end
    return baseObj
  end
  
  return dumpAirbaseData()