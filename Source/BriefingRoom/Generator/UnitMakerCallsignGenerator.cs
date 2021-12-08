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
    internal class UnitMakerCallsignGenerator
    {
        private static readonly int NATO_CALLSIGN_COUNT = Toolbox.EnumCount<UnitCallsignFamily>();

        private static readonly string[][] NATO_CALLSIGN_NAMES = new string[][]
        {
            new string[] { "Enfield", "Springfield", "Uzi", "Colt", "Dodge", "Ford", "Chevy", "Pontiac" },
            new string[] { "Overlord", "Magic", "Wizard", "Focus", "Darkstar" },
            new string[] { "Axeman", "Darknight", "Warrior", "Pointer", "Eyeball", "Moonbeam", "Whiplash", "Finger", "Pinpoint", "Ferret", "Shaba", "Playboy", "Hammer", "Jaguar", "Deathstar", "Anvil", "Firefly", "Mantis", "Badger" },
            new string[] { "Texaco", "Arco", "Shell" },
        };

        private readonly DBEntryCoalition[] CoalitionsDB;

        private readonly int[][] NATOCallsigns = new int[NATO_CALLSIGN_COUNT][];

        private readonly List<string> RussianCallsigns = new List<string>();

        internal UnitMakerCallsignGenerator(DBEntryCoalition[] coalitionsDB)
        {
            CoalitionsDB = coalitionsDB;

            int i, j;

            for (i = 0; i < NATO_CALLSIGN_COUNT; i++)
            {
                NATOCallsigns[i] = new int[NATO_CALLSIGN_NAMES[i].Length];
                for (j = 0; j < NATO_CALLSIGN_NAMES[i].Length; j++)
                    NATOCallsigns[i][j] = 0;
            }

            RussianCallsigns.Clear();
        }

        internal UnitCallsign GetCallsign(UnitFamily unitFamily, Coalition coalition, Side side, bool isUsingSkynet)
        {
            UnitCallsignFamily callsignFamily = GetCallsignFamilyFromUnitFamily(unitFamily);

            if (CoalitionsDB[(int)coalition].NATOCallsigns)
                return GetNATOCallsign(callsignFamily, unitFamily, side, isUsingSkynet);

            return GetRussianCallsign(unitFamily, side, isUsingSkynet);
        }

        private UnitCallsign GetNATOCallsign(UnitCallsignFamily callsignFamily, UnitFamily unitFamily, Side side, bool isUsingSkynet)
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
            if (isUsingSkynet && unitFamily == UnitFamily.PlaneAWACS)
                unitName = SetSkyNetPrefix(unitName, side);
            return new UnitCallsign(groupName, unitName/*, onboardNum*/, lua);
        }


        private UnitCallsign GetRussianCallsign(UnitFamily unitFamily, Side side, bool isUsingSkynet)
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
            string oldUnitName = unitName;

            if (isUsingSkynet && unitFamily == UnitFamily.PlaneAWACS)
                unitName = SetSkyNetPrefix(unitName, side);

            return new UnitCallsign(fgName + "0", unitName, oldUnitName);
        }

        private string SetSkyNetPrefix(string unitName, Side side)
        {
            var prefix = side == Side.Ally ? "BLUE-" : "";
            return $"{prefix}EW-{unitName}";
        }

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
    }
}
