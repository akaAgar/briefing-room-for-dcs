![BriefingRoom logo](Media/SplashScreen.png)

----
## Installation
 App is generally standalone .exe with caveats on desktop:
 1. Desktop version requires [WebView2 Runtime](https://go.microsoft.com/fwlink/p/?LinkId=2124703) this may not be installed on windows by default. If Web Exe works and Desktop doesn't this is likely the issue.
 1. Desktop won't run if placed within `Program Files` or `Program Files (x86)` (Its a Microsoft issue not us :D)


## What is BriefingRoom for DCS World?

BriefingRoom for DCS World is a massively improved version of DCS World's own fast mission generator. BriefingRoom allows you to create complete scenarios in just a few clicks. Missions generated with BriefingRoom tend to be rather "player-centric". Their purpose is not to simulate a real war theater, like Falcon 4's dynamic campaign, but to provide the player(s) with interesting objectives and challenges. This approach is a perfect fit for DCS World's limited number of units and very capable scripting engine.

While BriefingRoom is designed to be easy to use and to allow the creation of missions in mere seconds, it is also heavily moddable and will give many options to power users ready to tackle with its most advanced features.

**Not interested in the source code? Get the latest version from the project website and begin creating missions right now: [akaagar.itch.io/briefing-room-for-dcs](https://akaagar.itch.io/briefing-room-for-dcs/)**

BriefingRoom is free and open source and will always remain so, but if you want to support its development, you can make a one-time or monthly donation.

[![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=VTLATJ7URMMWY)

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
* No units spawned through runtime scripting. All units are added to the mission itself, so they can be edited with DCS World's mission editor for further customization.
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
* App makes use of [wkhtmltoimage](https://wkhtmltopdf.org/) and uses a wrapper heavily based on [flopik3-5 WkHtmlWrapper](https://github.com/flopik3-5/WkHtmlWrapper)

[Credits](Include/Markdown/Manuals/Credits.md)

### Planned for future versions

The development roadmap and a list of features planned for future versions can be found on Trello: [trello.com/b/iGsqgbTu/briefingroom-project-tracker](https://trello.com/b/iGsqgbTu/briefingroom-project-tracker)

## Changelog

(Changelog for older versions can be found in Changelog.md)

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