  ["lateActivation"] = false,
  ["modulation"] = $RADIOBAND$,
  ["tasks"] =
  {
  }, -- end of ["tasks"]
  ["radioSet"] = false,
  ["task"] = "CAS",
  ["uncontrolled"] = true,
  ["route"] =
  {
    ["routeRelativeTOT"] = true,
    ["points"] =
    {
      [1] =
      {
        ["alt"] = $ALTITUDE$,
        ["action"] = "From Parking Area Hot",
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
                ["key"] = "CAS",
                ["id"] = "EngageTargets",
                ["number"] = 1,
                ["auto"] = true,
                ["params"] =
                {
                  ["targetTypes"] =
                  {
                    [1] = "ALL",
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
                    ["id"] = "EPLRS",
                    ["params"] =
                    {
                      ["value"] = $EPLRS$,
                      ["groupId"] = $GROUPID$,
                    }, -- end of ["params"]
                  }, -- end of ["action"]
                }, -- end of ["params"]
              }, -- end of [2]
            }, -- end of ["tasks"]
          }, -- end of ["params"]
        }, -- end of ["task"]
        ["type"] = "TakeOffParkingHot",
        ["ETA"] = 0,
        ["ETA_locked"] = true,
        ["y"] = $GROUPY$,
        ["x"] = $GROUPX$,
        ["name"] = "",
        ["formation_template"] = "",
        ["airdromeId"] = $GROUPAIRBASEID$,
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
        ["y"] = $GROUPY2$,
        ["x"] = $GROUPX2$,
        ["name"] = "",
        ["formation_template"] = "",

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
  ["communication"] = true,
  ["start_time"] = 0,
  ["frequency"] = $RADIOFREQUENCY$,
