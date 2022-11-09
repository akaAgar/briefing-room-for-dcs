const waypointColors = ["Cyan", "orange", "Chartreuse", "Magenta", "DeepPink", "Gold"]
let mapGroups = {}
const situationMapLayers = {
    "BLUE": null,
    "RED": null,
    "NEUTRAL": null,
}
let leafMap, leafHintMap, leafSituationMap, hintPos, hintMarker, hintMarkerMap

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

function distance(p, point) {
    return Math.sqrt(Math.pow(point.lat - p.x, 2) + Math.pow(point.lng - p.y, 2))
}

async function RenderHintMap(map) {
    if (map != hintMarkerMap) {
        hintPos = null
        hintMarker = null
    }
    var MapCoordMap = await GetMapData(map)
    if (leafHintMap) {
        leafHintMap.off();
        leafHintMap.remove();
    }

    try {
        leafHintMap = L.map('hintMap')
        L.esri.basemapLayer("Imagery").addTo(leafHintMap);
        L.esri.basemapLayer("ImageryLabels").addTo(leafHintMap);
    } catch (error) {
        console.warn(error)
    }
    if (hintPos) {
        hintMarker = new L.marker([hintPos.x, hintPos.y]).addTo(leafHintMap);
    }
    let keys = Object.keys(MapCoordMap);
    let randomPos = MapCoordMap[keys[Math.floor(keys.length * Math.random())]];
    leafHintMap.setView([randomPos["x"], randomPos["y"]], 6.5);
    leafHintMap.on('click', function (e) {
        hintMarkerMap = map
        hintPos = Object.values(MapCoordMap).reduce((a, b) => distance(a, e.latlng) < distance(b, e.latlng) ? a : b);
        if (!hintMarker) {
            hintMarker = new L.marker([hintPos.x, hintPos.y]).addTo(leafHintMap);
        } else {
            hintMarker.setLatLng([hintPos.x, hintPos.y])
        }
    });
}

async function GetHintPoint(map) {
    var MapCoordMap = await GetMapData(map)
    for (const key in MapCoordMap) {
        const pos = MapCoordMap[key]
        if (hintPos.x == pos.x && hintPos.y == pos.y) {
            const parts = key.replace("x:", "").replace("z:", "").split(",")
            return parts.map(x => parseFloat(x));
        }
    }
}

async function RenderEditorMap(map) {
    var MapCoordMap = await GetMapData(map)
    if (leafSituationMap) {
        leafSituationMap.off();
        leafSituationMap.remove();
    }

    try {
        leafSituationMap = L.map('situationMap')
        L.esri.basemapLayer("Imagery").addTo(leafSituationMap);
        L.esri.basemapLayer("ImageryLabels").addTo(leafSituationMap);
    } catch (error) {
        console.warn(error)
    }
    const MapCoordArray = Object.values(MapCoordMap);
    const XCoords = MapCoordArray.map(o => o.x)
    const YCoords = MapCoordArray.map(o => o.y)
    const bounds = [[XCoords.reduce((min, v) => min <= v ? min : v, Infinity), YCoords.reduce((min, v) => min <= v ? min : v, Infinity)], [XCoords.reduce((max, v) => max >= v ? max : v, -Infinity), YCoords.reduce((max, v) => max >= v ? max : v, -Infinity)]];
    const viewCoords = [(bounds[0][0] + (bounds[1][0] - bounds[0][0]) / 2), (bounds[0][1] + (bounds[1][1] - bounds[0][1]) / 2)]
    leafSituationMap.setView(viewCoords, 6.5);
    L.rectangle(bounds, {
        color: 'Yellow',
        weight: 5,
        fillOpacity: 0.0
    }).addTo(leafSituationMap);
    var drawnItems = new L.FeatureGroup();
    leafSituationMap.addLayer(drawnItems);

    var drawControl = new L.Control.Draw({
        draw: {
            polygon: {
                shapeOptions: {
                    color: 'purple'
                },
                allowIntersection: false,
                drawError: {
                    color: 'orange',
                    timeout: 1000
                },
                showArea: true,
                metric: false,
                repeatMode: true
            },
            polyline: false,
            rect: false,
            circle: false,
            marker: false
        },
        edit: {
            featureGroup: drawnItems
        }
    });
    leafSituationMap.addControl(drawControl);
    leafSituationMap.on('draw:created', function (e) {
        const type = e.layerType,
            layer = e.layer;
        const areaType = document.querySelector('input[name="sideRadio"]:checked').value
        switch (areaType) {
            case "RED":
                layer.options.color = "red"
                break;
            case "BLUE":
                layer.options.color = "blue"
                break;
            case "NEUTRAL":
                layer.options.color = "green"
                break;
            default:
                break;
        }
        if (situationMapLayers[areaType]) {
            situationMapLayers[areaType].remove()
        }
        situationMapLayers[areaType] = layer
        drawnItems.addLayer(layer);
    });
}



function CoordToString(coord, MapCoordMap) {
    const spotKey = Object.keys(MapCoordMap).find(k => distance(MapCoordMap[k], coord) < 0.007)
    if (spotKey == undefined) {
        const min_dist = Object.keys(MapCoordMap).map(k => distance(MapCoordMap[k], coord)).reduce((min, v) => min <= v ? min : v, Infinity)
        throw `Can't find coordinate spot to map to. Nearest is ${min_dist}`
    }
    return spotKey.replace("x:", "").replace("z:", "")
}

function CreateCoordsString(coords, MapCoordMap) {
    return coords.map((x, i) => `Waypoint${i.toString().padStart(4, "0")}=${CoordToString(x, MapCoordMap)}`).join("\n")
}

async function GetSituationCoordinates(map) {
    let redCoordsString, blueCoordsString, neutralCoordString;
    var MapCoordMap = await GetMapData(map)

    blueCoordsString = CreateCoordsString(situationMapLayers.BLUE.editing.latlngs[0][0], MapCoordMap)
    redCoordsString = CreateCoordsString(situationMapLayers.RED.editing.latlngs[0][0], MapCoordMap)
    if (situationMapLayers.NEUTRAL) {
        neutralCoordString = CreateCoordsString(situationMapLayers.NEUTRAL.editing.latlngs[0][0], MapCoordMap)
    }
    return [redCoordsString, blueCoordsString, neutralCoordString]
}

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
    const title = key.replace("WAYPOINT_", "")
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
