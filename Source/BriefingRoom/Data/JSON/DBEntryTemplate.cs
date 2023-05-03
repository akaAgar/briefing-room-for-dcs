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
using BriefingRoom4DCS.Template;
using Newtonsoft.Json;

namespace BriefingRoom4DCS.Data
{
    internal class DBEntryTemplate : DBEntry
    {
        
        internal string DCSCategory { get; init; }
        internal string Type { get; init; }
        internal List<Country> Country { get; init; }
        internal List<DBEntryTemplateUnit> Units { get; init; }
        internal UnitFamily Family {get; init;}

        internal List<Decade> Operational { get; init; }

        protected override bool OnLoad(string o)
        {
            throw new NotImplementedException();
        }

        internal static Dictionary<string, DBEntry> LoadJSON(string filepath)
        {
            var itemMap = new Dictionary<string, DBEntry>(StringComparer.InvariantCultureIgnoreCase);
            var data = JsonConvert.DeserializeObject<List<BriefingRoom4DCS.Data.JSON.Template>>(File.ReadAllText(filepath));
            var supportData = JsonConvert.DeserializeObject<List<BriefingRoom4DCS.Data.JSON.TemplateBRInfo>>(File.ReadAllText($"{filepath.Replace(".json","")}BRInfo.json"));
            foreach (var template in data)
            {   
                var id = template.name;
                var supportInfo = supportData.Find(x => x.type == id);
                if(supportInfo == null)
                {
                    BriefingRoom.PrintToLog($"Template {id} missing support data.", LogMessageErrorLevel.Warning);
                    continue;
                }
                var countryList = new List<Country>{(Country)template.country};
                countryList.AddRange(supportInfo.extraOperators.Select(x => (Country)Enum.Parse(typeof(Country), x, true)));
                itemMap.Add(id, new DBEntryTemplate
                {
                    ID = id,
                    UIDisplayName = new LanguageString(template.name),
                    Type = template.type,
                    Country = countryList.Distinct().ToList(),
                    Family = (UnitFamily)Enum.Parse(typeof(UnitFamily), supportInfo.family, true),
                    Operational = supportInfo.operational.Select(x => (Decade)x).ToList(),
                    Units = template.units.Select(x => new DBEntryTemplateUnit {
                        DCoordinates = new Coordinates(x.dx, x.dy),
                        DCSID = x.name,
                        Heading = x.heading
                    }).ToList()
                });
            }

            return itemMap;
        }

        public DBEntryTemplate(){}
    }

    internal class DBEntryTemplateUnit
    {
        public DBEntryTemplateUnit(){}
        internal Coordinates DCoordinates { get; init; }
        internal double Heading { get; init; }
        internal string DCSID { get; init; }
    }
}
