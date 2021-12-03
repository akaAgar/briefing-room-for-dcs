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

using BriefingRoom4DCS.Template;
using System;
using System.IO;

namespace BriefingRoom4DCS.Data
{
    internal class DatabaseCommon : IDisposable
    {
        internal string[] CommonOGG { get; private set; }

        internal DBCommonAirDefense AirDefense { get; private set; }

        internal DBCommonCAP CAP { get; private set; }

        internal DBCommonCarrierGroup CarrierGroup { get; private set; }

        internal DBCommonNames Names { get; private set; }

        internal DBCommonWind[] Wind { get; private set; }

        internal DatabaseCommon() { }

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
            }

            BriefingRoom.PrintToLog("Loading common air defense settings...");
            AirDefense = new DBCommonAirDefense();

            BriefingRoom.PrintToLog("Loading common CAP settings...");
            CAP = new DBCommonCAP();

            BriefingRoom.PrintToLog("Loading common carrier group settings...");
            CarrierGroup = new DBCommonCarrierGroup();

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

        public void Dispose() { }
    }
}