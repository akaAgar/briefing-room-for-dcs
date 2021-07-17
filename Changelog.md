## Changelog

* Version 0.5.107.18 (July 18, 2021)
  * Virtually rewrote everything from scratch, countless bugs fixed
  * Most notable new features
    * Project split between a library (BriefingRoom.dll), a command-line interface and a Razor/WebView GUI
    * New GUI (by John Harvey) allows both quick mission generation (for quick skirmish generation) and full mission generation (for complete control over all aspect of the mission)
    * New objective system with custom selection of task and target type, count and behavior for each objective
    * New objective/mission feature system allows for fine selection of mission elements, from target designation to rules of engagement
    * Proper carrier group generation (by John Harvey)
    * Graphical kneeboard briefing (by John Harvey)
  * Regressions
    * Campaign generator temporarily disabled, will be restored in the next version
* Version 0.4.104.21 (April 21, 2021)
  * All new UI
  * Bug fixes
    * Fixed minor typos (by tgrandgent)
    * Fixed typo in "Soldier M249" unit ID
    * Fixed missing gun in Fix F-5E-3 (by John Harvey)
    * Fixed AWACS altitude (by John Harvey)
    * Removed erroneous settings in Options.lua
    * Improved wind settings, high winds should not be as frequent (by John Harvey)
  * Improvements
    * CAP/SEAD support called by radio (with John Harvey)
    * Changed cruise altitude and cruise speed to integers in aircraft ini files
    * Better weather management (by John Harvey)
  * New features
    * Added "show waypoints as map markers" extensions (by John Harvey)
    * Support for DCS World 2.7 cloud presets (by John Harvey)
    * New "aircraft properties" field in unit properties, radio presets managements (by John Harvey)
* Version 0.3.103.05 (March 5, 2021)
  * Bug fixes
    * AFAC and search & destroy targets are now always hidden on map, regardless of "show enemy units on map" setting
    * Fixed a bug with static units/building not spawned exactly the proper coordinates
    * Fixed "Deep strike" static buldings now only spawn on large spawnpoints (so only on open ground, not in the middle of a field)
    * Fixed JTAC not lasing/marking static objects correctly (by David Pierron)
    * Fixed wrong spawning altitude for aircraft units, which sometimes spawned in the ground (by John Harvey)
  * New template options
    * Civilian traffic level can be selected
    * Distance to objective(s) is now set via an integer value, so very large or very short distances (e.g. for rotary wing operations) can be generated
    * Realism settings (disable external views, enable bird strikes...) can be enforced*
    * Starting airbase and starting carrier selection added to the campaign generator
  * New flyable aircraft
    * A-4E-C Skyhawk (mod) (by John Harvey)
    * F-14A player aircraft (by John Harvey)
    * F-22A Raptor (mod) (by John Harvey)
    * F-86F Sabre
    * Messerschmitt Bf 109K
    * Spitfire (by John Harvey)
  * New mission objectives
    * Battle damage assessment
    * Bomber escort
    * Offensive counter air (with help from John Harvey)
    * Patrol (by John Harvey)
  * Misc. improvements
    * Added keyboard shortcut for campaign generator
    * Aircraft carriers now sail into the wind (by John Harvey)
    * Crash site (with fire and smoke) now spawned near the stranded pilots in CSAR missions
    * Default settings are now set in a "Default.brt" mission template files stored in program directory (by John Harvey)
    * Embedded air defense is now spawned near static buildings, not only near vehicles
    * Now supports custom Lua for unit-specific settings (laser code for the F-16, NVG/HMD selection, etc)
    * GPS is now available to all coalitions, so there's no need to artificially add the USA to the player's coalition to give them access to the GPS
    * Mission features can now be used to spawn unit groups, like objectives
    * Multiple carriers can now be spawned (by John Harvey)
    * Different payloads for each decade can now be specified for aircraft in the database. The payloads themselves have not been added yet. (by John Harvey)
    * Updated Moroccan units (by Nomadzy)
* Version 0.3.101.20 (January 20, 2021)
  * Bug fixes
    * Now assigns the USA to the player's coalition because DCS World restricts access to the GPS to coalitions including the US (thanks to John Harvey for noticing)
    * Fixed parking spawn coordinates (fix by John Harvey)
    * SAM sites prefabs rotation now works correctly (code by John Harvey)
    * Static/buildings now spawned properly
    * Tanker TACAN now works properly, with remark in the briefing FG list (code by John Harvey)
    * Fixed single player AI CAP escorts going back to base on mission start: they now fly straight to the mission area on player takoff like their multiplayer counterparts
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
    * Enemy AI skill can now be selected (uses code by John Harvey)
    * Friendly air defense (code by John Harvey)
    * Carrier groups and carrier takeoff/landing (EARLY VERSION) (code by John Harvey)
    * Now generates custom "title" pictures to campaigns and missions
    * Option to end the mission when the last player has landed or a few minutes after objectives have been completed (code by John Harvey)
    * Option to generate additional ingress/egress waypoints (uses some code by John Harvey)
    * "Random" setting for enemy CAP/SAMs (code by John Harvey)
  * Misc. changes
    * Merged all options (extra waypoints, hide enemy units, etc) into a single list for better readability of the mission template editor
    * Improved briefing descriptions
    * Mission template file extension changed from .brtemplate to .brt
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
