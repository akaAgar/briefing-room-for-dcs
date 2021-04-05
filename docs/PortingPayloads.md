# Porting Payloads

Briefing Room (BR) has Payload configurations for all aircraft (playable or not) across **4 task types**:
- Default (Air to Ground)
- Air to Air
- SEAD
- Anti-Ship

In addition to that we also support different configurations depending on the decade your mission is set in from **1940s-2020s** (2000s by default).

## Porting Payloads from DCS ME to BR

Porting a payload its relatively simple.

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
1. Open BR aircraft config `<BR-Folder>\Database\Units` in my case the F/A-18 path is `Database\Units\PlaneFighter\FA-18C_hornet.ini`
1. Format Payload into config like so *once again my AT config*
 
    ```ini
    Payload.Decade2000.Task.Default.Pylon01={AIM-9L}
    Payload.Decade2000.Task.Default.Pylon02=LAU_117_AGM_65F
    Payload.Decade2000.Task.Default.Pylon03=LAU_117_AGM_65F
    Payload.Decade2000.Task.Default.Pylon04={AAQ-28_LEFT}
    Payload.Decade2000.Task.Default.Pylon05={FPU_8A_FUEL_TANK}
    Payload.Decade2000.Task.Default.Pylon06={40EF17B7-F508-45de-8566-6FFECC0C1AB8}
    Payload.Decade2000.Task.Default.Pylon07=LAU_117_AGM_65F
    Payload.Decade2000.Task.Default.Pylon08=LAU_117_AGM_65F
    Payload.Decade2000.Task.Default.Pylon09={AIM-9L}
    ```
    **Be careful of to copy the exact weapon format**
1. Save file
1. Test by generating a mission.

## Call for contribution

Unfortunately as of (Q1 2021) most many aircraft are not fully configured especially when it comes to different configurations by decade. 

If you are interested in a specific decade or aircraft you are most welcome to contribute your payload configs to the project on github. 

Few things to remember:

* SEAD & Air-to-Air payloads are used by AI so consider giving them extra fuel and weapons you know the AI can use effectively.
* Use a variety of weapons where possible, not everyone loves the AGM_65F as I do nor is it always suitable.
* No Training/Practice/Smoke/Illumination.
* Avoid super heavy payloads, players may need to fly in adverse conditions.
* Consider ECM/Targeting pods, players may need them.
* F/A-18C is a good example.

## Can I set my own custom payloads?

Sometimes the default payloads aren't suitable for your needs and you don't want to re-arm each time. Feel free to change the aircraft configs to your needs.

*Word of warning though make sure the backup your custom configs when you update and manually copy paste the custom payloads back in. Changes in the configs do occur and this will help avoid issues.*