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

### Additional information

* Created by Ambroise Garel (@akaAgar), with additional code by John Harvey (@john681611)
* Released under the [GNU General Public License 3.0](https://www.gnu.org/licenses/gpl-3.0.en.html)
* Project website: [akaagar.itch.io/briefing-room-for-dcs](https://akaagar.itch.io/briefing-room-for-dcs/)
* Source code repository: [github.com/akaAgar/briefing-room-for-dcs](https://github.com/akaAgar/briefing-room-for-dcs)
* Requires the .NET Framework 4.8 (already installed on modern Windows systems) [dotnet.microsoft.com/download/dotnet-framework/thank-you/net48-web-installer](https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net48-web-installer)
* Desktop version requires [WebView2 Runtime](https://go.microsoft.com/fwlink/p/?LinkId=2124703) and .NET Framework 6.0 (packaged with the release)

### Planned for future versions

The development roadmap and a list of features planned for future versions can be found on Trello: [trello.com/b/iGsqgbTu/briefingroom-project-tracker](https://trello.com/b/iGsqgbTu/briefingroom-project-tracker)

## Changelog

(Changelog for older versions can be found in Changelog.md or Changelog.html)

* New mission features: frontline friendly units, frontline enemy units
* New objective feature: target emits radio transmission
* New units: NASAMS (AIM-120C) air defense system
* New DCS World mods supported: C-130J, F-117A, F-35A/B/C (planned for mod release), French Pack
* Added check for non-replaced values in the .miz Lua files 
* Fixed briefing HTML encoding (now UTF-8) (by John Harvey)
