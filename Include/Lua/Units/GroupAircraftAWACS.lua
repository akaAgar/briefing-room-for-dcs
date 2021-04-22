[$INDEX$] = 
{
    ["lateActivation"] = true,
    ["modulation"] = $RADIOBAND$,
    ["tasks"] = 
    {
    },
    ["task"] = "AWACS",
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
                                ["id"] = "AWACS",
                                ["number"] = 1,
                                ["params"] = 
                                {
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
                                        ["id"] = "EPLRS",
                                        ["params"] = 
                                        {
                                            ["value"] = $EPLRS$,
                                            ["groupId"] = $ID$,
                                        },
                                    },
                                },
                            },
                            [3] = 
                            {
                                ["enabled"] = true,
                                ["auto"] = false,
                                ["id"] = "WrappedAction",
                                ["number"] = 3,
                                ["params"] = 
                                {
                                    ["action"] = 
                                    {
                                        ["id"] = "SetInvisible",
                                        ["params"] = 
                                        {
                                            ["value"] = true,
                                        },
                                    },
                                },
                            },
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
                                        ["id"] = "SetImmortal",
                                        ["params"] = 
                                        {
                                            ["value"] = true,
                                        },
                                    },
                                },
                            },
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
                                        },
                                    },
                                },
                            },
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
                                },
                            },
                        },
                    },
                },
                ["type"] = "Turning Point",
                ["ETA"] = 0,
                ["ETA_locked"] = true,
                ["y"] = $Y$,
                ["x"] = $X$,
                ["name"] = "",
                ["formation_template"] = "",
                ["speed_locked"] = true,
            },
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
                        },
                    },
                },
                ["type"] = "Turning Point",
                ["ETA"] = 0,
                ["ETA_locked"] = false,
                ["y"] = $Y2$,
                ["x"] = $X2$,
                ["name"] = "",
                ["formation_template"] = "",
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
