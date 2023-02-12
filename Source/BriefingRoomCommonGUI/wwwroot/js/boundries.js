function ConvertDMSToDD(degrees, minutes, seconds, direction) {
    var dd = degrees + minutes/60 + seconds/(60*60);

    if (direction == "S" || direction == "W") {
        dd = dd * -1;
    } // Don't do anything for N or E
    return dd;
}

const MapBoundaries = {
    "Caucasus": [
        // top left
        {
            lat: 48.387663480938,
            lon: 26.778743595881,
        },
        // bottom left
        {
            lat:  47.382221906262,
            lon: 49.309787386754,
        },
        // bottom right
        {
            lat: 38.86511140611,
            lon: 47.142314272867,
        },
        // top right
        {
            lat: 39.608931903399,
            lon: 27.637331401126,
        },
    ],

    "Syria": [
        // top left
        {
            lat:  37.470301761465,
            lon: 29.480123666167,
        },
        // bottom left
        {
            lat: 37.814134114831,
            lon: 42.148931009427,
        },
        // bottom right
        {
            lat: 31.960960214436,
            lon: 41.932899899137,
        },
        // top right
        {
            lat: 31.683960285685,
            lon: 30.123622480902,
        },
    ],

    "PersianGulf": [
        {
            lat: 32.955527544002,
            lon: 46.583433745255
        },
        {
            lat: 33.150981840679,
            lon: 64.756585025318
        },
        {
            lat: 21.869681127563,
            lon: 63.997389263298
        },
        {
            lat: 21.749230188233,
            lon: 47.594358099874
        },
    ],

    "TheChannel": [
        // top left
        {
            lat: 51.5174,
            lon: -0.08918
        },
        // bottom left
        {
            lat: 49.6773349,
            lon: 0.0290759
        },
        // bottom right
        {
            lat: 49.7137467,
            lon: 3.4207356
        },
        // top right
        {
            lat: 51.5572647,
            lon: 3.4399315
        },
    ],


    "Normandy": [
        // top left
        {
            lat: 51.4869242,
            lon: -4.2146587
        },
        // bottom left
        {
            lat: 48.3421117,
            lon: -4.1381245
        },
        // bottom right
        {
            lat: 48.2294762,
            lon: 2.184522
        },
        // top right
        {
            lat: 51.3637243,
            lon: 2.5322495
        },
    ],

    "Nevada":[
            {
                lat: 39.801712624973,
                lon: -119.9902311096,
            },
            {
                lat: 39.737162541546,
                lon: -112.44599267994,
            },
            {
                lat: 34.346907399159,
                lon: -112.4519427,
            },
            {
                lat: 34.400025213159,
                lon: -119.78488669575,
            },
        ],

    "MarianaIslands":
        [
            {
                lat:22.220143285088,
                lon: 136.96126049266
            },
            {
                lat: 22.44081213808,
                lon: 152.4517401234
            },
            {
                lat: 10.739229846557,
                lon: 152.12973515767
            },
            {
                lat: 10.637681299806,
                lon: 137.54638410345
            }
        ],

}