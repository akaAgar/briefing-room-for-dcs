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

using BriefingRoom4DCSWorld.DB;
using BriefingRoom4DCSWorld.Debug;
using BriefingRoom4DCSWorld.Mission;
using BriefingRoom4DCSWorld.Template;
using System;
using System.ComponentModel;

namespace BriefingRoom4DCSWorld.Generator
{
    /// <summary>
    /// Generates Ally surface-to-air defense unit groups,
    /// except "embedded" air defense, which is generated at the same time as the group objectives.
    /// </summary>
    public class MissionGeneratorAllyAirDefense : IDisposable
    {
        /// <summary>
        /// Unit maker class to use to generate units.
        /// </summary>
        private readonly UnitMaker UnitMaker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker class to use to generate units</param>
        public MissionGeneratorAllyAirDefense(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
        }

        /// <summary>
        /// Main unit generation method.
        /// </summary>
        /// <param name="mission">Mission to which generated units should be added</param>
        /// <param name="template">Mission template to use</param>
        /// <param name="allyCoalitionDB">Ally coalition database entry</param>
        public void CreateUnitGroups(DCSMission mission, MissionTemplate template, DBEntryCoalition allyCoalitionDB)
        {
            foreach (AirDefenseRange airDefenseRange in (AirDefenseRange[])Enum.GetValues(typeof(AirDefenseRange)))
            {
                DebugLog.Instance.WriteLine($"Adding {Toolbox.SplitCamelCase(airDefenseRange)} air defense", 1);
                AddAirDefenseUnits(mission, template, airDefenseRange, allyCoalitionDB);
            }
        }

        /// <summary>
        /// Add surface-to-air defense groups.
        /// </summary>
        /// <param name="mission">Mission to which generated units should be added</param>
        /// <param name="template">Mission template to use</param>
        /// <param name="airDefenseRange">Air-defense range category</param>
        /// <param name="allyCoalitionDB">Ally coalition database entry</param>
        private void AddAirDefenseUnits(DCSMission mission, MissionTemplate template, AirDefenseRange airDefenseRange, DBEntryCoalition allyCoalitionDB)
        {
            // Get the proper number of groups
            int groupCount = Database.Instance.Common.
                AllyAirDefense[(int)template.AllyAirDefense].GroupsInArea[(int)airDefenseRange].GetValue();

            if (groupCount < 1) return;  // No groups to add, no need to go any further

            DCSMissionUnitGroupFlags flags =  0;

            UnitFamily[] unitFamilies;
            TheaterLocationSpawnPointType[] validSpawnPoints;
            switch (airDefenseRange)
            {
                default: // case AirDefenseRange.ShortRange:
                    unitFamilies = new UnitFamily[] { UnitFamily.VehicleAAA, UnitFamily.VehicleAAAStatic, UnitFamily.VehicleInfantryMANPADS, UnitFamily.VehicleSAMShort, UnitFamily.VehicleSAMShort, UnitFamily.VehicleSAMShortIR, UnitFamily.VehicleSAMShortIR };
                    validSpawnPoints = new TheaterLocationSpawnPointType[] { TheaterLocationSpawnPointType.LandSmall, TheaterLocationSpawnPointType.LandMedium, TheaterLocationSpawnPointType.LandLarge };
                    break;
                case AirDefenseRange.MediumRange:
                    unitFamilies = new UnitFamily[] { UnitFamily.VehicleSAMMedium };
                    validSpawnPoints = new TheaterLocationSpawnPointType[] { TheaterLocationSpawnPointType.LandMedium, TheaterLocationSpawnPointType.LandLarge };
                    break;
                case AirDefenseRange.LongRange:
                    unitFamilies = new UnitFamily[] { UnitFamily.VehicleSAMLong };
                    validSpawnPoints = new TheaterLocationSpawnPointType[] { TheaterLocationSpawnPointType.LandLarge };
                    break;
            }

            for (int i = 0; i < groupCount; i++)
            {
                // Find spawn point at the proper distance from the objective(s), but not to close from starting airbase
                DBEntryTheaterSpawnPoint? spawnPoint =
                    UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        validSpawnPoints,
                        mission.InitialPosition,
                        Database.Instance.Common.AllyAirDefenseDistanceFromTakeOffLocation[(int)airDefenseRange],
                        mission.ObjectivesCenter,
                        new MinMaxD(Database.Instance.Common.AllyAirDefenseDistanceFromObjectives[(int)airDefenseRange], 99999),
                        GeneratorTools.GetAllySpawnPointCoalition(template));

                // No spawn point found, stop here.
                if (!spawnPoint.HasValue)
                {
                    DebugLog.Instance.WriteLine($"No spawn point found for {airDefenseRange} air defense unit groups", 1, DebugLogMessageErrorLevel.Warning);
                    return;
                }

                string[] units = allyCoalitionDB.GetRandomUnits(Toolbox.RandomFrom(unitFamilies), 1);

                DCSMissionUnitGroup group = UnitMaker.AddUnitGroup(
                    mission, units, Side.Ally,
                    spawnPoint.Value.Coordinates,
                    "GroupVehicle", "UnitVehicle",
                    Toolbox.BRSkillLevelToDCSSkillLevel(template.PlayerAISkillLevel),
                    flags);
                
                if (group == null)
                    DebugLog.Instance.WriteLine($"Failed to add {airDefenseRange} air defense unit group of type {units[0]}", 1, DebugLogMessageErrorLevel.Warning);
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
