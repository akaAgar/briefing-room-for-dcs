let map
const waypointColors = ["Cyan", "orange", "Chartreuse", "Magenta", "DeepPink", "Gold"]
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

function getFromMapCoordData(pos, mapCoordData) {
  x = Math.round(pos[0] / 1000) * 1000
  z = Math.round(pos[1] / 1000) * 1000
  key = `x:${x},z:${z}`
  pos2 = mapCoordData[key]
  if (pos2 == undefined) {
    throw `Key ${key} not found in positional data.`
  }
  return [pos2["x"], pos2["y"]]
}

async function RenderMap(mapData, map) {
  try {
    const response = await fetch(`_content/BriefingRoomCommonGUI/j/${map}.json.gz`)
    const fileReader = await response.arrayBuffer();
    const binData = new Uint8Array(fileReader);
    var MapCoordMap = JSON.parse(pako.ungzip(binData,{ 'to': 'string' }));
  } catch (error) {
    throw `Either can't find ${map} data file or failed to parse it. raw error: ${error}`
  }
  
  try {
    map = L.map('map')
    L.esri.basemapLayer("Imagery").addTo(map);
    L.esri.basemapLayer("ImageryLabels").addTo(map);
  } catch (error) {

  }
  Object.keys(mapData).forEach(key => {
    if (key.includes('ISLAND')) {
      return
    }
    data = mapData[key]
    if (data.length == 1) {
      new L.Marker(getFromMapCoordData(data[0], MapCoordMap), {
        icon: new L.DivIcon({
          html: `<div class="map_point_icon" style="background-color: ${GetColour(key)};">${GetText(key)}</div>`
        })
      }).addTo(map)
    } else if (key.includes("WAYPOINTS")) {
      let color = waypointColors[Math.floor(Math.random() * waypointColors.length)];
      let coords = data.map(x => getFromMapCoordData(x, MapCoordMap))
      new L.polyline(coords, {
        color: color,
        weight: 2,
        opacity: 1,
        smoothFactor: 2
      }).addTo(map);
      L.featureGroup(getArrows(coords, color, 1, map)).addTo(map);
    } else {
      let coords = data.map(x => getFromMapCoordData(x, MapCoordMap))
      L.polygon(coords, {
        color: GetColour(key),
        fillColor: GetColour(key),
        fillOpacity: 0.8,
      }).addTo(map);
    }
  })
  map.setView(getFromMapCoordData(mapData["AIRBASE_HOME"][0], MapCoordMap), 5);
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
    default:
      return null
  }
}

function GetColour(id) {
  switch (true) {
    case id === "RED":
      return '#ff000055'
    case id === "BLUE":
      return '#0000ff55'
    case id === 'WATER':
      return '#00000000'
    case id === 'NOSPAWN':
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

function getArrows(arrLatlngs, color, arrowCount, mapObj) {

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
    var icon = L.divIcon({ className: 'arrow-icon', bgPos: [5, 5], html: '<div style="' + color + ';transform: rotate(' + getAngle(arrLatlngs[i - 1], arrLatlngs[i], -1).toString() + 'deg)">▶</div>' });
    for (var c = 1; c <= arrowCount; c++) {
      result.push(L.marker(myMidPoint(arrLatlngs[i], arrLatlngs[i - 1], (c / (arrowCount + 1)), mapObj), { icon: icon }));
    }
  }
  return result;
}

function getAngle(latLng1, latlng2, coef) {
  var dy = latlng2[0] - latLng1[0];
  var dx = Math.cos(Math.PI / 180 * latLng1[0]) * (latlng2[1] - latLng1[1]);
  var ang = ((Math.atan2(dy, dx) / Math.PI) * 180 * coef);
  return (ang).toFixed(2);
}

function myMidPoint(latlng1, latlng2, per, mapObj) {
  if (!mapObj)
    throw new Error('map is not defined');

  var halfDist, segDist, dist, p1, p2, ratio,
    points = [];

  p1 = mapObj.project(new L.latLng(latlng1));
  p2 = mapObj.project(new L.latLng(latlng2));

  halfDist = distanceTo(p1, p2) * per;

  if (halfDist === 0)
    return mapObj.unproject(p1);

  dist = distanceTo(p1, p2);

  if (dist > halfDist) {
    ratio = (dist - halfDist) / dist;
    var res = mapObj.unproject(new Point(p2.x - ratio * (p2.x - p1.x), p2.y - ratio * (p2.y - p1.y)));
    return [res.lat, res.lng];
  }

}

function distanceTo(p1, p2) {
  var x = p2.x - p1.x,
    y = p2.y - p1.y;

  return Math.sqrt(x * x + y * y);
}

function toPoint(x, y, round) {
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
