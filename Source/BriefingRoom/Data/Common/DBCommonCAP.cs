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
using System.IO;

namespace BriefingRoom4DCS.Data
{
    internal class DBCommonCAP : IDisposable
    {
        /// <summary>
        /// Min/max distance between enemy CAP and the mission objectives.
        /// </summary>
        internal MinMaxD EnemyCAPDistanceFromObjectives { get; private set; }

        /// <summary>
        /// Min distance (in nautical miles) between enemy CAP and players take off location.
        /// </summary>
        internal int EnemyCAPMinDistanceFromTakeOffLocation { get; private set; }

        /// <summary>
        /// Settings for CAP at various CAP levels.
        /// </summary>
        internal DBCommonCAPLevel[] CAPLevels { get; }

        internal DBCommonCAP()
        {
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}CAP.ini"))
            {


                //AirDefenseLevels = new DBCommonAirDefenseLevel[Toolbox.EnumCount<AmountNR>()];
                //for (i = 0; i < Toolbox.EnumCount<AmountNR>(); i++)
                //    AirDefenseLevels[i] = new DBCommonAirDefenseLevel(ini, (AmountNR)i);

                //DistanceFromCenter = new MinMaxD[2, Toolbox.EnumCount<AirDefenseRange>()];
                //MinDistanceFromOpposingPoint = new double[2, Toolbox.EnumCount<AirDefenseRange>()];
                //foreach (Side side in Toolbox.GetEnumValues<Side>())
                //{
                //    foreach (AirDefenseRange airDefenseRange in Toolbox.GetEnumValues<AirDefenseRange>())
                //    {
                //        DistanceFromCenter[(int)side, (int)airDefenseRange] = ini.GetValue<MinMaxD>($"AirDefenseRange.{side}", $"{airDefenseRange}.DistanceFromCenter");
                //        MinDistanceFromOpposingPoint[(int)side, (int)airDefenseRange] = ini.GetValue<double>($"AirDefenseRange.{side}", $"{airDefenseRange}.MinDistanceFromOpposingPoint");
                //    }
                //}
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}