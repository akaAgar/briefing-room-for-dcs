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

### Additional information

* Created by Ambroise Garel (@akaAgar), maintained by John Harvey (@john681611)
* Released under the [GNU General Public License 3.0](https://www.gnu.org/licenses/gpl-3.0.en.html)
* Project website: [akaagar.itch.io/briefing-room-for-dcs](https://akaagar.itch.io/briefing-room-for-dcs/)
* Source code repository: [github.com/akaAgar/briefing-room-for-dcs](https://github.com/akaAgar/briefing-room-for-dcs)

### Planned for future versions

The development roadmap and a list of features planned for future versions can be found on Trello: [trello.com/b/iGsqgbTu/briefingroom-project-tracker](https://trello.com/b/iGsqgbTu/briefingroom-project-tracker)

## Changelog

(Changelog for older versions can be found in Changelog.md or Changelog.html)

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
