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

using System.ComponentModel.DataAnnotations;

namespace BriefingRoom4DCS.Template
{
    /// <summary>
    /// Enumerates all decades between 1940 and 2010.
    /// </summary>
    public enum Decade
    {
        [Display(Name = "1940s (World war 2)", Description = "A random year between 1942 and 1945.")]
        Decade1940,

        [Display(Name = "1950s", Description = "A random year between 1950 and 1959.")]
        Decade1950,

        [Display(Name = "1960s", Description = "A random year between 1960 and 1969.")]
        Decade1960,

        [Display(Name = "1970s", Description = "A random year between 1970 and 1979.")]
        Decade1970,

        [Display(Name = "1980s", Description = "A random year between 1980 and 1989.")]
        Decade1980,

        [Display(Name = "1990s", Description = "A random year between 1990 and 1999.")]
        Decade1990,

        [Display(Name = "2000s", Description = "A random year between 2000 and 2009.")]
        Decade2000,

        [Display(Name = "2010s", Description = "A random year between 2010 and 2019.")]
        Decade2010,

        [Display(Name = "2020s", Description = "A random year between 2020 and 2029.")]
        Decade2020
    }
}
