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

namespace BriefingRoom4DCS
{
    public class LanguageString : Dictionary<string, string>
    {
        public LanguageString() { }

        public LanguageString(Dictionary<string, string> dict) {
            foreach (var item in dict)
            {
                this.Add(item.Key, item.Value);
            }
        }
        public LanguageString(string value)
        {
            this.Add("EN", value);
        }
        public string Get()
        {
            var key = BriefingRoom.LanguageDB.Language;
            if (this.ContainsKey(key)) return this[key];
            if (this.ContainsKey("EN")) return this["EN"];
            return $"";
        }

        public Dictionary<string, string> ToDictionary()
        {
            return this;
        }
    }
}
