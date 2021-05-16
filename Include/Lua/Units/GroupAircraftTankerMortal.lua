    ["lateActivation"] = true,
    ["modulation"] = $RADIOBAND$,
    ["radioSet"] = true,
    ["tasks"] = 
    {
    }, -- end of ["tasks"]
    ["task"] = "Refueling",
    ["uncontrolled"] = false,
    ["taskSelected"] = true,
    ["route"] = 
    {
        ["points"] = 
        {
            [1] = 
            {
                ["alt"] = $ALTITUDE$,
                ["action"] = "Turning Point",
                ["alt_type"] = "BARO",
                ["speed"] = $SPEED$,
                ["task"] = 
                {
                    ["id"] = "ComboTask",
                    ["params"] = 
                    {
                        ["tasks"] = 
                        {
                            [1] = 
                            {
                                ["enabled"] = true,
                                ["auto"] = true,
                                ["id"] = "Tanker",
                                ["number"] = 1,
                                ["params"] = 
                                {
                                }, -- end of ["params"]
                            }, -- end of [1]
                            [2] = 
                            {
                                ["enabled"] = true,
                                ["auto"] = true,
                                ["id"] = "WrappedAction",
                                ["number"] = 2,
                                ["params"] = 
                                {
                                    ["action"] = 
                                    {
                                        ["id"] = "ActivateBeacon",
                                        ["params"] = 
                                        {
                                            ["type"] = 4,
                                            ["AA"] = true,
                                            ["frequency"] = $TACANFREQUENCY$,
                                            ["callsign"] = "$TACANCALLSIGN$",
                                            ["channel"] = $TACANCHANNEL$,
                                            ["unitId"] = $UNITID$,
                                            ["modeChannel"] = "X",
                                            ["bearing"] = true,
                                            ["system"] = 4,
                                        }, -- end of ["params"]
                                    }, -- end of ["action"]
                                }, -- end of ["params"]
                            }, -- end of [2]
                            [3] = 
                            {
                                ["enabled"] = true,
                                ["auto"] = true,
                                ["id"] = "WrappedAction",
                                ["number"] = 3,
                                ["params"] = 
                                {
                                    ["action"] = 
                                    {
                                        ["id"] = "EPLRS",
                                        ["params"] = 
                                        {
                                            ["value"] = $EPLRS$,
                                            ["groupId"] = $GROUPID$,
                                        }, -- end of ["params"]
                                    }, -- end of ["action"]
                                }, -- end of ["params"]
                            }, -- end of [3]
                            [4] = 
                            {
                                ["enabled"] = true,
                                ["auto"] = false,
                                ["id"] = "WrappedAction",
                                ["number"] = 4,
                                ["params"] = 
                                {
                                    ["action"] = 
                                    {
                                        ["id"] = "SetInvisible",
                                        ["params"] = 
                                        {
                                            ["value"] = true,
                                        }, -- end of ["params"]
                                    }, -- end of ["action"]
                                }, -- end of ["params"]
                            }, -- end of [4]
                            [5] = 
                            {
                                ["enabled"] = true,
                                ["auto"] = false,
                                ["id"] = "WrappedAction",
                                ["number"] = 5,
                                ["params"] = 
                                {
                                    ["action"] = 
                                    {
                                        ["id"] = "Option",
                                        ["params"] = 
                                        {
                                            ["value"] = 0,
                                            ["name"] = 1,
                                        }, -- end of ["params"]
                                    }, -- end of ["action"]
                                }, -- end of ["params"]
                            }, -- end of [5]
                            [6] = 
                            {
                                ["enabled"] = true,
                                ["auto"] = false,
                                ["id"] = "Orbit",
                                ["number"] = 6,
                                ["params"] = 
                                {
                                    ["altitude"] = $ALTITUDE$,
                                    ["pattern"] = "Race-Track",
                                    ["speed"] = $SPEED$,
                                }, -- end of ["params"]
                            }, -- end of [6]
                        }, -- end of ["tasks"]
                    }, -- end of ["params"]
                }, -- end of ["task"]
                ["type"] = "Turning Point",
                ["ETA"] = 0,
                ["ETA_locked"] = true,
                ["y"] = $GROUPY$,
                ["x"] = $GROUPX$,
                ["name"] = "",
                ["formation_template"] = "",
                ["speed_locked"] = true,
            }, -- end of [1]
            [2] = 
            {
                ["alt"] = $ALTITUDE$,
                ["action"] = "Turning Point",
                ["alt_type"] = "BARO",
                ["speed"] = $SPEED$,
                ["task"] = 
                {
                    ["id"] = "ComboTask",
                    ["params"] = 
                    {
                        ["tasks"] = 
                        {
                        }, -- end of ["tasks"]
                    }, -- end of ["params"]
                }, -- end of ["task"]
                ["type"] = "Turning Point",
                ["ETA"] = 0.0,
                ["ETA_locked"] = false,
                ["y"] = $GROUPY2$,
                ["x"] = $GROUPX2$,
                ["name"] = "",
                ["formation_template"] = "",
                ["speed_locked"] = true,
            }, -- end of [2]
        }, -- end of ["points"]
    }, -- end of ["route"]
    ["groupId"] = $GROUPID$,
    ["hidden"] = $HIDDEN$,
    ["units"] = 
    {
$UNITS$
    }, -- end of ["units"]
    ["y"] = $GROUPY$,
    ["x"] = $GROUPX$,
    ["name"] = "$NAME$",
    ["communication"] = true,
    ["start_time"] = 0,
    ["frequency"] = $RADIOFREQUENCY$,
