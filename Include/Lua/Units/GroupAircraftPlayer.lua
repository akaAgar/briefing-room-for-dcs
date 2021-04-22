[$INDEX$] = 
    {
        ["modulation"] = $RADIOBAND$,
        ["tasks"] = 
        {
        },
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
                    ["alt_type"] = "BARO",
                    ["properties"] = 
                    {
                        ["vnav"] = 1,
                        ["scale"] = 0,
                        ["vangle"] = 0,
                        ["angle"] = 0,
                        ["steer"] = 2,
                    },
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
                                            },
                                        },
                                    },
                                },
                            },
                        },
                    },
                    ["type"] = "$PLAYERSTARTINGTYPE$",
                    ["ETA"] = 0,
                    ["ETA_locked"] = true,
                    ["y"] = $Y$,
                    ["x"] = $X$,
                    ["name"] = "$INITIALWPNAME$",
                    ["formation_template"] = "",
                    ["airdromeId"] = $MISSIONAIRBASEID$,
                    ["speed_locked"] = true,
                },
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
                            },
                        },
                    },
                    ["type"] = "Land",
                    ["ETA"] = 0,
                    ["ETA_locked"] = false,
                    ["y"] = $MISSIONAIRBASEY$,
                    ["x"] = $MISSIONAIRBASEX$,
                    ["name"] = "$FINALWPNAME$",
                    ["formation_template"] = "",
                    ["airdromeId"] = $MISSIONAIRBASEID$,
                    ["speed_locked"] = true,
                }, -- end of [$LASTPLAYERWAYPOINTINDEX$]
            },
        },
        ["groupId"] = $ID$,
        ["hidden"] = $HIDDEN$,
        ["units"] = 
        {
$UNITS$
        },
        ["y"] = $Y$,
        ["x"] = $X$,
        ["name"] = "$NAME$",
        ["communication"] = true,
        ["start_time"] = 0,
        ["frequency"] = $RADIOFREQUENCY$,
    },
