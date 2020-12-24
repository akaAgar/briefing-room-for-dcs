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

namespace BriefingRoom4DCSWorld
{
    /// <summary>
    /// Enumerates skill levels shown in BriefingRoom mission templates.
    /// NOT the internal DCS World skill level (<see cref="DCSSkillLevel"/>).
    /// Can be converted to a <see cref="DCSSkillLevel"/> by <see cref="Toolbox.BRSkillLevelToDCSSkillLevel(BRSkillLevel)"
    /// </summary>
    public enum BRSkillLevel
    {
        /// <summary>
        /// Select a random value for each unit.
        /// </summary>
        Random,
        /// <summary>
        /// Easy AI
        /// </summary>
        Rookie,
        /// <summary>
        /// Average AI
        /// </summary>
        Regular,
        /// <summary>
        /// Hard AI
        /// </summary>
        Veteran,
        /// <summary>
        /// Elite AI
        /// </summary>
        Ace
    }
}
