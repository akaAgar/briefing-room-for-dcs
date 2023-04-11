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
using BriefingRoom4DCS.Data.JSON;
using Newtonsoft.Json;

namespace BriefingRoom4DCS.Data
{
    internal class DBEntryCar : DBEntry
    {
        internal string DCSID { get; private set; }
        internal string DCSCategory { get; set; }
        internal Dictionary<string, List<string>> Liveries { get; private set; }
        internal List<string> Countries { get; private set; }
        internal List<int> CountriesWorldID { get; private set; }
        internal UnitCategory Category { get { return Families[0].GetUnitCategory(); } }
        internal UnitFamily[] Families { get; private set; }
        internal bool IsAircraft { get { return (Category == UnitCategory.Helicopter) || (Category == UnitCategory.Plane); } }


        protected override bool OnLoad(string o)
        {
            throw new NotImplementedException();
        }


        internal static Dictionary<string, DBEntry> LoadJSON(string filepath)
        {
            var itemMap = new Dictionary<string, DBEntry>(StringComparer.InvariantCultureIgnoreCase);
            var data = JsonConvert.DeserializeObject<List<Car>>(File.ReadAllText(filepath));
            foreach (var car in data)
            {
                var id = car.type;
                itemMap.Add(id, new DBEntryCar
                {
                    ID = id,
                    UIDisplayName = new LanguageString(car.DisplayName),
                    DCSID = car.type,
                    Liveries = car.paintSchemes,
                    Countries = car.Countries,
                    DCSCategory = car.category
                });
            }

            return itemMap;
        }

        public DBEntryCar() { }

        private UnitCategory GetUnitCategory(string dcsCategory)
        {
            switch (dcsCategory)
            {
                case "Infantry":
                    return UnitCategory.Infantry;
                case "MissilesSS":
                case "Locomotive":
                case "Unarmed":
                case "Air Defense":
                case "Artillery":
                case "Carriage":
                case "Armor":
                    return UnitCategory.Vehicle;
                case "Fortification":
                    return UnitCategory.Static;
                default:
                    throw new Exception($"Unknown category: {dcsCategory}");
            }
        }
    }
}
