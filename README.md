![BriefingRoom logo](Media/SplashScreen.png)

![Discord Banner 2](https://discordapp.com/api/guilds/809863694678491157/widget.png?style=banner2)
----
## Installation
 App is generally standalone .exe with caveats on desktop:
 1. Desktop version requires [WebView2 Runtime](https://go.microsoft.com/fwlink/p/?LinkId=) this may not be installed on windows by default. If Web Exe works and Desktop doesn't this is likely the issue.
 1. Desktop won't run if placed within `Program Files` or `Program Files (x86)` (Its a Microsoft issue not us :D)


## What is BriefingRoom for DCS World?

BriefingRoom for DCS World is a massively improved version of DCS World's own fast mission generator. BriefingRoom allows you to create complete scenarios in just a few clicks. Missions generated with BriefingRoom tend to be rather "player-centric". Their purpose is not to simulate a real war theater, like Falcon 4's dynamic campaign, but to provide the player(s) with interesting objectives and challenges. This approach is a perfect fit for DCS World's limited number of units and very capable scripting engine.

While BriefingRoom is designed to be easy to use and to allow the creation of missions in mere seconds, it is also heavily moddable and will give many options to power users ready to tackle with its most advanced features.

**Not interested in the source code? Get the latest version from the project website and begin creating missions right now: [akaagar.itch.io/briefing-room-for-dcs](https://akaagar.itch.io/briefing-room-for-dcs/)**

BriefingRoom is free and open source and will always remain so, but if you want to support its development, you can make a one-time or monthly donation.

Hosting costs:[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/V7V0IZI9N)

Original Creator: [![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=VTLATJ7URMMWY)

### Features summary

* Can generate any kind of single-player or multiplayer mission, from deep strikes behind a hostile superpower's cutting-edge integrated air defense network to photo reconnaissance flights over a terrorist training camp.
* Choose from a large variety of mission types, from combat air patrols to bomber interception, target designation for artillery strikes, photo reconnaissance, bomb damage assessment and many others, or create your own by picking target types, players tasking, and various objective and mission features.
* Extremely easy to use: generate missions in seconds without any technical knowledge, then export them to a DCS World .MIZ file in just a few clicks.
* Automatically generates proper friendly/enemy units according to countries and time period, but also able to create "what if" scenarios (2010s USAF vs 1940s Luftwaffe) and freely choose player aircraft in any country/time period.
* Generated mission can include custom scripts for various effects (advanced SAM AI, JTAC smoke and laser designation, artillery strikes...)
* Save mission templates to small template files and share them with your friends.
* Easily moddable to add more countries, units and scenarios
* Available as both a graphical user interface for ease of use and as a command-line tool for batch mission generation
* Customize enemy SAM and air-to-air opposition for any mission difficulty.
* No units spawned through runtime scripting. All units are  to the mission itself, so they can be edited with DCS World's mission editor for further customization.
* **Please read the user's manual and the modder's manual for more information**

### Included Scripted systems (Thanks to them)

* Skynet IADS by walder
* Splash Damage 2.9 by spencershepard (GRIMM)
* CSAR (ciribob)
* CTLD (ciribob)

### Call for Contributions

We are always looking for contributors to keep us up to date with the latest DCS content. You don't need any technical skill and we are always happy to help. Areas we are looking for help to improve:

* Aircraft Loadouts
* Implementing Mod Units
* Write better descriptions and briefing parts
* Updating vehicle/unit operators & liveries
* Keeping map data upto date
* Creation of coalitions
* Creation of map situations

### Kneeboard Image errors

The generation of Kneeboard images has been prone to issues. While we aim to fix as many as possible odd issues get through. Check or try some of these previous solutions:

* Check your running the latest version from github. Try the latest beta.
* Use Web version to see more detailed error message in the console terminal.
* Check the image.win.exe file is in the right location.
* Turkish OS users (and others who use `ı`) rename file from `image.win.exe` to `ımage.win.exe`

### Additional information

* Created by Ambroise Garel (@akaAgar), maintained by John Harvey (@john681611)
* Released under the [GNU General Public License 3.0](https://www.gnu.org/licenses/gpl-3.0.en.html)
* Project website: [akaagar.itch.io/briefing-room-for-dcs](https://akaagar.itch.io/briefing-room-for-dcs/)
* Source code repository: [github.com/akaAgar/briefing-room-for-dcs](https://github.com/akaAgar/briefing-room-for-dcs)

[Credits](Include/Markdown/Manuals/Credits.md)

### Docker
Build

    docker build -t johnharvey/dcs-briefing-room-web:latest .

Run responding on port localhost:5000 

    docker run --rm -it -p 5000:80  johnharvey/dcs-briefing-room-web:latest


## Changelog

(Changelog for older versions can be found in Changelog.md)

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