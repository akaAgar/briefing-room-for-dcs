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
    internal class DBEntryCar : DBEntryJSONUnit
    {
        
        internal string DCSCategory { get; init; }

        protected override bool OnLoad(string o)
        {
            throw new NotImplementedException();
        }

        internal static Dictionary<string, DBEntry> LoadJSON(string filepath, Dictionary<string, DBEntryUnit> unitDict)
        {
            var itemMap = new Dictionary<string, DBEntry>(StringComparer.InvariantCultureIgnoreCase);
            var data = JsonConvert.DeserializeObject<List<Car>>(File.ReadAllText(filepath));
            foreach (var car in data)
            {
                var id = car.type;
                if (!unitDict.ContainsKey(id))
                {
                    BriefingRoom.PrintToLog($"Ini unit missing {id}", LogMessageErrorLevel.Warning);
                    continue;
                }
                var iniUnit = unitDict[id];
                itemMap.Add(id, new DBEntryCar
                {
                    ID = id,
                    UIDisplayName = new LanguageString(car.displayName),
                    DCSID = car.type,
                    Liveries = car.paintSchemes.ToDictionary(pair => (Country)Enum.Parse(typeof(Country), pair.Key.Replace(" ", ""), true), pair => pair.Value),
                    Countries = car.countries.Select(x => (Country)Enum.Parse(typeof(Country), x.Replace(" ", ""), true)).ToList(),
                    DCSCategory = car.category,
                    Module = car.module,

                    // Look to replace/simplify
                    Families = iniUnit.Families,
                    Operational = GetOperationalPeriod(iniUnit.Operators),
                    LowPoly = iniUnit.Flags.HasFlag(DBEntryUnitFlags.LowPolly)
                });
            }

            return itemMap;
        }

        public DBEntryCar(){}
    }
}
