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

using BriefingRoom4DCSWorld.DB;
using BriefingRoom4DCSWorld.TypeConverters;
using System;
using System.ComponentModel;

namespace BriefingRoom4DCSWorld.Template
{
    /// <summary>
    /// A multiplayer flight group, to be stored in <see cref="MissionTemplate.PlayerMPFlightGroups"/>
    /// </summary>
    public class MissionTemplateMPFlightGroup : IDisposable
    {
        /// <summary>
        /// Type of aircraft in this flight group.
        /// </summary>
        [DisplayName("Aircraft"), Description("Type of aircraft in this flight group.")]
        [TypeConverter(typeof(DBEntryPlayerAircraftTypeConverter))]
        public string AircraftType { get { return AircraftType_; } set { AircraftType_ = TemplateTools.CheckValuePlayerAircraft(value); } }
        private string AircraftType_;

        /// <summary>
        /// Number of aircraft in this flight group.
        /// </summary>
        [DisplayName("Count"), Description("Number of aircraft in this flight group.")]
        public int Count { get { return _Count; } set { _Count = Toolbox.Clamp(value, 1, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE); } }
        private int _Count = 1;

        /// <summary>
        /// Task assigned to this flight group. Can be the mission objectives or CAP/SEAD escort.
        /// </summary>
        [DisplayName("Task"), Description("Task assigned to this flight group. Can be the mission objectives or CAP/SEAD escort.")]
        [TypeConverter(typeof(EnumTypeConverter<MissionTemplateMPFlightGroupTask>))]
        public MissionTemplateMPFlightGroupTask Task { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MissionTemplateMPFlightGroup()
        {
            Clear();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ini">The .ini file to load from</param>
        /// <param name="section">The ini section to load from</param>
        /// <param name="key">The ini key to load from</param>
        public MissionTemplateMPFlightGroup(INIFile ini, string section, string key)
        {
            Clear();
            
            AircraftType = TemplateTools.CheckValuePlayerAircraft(ini.GetValue(section, $"{key}.AircraftType", AircraftType));
            Count = ini.GetValue(section, $"{key}.Count", Count);
            Task = ini.GetValue(section, $"{key}.Task", Task);
        }

        /// <summary>
        /// Resets all settings to their default values.
        /// </summary>
        private void Clear()
        {
            AircraftType = TemplateTools.CheckValuePlayerAircraft(TemplateTools.DEFAULT_PLAYER_AIRCRAFT);
            Count = 2;
            Task = MissionTemplateMPFlightGroupTask.Objectives;
        }

        /// <summary>
        /// Saves the flight group to an .ini file.
        /// </summary>
        /// <param name="ini"></param>
        /// <param name="section">The ini section to save to</param>
        /// <param name="key">The ini key to save to</param>
        public void SaveToFile(INIFile ini, string section, string key)
        {
            ini.SetValue(section, $"{key}.AircraftType", AircraftType);
            ini.SetValue(section, $"{key}.Count", Count);
            ini.SetValue(section, $"{key}.Task", Task);
        }

        /// <summary>
        /// ToString() override.
        /// </summary>
        /// <returns>A string representing this flight group to display in the PropertyGrid.</returns>
        public override string ToString()
        {
            return $"{Toolbox.ValToString(Count)}x {AircraftType} - {Toolbox.SplitCamelCase(Task)}";
        }

        /// <summary>
        /// IDispose implementation.
        /// </summary>
        public void Dispose() { }
    }
}
