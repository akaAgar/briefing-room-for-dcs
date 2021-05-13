  ["lateActivation"] = true,
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
            }, -- end of ["tasks"]
          }, -- end of ["params"]
        }, -- end of ["task"]
        ["type"] = "Turning Point",
        ["ETA"] = 0,
        ["ETA_locked"] = true,
        ["y"] = $Y$,
        ["x"] = $X$,
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
              [1] =
              {
                ["enabled"] = true,
                ["auto"] = false,
                ["id"] = "Orbit",
                ["number"] = 1,
                ["params"] =
                {
                  ["altitude"] = $ALTITUDE$,
                  ["pattern"] = "Circle",
                  ["speed"] = $SPEED$,
                }, -- end of ["params"]
              }, -- end of [1]
            }, -- end of ["tasks"]
          }, -- end of ["params"]
        }, -- end of ["task"]
        ["type"] = "Turning Point",
        ["ETA"] = 0,
        ["ETA_locked"] = false,
        ["y"] = $Y2$,
        ["x"] = $X2$,
        ["name"] = "",
        ["formation_template"] = "",
        ["speed_locked"] = true,
      }, -- end of [2]
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
