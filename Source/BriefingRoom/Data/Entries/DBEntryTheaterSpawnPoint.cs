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

namespace BriefingRoom4DCS.Data
{
    internal struct DBEntryTheaterSpawnPoint
    {

        public Coordinates Coordinates { get; internal set; }

        public SpawnPointType PointType { get; internal set; }

        public DBEntryTheaterSpawnPoint() {}

        internal bool Load(INIFile ini, string key)
        {
            string[] vals = ini.GetValueArray<string>("SpawnPoints", key, ',');

            if (vals.Length < 3) return false;

            try
            {
                Coordinates = new Coordinates(Toolbox.StringToDouble(vals[0]), Toolbox.StringToDouble(vals[1]));
                PointType = (SpawnPointType)Enum.Parse(typeof(SpawnPointType), vals[2], true);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
