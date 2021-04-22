[$INDEX$] = 
{
  ["lateActivation"] = true,
  ["tasks"] =
  {
  },
  ["task"] = "Nothing",
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
                  },
              },
          },
          ["type"] = "Turning Point",
          ["ETA"] = 0,
          ["ETA_locked"] = true,
          ["y"] = $Y$,
          ["x"] = $X$,
          ["formation_template"] = "",
          ["speed_locked"] = true,
      },
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
                  },
              },
          },
          ["type"] = "Turning Point",
          ["ETA"] = 0,
          ["ETA_locked"] = false,
          ["y"] = $Y2$,
          ["x"] = $X2$,
          ["formation_template"] = "",
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
  ["communication"] = true,
  ["start_time"] = 0,
  ["modulation"] = $RADIOBAND$,
  ["frequency"] = $RADIOFREQUENCY$,
},
