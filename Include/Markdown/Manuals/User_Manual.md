# BriefingRoom for DCS World - User's manual
__Last Edited: 28/10/2021__

This manual contains everything you need to know if you just plan to use BriefingRoom to generate missions. If you plan on modding BriefingRoom, please read the Modder's manual.

**This manual is not complete yet, additional information will be added in future revisions.**

## Table of contents
1. [Graphical user interface](#graphical-user-interface)
    1. [Quick Builder](#quick-builder)
    1. [Full Builder](#full-builder)
    1. [Campaign Builder](#campaign-builder)
1. [Command-line tool](#command-line-tool)
    1. [Command-line syntax](#command-line-syntax)
    1. [Using the command-line tool from the Windows file explorer](#using-the-command-line-tool-from-the-windows-file-explorer)

## Graphical user interface

Comes in 2 forms Desktop and Web. Most use desktop but if you have issues its recommended to try Web. Everything is tested with Chrome so odd browsers may have issues (that can show on desktop too its a browser underneath)

There are 3 ways of generating missions:
* Quick Builder
* Full Builder
* Campaign Builder (BETA)

The basics are:
* **Lots of options** Check Full Builder section fo descriptions of each
* **Templates** saving/loading settings for the tool always on the upper right. Reset returns it to default file (you can overwrite that file)
* **Generate Button** Generates missions/campings.
* **Mission Download** as .Miz file (Save window should pop up)
* **Briefing Download** as .html file (Save window should pop up)
* **Warnings** Errors occurred in generator that mean mission may be lacking requested features
* **Clear Mission Button** clears the mission for full screen options again.

**Note**: we do not generate missions for maximum realism we generally try and strike a balance between realism and game play this means we do expect you to have to make changes in the ME if you require a particular setup.


### Quick Builder

The quick builder is a very cut down version of the Full Builder as is intended to be used in conjunction with a template created in the full builder. Think of it as *"I liked that mission I want similar mission".* It uses the same generator behind the scenes as the full builder so any options you can't see will be set when you load your template.

Its all on one page and as stated you only see a subset of the full options. The only major difference is the Objectives which instead of using the full options list it uses Presets configured in the Database (see Modder Manual if you want to adjust these)

### Full Builder

Hold all possible options for generating a single mission. Options are split into tabs in which we will go through each one.

#### Context
`TDLR: Who, When, Where`

**Blue/Red Coalitions (Type to search)**: Primary faction on each side (other factions may spawn)

**Time Period**: Decade that mission takes place

**Player Side**: Blue/Red

**Theater**: Map

**Home Airbase**: Where is the main airbase for the mission? (objectives and other stuff uses this base to calculate distances)

#### Environment
`TDLR: Whats the weather do I need NVGs?`

**Season**: General time of year

**Time Of Day**: Start time of the mission

**Weather Preset**: Pretty Pretty clouds preset

**Wind**: Force scale. Do I need to land sideways?

#### Objectives
`TDLR: What am I to do?`

**Objective Distance (Nautical Miles)**: How far is the objective from the home base? (certain variation applied)

**Objective Separation (Nautical Miles)**: How far between objectives?

Set of tabs for ach objective can duplicate if you like

**Task**: Generally Blow it all up, Disable the Radars, Fly over (recon) or land near it

**Target**: What kinda stuff is there, ships, vehicles, planes, infantry, Daves house.

**Target Behavior**: What is the target doing? Idle, Advancing on you, patrolling ect.

**Target Count**: Generally how many targets are there, more players/capable aircraft you have add more targets

**Objective Features**: Designation items through F-10 radio options on each objective, Scenery items ect.

**Objective Options**: Can it have AA, Is the waypoint accurate (imagine known area objective)

#### Flight Groups
`TDLR: What am I to Fly`

Set of tabs for each flyable set of aircraft.

**Aircraft (Type to search)**: What model?

**Count**: How many Max 4.

**Payload**: What weapons (extra options addable through custom configs)?

**Starting Setup**: Runway, Parking Hot or Cold.

**Carrier**: Aircraft Carriers or FARPS (Note you can't have more than one of the same carrier it will just put more than one group on the same carrier) N/A = spawn at airbase

**Country**: Where is the aircraft from?

**Livery**: What paint job do you have (extra options addable through custom configs)?

**AI wingmen**: If on only first unit will be playable rest will be AI.


#### Strike Packages
`TDLR: How do you breakdown the work in MP`

Only usable if you have more than one Flight group. Set of tabs to group flights up to attack specific objectives

**Objectives**: What are you doing in this group (sorts out waypoints)?

**Flight Groups**: Who is involved?

**Starting Base**: Custom starting base overriding the general home base.

#### Situation
`TDLR: How strong are us/them?`

Two tabs containing the same options for Enemies and Friendlies

**Air Defense**: How strong is AA systems?

**Air Force**: How many aircraft are they likely to send?

#### Options
`TDLR: What else?`

**Fog Of war**: F-10 Map settings and Labels (DCS setting)

**Mission Features**: Various options of what is going on. Hover to see extra info

**Mission Options**: Options around spawning logic, areas used to spawn, Waypoint markers and audio messages

**Realism Options**: DCS mission options about realism

**Unit Mods**: What extra modded units do you want to spawn.


### Campaign Builder (Beta)
`TDLR: No its not dynamic look at Liberation for that`

Builds campaigns in a simplistic way. More accurately it generates a bunch of missions in a campaign format based off the same settings.

Many options are shared with the Full Builder but some are more orientated around progression and multi missions. 

Standout options are:

**Environment**: Uses Bad Weather and Night chance rather than detailed weather.

**Missions**: How many missions what features, Difficulty variation, Objectives (based of presets), per mission objective count and distance.

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
