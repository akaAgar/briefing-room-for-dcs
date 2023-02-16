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

[Credits](Include/Markdown/Manuals/Credits.md)

### Docker
Build

    docker build -t johnharvey/dcs-briefing-room-web:latest .

Run responding on port localhost:5000 

    docker run --rm -it -p 5000:80  johnharvey/dcs-briefing-room-web:latest


## Changelog

(Changelog for older versions can be found in Changelog.md)

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