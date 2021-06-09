/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar
(https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World.
If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using BriefingRoom4DCS.Data;
using System;
using System.Collections.Generic;

namespace BriefingRoom4DCS.Generator
{
    /// <summary>
    /// Generates unique callsigns for each aircraft group and unit in the mission.
    /// Used by <see cref="UnitMaker"/>.
    /// </summary>
    internal class UnitMakerCallsignGenerator : IDisposable
    {
        /// <summary>
        /// Total number of different NATO callsigns "families".
        /// </summary>
        private static readonly int NATO_CALLSIGN_COUNT = Toolbox.EnumCount<UnitCallsignFamily>();

        /// <summary>
        /// List of NATO callsigns.
        /// Must match names and indices in DCS World: https://wiki.hoggitworld.com/view/DCS_enum_callsigns
        /// </summary>
        private static readonly string[][] NATO_CALLSIGN_NAMES = new string[][]
        {
            new string[] { "Enfield", "Springfield", "Uzi", "Colt", "Dodge", "Ford", "Chevy", "Pontiac" },
            new string[] { "Overlord", "Magic", "Wizard", "Focus", "Darkstar" },
            new string[] { "Axeman", "Darknight", "Warrior", "Pointer", "Eyeball", "Moonbeam", "Whiplash", "Finger", "Pinpoint", "Ferret", "Shaba", "Playboy", "Hammer", "Jaguar", "Deathstar", "Anvil", "Firefly", "Mantis", "Badger" },
            new string[] { "Texaco", "Arco", "Shell" },
        };

        /// <summary>
        /// Blue and red coalitions database entries.
        /// </summary>
        private readonly DBEntryCoalition[] CoalitionsDB;

        /// <summary>
        /// How many groups already use each of the NATO callsigns.
        /// So for instance, if "Enfield 1" is already in use, next "Enfield" group will be "Enfield 2"
        /// </summary>
        private readonly int[][] NATOCallsigns = new int[NATO_CALLSIGN_COUNT][];

        /// <summary>
        /// List of already used Russian callsigns.
        /// </summary>
        private readonly List<string> RussianCallsigns = new List<string>();

        /// <summary>
        /// Constructor.
        /// </summary>
        internal UnitMakerCallsignGenerator(DBEntryCoalition[] coalitionsDB)
        {
            CoalitionsDB = coalitionsDB;

            Clear();
        }

        internal void Clear()
        {
            int i, j;

            for (i = 0; i < NATO_CALLSIGN_COUNT; i++)
            {
                NATOCallsigns[i] = new int[NATO_CALLSIGN_NAMES[i].Length];
                for (j = 0; j < NATO_CALLSIGN_NAMES[i].Length; j++)
                    NATOCallsigns[i][j] = 0;
            }

            RussianCallsigns.Clear();
        }

        /// <summary>
        /// Returns an unique callsign for an aircraft group.
        /// </summary>
        /// <param name="unitFamily">The unit family</param>
        /// <param name="natoCallsign">Should this callsign be in the NATO format (true) or the Russian format (false)</param>
        /// <returns></returns>
        internal UnitCallsign GetCallsign(UnitFamily unitFamily, Coalition coalition)
        {
            UnitCallsignFamily callsignFamily = GetCallsignFamilyFromUnitFamily(unitFamily);

            if (CoalitionsDB[(int)coalition].NATOCallsigns)
                return GetNATOCallsign(callsignFamily);

            return GetRussianCallsign();
        }

        /// <summary>
        /// Returns an unique callsign in the NATO format (Callsign Number Number)
        /// </summary>
        /// <param name="callsignFamily">A callsign family</param>
        /// <returns>The callsign</returns>
        private UnitCallsign GetNATOCallsign(UnitCallsignFamily callsignFamily)
        {
            int callsignIndex;

            do
            {
                callsignIndex = Toolbox.RandomInt(NATO_CALLSIGN_NAMES[(int)callsignFamily].Length);
            } while (NATOCallsigns[(int)callsignFamily][callsignIndex] >= 9);

            NATOCallsigns[(int)callsignFamily][callsignIndex]++;

            string groupName =
                NATO_CALLSIGN_NAMES[(int)callsignFamily][callsignIndex] + " " +
                Toolbox.ValToString(NATOCallsigns[(int)callsignFamily][callsignIndex]);

            string unitName = groupName + " $INDEX$";

            string lua =
                $"{{ [1]= {Toolbox.ValToString(callsignIndex + 1)}, " +
                $"[2]={Toolbox.ValToString(NATOCallsigns[(int)callsignFamily][callsignIndex])}, " +
                "[3]=$INDEX$, " +
                $"[\"name\"] = \"{unitName.Replace(" ", "")}\", }}";

            return new UnitCallsign(groupName, unitName/*, onboardNum*/, lua);
        }

        /// <summary>
        /// Returns an unique callsign in the russian format (3-digits)
        /// </summary>
        /// <returns>The callsign</returns>
        private UnitCallsign GetRussianCallsign()
        {
            string fgName;
            int[] fgNumber = new int[2];

            do
            {
                fgNumber[0] = Toolbox.RandomMinMax(1, 9);
                fgNumber[1] = Toolbox.RandomMinMax(0, 9);

                fgName = Toolbox.ValToString(fgNumber[0]) + Toolbox.ValToString(fgNumber[1]);
            } while (RussianCallsigns.Contains(fgName));

            RussianCallsigns.Add(fgName);

            string unitName = fgName + "$INDEX$";

            return new UnitCallsign(fgName + "0", unitName, unitName);
        }

        /// <summary>
        /// Gets the NATO callsign family to use depending on the unit family
        /// (tankers don't use the same callsigns as early warning aircraft, etc)
        /// </summary>
        /// <param name="unitFamily">Unit family the unit group belongs to</param>
        /// <returns>A callsign family</returns>
        private static UnitCallsignFamily GetCallsignFamilyFromUnitFamily(UnitFamily unitFamily)
        {
            switch (unitFamily)
            {
                case UnitFamily.PlaneAWACS:
                    return UnitCallsignFamily.AWACS;
                case UnitFamily.PlaneTankerBasket:
                case UnitFamily.PlaneTankerBoom:
                    return UnitCallsignFamily.Tanker;
                default:
                    return UnitCallsignFamily.Aircraft;
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
