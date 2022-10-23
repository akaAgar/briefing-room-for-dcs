const waypointColors = ["Cyan", "orange", "Chartreuse", "Magenta", "DeepPink", "Gold"]
let mapGroups = {}
let leafMap
async function BlazorDownloadFile(filename, contentType, data) {
  // Create the URL
  const fileType = filename.split(".").at(-1)
  const file = new File([data], filename, { type: contentType });
  if (self.showSaveFilePicker) {
    const fileHandle = await self.showSaveFilePicker({
      suggestedName: filename,
      types: [{
        description: 'Text documents',
        accept: {
          'text/plain': [`.${fileType}`],
        },
      }],
    });

    const writable = await fileHandle.createWritable();
    // Write the contents of the file to the stream.
    await writable.write(file);
    // Close the file and write the contents to disk.
    await writable.close();
  } else {

    // // Create the <a> element and click on it
    const exportUrl = URL.createObjectURL(file);
    const a = document.createElement("a");
    document.body.appendChild(a);
    a.href = exportUrl;
    a.download = filename;
    a.target = "_self";
    a.click();
    // We don't need to keep the url, let's release the memory
    // On Safari it seems you need to comment this line... (please let me know if you know why)
    URL.revokeObjectURL(exportUrl);
  }

}

const Memoize = (fn) => {
  let cache = {};
  return async (...args) => {
    let strX = JSON.stringify(args);
    return strX in cache
      ? cache[strX]
      : (cache[strX] = await fn(...args));
  };
};

function ToggleLayer(id) {
  group = mapGroups[id]
  if (group._map) {
    group.remove()
    return
  }
  group.addTo(leafMap)
}

function GetFromMapCoordData(pos, mapCoordData) {
  x = Math.round(pos[0] / 1000) * 1000
  z = Math.round(pos[1] / 1000) * 1000
  key = `x:${x},z:${z}`
  pos2 = mapCoordData[key]
  if (pos2 == undefined) {
    throw `Key ${key} not found in positional data.`
  }
  return [pos2["x"], pos2["y"]]
}

const GetMapData = Memoize(async (map) => {
  try {
    const response = await fetch(`_content/BriefingRoomCommonGUI/js/${map}.json.gz`)
    const fileReader = await response.arrayBuffer();
    const binData = new Uint8Array(fileReader);
    return JSON.parse(pako.ungzip(binData, { 'to': 'string' }));
  } catch (error) {
    throw `Either can't find ${leafMap} data file or failed to parse it. raw error: ${error} ${error.stack}`
  }
})

async function RenderMap(mapData, map) {
  var MapCoordMap = await GetMapData(map)
  if (leafMap) {
    leafMap.off();
    leafMap.remove();
  }

  try {
    mapGroups = {
      "SAMs": new L.layerGroup(),
      "GroundForces": new L.layerGroup(),
    }
    leafMap = L.map('map')
    L.esri.basemapLayer("Imagery").addTo(leafMap);
    L.esri.basemapLayer("ImageryLabels").addTo(leafMap);
    addButtons()
  } catch (error) {
    console.warn(error)
  }

  Object.keys(mapData).forEach(key => {
    if (key.includes('ISLAND')) {
      return
    }
    data = mapData[key]
    if (data.length == 1) {
      AddIcon(key, data, leafMap, MapCoordMap)
    } else if (key.includes("ROUTE_")) {
      AddWaypoints(data, leafMap, MapCoordMap)
    } else {
      AddZone(key, data, leafMap, MapCoordMap)
    }
  })
  leafMap.setView(GetFromMapCoordData(mapData["AIRBASE_HOME"][0], MapCoordMap), 6.5);
  new ResizeObserver(() => leafMap.invalidateSize()).observe(document.querySelector(".generator-preview"))
}

function addButtons() {
  L.easyButton('oi oi-audio', function (btn, map) {
    ToggleLayer("SAMs")
  }).addTo(leafMap);
  L.easyButton('oi oi-dial', function (btn, map) {
    ToggleLayer("GroundForces")
  }).addTo(leafMap);
}

function AddIcon(key, data, map, MapCoordMap) {
  if (key.startsWith("UNIT")) {
    AddUnit(key, data, map, MapCoordMap)
  } else if (key.includes("WAYPOINT_")) {
    AddWaypoint(key, data, map, MapCoordMap)
  } else {
    new L.Marker(GetFromMapCoordData(data[0], MapCoordMap), {
      title: GetTitle(key),
      icon: new L.DivIcon({
        html: `<img class="map_point_icon" src="_content/BriefingRoomCommonGUI/img/nato-icons/${GetNatoIcon(key)}.svg" alt="${key}"/>`
      }),
      zIndexOffset: key == "AIRBASE_HOME" || key.includes("OBJECTIVE_AREA") ? 200 : 100
    }).addTo(map)
  }
}

