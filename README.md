![BriefingRoom logo](Media/SplashScreen.png)

----

## What is BriefingRoom for DCS World?
BriefingRoom for DCS World is a massively improved version of DCS World's own fast mission generator. BriefingRoom allows you to create complete scenarios in just a few clicks. Missions generated with BriefingRoom tend to be rather "player-centric". Their purpose is not to simulate a real war theater, like Falcon 4's dynamic campaign, but to provide the player(s) with interesting objectives and challenges. This approach is a perfect fit for DCS World's limited number of units and very capable scripting engine.

While BriefingRoom is designed to be easy to use and to allow the creation of missions in mere seconds, it is also heavily moddable and will give many options to power users ready to tackle with its most advanced features.

**Not interested in the source code? Get the latest version from the project website and begin creating missions right now: [akaagar.itch.io/briefing-room-for-dcs](https://akaagar.itch.io/briefing-room-for-dcs/)**

BriefingRoom is free and open source and will always remain so, but if you want to support its development, you can make a one-time or monthly donation.

[![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=VTLATJ7URMMWY)

### Features summary

* Can generate any kind of single-player or multiplayer mission, from deep strikes behind a hostile superpower's cutting-edge integrated air defense network to photo reconnaissance flights over a terrorist training camp.
* Choose from a large variety of mission types, from combat air patrols to bomber interception, target designation for artillery strikes, photo reconnaissance, bomb damage assessment and many others...
* Extremely easy to use: generate missions in seconds without any technical knowledge, then export them to a DCS World .MIZ file in just a few clicks.
* Automatically generates proper friendly/enemy units according to countries and time period, but also able to create "what if" scenarios (2010s USAF vs 1940s Luftwaffe) and freely choose player aircraft in any country/time period.
* Generated mission can include custom scripts for various effects (advanced SAM AI, JTAC smoke and laser designation, artillery strikes...)
* Save mission templates to small .ini files and share them with your friends.
* Easily moddable to add more countries, units and scenarios
* Available as both a graphical user interface for ease of use and as a command-line tool for batch mission generation
* Customize enemy SAM and air-to-air opposition for any mission difficulty.
* No units spawned through runtime scripting. All units are added to the mission itself, so they can be edited with DCS World's mission editor for further customization.
* **Please read the user's manual and the modder's manual for more information**

### Additional information

* Created by Ambroise Garel (@akaAgar), with additional code by John Harvey (@john681611)
* Released under the [GNU General Public License 3.0](https://www.gnu.org/licenses/gpl-3.0.en.html)
* Project website: [akaagar.itch.io/briefing-room-for-dcs](https://akaagar.itch.io/briefing-room-for-dcs/)
* Source code repository: [github.com/akaAgar/briefing-room-for-dcs](https://github.com/akaAgar/briefing-room-for-dcs)
* Uses the Silk icon set [famfamfam.com/lab/icons/silk/](https://famfamfam.com/lab/icons/silk/)
* Requires the .NET Framework 4.8 (already installed on modern Windows systems) [dotnet.microsoft.com/download/dotnet-framework/thank-you/net48-web-installer](https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net48-web-installer)

### Planned for future versions

The development roadmap and a list of features planned for future versions can be found on Trello: [trello.com/b/iGsqgbTu/briefingroom-project-tracker](https://trello.com/b/iGsqgbTu/briefingroom-project-tracker)

## Changelog

(Changelog for older versions can be found in Changelog.md or Changelog.html)

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
