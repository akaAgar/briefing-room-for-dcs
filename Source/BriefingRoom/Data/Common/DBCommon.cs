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

namespace BriefingRoom4DCS.Data
{
    /// <summary>
    /// Stores miscellaneous shared settings for mission generation.
    /// </summary>
    internal class DatabaseCommon : IDisposable
    {
        /// <summary>
        /// Ogg files to include in every mission.
        /// </summary>
        internal string[] CommonOGG { get; private set; }

        /// <summary>
        /// Ogg files to include in every mission of a given game mode.
        /// </summary>
        internal string[][] CommonOGGForGameMode { get; private set; }

        /// <summary>
        /// Stores information about surface-to-air defense.
        /// </summary>
        internal DBCommonAirDefense AirDefense { get; private set; }

        /// <summary>
        /// Stores information about combat air patrols.
        /// </summary>
        internal DBCommonCAP CAP { get; private set; }

        /// <summary>
        /// Stores information about common mission names/wording.
        /// </summary>
        internal DBCommonNames Names { get; private set; }

        /// <summary>
        /// Data about wind speeds.
        /// </summary>
        internal DBCommonWind[] Wind { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        internal DatabaseCommon() { }

        /// <summary>
        /// Loads common settings from <see cref="COMMON_SETTINGS_FILE"/>
        /// </summary>
        internal void Load()
        {
            int i;

            BriefingRoom.PrintToLog("Loading common global settings...");
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}Common.ini"))
            {
                CommonOGG = ini.GetValueArray<string>("Include", "CommonOgg");
                foreach (string f in CommonOGG)
                    if (!File.Exists($"{BRPaths.INCLUDE_OGG}{f}.ogg"))
                        BriefingRoom.PrintToLog($"File \"Include\\Ogg\\{f}.ogg\" doesn't exist.", LogMessageErrorLevel.Warning);

                CommonOGGForGameMode = new string[Toolbox.EnumCount<MissionType>()][];
                for (i = 0; i < CommonOGGForGameMode.Length; i++)
                {
                    CommonOGGForGameMode[i] = ini.GetValueArray<string>("Include", $"CommonOgg.{(MissionType)i}");
                    foreach (string f in CommonOGGForGameMode[i])
                        if (!File.Exists($"{BRPaths.INCLUDE_OGG}{f}.ogg"))
                            BriefingRoom.PrintToLog($"File \"Include\\Ogg\\{f}.ogg\" doesn't exist.", LogMessageErrorLevel.Warning);
                }
            }

            BriefingRoom.PrintToLog("Loading common air defense settings...");
            AirDefense = new DBCommonAirDefense();

            BriefingRoom.PrintToLog("Loading common CAP settings...");
            CAP = new DBCommonCAP();

            BriefingRoom.PrintToLog("Loading common names settings...");
            Names = new DBCommonNames();

            BriefingRoom.PrintToLog("Loading common wind settings...");
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}Wind.ini"))
            {
                Wind = new DBCommonWind[Toolbox.EnumCount<Wind>() - 1]; // -1 because we don't want "Random"
                for (i = 0; i < Wind.Length; i++)
                    Wind[i] = new DBCommonWind(ini, ((Wind)i).ToString());
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}