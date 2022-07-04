# Translations Manual
__Last Edited: 04/07/2022__

**Translation support is currently partial** being limited to Briefings, Kneeboards and some UI support. Full UI and Scripting Message support will be added in time.

If you are attempting to add or improve any translation of this tool we thank you. We strongly suggest you make your submissions to be though an github pull request. Though running the source code is not required. The translations are designed to be developed using the normal installation of the tool with configuration being entirely within the `Database` folder.

## Adding a new Language

Languages are configured within `Database\Common.ini` under `Languages` the format is fairly easy

    <Short Identifier>=<Display Title>

for example 

    PRT=Pirate (English)

We shall be using Pirate for all our examples (feel free to expand its translations for just pure fun)


## Adding a translation

Its worth noting translations always fallback to english if no value has been provided. Translations are located in the same file as the english versions and a translation is just another value with `.<Short Identifier>` added to the key. For example in the `Database\BriefingDescriptions\Destroy.ini` we can find a pirate translation for the default briefing and a Attack helicopter version.

    [BriefingDescription]
    Description=Recon units {have learned the location of several|report large concentrations of|have spotted many} enemy forces....
    Description.PRT=Yarr me harties there be scoundruls over there. I wanna see them sunk!

    Description.HelicopterAttack={Enemy helicopters have {caused|inflicted} {heavy|unacceptable}...
    Description.HelicopterAttack.PRT=Jonny Law has wirlybirds turn them back into landlubbers!

Note you can still use the `{one variant|another variant}` formatting.

## Database/Language

All static ie, Briefing, Kneeboard & in future UI and Scripting translations can be found in the `Database/Language` folder. These are usually just short sentences or single words.

## Other Translation Locations

Translations can exist in anywhere that english text would be used in our configs. Generally if you see `DisplayName`, `Description` or `Remarks` they are all translatable. If you use a global search in a editor like VSCode and search for `.PRT=` you will see lots of examples. Including Renaming vehicles if its more common they are known by a different name.
