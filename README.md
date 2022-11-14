![BriefingRoom logo](Media/SplashScreen.png)

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
* App makes use of [wkhtmltoimage](https://wkhtmltopdf.org/) and uses a wrapper heavily based on [flopik3-5 WkHtmlWrapper](https://github.com/flopik3-5/WkHtmlWrapper)settings

[Credits](Include/Markdown/Manuals/Credits.md)

### Planned for future versions

The development roadmap and a list of features planned for future versions can be found on Trello: [trello.com/b/iGsqgbTu/briefingroom-project-tracker](https://trello.com/b/iGsqgbTu/briefingroom-project-tracker)

## Changelog

(Changelog for older versions can be found in Changelog.md)

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
