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

        private static readonly List<Country> NON_NATO_CALLSIGN_NATIONS = new() { Country.Russia, Country.Abkhazia, Country.Belarus, Country.China, Country.Insurgents, Country.SouthOssetia, Country.Ukraine, Country.USSR, Country.Yugoslavia };

        private readonly List<string> NATOCallsigns = new();

        private readonly List<string> RussianCallsigns = new();

        internal UnitMakerCallsignGenerator()
        {
            NATOCallsigns.Clear();
            RussianCallsigns.Clear();
        }

        internal UnitCallsign GetCallsign(DBEntryAircraft unitDB, Country country, Side side, bool isUsingSkynet, string overrideName, int overrideNumber)
        {

            if (NON_NATO_CALLSIGN_NATIONS.Contains(country))
                return GetRussianCallsign(unitDB.Families[0], side, isUsingSkynet);

            if (!string.IsNullOrEmpty(overrideName))
                return GetOverriddenCallsign(overrideName, overrideNumber);

            return GetNATOCallsign(unitDB, country, side, isUsingSkynet);
        }

        private UnitCallsign GetNATOCallsign(DBEntryAircraft unitDB, Country country, Side side, bool isUsingSkynet)
        {
            string groupName;
            int randomNumber;
            string[] callSignEnum;
            do
            {
                callSignEnum = Toolbox.RandomFrom<string>(unitDB.CallSigns[country]).Split(":");
                randomNumber = Toolbox.RandomMinMax(1, 9);
                groupName = $"{callSignEnum[1]} {randomNumber}";
            } while (NATOCallsigns.Contains(groupName));
            NATOCallsigns.Add(groupName);

            var unitName = groupName + " $INDEX$";
            var prefixedUnitName = unitName;
            if (isUsingSkynet && unitDB.Families[0] == UnitFamily.PlaneAWACS)
                prefixedUnitName = SetSkyNetPrefix(unitName, side);
            return new UnitCallsign(groupName, prefixedUnitName/*, onboardNum*/, new Dictionary<object, object> { { 1, callSignEnum[0] }, { 2, randomNumber }, { "name", unitName.Replace(" ", "") } });
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

        private UnitCallsign GetOverriddenCallsign(string overrideName, int overrideNumber)
        {
            var splitOverrideName = overrideName.Split(":");
            var groupName = $"{splitOverrideName[1]} {overrideNumber}";
            var unitName = groupName + " $INDEX$";
            NATOCallsigns.Add(groupName);
            return new UnitCallsign(groupName, unitName/*, onboardNum*/, new Dictionary<object, object> { { 1, int.Parse(splitOverrideName[0]) }, { 2, overrideNumber }, { "name", unitName.Replace(" ", "") } });
        }

        private static string SetSkyNetPrefix(string unitName, Side side)
        {
            var prefix = side == Side.Ally ? "BLUE-" : "";
            return $"{prefix}EW-{unitName}";
        }
    }
}
