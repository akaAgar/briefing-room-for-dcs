# Custom Configs
__Last Edited: 27/10/2021__

Briefing Room (BR) allows you to create custom configs. For now (Oct 2021) this is limited to Aircraft Payloads and Liveries.

The folder structure follows the same as in `Database` in the way that you should put your configs in the same relative place.

**Example Files have been provided, just change `.Example` to `.ini` to get working.**

All Custom configs require a basic setup.
1. Matching Location and Filename as in Database.
1. File Has required info at the top of the file _Copy this from the Database file_.

        [Unit]
        DCSID=A-10C_2
        Families=PlaneAttack

        [Aircraft]


## Importing Payloads from DCS ME to BR 

1. Setup your payloads and save your payloads in the Mission Editor
1. In windows navigate to `C:\Users\<USER>\Saved Games\DCS.openbeta\MissionEditor\UnitPayloads` (this example uses openBeta)
1. Open the Lua file of your chosen aircraft **DO NOT CHANGE THIS FILE**
1. Find the payload you want to port by name eg *my personal F/A-18C AT payload of 4\*AGM-65F, 2\*AIM-9L Fuel Tank, AIM-120C and LIGHTNING Pod*
    ```lua
        [8] = {
			["name"] = "AT",
			["pylons"] = {
				[1] = {
					["CLSID"] = "{AIM-9L}", <-- Weapon/FuelTank/Rack
					["num"] = 9, <-- Pylon number
				},
				[2] = {
					["CLSID"] = "{AIM-9L}",
					["num"] = 1,
				},
				[3] = {
					["CLSID"] = "LAU_117_AGM_65F",
					["num"] = 8,
				},
				[4] = {
					["CLSID"] = "LAU_117_AGM_65F",
					["num"] = 2,
				},
				[5] = {
					["CLSID"] = "LAU_117_AGM_65F",
					["num"] = 3,
				},
				[6] = {
					["CLSID"] = "LAU_117_AGM_65F",
					["num"] = 7,
				},
				[7] = {
					["CLSID"] = "{FPU_8A_FUEL_TANK}",
					["num"] = 5,
				},
				[8] = {
					["CLSID"] = "{AAQ-28_LEFT}",
					["num"] = 4,
				},
				[9] = {
					["CLSID"] = "{40EF17B7-F508-45de-8566-6FFECC0C1AB8}",
					["num"] = 6,
				},
			},
			["tasks"] = {
				[1] = 11,
			},
		},
    ```
1. Open your custom config in my case the F/A-18 path is `CustomConfigs\Units\PlaneFighter\FA-18C_hornet.ini`
1. Format Payload into config like so *once again my AT config* match `PylonXX` to `num` in the data for correct order. Make sure its under `[Aircraft]` line
 
    ```ini
    Payload.Task.AT.Pylon01={AIM-9L}
    Payload.Task.AT.Pylon02=LAU_117_AGM_65F
    Payload.Task.AT.Pylon03=LAU_117_AGM_65F
    Payload.Task.AT.Pylon04={AAQ-28_LEFT}
    Payload.Task.AT.Pylon05={FPU_8A_FUEL_TANK}
    Payload.Task.AT.Pylon06={40EF17B7-F508-45de-8566-6FFECC0C1AB8}
    Payload.Task.AT.Pylon07=LAU_117_AGM_65F
    Payload.Task.AT.Pylon08=LAU_117_AGM_65F
    Payload.Task.AT.Pylon09={AIM-9L}
    ```
    **Be careful of to copy the exact weapon format**
    
    Here I have named it `AT` but you can name it whatever you like as long as you avoid spaces and `.` Note `-` are replaced with spaces in the UI.
1. Save file
1. Load BR app and see it in the UI.


## Importing Liviers

1. Find Livery file you want to import eg `C:\Users\<user>\Saved Games\DCS.openbeta\Liveries\A-10CII\Ironfists - A-10C`
1. Add the folder name to the custom config like so. Make sure its under `[Aircraft]` line

        Liveries=Ironfists - A-10C
2. If you have multiple just comma separate them. _Leave no spaces between_

        Liveries=Ironfists - A-10C,Another Livery,NextOne

