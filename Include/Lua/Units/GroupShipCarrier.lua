  ["visible"] = false,
  ["tasks"] =
  {
  }, -- end of ["tasks"]
  ["uncontrollable"] = false,
  ["route"] =
  {
    ["points"] =
    {
      [1] =
      {
        ["alt"] = -0,
        ["type"] = "Turning Point",
        ["ETA"] = 0,
        ["alt_type"] = "BARO",
        ["formation_template"] = "",
        ["y"] = $GROUPY$,
        ["x"] = $GROUPX$,
        ["name"] = "",
        ["ETA_locked"] = true,
        ["speed"] = $SPEED$,
        ["action"] = "Turning Point",
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
                            ["id"] = "ActivateBeacon",
                            ["params"] = 
                            {
                                ["type"] = 4,
                                ["AA"] = false,
                                ["unitId"] = $UNITID$,
                                ["modeChannel"] = "$TACANMODE$",
                                ["channel"] = $TACANCHANNEL$,
                                ["system"] = 3,
                                ["callsign"] = "$TACANCALLSIGN$",
                                ["bearing"] = true,
                                ["frequency"] = $TACANFREQUENCY$,
                            }, -- end of ["params"]
                        }, -- end of ["action"]
                    }, -- end of ["params"]
                }, -- end of [1]
                [2] = 
                {
                    ["enabled"] = true,
                    ["auto"] = false,
                    ["id"] = "WrappedAction",
                    ["number"] = 2,
                    ["params"] = 
                    {
                        ["action"] = 
                        {
                            ["id"] = "ActivateICLS",
                            ["params"] = 
                            {
                                ["type"] = 131584,
                                ["channel"] = $ILS$,
                                ["unitId"] = $UNITID$,
                            }, -- end of ["params"]
                        }, -- end of ["action"]
                    }, -- end of ["params"]
                }, -- end of [2]
            }, -- end of ["tasks"]
          }, -- end of ["params"]
        }, -- end of ["task"]
        ["speed_locked"] = true,
      }, -- end of [1]
      [2] = 
      {
          ["alt"] = -0,
          ["type"] = "Turning Point",
          ["speed"] = $SPEED$,
          ["alt_type"] = "BARO",
          ["formation_template"] = "",
          ["y"] = $GROUPY2$,
          ["x"] = $GROUPX2$,
          ["ETA_locked"] = false,
          ["action"] = "Turning Point",
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
  ["start_time"] = 0,
