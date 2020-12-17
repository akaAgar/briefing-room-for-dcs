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

using BriefingRoom4DCSWorld.Debug;

namespace BriefingRoom4DCSWorld.DB
{
    /// <summary>
    /// Stores information about a coalition.
    /// </summary>
    public class DBEntryObjective : DBEntry
    {
        /// <summary>
        /// Randomly-parsed (<see cref="Toolbox.ParseRandomString(string)"/>) multi-line description of the mission in the mission briefing.
        /// </summary>
        public string BriefingDescription { get; private set; }

        /// <summary>
        /// Randomly-parsed (<see cref="Toolbox.ParseRandomString(string)"/>) multi-line description of the mission in the mission briefing.
        /// By target family. If no value is found, the default <see cref="BriefingDescription"/> will be used.
        /// Should probably be used with <see cref="DBEntryObjectiveFlags.SingleTargetUnitFamily"/>, but if multiple target families are
        /// present, if will check the family of the first objective.
        /// </summary>
        public string[] BriefingDescriptionByUnitFamily { get; private set; }

        /// <summary>
        /// Randomly-parsed (<see cref="Toolbox.ParseRandomString(string)"/>) single-line remarks to add to the mission briefing.
        /// </summary>
        public string[] BriefingRemarks { get; private set; }

        /// <summary>
        /// Randomly-parsed (<see cref="Toolbox.ParseRandomString(string)"/>) single-line description of the task in the mission briefing.
        /// </summary>
        public string BriefingTask { get; private set; }

        /// <summary>
        /// Short description of the task, as it will appear in the flight group table.
        /// </summary>
        public string BriefingTaskFlightGroup { get; private set; }

        /// <summary>
        /// Special flags for this objective.
        /// </summary>
        public DBEntryObjectiveFlags Flags { get; private set; }

        /// <summary>
        /// Mission features to enable when this objective is selected.
        /// </summary>
        public string[] MissionFeatures { get; private set; }

        /// <summary>
        /// Mission payload to use when this objective is selected.
        /// </summary>
        public UnitTaskPayload Payload { get; private set; }

        /// <summary>
        /// Unit group to spawn when this objective is selected.
        /// </summary>
        public DBUnitGroup UnitGroup { get; private set; }

        /// <summary>
        /// Min/max distance between the waypoint and the target, in nautical miles.
        /// </summary>
        public MinMaxI WaypointInaccuracy { get; private set; }

        /// <summary>
        /// Should the waypoint be spawned on the ground?
        /// </summary>
        public bool WaypointOnGround { get; private set; }

        /// <summary>
        /// Loads a database entry from an .ini file.
        /// </summary>
        /// <param name="iniFilePath">Path to the .ini file where entry inforation is stored</param>
        /// <returns>True is successful, false if an error happened</returns>
        protected override bool OnLoad(string iniFilePath)
        {
            using (INIFile ini = new INIFile(iniFilePath))
            {
                BriefingDescription = ini.GetValue<string>("Briefing", "Description");
                BriefingDescriptionByUnitFamily = new string[Toolbox.EnumCount<UnitFamily>()];
                foreach (UnitFamily family in Toolbox.GetEnumValues<UnitFamily>())
                    BriefingDescriptionByUnitFamily[(int)family] = ini.GetValue<string>("Briefing", $"Description.{family}");

                BriefingRemarks = ini.GetValueArray<string>("Briefing", "Remarks");
                BriefingTask = ini.GetValue<string>("Briefing", "Task");
                BriefingTaskFlightGroup = ini.GetValue<string>("Briefing", "Task.FlightGroup");

                Flags = ini.GetValueArrayAsEnumFlags<DBEntryObjectiveFlags>("Objective", "Flags");
                MissionFeatures = GetValidDBEntryIDs<DBEntryMissionFeature>(
                    ini.GetValueArray<string>("Objective", "MissionFeatures"), out string[] invalidFeatures);
                foreach (string f in invalidFeatures)
                    DebugLog.Instance.WriteLine($"Mission feature \"{f}\" not found in objective \"{ID}\"", DebugLogMessageErrorLevel.Warning);

                Payload = ini.GetValue<UnitTaskPayload>("Objective", "Payload");

                UnitGroup = new DBUnitGroup(ini, "UnitGroup");

                WaypointInaccuracy = ini.GetValue<MinMaxI>("Waypoint", "Inaccuracy");
                WaypointOnGround = ini.GetValue<bool>("Waypoint", "OnGround");
            }

            return true;
        }

        /// <summary>
        /// Returns a valid unit family for this objective, or defaults to <see cref="UnitFamily.VehicleTransport"/>
        /// </summary>
        /// <returns>An unit family</returns>
        public UnitFamily GetRandomUnitFamily()
        {
            return  (UnitGroup.Families.Length > 0) ? Toolbox.RandomFrom(UnitGroup.Families) : UnitFamily.VehicleTransport;
        }
    }
}
