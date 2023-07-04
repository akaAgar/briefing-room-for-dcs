const fs = require('fs');
const clustersKmeans = require("@turf/clusters-kmeans").default
const { point, featureCollection } = require('@turf/helpers')
const mission = fs.readFileSync('./emptyMission.lua', 'utf8');
const unit = fs.readFileSync('./unit.lua', 'utf8');
const pointLua = fs.readFileSync('./point.lua', 'utf8');
const spawnPoints = JSON.parse(fs.readFileSync('../../DatabaseJSON/TheaterSpawnPoints/Caucasus.json', 'utf8'))
let modmission = mission.replaceAll("$THEATER$", "Caucasus")

const spots = {
    "LandSmall": [],
    "LandMedium": [],
    "LandLarge": []
}


spawnPoints.forEach(spot => {
    spots[spot.BRtype].push(point(spot.coords))
});

let globIndex = 1;


Object.keys(spots).forEach(spType => {
    var tempunits = []
    var options = { numberOfClusters: Math.ceil(spots[spType].length / 50) };
    var clustered = clustersKmeans(featureCollection(spots[spType]), options);
    var cluserObj = {};
    clustered.features.forEach((feat, gi) => {
        if(!cluserObj[feat.properties.cluster])
        {
            cluserObj[feat.properties.cluster] = []
        }
        cluserObj[feat.properties.cluster].push(feat.geometry.coordinates)
    })
    Object.keys(cluserObj).forEach((k, gi) => {
        const batch = cluserObj[k];
        let tempUnit = unit;
        tempUnit = tempUnit.replaceAll("$X$", batch[0][0])
        tempUnit = tempUnit.replaceAll("$Y$", batch[0][1])
        let unitPoints = batch.map((x, i) => {
            let tempPoint = pointLua;
            tempPoint = tempPoint.replaceAll("$FIRST$", i === 0)
            tempPoint = tempPoint.replaceAll("$IDX$", i + 1)
            tempPoint = tempPoint.replaceAll("$X$", x[0])
            tempPoint = tempPoint.replaceAll("$Y$", x[1])
            return tempPoint
        })
        tempUnit = tempUnit.replaceAll("$POINTS$", unitPoints.join("\n"))
        tempUnit = tempUnit.replaceAll("$IDX$", gi + 1)
        tempUnit = tempUnit.replaceAll("$GLOBIDX$", globIndex)
        tempunits.push(tempUnit)
        globIndex++
    })
    modmission = modmission.replaceAll(`$${spType}$`, tempunits.join("\n"))
})


fs.writeFileSync('./out/mission.lua', modmission)

