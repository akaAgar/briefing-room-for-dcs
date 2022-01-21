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

### Additional information

* Created by Ambroise Garel (@akaAgar), maintained by John Harvey (@john681611)
* Released under the [GNU General Public License 3.0](https://www.gnu.org/licenses/gpl-3.0.en.html)
* Project website: [akaagar.itch.io/briefing-room-for-dcs](https://akaagar.itch.io/briefing-room-for-dcs/)
* Source code repository: [github.com/akaAgar/briefing-room-for-dcs](https://github.com/akaAgar/briefing-room-for-dcs)

[Credits](Include/Markdown/Manuals/Credits.md)

### Planned for future versions

The development roadmap and a list of features planned for future versions can be found on Trello: [trello.com/b/iGsqgbTu/briefingroom-project-tracker](https://trello.com/b/iGsqgbTu/briefingroom-project-tracker)

## Changelog

(Changelog for older versions can be found in Changelog.md)

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
