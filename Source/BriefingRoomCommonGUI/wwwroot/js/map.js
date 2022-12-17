const waypointColors = ["Cyan", "orange", "Chartreuse", "Magenta", "DeepPink", "Gold"]
let mapGroups = {}
const situationMapLayers = {
    "BLUE": null,
    "RED": null,
    "NEUTRAL": null,
}
let leafMap, leafHintMap, leafSituationMap, hintMarkerMap, hintTarget


let hintPositions = {}
let hintMarkers = {}

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

function GetFromMapCoordDataXY(pos, mapCoordData){
    const postArry = GetFromMapCoordData(pos, mapCoordData)
    return {x: postArry[0], y:postArry[1]}
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

async function SetHintPositions(positionsDict, map){
    hintPositions = {}
    var MapCoordMap = await GetMapData(map)
    Object.keys(positionsDict).forEach(key => {
        hintPositions[key] = GetFromMapCoordDataXY(positionsDict[key], MapCoordMap)
    })
}

function distance(p, point) {
    return Math.sqrt(Math.pow(point.lat - p.x, 2) + Math.pow(point.lng - p.y, 2))
}

const getNearestValidPos = (pos, MapCoordMap) => Object.values(MapCoordMap).reduce((a, b) => distance(a, pos) < distance(b, pos) ? a : b);

async function RenderHintMap(map, hintKey) {
    //Show hint map
    if(hintKey){
        var bsOffcanvas = new bootstrap.Offcanvas(document.getElementById('offcanvasBottom'))
        bsOffcanvas.show()
    }
    if (map != hintMarkerMap) {
        hintMarkers = {}
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
    Object.keys(hintPositions).forEach(key => {
        createHintMarker(key, map)
    })
    let keys = Object.keys(MapCoordMap);
    let randomPos = MapCoordMap[keys[Math.floor(keys.length * Math.random())]];
    leafHintMap.setView([randomPos["x"], randomPos["y"]], 6.5);
    if(hintKey){
        leafHintMap.once('click', function (e) {
            hintMarkerMap = map
            hintPositions[hintKey] = getNearestValidPos(e.latlng, MapCoordMap)
            if (hintKey in hintMarkers) {
                hintMarkers[hintKey].setLatLng([hintPositions[hintKey].x, hintPositions[hintKey].y])
            } else {
                createHintMarker(hintKey, map)
            }
        });
    }
}



function createHintMarker(key, map) {
    var marker = new L.marker([hintPositions[key].x, hintPositions[key].y], {
        icon: new L.DivIcon({
            className: 'my-div-icon',
            html: `<img class="map_point_icon" src="_content/BriefingRoomCommonGUI/img/nato-icons/${GetNatoIcon(key, false)}.svg" alt="${key}"/>` +
                `<span class="map_point_icon">${key.split('_')[1]}</span>`
        }),
        draggable:'true'
    })
    hintMarkers[key] = marker
    marker.addTo(leafHintMap);
    hintMarkers[key].on('dragend', async function(event){
        var marker = event.target;
        var position = marker.getLatLng();
        var MapCoordMap = await GetMapData(map)
        hintPositions[key] = getNearestValidPos(position, MapCoordMap)
        marker.setLatLng(position,{id:uni,draggable:'true'}).bindPopup(position).update();
    });
}

async function GetHintPoints(map) {
    var MapCoordMap = await GetMapData(map)
    const data = {}
    Object.keys(hintPositions).forEach(hintKey => {
        for (const key in MapCoordMap) {
            const pos = MapCoordMap[key]
            if (hintPositions[hintKey].x == pos.x && hintPositions[hintKey].y == pos.y) {
                const parts = key.replace("x:", "").replace("z:", "").split(",")
                data[hintKey] = parts.map(x => parseFloat(x));
                return
            }
        }
    });
    return data
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
                repeatMode: false
            },
            polyline: false,
            rectangle: false,
            circle: false,
            circlemarker: false,
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

function CoordFind(coord, MapCoordMap) {
    return Object.keys(MapCoordMap).map(k => ({ k, d: distance(MapCoordMap[k], coord), v: MapCoordMap[k] })).reduce((min, v) => min.d <= v.d ? min : v, { d: Infinity })
}


function CoordToString(spot) {
    return spot.k.replace("x:", "").replace("z:", "")
}

function CreateCoordsString(layer, MapCoordMap) {
    const coordMap = layer.editing.latlngs[0][0].map(x => CoordFind(x, MapCoordMap))
    const newCoords = coordMap.map(x => ({ lat: x.v.x, lng: x.v.y }))
    layer.setLatLngs(newCoords)
    return coordMap.map((x, i) => `Waypoint${i.toString().padStart(4, "0")}=${CoordToString(x)}`).join("\n")
}

async function GetSituationCoordinates(map) {
    let redCoordsString, blueCoordsString, neutralCoordString;
    var MapCoordMap = await GetMapData(map)

    blueCoordsString = CreateCoordsString(situationMapLayers.BLUE, MapCoordMap)
    redCoordsString = CreateCoordsString(situationMapLayers.RED, MapCoordMap)
    if (situationMapLayers.NEUTRAL) {
        neutralCoordString = CreateCoordsString(situationMapLayers.NEUTRAL, MapCoordMap)
    }
    return [redCoordsString, blueCoordsString, neutralCoordString]
}

async function RenderMap(mapData, map, inverted) {
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
            AddIcon(key, data, leafMap, MapCoordMap, inverted)
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

function AddIcon(key, data, map, MapCoordMap, inverted) {
    if (key.startsWith("UNIT")) {
        AddUnit(key, data, map, MapCoordMap, inverted)
    } else if (key.includes("WAYPOINT_")) {
        AddWaypoint(key, data, map, MapCoordMap)
    } else {
        new L.Marker(GetFromMapCoordData(data[0], MapCoordMap), {
            title: GetTitle(key),
            icon: new L.DivIcon({
                html: `<img class="map_point_icon" src="_content/BriefingRoomCommonGUI/img/nato-icons/${GetNatoIcon(key, inverted)}.svg" alt="${key}"/>`
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

function AddUnit(key, data, map, MapCoordMap, inverted) {
    const coords = GetFromMapCoordData(data[0], MapCoordMap)
    group = mapGroups[GetGroup(key)]
    group.addLayer(new L.Marker(coords, {
        title: GetTitle(key),
        icon: new L.DivIcon({
            html: `<img class="map_point_icon map_unit"src="_content/BriefingRoomCommonGUI/img/nato-icons/${GetNatoIcon(key, inverted)}.svg" alt="${key}"/>`
        })
    }))
    const range = GetRange(key);
    if (range > 0) {
        group.addLayer(new L.Circle(coords, {
            radius: range,
            color: GetColour(key, inverted),
            fillColor: GetColour(key, inverted),
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
        case id.includes("SUPPLY"):
            return 'Supply Base'
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

function GetColour(id, inverted) {
    switch (true) {
        case id.includes("Enemy"):
            return inverted ? '#5555bb' : '#bb0000'
        case id.includes("Ally"):
            return inverted ? '#bb0000' : '#5555bb'
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


function GetNatoIcon(id, invert = false) {
    let prefix = invert ? "RED_" : "BLUE_"
    switch (true) {
        case id.includes("Enemy"):
            prefix = invert ? "BLUE_" : "RED_"
            break
        case id.includes("Neutral"):
            prefix = "GREEN_"
            break
    }
    switch (true) {
        case id.includes("AIRBASE"):
            return prefix + "AIRBASE"
        case id.includes("OBJECTIVE"):
            return "OBJECTIVE"
        case id.includes("FOB"):
            return prefix + 'FOB'
        case id.includes("CARRIER"):
            return prefix + 'CARRIER'
        case id.includes("SUPPLY"):
            return prefix + 'SUPPLY'
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
