[$INDEX$] = 
{
    ["modulation"] = $RADIOBAND$,
    ["tasks"] = 
    {
    },
    ["task"] = "Escort",
    ["uncontrolled"] = false,
    ["taskSelected"] = true,
    ["route"] = 
    {
        ["points"] = 
        {
            [1] = 
            {
                ["alt"] = $PLAYERESCORTSTARTINGALTITUDE$,
                ["action"] = "$PLAYERESCORTSTARTINGACTION$",
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
                            [2] = 
                            {
                                ["enabled"] = true,
                                ["auto"] = false,
                                ["id"] = "Escort",
                                ["number"] = 2,
                                ["params"] = 
                                {
                                    ["groupId"] = $PLAYERGROUPID$,
                                    ["engagementDistMax"] = 60000,
                                    ["lastWptIndexFlagChangedManually"] = false,
                                    ["lastWptIndex"] = 2,
                                    ["targetTypes"] = 
                                    {
                                        [1] = "Fighters",
                                    },
                                    ["value"] = "Fighters;",
                                    ["lastWptIndexFlag"] = false,
                                    ["noTargetTypes"] = 
                                    {
                                        [1] = "Bombers",
                                        [2] = "Helicopters",
                                    },
                                    ["pos"] = 
                                    {
                                        ["y"] = 0,
                                        ["x"] = -500,
                                        ["z"] = 200,
                                    },
                                },
                            },
                        },
                    },
                },
                ["type"] = "$PLAYERESCORTSTARTINGTYPE$",
                ["ETA"] = 0,
                ["ETA_locked"] = true,
                ["y"] = $Y$,
                ["x"] = $X$,
                ["name"] = "",
                ["formation_template"] = "",
                ["airdromeId"] = $MISSIONAIRBASEID$,
                ["speed_locked"] = true,
            },
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
