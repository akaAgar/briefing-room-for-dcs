# BriefingRoom for DCS World - User's manual

This manual contains everything you need to know in order to create mods for BriefingRoom. If you only want to generate missions, please read the User's manual.

**This manual is not complete yet, additional information will be added in future revisions.**

## Table of contents
1. [Database directory](#database-directory)
    1. [Database/Common.ini](#databasecommonini)
    1. [Database/Defaults.ini](#databasedefaultsini)
    1. [Database/EnemyAirDefense.ini](#databaseenemyairdefenseini)
    1. [Database/Names.ini](#databasenamesini)
    1. [Database/Objectives.ini](#databaseobjectivesini)
    1. [Database/Coalitions directory](#databasecoalitions-directory)
    1. [Database/Extensions directory](#databaseextensions-directory)
    1. [Database/MissionFeatures directory](#databasemissionfeatures-directory)
    1. [Database/Objectives directory](#databaseobjectives-directory)
    1. [Database/Theaters directory](#databasetheaters-directory)
    1. [Database/Units directory](#databaseunits-directory)
1. [Include directory](#include-directory)
    1. [Include/Briefing.html](#includebriefinghtml)
    1. [Include/Jpg directory](#includejpg-directory)
    1. [Include/Lua directory](#includelua-directory)
        1. [Include/Lua/IncludedScripts directory](#includeluaincludedscripts-directory)
        1. [Include/Lua/Mission directory](#includeluamission-directory)
        1. [Include/Lua/Units directory](#includeluaunits-directory)
        1. [Include/Lua/Warehouses directory](#includeluawarehouses-directory)
    1. [Include/Ogg directory](#includeogg-directory)

## Database directory

The database directory contains a series of .ini files used to describe almost everything in BriefingRoom, from the available units to the various mission types.

### Database/Common.ini

* ***[Include]* section**
  * **CommonOgg**: Comma-separated list of ogg files (read from the Include/Ogg directory) to be included in all missions, WITHOUT THE .OGG EXTENSION.

* ***[Versions]* section**
  * **DCSVersion**: Targeted DCS World version. Just for informational purpose, BriefingRoom should work with another version provided Eagle Dynamics made no major changes to the mission format.

### Database/Defaults.ini

Documentation coming soon

### Database/EnemyAirDefense.ini

Documentation coming soon

### Database/Names.ini

This files stores information about names and string constants used by the mission generation.

* ***[UnitGroup]* section**
Stores random names to be used for unit groups, by unit family. Helicopter and plane groups won't use these as they use callsigns instead, but they're included nevertheless.
  * $NTH$ is replaced with an ordinal adjective based on the group number.
  * $N$ is replaced with a number number based on the group number.

### Database/Objectives.ini

This files stores information about common objective settings.

* ***[DistanceToObjective]* section**
  * **[DISTANCECATEGORY].DistanceFromTakeOffLocation**: Approximate distance (in nautical miles) between the player(s) starting location and the objectives when this "Distance to objective" category is picked in the mission template.
  * **[DISTANCECATEGORY].DistanceBetweenObjectives**: Approximate distance (in nautical miles) between objectives when this "Distance to objective" category is picked in the mission template.

### Database/Coalitions directory

Documentation coming soon

### Database/MissionFeatures directory

Documentation coming soon

### Database/Objectives directory

Documentation coming soon

### Database/Theaters directory

Documentation coming soon

### Database/Units directory

Documentation coming soon

## Include directory

The include directory stores all Lua scripts, sound and images to be included in the output .miz files.

### Include/Briefing.html

Documentation coming soon

### Include/Jpg directory

Documentation coming soon

### Include/Lua directory

Documentation coming soon

#### Include/Lua/IncludedScripts directory

Documentation coming soon

#### Include/Lua/Mission directory

Documentation coming soon

#### Include/Lua/Units directory

Documentation coming soon

#### Include/Lua/Warehouses directory

Documentation coming soon

### Include/Ogg directory

Documentation coming soon
