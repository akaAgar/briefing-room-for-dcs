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
using BriefingRoom4DCS.Mission.DCSLuaObjects;

namespace BriefingRoom4DCS.Generator
{
    internal struct Waypoint
    {
        internal string Name { get; }

        internal Coordinates Coordinates { get; }

        internal bool OnGround { get; }

        internal bool ScriptIgnore { get; }

        internal Waypoint(string name, Coordinates coordinates, bool onGround = false, bool scriptIgnore = false)
        {
            Name = name;
            Coordinates = coordinates;
            OnGround = onGround;
            ScriptIgnore = scriptIgnore;
        }

        internal DCSWaypoint ToDCSWaypoint(Data.DBEntryUnitAircraftData aircraftData)
        {
            return new DCSWaypoint
            {
                Alt = OnGround ? 0 : aircraftData.CruiseAltitude,
                AltType = OnGround ? "RADIO" : "BARO",
                Action = "Turning Point",
                Speed = aircraftData.CruiseSpeed,
                Type = "Turning Point",
                EtaLocked = false,
                SpeedLocked = true,
                X = Coordinates.X,
                Y = Coordinates.Y,
                Name = Name,
            };
        }
    }
}
