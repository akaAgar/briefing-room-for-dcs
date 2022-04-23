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

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BriefingRoom4DCS
{
    internal static class BRPaths
    {
        internal static string ROOT { get; } = FindRoot();

        internal static string DATABASE { get; } = $"{ROOT}Database\\";

        internal static string CUSTOMDATABASE { get; } = $"{ROOT}CustomConfigs\\";

#if DEBUG
        internal static string DEBUGOUTPUT { get; } = $"{ROOT}DebugOutput\\";
#endif

        internal static string INCLUDE { get; } = $"{ROOT}Include\\";

        internal static string INCLUDE_HTML { get; } = $"{INCLUDE}Html\\";

        internal static string INCLUDE_JPG { get; } = $"{INCLUDE}Jpg\\";

        internal static string INCLUDE_LUA { get; } = $"{INCLUDE}Lua\\";

        internal static string INCLUDE_MARKDOWN { get; } = $"{INCLUDE}Markdown\\";

        internal static string INCLUDE_YAML { get; } = $"{INCLUDE}Yaml\\";


        internal static string INCLUDE_LUA_MISSIONFEATURES { get; } = $"{INCLUDE_LUA}MissionFeatures\\";

        internal static string INCLUDE_LUA_OPTIONSMISSION { get; } = $"{INCLUDE_LUA}OptionsMission\\";

        internal static string INCLUDE_LUA_OBJECTIVEFEATURES { get; } = $"{INCLUDE_LUA}ObjectiveFeatures\\";

        internal static string INCLUDE_LUA_OBJECTIVETRIGGERS { get; } = $"{INCLUDE_LUA}ObjectiveTriggers\\";

        internal static string INCLUDE_LUA_MISSION { get; } = $"{INCLUDE_LUA}Mission\\";

        internal static string INCLUDE_LUA_UNITS { get; } = $"{INCLUDE_LUA}Units\\";

        internal static string INCLUDE_YAML_UNIT { get; } = $"{INCLUDE_YAML}Unit\\";

        internal static string INCLUDE_YAML_GROUP { get; } = $"{INCLUDE_YAML}Group\\";

        internal static string INCLUDE_OGG { get; } = $"{INCLUDE}Ogg\\";

        internal static string MEDIA { get; } = $"{ROOT}Media\\";

        internal static string MEDIA_ICONS16 { get; } = $"{MEDIA}Icons16\\";

        private static string FindRoot(string path = "", int loop = 0)
        {
            if (string.IsNullOrEmpty(path))
                path = AppDomain.CurrentDomain.BaseDirectory;
            path = Path.GetFullPath(path);
            DirectoryInfo di = new DirectoryInfo(path);
            var directories = di.GetDirectories();
            if (directories.Any(x => x.Name == "Database"))
                return Toolbox.NormalizeDirectoryPath(path);
            if (loop > 10)
                throw new Exception("Can't Find Database within 10 levels up on app");
            return FindRoot(path + "/..", loop + 1);
        }
    }
}
