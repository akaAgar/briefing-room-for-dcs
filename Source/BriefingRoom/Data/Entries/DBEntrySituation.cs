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
using System.IO;
using System.Linq;
using BriefingRoom4DCS.Data.JSON;
using Newtonsoft.Json;

namespace BriefingRoom4DCS.Data
{
    internal class DBEntrySituation : DBEntry
    {

        internal List<List<Coordinates>> RedZones { get; set; }

        internal List<List<Coordinates>> BlueZones { get; set; }

        internal List<List<Coordinates>> NoSpawnZones { get; set; }

        internal string Theater { get; private set; }

        internal List<LanguageString> BriefingDescriptions { get; private set; }

        internal List<string> RelatedSituations { get; private set; }

        protected override bool OnLoad(string iniFilePath)
        {
            var ini = new INIFile(iniFilePath);
            Theater = ini.GetValue<string>("Situation", "Theater").ToLower();
            RelatedSituations = ini.GetValueList<string>("Situation", "RelatedSituations");

            var zonesJsonFilePath = Path.Combine(BRPaths.DATABASEJSON, "SituationZones", $"{ID}.json");
            if(!File.Exists(zonesJsonFilePath)) 
                throw new BriefingRoomException($"Situation {ID} Missing Zone Data. File not found: {zonesJsonFilePath}");
            
            var zoneData = JsonConvert.DeserializeObject<SituationZones>(File.ReadAllText(zonesJsonFilePath));
            RedZones = zoneData.redZones.Select(x => x.Select(y => new Coordinates(y)).ToList()).ToList();
            BlueZones = zoneData.blueZones.Select(x => x.Select(y => new Coordinates(y)).ToList()).ToList();
            NoSpawnZones = zoneData.noSpawnCoordinates.Select(x => x.Select(y => new Coordinates(y)).ToList()).ToList();
            return true;
        }

        internal List<List<Coordinates>> GetRedZones(bool invertCoalition) => !invertCoalition ? RedZones : BlueZones;
        internal List<List<Coordinates>> GetBlueZones(bool invertCoalition) => !invertCoalition ? BlueZones : RedZones;

        internal DBEntryAirbase[] GetAirbases(bool invertCoalition)
        {
            var airbases = (from DBEntryAirbase airbase in Database.Instance.GetAllEntries<DBEntryAirbase>()
                            where Toolbox.StringICompare(airbase.Theater, Theater)
                            select airbase).ToArray();
            foreach (var airbase in airbases)
            {
                if (ShapeManager.IsPosValid(airbase.Coordinates, GetBlueZones(invertCoalition)))
                    airbase.Coalition = Coalition.Blue;
                if (ShapeManager.IsPosValid(airbase.Coordinates, GetRedZones(invertCoalition)))
                    airbase.Coalition = Coalition.Red;
            }
            return airbases;
        }
    }
}
