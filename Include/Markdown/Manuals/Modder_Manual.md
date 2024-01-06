# Modders manual (TODO: UPDATE 2024)
__Last Edited: 16/04/2022__

This manual contains everything you need to know in order to modify BriefingRoom to your needs [for Aircraft Payloads and Liveries use Custom Configs](Custom_Configs.md).
If you only want to generate missions, please read the User's manual.

**This manual is not complete detail as code is often changing. This should give you an idea about what is where. Some folders will have more detail than others**

## Table of contents
1. [Briefing Text](#briefing-text)
1. [Database directory](#database-directory)
    1. [Database/Common.ini](#databasecommonini)
    1. [Database/Briefing.ini](#databasebriefingini)
    1. [Database/EnemyAirDefense.ini](#databaseenemyairdefenseini)
    1. [Database/Names.ini](#databasenamesini)
    1. [Database/Objectives.ini](#databaseobjectivesini)
    1. [Database/Coalitions directory](#databasecoalitions-directory)
    1. [Database/MissionFeatures directory](#databasemissionfeatures-directory)
    1. [Database/ObjectiveFeatures directory](#databaseobjectivefeatures-directory)
    1. [Database/ObjectiveTargets directory](#databaseobjectivetargets-directory)
    1. [Database/ObjectiveTargetBehaviors directory](#databaseobjectivetargetbehaviors-directory)
    1. [Database/ObjectiveTasks directory](#databaseobjectivetasks-directory)
    1. [Database/ObjectivePresets directory](#databaseobjectivepresets-directory)
    1. [Database/Theaters directory](#databasetheaters-directory)
    1. [Database/WeatherPresets directory](#databaseweatherpresets-directory))
    1. [Database/DCSMods directory](#databasedcsmods-directory)
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

### Database/Briefing.ini

* ***[Briefing]* section**
  * **MaxObjectiveDescriptionCount**: Max number of objectives it will describe.
  * **OverflowObjectiveDescriptionText**: Tail off text when above MaxObjectiveDescriptionCount.
  * **ObjectiveDescriptionConnectors**: Comma separated list of words or short phrases that attempt to link the objectives together (nothing is valid so you may see `,,`).


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

### Database/WeatherPresets directory
Configuration of Cloud Weather Presets (As per Inital Cloud Presets)
* GUI
* Briefing Display
* ED Cloud config (hopefully once they add more options we can do more here)

### Database/DCSMods directory
Configuration of unit mods
* GUI
* Module (DCS id of the mod - create mission in ME using units from the mod and check the requiredModules section in the saved mission file. If missing unit DCSID will be used)

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
