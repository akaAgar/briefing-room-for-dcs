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

using System.Collections.Generic;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Data
{
    internal struct DBEntryUnitRadioPreset
    {
        internal int[] Modulations { get; }

        internal int[] Channels { get; }

        internal DBEntryUnitRadioPreset(int[] channels, int[] modulations)
        {
            Channels = channels;
            Modulations = modulations;
        }

        public string ToLuaString(int number)
        {
            return LuaSerialiser.Serialize(new Dictionary<string, object>{
                {"modulations", Modulations},
                {"channels", Channels}
            });
        }
    }
}
