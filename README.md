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
* Updating vehicle/unit operators
* Keeping map data upto date
* Creation of coalitions
* Creation of map situations


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