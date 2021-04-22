[$INDEX$] = 
{
  ["lateActivation"] = true,
  ["tasks"] =
  {
  },
  ["task"] = "Ground Attack",
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
                      [1] = 
                      {
                          ["enabled"] = true,
                          ["auto"] = false,
                          ["id"] = "Bombing",
                          ["number"] = 1,
                          ["params"] = 
                          {
                              ["direction"] = 0,
                              ["attackQtyLimit"] = false,
                              ["attackQty"] = 4,
                              ["expend"] = "Quarter",
                              ["y"] = $Y2$,
                              ["directionEnabled"] = false,
                              ["groupAttack"] = true,
                              ["altitude"] =  $ALTITUDE$,
                              ["altitudeEnabled"] = false,
                              ["weaponType"] = 2147485694,
                              ["x"] = $X2$,
                          },
                      },
                  },
              },
          },
          ["type"] = "Turning Point",
          ["ETA"] = 0,
          ["ETA_locked"] = false,
          ["y"] = $Y2$,
          ["x"] = $X2$,
          ["formation_template"] = "",
          ["speed_locked"] = false,
      },
      [3] = 
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
