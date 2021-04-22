[$INDEX$] =
{
  ["visible"] = false,
  ["tasks"] =
  {
  },
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
        ["y"] = $Y$,
        ["x"] = $X$,
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
                                ["modeChannel"] = "X",
                                ["channel"] = $TACANCHANNEL$,
                                ["system"] = 3,
                                ["callsign"] = "$TACANCALLSIGN$",
                                ["bearing"] = true,
                                ["frequency"] = $TACANFREQUENCY$,
                            },
                        },
                    },
                },
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
                            },
                        },
                    },
                },
            },
          },
        },
        ["speed_locked"] = true,
      },
      [2] = 
      {
          ["alt"] = -0,
          ["type"] = "Turning Point",
          ["speed"] = $SPEED$,
          ["alt_type"] = "BARO",
          ["formation_template"] = "",
          ["y"] = $Y2$,
          ["x"] = $X2$,
          ["ETA_locked"] = false,
          ["action"] = "Turning Point",
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
  ["start_time"] = 0,
},
