## Changelog

* Version 0.3.101.20 (January 20, 2021)
  * Bug fixes
    * Now assigns the USA to the player's coalition because DCS World restricts access to the GPS to coalitions including the US (thanks to John Harvey for noticing)
    * Fixed parking spawn coordinates (fix by John Harvey)
    * SAM sites prefabs rotation now works correctly (code by John Harvey)
    * Static/buildings now spawned properly
    * Tanker TACAN now works properly, with remark in the briefing FG list (code by John Harvey)
  * New units
    * Added many missing aircraft and ground units
    * All player-controllable aircraft are now available (except for user mods)
    * WW2 assets pack units are available
  * New theaters
    * The Channel (EARLY VERSION)
    * Nevada
    * Normandy
    * Syria (EARLY VERSION) (created by John Harvey)
  * New objectives
    * CAS and convoy escort (by John Harvey)
    * Deep strike on enemy structures (by John Harvey)
  * New coalitions/countries: Algeria, Argentina, Belgium, Canada, China, EU, France, Germany, Imperial Japan, India, Insurgents, Iran, Iraq, Israel, Italy, Japan, Libya, Mexico, NATO, Nazi Germany, Norway, Pakistan, Spain, Sweden, Switzerland, Syria, Terrorists, Tunisia, Turkey, UK, Vietnam, WW2 Allies, WW2 Axis
  * New features
    * Campaign generator (EARLY VERSION)
    * All new coalitions/units pairing system (list of operators in each unit definition instead of a list of units for each coalition)
    * Selection of mission decade/era is now independent from coalition, proper units are chosen for the year during which the mission takes place
    * Friendly air defense (code by John Harvey)
    * Carrier groups and carrier takeoff/landing (EARLY VERSION) (code by John Harvey)
    * Now generates custom "title" pictures to campaigns and missions
    * Option to end the mission when the last player has landed or a few minutes after objectives have been completed (code by John Harvey)
    * Option to generate additional ingress/egress waypoints (uses some code by John Harvey)
    * "Random" setting for enemy CAP/SAMs (code by John Harvey)
  * Misc. changes
    * Merged all options (extra waypoints, hide enemy units, etc) into a single list for better readability of the mission template editor
    * Improved briefing descriptions
* Version 0.3.012.28 (December 28, 2020)
  * Bug fixes
    * Fixed DCS crashing when a Ka-27 was spawned
    * Fixed F-14B spawning an F-14A
    * Fixed antiship payload of F/A-18 registered as a SEAD payload (fix by John Harvey)
    * Fixed bug with Maverick F in F/A-18 default loadout (fix by John Harvey)
    * Waypoints for ground targets are now spawned on the ground
    * Starting airbase now picked correctly when red/blue countries are inverted on the map
  * New player aircraft: all SA342 Gazelles, F-5E, F-5E-3
  * New objectives
    * CSAR: locate downed pilots (who can shoot distress flares to help you locate them) and land near them in a helicopter to pick them up
    * FAC(A): direct artillery strikes on target
    * Recon: locate enemy vehicles and fly over them to gather information
  * Improved GUI
    * Better toolbar buttons
    * App now scales better at higher DPI (code by Taosenai)
    * "Miz export complete" (with time taken for the export) now displayed in status bar
  * New mission template settings
    * CAP/SEAD AI escorts now available both in singleplayer and in multiplayer
    * Option to disable radio voices (uses code by John Harvey)
    * "Enemy units location": new option to make sure enemy units and objectives are only spawned in red or blue countries for increased realism
    * AI level selection for friendly aircraft, enemy ground units and enemy aircraft
  * New factions
    * Russia/USSR from the 1970s to the 1990s
  * JTAC can now transmit target coordinates (LL and MGRS) to the players
  * Decreased distance between waypoints and targets on most objectives
  * Improved home airbase selection algorithm
  * Default values for the mission templates now stored in Database/Defaults.ini
  * Project now targets .NET Framework 4.8 instead of 4.5
* Version 0.3.012.19 (December 19, 2020)
  * Added Persian Gulf theater
* Version 0.3.012.17 (December 17, 2020)
  * First public release
