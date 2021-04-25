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

namespace BriefingRoom4DCS.Data
{
    public class RadioPreset
    {
        public int[] Modulations {get; set;}
        public int[] Channels {get; set;}


        public string ToLuaString() {
            var lua = @"{[""modulations""] = {";
            for (int index = 1; index < Modulations.Length; index++)
            {
                lua += $"[{index}] = {Modulations[index]},";
            }
            lua += @"},[""channels""] = {";
            for (int index = 1; index < Channels.Length; index++)
            {
                lua += $"[{index}] = {Channels[index]},";
            }   
            lua += "},},";
            return lua;
        }
    }
}