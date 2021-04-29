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

using BriefingRoom4DCS.Mission;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Miz
{
    /// <summary>
    /// Creates the "l10n/DEFAULT/script.lua" entry in the MIZ file.
    /// </summary>
    internal class MizMakerLuaScript : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal MizMakerLuaScript() { }

        /// <summary>
        /// Generates the content of the Lua file.
        /// </summary>
        /// <param name="mission">An HQ4DCS mission.</param>
        /// <returns>The contents of the Lua file.</returns>
        internal string MakeLua(DCSMission mission)
        {
            BriefingRoom.PrintToLog("Concatenating source Lua files...");
            string lua = LuaTools.ReadIncludeLuaFile("Script.lua");

            BriefingRoom.PrintToLog("Making replacements in the Lua files...");
            LuaTools.ReplaceKey(ref lua, "CoreLua", mission.LuaScriptObjectives.Trim('\r', '\n', ' '));
            LuaTools.ReplaceKey(ref lua, "RadioSounds", !mission.OptionsMission.Contains(Template.MissionOption.RadioMessagesTextOnly));
            //LuaTools.ReplaceKey(ref lua, "LuaSettings", mission.LuaSettings);
            LuaTools.ReplaceKey(ref lua, "IncludedLua", GetIncludedMissionFeaturesLua(mission.IncludedLuaScripts));
           
            LuaTools.ReplaceKey(ref lua, "EnemyCoalition", $"coalition.side.{mission.CoalitionEnemy.ToString().ToUpperInvariant()}");
            LuaTools.ReplaceKey(ref lua, "PlayerCoalition", $"coalition.side.{mission.CoalitionPlayer.ToString().ToUpperInvariant()}");
            //PLAYER TODO actually generate waypoint data
            LuaTools.ReplaceKey(ref lua, "PlayerWaypoints", GetPlayerWaypoints(mission.Waypoints));
            LuaTools.ReplaceKey(ref lua, "StaticObjective", mission.ObjectiveIsStatic);
            //LuaTools.ReplaceKey(ref lua, "EndMode", (int)mission.EndMode);

            return lua;
        }

        /// <summary>
        /// Reads all scripts to be included from the mission features and concatenate them into a string ready to be insert in the
        /// mission script file.
        /// Scripts should be located in the Include\Lua\MissionFeatures directory.
        /// </summary>
        /// <param name="luaFiles">List of Lua files to include, without the .lua extension</param>
        /// <returns>A string</returns>
        private string GetIncludedMissionFeaturesLua(List<string> luaFiles)
        {
            string lua = "";

            foreach (string f in luaFiles)
            {
                string filePath = $"{BRPaths.INCLUDE_LUA_MISSIONFEATURES}{f}.lua";
                if (!File.Exists(filePath)) continue;
                lua += File.ReadAllText(filePath) + "\n";
            }

            return lua;
        }

        private string GetPlayerWaypoints(List<DCSMissionWaypoint> waypoints)
        {
            var output = new List<string>();
            foreach (var waypoint in waypoints)
            {
                output.Add($"{{name = \"{waypoint.Name}\", location = {{x = {waypoint.Coordinates.X}, y = 0, z = {waypoint.Coordinates.Y}}}}}");
            }
            return $"{{{string.Join(",",output)}}}";
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
