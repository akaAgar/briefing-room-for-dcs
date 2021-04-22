[$INDEX$] = 
{
    ["modulation"] = $RADIOBAND$,
    ["tasks"] = 
    {
    },
    ["task"] = "SEAD",
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
                                ["key"] = "SEAD",
                                ["id"] = "EngageTargets",
                                ["number"] = 1,
                                ["auto"] = true,
                                ["params"] = 
                                {
                                    ["targetTypes"] = 
                                    {
                                        [1] = "Air Defence",
                                    },
                                    ["priority"] = 0,
                                },
                            },
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
                                        ["id"] = "Option",
                                        ["params"] = 
                                        {
                                            ["value"] = 4,
                                            ["name"] = 1,
                                        },
                                    },
                                },
                            },
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
                                            ["groupId"] = $ID$,
                                        },
                                    },
                                },
                            },
                            [4] = 
                            {
                                ["enabled"] = true,
                                ["auto"] = false,
                                ["id"] = "Escort",
                                ["number"] = 4,
                                ["params"] = 
                                {
                                    ["groupId"] = $PLAYERGROUPID$,
                                    ["engagementDistMax"] = 59200,
                                    ["targetTypes"] = 
                                    {
                                        [1] = "SAM related",
                                    },
                                    ["lastWptIndexFlagChangedManually"] = true,
                                    ["value"] = "SAM related;",
                                    ["lastWptIndexFlag"] = false,
                                    ["noTargetTypes"] = 
                                    {
                                        [1] = "AAA",
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
