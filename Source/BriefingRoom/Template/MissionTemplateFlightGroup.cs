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
        /// Should all aircraft in this group except the leader be AI-controlled?
        /// </summary>
        public bool AIWingmen { get; set; }

        /// <summary>
        /// Type of carrier this flight group takes off from (empty means "use land airbase").
        /// </summary>
        public string Carrier { get; set; }

        /// <summary>
        /// Number of aircraft in the group.
        /// </summary>
        public int Count { get { return _Count; } set { _Count = Toolbox.Clamp(value, 1, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE); } }
        private int _Count = 1;

        /// <summary>
        /// Country this aircraft group belongs to (mainly used for liveries).
        /// </summary>
        public Country Country { get; set; }

        /// <summary>
        /// Payload this aircraft group will carry.
        /// </summary>
        public AircraftPayload Payload { get; set; }

        /// <summary>
        /// Start location for this flight group.
        /// </summary>
        public PlayerStartLocation StartLocation { get; set; }

        /// <summary>
        /// Returns the number of player slots in this flight group.
        /// </summary>
        public int PlayerSlots { get { return AIWingmen ? 1 : _Count; } }

        public string Livery { get; set; } = "default";

        /// <summary>
        /// Constructor.
        /// </summary>
        public MissionTemplateFlightGroup()
        {
            Clear();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aircraft">Payload this aircraft group will carry.</param>
        /// <param name="count">Number of aircraft in the group.</param>
        /// <param name="payload">Payload this aircraft group will carry.</param>
        /// <param name="carrier">Type of carrier this flight group takes off from (empty means "use land airbase").</param>
        /// <param name="country">Country this aircraft group belongs to (mainly used for liveries).</param>
        /// <param name="startLocation">Start location for this flight group.</param>
        /// <param name="aiWingmen">Should all aircraft in this group except the leader be AI-controlled?</param>
        public MissionTemplateFlightGroup(string aircraft, int count, AircraftPayload payload, string carrier, Country country, PlayerStartLocation startLocation, bool aiWingmen, string livery)
        {
            Aircraft = aircraft;
            AIWingmen = aiWingmen;
            Count = count;
            Payload = payload;
            Carrier = carrier;
            Country = country;
            StartLocation = startLocation;
            Livery = livery;
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

            Aircraft = ini.GetValue(section, $"{key}.AircraftType", Aircraft); // TODO: Database.CheckValue<DBPseudoEntryPlayerAircraft>(ini.GetValue(section, $"{key}.AircraftType", Aircraft));
            AIWingmen = ini.GetValue(section, $"{key}.AIWingmen", AIWingmen);
            Carrier = ini.GetValue(section, $"{key}.Carrier", Carrier);
            Count = ini.GetValue(section, $"{key}.Count", Count);
            Payload = ini.GetValue(section, $"{key}.Payload", Payload);
            Country = ini.GetValue(section, $"{key}.Country", Country);
            StartLocation = ini.GetValue(section, $"{key}.StartLocation", StartLocation);
            Livery = ini.GetValue(section, $"{key}.Livery", Livery);
        }

        /// <summary>
        /// Resets all settings to their default values.
        /// </summary>
        private void Clear()
        {
            Aircraft = "Su-25T"; // Database.CheckValue<DBPseudoEntryPlayerAircraft>("Su-25T", "Su-25T");
            AIWingmen = false;
            Carrier = "";
            Count = 2;
            Payload = AircraftPayload.Default;
            Country = Country.CJTFBlue;
            StartLocation = PlayerStartLocation.Runway;
            Livery = "default";
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
            ini.SetValue(section, $"{key}.AIWingmen", AIWingmen);
            ini.SetValue(section, $"{key}.Carrier", Carrier);
            ini.SetValue(section, $"{key}.Count", Count);
            ini.SetValue(section, $"{key}.Payload", Payload);
            ini.SetValue(section, $"{key}.Country", Country);
            ini.SetValue(section, $"{key}.StartLocation", StartLocation);
            ini.SetValue(section, $"{key}.Livery", Livery);
        }

        /// <summary>
        /// IDispose implementation.
        /// </summary>
        public void Dispose() { }
    }
}
