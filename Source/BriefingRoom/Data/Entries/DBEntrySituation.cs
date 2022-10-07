/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar (https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World. If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCS.Data
{
    internal class DBEntrySituation : DBEntry
    {

        internal List<Coordinates> RedCoordinates { get; set; }

        internal List<Coordinates> BlueCoordinates { get; set; }

        internal List<Coordinates> NoSpawnCoordinates { get; set; }

        internal string Theater { get; private set; }

        internal List<LanguageString> BriefingDescriptions { get; private set; }

        protected override bool OnLoad(string iniFilePath)
        {
            var ini = new INIFile(iniFilePath);
            Theater = ini.GetValue<string>("Situation", "Theater").ToLowerInvariant();

            RedCoordinates = new List<Coordinates>();
            foreach (string key in ini.GetKeysInSection("RedCoordinates"))
                RedCoordinates.Add(ini.GetValue<Coordinates>("RedCoordinates", key));

            BlueCoordinates = new List<Coordinates>();
            foreach (string key in ini.GetKeysInSection("BlueCoordinates"))
                BlueCoordinates.Add(ini.GetValue<Coordinates>("BlueCoordinates", key));

            if (ini.GetSections().Contains("nospawncoordinates"))
            {
                NoSpawnCoordinates = new List<Coordinates>();
                foreach (string key in ini.GetKeysInSection("NoSpawnCoordinates"))
                    NoSpawnCoordinates.Add(ini.GetValue<Coordinates>("NoSpawnCoordinates", key));
            }

            if (ini.GetSections().Contains("briefingdescription"))
            {
                BriefingDescriptions = new List<LanguageString>();
                foreach (string key in ini.GetKeysInSection("BriefingDescription", true))
                    BriefingDescriptions.Add(ini.GetLangStrings("BriefingDescription", key));
            }

            if (!Database.Instance.EntryExists<DBEntryTheater>(Theater))
                throw new Exception($"Situation \"{ID}\" located on non-existing theater \"{Theater}\".");

            return true;
        }

        internal List<Coordinates> GetRedZone(bool invertCoalition) => !invertCoalition ? RedCoordinates : BlueCoordinates;
        internal List<Coordinates> GetBlueZone(bool invertCoalition) => !invertCoalition ? BlueCoordinates : RedCoordinates;

        internal DBEntryAirbase[] GetAirbases(bool invertCoalition)
        {
            var airbases = (from DBEntryAirbase airbase in Database.Instance.GetAllEntries<DBEntryAirbase>()
                            where Toolbox.StringICompare(airbase.Theater, Theater)
                            select airbase).ToArray();
            foreach (var airbase in airbases)
            {
                if (ShapeManager.IsPosValid(airbase.Coordinates, GetBlueZone(invertCoalition)))
                    airbase.Coalition = Coalition.Blue;
                if (ShapeManager.IsPosValid(airbase.Coordinates, GetRedZone(invertCoalition)))
                    airbase.Coalition = Coalition.Red;
            }
            return airbases;
        }
    }
}
