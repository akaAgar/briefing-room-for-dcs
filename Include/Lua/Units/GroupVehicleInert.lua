[$INDEX$] =
{
  ["visible"] = false,
  ["lateActivation"] = false,
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
                    },
                  },
                },
              },
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
                    },
                  },
                },
              },
              [3] =
              {
                ["enabled"] = true,
                ["auto"] = false,
                ["id"] = "Hold",
                ["number"] = 3,
                ["params"] =
                {
                  ["templateId"] = "",
                },
              },
            },
          },
        },
        ["speed_locked"] = true,
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
