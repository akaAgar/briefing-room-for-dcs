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
using System.Linq;

namespace BriefingRoom4DCSWorld.TypeConverters
{
    public class CheckedListBoxUIEditorEnum<T> : CheckedListBoxUIEditor where T : struct
    {
        protected override bool ValueIsSet(string[] array, string value)
        {
            return array.Contains(value);
        }

        protected override string[] ParseValuesIn(object value)
        {
            if (value == null) return new string[0];

            return
                (from T v in (T[])value
                 select v.ToString()).Distinct(StringComparer.InvariantCultureIgnoreCase).OrderBy(x => x).ToArray();
        }

        protected override object ParseValuesOut(string[] selectedValues)
        {
            return
                (from string v in selectedValues
                 where Enum.TryParse(v, false, out T nothing)
                 select (T)Enum.Parse(typeof(T), v)).Distinct().OrderBy(x => x).ToArray();
        }

        protected override string[] GetPossibleArrayValues()
        {
            return Enum.GetNames(typeof(T));
        }
    }
}