function AddWaypoint(key, data, map, MapCoordMap) {
  const title =  key.replace("WAYPOINT_", "")
  const coords = GetFromMapCoordData(data[0], MapCoordMap)
  new L.circleMarker(coords, {
    title: title,
    radius: 10,
    weight: 5,
    color: `Gold`,
    fillOpacity: 0.0,
    zIndexOffset: 100
  }).addTo(map)
  new L.Marker(coords, {
    title: title,
    icon: new L.DivIcon({
      html: `<p class="map-label">${title}</p>`
    })
  }).addTo(map)
}

function AddUnit(key, data, map, MapCoordMap) {
  const coords = GetFromMapCoordData(data[0], MapCoordMap)
  group = mapGroups[GetGroup(key)]
  group.addLayer(new L.Marker(coords, {
    title: GetTitle(key),
    icon: new L.DivIcon({
      html: `<img class="map_point_icon map_unit"src="_content/BriefingRoomCommonGUI/img/nato-icons/${GetNatoIcon(key)}.svg" alt="${key}"/>`
    })
  }))
  const range = GetRange(key);
  if (range > 0) {
    group.addLayer(new L.Circle(coords, {
      radius: range,
      color: GetColour(key),
      fillColor: GetColour(key),
      fillOpacity: 0.25,
      zIndexOffset: 99999999999
    }))
  }
}

function AddWaypoints(data, map, MapCoordMap) {
  let color = waypointColors[Math.floor(Math.random() * waypointColors.length)];
  let coords = data.map(x => GetFromMapCoordData(x, MapCoordMap))
  new L.polyline(coords, {
    color: color,
    weight: 2,
    opacity: 1,
    smoothFactor: 2
  }).addTo(map);
  L.featureGroup(GetArrows(coords, color, 1, map)).addTo(map);
}

function AddZone(key, data, map, MapCoordMap) {
  let coords = data.map(x => GetFromMapCoordData(x, MapCoordMap))
  L.polygon(coords, {
    color: GetColour(key),
    fillColor: GetColour(key),
    fillOpacity: 0.2,
  }).addTo(map);
}

function GetGroup(id) {
  switch (true) {
    case id.includes("SAM"):
      return "SAMs";
    default:
      return "GroundForces";
  }
}

function GetRange(id) {
  switch (true) {
    case id.includes("Long"):
      return 40 * 1852;
    case id.includes("Medium"):
      return 20 * 1852;
    default:
      return 0
  }
}

function GetTitle(id) {
  switch (true) {
    case id.includes("AIRBASE"):
      return "Airbase"
    case id.includes("OBJECTIVE"):
      return "Objective"
    case id.includes("FOB"):
      return 'FOB'
    case id.includes("CARRIER"):
      return 'Carrier'
    case id.includes("SAM"):
      switch (true) {
        case id.includes("Long"):
          return "SAM Site Long Range"
        case id.includes("Medium"):
          return "SAM Site Medium Range"
        case id.includes("Short"):
          return "SAM Site Short Range"
      }
    case id.includes("Vehicle"):
      switch (true) {
        case id.includes("APC"):
          return "APC"
        case id.includes("Artillery"):
          return "Artillery"
        case id.includes("Infantry"):
          return "Infantry"
        case id.includes("MBT"):
          return "MBT"
        case id.includes("Missile"):
          return "Missile Launcher"
        case id.includes("Transport"):
          return "Transport"
        default:
          return "Vehicle Type Unknown"
      }
    case id.includes("Ship"):
      switch (true) {
        case id.includes("Cruiser"):
          return "Cruiser"
        case id.includes("Frigate"):
          return "Frigate"
        case id.includes("Submarine"):
          return "Submarine"
        default:
          return "Ship"
      }
    case id.includes("Static"):
      switch (true) {
        case id.includes("Military"):
          return "Military Facility"
        case id.includes("Production"):
          return "Production Facility"
        case id.includes("Offshore"):
          return "Offshore Facility"
        default:
          return "Facility"
      }
  }
}

function GetColour(id) {
  switch (true) {
    case id.includes("Enemy"):
      return '#bb0000'
    case id.includes("Ally"):
      return '#5555bb'
    case id.includes("RED"):
      return '#ff000055'
    case id.includes("BLUE"):
      return '#0000ff55'
    case id.includes('WATER'):
      return '#00000000'
    case id.includes('NOSPAWN'):
      return '#50eb5d55'
    case id.includes('ISLAND'):
      return '#d4eb5088'
    case id.includes("AIRBASE"):
      return '#ffffff'
    case id.includes("OBJECTIVE"):
      return '#eba134'
    case id.includes("FOB"):
      return '#b0b0b0'
    case id.includes("CARRIER"):
      return '#919191'
    default:
      return '#ffffff'
  }
}


