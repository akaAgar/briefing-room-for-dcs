  ["visible"] = false,
  ["lateActivation"] = false,
  ["tasks"] =
  {
  }, -- end of ["tasks"]
  ["uncontrollable"] = $NOCM$,
  ["task"] = "Ground Nothing",
  ["hiddenOnMFD"] = $HIDDEN$,
  ["route"] =
  {
    ["spans"] =
    {
    }, -- end of ["spans"]
    ["points"] =
    {
      [1] =
      {
        ["alt"] = 8,
        ["type"] = "Turning Point",
        ["ETA"] = 0,
        ["alt_type"] = "BARO",
        ["formation_template"] = "",
        ["y"] = $GROUPY$,
        ["x"] = $GROUPX$,
        ["name"] = "$NAME$",
        ["ETA_locked"] = true,
        ["speed"] = 5.5555555555556,
        ["action"] = "Off Road",
        ["task"] =
        {
          ["id"] = "ComboTask",
          ["params"] =
          {
            ["tasks"] =
            {
              [1] =
              {
                ["number"] = 1,
                ["auto"] = false,
                ["id"] = "WrappedAction",
                ["enabled"] = true,
                ["params"] =
                {
                  ["action"] =
                  {
                    ["id"] = "Option",
                    ["params"] =
                    {
                      ["value"] = 4,
                      ["name"] = 0,
                    }, -- end of ["params"]
                  }, -- end of ["action"]
                }, -- end of ["params"]
              }, -- end of [1]
              [2] =
              {
                ["number"] = 2,
                ["auto"] = false,
                ["id"] = "WrappedAction",
                ["enabled"] = true,
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
              }, -- end of [2]
              [3] =
              {
                ["number"] = 3,
                ["auto"] = false,
                ["id"] = "WrappedAction",
                ["enabled"] = true,
                ["params"] =
                {
                  ["action"] =
                  {
                    ["id"] = "SetImmortal",
                    ["params"] =
                    {
                      ["value"] = true,
                    }, -- end of ["params"]
                  }, -- end of ["action"]
                }, -- end of ["params"]
              }, -- end of [3]
              [4] =
              {
                ["enabled"] = true,
                ["auto"] = false,
                ["id"] = "Hold",
                ["number"] = 4,
                ["params"] =
                {
                  ["templateId"] = "",
                }, -- end of ["params"]
              }, -- end of [4]
            }, -- end of ["tasks"]
          }, -- end of ["params"]
        }, -- end of ["task"]
        ["speed_locked"] = true,
      }, -- end of [1]
    }, -- end of ["points"]
    ["routeRelativeTOT"] = false,
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
  ["hiddenOnPlanner"] = $HIDDEN$,
