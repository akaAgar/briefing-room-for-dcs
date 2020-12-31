mission = 
{
    ["requiredModules"] = 
    {
    }, -- end of ["requiredModules"]
    ["date"] = 
    {
        ["Day"] = $DATEDAY$,
        ["Year"] = $DATEYEAR$,
        ["Month"] = $DATEMONTH$,
    }, -- end of ["date"]
    ["trig"] = 
    {
        ["actions"] = 
        {
            [1] = "a_do_script_file(getValueResourceByKey(\"ResKey_Script\"));",
        }, -- end of ["actions"]
        ["events"] = 
        {
        }, -- end of ["events"]
        ["custom"] = 
        {
        }, -- end of ["custom"]
        ["func"] = 
        {
        }, -- end of ["func"]
        ["flag"] = 
        {
            [1] = true,
        }, -- end of ["flag"]
        ["conditions"] = 
        {
            [1] = "return(true)",
        }, -- end of ["conditions"]
        ["customStartup"] = 
        {
        }, -- end of ["customStartup"]
        ["funcStartup"] = 
        {
            [1] = "if mission.trig.conditions[1]() then mission.trig.actions[1]() end",
        }, -- end of ["funcStartup"]
    }, -- end of ["trig"]
    ["result"] = 
    {
        ["offline"] = 
        {
            ["conditions"] = 
            {
                [1] = "return(c_flag_is_true(1))",
            }, -- end of ["conditions"]
            ["actions"] = 
            {
                [1] = "a_set_mission_result(100)",
            }, -- end of ["actions"]
            ["func"] = 
            {
                [1] = "if mission.result.offline.conditions[1]() then mission.result.offline.actions[1]() end",
            }, -- end of ["func"]
        }, -- end of ["offline"]
        ["total"] = 1,
        ["blue"] = 
        {
            ["conditions"] = 
            {
            }, -- end of ["conditions"]
            ["actions"] = 
            {
            }, -- end of ["actions"]
            ["func"] = 
            {
            }, -- end of ["func"]
        }, -- end of ["blue"]
        ["red"] = 
        {
            ["conditions"] = 
            {
            }, -- end of ["conditions"]
            ["actions"] = 
            {
            }, -- end of ["actions"]
            ["func"] = 
            {
            }, -- end of ["func"]
        }, -- end of ["red"]
    }, -- end of ["result"]
    ["maxDictId"] = 0,
    ["pictureFileNameN"] = 
    {
    }, -- end of ["pictureFileNameN"]
    ["groundControl"] = 
    {
        ["isPilotControlVehicles"] = false,
        ["roles"] = 
        {
            ["artillery_commander"] = 
            {
                ["neutrals"] = 0,
                ["blue"] = 0,
                ["red"] = 0,
            }, -- end of ["artillery_commander"]
            ["instructor"] = 
            {
                ["neutrals"] = 0,
                ["blue"] = 0,
                ["red"] = 0,
            }, -- end of ["instructor"]
            ["observer"] = 
            {
                ["neutrals"] = 0,
                ["blue"] = 0,
                ["red"] = 0,
            }, -- end of ["observer"]
            ["forward_observer"] = 
            {
                ["neutrals"] = 0,
                ["blue"] = 0,
                ["red"] = 0,
            }, -- end of ["forward_observer"]
        }, -- end of ["roles"]
    }, -- end of ["groundControl"]
    ["goals"] = 
    {
        [1] = 
        {
            ["rules"] = 
            {
                [1] = 
                {
                    ["flag"] = 1,
                    ["predicate"] = "c_flag_is_true",
                    ["zone"] = "",
                }, -- end of [1]
            }, -- end of ["rules"]
            ["side"] = "OFFLINE",
            ["score"] = 100,
            ["predicate"] = "score",
            ["comment"] = "",
        }, -- end of [1]
    }, -- end of ["goals"]
    ["weather"] = 
    {
        ["atmosphere_type"] = 0,
        ["groundTurbulence"] = $WEATHERGROUNDTURBULENCE$,
        ["enable_fog"] = $WEATHERFOGENABLED$,
        ["wind"] = 
        {
            ["at8000"] = 
            {
                ["speed"] = $WEATHERWIND3$,
                ["dir"] = $WEATHERWIND3DIR$,
            }, -- end of ["at8000"]
            ["atGround"] = 
            {
                ["speed"] = $WEATHERWIND1$,
                ["dir"] = $WEATHERWIND1DIR$,
            }, -- end of ["atGround"]
            ["at2000"] = 
            {
                ["speed"] = $WEATHERWIND2$,
                ["dir"] = $WEATHERWIND2DIR$,
            }, -- end of ["at2000"]
        }, -- end of ["wind"]
        ["season"] = 
        {
            ["temperature"] = $WEATHERTEMPERATURE$,
        }, -- end of ["season"]
        ["type_weather"] = 0,
        ["qnh"] = $WEATHERQNH$,
        ["cyclones"] = 
        {
        }, -- end of ["cyclones"]
        ["name"] = "Winter, clean sky",
        ["fog"] = 
        {
            ["thickness"] = $WEATHERFOGTHICKNESS$,
            ["visibility"] = $WEATHERFOGVISIBILITY$,
        }, -- end of ["fog"]
        ["dust_density"] = $WEATHERDUSTDENSITY$,
        ["enable_dust"] = $WEATHERDUSTENABLED$,
        ["visibility"] = 
        {
            ["distance"] = $WEATHERVISIBILITYDISTANCE$,
        }, -- end of ["visibility"]
        ["clouds"] = 
        {
            ["thickness"] = $WEATHERCLOUDSTHICKNESS$,
            ["density"] = $WEATHERCLOUDSDENSITY$,
            ["base"] = $WEATHERCLOUDSBASE$,
            ["iprecptns"] = $WEATHERCLOUDSPRECIPITATION$,
        }, -- end of ["clouds"]
    }, -- end of ["weather"]
    ["theatre"] = "$THEATERID$",
    ["triggers"] = 
    {
        ["zones"] = 
        {
        }, -- end of ["zones"]
    }, -- end of ["triggers"]
    ["map"] = 
    {
        ["centerY"] = $MISSIONAIRBASEY$,
        ["zoom"] = 512000.000,
        ["centerX"] = $MISSIONAIRBASEX$,
    }, -- end of ["map"]
    ["coalitions"] = 
    {
        ["neutrals"] = 
        {
            [1] = 18,
            [2] = 70,
            [3] = 21,
            [4] = 23,
            [5] = 65,
            [6] = 24,
            [7] = 11,
            [8] = 64,
            [9] = 25,
            [10] = 8,
            [11] = 63,
            [12] = 27,
            [13] = 28,
            [14] = 76,
            [15] = 26,
            [16] = 13,
            [17] = 29,
            [18] = 62,
            [19] = 30,
            [20] = 5,
            [21] = 78,
            [22] = 16,
            [23] = 6,
            [24] = 31,
            [25] = 61,
            [26] = 32,
            [27] = 33,
            [28] = 60,
            [29] = 17,
            [30] = 34,
            [31] = 35,
            [32] = 15,
            [33] = 69,
            [34] = 20,
            [35] = 36,
            [36] = 59,
            [37] = 37,
            [38] = 71,
            [39] = 79,
            [40] = 58,
            [41] = 57,
            [42] = 56,
            [43] = 55,
            [44] = 38,
            [45] = 12,
            [46] = 73,
            [47] = 39,
            [48] = 54,
            [49] = 40,
            [50] = 77,
            [51] = 72,
            [52] = 41,
            [53] = 0,
            [54] = 42,
            [55] = 43,
            [56] = 44,
            [57] = 75,
            [58] = 45,
            [59] = 19,
            [60] = 9,
            [61] = 53,
            [62] = 46,
            [63] = 22,
            [64] = 47,
            [65] = 52,
            [66] = 10,
            [67] = 66,
            [68] = 51,
            [69] = 3,
            [70] = 4,
            [71] = 1,
            [72] = 74,
            [73] = 82,
            [74] = 2,
            [75] = 7,
            [76] = 68,
            [77] = 50,
            [78] = 49,
            [79] = 48,
            [80] = 67,
        }, -- end of ["neutrals"]
        ["blue"] = 
        {
            [1] = 80,
        }, -- end of ["blue"]
        ["red"] = 
        {
            [1] = 81,
        }, -- end of ["red"]
    }, -- end of ["coalitions"]
    ["descriptionText"] = "$BRIEFINGDESCRIPTION$",
    ["pictureFileNameR"] = 
    {
    }, -- end of ["pictureFileNameR"]
    ["descriptionNeutralsTask"] = "",
    ["descriptionBlueTask"] = "",
    ["descriptionRedTask"] = "",
    ["pictureFileNameB"] = 
    {
    }, -- end of ["pictureFileNameB"]
    ["coalition"] = 
    {
        ["neutrals"] = 
        {
            ["bullseye"] = 
            {
                ["y"] = 0,
                ["x"] = 0,
            }, -- end of ["bullseye"]
            ["nav_points"] = 
            {
            }, -- end of ["nav_points"]
            ["name"] = "neutrals",
            ["country"] = 
            {
            }, -- end of ["country"]
        }, -- end of ["neutrals"]
        ["blue"] = 
        {
            ["bullseye"] = 
            {
                ["y"] = $BULLSEYEBLUEY$,
                ["x"] = $BULLSEYEBLUEX$,
            }, -- end of ["bullseye"]
            ["nav_points"] = 
            {
            }, -- end of ["nav_points"]
            ["name"] = "blue",
            ["country"] = 
            {
                [1] = 
                {
                    ["id"] = 80,
                    ["name"] = "CJTF Blue",
                    ["helicopter"] = 
                    {
                        ["group"] = 
                        {
$GROUPSHELICOPTERBLUE$
                        }, -- end of ["group"]
                    }, -- end of ["helicopter"]
                    ["plane"] = 
                    {
                        ["group"] = 
                        {
$GROUPSPLANEBLUE$
                        }, -- end of ["group"]
                    }, -- end of ["plane"]
                    ["ship"] = 
                    {
                        ["group"] = 
                        {
$GROUPSSHIPBLUE$
                        }, -- end of ["group"]
                    }, -- end of ["ship"]
                    ["static"] = 
                    {
                        ["group"] = 
                        {
$GROUPSSTATICBLUE$
                        }, -- end of ["group"]
                    }, -- end of ["static"]
                    ["vehicle"] = 
                    {
                        ["group"] = 
                        {
$GROUPSVEHICLEBLUE$
                        }, -- end of ["group"]
                    }, -- end of ["vehicle"]
                }, -- end of [1]
            }, -- end of ["country"]
        }, -- end of ["blue"]
        ["red"] = 
        {
            ["bullseye"] = 
            {
                ["y"] = $BULLSEYEREDY$,
                ["x"] = $BULLSEYEREDX$,
            }, -- end of ["bullseye"]
            ["nav_points"] = 
            {
            }, -- end of ["nav_points"]
            ["name"] = "red",
            ["country"] = 
            {
                [1] = 
                {
                    ["id"] = 81,
                    ["name"] = "CJTF Red",
                    ["helicopter"] = 
                    {
                        ["group"] = 
                        {
$GROUPSHELICOPTERRED$
                        }, -- end of ["group"]
                    }, -- end of ["helicopter"]
                    ["plane"] = 
                    {
                        ["group"] = 
                        {
$GROUPSPLANERED$
                        }, -- end of ["group"]
                    }, -- end of ["plane"]
                    ["ship"] = 
                    {
                        ["group"] = 
                        {
$GROUPSSHIPRED$
                        }, -- end of ["group"]
                    }, -- end of ["ship"]
                    ["static"] = 
                    {
                        ["group"] = 
                        {
$GROUPSSTATICRED$
                        }, -- end of ["group"]
                    }, -- end of ["static"]
                    ["vehicle"] = 
                    {
                        ["group"] = 
                        {
$GROUPSVEHICLERED$
                        }, -- end of ["group"]
                    }, -- end of ["vehicle"]
                }, -- end of [1]
            }, -- end of ["country"]
        }, -- end of ["red"]
    }, -- end of ["coalition"]
    ["sortie"] = "$MISSIONNAME$",
    ["version"] = 18,
    ["trigrules"] = 
    {
        [1] = 
        {
            ["rules"] = 
            {
            }, -- end of ["rules"]
            ["eventlist"] = "",
            ["predicate"] = "triggerStart",
            ["actions"] = 
            {
                [1] = 
                {
                    ["density"] = 1,
                    ["zone"] = "",
                    ["preset"] = 1,
                    ["file"] = "ResKey_Script",
                    ["predicate"] = "a_do_script_file",
                    ["ai_task"] = 
                    {
                        [1] = "",
                        [2] = "",
                    }, -- end of ["ai_task"]
                }, -- end of [1]
            }, -- end of ["actions"]
            ["comment"] = "Run main mission script",
        }, -- end of [1]
    }, -- end of ["trigrules"]
    ["currentKey"] = 0,
    ["start_time"] = $STARTTIME$,
    ["forcedOptions"] = 
    {
    }, -- end of ["forcedOptions"]
    ["failures"] = 
    {
    }, -- end of ["failures"]
} -- end of mission
