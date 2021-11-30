                ["alt"] = $ALTITUDE$,
                ["alt_type"] = "BARO",
                ["livery_id"] = "$LIVERY$",
                ["skill"] = "$SKILL$",
                ["speed"] = 138.88888888889,
$EXTRALUA$
                ["AddPropAircraft"] = 
                {
$PROPSLUA$
                }, -- end of ["AddPropAircraft"]
                ["type"] = "$DCSID$",
                ["unitId"] = $UNITID$,
                ["psi"] = 0,
                ["parking"] = "$PARKINGID$",
                ["y"] = $UNITY$,
                ["x"] = $UNITX$,
                ["name"] = "$NAME$",
                ["payload"] = 
                {
                    ["pylons"] = 
                    {
$PAYLOADPYLONS$
                    }, -- end of ["pylons"]
$PAYLOADCOMMON$
                }, -- end of ["payload"]
                ["heading"] = $HEADING$,
                ["callsign"] = $CALLSIGN$,
                ["onboard_num"] = "$ONBOARDNUMBER$",
                ["Radio"] = 
                {
$RADIOPRESETSLUA$
                }
