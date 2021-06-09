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
          ["y"] = $GROUPY$,
          ["x"] = $GROUPX$,
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
                              ["attackQty"] = 4,
                              ["expend"] = "Quarter",
                              ["y"] = $GROUPY2$,
                              ["directionEnabled"] = false,
                              ["groupAttack"] = true,
                              ["altitude"] =  $ALTITUDE$,
                              ["altitudeEnabled"] = false,
                              ["weaponType"] = 2147485694,
                              ["x"] = $GROUPX2$,
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
          ["y"] = $GROUPY$,
          ["x"] = $GROUPX$,
          ["formation_template"] = "",
          ["speed_locked"] = true,
      }, -- end of [3]
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
