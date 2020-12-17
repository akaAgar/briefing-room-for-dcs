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

using BriefingRoom4DCSWorld.Template;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace BriefingRoom4DCSWorld.TypeConverters
{
    public class MissionTemplateFlightGroupConverter : ArrayConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string)) return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            MissionTemplateMPFlightGroup[] flightGroups = (MissionTemplateMPFlightGroup[])value;

            if (flightGroups.Length == 0) return "No MP groups, mission is single-player";

            int plCount = (from MissionTemplateMPFlightGroup fg in flightGroups select fg.Count).Sum();
            int fgCount = flightGroups.Length;

            return $"{plCount} player(s), {fgCount} flight group(s)";
        }
    }
}