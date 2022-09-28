let map
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

function CopyLogs(logs) {
  navigator.clipboard.writeText(logs.join('\n'));
}


const getMinCoors = (arr) => [Math.min.apply(Math, arr.map(x => x[0])), Math.min.apply(Math, arr.map(x => x[1]))]
const getMaxCoors = (arr) => [Math.max.apply(Math, arr.map(x => x[0])), Math.max.apply(Math, arr.map(x => x[1]))]

function RenderMap(mapData) {
  try {
    map = L.map('map')
    L.esri.basemapLayer("Imagery").addTo(map);
    L.esri.basemapLayer("ImageryLabels").addTo(map);

    // L.esri.Vector.vectorBasemapLayer("ArcGIS:Imagery", { apiKey: apiKey }).addTo(map);
  } catch (error) {

  }
  Object.keys(mapData).forEach(key => {
    console.log(key, [0])
    data = mapData[key]
    if (data.length == 1) {
      new L.Marker(mapData[key][0], {
        icon: new L.DivIcon({
            html: `<div class="map_point_icon" style="background-color: ${GetColour(key)};">${GetText(key)}</div>`
        })
    }).addTo(map)
    } else {
      L.polygon(mapData[key], {
        color: GetColour(key),
        fillColor: GetColour(key),
        fillOpacity: 0.8,
      }).addTo(map);

    }
  })
  map.setView(mapData["AIRBASE_HOME"][0], 5);
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

function RenderDot(x, y, color, text, ctx) {
  ctx.strokeStyle = "#000000";
  ctx.fillStyle = color;
  ctx.beginPath();
  ctx.arc(x, y, 10, 0, 2 * Math.PI);
  ctx.stroke();
  ctx.fill();
  if (text) {
    ctx.fillStyle = "#000000";
    ctx.textAlign = "center";
    ctx.font = "15px Arial";
    ctx.fillText(text, x, y + 4);
  }
}

function RenderPolygon(coords, color, ctx, isWater) {
  ctx.strokeStyle = isWater ? '#00000000' : "#000000";
  ctx.fillStyle = color;
  ctx.beginPath();
  let first = true
  coords.forEach(coord => {
    if (first) {
      ctx.moveTo(coord[0], coord[1])
      first = false
    } else {
      ctx.lineTo(coord[0], coord[1])
    }
  })
  ctx.closePath();
  ctx.stroke();
  ctx.fill();
}

function clearCanvas(ctx, canvas) {
  const canvasW = canvas.getBoundingClientRect().width;
  const canvasH = canvas.getBoundingClientRect().height;
  ctx.clearRect(0, 0, canvasW, canvasH);
}

function centerData(mapData) {
  const clonedMap = structuredClone(mapData);
  const minCoords = getMinCoors(Object.keys(clonedMap).map(key => getMinCoors(clonedMap[key])));
  Object.keys(clonedMap).forEach(key => {
    clonedMap[key].forEach(coord => {
      coord[0] = coord[0] + (minCoords[0] * -1)
      coord[1] = coord[1] + (minCoords[1] * -1)
    })
  })
  return clonedMap
}

function scaleCoordinates(mapData, canvas) {
  const canvasW = canvas.getBoundingClientRect().width;
  const canvasH = canvas.getBoundingClientRect().height;
  const clonedMap = structuredClone(mapData);
  const maxCoords = getMaxCoors(Object.keys(clonedMap).map(key => getMaxCoors(clonedMap[key])));
  const scaleMultiplier = canvasW / (maxCoords[0] > maxCoords[1] ? maxCoords[0] : maxCoords[1]);

  Object.keys(clonedMap).forEach(key => {
    clonedMap[key].forEach(coord => {
      coord[0] = (coord[0] * scaleMultiplier)
      coord[1] = canvasH - (coord[1] * scaleMultiplier)
    })
  })
  return clonedMap;
}

