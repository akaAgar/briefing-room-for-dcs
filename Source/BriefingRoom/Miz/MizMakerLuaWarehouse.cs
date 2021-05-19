///*
//==========================================================================
//This file is part of Briefing Room for DCS World, a mission
//generator for DCS World, by @akaAgar
//(https://github.com/akaAgar/briefing-room-for-dcs)

//Briefing Room for DCS World is free software: you can redistribute it
//and/or modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation, either version 3 of
//the License, or (at your option) any later version.

//Briefing Room for DCS World is distributed in the hope that it will
//be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
//of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Briefing Room for DCS World.
//If not, see https://www.gnu.org/licenses/
//==========================================================================
//*/

//using BriefingRoom4DCS.Mission;
//using System;

//namespace BriefingRoom4DCS.Miz
//{
//    /// <summary>
//    /// Creates the "Warehouses" entry in the MIZ file.
//    /// </summary>
//    internal class MizMakerLuaWarehouse : IDisposable
//    {
//        /// <summary>
//        /// The warehouse Lua template, loaded from Include\Lua
//        /// </summary>
//        private readonly string WarehouseLua;

//        /// <summary>
//        /// The Lua template for an airport warehouse, loaded from Include\Lua\Warehouses
//        /// </summary>
//        private readonly string WarehouseAirportLua;

//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        internal MizMakerLuaWarehouse()
//        {
//            WarehouseLua = LuaTools.ReadIncludeLuaFile("Warehouses.lua");
//            WarehouseAirportLua = LuaTools.ReadIncludeLuaFile("Warehouses\\Airport.lua");
//        }

//        /// <summary>
//        /// Generates the content of the Lua file.
//        /// </summary>
//        /// <param name="missHQ">An HQ4DCS mission.</param>
//        /// <returns>The contents of the Lua file.</returns>
//        internal string MakeLua(DCSMission missHQ)
//        {
//            string airportsLua = "";

//            foreach (int id in missHQ.AirbasesCoalition.Keys)
//            {
//                string apLua = WarehouseAirportLua;
//                GeneratorTools.ReplaceKey(ref apLua, "Index", id);
//                GeneratorTools.ReplaceKey(ref apLua, "Coalition", missHQ.AirbasesCoalition[id].ToString().ToUpperInvariant());
//                airportsLua += apLua + "\n";
//            }

//            string lua = WarehouseLua;
//            GeneratorTools.ReplaceKey(ref lua, "Airports", airportsLua);

//            return lua;
//        }

//        /// <summary>
//        /// <see cref="IDisposable"/> implementation.
//        /// </summary>
//        public void Dispose() { }
//    }
//}
