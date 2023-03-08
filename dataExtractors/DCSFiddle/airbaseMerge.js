const { writeFileSync } = require('fs');
const terrain = require('./dumps/terrain.json');
const missionAirbase = require('./dumps/missionAirbase.json');
const beaconMap = {
    0: "NULL",
    1: "VOR",
    2: "DME",
    3: "VOR_DME",
    4: "TACAN",
    5: "VORTAC",
    8: "HOMER",
    128: "RSBN",
    1024: "BROADCAST_STATION",
    4104: "AIRPORT_HOMER",
    4136: "AIRPORT_HOMER_WITH_MARKER",
    16408: "ILS_FAR_HOMER",
    16424: "ILS_NEAR_HOMER",
    16640: "ILS_LOCALIZER",
    16896: "ILS_GLIDESLOPE",
    33024: "PRMG_LOCALIZER",
    33280: "PRMG_GLIDESLOPE",
    65536: "NAUTICAL_HOMER",
    131328: "ICLS_LOCALIZER",
    131584: "ICLS_GLIDESLOPE"
  }
  

const getAirbaseBeacons = airbase => {
    const list = []
    if(!airbase.raw.terrainData.beacons|| !Object.keys(airbase.raw.terrainData.beacons).length) return list
    let beacons = airbase.raw.terrainData.beacons
    if (!Array.isArray(beacons))
    {
        beacons = Object.values(beacons) 
    }
    beacons.forEach(element => {
        list.push(terrain.beacons.find(x => x.beaconId == element.beaconId))
    });
    list.forEach(x => x.typeStr = beaconMap[x.type])
    return list
}

const getAirbaseRadios = airbase => {
    const list = []
    airbase.raw.terrainData.radio.forEach(element => {
        list.push(terrain.radios.find(x => x.radioId == element))
    });
    return list
}

const formatRadioFrequency =  (val) => (val/1000000.0).toFixed(2)

const getAirdromeData = airbase => {
    const radios = []
    let runways = airbase.raw.terrainData.runways
    if (!Array.isArray(runways))
    {
        runways = Object.values(runways) 
    }
    airbase.raw.radios.forEach(radio => radio.frequency.forEach(feq => radios.push(formatRadioFrequency(feq[1]))))
    return {
        ATC: radios,
        TACAN: airbase.raw.beacons.filter(x => x.typeStr === "TACAN").map(x => `${x.channel}X`),
        ILS: airbase.raw.beacons.filter(x => x.typeStr === "ILS_GLIDESLOPE").map(x => formatRadioFrequency(x.frequency)),
        RUNWAYS: runways.map(x => x.name)
    }
}
Object.keys(missionAirbase).forEach(airbaseID => {
    const airbase = missionAirbase[airbaseID]
    airbase.raw = {}
    airbase.raw.terrainData = terrain.airdromes[airbaseID]
    airbase.raw.beacons = getAirbaseBeacons(airbase)
    airbase.raw.radios = getAirbaseRadios(airbase)
    airbase.airdromeData = getAirdromeData(airbase)
});

writeFileSync("Airbases.json", JSON.stringify(missionAirbase, null, 2), 'utf8');