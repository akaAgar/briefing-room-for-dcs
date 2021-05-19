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
using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    internal class MissionGeneratorWarehouses : IDisposable
    {
        private static readonly string AIRPORT_TEMPLATE_FILEPATH = $"{BRPaths.INCLUDE_LUA}Warehouses\\Airport.lua";

        /// <summary>
        /// Constructor.
        /// </summary>
        internal MissionGeneratorWarehouses()
        {

        }

        internal void GenerateWarehouses(DCSMission mission)
        {
            string warehousesAirportLua = "";

            if (!File.Exists(AIRPORT_TEMPLATE_FILEPATH))
                throw new Exception("Airport warehouse template file (Include\\Lua\\Warehouses\\Airport.lua) not found.");

            string airportLuaTemplate = File.ReadAllText(AIRPORT_TEMPLATE_FILEPATH);

            foreach (int airbaseID in mission.Airbases.Keys)
            {
                string airportLua = airportLuaTemplate;
                GeneratorTools.ReplaceKey(ref airportLua, "index", airbaseID);
                GeneratorTools.ReplaceKey(ref airportLua, "coalition", mission.Airbases[airbaseID].ToString().ToUpperInvariant());

                warehousesAirportLua += airportLua + "\r\n";
            }

            mission.SetValue("WAREHOUSES_AIRPORTS", warehousesAirportLua);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }

    }
}
