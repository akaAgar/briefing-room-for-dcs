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

namespace BriefingRoom4DCS
{
    /// <summary>
    /// Enumerates the various coalitions map countries (and their airbase) can belong to,
    /// relative to the default settings in <see cref="DB.DBEntryTheaterAirbase"/> and <see cref="DB.DBEntryTheaterSpawnPoint"/>.
    /// </summary>
    public enum CountryCoalition
    {
        [Display(Name = "Default", Description = "Use default theater setting.")]
        Default,
        
        [Display(Name = "Inverted", Description = "Use the opposite of the default theater setting (blue becomes red and vice versa).")]
        Inverted,
        
        [Display(Name = "All blue", Description = "All airbases on the map (unless specified otherwise during mission generation, e.g. the starting airbase of red players) belong to the blue coalition.")]
        AllBlue,

        [Display(Name = "All red", Description = "All airbases on the map (unless specified otherwise during mission generation, e.g. the starting airbase of blue players) belong to the red coalition.")]
        AllRed
    }
}
