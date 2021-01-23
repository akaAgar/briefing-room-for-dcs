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
            [2] = "a_end_mission(\"$PLAYERCOALITION$\", \"\", 0); mission.trig.func[2]=nil;",
        }, -- end of ["actions"]
        ["events"] = 
        {
        }, -- end of ["events"]
        ["custom"] = 
        {
        }, -- end of ["custom"]
        ["func"] = 
        {
            [2] = "if mission.trig.conditions[2]() then mission.trig.actions[2]() end",
        }, -- end of ["func"]
        ["flag"] = 
        {
            [1] = true,
            [2] = true,
        }, -- end of ["flag"]
        ["conditions"] = 
        {
            [1] = "return(true)",
            [2] = "return(c_flag_is_true(2) )",
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
      [1] = "ResKey_TitleImage_$MISSIONID$",
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
            [3] = 83,
            [4] = 21,
            [5] = 23,
            [6] = 65,
            [7] = 24,
            [8] = 11,
            [9] = 64,
            [10] = 25,
            [11] = 8,
            [12] = 63,
            [13] = 27,
            [14] = 28,
            [15] = 76,
            [16] = 26,
            [17] = 13,
            [18] = 29,
            [19] = 62,
            [20] = 30,
            [21] = 5,
            [22] = 78,
            [23] = 16,
            [24] = 6,
            [25] = 31,
            [26] = 61,
            [27] = 32,
            [28] = 33,
            [29] = 60,
            [30] = 17,
            [31] = 34,
            [32] = 35,
            [33] = 15,
            [34] = 69,
            [35] = 20,
            [36] = 36,
            [37] = 59,
            [38] = 37,
            [39] = 71,
            [40] = 79,
            [41] = 58,
            [42] = 57,
            [43] = 56,
            [44] = 55,
            [45] = 38,
            [46] = 12,
            [47] = 73,
            [48] = 39,
            [49] = 54,
            [50] = 40,
            [51] = 77,
            [52] = 72,
            [53] = 41,
            [54] = 0,
            [55] = 42,
            [56] = 43,
            [57] = 44,
            [58] = 75,
            [59] = 45,
            [60] = 19,
            [61] = 9,
            [62] = 53,
            [63] = 46,
            [64] = 22,
            [65] = 47,
            [66] = 52,
            [67] = 10,
            [68] = 66,
            [69] = 51,
            [70] = 3,
            [71] = 4,
            [72] = 1,
            [73] = 74,
            [74] = 82,
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
$USACOUNTRYBLUE$
        }, -- end of ["blue"]
        ["red"] = 
        {
            [1] = 81,
$USACOUNTRYRED$
        }, -- end of ["red"]
    }, -- end of ["coalitions"]
    ["descriptionText"] = "$BRIEFINGDESCRIPTION$",
    ["pictureFileNameR"] = 
    {
      [1] = "ResKey_TitleImage_$MISSIONID$",
    }, -- end of ["pictureFileNameR"]
    ["descriptionNeutralsTask"] = "",
    ["descriptionBlueTask"] = "",
    ["descriptionRedTask"] = "",
    ["pictureFileNameB"] = 
    {
      [1] = "ResKey_TitleImage_$MISSIONID$",
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
        [2] = 
        {
            ["rules"] = 
            {
                [1] = 
                {
                    ["flag"] = 2,
                    ["predicate"] = "c_flag_is_true",
                    ["zone"] = "",
                }, -- end of [1]
            }, -- end of ["rules"]
            ["comment"] = "Ends mission when trigger 2 is true",
            ["eventlist"] = "",
            ["predicate"] = "triggerOnce",
            ["actions"] = 
            {
                [1] = 
                {
                    ["text"] = "",
                    ["start_delay"] = 0,
                    ["zone"] = "",
                    ["predicate"] = "a_end_mission",
                    ["winner"] = "blue",
                    ["KeyDict_text"] = "",
                    ["meters"] = 1000,
                }, -- end of [1]
            }, -- end of ["actions"]
        }, -- end of [2]        
    }, -- end of ["trigrules"]
    ["currentKey"] = 0,
    ["start_time"] = $STARTTIME$,
    ["forcedOptions"] = 
    {
        ["unrestrictedSATNAV"] = true,
        ["userMarks"] = true,
    }, -- end of ["forcedOptions"]
    ["failures"] = 
    {
    }, -- end of ["failures"]
} -- end of mission
