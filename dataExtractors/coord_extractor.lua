local function dump_map_coords()
    local spot = {
        ["x"] = -525000,
        ["y"] = 0,
        ["z"] = -142000,
    }
    local coordinates = {}
    while spot["x"] < 28500 do
      while spot["z"] < 1072000 do
        lat, lon, alt = coord.LOtoLL(spot)
        coordinates["\"x:"..spot["x"]..",z:"..spot["z"]] ={
          ["x"] = lat,
          ["y"] = lon
        }
        spot["z"] = spot["z"] + 1000
      end
      spot["z"] = -142000
      spot["x"] = spot["x"] + 1000
    end
  
    local fp = io.open(lfs.writedir() .. "\\coords.json", 'w')
    fp:write(json:encode(coordinates))
    fp:close()
  
  end
  dump_map_coords()