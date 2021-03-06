# BriefingRoom for DCS World - User's manual

This manual contains everything you need to know if you just plan to use BriefingRoom to generate missions. If you plan on modding BriefingRoom, please read the Modder's manual.

**This manual is not complete yet, additional information will be added in future revisions.**

## Table of contents
1. [Graphical user interface](#graphical-user-interface)
    1. [Mission template settings](#mission-template-settings)
1. [Command-line tool](#command-line-tool)
    1. [Command-line syntax](#command-line-syntax)
    1. [Using the command-line tool from the Windows file explorer](#using-the-command-line-tool-from-the-windows-file-explorer)

## Graphical user interface

When ran without any command-line parameters, BriefingRoom starts in GUI mode, which allows you to edit your mission parameters using an user-friendly interface, export mission to .miz directly or save your mission templates for later use, or to use them with the command-line tool (see below).

You can setup a default template by overwriting or editing the "Default.brt" template in the program directory, which is loaded each time you start Briefing Room.

### Mission template settings

On the left side of the screen you can see a list of all settings currently used by the mission template. 

**Briefing**
* **Mission date**: The exact date at which the mission should take place. If enabled, the "Season" setting from the "Environment" category will be ignored. If not enabled, a random year will be selected according to the selected coalitions.
* **Mission description**: Description of the mission to display in the briefing. If left empty, a random description will be generated.
* **Mission name**: Name/title of the mission. If left empty, a random name will be generated.
* **Unit system**: Unit system (metric or imperial) to use in the mission briefing.

**Coalitions**
* **Coalition, blue**: Who belongs to the blue coalition?
* **Coalition, red**: Who belongs to the red coalition?
* **Mission date**: During which decade will this mission take place? This value is ignored if "Briefing > Mission date" is set.
* **Player coalition**: Which coalition does the player(s) belong to?

**Environment**
* **Season**: Season during which the mission will take place.
* **Time of day**: Starting time of the mission.
* **Weather**: What the weather be like during the mission.
* **Wind**: How windy will the weather be during the mission. "Auto" means "choose according to the weather".

**Objectives**
* **Objective count**: How many objectives/targets will be present in the mission.
* **Objective distance**: How far from the player's starting location will the objectives be.
* **Objective type**: The type of task player must accomplish in this mission.

**Opposition**
* **Air defense**: Intensity and quality of enemy surface-to-air defense.
* **Air force**: Relative power of the enemy air force. Enemy air force will always be proportional to the number and air-to-air efficiency of aircraft in the player mission package, so more player/AI friendly aircraft means more enemy aircraft, regardless of this setting.
* **CAP on station**: Chance that enemy fighter planes will already be patrolling on mission start rather than popping up during the mission on objective completion.
* **Enemy units location**: Can enemy units be spawned in any country (recommended) or only in countries aligned with a given coalition? Be aware that when choosing an option other than "Any", depending on the theater and the "Theater region coalitions" setting, objectives may end up VERY far from the player(s) starting location, no matter the value of "Objective distance". Keep in mind that "Theaters regions coalitions" has a influence on this setting.
* **Skill level, aircraft**: Skill level of enemy planes and helicopters.
* **Skill level, ground units**: Skill level of enemy ground units and air defense.

**Options**
* **End mode**: When (and if) should the mission automatically end after all objectives are complete?
* **Preferences**: Preferences and options to apply to this mission.
* **Script extensions**: (advanced) Script extensions to include in this mission to provide additional features.
* **Unit mods**: Which unit mods should be enabled in this mission? Make sure units mods are installed and active in your version of DCS World or the units won't be spawned.

**Player**
* **AI CAP escort**: Number of AI aircraft tasked with escorting the player against enemy fighters. In single-player missions, escorts will be spawned on the ramp if the player starts from the ramp (cold or hot), or in the air above the airbase if the player starts on the runway. In multiplayer missions, escorts will be spawned as soon as one player takes off.
* **AI SEAD escort**: Number of AI aircraft tasked with escorting the player against enemy SAMs. In single-player missions, escorts will be spawned on the ramp if the player starts from the ramp (cold or hot), or in the air above the airbase if the player starts on the runway. In multiplayer missions, escorts will be spawned as soon as one player takes off.
* **AI skill level**: Skill level of AI wingmen and escort aircraft.
* **Air defense**: Intensity and quality of friendly air defense.
* **Carrier**: Type of aircraft carrier spawned. If none, player(s) will take off from an airbase. In single player, the player's aircraft will be spawed on the carrier. In multiplayer, aircraft will spawn on carrier if the option is set for their flight group. Make sure aircraft are suitable for the carrier type.
* **Start location**: Where should the player(s) take off from?

**Player, single-player only**
* **Aircraft**: Type of aircraft the player will fly. As with all values in the "Player, single player only" category, this value is ignored if any flight group is specified in "MP flight groups", the multiplayer flight groups are then used instead.
* **Wingmen**: Number of AI wingmen in the player's flight group. As with all values in the "Player, single player only" category, this value is ignored if any flight group is specified in "MP flight groups", the multiplayer flight groups are then used instead.

**Player, multiplayer only**
* **MP flight groups**: Multiplayer flight groups. If any flight group is specified here, the mission then becomes a multiplayer mission and all values in the "Player, single player only" are ignored.

**Theater**
* **Theater ID**: DCS World theater in which the mission will take place.
* **Theater regions coalitions**: To which coalitions should the countries on the map (and their airbases) belong to?
* **Theater starting airbase**: Name of the airbase the player must take off from. If left empty, or if the airbase doesn't exist in this theater, a random airbase will be selected. Be aware that if the selected airbase doesn't have enough parking spots for the player mission package, some units may not spawn properly.

## Command-line tool

BriefingRoom can also be used in command-line mode, to allow for the quick and easy generation of multiple missions.

### Command-line syntax

Syntax is:
**BriefingRoom.exe templateFile [anotherTemplateFile] [yetAnotherTemplateFile] [numberOfMissionsToGenerate]...**
where *templateFile*, *anotherTemplateFile*, etc are relative or absolute path to Briefing Room mission template files (with the extension .brTemplate) files which have previously saved in the GUI Interface and *numberOfMissionsToGenerate* is the number of .miz files to create from **each** template file.

Example:
**BriefingRoom.exe StrikeMissionTest.brTemplate "D:\My Mission Templates\Eagles over Caucasus.brTemplate" 4**
will generate 4 .miz files from StrikeMissionTest.brTemplate (located in the BriefingRoom.exe directory) and 4 .miz files from D:\My Mission Templates\Eagles over Caucasus.brTemplate.

Please note that:
* As always when using the Windows command prompt, filenames or paths with spaces must be enclosed in double-quotes
* Parameters can be in any order. If multiple numerical values are provided as parameters, the last one will determine how many missions should be generated.
* If no numerical value is provided, one mission will be generated from each template.

### Using the command-line tool from the Windows file explorer

If you don't know how to (or don't want to) open a Windows command prompt, you can just drag-and-drop one or more .brTemplate files on BriefingRoom.exe in the Windows file explorer and it will generate one .miz file from each template. This can be useful to quickly generate a few missions from templates you like without having to spend time in the graphical user interface.
