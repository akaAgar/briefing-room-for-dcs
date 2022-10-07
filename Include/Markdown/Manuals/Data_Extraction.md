# Data Extraction
__Last Edited: 07/11/2022__

DCS doesn't supply an easy to access source of info for tools like BR this manual documents the requirements and methods of extracting data from DCS for BR. More tools will be added as we go on.

## Pre-Requisite Modding `MissionScripting.lua`
*Originally from DCS liberation docs*
To efficiently export data from DCS we need to re-enable some lua modules that ED disabled for security reasons.

The easiest way to do it is to edit the original file (`<dcs installation>\Scripts\MissionScripting.lua`)


Sometimes, you'll have an already modded file because you're using scripts that need access to specific packages.
That's alright, as long as the `os`, `io` and `lfs` packages are accessible.

To ensure this, simply be certain that these particular lines are commented out : 

__Original:__

```lua
	sanitizeModule('os')
	sanitizeModule('io')
	sanitizeModule('lfs')
```

__Commented out:__

```lua
	--sanitizeModule('os')
	--sanitizeModule('io')
	--sanitizeModule('lfs')
```

*Be advised, as soon as you use the dcs-updater to install a new version of DCS, this file will be restored and you'll have to mod it again !*

## Dump Map Data for the tools map
The map in the tool is powered by a big mapping on 1km squares from DCS coordinates to Lat, Long coordinates. In the event of a new map or a map having a large area expansion this data needs to be created/refreshed.

1. Create mission on map with the "Debug" feature turned on.
1. Load DCS and Run mission
1. Use F-10 to go to Debug menu and run Dump Map Data (you game will freeze for a few seconds depending on map size)
1. Message will print with output location
1. Open File and replace all `"\` with nothing. This should now be valid json
1. Gzip the file using 7-Zip
1. Rename the `coords.json.gz` file with the map id eg Caucasus.json.gz
1. Place file `Source\BriefingRoomCommonGUI\wwwroot\js` (replacing file if needed)


## Dump Airbase data
Airbases often get updated and added in map development. To get started on updating this its best to run a data extract of airbase positions

1. Create mission on map with the "Debug" feature turned on.
1. Load DCS and Run mission
1. Use F-10 to go to Debug menu and run Dump Airport parking data
1. Message will print with output locations
1. The files produced will form the basis of the airbase config files.
1. Copy any new airbase files into `Database\TheatersAirbases` (renaming may be needed)
1. Otherwise use the files to update existing files.
1. Fill in any missing data

