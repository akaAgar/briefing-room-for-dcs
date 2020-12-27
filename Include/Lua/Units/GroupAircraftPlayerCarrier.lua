[$INDEX$] = 
    {
        ["modulation"] = $RADIOBAND$,
        ["tasks"] = 
        {
        }, -- end of ["tasks"]
        ["task"] = "CAS",
        ["uncontrolled"] = false,
        ["route"] = 
        {
            ["points"] = 
            {
                [1] = 
                {
                    ["alt"] = 13,
                    ["action"] = "$PLAYERSTARTINGACTION$",
                    ["linkUnit"] = $LINKUNIT$,
                    ["helipadId"] = 1,
                    ["alt_type"] = "BARO",
                    ["properties"] = 
                    {
                        ["vnav"] = 1,
                        ["scale"] = 0,
                        ["vangle"] = 0,
                        ["angle"] = 0,
                        ["steer"] = 2,
                    }, -- end of ["properties"]
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
                                    ["id"] = "WrappedAction",
                                    ["number"] = 1,
                                    ["params"] = 
                                    {
                                        ["action"] = 
                                        {
                                            ["id"] = "EPLRS",
                                            ["params"] = 
                                            {
                                                ["value"] = $EPLRS$,
                                                ["groupId"] = $ID$,
                                            }, -- end of ["params"]
                                        }, -- end of ["action"]
                                    }, -- end of ["params"]
                                }, -- end of [1]
                            }, -- end of ["tasks"]
                        }, -- end of ["params"]
                    }, -- end of ["task"]
                    ["type"] = "$PLAYERSTARTINGTYPE$",
                    ["ETA"] = 0,
                    ["ETA_locked"] = true,
                    ["y"] = $CARRIERBASEY$,
                    ["x"] = $CARRIERBASEX$,
                    ["name"] = "$INITIALWPNAME$",
                    ["formation_template"] = "",
                    ["speed_locked"] = true,
                }, -- end of [1]
$PLAYERWAYPOINTS$
                [$LASTPLAYERWAYPOINTINDEX$] = 
                {
                    ["alt"] = 13,
                    ["action"] = "Landing",
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
                    ["type"] = "Land",
                    ["ETA"] = 0,
                    ["ETA_locked"] = false,
                    ["y"] = $CARRIERBASEY$,
                    ["x"] = $CARRIERBASEY$,
                    ["name"] = "$FINALWPNAME$",
                    ["formation_template"] = "",
                    ["speed_locked"] = true,
                }, -- end of [$LASTPLAYERWAYPOINTINDEX$]
            }, -- end of ["points"]
        }, -- end of ["route"]
        ["groupId"] = $ID$,
        ["hidden"] = $HIDDEN$,
        ["units"] = 
        {
$UNITS$
        }, -- end of ["units"]
        ["y"] = $CARRIERBASEY$,
        ["x"] = $CARRIERBASEY$,
        ["name"] = "$NAME$",
        ["communication"] = true,
        ["start_time"] = 0,
        ["frequency"] = $RADIOFREQUENCY$,
    }, -- end of [$INDEX$]
