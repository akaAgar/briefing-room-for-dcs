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
    /// Enumerates possible settings for mission auto-ending.
    /// </summary>
    public enum MissionEndMode
    {
        [TreeViewEnum("Never", "Never end the mission automatically, players have to exit the mission.")]
        Never = -1,

        [TreeViewEnum("All players have landed", "End mission automatically when all objective are complete and all players have landed.")]
        AllPlayersHaveLanded = -3,

        [TreeViewEnum("F10 menu command", "End mission when using the \"End mission\" menu command.")]
        F10Command = -2,

        [TreeViewEnum("Instantly", "End mission instantly when the last objective is completed.")]
        Instant = 0,

        [TreeViewEnum("After 5 minutes", "End mission 5 minutes after the last objective is completed.")]
        FiveMins = 5,

        [TreeViewEnum("After 10 minutes", "End mission 10 minutes after the last objective is completed.")]
        TenMins = 10,

        [TreeViewEnum("After 20 minutes", "End mission 20 minutes after the last objective is completed.")]
        TwentyMins = 20,
    }
}
