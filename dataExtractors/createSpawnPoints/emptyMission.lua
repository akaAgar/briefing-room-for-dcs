mission = 
{
    ["requiredModules"] = 
    {
    }, -- end of ["requiredModules"]
    ["date"] = 
    {
        ["Day"] = 21,
        ["Year"] = 2016,
        ["Month"] = 6,
    }, -- end of ["date"]
    ["result"] = 
    {
        ["offline"] = 
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
        }, -- end of ["offline"]
        ["total"] = 0,
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
    ["maxDictId"] = 5,
    ["trig"] = 
    {
        ["actions"] = 
        {
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
        }, -- end of ["flag"]
        ["conditions"] = 
        {
        }, -- end of ["conditions"]
        ["customStartup"] = 
        {
        }, -- end of ["customStartup"]
        ["funcStartup"] = 
        {
        }, -- end of ["funcStartup"]
    }, -- end of ["trig"]
    ["pictureFileNameN"] = 
    {
    },
    ["drawings"] = 
    {
        ["options"] = 
        {
            ["hiddenOnF10Map"] = 
            {
                ["Observer"] = 
                {
                    ["Neutral"] = false,
                    ["Blue"] = false,
                    ["Red"] = false,
                }, -- end of ["Observer"]
                ["Instructor"] = 
                {
                    ["Neutral"] = false,
                    ["Blue"] = false,
                    ["Red"] = false,
                }, -- end of ["Instructor"]
                ["ForwardObserver"] = 
                {
                    ["Neutral"] = false,
                    ["Blue"] = false,
                    ["Red"] = false,
                }, -- end of ["ForwardObserver"]
                ["Spectrator"] = 
                {
                    ["Neutral"] = false,
                    ["Blue"] = false,
                    ["Red"] = false,
                }, -- end of ["Spectrator"]
                ["ArtilleryCommander"] = 
                {
                    ["Neutral"] = false,
                    ["Blue"] = false,
                    ["Red"] = false,
                }, -- end of ["ArtilleryCommander"]
                ["Pilot"] = 
                {
                    ["Neutral"] = false,
                    ["Blue"] = false,
                    ["Red"] = false,
                }, -- end of ["Pilot"]
            }, -- end of ["hiddenOnF10Map"]
        }, -- end of ["options"]
        ["layers"] = 
        {
            [1] = 
            {
                ["visible"] = true,
                ["name"] = "Red",
                ["objects"] = 
                {
                }, -- end of ["objects"]
            }, -- end of [1]
            [2] = 
            {
                ["visible"] = true,
                ["name"] = "Blue",
                ["objects"] = 
                {
                }, -- end of ["objects"]
            }, -- end of [2]
            [3] = 
            {
                ["visible"] = true,
                ["name"] = "Neutral",
                ["objects"] = 
                {
                }, -- end of ["objects"]
            }, -- end of [3]
            [4] = 
            {
                ["visible"] = true,
                ["name"] = "Common",
                ["objects"] = 
                {
                }, -- end of ["objects"]
            }, -- end of [4]
            [5] = 
            {
                ["visible"] = true,
                ["name"] = "Author",
                ["objects"] = 
                {
                    [1] = 
                    {
                        ["visible"] = true,
                        ["radius"] = 14.8,
                        ["colorString"] = "0xff0000ff",
                        ["mapY"] = $MAPY$,
                        ["primitiveType"] = "Polygon",
                        ["polygonMode"] = "circle",
                        ["style"] = "solid",
                        ["thickness"] = 8,
                        ["mapX"] = $MAPX$,
                        ["layerName"] = "Author",
                        ["name"] = "Small (Red)",
                        ["fillColorString"] = "0xff000080",
                    }, -- end of [1]
                    [2] = 
                    {
                        ["visible"] = true,
                        ["radius"] = 277.5,
                        ["colorString"] = "0x0000ffff",
                        ["mapY"] = $MAPY$,
                        ["primitiveType"] = "Polygon",
                        ["polygonMode"] = "circle",
                        ["style"] = "solid",
                        ["thickness"] = 8,
                        ["mapX"] = $MAPX$,
                        ["layerName"] = "Author",
                        ["name"] = "Large (Blue)",
                        ["fillColorString"] = "0x0000ff80",
                    }, -- end of [2]
                    [3] = 
                    {
                        ["visible"] = true,
                        ["radius"] = 92.5,
                        ["colorString"] = "0x808080ff",
                        ["mapY"] = $MAPY$,
                        ["primitiveType"] = "Polygon",
                        ["polygonMode"] = "circle",
                        ["style"] = "solid",
                        ["thickness"] = 8,
                        ["mapX"] = $MAPX$,
                        ["layerName"] = "Author",
                        ["name"] = "Medium (Neutural)",
                        ["fillColorString"] = "0x80808080",
                    }, -- end of [3]
                }, -- end of ["objects"]
            }, -- end of [5]
        }, -- end of ["layers"]
    }, -- end of ["drawings"] -- end of ["pictureFileNameN"]
    ["groundControl"] = 
    {
        ["passwords"] = 
        {
            ["artillery_commander"] = 
            {
            }, -- end of ["artillery_commander"]
            ["instructor"] = 
            {
            }, -- end of ["instructor"]
            ["observer"] = 
            {
            }, -- end of ["observer"]
            ["forward_observer"] = 
            {
            }, -- end of ["forward_observer"]
        }, -- end of ["passwords"]
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
        ["isPilotControlVehicles"] = false,
    }, -- end of ["groundControl"]
    ["descriptionBlueTask"] = "DictKey_descriptionBlueTask_3",
    ["weather"] = 
    {
        ["wind"] = 
        {
            ["at8000"] = 
            {
                ["speed"] = 0,
                ["dir"] = 0,
            }, -- end of ["at8000"]
            ["atGround"] = 
            {
                ["speed"] = 0,
                ["dir"] = 0,
            }, -- end of ["atGround"]
            ["at2000"] = 
            {
                ["speed"] = 0,
                ["dir"] = 0,
            }, -- end of ["at2000"]
        }, -- end of ["wind"]
        ["enable_fog"] = false,
        ["season"] = 
        {
            ["temperature"] = 20,
        }, -- end of ["season"]
        ["qnh"] = 760,
        ["cyclones"] = 
        {
        }, -- end of ["cyclones"]
        ["dust_density"] = 0,
        ["enable_dust"] = false,
        ["clouds"] = 
        {
            ["thickness"] = 200,
            ["density"] = 0,
            ["preset"] = "Preset2",
            ["base"] = 2500,
            ["iprecptns"] = 0,
        }, -- end of ["clouds"]
        ["atmosphere_type"] = 0,
        ["groundTurbulence"] = 0,
        ["halo"] = 
        {
            ["preset"] = "auto",
        }, -- end of ["halo"]
        ["type_weather"] = 0,
        ["modifiedTime"] = false,
        ["name"] = "Winter, clean sky",
        ["fog"] = 
        {
            ["thickness"] = 0,
            ["visibility"] = 0,
        }, -- end of ["fog"]
        ["visibility"] = 
        {
            ["distance"] = 80000,
        }, -- end of ["visibility"]
    }, -- end of ["weather"]
    ["theatre"] = "$THEATER$",
    ["triggers"] = 
    {
        ["zones"] = 
        {
        }, -- end of ["zones"]
    }, -- end of ["triggers"]
    ["map"] = 
    {
        ["centerY"] = 638709.05049212,
        ["zoom"] = 30144.823662376,
        ["centerX"] = -286944.93926642,
    }, -- end of ["map"]
    ["coalitions"] = 
    {
        ["neutrals"] = 
        {
            [1] = 70,
            [2] = 83,
            [3] = 23,
            [4] = 65,
            [5] = 86,
            [6] = 64,
            [7] = 25,
            [8] = 63,
            [9] = 76,
            [10] = 84,
            [11] = 90,
            [12] = 29,
            [13] = 62,
            [14] = 30,
            [15] = 78,
            [16] = 87,
            [17] = 31,
            [18] = 61,
            [19] = 32,
            [20] = 33,
            [21] = 60,
            [22] = 17,
            [23] = 35,
            [24] = 69,
            [25] = 36,
            [26] = 59,
            [27] = 71,
            [28] = 79,
            [29] = 58,
            [30] = 57,
            [31] = 56,
            [32] = 55,
            [33] = 88,
            [34] = 73,
            [35] = 39,
            [36] = 89,
            [37] = 54,
            [38] = 77,
            [39] = 72,
            [40] = 41,
            [41] = 42,
            [42] = 44,
            [43] = 85,
            [44] = 75,
            [45] = 53,
            [46] = 22,
            [47] = 52,
            [48] = 66,
            [49] = 51,
            [50] = 74,
            [51] = 82,
            [52] = 7,
            [53] = 68,
            [54] = 50,
            [55] = 49,
            [56] = 48,
            [57] = 67,
        }, -- end of ["neutrals"]
        ["blue"] = 
        {
            [1] = 21,
            [2] = 11,
            [3] = 8,
            [4] = 80,
            [5] = 28,
            [6] = 26,
            [7] = 13,
            [8] = 5,
            [9] = 16,
            [10] = 6,
            [11] = 15,
            [12] = 20,
            [13] = 12,
            [14] = 40,
            [15] = 45,
            [16] = 9,
            [17] = 46,
            [18] = 10,
            [19] = 3,
            [20] = 4,
            [21] = 1,
            [22] = 2,
        }, -- end of ["blue"]
        ["red"] = 
        {
            [1] = 18,
            [2] = 24,
            [3] = 27,
            [4] = 81,
            [5] = 34,
            [6] = 37,
            [7] = 38,
            [8] = 0,
            [9] = 43,
            [10] = 19,
            [11] = 47,
        }, -- end of ["red"]
    }, -- end of ["coalitions"]
    ["descriptionText"] = "DictKey_descriptionText_1",
    ["pictureFileNameR"] = 
    {
    }, -- end of ["pictureFileNameR"]
    ["goals"] = 
    {
    }, -- end of ["goals"]
    ["descriptionNeutralsTask"] = "DictKey_descriptionNeutralsTask_4",
    ["descriptionRedTask"] = "DictKey_descriptionRedTask_2",
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
                [1] = 
                {
                    ["id"] = 70,
                    ["vehicle"] = 
                    {
                        ["group"] = 
                        {
                            $LandMedium$
                        }, -- end of ["group"]
                    }, -- end of ["vehicle"]
                    ["name"] = "Algeria",
                }, -- end of [1]
            }, -- end of ["country"]
        }, -- end of ["neutrals"]
        ["blue"] = 
        {
            ["bullseye"] = 
            {
                ["y"] = 617414,
                ["x"] = -291014,
            }, -- end of ["bullseye"]
            ["nav_points"] = 
            {
            }, -- end of ["nav_points"]
            ["name"] = "blue",
            ["country"] = 
            {
                [1] = 
                {
                    ["id"] = 2,
                    ["vehicle"] = 
                    {
                        ["group"] = 
                        {
                            $LandLarge$
                        }, -- end of ["group"]
                    }, -- end of ["vehicle"]
                    ["name"] = "USA",
                }, -- end of [1]
            }, -- end of ["country"]
        }, -- end of ["blue"]
        ["red"] = 
        {
            ["bullseye"] = 
            {
                ["y"] = 371700,
                ["x"] = 11557,
            }, -- end of ["bullseye"]
            ["nav_points"] = 
            {
            }, -- end of ["nav_points"]
            ["name"] = "red",
            ["country"] = 
            {
                [1] = 
                {
                    ["id"] = 0,
                    ["vehicle"] = 
                    {
                        ["group"] = 
                        {
                            $LandSmall$
                        }, -- end of ["group"]
                    }, -- end of ["vehicle"]
                    ["name"] = "Russia",
                }, -- end of [1]
            }, -- end of ["country"]
        }, -- end of ["red"]
    }, -- end of ["coalition"]
    ["sortie"] = "DictKey_sortie_5",
    ["version"] = 22,
    ["trigrules"] = 
    {
    }, -- end of ["trigrules"]
    ["currentKey"] = 156,
    ["failures"] = 
    {
        ["CADC_FAILURE_TAS"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["CADC_FAILURE_TAS"]
        ["sas_pitch_left"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["sas_pitch_left"]
        ["AN_ALR69V_FAILURE_SENSOR_TAIL_LEFT"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["AN_ALR69V_FAILURE_SENSOR_TAIL_LEFT"]
        ["rws"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["rws"]
        ["AN_ALR69V_FAILURE_SENSOR_TAIL_RIGHT"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["AN_ALR69V_FAILURE_SENSOR_TAIL_RIGHT"]
        ["eos"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["eos"]
        ["ecm"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["ecm"]
        ["AAR_47_FAILURE_SENSOR_BOTTOM"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["AAR_47_FAILURE_SENSOR_BOTTOM"]
        ["AAR_47_FAILURE_SENSOR_LEFT"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["AAR_47_FAILURE_SENSOR_LEFT"]
        ["l_gen"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["l_gen"]
        ["hud"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["hud"]
        ["AAR_47_FAILURE_TOTAL"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["AAR_47_FAILURE_TOTAL"]
        ["CADC_FAILURE_TEMPERATURE"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["CADC_FAILURE_TEMPERATURE"]
        ["RADAR_ALTIMETR_LEFT_ANT_FAILURE"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["RADAR_ALTIMETR_LEFT_ANT_FAILURE"]
        ["hydro"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["hydro"]
        ["AN_ALE_40V_FAILURE_TOTAL"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["AN_ALE_40V_FAILURE_TOTAL"]
        ["ILS_FAILURE_ANT_MARKER"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["ILS_FAILURE_ANT_MARKER"]
        ["AN_ALE_40V_FAILURE_CONTAINER_LEFT_WING"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["AN_ALE_40V_FAILURE_CONTAINER_LEFT_WING"]
        ["sas_pitch_right"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["sas_pitch_right"]
        ["AAR_47_FAILURE_SENSOR_RIGHT"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["AAR_47_FAILURE_SENSOR_RIGHT"]
        ["CADC_FAILURE_IAS"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["CADC_FAILURE_IAS"]
        ["autopilot"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["autopilot"]
        ["asc"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["asc"]
        ["AN_ALE_40V_FAILURE_CONTAINER_RIGHT_WING"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["AN_ALE_40V_FAILURE_CONTAINER_RIGHT_WING"]
        ["CADC_FAILURE_BARO_ALT"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["CADC_FAILURE_BARO_ALT"]
        ["CADC_FAILURE_PRESSURE_ALT"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["CADC_FAILURE_PRESSURE_ALT"]
        ["RADAR_ALTIMETR_RIGHT_ANT_FAILURE"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["RADAR_ALTIMETR_RIGHT_ANT_FAILURE"]
        ["TGP_FAILURE_LEFT"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["TGP_FAILURE_LEFT"]
        ["CADC_FAILURE_TOTAL"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["CADC_FAILURE_TOTAL"]
        ["hydro_right"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["hydro_right"]
        ["AN_ALR69V_FAILURE_SENSOR_NOSE_LEFT"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["AN_ALR69V_FAILURE_SENSOR_NOSE_LEFT"]
        ["sas_yaw_right"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["sas_yaw_right"]
        ["SADL_FAILURE_TOTAL"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["SADL_FAILURE_TOTAL"]
        ["AAR_47_FAILURE_SENSOR_TAIL"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["AAR_47_FAILURE_SENSOR_TAIL"]
        ["hydro_left"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["hydro_left"]
        ["TACAN_FAILURE_TRANSMITTER"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["TACAN_FAILURE_TRANSMITTER"]
        ["ILS_FAILURE_TOTAL"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["ILS_FAILURE_TOTAL"]
        ["l_conv"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["l_conv"]
        ["AN_ALR69V_FAILURE_SENSOR_NOSE_RIGHT"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["AN_ALR69V_FAILURE_SENSOR_NOSE_RIGHT"]
        ["TACAN_FAILURE_RECEIVER"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["TACAN_FAILURE_RECEIVER"]
        ["sas_yaw_left"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["sas_yaw_left"]
        ["IFFCC_FAILURE_TOTAL"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["IFFCC_FAILURE_TOTAL"]
        ["CADC_FAILURE_STATIC"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["CADC_FAILURE_STATIC"]
        ["HUD_FAILURE"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["HUD_FAILURE"]
        ["CDU_FAILURE_TOTAL"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["CDU_FAILURE_TOTAL"]
        ["TACAN_FAILURE_TOTAL"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["TACAN_FAILURE_TOTAL"]
        ["mfd"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["mfd"]
        ["VHF_FM_RADIO_FAILURE_TOTAL"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["VHF_FM_RADIO_FAILURE_TOTAL"]
        ["RIGHT_MFCD_FAILURE"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["RIGHT_MFCD_FAILURE"]
        ["r_engine"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["r_engine"]
        ["AN_ALE_40V_FAILURE_CONTAINER_LEFT_GEAR"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["AN_ALE_40V_FAILURE_CONTAINER_LEFT_GEAR"]
        ["CADC_FAILURE_DYNAMIC"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["CADC_FAILURE_DYNAMIC"]
        ["CLOCK_FAILURE"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["CLOCK_FAILURE"]
        ["CICU_FAILURE_TOTAL"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["CICU_FAILURE_TOTAL"]
        ["LEFT_MFCD_FAILURE"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["LEFT_MFCD_FAILURE"]
        ["AN_ALR69V_FAILURE_TOTAL"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["AN_ALR69V_FAILURE_TOTAL"]
        ["r_conv"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["r_conv"]
        ["ILS_FAILURE_ANT_GLIDESLOPE"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["ILS_FAILURE_ANT_GLIDESLOPE"]
        ["EGI_FAILURE_TOTAL"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["EGI_FAILURE_TOTAL"]
        ["l_engine"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["l_engine"]
        ["UHF_RADIO_FAILURE_TOTAL"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["UHF_RADIO_FAILURE_TOTAL"]
        ["TGP_FAILURE_RIGHT"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["TGP_FAILURE_RIGHT"]
        ["AN_ALE_40V_FAILURE_CONTAINER_RIGHT_GEAR"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["AN_ALE_40V_FAILURE_CONTAINER_RIGHT_GEAR"]
        ["CADC_FAILURE_MACH"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["CADC_FAILURE_MACH"]
        ["IFFCC_FAILURE_GUN"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["IFFCC_FAILURE_GUN"]
        ["ILS_FAILURE_ANT_LOCALIZER"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["ILS_FAILURE_ANT_LOCALIZER"]
        ["AIRSPEED_INDICATOR_FAILURE"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["AIRSPEED_INDICATOR_FAILURE"]
        ["r_gen"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["r_gen"]
        ["VHF_AM_RADIO_FAILURE_TOTAL"] = 
        {
            ["hh"] = 0,
            ["prob"] = 100,
            ["enable"] = false,
            ["mmint"] = 1,
            ["mm"] = 0,
        }, -- end of ["VHF_AM_RADIO_FAILURE_TOTAL"]
    }, -- end of ["failures"]
    ["forcedOptions"] = 
    {
    }, -- end of ["forcedOptions"]
    ["start_time"] = 28800,
} -- end of mission
