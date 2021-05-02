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

using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    /// <summary>
    /// Generates list of countries for each coalition.
    /// </summary>
    internal class MissionGeneratorCountries : IDisposable
    {
        /// <summary>
        /// Default country for <see cref="Coalition.Blue"/> (#0) and <see cref="Coalition.Red"/> (#1) coalitions.
        /// </summary>
        private static readonly Country[] DEFAULT_COUNTRIES = new Country[] { Country.CJTFBlue, Country.CJTFRed };

        /// <summary>
        /// Constructor.
        /// </summary>
        internal MissionGeneratorCountries() { }

        /// <summary>
        /// IDispose implementation.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Generates list of countries for each coalition.
        /// </summary>
        /// <param name="mission">Mission to generate.</param>
        /// <param name="template">Mission template to use.</param>
        internal void GenerateCountries(DCSMission mission, MissionTemplate template)
        {
            int i;

            List<Country>[] countries = new List<Country>[] { new List<Country>(), new List<Country>() };

            // Add default country for each coalition
            for (i = 0; i < 2; i++)
                countries[i].Add(DEFAULT_COUNTRIES[i]);

            // Add countries for player FGs to player coalition
            foreach (MissionTemplateFlightGroup flightGroup in template.PlayerFlightGroups)
                countries[(int)template.ContextPlayerCoalition].Add(flightGroup.Country);

            // Removes countries added multiple times
            countries[(int)template.ContextPlayerCoalition] = countries[(int)template.ContextPlayerCoalition].Distinct().ToList();

            // Make sure each country doesn't contain the other's coalition default country
            for (i = 0; i < 2; i++)
                if (countries[i].Contains(DEFAULT_COUNTRIES[1 - i]))
                    countries[i].Remove(DEFAULT_COUNTRIES[1 - i]);

            // Add all non-aligned countries to the list of neutral countries
            List<Country> neutralCountries = new List<Country>(Toolbox.GetEnumValues<Country>());
            for (i = 0; i < 2; i++) neutralCountries = neutralCountries.Except(countries[i]).ToList();

            mission.SetValue("COALITION_NEUTRAL", GetCountriesLuaTable(neutralCountries));
            mission.SetValue("COALITION_BLUE", GetCountriesLuaTable(countries[(int)Coalition.Blue]));
            mission.SetValue("COALITION_RED", GetCountriesLuaTable(countries[(int)Coalition.Red]));
        }

        private string GetCountriesLuaTable(List<Country> countries)
        {
            string luaTable = "{ ";
            for (int i = 0; i < countries.Count; i++)
                luaTable += $"[{i + 1}] = {(int)countries[i]}{(i < countries.Count - 1 ? ", " : "")}";
            luaTable += " }";

            return luaTable;
        }
    }
}
