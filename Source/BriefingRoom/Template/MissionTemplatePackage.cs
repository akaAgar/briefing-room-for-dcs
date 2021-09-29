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

using System.Collections.Generic;
using System.Linq;
using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Generator;

namespace BriefingRoom4DCS.Template
{

    public sealed class MissionTemplatePackage: MissionTemplateGroup
    {

        public List<int> FlightGroupIndexes { get; set; }

        public List<int> ObjectiveIndexes { get; set; }

        public string StartingAirbase { get; set; }

        internal List<Waypoint> Waypoints { get; set; }

        internal DBEntryAirbase Airbase { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public MissionTemplatePackage()
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
        public MissionTemplatePackage(List<int> flightGroupIndexes, List<int> objectiveIndexes, string startingAirbase)
        {
            FlightGroupIndexes = flightGroupIndexes;
            ObjectiveIndexes = objectiveIndexes;
            StartingAirbase = startingAirbase;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ini">The .ini file to load from</param>
        /// <param name="section">The ini section to load from</param>
        /// <param name="key">The ini key to load from</param>
        internal MissionTemplatePackage(INIFile ini, string section, string key)
        {
            Clear();

            FlightGroupIndexes = ini.GetValueArray<int>(section, $"{key}.FlightGroupIndexes").ToList();
            ObjectiveIndexes = ini.GetValueArray<int>(section, $"{key}.ObjectiveIndexes").ToList();
            StartingAirbase = ini.GetValue(section, $"{key}.StartingAirbase", StartingAirbase);
        }

        /// <summary>
        /// Resets all settings to their default values.
        /// </summary>
        private void Clear()
        {
            FlightGroupIndexes = new();
            ObjectiveIndexes = new();
            StartingAirbase = "home";
        }

        /// <summary>
        /// Saves the flight group to an .ini file.
        /// </summary>
        /// <param name="ini"></param>
        /// <param name="section">The ini section to save to</param>
        /// <param name="key">The ini key to save to</param>
        internal void SaveToFile(INIFile ini, string section, string key)
        {
            ini.SetValue(section, $"{key}.FlightGroupIndexes", FlightGroupIndexes);
            ini.SetValue(section, $"{key}.ObjectiveIndexes", ObjectiveIndexes);
            ini.SetValue(section, $"{key}.StartingAirbase", StartingAirbase);
        }

        /// <summary>
        /// IDispose implementation.
        /// </summary>
        public void Dispose() { }
    }
}
