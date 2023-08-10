## Changelog
* Version 0.5.308.10(Aug, 10, 2023)
    * Added:
        * Added: IADS Command Center Area Mark (Generated on mission start)
        * Added: Block Supplier Logic as mission Option
        * Added: Normandy Dday Situations (Campaign linked)
        * Added: 2 More Sinai Situations
        * Added: Spawn Point View option within situation viewer
        * Added: Mission Feature Fires around objective and home base
        * Added: Many large spawn points to Caucasus map (new total: 8632)
        * Added: Support for JSON based spawn points
        * Added: Stronger Template Validation
        * Added: Missing WW2 Building BR data
        * Added: Missing WW2 Ships BR data
        * Added: Missing WW2 Aircraft BR data
        * Added: FrontLine Config now in FrontLine.ini
        * Added: Missing WWII Armour and Technics module units
        * Added: Front Line bias based on task type
        * Added: Front line positioning logic for optional Buildings and Ground units
        * Added: Front line disable option
        * Added: Support for front line positioning logic
        * Added: Support for SouthernHemisphere maps reversing season selection
        * Added: Windsock to FARPS
        * Added: Air defense knock down logic (If you can't spawn SAMs add the number of SAMs you would of spawned to AAA spawn count)
        * Added: F-15E to core mods list
        * Added: Situation Loading
        * Added: Situation Editor saving
        * Added: Multi Zone support for the Situation Editor
        * Added: Converted Existing Situation zone data to json
        * Added: Support for Multiple Control Zones per side
        * Added: Json Terrain data + Sinai south sea
        * Added: Support for Multiple Water zones
        * Added: F-15ESE
        * Added: Sinai Map support (2 situations, 3k spawn points, more dense about Gaza and Suez Canal)
        * Added: SinaiMap Airfields JSON data
        * Added: Livery and Payload Loading fom local save game (Local run only ovo)
        * Added: Briefing Name and Description overrides
        * Added: Go To Airbase Behavior
        * Added: Active Infantry near objective
        * Added: Populate operational eras by country from old DB
        * Added: Support for country specific operational eras
        * Added: Rafales Mod
        * Added: Flyable F-117A mod
        * Added: F-22A mod
        * Added: Super Hercules mod
        * Added: A-29 Mod support
        * Added: Mod aircraft data F-117, F-22A, Hercules, Rafale, A-29B
        * Added: Yak-52 and Mirage-F1 (all variants)
        * Added: Mod Plane extractor script for dcsfiddle
        * Added: JAS39 Gripen,  A-4E-C, Bronco-OV-10A, Civ aircraft mod support
        * Added: Ship mod support
        * Added: HighDigitSAMs Mod support
        * Added: UH-60L Mod support
        * Added: Mod unit support
        * Added: Enemy CAP objective feature (Escort if target is plane)
        * Added: ShortRangeBattery (Manpads & AAA) air defense type
        * Added: MANPADS and Static AAA templates
        * Added: More Unit Layouts
        * Added: AI tasks on player waypoints
        * Added: Space based parking (Not supported on all maps)
        * Added: assign correct task based on objectives
        * Added: Air Start for player aircraft (marked (A))
        * Added: More layouts
        * Added: Layouts support
        * Added: Use FARP and Carrier templates is available
        * Added: BR templates and module support for templates
        * Added: Internal template selector
        * Added: EWR to standard air defense
        * Added: BRInfo files for Ships
        * Added: BRInfo files for Aircraft
        * Added: Cargo support
        * Added: Proper templates
        * Added: start of Template spawning
        * Added: Group Level payloads and Season and Location aware liveries at group level
        * Added: Player loadouts and fixed lack of fuel setting
        * Added: Operational and Low Poly filtering
        * Added: Make Technically functional
        * Added: UnitShips Data
        * Added: Load Helicopters and Planes Json data
        * Added: Helicopters and Planes Json data
        * Added: Load JSON UnitCars data
        * Added: Player exact position markers
        * Added: FOB Feature TACAN on airbase table
        * Added: Airbase Feature TACAN on airbase table
        * Added: Docker compatible Kneeboard rendering
        * Added: Target Count to Campaign Generator
        * Added: Presets for Full Builder
        * Added: DSMC file prefix when mod selected
    * * Updated: 
        * Updated: Fixes to spawning system
        * Updated: Added 1000 Spawn points to PersianGulf Map - Thanks LawnDart
        * Updated: Added 500 Spawn points to PersianGulf Map - Thanks LawnDart
        * Updated: Added 500 Spawn points to Normandy Map - Thanks Eagle01
        * Updated: F-15 Liveries Options
        * Updated: Added 500 Spawn points to Mariana Islands Map
        * Updated: Added 2k Spawn points to South Atlantic Map
        * Updated: Added 3k Spawn points to Sinai
        * Updated: Added 2k Spawn points to Syria Map
        * Updated: Moose 2.7.21
        * Updated: Database to latest patch
        * Updated: Coalition Operational Eras
        * Updated: Various Situations to have smaller control zones
        * Updated: SituationEditor JSON usage
        * Updated: Existing Situations
        * Updated: Situations now entirely in JSON
        * Updated: DSMC moved to Mission Options
        * Updated: Spawn point recovery on failed spawn
        * Updated: Unit selection now checks potential allies of coalition nations rather than using default unit lists.
        * Updated: Iframe post message (for Web editor)
        * Updated: Use Custom Unit Operators
        * Updated: Set SU-24M as bombers
        * Updated: All maps to JSON spawn point data
        * Updated: Added More Spawn points the Caucasus
        * Updated: Increased AAA at FARP distance
        * Updated: No longer required to have a default unit for unit types that didn't exist yet (eg SAMs in the 1940s)
        * Updated: KC135MPRS as boom tanker
        * Updated: SinaiMap as desert map
        * Updated: Data from Sinai update patch
        * Updated: Apache Payloads
        * Updated: Fortifications data
        * Updated: Cars data
        * Updated: Airbase data for all maps
        * Updated: Disabled Mods that lost support (If you liked these mods consider helping return support)
        * Updated: Adjust escort AI mode (still not entirely functional)
        * Updated: Bofors 40mm to be in use in 2020 (represents the L70 gun still in use) & KS-19 still in use
        * Updated: Removed auto spawn EWR unit from Skynet  feature
        * Updated: Feature Aircraft payloads
        * Updated: Unit file names in DB
        * Updated: Ground unit destinations no longer takeup spawn points
        * Updated: Patrol Circle now max distance from start.
        * Updated: Use Area marker for patrols and shorten variation distances for vehicles
        * Updated: Reduce search range for Patroling Vehicles
        * Updated: UnitCars to use better IDs
        * Updated: TheaterAirbases JSON for all maps
        * Updated: WW2 Units In general
        * Updated: WW2 Asset pack AAA (Many going to AAA sites)
        * Updated: Moose to 2023-03-30
        * Updated: Normandy & Channel Boundaries
        * Updated: docker warning
        * Updated: Skynet IADS: 3.1.0
        * Updated: Some map boundaries
        * Updated: Removed httpd as no longer used in DCS fiddle
        * Updated: Moose 2.7.18
    * Fixed:
        * Fix: Rename Flag file RSII
        * Fix: Template positions
        * Fix: Correct Airbase Strike to Attacking aircraft
        * Fix: Fallback to ignore control areas if using front line logic but can't find spawn points
        * Fix: Add more spawn points types for Enemy airbase AAA
        * Fix: Stopped mission generations from failing when failed to place non critical aircraft at airbases (Failed cap spawn in air & features won't spawn)
        * Fix: Now attempts all possible airbases to spawn objective aircraft at rather than nearest.
        * Fix: Rendering no-spawn zones and multi blue zones on preview map
        * Fix: Point recovery not rejecting Spawn points that aren't on the list
        * Fix: Situation editor loading zones showed wrong
        * Fix: Deal with empty spawn point lists when generating SP mission
        * Fix: Mods not being selected correctly.
        * Fix: Preset Objectives Ignoring Selected Objective features
        * Fix: Docker build
        * Fix: Iframe crash and default to dark mode
        * FIx: improve Destroy lua triggers - Thanks Eagle01
        * FIx: DIsable trigger not working - Thanks Eagle01
        * Fix: Unable select specific FOB templates
        * Fix: Removed spam console log
        * Fix: Nato Callsigns same for each aircraft
        * Fix: CAP cover for parked aircraft no longer given escort tasking
        * Fix: Objective CAP ground starting when their task starts flying by default
        * Fix: Objectives not using templates
        * Fix: Artillery and Missile Launchers no longer varied units within group
        * Fix: Prevent Yaml wrapped happening when set as false
        * Fix: Escort AI Not spawning
        * Fix: Auto mod append not filtering core mods out
        * Fix: Bad parameter name
        * Fix: Correct spelling of Realism
        * Fix: Sinai Projection and bounds on UI map
        * Fix: Throw errors if template loading fails due to parsing
        * Fix: Air start unable to start mission scripts
        * Fix: Landing aircraft altitude not set
        * Fix: Error when airbase had no parking spots
        * Fix: Mark MANPAD commanders VehicleStatic to avoid immovable infantry
        * Fix: Bad default breaking unit selection
        * Fix: Proper Callsign assignment using ME data
        * Fix: Unreliable Hint Interactions
        * Fix: Missing airbases from data
        * Fix: Briefing Editor
        * FIx: Default issues with Livery and Payload
        * Fix: Instant spawn aircraft spawn quicker
        * Fix: Yaml parsing issue
        * Fix: complication error
        * FIx: Carriers should no longer be routed though land
        * Fix: Manpads commanders can't fire
        * FIx: Cruise Speed using max altitude aircraft too speedy
        * Fix: Bad if statement in layout chance
        * Fix: SAM unit families
        * Fix: Reg) => Reg
        * Fix: Missing default unit warnings
        * Fix: Another payload error
        * Fix: Remove IronFists from Liveries
        * Fix: More file names
        * Fix: Liveries error
        * Fix: Bugs with creating missions
        * Fix: Malformed KC130 & KC135MPRS
        * Fix: Get basic missions generating
        * Fix:  Player controllable can't come from ED for now
        * Fix: Taskable CAS scripting error
        * Fix: Objective complete debug scripting error
        * Fix: Correct Country spellings
        * Fix: Radio Frequency formatting
        * Fix: Bad parking parsing
        * Fix: B-17G No longer German WW2
        * Fix: Ju-88A4 not marked as bomber
        * Fix: Missing DatabaseJSON files
        * Fix: Close to sea logic
        * Fix: bad parking space marker
        * Fix: briefing.html & credits.txt file were lost after ME editing. Also added BR purity seal
        * Fix: Fog of war setting shouldn't be force hiding units
        * Fix: Use FOB call sign for start position
        * Fix: Bad Pathing breaking feature scripting
        * Fix: F-5E-3 Tiger II invalid default radio
        * Fix: Map bounds lines not Geodesic
        * Fix: Pre-load items in docker image
        * Fix: Attempt beta build fix
        * Fix: bad sln
        * Fix: Nato AWACS comms broken when using Skynet
        * Fix: Spinner not spinning in docker
        * Fix: SA boundries
        * Fix: Regional settings causing app to fail to load in Turkish
    * UI:
        * UI: Preview Min width 50% of screen
        * UI: Warning info added
        * UI: Compact Logger logs
        * UI: Added EWR Map markers
        * UI: Add github and discord image links
        * UI: Fix Flight Group and Mission Sub tabs not resetting
        * UI: Show map borders on generated missions
    * Upgraded:
        * Upgrade: Bump IronPdf from 2023.6.9 to 2023.6.10 in /Source
        * Upgrade: Bump Markdig from 0.31.0 to 0.32.0 in /Source
        * Upgrade: Bump Microsoft.Maui.Graphics from 7.0.86 to 7.0.92 in /Source
        * Upgrade: Bump Newtonsoft.Json from 13.0.2 to 13.0.3 in /Source
        * Upgrade: Bump Polly from 7.2.3 to 7.2.4 in /Source
        * Upgrade: Bump YamlDotNet from 13.1.0 to 13.1.1 in /Source
    * Misc:
        * Misc: Code cleanup
        * Misc: Code cleanup
        * MIsc: provide scale guides for spawn points
        * Misc: Format JSON files
        * Misc: Remove useless console log
        * Misc: Add temp Operational list code
        * Misc: Put build version in more places
        * Misc: Remove old ini spawn point loading code
        * Misc: Generate Mission file from JSON spawn points
        * Misc: Spawnpoint extractor dumps to JSON
        * Misc: Remove non functioning code
        * Misc: Removed Redundant Zone data in .ini files
        * Misc: Remove Old water data in Ini
        * Misc: Improve SEO
        * Misc: Identify which airbase didn't have enough parking
        * Misc: Format all files
        * Misc: Remove unused code
        * Misc: Remove unsupported mod files
        * Misc: Mark old units DB as deprecated
        * Misc: Update github pages page
        * Misc: Map unknown unit warning
        * Misc: Templates, Fortifications and Warehouse json data
        * Misc: Throw error on missing shore feature
        * Misc: Removed redundant airbase files
        * Misc: Add Ko-fi links for docker version
        * Misc: Add debug for early mission complete
        * Misc: Remove Console log
* Version 0.5.301.28(Jan, 28, 2023)
    * Added:    
        * Added: httpd script for DCS Fiddle
        * Added: SA new Airbases
        * Added: Ropucha-class landing ship & Mirage-F1EE (as playable)
        * Added: End Mission on command option
        * Added: Auto End mission after 10 mins option
        * Added: Super Hornet Mod support
        * Added: Custom transport helicopter calls
        * Added: Taskable Transport Helicopter
        * Added: Support attack started with radio command
        * Added: Proximity for Complete defend & possible to fail if all die post completion
        * Added: Extract troops task
        * Added: Defend Attack Task
        * Added: Support Attack Objective Task
        * Added: Support for multiple event triggers per objective
        * Added: lowUnitVariation support (for attacking groups)
        * Added: Objective Patrol extra waypoints
        * Added: MB-339
        * Added: All aircraft have a clean payload option
        * Added: FOB and Carrier Hint locations
        * Added: Hint Saving and Better UI
        * Added: hint per objective
        * Added: SA Rio Chico
        * Added: Ka-50 Hokum 3.0
        * Added: True and Magnetic Heading to briefings
        * Added: CAP random mid course route changes
        * Added: Carrier Recovery Tankers
    * Fixed:
        * Fix: Hide Landmass
        * Fix: Get Taskable helos working from ground start
        * Fix: Moose ATIS wasn't working
        * Fix: Destroy Tracking radars causing script errors
        * Fix: Taskable attack helicopters broken
        * Fix: Taskable affirm sounds not working
        * Fix: Remove Culture issue risk from changing cases
        * Fix: broken Gamemaster_Functions script post formatting
        * Fix: Cargo task not generating
        * Fix: attempt mitigation of map point failure error
        * Fix: Minor spelling
        * Fix: Transport counts showing as 0
        * Fix: Campaign gen crashed if can't find another airbase to move to.
        * Fix: Objective Feature Unit positioning
        * Fix: UH-1H Bad radio
        * Fix: Combat Search and Rescue Preset
        * Fix: Bad troop transport spelling
        * Fix: Bad A-4E Radio
        * Fix: Objective Scenery Fire not near objective units
        * Fix: CTLD off by one meant first pilot didn't get CTLD options
        * Fix: Cargo Objective scripting error
        * Fix: double parsing not InvariantCulture
        * Fix: CTLD logistic vehicles
        * Fix: Bad F-86F Sabre radio frequency
        * Fix: Mirage 2000C bad default radio
        * Fix: Magnetic heading could go into negatives
        * Fix: Significantly reduce bad position chance.
        * Fix: Objective ground units cannot be directed into the sea
        * Fix: Ka-50 3 Modification not set
        * Fix: Minor Typo in Location Hints text.
        * Fix: Support target units picked up by ActiveGround units
        * Fix: Don't crash on missing translation
        * Fix: UI map crash if AIRBASE_HOME doesn't exist in map data
        * Fix: Friendly Taskable Bomber not bombing  - Thanks Eagle01
        * Fix: remove odd lua code
        * Fix: Destroy all but air defense script error  - Thanks Eagle01
        * Fix: Missing Proj4 Import on desktop
        * Fix: No longer default to UnitID for mods if required mod string isn't provided
        * Fix: Mission not marking as complete
    * Updated:
        * Updated: Support Proj4Js projector for maps
        * Updated: Removed Position Lookup Files
        * Updated: Various fixes to Taskable Units - Thanks Eagle01
        * Updated: PersianGulf radio frequencies
        * Updated: Mitigate double troop pickups
        * Updated: Transported Troops considered main characters and survive crashes.
        * Updated: Embedded Air defense more suitable Infantry now only have infantry AAA Statics can now have static AAA (units not SAM sites)
        * Updated: Scale patrol distances by unit type
        * Updated: Core to dotnet7
        * Updated: Refactored Page & Template code
        * Updated: changed distance settings using hint and ignore borders
        * Updated: Campaign generator now has specific min-max objective distance
        * Updated: Troop Transport AI now move to aircraft
        * Updated: MIST CSAR script added MI-24 & AH-64
        * Updated: Default.cbrt and Default.brt
        * Updated: CTLD Pickup zones marked on maps
        * Updated: CTLD to Sept 2020 dev version
        * Updated: Remove CTLD default spawned units
        * Updated: CSAR script to Moose version
        * Updated: escort, troop or cargo transport missions reverse direction with GoToPlayerBase
        * Updated: South Atlantic Airfields
    * UI:
        * UI: missing AAA icon
        * UI: Hide purposely hidden objective features
        * UI: Hints now show bounding box
        * UI: Better hint info
        * UI: Able to remove location hints in quick builder
        * UI: Note Moose script is automatically loaded by other scripts and not mandatory
        * UI: Rename Moose ATIS to ATIS (may break templates)
    * Misc:
        * Misc: de-risk releases failing
        * Misc: Added jboecker (httpd) credit
        * Misc: Spelling
        * Misc: Update credits
        * Misc: revert branch CI net7 change
        * Misc: Mass formatting
    * Upgrades:
        * Upgrade: Bump Microsoft.AspNetCore.Components.WebView.WindowsForms
        * Upgrade: Bump Swashbuckle.AspNetCore from 6.4.0 to 6.5.0 in /Source
        * Upgrade: Bump Microsoft.AspNetCore.Components.Web in /Source
        * Upgrade: Bump YamlDotNet from 12.2.0 to 12.3.1 in /Source
        * Upgrade: Bump Blazored.LocalStorage from 4.2.0 to 4.3.0 in /Source
        * Upgrade: Bump Newtonsoft.Json from 13.0.1 to 13.0.2 in /Source
        * Upgrade: Bump YamlDotNet from 12.0.2 to 12.2.0 in /Source
* Version 0.5.211.15(Nov, 15, 2022)
    * Added:
        * Added: Enhanced Gamemaster Script (Zeus)
        * Added: Situation Editor
        * Added: Auto convert ground static aircraft to actual static units (for performance, though loss of loadout)
        * Added: Ice Halo set to auto
        * Added: Full Builder Objective Location hint option
        * Added: Linkage between campaign missions (starting airbase, objective area & situation)
        * Added: Friendly and Enemy Bombers as mission feature
        * Added: Moose Core and Moose ATIS for home base
        * Added: Ingress and Egress points have unique names
        * Added: Active Ground units
        * Added: Objective feature friendly/enemy ground forces
        * Added: Mapping support for Caucuses, South Atlantic, Mariana Islands, Persian Gulf and Syria (using Gzip Tech)
        * Added: Basic Data based mapping method
        * Added: waypoints for flights on UI map
        * Added: Js real world map the UI
        * Added: No BDA and Target Only BDA options
        * Added: Aircraft Respawn Script
        * Added: F-16 auto preset config
        * Added: Auto set Air & ATC frequencies for players (where configured)
        * Added: OV-10A Bronco mod
    * Fixed:
        * Fix: Spelling
        * Fix: Map Icons showing Blue if player on Red side & Neutral airbase icon missing
        * Fix: Briefings showing trigger suffixes
        * Fix: Briefings ignore static aircraft
        * Fix: Scripts not checking for NEUTRAL coalition units.
        * Fix: Static carrier aircraft not having group names
        * Fix: Script crash
        * Fix: Singleton scripts (such as Moose) should only run once & before other feature scripts
        * Fix: UI map giving away position of objective with inaccurate waypoint
        * Fix: Strike packages given wrong waypoints in game
        * Fix: Strike Packages not correctly identified
        * Fix: MP FARPS no-rearm & double spawing
        * Fix: Dedicated Server Duplicate FARPS (no more extra units)
        * Fix: Corrupted FARP
        * Fix: Some Ships not able to re-arm
        * Fix: Immortal and invisible commands take priority in AI commands
        * Fix: Friendly AWACS and Tankers now invisible to AI not invincible
        * Fix: No ATC caused failed generate
        * Fix: Cache map data on UI
        * Fix: Broken file path
        * Fix: Danger Close units not spawning danger close
        * Fix: Removing old spawn anywhere broke quick builder
        * Fix: Template can now survive ED's editing
        * Fix: Remove Skill None, Use DCS Random Skill & Choose skill per unit not per group.
        * Fix: Flair and Illumination Bomb calls
        * Fix: Spawn anywhere not using any airbase
        * Fix: Parked target AI aircraft activating when they shouldn't
        * Fix: Set Waypoint to AGL 0 for ground targets
        * Fix: Correct Bronco AI speed
    * Updated:
        * Updated: Let AI sort out Orbit speed and Alt
        * Updated: Refactored Taskable unit scripts
        * Updated: Moose to 2.7.16
        * Updated: Situation Editor now adjusts to actual positions on generate
        * Updated: Skynet to 3.0.1
        * Updated: Moose ATIS to 227.00
        * Updated: MarianaIslandsHopping situations related for campaign usage.
        * Updated: Give AI Aircraft hints that they can avoid threats and fire back sometimes.
        * Updated: B-1B Loadouts
        * Updated: AircraftRespawn Script get independent 5-10 min check interval with 30% spawn chance.
        * Updated: Tuned Ground forces mission feature placement
        * Updated: Map warning
        * Updated: Migrate Spawn anywhere to Situation option
        * Updated: Syria ATC Radios
        * Updated: All packages to net6
        * Updated: Increase Respawn time and lower chance for AI
        * Updated: FA-18C_hornet.ini
        * Updated: Script.ini
        * Updated: AircraftBomb.yml
        * Updated: AircraftCAP.yml
        * Updated: AircraftCAS.yml
        * Updated: AircraftAWACS.yml
        * Updated: AircraftSEAD.yml
        * Updated: AircraftPatrol.yml
        * Updated: AircraftOrbitingSEAD.yml
        * Updated: AircraftUncontrolled.yml
        * Updated: AircraftOrbiting.yml
        * Updated: AircraftTankerNOTACAN.yml
        * Updated: AircraftOrbitingCAS.yml
        * Updated: AircraftMoving.yml
    * UI:
        * UI: Situation Editor improve descriptions
        * UI: Situation Editor disable circle and rectangle
        * UI: Minor updates for Situation Editor
        * UI: Fix missing images for objectives
        * UI: Map better routing
        * UI: Improve waypoints and objectives
        * UI: Complete NATO icons
        * UI: Add nato unit icons (general)
        * UI: Add Channel Map support
        * UI: Normandy Map Support
        * UI: Add Nevada Map support
        * UI: Minor map updates
        * UI: Icon hits and SubTasks
        * UI: Remove aircraft from map icons
        * UI: Map add units, SAM sites & Bases
        * UI: Fix full screen Briefing
        * UI: Add missing maps and Allow Full screen Briefing
    * Misc:
        * Misc: Format Files
        * Misc: Credit Moose
        * Misc: Credit NaDs
        * Misc: Support task priority ordering
        * Misc: Improved map error message
        * Misc: enable and document data extraction for dev
    * Docs:
        * Docs: Improve Dump map data docs
        * Docs: Credit Juanillus
    * Upgrades:
        * Upgrade: Bump Microsoft.AspNetCore.Components.Web in /Source
        * Upgrade: Bump Microsoft.AspNetCore.Components.WebView.WindowsForms
        * Upgrade: Bump YamlDotNet from 12.0.1 to 12.0.2 in /Source
        * Upgrade: Bump Microsoft.AspNetCore.Components.WebView.WindowsForms
        * Upgrade: Bump Markdig from 0.30.3 to 0.30.4 in /Source
        * Upgrade: Bump Microsoft.AspNetCore.Components.Web in /Source
        * Upgrade: Bump YamlDotNet from 12.0.0 to 12.0.1 in /Source
        * Upgrade: Microsoft.AspNetCore.Components.WebView.WindowsForms

* Version 0.5.209.03(Sept, 03, 2022)
    * Added:
        * Added: Mirage F1
    * Fixed:
        * Fix: Command Line Exe
        * Fix: correct Chile spelling in scenario
        * Fix: Static objective was always singular in tasks
        * Fix: Script failing to track static unit objectives
        * Fix: Player spawning didn't account for taken spots
        * Fix: Quick builder not picking up on custom objectives (also  details)
        * Fix: Title improvements
        * Fix: Remove custom hornet payload file name
        * Fix: Untranslated Remark Sections
        * Fix: Flight Kneeboard missing translations
        * Fix: Tankers not taking up comms settings
        * Fix: Corrupt Lua
        * Fix: Bad Playable Mirage config
        * Fix: More incomplete translations   
    * Updated:
        * Updated: use LuaTableSerializer package over localized version
        * Updated: South Atlantic map with new airbases
        * Updated: Doubled max search area for mission feats
        * Updated: Disabled South Atlantic Assets pack and migrate units to base game
        * Updated: Move limits into CommonDB
        * Updated: F1 Cruise Speed
        * Updated: Skynet to 3.0.0 main release
    * Misc:
        * Misc: Removed unused make file
    * Upgrades:
        * Upgrade: Bump Microsoft.AspNetCore.Components.Web in /Source
        * Upgrade: Bump Markdig from 0.30.2 to 0.30.3 in /Source
        * Upgrade: Bump Swashbuckle.AspNetCore from 6.3.2 to 6.4.0 in /Source
        * Upgrade: Bump YamlDotNet from 11.2.1 to 12.0.0 in /Source
        * Upgrade: Bump Microsoft.AspNetCore.Components.Web in /Source
* Version 0.5.206.22(Jun, 22, 2022)    
    * Added    
        * Added: Initial South Atlantic Asset Pack Mod
        * Added: Can load templates from Within Missions and Campaigns
        * Added: Embedded template.br and version number in .miz file
        * Added: Custom Config support for Operators and Operator Liveries
        * Added: Ability to Override Operators and OperatorLiveries in custom configs
        * Added: Support for no airbase situations
        * Added: South Atlantic Briefing Images
        * Added: Neutral Static Aircraft
        * Added: Neutral Air Traffic
        * Added: Civil Aircraft Mod
        * Added: Required Mods to Briefings
        * Added: Type-59
        * Added: Advanced overrides for player flights (radio and callsign)
        * Added: South Atlantic map initial support
        * Added: Auto Import Liveries
        * Added: Auto Import of Player Custom Loadouts
        * Added: Cardinal Wind Directions to Briefing
        * Added: Carrier Link4 support
        * Added: Advanced Aircraft Spawning
        * Added: Scramble Start
        * Added: Waypoint number to briefings and Kneeboards
        * Added: Random Objective presets for Quick Generator
        * Added: Option to disable kneeboard images
        * Added: Airbase Vehicle Spawn spots Caucasus & Syria
        * Added: Airbase Vehicle Spawn spots Gulf
        * Added: Airbase Vehicle Spawn spots Marianas
        * Added: AA at FOBS & Airbases (Friendly & Enemy)
        * Added: TACAN beacons at all FOBS and populated Airbases
        * Added: TACAN Beacons at objectives and home base
        * Added: S-3B and F-4 as low res aircraft
        * Added: include Low-res aircraft option (marked particularly low res aircraft)
        * Added: Ground Start Aircraft can spawn on carriers
        * Added: Initial carrier static aircraft
        * Added: Images to new coalitions
        * Added: Common Alliances as Coalitions
        * Added: Vehicle Operator Liveries
        * Added: Vehicle Operator Liveries
        * Added: Vehicle Operator Liveries
        * Added: Vehicle Operator Liveries
        * Added: Vehicle Operator Liveries
        * Added: More Aircraft Operator Liveries
        * Added: More Aircraft Operator Liveries
        * Added: More aircraft OperatorLiveries
        * Added: More Aircraft Operator Liveries
        * Added: Operator Liveries for playable helicopters
        * Added: Missing National Flags
        * Added: a bunch more countries as coalitions
        * Added: M-2000C OperatorLiveries
        * Added: Support for Default liveries based on Country
        * Added: a bunch of countries to coalitions
        * Added: Helux and Land Cruiser variants
        * Added: Egypt flag
        * Added: Egypt coalition
    * Updated
        * Updated: A bunch of aircraft runway requirements
        * Updated: South atlantic Temps and daytimes
        * Updated: Aircraft move on airbase have now bomb if suitable
        * Updated: MIST to 4.5.1.07
        * Updated: Campaigns now in chronological date order.
        * Updated: Tornado IDS Operator Liveries
        * Updated: SU-24M Operator Liveries
        * Updated: SU-25 Operator Liveries
        * Updated: Retire Bofors 40mm in 1990 rather than 2020
        * Updated: Rapier promoted to medium SAM
        * Updated: Allow units to use small spawn points
        * Updated: Mark SU-30 as low-res
        * Updated: Aircraft loadouts and roles
        * Updated: Helicopter loadouts & Families
        * Updated: Aircraft Operators
        * Updated: Helicopter Operators
        * Updated: Waypoint marker colours
        * Updated: Morocco.ini
        * Updated: M109 Howitzer.ini
        * Updated: M-1 Abrams.ini
    * UI
        * UI: Improve Carrier Label
        * UI: Can now clone tasks
        * UI: Remove "improvement" from stronger splash damage
        * UI: Update F-16C display name to include Viper
        * UI: Use website font for titles
    * Fix
        * Fix: Remove useless config
        * Fix: Bad situation name
        * Fix: Move to player airbase wasn't set right with not airbase
        * Fix: HighDigitSAMs mod units not designated as HighDigitSAMs
        * Fix: Cargo missing group name corrupting missions
        * Fix: More cases of internal value not changing when its no longer in the option list.
        * Fix: Fast generations broke map.
        * Fix: Don't waste spawn points on features without units
        * Fix: Bad pickups with ZU-23 unit
        * Fix: Custom Coalitions failing on missing image
        * Fix: Not internally selecting valid target when Task changed with invalid target UI
        * Fix: A bunch of configs
        * Fix: Bumped Tanker TACAN to start at 26X (removing airbase overlap)
        * Fix: Unable to have more than one strike package at a airbase
        * Fix: Spelling of Stanley
        * Fix: Briefing editor dealing with quotes
        * Fix: More bad aircraft configs
        * Fix: Aircraft & Ship SubTask Spread
        * Fix: Missing ActivateACLS
        * Fix: Increase advanced aircraft spawn detection interval to 1 min
        * Fix: Bomber and Transport targets had no scaling
        * Fix: CAP not able to trigger at start. (25% chance per CAP group)
        * Fix: Format dates DD/MM/YYYY (like most nations not that odd one)
        * Fix: Coalition determining Nato Callsign corrupting mission files
        * Fix: Remove Marianas spawn points too close to the water
        * Fix: Correct Mariana islands airbase spawn
        * Fix: Get ForEachAirbase positioning actually working
        * Fix: Don't spawn CTLD zones if not using CTLD
        * Fix: Missing Gun parameter
        * Fix: correct loadout configs
        * Fix: Remove spawn points in the sea (and throw error if it occurs again)
        * Fix: Carrier Frequencies not being set
        * Fix: Correct missing offshore names
        * Fix: Missing Ship Patrolling file
        * Fix: Retry logic wasn't working
        * Fix: Command line build
        * Fix: Aircraft not actually patrolling
        * Fix: Ramat David Radio
        * Fix: Add missing mod module IDs
        * Fix: Objective aircraft where not being spawned on takeoff
    * Misc
        * Misc: Minor table styling
        * Misc: Remove unused code
        * Misc: Remove extra commas and spaces from configs
        * Misc: added common Kneeboard issue fixes
        * Misc: Remove TACAN=-
        * Misc: Cleanup config files
        * Misc: Github page test
        * Misc: Call for Contributors
        * Misc: Parking spot info added in DB
    * Website
        * Website: Update Page order
        * Website: Add Image Carousels
        * Website: Added dynamic release info
        * Website: Updated styling
    * Upgrade
        * Upgrade: Bump Microsoft.AspNetCore.Components.WebView.WindowsForms
        * Upgrade: Bump Microsoft.AspNetCore.Components.Web in /Source
        * Upgrade: Bump FluentRandomPicker from 3.1.0 to 3.2.0 in /Source
        * Upgrade: Bump Markdig from 0.28.1 to 0.30.2 in /Source
        * Upgrade: Bump Swashbuckle.AspNetCore from 6.3.0 to 6.3.1 in /Source
        * Upgrade: Bump Microsoft.AspNetCore.Components.Web in /Source
* Version 0.5.204.17(Apr, 17, 2022)
  * Added
    * Added: Coordinate readouts now contains altitude in ft
    * Added: Persistent Waypoint Coordinates in Objective Radio Menu
    * Added: Persistent Target Coordinates in Objective Radio Menu
    * Added: Combined Arms Pilot Control mission option
    * Added: Combined Arms Template support
    * Added: Backend support for Combined Arms
    * Added: Campaign support multiple player flights
    * Added: Mission Feature Ships for Enemy and Friendly
    * Added: Static and Offshore static buildings
    * Added: Missing Spinner to Campaign Generator
    * Added: Syria Iraq 2003 situation (no short flights)
    * Added: Syria new airbases
    * Added: Playable Apache
    * Added: Blackhawk Clean and External Tanks Loadouts
    * Added: First implementation of Briefing Editor
    * Added: ME compatible mod requirements.
    * Added: HTML Rendered KneeBoards
    * Added: Initial attempt at multi objective descriptions
    * Added: Initial situation briefing sections
    * Added: Support for more rich Briefings
    * Added: Missing Clear Weather Preset
    * Added: CTLD native support
    * Added: Zone support
    * Added: Escort Presets
    * Added: Transport Troop Preset
    * Added: Troop Transport Radio Calls
    * Added: BR script for transport over CTLD
    * Added: Taskable attack helicopters
    * Added: Fleshed out escort task
    * Added: basic initial escort task
    * Added: Script Assisted Troop Transport
    * Added: Initial Troop Transport Task (WIP)
    * Added: CTLD Script
    * Added: CSAR script
    * Added: FOB Static items
    * Added: Takeoff distance to Bomber
    * Added: Takeoff distance to PlaneFighter
    * Added: changes to some Fighter AC on Rwy length
    * Added: More units variety in static targets
    * Added: some more static groups mostly large things
    * Added: Static Enemy Aircraft
    * Added: High Cloud
  * Updated
    * Updated: Channel new airfields - thanks Sandman
    * Updated: Skynet IADS to 3.0.0-dev
    * Updated: Enable CM control for Anti-Air disable for Carrier and FOB
    * Updated: Carriers loop back to start point
    * Updated: Limit Units from spawning more than 70nm from own controlled area.
    * Updated: Syria Situations
    * Updated: Syria default zone
    * Updated: Merge Flight Plan, Airbase and JTAC Kneepages
    * Updated: Removed or shortened remarks
    * Updated: Kneeboard notes and added JTAC section to briefings
    * Updated: Increase KneeBoard Resolution
    * Updated: Tune Taskable AI
    * Updated: Limit kill radio call outs to just players
    * Updated: AI Radio Freqencies
    * Updated: CSAR script
    * Updated: Commando Drop in Preset to be Transport Troop mission
    * Updated: No longer allow CTLD to manage Transport AI groups
    * Updated: EnemyStaticAircraft.ini
    * Updated: FriendlyHelicopters.ini
    * Updated: FriendlyCAS.ini
    * Updated: EnemyStaticAircraft.ini
    * Updated: FriendlyStaticAircraft.ini
    * Updated: FriendlySEAD.ini
    * Updated: FriendlyHelicopters.ini
    * Updated: FriendlyCAS.ini
    * Updated: EnemyTanker.ini
    * Updated: EnemySEAD.ini
    * Updated: EnemyHelicopters.ini
    * Updated: EnemyCAS.ini
    * Updated: EnemyAWACS.ini
    * Updated: FriendlySEAD.ini
    * Updated: F-117A moved to bomber
  * Fixed
    * Fix: Correct bad HDS mod required ID
    * Fix: Campaign unable to select starting airbase
    * Fix: Campaign Generator  not using mission retry logic
    * Fix: Logs where needlessly cleared on mission generation
    * Fix: Correctly scale map
    * Fix: Correct weapon configs Mi-24V & JF-17
    * Fix: Show FCR on Apache
    * Fix: Turkey was assigned to UK underneath
    * Fix: Dog Ear radar ref
    * Fix: add missing exe
    * Fix: Can't find image issue due to pathing issues
    * Fix: Increase Briefing Editor save mission size
    * Fix: Use temp file in proper temp folder for windows
    * Fix: Increase Zip memory size
    * Fix: Bad Incirlik VHF frequency
    * Fix: Black Hawk mod references
    * Fix: Preset typo
    * Fix: Broke kill messages (ignore player only calls)
    * Fix: Callsigns started with 2 not 1
    * Fix: ignore parking on airbase if not aircraft
    * Fix: Taskable support not spawning
    * Fix: Always populate country units block even if no units.
    * Fix: only remove payload for harrier on STOVL ships
    * Fix: FOB's where not regularly closer than main airbase
    * Fix: Not spawning FOBs at all
    * Fix: Flight groups not auto changing CJTF sides
  * UI
    * UI: added Channel ground mapping - thanks sandman
    * UI: Update with Combined Arms options
    * UI: Added Combined Arms Options to Full and Campaign Builder
    * UI: Warnings only shown for mission shown in UI not failed generation attempts
    * UI: Added copy logs button
    * UI: Map added to Campaign Builder UI
    * UI: Added abstract map of mission
    * UI: Add simple loading spinner
    * UI: More accurate names for some situation options
    * UI: More indications in quick builder of inherited options
    * UI: Added home base option to quick builder
    * UI: Stronger indications of Quick Builder limitations and features loaded from templates
    * UI: Add more task validation
  * Misc
    * Misc: Lower Found a non-assigned value warning to info
    * Misc: Log Excessive Distances
    * Misc: Log Recoverable Error
    * Misc: add airbase ID grabber
    * Misc: added build version to github issue
    * Misc: report error logs when kneeboard convert fails
    * Misc: Remove usage of temp file for KneeBoard generation
    * Misc: improve auto release name
    * Misc: Use MIST by default
    * Misc: Update Country conflict warning
    * Misc: Bump Markdig from 0.28.0 to 0.28.1 in /Source
    * Misc: Bump Markdig from 0.27.0 to 0.28.0 in /Source
    * Misc: Bump Microsoft.AspNetCore.Components.Web in /Source
    * Misc: Bump Swashbuckle.AspNetCore from 6.2.3 to 6.3.0 in /Source
    * Misc: Bump Microsoft.AspNetCore.Components.Web in /Source
    * Misc: Bump Blazored.LocalStorage from 4.1.5 to 4.2.0 in /Source
    * Misc: Bump Polly from 7.2.2 to 7.2.3 in /Source
    * Misc: Bump Markdig from 0.26.0 to 0.27.0 in /Source
* Version 0.5.201.21(Jan, 22, 2022)
  * BREAKING: feature Ids (You will need to reapply these on templates)
  * Added
    * Added: Lower Cloud Migrated to Mission options (Mission options now more extendable)
    * Added: Low Cloud option (references cloud to sea level rather than airbase)
    * Added: Skynet Command Center Support
    * Added: simple docs on creating Situations
    * Added: Situations for map varied map zoning
    * Added: auto retry if BriefingRoomException received on generation
    * Added: auto retry and warning when objective distance is over 1.7x requested
    * Added: scenery artillery for takable artillery
    * Added: Ground Start Aircraft
    * Added: General Training level under situation
    * Added: Infantry and Cars to front line pool.
    * Added: High Digit SAMS mod support - thanks gregfp66
    * Added: Updated Campaign enough to call out of BETA
    * Added: Friendly CAS and SEAD (not tasked)
    * Added: Feature Enemy SEAD and CAS
    * Added: Scenery, Friendly Static Aircraft
    * Added: Situation Syria Israel War with Lebanon (No Allies)
    * Added: Situations Marianas (multipule)
    * Added: Situation Gulf Oman Invades UAE
    * Added: Random option to Situation
    * Added: Situation: Syrian Civil War (Pre Lebanon spill over)
    * Added: Support for No Spawning area in Situations
    * Added: Situation: Caucasus Russo Georgian War 2008
    * Added: Situation: Caucasus Russian Costal Invasion (Early and late)
    * Added: Situation: Caucasus Beslan Rebellion
    * Added: Situation Persian Gulf - Civil War in Iran -Thanks anthon
    * Added: PersianGulf Situations - Thanks Diesel
    * Added: Nevada Situations - Thanks Diesel
    * Added: Friendly and Enemy Helicopters (split out from CAS)
    * Added: SubTask Template loading and saving
    * Added: KJ-2000, Tornado (IDS & GR4), KC-130, SU-24MR, Yak-40
    * Added: Players can be on hostile side (Simple)
    * Added: Hide Borders Mission option
    * Added: USAF Aggressors
    * Added: JTAC Single Laser Code mission option
    * Added: Mark cold start player aircraft with "(C)"
    * Added: 10,000ft minimum runway requirement for large aircraft (Data still needed)
    * Added: Dynamic Laser codes for each objective
    * Added: Allow Custom Coalition Configs
    * Added: Units Assigned to respective coalition countries if possible
    * Added: Credits under Manual.
    * Added: Runway Lengths for The Channel
    * Added: Runway Lengths to Normandy
    * Added: Nevada runway lengths
    * Added: Warn player if specifically chosen airbase has a too short runway for chosen aircraft
    * Added: Support Aircraft Minimum Runway lengths (Data still required)
    * Added: Syria Nicosa airbase spawn points (tiny chance of FOB spawning there)
    * Added: Syria Runway Lengths
    * Added: PersianGuld Runway Lengths
    * Added: Runway Lengths Mariana Islands
    * Added: Runway Lengths Caucuses
    * Added: Spawning on non dedicated carrier ships
    * Added: Speaker Identifiers on radio message text
    * Added: SkynetIADS EWR Radars
    * Added: Initial SKYNET IADS system
    * Added: In mission user manual
    * Added: Extra Group feature support
    * Added: Sub Objectives as extra Tasks.
    * Added: SubObjective Template Props
    * Added: Initial UH-60L Mod support
    * Added: Splash Damage script as optional feature
    * Added: More Task Description options to Briefings
    * Added: UH-60L Radio config
    * Added: Gave JTAC Bigger brain swishes target if its considered complete in objectives. Can also switch target on command
    * Added: Disable Task (better suited for attacking parked aircraft)
    * Added: Plane Attack minimum Runway Length
    * Added: Basic DSMC compatibility by auto completing objectives on start
    * Added: Minimum Runway Length to Tankers
    * Added: complete User Mission Manual
    * Added: Scenery Drone and JTAC as objective features
    * Added: Spawn Friendly CAS and SEAD by player
    * Added: Missing Sounds for Laser Designation
  * Updated
    * Updated: default template
    * Updated: Gulf Parking Positions
    * Updated: Marianas Parking Positions
    * Updated: Caucasus parking positions
    * Updated: Syria Parking Positions
    * Updated: SEAD loadouts: F-16 & F-14B- Thanks Rhys
    * Updated: Front line troops now return fire
    * Updated: Gulf Civil War in Iran - thanks anthon
    * Updated: Cyprus Conflict to not spawn in Syria, Israel ect
    * Updated: Waypoint Marker is now Drawn
    * Updated: NevadaEchobay.ini
    * Updated: Reduced large aircraft runway requirement to 7000ft
    * Updated: A-10 Minimum Runway Length.
  * Fixed
    * Fix: JTAC looping issue
    * Fix: Airbase Parking spawn tasks waypoints are now on ground
    * Fix: Airbase spawn listens to side limitations
    * Fix: increased base cloud level range was just 650ft
    * Fix: Player not being set when wingman used (you didn't see this)
    * Fix: UI wasn't setting Situation right when map change
    * Fix: use parking over parking_id
    * Fix: Correct Kingsfield location
    * Fix: Correct first in player group position
    * Fix: Improvements to ground spawning logic
    * Fix: Group features a bit
    * Fix: Ground Late activating aircraft not working
    * Fix: Taskable aircraft take off right away on ground spawn
    * Fix: Aircraft refusing to take off
    * Fix: hopefully stop silly aircraft parking spawning (assuming data is ok)
    * Fix: aircraft spawning improvements
    * Fix: Helos loadouts where not editable
    * Fix: CAP and SEAD parked where late activation
    * Fix: A bunch of issues with aircraft parking and unit directions
    * Fix: Rename original situations
    * Fix: Reset default file again
    * Fix: Culture issue stopping certain Os setups from generating missions
    * Fix: Tell Scenic Ground Vehicles they can defend themselves
    * Fix: Extra aircraft show on flight list.
    * Fix: Convinced Admirals to position their carriers much closer to home base
    * Fix: Failing to find airbase had unusable error message.
    * Fix: Air and Sea units was able to ignore control zones
    * FIx: Scenery, JTAC was too danger close
    * FIx: SEAD and CAP not doing their roles
    * Fix: Parked Late Activation Aircraft not editable in ME
    * Fix: Missing Country Name Crashing Skynet Script
    * Fix: Laser Target Not existing
    * Fix: MarianaIslands badly named
    * Fix: Carrier Destinations on land.
    * Fix: Ground Start Aircraft and Spawn anywhere not mixing
    * Fix: Missing default mission features
    * Fix: GroundStart Tankers race-tracking right back to airbase
    * Fix: Build version path
    * Fix: Sub Objectives not loading right
    * Fix: Strike Packages not waypoints to with Sub Objectives
    * Fix: Close Air Support description
    * Fix: Player Aircraft with required runway length couldn't be generated.
    * Fix: Artillery asking for Zero units
    * Fix: UnitVehicleParked somehow was possible
    * Fix: Distance warning was wrong way round
    * Fix: Invert Coalitions wasn't reliable
    * Fix: Enemy Feature Units follow objective hidden
    * Fix: SubObjective can be on airbase if main objective is on airbase.
    * Fix: correct Al Maktoum Intl radios
    * Fix: UH-1H & OH-58D having no loadouts
  * UI
    * UI: General Sort out
    * UI: More improvement to Mission Features
    * UI: Mark player groups with (P) on flight list
    * UI: Re-label Air force to Combat air patrols.
    * UI: Add build version on index page
    * UI: Load Template is limited to expected file formats
    * UI: Missing Web Favicon
  * Misc
    * Misc: Properly set save and load file types
    * Misc: Add dependabot to automation
    * Misc: Newer Airbase Position extract method
    * Misc: Update Template extractor to do vehicles
    * Misc: Run code format pass and remove Summaries
    * Misc: Fix build issue
    * Misc: Switch to TemplateRecord in generator to avoid mutating template
    * Misc: Big code cleanup
    * Misc: remove pre-release from builds
    * Misc: Remove excess valid spawn types
    * Misc: Remove Disabled Units
    * Misc: Include Readme in download & Desktop warnings thanks - BlackRook
    * Misc: Try manual build version
    * Misc: Remove useless code.
    * Misc: Missing Bracket
    * Misc: auto lower case payload
    * Misc: Bump Microsoft.AspNetCore.Components.Web in /Source
    * Misc: another attempt at build version
    * Misc: Credit Walder for Skynet
    * Misc: Github Update Issue for new upload limits
* Version 0.5.111.18 (Nov, 9, 2021)
  * New spawning system:
    * Aircraft can spawn anywhere
    * Sea zoned out with island exclusions (not limited by spawn points, carriers affected too)
    * Blue and Red Areas of control (rest is neutral)
    * Spawn points don't have hardcoded side (checks run to see if side zones for limitation)
    * Insurgency mission option to allow spawning irrespective of side zoning (replaces "Only spawn in friendly countries")
    * Maps with added spawn points
      * Caucasus (Spawn points added to south and Mountains)
      * PersianGulf (Spawn points added for mountains, islands & cities)
      * Syria (Completely new set of spawn points ~200 => ~3700)
      * Marianas Islands (Completely new set of spawn points ~200 => ~900)
  * General spawning changes:
    * Objective distance is now between home base and objective no matter if first objective or not
    * Objective distance now on quick generator
    * Objective distance max 200 => 300
    * Objective separation option on Full and Quick generator
    * Make Medium air defences large space only
    * FOB as Carrier Option
    * Improve spawning of objective units
  * Map Changes:
    * Add Mariansas Islands
    * Add Syria Gaziantep Airbase
    * Fix Syria H4 airbase
  * Units:
    * Added NASAMS
    * Added Mosquito FB Mk VI
    * Added SA-5 site
    * Added H-6J Bomber
    * Added some missing ships
    * Add USS Forrestal
    * Add JAS-39-C Grippen Mod
  * Customisation:
    * Add custom db for liveries and loadouts
    * Add Implement custom loadout names
    * Add Implement basic custom loadouts
  * Add Campaign Beta (Don't report bugs on this please its too early)
  * Add Set aircraft liveries
  * Add Radio Transmission Objective Feature
  * Add Taskable Bomber, CAS, CAP & SEAD (Known issues with AI and callouts)
  * Add Implement Strike Packages setup
  * Add a ton of default liveries
  * Added UI Show warnings from generation (warnings may indicate some features requested could not be spawned)
  * Add Ingress and Egress waypoints optional
  * Add FX radio sound
  * Add Text to Template buttons
  * Add Custom Config Docs
  * Add colouring to map
  * Add drawings maker
  * Add Tarwa back in as usable by adding temp extra family
  * Add logs on all pages
  * Add more structures
  * Add embedded air defence for structures
  * Add Static groups
  * Add more target options
  * Add Manual Page
  * Add MarianaIslands images
  * Update to .Net 6 main release
  * Rename Packages => Strike Packages
  * Remove carrier waypoint faff
  * Fix Typehead removed scroll limit
  * Fix Template Load/Save improvements
  * Fix Scenery infantry is no longer immortal #253
  * Fix Improve Kneeboard Font
  * Fix Move awacs back towards base
  * Fix UI Improvements
  * Fix stop Custom config deleting default payload
  * Fix description error
  * Fix spawn selector for CAP
  * Fix Briefing Encoding
  * Fix MP script not loading
  * Fix Ships breaking mission
  * Fix to Various payloads not being editable in ME #168  (please report any remaining buggy aircraft)
  * Fix Load Template click issues
  * Fix Mark Waypoints #247  migrated to new mission option - default on
  * Fix multiple FOBS wouldn't spawn
  * Fix download UI issue
  * Fix Package Saving logic
  * Fix missing config folder in release
  * Fix Static Buildings not seen in JTAC and other marking features

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
