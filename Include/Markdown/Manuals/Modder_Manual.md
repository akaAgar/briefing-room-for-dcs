# BriefingRoom for DCS World - User's manual
__Last Edited: 12/02/2022__

This manual contains everything you need to know in order to modify BriefingRoom to your needs [for Aircraft Payloads and Liveries use Custom Configs](Custom_Configs.md).
If you only want to generate missions, please read the User's manual.

**This manual is not complete detail as code is often changing. This should give you an idea about what is where. Some folders will have more detail than others**

## Table of contents
1. [Briefing Text](#briefing-text)
1. [Database directory](#database-directory)
    1. [Database/Common.ini](#databasecommonini)
    1. [Database/EnemyAirDefense.ini](#databaseenemyairdefenseini)
    1. [Database/Names.ini](#databasenamesini)
    1. [Database/Objectives.ini](#databaseobjectivesini)
    1. [Database/Coalitions directory](#databasecoalitions-directory)
    1. [Database/Extensions directory](#databaseextensions-directory)
    1. [Database/MissionFeatures directory](#databasemissionfeatures-directory)
    1. [Database/ObjectiveFeatures directory](#databaseobjectivefeatures-directory)
    1. [Database/ObjectiveTargets directory](#databaseobjectivetargets-directory)
    1. [Database/ObjectiveTargetBehaviors directory](#databaseobjectivetargetbehaviors-directory)
    1. [Database/ObjectiveTasks directory](#databaseobjectivetasks-directory)
    1. [Database/ObjectivePresets directory](#databaseobjectivepresets-directory)
    1. [Database/Theaters directory](#databasetheaters-directory)
    1. [Database/Situations directory](#databasesituations-directory)
    1. [Database/TheatersAirbases directory](#databasetheatersairbases-directory)
    1. [Database/WeatherPresets directory](#databaseweatherpresets-directory)
    1. [Database/Units directory](#databaseunits-directory)
1. [Include directory](#include-directory)
    1. [Include/Html](#includehtml)
    1. [Include/Jpg directory](#includejpg-directory)
    1. [Include/Lua directory](#includelua-directory)
    1. [Include/Ogg directory](#includeogg-directory)
    1. [Include/Markdown/Manuals directory](#includemarkdownmanuals-directory)

## Briefing Text

Throughout the Database you will come across parts of briefings that are used to generate the briefing text within BR. If your interested in writing your own or improving what is there and contributing there is a few things you should know.

1. Due to INI limitations you have to have everything on one line.
1. Some Briefings allow multipule descriptions
    1. `BriefingDescriptions` are orientated by task and unit family of the objective.
    1. `TheatreSituation` allow multiple descriptions to be randomly selected (easier to edit)
1. Variety is created by setting randomly selected options with formatting like so `There is a {large|guerrilla|significant gathering of much|} force.` you can use as many options as you like but be careful about spaces. This can render to:
    1. There is a large force.
    1. There is a guerrilla force.
    1. There is a significant gathering of much force.
    1. There is a force.
1. Values from within the generator can also be replaced in eg. `$BRIEFINGENEMYCOALITION$` will replace for the name of the enemy coalition. There are many of these values and I suggest digging around the `include/html` files to see what is possible.

## Database directory

The database directory contains a series of .ini files used to describe almost everything in BriefingRoom, from the available units to the various mission types.

### Database/Common.ini

* ***[Include]* section**
  * **CommonOgg**: Comma-separated list of ogg files (read from the Include/Ogg directory) to be included in all missions, WITHOUT THE .OGG EXTENSION.

* ***[Versions]* section**
  * **DCSVersion**: Targeted DCS World version. Just for informational purpose, BriefingRoom should work with another version provided Eagle Dynamics made no major changes to the mission format.


### Database/EnemyAirDefense.ini

Configuration for Air Defense (SAMs,AAA)

* ***[AirDefense]* section**

  \<ChanceSetting>.Embedded.Chance=\<chance of 100>

  \<ChanceSetting>.Embedded.UnitCount=\<min>,\<max>

  \<ChanceSetting>.GroupsInArea.ShortRange=\<min>,\<max>

  \<ChanceSetting>.GroupsInArea.MediumRange=\<min>,\<max>

  \<ChanceSetting>.GroupsInArea.LongRange=\<min>,\<max>

  \<ChanceSetting>.SkillLevel=\<Skill Setting>

* ***[AirDefenseRange.Ally/Enemy]* section**
  For allies, center is initial airbase, opposing point is objectives
  For enemies, center is objectives, opposing point is initial airbase

  ShortRange.DistanceFromCenter=\<min Nm>,\<max Nm>

  ShortRange.MinDistanceFromOpposingPoint=\<distance Nm>

  MediumRange.DistanceFromCenter=\<min Nm>,\<max Nm>

  MediumRange.MinDistanceFromOpposingPoint=\<distance Nm>

  LongRange.DistanceFromCenter=\<min Nm>,\<max Nm>

  LongRange.MinDistanceFromOpposingPoint=\<distance Nm>

### Database/Names.ini

This files stores information about names and string constants used by the mission generation.

* ***[UnitGroup]* section**
Stores random names to be used for unit groups, by unit family. Helicopter and plane groups won't use these as they use callsigns instead, but they're included nevertheless.
  * $NTH$ is replaced with an ordinal adjective based on the group number.
  * $N$ is replaced with a number number based on the group number.

### Database/Coalitions directory
Configuration of nations/coalitions that are available.
Algeria.ini Example(03/21)

    [Briefing]
    Elements.Adjective=Algerian
    Elements.Recon=friendly agents,local resistance troops,reconnaissance units,special forces
    Elements.StrategicCommand=Algiers
    Elements.TacticalCommand=Central command
    Elements.TheCoalition=the Algerians

    [Coalition]
    Countries=All,Algeria
    DefaultUnitList=ThirdWorld
    NATOCallsigns=false

[Briefing] sections helps generate realistic sounding documents and is relatively self explanatory `Recon` is a comma separated list of options so if you know of specific groups for recon you can add them.

[Coalition] Defines details about the nation or coalition as in what game countries are included (nations just contain their own country). If they are NATO affiliated then do they use NATO callsigns. And what kinda grade units would they have (Used in the event a type of unit is needed but no units of that type are configured under the nation. ie what they could purchase)


### Database/MissionFeatures directory
Common Features/Script configs needed for objectives
* Remarks
* Lua Script files needed
* Ogg Sound Files needed
* Specific Lua Script Settings
* Units needed

### Database/ObjectiveFeatures directory
All Features you can add to a single Objective
* GUI
* Remarks
* Lua Script files needed
* Ogg Sound Files needed
* Specific Lua Script Settings
* Units needed

### Database/ObjectiveTargets directory
Type Of targets for an objective
* GUI
* Briefing Name (single and plural)
* Unit Families (Group of units to be selected)
* Units.Count.\<amount>=\<min>,\<max>
* ValidSpawnPoints (Size/Type of spawn points)

### Database/ObjectiveTargetsBehaviors directory
What your target will be doing
* GUI
* Location (Where to start or what to do)
* Lua Group and unit custom lua to use

### Database/ObjectiveTasks directory
What to do with your target
* GUI
* Briefing Info
* Remarks
* Trigger Lua
* Target Side
* Valid targets
* Ogg files to include

### Database/ObjectivePresets directory
Pre Made Objectives for Quick Builder
* GUI
* Options (Objective features)
* Targets (Objective Targets)
* TargetsBehaviors (Objective Target Behaviours)
* Tasks (Objective Task)

### Database/Theaters directory
Map Configuration covers
* Map DCS config
* Local Daylight
* Temps

Following recommended to use `dataExtractors\ZoneExtractor.lua` and example mission under `dataExtractors\PositioningMissions` to generate
* Spawn points
* Water Area
* Water Exclusion Areas (Islands)


### Database/Situations directory
A Situation is red and blue control areas.
Its  recommended to use `dataExtractors\ZoneExtractor.lua` using a mission that has one Red Side polygon and one Blue side Polygon
* Red Control Area
* Blue Control Area

### Database/TheaterAirbases directory
Configuration of airbases (\<Theatre>\<AirbaseName>)
* General Airbase config
* Runway Spawn points
* Parking Spawn Points

### Database/WeatherPresets directory
Configuration of Cloud Weather Presets (As per Inital Cloud Presets)
* GUI
* Briefing Display
* ED Cloud config (hopefully once they add more options we can do more here)

### Database/Units directory
Configuration of units playable or spawnable in missions.

#### Single Ground Unit
Probably the most basic

Flakpanzer Gepard.ini Example(03/21)

    [Unit]
    DCSID=Gepard <-- Unit ID pulled from mission file
    Families=VehicleAAA <-- Unit Families Options(UnitFamily.cs)
    ExtraLua= <-- Any extra lua code needed(mostly aircraft) again mission file
    Flags= <-- Options (DBEntryUnitFlags.cs)

    [Operators] 
    Belgium=Decade1970,Decade2010 <-- Coalitions/Nations (Database/Coalitions) used the unit when (Decade.cs)
    Brazil=Decade2010,Decade2020
    Chile=Decade2010,Decade2010
    Germany=Decade1970,Decade2020
    Jordan=Decade2010,Decade2020
    Romania=Decade2000,Decade2020
    Netherlands=Decade1970,Decade2020

#### Aircraft

A-10C Thunderbolt II 2.0.ini Example(03/21)
For Payload configs see [Porting Payloads](docs/PortingPayloads.md)

    [Unit]
    DCSID=A-10C_2
    Families=PlaneAttack
    ExtraLua=
    Flags=EPLRS

    [Aircraft]
    A2ARating.AirToAir=1 <-- Relative Air to air power dedicated loadout
    A2ARating.Default=1 <-- Relative Air to air power average loadout

    CarrierTypes= <-- Capable of operating of what carrier types (CarrierType.cs)

    CruiseAltitude=Average <- relative cruise altitude
    CruiseSpeed=Average <- relative cruise altitude

    PlayerControllable=True

    Radio.Modulation=FM <- default radio (simple)
    Radio.Frequency=251.0

    Payload.Common=["chaff"] = 240,["flare"] = 120,["fuel"] = 5029,["ammo_type"] = 1,["gun"] = 100 <- Common payload params for aircraft 

    <- Payload Config Payload.Task.(PayloadId(- is space)).Pylon(pos)=Item ID
    ; Default: AGM-65D*4,GBU-12*2,GBU-38,Mk-82,AIM-9,TGP,ECM
    Payload.Task.Default.pylon01=ALQ_184 
    Payload.Task.Default.pylon03={E6A6262A-CA08-4B3D-B030-E1A993B98452}
    Payload.Task.Default.pylon04={DB769D48-67D7-42ED-A2BE-108D566C8B1E}
    Payload.Task.Default.pylon05={BCE4E030-38E9-423E-98ED-24BE3DA87C32}
    Payload.Task.Default.pylon07={GBU-38}
    Payload.Task.Default.pylon08={DB769D48-67D7-42ED-A2BE-108D566C8B1E}
    Payload.Task.Default.pylon09={E6A6262A-CA08-4B3D-B030-E1A993B98453}
    Payload.Task.Default.pylon10={A111396E-D3E8-4b9c-8AC9-2432489304D5}
    Payload.Task.Default.pylon11={DB434044-F5D0-4F1F-9BA9-B73027E18DD3}

    [Operators]
    USA=Decade2000,Decade2020


#### Ground Groups (SAMs)

SA-10 Grumble.ini Example(03/21)

    [Unit]
    <- Comma seperated list of unit ids from mission file
    DCSID=S-300PS 40B6M tr,S-300PS 5P85D ln,S-300PS 40B6MD sr,S-300PS 64H6E sr,S-300PS 5P85C ln,S-300PS 5P85D ln,S-300PS 5P85C ln,S-300PS 5P85D ln,S-300PS 54K6 cp,S-300PS 5P85D ln,Ural-4320T
    Families=VehicleSAMLong
    ExtraLua=
    Flags=
    <-  Relative offset of each unit to leader at (0,0) ";" semi colon separated
    Offset.Coordinates=-18.227276489837,82.547616217684;0,0;0.69314285700057,127.97571427998;-22.516027817775,246.55467524595;-82.640406328599,-0.41562629461987;-81.939684967575,17.115632734552;83.349983285123,-1.3806866992963;-81.93968496757,-17.99454369233;23.579234991226,246.55467524595;82.498640577195,16.104647497938;2.3794285710001,-70.390285720001
    <-  Relative offset heading of each unit to leader at 0 "," comma separated
    Offset.Heading=4.7123889803847,2.9670597283904,0,3.1415926535898,0,6.1086523819802,3.1415926535898,0.17453292519943,3.1415926535898,3.3161255787892,0

    [Operators]
    Algeria=Decade2010,Decade2020
    Armenia=Decade2010,Decade2020
    Azerbaijan=Decade2010,Decade2020
    Belarus=Decade1970,Decade2020
    Bulgaria=Decade1980,Decade2020
    China=Decade1990,Decade2020
    Czechoslovakia=Decade1990,Decade2020
    Egypt=Decade2010,Decade2020
    EastGermany=Decade1980,Decade2020
    Greece=Decade2000,Decade2020
    Iran=Decade2010,Decade2020
    Kazakhstan=Decade1970,Decade2020
    Russia=Decade1970,Decade2020
    Slovakia=Decade1980,Decade2020
    Ukraine=Decade1970,Decade2020
    Venezuela=Decade2010,Decade2020
    Vietnam=Decade2000,Decade2020

## Include directory

The include directory stores all Lua scripts, sound and images to be included in the output .miz files.

### Include/Html 

Templates in html or text mainly for Briefing but also for Kneeboard 

### Include/Jpg directory

Images (of course) of interest to you:
*  Flags (links to Coalitions)
* Theatres (background mission images based of DCSID)

### Include/Lua directory

Segments and Templates of Lua including scripting sections of intrest to you:
* Mission Features (Lua Script location referenced in Database)
* Objective Features (Lua Script location referenced in Database)
* Objective Triggers (Lua Script location referenced in Database (Tasks))
* Units 
  * Group (Group stuff like waypoints)
  * Unit (Single Unit level stuff)
* Script Lua (Base Lua template)

### Include/Ogg directory

Ogg Sound files used in Lua Scripts

### Include/Markdown/Manuals directory

Markdown files for Manual in UI (Your reading one)
