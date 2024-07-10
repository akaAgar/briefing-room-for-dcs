# Translations Manual
__Last Edited: 10/07/2024__

Translations are now mainly managed though [PoEditor](https://poeditor.com/join/project/MJES2FW5lh) If you are improving translations there is the best spot.

There are a lot of english fields defined in the `Database` and `DatabaseJSON` folders. Translations can be provided like so:
```
[Data]
Theater.Caucasus.GUI.DisplayName=TEST
Coalition.USA.GUI.DisplayName=FREEDOM!
Aircraft.Su-25T.DisplayName=NOT-FREEDOM!
BriefingDescription.Destroy.BriefingDescription.Description=Are you ready, kids? Aye, aye, Captain I can't hear you Aye, aye, Captain Ooooooooooooooooooooooooooooooooooooohhhhhhhhhhhhhhhhhhhhhhhhhhh
Situation.CaucasusSecondRussoGeorgianWarCostalEncirclement.DisplayName=Bad Day
Situation.CaucasusSecondRussoGeorgianWarCostalEncirclement.BriefingDescription=Ah Shoot
```
For clarity the patterns are:

.ini: 

        <CLASS derived from DBEntry<N>>.<ID>.<SECTION>.<KEY>
JSON:

        <CLASS derived from DBEntry<N>>.<ID>.<KEY>

## Adding a new Language

Languages are configured within `Database\Common.ini` under `Languages` the format is fairly easy

    <Short Identifier>=<Display Title>

for example 

    PRT=Pirate (English)

We shall be using Pirate for all our examples (feel free to expand its translations for just pure fun)
