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
            lat: ConvertDMSToDD(48, 23, 15, 'N'),
            lon: ConvertDMSToDD(26, 46, 43, 'E'),
        },
        // bottom left
        {
            lat: ConvertDMSToDD(39, 36, 32, "N"),
            lon: ConvertDMSToDD(27, 38, 14, "E"),
        },
        // bottom right
        {
            lat: ConvertDMSToDD(38, 51, 54, 'N'),
            lon: ConvertDMSToDD(47, 8, 32, 'E'),
        },
        // top right
        {
            lat: ConvertDMSToDD(47, 22, 56, 'N'),
            lon: ConvertDMSToDD(49, 18, 35, 'E'),
        },
    ],

    "Syria": [
        // top left
        {
            lat: ConvertDMSToDD(37, 21.654, 0, 'N'),
            lon: ConvertDMSToDD(29, 16.483, 0, 'E'),
        },
        // bottom left
        {
            lat: ConvertDMSToDD(31, 50.940, 0, 'N'),
            lon: ConvertDMSToDD(29, 53.849, 0, 'E'),
        },
        // bottom right
        {
            lat: ConvertDMSToDD(32, 8.514, 0, 'N'),
            lon: ConvertDMSToDD(42, 8.502, 0, 'E'),
        },
        // top right
        {
            lat: ConvertDMSToDD(37, 43.72, 0, "N"),
            lon: ConvertDMSToDD(42, 22.301, 0, "E"),
        },
    ],

    "Persian Gulf": [
        {
            lat: 32.9355285,
            lon: 46.5623682
        },
        {
            lat: 21.729393,
            lon: 47.572675
        },
        {
            lat: 21.8501348,
            lon: 63.9734737
        },
        {
            lat: 33.131584,
            lon: 64.7313594
        },
    ],

    "Channel": [
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

    "Nevada":
        [
            {
                lat: 38.8449971,
                lon: -118.1394373,
            },
            {
                lat: 34.3919386,
                lon: -118.1394373,
            },
            {
                lat: 34.3919386,
                lon: -112.4519427,
            },
            {
                lat: 38.8449971,
                lon: -112.4519427,
            },
        ],

    "Marianas":
        [
            {
                lat: 22.09,
                lon: 135.0572222
            },
            {
                lat: 10.5777778,
                lon: 135.7477778
            },
            {
                lat: 10.7725,
                lon: 149.3918333
            },
            {
                lat: 22.5127778,
                lon: 149.5427778
            }
        ],

}