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

using BriefingRoom.Attributes;

namespace BriefingRoom
{
    /// <summary>
    /// How good (or bad) the weather should be during a mission.
    /// </summary>
    public enum Weather
    {
        [TreeViewEnum("Random", "Any weather. Calmer weather will still be picked more often than stormy/dangerous ones.")]
        Random = -1, // Random must be < 0 for casts from Int32

        [TreeViewEnum("Clear", "Clear weather. Not a cloud on the horizon.")]
        Clear,

        [TreeViewEnum("Light clouds", "Light clouds.")]
        LightClouds,

        [TreeViewEnum("Some clouds", "Some clouds.")]
        SomeClouds,

        [TreeViewEnum("Overcast", "Lots of clouds. Visibility may be impaired.")]
        Overcast,
        
        [TreeViewEnum("Precipitation", "Rain, snow, light dust storm (according to theater and season).")]
        Precipitation,
        
        [TreeViewEnum("Storm", "Heavy rain, lightning, snowstorm, heavy dust storm (according to theater and season).")]
        Storm
    }
}
