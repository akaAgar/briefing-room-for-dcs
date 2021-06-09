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
//    /// Creates the "l10n/DEFAULT/mapResource" entry in the MIZ file.
//    /// </summary>
//    internal class MizMakerLuaMapResource : IDisposable
//    {
//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        internal MizMakerLuaMapResource() { }

//        /// <summary>
//        /// Generates the content of the Lua file.
//        /// </summary>
//        /// <param name="mission">The mission from which to generate the .Miz file.</param>
//        /// <param name="resourceOggString">An string containing the Lua declaring all embedded .ogg files.</param>
//        /// <returns>The contents of the Lua file.</returns>
//        internal string MakeLua(DCSMission mission, string resourceOggString)
//        {
//            string lua = LuaTools.ReadIncludeLuaFile("MapResource.lua");

//            GeneratorTools.ReplaceKey(ref lua, "OggFiles", resourceOggString);
//            GeneratorTools.ReplaceKey(ref lua, "MissionID", mission.UniqueID);

//            return lua;
//        }

//        /// <summary>
//        /// <see cref="IDisposable"/> implementation.
//        /// </summary>
//        public void Dispose() { }
//    }
//}
