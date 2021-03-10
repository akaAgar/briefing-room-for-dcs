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

namespace BriefingRoom4DCSWorld.Mission
{
    /// <summary>
    /// Stores information about an unit group.
    /// </summary>
    public class DCSMissionUnitGroup : IDisposable
    {
        /// <summary>
        /// ID of the airbase this group is linked to, or zero if none.
        /// </summary>
        public int AirbaseID { get; set; } = 0;

        /// <summary>
        /// DCS World unit category this unit group belongs to.
        /// </summary>
        public UnitCategory Category { get; set; } = UnitCategory.Vehicle;

        /// <summary>
        /// Lua table for this unit callsign, if any.
        /// </summary>
        public string CallsignLua { get; set; } = "";

        /// <summary>
        /// Coalition this group belongs to.
        /// </summary>
        public Coalition Coalition { get; set; } = Coalition.Blue;

        /// <summary>
        /// Initial coordinates for this group.
        /// </summary>
        public Coordinates Coordinates { get; set; } = new Coordinates();

        /// <summary>
        /// Alternate coordinates for this group. May be used for patrols, etc.
        /// </summary>
        public Coordinates Coordinates2 { get; set; } = new Coordinates();

        /// <summary>
        /// Various flags applied to this unit group.
        /// </summary>
        public DCSMissionUnitGroupFlags Flags { get; set; } = 0;

        /// <summary>
        /// Unique ID of this group.
        /// </summary>
        public int GroupID { get; set; } = 0;

        /// <summary>
        /// Lua file (read from "Include\Lua\Mission") to use as a template for this group.
        /// </summary>
        public string LuaGroup { get; set; } = "";

        /// <summary>
        /// Lua file (read from "Include\Lua\Mission") to use as a template for each unit of this group.
        /// </summary>
        public string LuaUnit { get; set; } = "";

        /// <summary>
        /// Name of this group.
        /// </summary>
        public string Name { get; set; } = null;

        /// <summary>
        /// Skill level of this unit group.
        /// </summary>
        public DCSSkillLevel Skill { get; set; } = DCSSkillLevel.Good;

        /// <summary>
        /// Units belonging to this group.
        /// </summary>
        public DCSMissionUnitGroupUnit[] Units { get; set; } = new DCSMissionUnitGroupUnit[0];

        /// <summary>
        /// ID of the unit database entry used to load additional data for this group.
        /// Only used for aircraft groups.
        /// </summary>
        public string UnitID { get; set; } = "";

        /// <summary>
        /// The type of payload units of this group will carry.
        /// </summary>
        public UnitTaskPayload Payload { get; set; } = UnitTaskPayload.Default;

        /// <summary>
        /// Speed Of group;
        /// </summary>
        public double Speed {get; set;}

        /// <summary>
        /// TACAN Value to be transmitted (Just single unit goups for now).
        /// </summary>
        public Tacan TACAN {get; set;}

        /// <summary>
        /// ILS Channel to be transmitted (Just single unit goups for now).
        /// </summary>
        public int ILS {get; set;}

        /// <summary>
        /// ID of Carrier group is linked to
        /// </summary>
        public int CarrierId {get; set;}
        /// <summary>
        /// Default radio frequency for this aircraft.
        /// </summary>
        public float RadioFrequency { get; set; }

        /// <summary>
        /// Default radio modulation for this aircraft.
        /// </summary>
        public RadioModulation RadioModulation { get; set; }
        /// <summary>
        /// Constructor.
        /// </summary>

        public Country Country {get; set;} = Country.CJTF_BLUE;
        public DCSMissionUnitGroup() {}

        /// <summary>
        /// Is this group a player-controlled flight group?
        /// </summary>
        /// <returns>True if at least one unit in this group is player-controlled, false otherwise</returns>
        public bool IsAPlayerGroup()
        {
            return Flags.HasFlag(DCSMissionUnitGroupFlags.FirstUnitIsPlayer) ||
                (Skill == DCSSkillLevel.Client) || (Skill == DCSSkillLevel.Player);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
