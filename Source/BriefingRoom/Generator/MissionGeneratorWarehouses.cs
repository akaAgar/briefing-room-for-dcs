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

using BriefingRoom4DCS.Mission;
using System;
using System.Collections.Generic;
using System.IO;

namespace BriefingRoom4DCS.Generator
{
    internal class MissionGeneratorWarehouses
    {
        private static readonly string AIRPORT_TEMPLATE_FILEPATH = Path.Combine(BRPaths.INCLUDE_LUA, "Warehouses", "Airport.lua");

        internal static void GenerateWarehouses(ref DCSMission mission, Dictionary<string, CarrierUnitMakerGroupInfo> carrierDictionary, bool globalDynamicSpawn, bool allowHotStart)
        {
            string warehousesAirportLua = "";

            if (!File.Exists(AIRPORT_TEMPLATE_FILEPATH))
                throw new Exception("Airport warehouse template file (Include\\Lua\\Warehouses\\Airport.lua) not found.");

            string airportLuaTemplate = File.ReadAllText(AIRPORT_TEMPLATE_FILEPATH);

            foreach (int airbaseID in mission.Airbases.Keys)
            {
                string airportLua = airportLuaTemplate;
                GeneratorTools.ReplaceKey(ref airportLua, "index", airbaseID);
                GeneratorTools.ReplaceKey(ref airportLua, "coalition", mission.Airbases[airbaseID].ToString().ToUpper());
                GeneratorTools.ReplaceKey(ref airportLua, "dynamicSpawn", globalDynamicSpawn);
                GeneratorTools.ReplaceKey(ref airportLua, "allowHotStart", allowHotStart);

                warehousesAirportLua += airportLua + "\r\n";
            }

            mission.SetValue("WarehousesAirports", warehousesAirportLua);

            string warehousesCarriersLua = "";

            foreach (CarrierUnitMakerGroupInfo carrier in carrierDictionary.Values)
            {
                string carrierLua = airportLuaTemplate;
                GeneratorTools.ReplaceKey(ref carrierLua, "index", carrier.UnitMakerGroupInfo.DCSGroup.Units[0].UnitId);
                GeneratorTools.ReplaceKey(ref carrierLua, "coalition", carrier.Coalition.ToString().ToUpper());
                 GeneratorTools.ReplaceKey(ref carrierLua, "dynamicSpawn", globalDynamicSpawn);
                GeneratorTools.ReplaceKey(ref carrierLua, "allowHotStart", allowHotStart);

                warehousesCarriersLua += carrierLua + "\r\n";
            }
            mission.SetValue("WAREHOUSESCARRIERS", warehousesCarriersLua);
        }
    }
}
