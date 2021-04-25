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

using BriefingRoom4DCS.Data;
using System;
using System.Linq;
using System.Windows.Forms;

namespace BriefingRoom4DCS.Template
{
    /// <summary>
    /// A player flight group, to be stored in <see cref="MissionTemplate.PlayerFlightGroups"/>
    /// </summary>
    public sealed class MissionTemplateFlightGroup
    {
        /// <summary>
        /// Type of aircraft in this flight group.
        /// </summary>
        public string Aircraft { get; set; }

        /// <summary>
        /// Type of carrier this flight group takes off from (leave empty for land airbase).
        /// </summary>
        public string Carrier { get; set; }

        /// <summary>
        /// Number of 
        /// </summary>
        public int Count { get { return _Count; } set { _Count = Toolbox.Clamp(value, 1, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE); } }
        private int _Count = 1;

        public Country Country { get; set; }

        public AircraftPayload Payload { get; set; }

        public PlayerStartLocation StartLocation { get; set; }

        public MissionTemplateFlightGroup()
        {
            Clear();
        }

        public MissionTemplateFlightGroup(string aircraft, int count, AircraftPayload payload, string carrier, Country country, PlayerStartLocation startLocation)
        {
            Aircraft = aircraft;
            Count = count;
            Payload = payload;
            Carrier = carrier;
            Country = country;
            StartLocation = startLocation;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ini">The .ini file to load from</param>
        /// <param name="section">The ini section to load from</param>
        /// <param name="key">The ini key to load from</param>
        internal MissionTemplateFlightGroup(INIFile ini, string section, string key)
        {
            Clear();

            Aircraft = ini.GetValue(section, $"{key}.AircraftType", Aircraft); //Database.CheckValue<DBPseudoEntryPlayerAircraft>(ini.GetValue(section, $"{key}.AircraftType", Aircraft));
            Carrier = ini.GetValue(section, $"{key}.Carrier", Carrier);
            Count = ini.GetValue(section, $"{key}.Count", Count);
            Payload = ini.GetValue(section, $"{key}.Payload", Payload);
            Country = ini.GetValue(section, $"{key}.Country", Country);
            StartLocation = ini.GetValue(section, $"{key}.StartLocation", StartLocation);
        }

        /// <summary>
        /// Resets all settings to their default values.
        /// </summary>
        private void Clear()
        {
            Aircraft = "Su-25T"; // Database.CheckValue<DBPseudoEntryPlayerAircraft>("Su-25T", "Su-25T");
            Carrier = "";
            Count = 2;
            Payload = AircraftPayload.Default;
            Country = Country.CJTFBlue;
            StartLocation = PlayerStartLocation.Runway;
        }

        /// <summary>
        /// Saves the flight group to an .ini file.
        /// </summary>
        /// <param name="ini"></param>
        /// <param name="section">The ini section to save to</param>
        /// <param name="key">The ini key to save to</param>
        internal void SaveToFile(INIFile ini, string section, string key)
        {
            ini.SetValue(section, $"{key}.AircraftType", Aircraft);
            ini.SetValue(section, $"{key}.Carrier", Carrier);
            ini.SetValue(section, $"{key}.Count", Count);
            ini.SetValue(section, $"{key}.Payload", Payload);
            ini.SetValue(section, $"{key}.Country", Country);
            ini.SetValue(section, $"{key}.StartLocation", StartLocation);
        }

        /// <summary>
        /// ToString() override.
        /// </summary>
        /// <returns>A string representing this flight group to display in the PropertyGrid.</returns>
        public override string ToString()
        {
            //string str = $"{Toolbox.ValToString(Count)}x {Aircraft}, from {Country}, tasked with {Toolbox.LowerCaseFirstLetter(GUITools.GetDisplayName(Tasking))}, starting {StartLocation}, take off from ";
            //if (string.IsNullOrEmpty(Carrier)) str += "land airbase";
            //else str += GUITools.GetDisplayName(Database, typeof(DBPseudoEntryCarrier), Carrier);

            string str = $"{Toolbox.ValToString(Count)}x {Aircraft}, from {Country}, loaded with {Toolbox.LowerCaseFirstLetter(Payload.ToString())}, starting {StartLocation}, take off from ";
            if (string.IsNullOrEmpty(Carrier)) str += "land airbase";
            else str += Carrier;

            return str;
        }

        /// <summary>
        /// IDispose implementation.
        /// </summary>
        public void Dispose() { }
    }
}
