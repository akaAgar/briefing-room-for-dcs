![BriefingRoom logo](Media/SplashScreen.png)

----
**Try the <a href="https://dcs-briefingroom.com/">Web version</a> now!**
## Installation
 App is generally standalone .exe with caveats on desktop:
 1. Desktop version requires [WebView2 Runtime](https://go.microsoft.com/fwlink/p/?linkid=2124703) this may not be installed on windows by default. If Web Exe works and Desktop doesn't this is likely the issue.
 1. Desktop won't run if placed within `Program Files` or `Program Files (x86)` (Its a Microsoft issue not us :D)


## What is BriefingRoom for DCS World?

BriefingRoom for DCS World is a massively improved version of DCS World's own fast mission generator. BriefingRoom allows you to create complete scenarios in just a few clicks. Missions generated with BriefingRoom tend to be rather "player-centric". Their purpose is not to simulate a real war theater, like Falcon 4's dynamic campaign, but to provide the player(s) with interesting objectives and challenges. This approach is a perfect fit for DCS World's limited number of units and very capable scripting engine.

While BriefingRoom is designed to be easy to use and to allow the creation of missions in mere seconds, it is also heavily moddable and will give many options to power users ready to tackle with its most advanced features.

**Not interested in the source code? Get the latest version from the project website and begin creating missions right now: [akaagar.itch.io/briefing-room-for-dcs](https://akaagar.itch.io/briefing-room-for-dcs/)**

BriefingRoom is free and open source and will always remain so, but if you want to support its development, you can make a one-time or monthly donation.

Hosting costs:[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/V7V0IZI9N) or <a href="https://www.patreon.com/bePatron?u=99514930" data-patreon-widget-type="become-patron-button">Become a member!</a><script async src="https://c6.patreon.com/becomePatronButton.bundle.js"></script>

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

    docker run --rm -it -p 5000:80 -e ASPNETCORE_HTTP_PORTS=80  johnharvey/dcs-briefing-room-web:latest


## Changelog

(Changelog for older versions can be found in Changelog.md)

* Version 0.5.403.05(March, 05, 2024)
    * Added:
        * Added: 3 more Syria Situations
        * Added: Hide Anti-air on MFD (Mission Option)
        * Added: Re-implemented Default units (will warn when used)
        * Added: Moving Ground Vehicle formation variance
        * Added: No Aircraft Waypoint objective option
        * Added: Progression hide waypoint map markers if hidden in brief
        * Added: Progression hide objectives in F-10 menu if hidden in brief
        * Added: Strike Packages now selectable per Objective no just per Objective Area.
        * Added: Bundle Objective Activation/Reveal
        * Added: Objective Progression Options: Delay Activation, Visible Before Activation and Hidden in Brief
        * Added: Ambient AAA and Artillery firing (Results may vary may need manual tuning)
        * Added: Currenthill Asset packs
        * Added: Brazilian Portuguese translation
    *  Updated:
        * Updated: PoEditor translated terms
        * Updated: Support Translations for Warnings
        * Updated: AirDefenses use fallback mechanism rather than default units
        * Updated: Existing Syria Situations
        * Updated: poeditor.com project links
        * Updated: Syria spawn points
        * Updated: DCS data to 2.9.3.5
        * Updated: Error Translations
        * Updated: Use new carrier storage capacity over outdated parking spots
        * Updated: Static carrier aircraft groups max 8
        * Updated: Made many carriers also ShipCarrierSTOVL type
        * Updated: EWR air defense can now use Large Spawn points
        * Updated: Moose to 2.9.4
        * Updated: MIST to 4.5.122
        * Updated: Coalitions to add more ally opportunities
        * Updated: Docs
        * Updated: VehicleAny spawns mix of vehicle types
        * Updated: Spawning logic using front line
        * Updated: Use KD tree for faster spawn selection
        * Updated: Use Backtracking logic for generation
        * Updated: Reduce FOB and Carrier spawn max distance (FOB aims to 30-50% of usual distance)
        * Updated: More fine grained spawn point search expansion with less iterations (Max ~ 120% expansion, prev ~2300%)
        * Updated: Narrow Flanking logic to max 1/3 original distance
        * Updated: Disable AI extra waypoints if going via Roads
        * Updated: Embed image data rather than reference in HTML files for images
        * Updated: Mission Title and Campaign Images to be HTML based (So anyone can improve it)
        * Updated: Current Hill asset pack operator lists
        * Updated: DCS Plane data
        * Updated: High Digit SAMs Mod data
        * Updated: SU-30MK Flanker Mod data
        * Updated: Bronco Mod data
        * Updated: A-29B Mod Data
        * Updated: Skynet Script to 3.3.0
        * Updated: Demote a bunch of vehicles to low polly
    * Fixed:
        * Fix: Spawn Anywhere didn't work for campaigns
        * Fix: Translation bugs
        * Fix: Handle multi DCS unit category error to remove error need
        * Fix: Templates receiving bad operational data
        * Fix: Dupe Airfield
        * Fix: increase chances of avoiding default units in mission features
        * Fix: Carrier AI spawning was disabled for some reason
        * Fix: Campaign zips where corrupt
        * FIx: Imported loadouts throwing error
        * Fix: User payloads not being imported
        * Fix: Remove log duplicating
        * Fix: Mission Fallback State not dealing with airbase ParkingSpots correctly
        * Fix: CommandLine Build
        * Fix: AShM SS-N-2 Silkworm not marked as static
        * Fix: Units spawning as neutral if from ally sources
        * Fix: Enemy buildings spawning on friendly side
        * Fix: error on artillery with no situation
        * Fix: Low Res aircraft switch not working
        * Fix: infinite loops possibility
        * Fix: Null error when carrier couldn't find spawn point
        * Fix: Zones Rendering incorrectly in DCS
        * Fix: Map UI error
        * Fix: Don't script mark waypoints if mark waypoints off
        * Fix: Missing script for revealing hidden waypoint markers
        * Fix: bad location script function
        * Fix: Offset waypoint marker and make more translucent
        * Fix: Image generator failed causing no saving
        * Fix: Strike Groups couldn't deal with multi waypoint objectives
        * Fix: download failing if taking too long (fall back to simple download)
        * Fix: Campaign spinner not working
        * Fix: Nullable error
        * Fix: Extra nations not supported by ED
        * Fix: Better data error logs
        * Fix: Incomplete translation causing scripting error
    * UI
        * UI: Translation setup for as many errors as possible
        * UI: Translate Errors from generator
        * UI: Better format logs
        * UI: Fix manual reference links
        * UI: Added tooltips to map buttons
        * UI: Hide tool tip if none
    * Upgraded:
        * Upgrade: Bump BlazorMonaco from 3.1.0 to 3.2.0 in /Source
        * Upgrade: Bump coverlet.collector from 6.0.0 to 6.0.1 in /Source
        * Upgrade: Bump Markdig from 0.34.0 to 0.35.0 in /Source
        * Upgrade: Bump Microsoft.AspNetCore.Components.Web in /Source
        * Upgrade: Bump Microsoft.Maui.Graphics from 8.0.6 to 8.0.7 in /Source
        * Upgrade: Bump Microsoft.NET.Test.Sdk from 17.8.0 to 17.9.0 in /Source
        * Upgrade: Bump Polly from 8.2.1 to 8.3.0 in /Source
        * Upgrade: Bump System.Drawing.Common from 8.0.0 to 8.0.2 in /Source
        * Upgrade: Bump xunit from 2.6.6 to 2.7.0 in /Source
        * Upgrade: Bump xunit.runner.visualstudio from 2.5.6 to 2.5.7 in /Source
        * Upgrade: Bump YamlDotNet from 15.1.0 to 15.1.1 in /Source
        * Upgrade: Leaflet JS to 1.9.4
    * Misc:
        * Misc: GetAirbases Optimization
        * Misc: Make fallback failure error less scary
        * Misc: Improve spawn error to hint and looking for erroneous hints
        * Misc: File formatting




