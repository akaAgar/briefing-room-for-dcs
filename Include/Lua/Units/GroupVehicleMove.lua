[$INDEX$] =
{
  ["visible"] = false,
  ["lateActivation"] = true,
  ["tasks"] =
  {
  }, -- end of ["tasks"]
  ["uncontrollable"] = false,
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
        ["y"] = $Y$,
        ["x"] = $X$,
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
            }, -- end of ["tasks"]
          }, -- end of ["params"]
        }, -- end of ["task"]
        ["speed_locked"] = true,
      }, -- end of [1]
      [2] = 
      {
          ["alt"] = 8,
          ["action"] = "Turning Point",
          ["alt_type"] = "BARO",
          ["speed"] = 5.5555555555556,
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
          ["type"] = "Turning Point",
          ["ETA"] = 0,
          ["ETA_locked"] = false,
          ["y"] = $Y$,
          ["x"] = $X$,
          ["name"] = "",
          ["formation_template"] = "",
          ["speed_locked"] = true,
          ["action"] = "On Road",
      }, -- end of [2]
      [3] = 
      {
          ["alt"] = 8,
          ["action"] = "Turning Point",
          ["alt_type"] = "BARO",
          ["speed"] = 5.5555555555556,
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
          ["type"] = "Turning Point",
          ["ETA"] = 0,
          ["ETA_locked"] = false,
          ["y"] = $Y2$,
          ["x"] = $X2$,
          ["name"] = "",
          ["formation_template"] = "",
          ["speed_locked"] = true,
          ["action"] = "On Road",
      }, -- end of [2]
      [4] = 
      {
          ["alt"] = 8,
          ["action"] = "Turning Point",
          ["alt_type"] = "BARO",
          ["speed"] = 5.5555555555556,
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
          ["type"] = "Turning Point",
          ["ETA"] = 0,
          ["ETA_locked"] = false,
          ["y"] = $Y2$,
          ["x"] = $X2$,
          ["name"] = "",
          ["formation_template"] = "",
          ["speed_locked"] = true,
          ["action"] = "Off Road",
      }, -- end of [2]
    }, -- end of ["points"]
    ["routeRelativeTOT"] = false,
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
  ["start_time"] = 0,
  ["hiddenOnPlanner"] = $HIDDEN$,
}, -- end of [$INDEX$]
