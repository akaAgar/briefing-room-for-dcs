  ["lateActivation"] = true,
  ["tasks"] =
  {
  }, -- end of ["tasks"]
  ["task"] = "SEAD",
  ["uncontrolled"] = false,
  ["taskSelected"] = true,
  ["route"] =
  {
    ["routeRelativeTOT"] = true,
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
        ["ETA_locked"] = true,
        ["y"] = $GROUPY$,
        ["x"] = $GROUPX$,
        ["name"] = "",
        ["formation_template"] = "",
        ["speed_locked"] = true,
      }, -- end of [1]
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
  ["modulation"] = $RADIOBAND$,
  ["frequency"] = $RADIOFREQUENCY$,
