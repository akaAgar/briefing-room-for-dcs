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
    /// Enumerates all decades between 1940 and 2010.
    /// </summary>
    public enum Decade
    {
        [TreeViewEnum("1940s (World war 2)", "A random year between 1942 and 1945.")]
        Decade1940,

        [TreeViewEnum("1950s", "A random year between 1950 and 1959.")]
        Decade1950,

        [TreeViewEnum("1960s", "A random year between 1960 and 1969.")]
        Decade1960,

        [TreeViewEnum("1970s", "A random year between 1970 and 1979.")]
        Decade1970,

        [TreeViewEnum("1980s", "A random year between 1980 and 1989.")]
        Decade1980,

        [TreeViewEnum("1990s", "A random year between 1990 and 1999.")]
        Decade1990,

        [TreeViewEnum("2000s", "A random year between 2000 and 2009.")]
        Decade2000,

        [TreeViewEnum("2010s", "A random year between 2010 and 2019.")]
        Decade2010,

        [TreeViewEnum("2020s", "A random year between 2020 and 2029.")]
        Decade2020
    }
}
