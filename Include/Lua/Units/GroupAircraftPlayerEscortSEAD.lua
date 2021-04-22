[$INDEX$] = 
{
    ["modulation"] = $RADIOBAND$,
    ["tasks"] = 
    {
    }, -- end of ["tasks"]
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
                                    }, -- end of ["targetTypes"]
                                    ["priority"] = 0,
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
                                        ["id"] = "Option",
                                        ["params"] = 
                                        {
                                            ["value"] = 4,
                                            ["name"] = 1,
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
                                            ["groupId"] = $ID$,
                                        }, -- end of ["params"]
                                    }, -- end of ["action"]
                                }, -- end of ["params"]
                            }, -- end of [3]
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
                                    }, -- end of ["targetTypes"]
                                    ["lastWptIndexFlagChangedManually"] = true,
                                    ["value"] = "SAM related;",
                                    ["lastWptIndexFlag"] = false,
                                    ["noTargetTypes"] = 
                                    {
                                        [1] = "AAA",
                                    }, -- end of ["noTargetTypes"]
                                    ["pos"] = 
                                    {
                                        ["y"] = 0,
                                        ["x"] = -500,
                                        ["z"] = 200,
                                    }, -- end of ["pos"]
                                }, -- end of ["params"]
                            }, -- end of [4]
                        }, -- end of ["tasks"]
                    }, -- end of ["params"]
                }, -- end of ["task"]
                ["type"] = "$PLAYERESCORTSTARTINGTYPE$",
                ["ETA"] = 0,
                ["ETA_locked"] = true,
                ["y"] = $Y$,
                ["x"] = $X$,
                ["name"] = "",
                ["formation_template"] = "",
                ["airdromeId"] = $MISSIONAIRBASEID$,
                ["speed_locked"] = true,
            }, -- end of [1]
        }, -- end of ["points"]
    }, -- end of ["route"]
    ["groupId"] = $ID$,
    ["hidden"] = $HIDDEN$,
    ["units"] = 
    {
$UNITS$
    }, -- end of ["units"]
    ["y"] = $Y$,
    ["x"] = $X$,
    ["name"] = "$NAME$",
    ["communication"] = true,
    ["start_time"] = 0,
    ["frequency"] = $RADIOFREQUENCY$,
}, -- end of [$INDEX$]
