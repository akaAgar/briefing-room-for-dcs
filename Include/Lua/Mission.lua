mission = 
{
    ["requiredModules"] = 
    {
    },
    ["date"] = 
    {
        ["Day"] = $DATEDAY$,
        ["Year"] = $DATEYEAR$,
        ["Month"] = $DATEMONTH$,
    },
    ["trig"] = 
    {
        ["actions"] = 
        {
            [1] = "a_do_script_file(getValueResourceByKey(\"ResKey_Script\"));",
            [2] = "a_end_mission(\"$PLAYERCOALITION$\", \"\", 0); mission.trig.func[2]=nil;",
        },
        ["events"] = 
        {
        },
        ["custom"] = 
        {
        },
        ["func"] = 
        {
            [2] = "if mission.trig.conditions[2]() then mission.trig.actions[2]() end",
        },
        ["flag"] = 
        {
            [1] = true,
            [2] = true,
        },
        ["conditions"] = 
        {
            [1] = "return(true)",
            [2] = "return(c_flag_is_true(2) )",
        },
        ["customStartup"] = 
        {
        },
        ["funcStartup"] = 
        {
            [1] = "if mission.trig.conditions[1]() then mission.trig.actions[1]() end",
        },
    },
    ["result"] = 
    {
        ["offline"] = 
        {
            ["conditions"] = 
            {
                [1] = "return(c_flag_is_true(1))",
            },
            ["actions"] = 
            {
                [1] = "a_set_mission_result(100)",
            },
            ["func"] = 
            {
                [1] = "if mission.result.offline.conditions[1]() then mission.result.offline.actions[1]() end",
            },
        },
        ["total"] = 1,
        ["blue"] = 
        {
            ["conditions"] = 
            {
            },
            ["actions"] = 
            {
            },
            ["func"] = 
            {
            },
        },
        ["red"] = 
        {
            ["conditions"] = 
            {
            },
            ["actions"] = 
            {
            },
            ["func"] = 
            {
            },
        },
    },
    ["maxDictId"] = 0,
    ["pictureFileNameN"] = 
    {
      [1] = "ResKey_TitleImage_$MISSIONID$",
    },
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
            },
            ["instructor"] = 
            {
                ["neutrals"] = 0,
                ["blue"] = 0,
                ["red"] = 0,
            },
            ["observer"] = 
            {
                ["neutrals"] = 0,
                ["blue"] = 0,
                ["red"] = 0,
            },
            ["forward_observer"] = 
            {
                ["neutrals"] = 0,
                ["blue"] = 0,
                ["red"] = 0,
            },
        },
    },
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
                },
            },
            ["side"] = "OFFLINE",
            ["score"] = 100,
            ["predicate"] = "score",
            ["comment"] = "",
        },
    },
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
            },
            ["atGround"] = 
            {
                ["speed"] = $WEATHERWIND1$,
                ["dir"] = $WEATHERWIND1DIR$,
            },
            ["at2000"] = 
            {
                ["speed"] = $WEATHERWIND2$,
                ["dir"] = $WEATHERWIND2DIR$,
            },
        },
        ["season"] = 
        {
            ["temperature"] = $WEATHERTEMPERATURE$,
        },
        ["type_weather"] = 0,
        ["qnh"] = $WEATHERQNH$,
        ["cyclones"] = 
        {
        },
        ["name"] = "Winter, clean sky",
        ["fog"] = 
        {
            ["thickness"] = $WEATHERFOGTHICKNESS$,
            ["visibility"] = $WEATHERFOGVISIBILITY$,
        },
        ["dust_density"] = $WEATHERDUSTDENSITY$,
        ["enable_dust"] = $WEATHERDUSTENABLED$,
        ["visibility"] = 
        {
            ["distance"] = $WEATHERVISIBILITYDISTANCE$,
        },
        ["clouds"] = 
        {
            ["thickness"] = $WEATHERCLOUDSTHICKNESS$,
            ["density"] = $WEATHERCLOUDSDENSITY$,
            ["preset"] = "$WEATHERCLOUDSPRESET$",
            ["base"] = $WEATHERCLOUDSBASE$,
            ["iprecptns"] = $WEATHERCLOUDSPRECIPITATION$,
        },
    },
    ["theatre"] = "$THEATERID$",
    ["triggers"] = 
    {
        ["zones"] = 
        {
        },
    },
    ["map"] = 
    {
        ["centerY"] = $MISSIONAIRBASEY$,
        ["zoom"] = 512000.000,
        ["centerX"] = $MISSIONAIRBASEX$,
    },
    ["coalitions"] = 
    {
        ["neutrals"] = $NEUTRALS$,
        ["blue"] = $BLUES$,
        ["red"] = $REDS$,
    },
    ["descriptionText"] = "$BRIEFINGDESCRIPTION$",
    ["pictureFileNameR"] = 
    {
      [1] = "ResKey_TitleImage_$MISSIONID$",
    },
    ["descriptionNeutralsTask"] = "",
    ["descriptionBlueTask"] = "",
    ["descriptionRedTask"] = "",
    ["pictureFileNameB"] = 
    {
      [1] = "ResKey_TitleImage_$MISSIONID$",
    },
    ["coalition"] = 
    {
        ["neutrals"] = 
        {
            ["bullseye"] = 
            {
                ["y"] = 0,
                ["x"] = 0,
            },
            ["nav_points"] = 
            {
            },
            ["name"] = "neutrals",
            ["country"] = 
            {
            },
        },
        ["blue"] = 
        {
            ["bullseye"] = 
            {
                ["y"] = $BULLSEYEBLUEY$,
                ["x"] = $BULLSEYEBLUEX$,
            },
            ["nav_points"] = 
            {
            },
            ["name"] = "blue",
            ["country"] = 
            {
                $COUNTRYSBLUE$
            },
        },
        ["red"] = 
        {
            ["bullseye"] = 
            {
                ["y"] = $BULLSEYEREDY$,
                ["x"] = $BULLSEYEREDX$,
            },
            ["nav_points"] = 
            {
            },
            ["name"] = "red",
            ["country"] = 
            {
                $COUNTRYSRED$
            },
        },
    },
    ["sortie"] = "$MISSIONNAME$",
    ["version"] = 18,
    ["trigrules"] = 
    {
        [1] = 
        {
            ["rules"] = 
            {
            },
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
                    },
                },
            },
            ["comment"] = "Run main mission script",
        },
        [2] = 
        {
            ["rules"] = 
            {
                [1] = 
                {
                    ["flag"] = 2,
                    ["predicate"] = "c_flag_is_true",
                    ["zone"] = "",
                },
            },
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
                },
            },
        },        
    },
    ["currentKey"] = 0,
    ["start_time"] = $STARTTIME$,
    ["forcedOptions"] = 
    {
        ["unrestrictedSATNAV"] = true,
        ["userMarks"] = true,
$FORCEDOPTIONS$
    },
    ["failures"] = 
    {
    },
}
