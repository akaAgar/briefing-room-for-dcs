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

namespace BriefingRoom4DCS.Data
{
    internal class DBEntryJSONUnit : DBEntry
    {
        internal string DCSID { get; init; }
        internal Dictionary<Country, List<string>> Liveries { get; init; }
        internal List<Country> Countries { get; init; }
        internal string Module { get; init; }
        internal UnitCategory Category { get { return Families[0].GetUnitCategory(); } }
        internal bool IsAircraft { get { return Category.IsAircraft(); } }
        internal UnitFamily[] Families { get; init; }


        protected override bool OnLoad(string o)
        {
            throw new NotImplementedException();
        }


        public DBEntryJSONUnit() { }
    }
}
