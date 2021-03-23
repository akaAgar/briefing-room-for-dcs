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

using BriefingRoom4DCSWorld.Attributes;

namespace BriefingRoom4DCSWorld
{
    /// <summary>
    /// How windy should the weather be during a mission.
    /// </summary>
    public enum Wind
    {
        [TreeViewEnum("Auto", "Wind level will be picked automatically according to the weather.")]
        Auto = -1, // Auto must be < 0 for casts from Int32

        [TreeViewEnum("Calm", "No wind at all.")]
        Calm,

        [TreeViewEnum("Light air", "Very light wind.")]
        LightAir,

        [TreeViewEnum("Gentle breeze", "Light wind.")]
        GentleBreeze,

        [TreeViewEnum("Strong breeze", "Average wind, flying and aiming may require skill.")]
        StrongBreeze,

        [TreeViewEnum("High wind", "Strong wind. Flying and aiming will be heavily impaired.")]
        HighWind,

        [TreeViewEnum("Strong gale", "Very strong wind. Flying in these condition may prove dangerous.")]
        StrongGale
    }
}
