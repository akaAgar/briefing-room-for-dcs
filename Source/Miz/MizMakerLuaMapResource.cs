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

using System;

namespace BriefingRoom4DCSWorld.Miz
{
    /// <summary>
    /// Creates the "l10n/DEFAULT/mapResource" entry in the MIZ file.
    /// </summary>
    public class MizMakerLuaMapResource : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MizMakerLuaMapResource() { }

        /// <summary>
        /// Generates the content of the Lua file.
        /// </summary>
        /// <param name="resourceOggString">An string containing the Lua declaring all embedded .ogg files.</param>
        /// <returns>The contents of the Lua file.</returns>
        public string MakeLua(string resourceOggString)
        {
            string lua = LuaTools.ReadIncludeLuaFile("MapResource.lua");

            LuaTools.ReplaceKey(ref lua, "OggFiles", resourceOggString);

            return lua;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
