[$INDEX$] =
{
  ["visible"] = false,
  ["lateActivation"] = true,
  ["tasks"] =
  {
  },
  ["uncontrollable"] = false,
  ["task"] = "Ground Nothing",
  ["hiddenOnMFD"] = $HIDDEN$,
  ["route"] =
  {
    ["spans"] =
    {
    },
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
        ["name"] = "",
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
            },
          },
        },
        ["speed_locked"] = true,
      },
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
                  },
              },
          },
          ["type"] = "Turning Point",
          ["ETA"] = 0,
          ["ETA_locked"] = false,
          ["y"] = $Y$,
          ["x"] = $X$,
          ["name"] = "",
          ["formation_template"] = "",
          ["speed_locked"] = true,
          ["action"] = "On Road",
      },
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
          ["action"] = "On Road",
      },
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
          ["action"] = "Off Road",
      },
    },
    ["routeRelativeTOT"] = false,
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
  ["hiddenOnPlanner"] = $HIDDEN$,
},
