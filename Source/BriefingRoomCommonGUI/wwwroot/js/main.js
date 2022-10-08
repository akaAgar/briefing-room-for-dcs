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
  if(leafMap) {
    leafMap.off();
    leafMap.remove();
  }

  try {
    mapGroups = {
      "SAMLongRange": new L.layerGroup(),
      "SAMMediumRange": new L.layerGroup(),
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
    } else if (key.includes("WAYPOINTS")) {
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
    ToggleLayer("SAMLongRange")
  }).addTo(leafMap);
  L.easyButton('oi oi-audio', function (btn, map) {
    ToggleLayer("SAMMediumRange")
  }).addTo(leafMap);
  L.easyButton('oi oi-audio', function (btn, map) {
    ToggleLayer("GroundForces")
  }).addTo(leafMap);
}

function AddIcon(key, data, map, MapCoordMap) {
  if (key.startsWith("UNIT")) {
    AddUnit(key, data, map, MapCoordMap)
  } else {
    new L.Marker(GetFromMapCoordData(data[0], MapCoordMap), {
      title: GetTitle(key),
      icon: new L.DivIcon({
        html: `<div class="map_point_icon ${key.includes("OBJECTIVE_SMALL") ? 'map_unit': ''}" style="background-color: ${GetColour(key)};">${GetText(key)}</div>`
      }),
      zIndexOffset: key == "AIRBASE_HOME" || key.includes("OBJECTIVE_AREA") ? 200 : 100
    }).addTo(map)
  }
}

function AddUnit(key, data, map, MapCoordMap) {
  const coords = GetFromMapCoordData(data[0], MapCoordMap)
  group = mapGroups[GetGroup(key)]
  group.addLayer(new L.Marker(coords, {
    title: GetTitle(key),
    icon: new L.DivIcon({
      html: `<div class="map_point_icon map_unit" style="background-color: ${GetColour(key)};">${GetText(key)}</div>`
    })
  }))
  const range = GetRange(key);
  if (range > 0) {
    group.addLayer(new L.Circle(coords, {
      radius: range,
      color: GetColour(key),
      fillColor: GetColour(key),
      fillOpacity: 0.4
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
    fillOpacity: 0.4,
  }).addTo(map);
}

function GetGroup(id) {
  switch (true) {
    case id.includes("LongRange"):
      return "SAMLongRange";
    case id.includes("MediumRange"):
      return "SAMMediumRange";
    default:
      return "GroundForces";
  }
}

function GetRange(id) {
  switch (true) {
    case id.includes("LongRange"):
      return 40 * 1852;
    case id.includes("MediumRange"):
      return 20 * 1852;
    default:
      return 0
  }
}

function GetTitle(id) {
  switch (true) {
    case id.includes("AIRBASE"):
      return 'Airbase'
    case id.includes("OBJECTIVE"):
      return 'Objective'
    case id.includes("FOB"):
      return 'FOB'
    case id.includes("CARRIER"):
      return 'Carrier'
    case id.includes("SAM"):
      return 'SAM site'
    case id.includes("Vehicle"):
      return 'Vehicles'
    case id.includes("Ship"):
      return 'Boat'
    case id.includes("Static"):
      return 'Facility'
    default:
      return null
  }
}

function GetText(id) {
  switch (true) {
    case id.includes("AIRBASE"):
      return 'A'
    case id.includes("OBJECTIVE"):
      return 'O'
    case id.includes("FOB"):
      return 'F'
    case id.includes("CARRIER"):
      return 'C'
    case id.includes("SAM"):
      return 'S'
    case id.includes("Vehicle"):
      return 'V'
    case id.includes("Ship"):
      return 'B'
    case id.includes("Static"):
      return 'F'
    default:
      return null
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