function GetNatoIcon(id) {
  let prefix = ""
  switch (true) {
    case id.includes("Enemy"):
      prefix = "RED_"
      break
    case id.includes("Ally"):
      prefix = "BLUE_"
      break
  }
  switch (true) {
    case id.includes("AIRBASE"):
      return prefix + "AIRBASE"
    case id.includes("OBJECTIVE"):
      return "OBJECTIVE"
    case id.includes("FOB"):
      return 'FOB'
    case id.includes("CARRIER"):
      return 'CARRIER'
    case id.includes("SAM"):
      switch (true) {
        case id.includes("Long"):
          return prefix + "SAM_LONG"
        case id.includes("Medium"):
          return prefix + "SAM_MID"
        case id.includes("Short"):
          return prefix + "SAM_SM"
      }
    case id.includes("Vehicle"):
      switch (true) {
        case id.includes("APC"):
          return prefix + "VEHICLE_APC"
        case id.includes("Artillery"):
          return prefix + "VEHICLE_ARTILLERY"
        case id.includes("Infantry"):
          return prefix + "VEHICLE_INFANTRY"
        case id.includes("MBT"):
          return prefix + "VEHICLE_MBT"
        case id.includes("Missile"):
          return prefix + "VEHICLE_MISSILE"
        case id.includes("Transport"):
          return prefix + "VEHICLE_TRANSPORT"
        default:
          return prefix + "VEHICLE"
      }
    case id.includes("Ship"):
      switch (true) {
        case id.includes("Cruiser"):
          return prefix + "SHIP_CRUISER"
        case id.includes("Frigate"):
          return prefix + "SHIP_FRIGATE"
        case id.includes("Submarine"):
          return prefix + "SHIP_SUBMARINE"
        default:
          return prefix + "SHIP"
      }
    case id.includes("Static"):
      switch (true) {
        case id.includes("Military"):
          return prefix + "STATIC_MILITARY"
        case id.includes("Production"):
          return prefix + "STATIC_PRODUCTION"
        case id.includes("Offshore"):
          return prefix + "STATIC_OFFSHORE"
        default:
          return prefix + "STATIC"
      }
  }
}

function GetArrows(arrLatlngs, color, arrowCount, mapObj) {

  if (typeof arrLatlngs === undefined || arrLatlngs == null ||
    (!arrLatlngs.length) || arrLatlngs.length < 2)
    return [];

  if (typeof arrowCount === 'undefined' || arrowCount == null)
    arrowCount = 1;

  if (typeof color === 'undefined' || color == null)
    color = '';
  else
    color = 'color:' + color;

  var result = [];
  for (var i = 1; i < arrLatlngs.length; i++) {
    var icon = L.divIcon({ className: 'arrow-icon', bgPos: [5, 5], html: '<div style="' + color + ';transform: rotate(' + GetAngle(arrLatlngs[i - 1], arrLatlngs[i], -1).toString() + 'deg)">▶</div>' });
    for (var c = 1; c <= arrowCount; c++) {
      result.push(L.marker(MyMidPoint(arrLatlngs[i], arrLatlngs[i - 1], (c / (arrowCount + 1)), mapObj), { icon: icon }));
    }
  }
  return result;
}

function GetAngle(latLng1, latlng2, coef) {
  var dy = latlng2[0] - latLng1[0];
  var dx = Math.cos(Math.PI / 180 * latLng1[0]) * (latlng2[1] - latLng1[1]);
  var ang = ((Math.atan2(dy, dx) / Math.PI) * 180 * coef);
  return (ang).toFixed(2);
}

function MyMidPoint(latlng1, latlng2, per, mapObj) {
  if (!mapObj)
    throw new Error('map is not defined');

  var halfDist, segDist, dist, p1, p2, ratio,
    points = [];

  p1 = mapObj.project(new L.latLng(latlng1));
  p2 = mapObj.project(new L.latLng(latlng2));
  halfDist = DistanceTo(p1, p2) * per;

  if (halfDist === 0)
    return mapObj.unproject(p1);

  dist = DistanceTo(p1, p2);

  if (dist > halfDist) {
    ratio = (dist - halfDist) / dist;
    var res = mapObj.unproject(new Point(p2.x - ratio * (p2.x - p1.x), p2.y - ratio * (p2.y - p1.y)));
    return [res.lat, res.lng];
  }

}

function DistanceTo(p1, p2) {
  var x = p2.x - p1.x,
    y = p2.y - p1.y;

  return Math.sqrt(x * x + y * y);
}

function ToPoint(x, y, round) {
  if (x instanceof Point) {
    return x;
  }
  if (isArray(x)) {
    return new Point(x[0], x[1]);
  }
  if (x === undefined || x === null) {
    return x;
  }
  if (typeof x === 'object' && 'x' in x && 'y' in x) {
    return new Point(x.x, x.y);
  }
  return new Point(x, y, round);
}

function Point(x, y, round) {
  this.x = (round ? Math.round(x) : x);
  this.y = (round ? Math.round(y) : y);
}
