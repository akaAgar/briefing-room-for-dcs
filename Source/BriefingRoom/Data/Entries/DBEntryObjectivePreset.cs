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

namespace BriefingRoom4DCS.Data
{
    /// <summary>
    /// Stores information about an objective preset.
    /// </summary>
    internal class DBEntryObjectivePreset : DBEntry
    {
        /// <summary>
        /// <see cref="DBEntryFeatureObjective"/> to add.
        /// </summary>
        internal string[] Features { get; private set; }

        /// <summary>
        /// Miscellaneous options to apply to this mission objective.
        /// </summary>
        internal ObjectiveOption[] Options { get; private set; }

        /// <summary>
        /// The preferred payload for aircraft tasked with this preset.
        /// </summary>
        internal AircraftPayload PreferredPayload { get; private set; }

        /// <summary>
        /// Random <see cref="DBEntryObjectiveTarget"/> IDs to choose from.
        /// </summary>
        internal string[] Targets { get; private set; }

        /// <summary>
        /// Random <see cref="DBEntryObjectiveTargetBehavior"/> IDs to choose from.
        /// </summary>
        internal string[] TargetsBehaviors { get; private set; }

        /// <summary>
        /// Random <see cref="DBEntryObjectiveTask"/> IDs to choose from.
        /// </summary>
        internal string[] Tasks { get; private set; }
        
        /// <summary>
        /// Loads a database entry from an .ini file.
        /// </summary>
        /// <param name="iniFilePath">Path to the .ini file where entry inforation is stored</param>
        /// <returns>True is successful, false if an error happened</returns>
        protected override bool OnLoad(string iniFilePath)
        {
            using (INIFile ini = new INIFile(iniFilePath))
            {
                Features = Database.CheckIDs<DBEntryFeatureObjective>(ini.GetValueArray<string>("ObjectivePreset", "Features"));
                Options = ini.GetValueArray<ObjectiveOption>("ObjectivePreset", "Options");
                PreferredPayload = ini.GetValue<AircraftPayload>("ObjectivePreset", "PreferredPayload");
                Targets = Database.CheckIDs<DBEntryObjectiveTarget>(ini.GetValueArray<string>("ObjectivePreset", "Targets"));
                if (Targets.Length == 0) { BriefingRoom.PrintToLog($"No valid targets for objective preset \"{ID}\"", LogMessageErrorLevel.Warning); return false; }
                TargetsBehaviors = Database.CheckIDs<DBEntryObjectiveTargetBehavior>(ini.GetValueArray<string>("ObjectivePreset", "TargetsBehaviors"));
                if (TargetsBehaviors.Length == 0) { BriefingRoom.PrintToLog($"No valid target behaviors for objective preset \"{ID}\"", LogMessageErrorLevel.Warning); return false; }
                Tasks = Database.CheckIDs<DBEntryObjectiveTask>(ini.GetValueArray<string>("ObjectivePreset", "Tasks"));
                if (Tasks.Length == 0) { BriefingRoom.PrintToLog($"No valid tasks for objective preset \"{ID}\"", LogMessageErrorLevel.Warning); return false; }
            }

            return true;
        }
    }
}
