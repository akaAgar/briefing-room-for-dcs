mission = 
{
    ["trig"] = 
    {
        ["actions"] = 
        {
        }, -- end of ["actions"]
        ["events"] = 
        {
        },
        ["custom"] = 
        {
        },
        ["func"] = 
        {
        }, -- end of ["func"]
        ["flag"] = 
        {
        }, -- end of ["flag"]
        ["conditions"] = 
        {
        }, -- end of ["conditions"]
        ["customStartup"] = 
        {
        },
        ["funcStartup"] = 
        {
        }, -- end of ["funcStartup"]
    }, -- end of ["trig"]
    ["requiredModules"] = 
    {
    }, -- end of ["requiredModules"]
    ["date"] = 
    {
        ["Day"] = $DATE_DAY$,
        ["Year"] = $DATE_YEAR$,
        ["Month"] = $DATE_MONTH$,
    }, -- end of ["date"]
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
            }, -- end of ["func"]
        }, -- end of ["red"]
    }, -- end of ["result"]
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
            }, -- end of ["forward_observer"]
        }, -- end of ["roles"]
    }, -- end of ["groundControl"]
    ["maxDictId"] = 0,
    ["pictureFileNameN"] = 
    {
    }, -- end of ["pictureFileNameN"]
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
        }, -- end of [1]
    }, -- end of ["goals"]
    ["descriptionNeutralsTask"] = "",
    ["weather"] = 
    {
        ["atmosphere_type"] = 0,
        ["wind"] = 
        {
            ["at8000"] = 
            {
                ["speed"] = $WEATHER_WIND_SPEED3$,
                ["dir"] = $WEATHER_WIND_DIRECTION3$,
            }, -- end of ["at8000"]
            ["at2000"] = 
            {
                ["speed"] = $WEATHER_WIND_SPEED2$,
                ["dir"] = $WEATHER_WIND_DIRECTION2$,
            }, -- end of ["at2000"]
            ["atGround"] = 
            {
                ["speed"] = $WEATHER_WIND_SPEED1$,
                ["dir"] = $WEATHER_WIND_DIRECTION1$,
            }, -- end of ["atGround"]
        }, -- end of ["wind"]
        ["enable_fog"] = false,
        ["groundTurbulence"] = $WEATHER_GROUND_TURBULENCE$,
        ["enable_dust"] = false,
        ["season"] = 
        {
            ["temperature"] = $WEATHER_TEMPERATURE$,
        }, -- end of ["season"]
        ["type_weather"] = 0,
        ["modifiedTime"] = false,
        ["cyclones"] = 
        {
        }, -- end of ["cyclones"]
        ["name"] = "Default",
        ["fog"] = 
        {
            ["thickness"] = 0,
            ["visibility"] = 0,
        }, -- end of ["fog"]
        ["dust_density"] = 0,
        ["qnh"] = $WEATHER_QNH$,
        ["visibility"] = 
        {
            ["distance"] = 80000,
        }, -- end of ["visibility"]
        ["clouds"] = 
        {
            ["thickness"] = 200,
            ["density"] = 0,
            ["preset"] = "$WEATHER_CLOUDS_PRESET$",
            ["base"] = 2500,
            ["iprecptns"] = 0,
        }, -- end of ["clouds"]
    }, -- end of ["weather"]
    ["theatre"] = "$THEATER_ID$",
    ["triggers"] = 
    {
        ["zones"] = 
        {
        },
    },
    ["map"] = 
    {
        ["centerY"] = $MISSION_AIRBASE_Y$,
        ["zoom"] = 512000.000,
        ["centerX"] = $MISSION_AIRBASE_X$,
    }, -- end of ["map"]
    ["coalitions"] = 
    {
        ["neutrals"] = $COALITION_NEUTRAL$,
        ["blue"] = $COALITION_BLUE$,
        ["red"] = $COALITION_RED$
    }, -- end of ["coalitions"]
    ["descriptionText"] = "$BRIEFING_DESCRIPTION$",
    ["pictureFileNameR"] = 
    {
      [1] = "ResKey_TitleImage_$MISSION_ID$",
    }, -- end of ["pictureFileNameR"]
    ["descriptionBlueTask"] = "",
    ["descriptionRedTask"] = "",
    ["pictureFileNameB"] = 
    {
      [1] = "ResKey_TitleImage_$MISSION_ID$",
    }, -- end of ["pictureFileNameB"]
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
                ["y"] = $BULLSEYE_BLUEY$,
                ["x"] = $BULLSEYE_BLUEX$,
            }, -- end of ["bullseye"]
            ["nav_points"] = 
            {
            },
            ["name"] = "blue",
            ["country"] = 
            {
                $COUNTRIES_BLUE$
            }, -- end of ["country"]
        }, -- end of ["blue"]
        ["red"] = 
        {
            ["bullseye"] = 
            {
                ["y"] = $BULLSEYE_RED_Y$,
                ["x"] = $BULLSEYE_RED_X$,
            }, -- end of ["bullseye"]
            ["nav_points"] = 
            {
            },
            ["name"] = "red",
            ["country"] = 
            {
                $COUNTRIES_RED$
            }, -- end of ["country"]
        }, -- end of ["red"]
    }, -- end of ["coalition"]
    ["sortie"] = "$MISSION_NAME$",
    ["version"] = 19,
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
                }, -- end of [1]
            }, -- end of ["actions"]
        }, -- end of [2]
    }, -- end of ["trigrules"]
    ["currentKey"] = 0,
    ["start_time"] = $START_TIME$,
    ["forcedOptions"] = 
    {
        ["unrestrictedSATNAV"] = true,
        ["userMarks"] = true,
$FORCED_OPTIONS$
    }, -- end of ["forcedOptions"]
    ["failures"] = 
    {
    },
}
