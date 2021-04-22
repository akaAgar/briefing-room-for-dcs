[$INDEX$] =
{
  ["visible"] = false,
  ["tasks"] =
  {
  },
  ["uncontrollable"] = false,
  ["task"] = "Ground Nothing",
  ["taskSelected"] = true,
  ["route"] =
  {
    ["points"] =
    {
      [1] =
      {
        ["alt"] = 10,
        ["type"] = "Turning Point",
        ["ETA"] = 0,
        ["alt_type"] = "BARO",
        ["formation_template"] = "",
        ["y"] = $Y$,
        ["x"] = $X$,
        ["name"] = "",
        ["ETA_locked"] = true,
        ["speed"] = 5.5555555555556,
        ["action"] = "On Road",
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
        ["alt"] = 10,
        ["type"] = "Turning Point",
        ["ETA"] = 0.0,
        ["alt_type"] = "BARO",
        ["formation_template"] = "",
        ["y"] = $Y2$,
        ["x"] = $X2$,
        ["name"] = "",
        ["ETA_locked"] = false,
        ["speed"] = 5.5555555555556,
        ["action"] = "On Road",
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
                ["id"] = "GoToWaypoint",
                ["number"] = 1,
                ["params"] =
                {
                  ["fromWaypointIndex"] = 2,
                  ["nWaypointIndx"] = 1,
                },
              },
            },
          },
        },
        ["speed_locked"] = true,
      },
    },
  },
  ["groupId"] = $ID$,
  ["hidden"] = $HIDDEN$,
  ["hiddenOnMFD"] = $HIDDEN$,
  ["units"] =
  {
    $UNITS$
  },
  ["y"] = $Y$,
  ["x"] = $X$,
  ["name"] = "$NAME$",
  ["start_time"] = 0,
},
