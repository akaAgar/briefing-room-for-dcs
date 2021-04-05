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

More detailed documentation coming soon

### Database/Objectives directory
Core configuration of available mission types includes
* Briefing templates
* Remarks
* Objective Features and flags
* Required Aircraft Loadout type
* Hostile and Friendly units involved
* Waypoint accuracy
More detailed documentation coming soon

### Database/Theaters directory
Map Configuration covers
* Map DCS config
* Local Daylight
* Temps
* Airbase Info
* Spawn points

More detailed documentation coming soon

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

    <- Payload Config Payload.(Decade.cs).Task.(UnitTaskPayload.cs).Pylon(pos)=Item ID
    ; Default: AGM-65D*4,GBU-12*2,GBU-38,Mk-82,AIM-9,TGP,ECM
    Payload.Decade2000.Task.Default.pylon01=ALQ_184 
    Payload.Decade2000.Task.Default.pylon03={E6A6262A-CA08-4B3D-B030-E1A993B98452}
    Payload.Decade2000.Task.Default.pylon04={DB769D48-67D7-42ED-A2BE-108D566C8B1E}
    Payload.Decade2000.Task.Default.pylon05={BCE4E030-38E9-423E-98ED-24BE3DA87C32}
    Payload.Decade2000.Task.Default.pylon07={GBU-38}
    Payload.Decade2000.Task.Default.pylon08={DB769D48-67D7-42ED-A2BE-108D566C8B1E}
    Payload.Decade2000.Task.Default.pylon09={E6A6262A-CA08-4B3D-B030-E1A993B98453}
    Payload.Decade2000.Task.Default.pylon10={A111396E-D3E8-4b9c-8AC9-2432489304D5}
    Payload.Decade2000.Task.Default.pylon11={DB434044-F5D0-4F1F-9BA9-B73027E18DD3}

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
