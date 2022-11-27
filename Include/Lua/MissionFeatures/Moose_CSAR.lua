   -- Instantiate and start a CSAR for the blue side, with template "Downed Pilot" and alias "Luftrettung"
   local my_csar = CSAR:New(briefingRoom.playerCoalition,"Downed Pilot","Downed Pilot")
   -- options
   my_csar.immortalcrew = true -- downed pilot spawn is immortal
   my_csar.invisiblecrew = false -- downed pilot spawn is visible
   -- start the FSM
   my_csar:__Start(5)