[$INDEX$] = 
{
  ["lateActivation"] = true,
  ["tasks"] =
  {
  }, -- end of ["tasks"]
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
                  }, -- end of ["tasks"]
              }, -- end of ["params"]
          }, -- end of ["task"]
          ["type"] = "Turning Point",
          ["ETA"] = 0,
          ["ETA_locked"] = true,
          ["y"] = $Y$,
          ["x"] = $X$,
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
                          ["id"] = "Bombing",
                          ["number"] = 1,
                          ["params"] = 
                          {
                              ["direction"] = 0,
                              ["attackQtyLimit"] = false,
                              ["attackQty"] = 1,
                              ["expend"] = "All",
                              ["y"] = $Y2$,
                              ["directionEnabled"] = false,
                              ["groupAttack"] = true,
                              ["altitude"] =  $ALTITUDE$,
                              ["altitudeEnabled"] = false,
                              ["weaponType"] = 2147485694,
                              ["x"] = $X2$,
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
          ["formation_template"] = "",
          ["speed_locked"] = false,
      }, -- end of [2]
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
                  }, -- end of ["tasks"]
              }, -- end of ["params"]
          }, -- end of ["task"]
          ["type"] = "Turning Point",
          ["ETA"] = 0,
          ["ETA_locked"] = true,
          ["y"] = $Y$,
          ["x"] = $X$,
          ["formation_template"] = "",
          ["speed_locked"] = true,
      }, -- end of [3]
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
  ["modulation"] = $RADIOBAND$,
  ["frequency"] = $RADIOFREQUENCY$,
}, -- end of [$INDEX$]
