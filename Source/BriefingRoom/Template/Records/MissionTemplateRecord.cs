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

namespace BriefingRoom4DCS.Template
{
    internal sealed record MissionTemplateRecord
    {
        internal string BriefingMissionName { get; init; }
        internal string BriefingMissionDescription { get; init; }
        internal string ContextCoalitionBlue { get; init; }
        internal string ContextCoalitionRed { get; init; }
        internal Decade ContextDecade { get; init; }
        internal Coalition ContextPlayerCoalition { get; init; }
        internal string ContextTheater { get; init; }
        internal string ContextSituation { get; init; }
        internal Season EnvironmentSeason { get; init; }
        internal TimeOfDay EnvironmentTimeOfDay { get; init; }
        internal string EnvironmentWeatherPreset { get; init; }
        internal Wind EnvironmentWind { get; init; }
        internal int FlightPlanObjectiveDistance { get; init; }
        internal int FlightPlanObjectiveSeperation { get; init; }
        internal string FlightPlanTheaterStartingAirbase { get; init; }
        internal List<string> MissionFeatures { get; init; }
        internal List<string> Mods { get; init; }
        internal List<MissionTemplateObjectiveRecord> Objectives { get; init; }
        internal FogOfWar OptionsFogOfWar { get; init; }
        internal List<string> OptionsMission { get; init; }
        internal List<RealismOption> OptionsRealism { get; init; }
        internal List<MissionTemplateFlightGroupRecord> PlayerFlightGroups { get; init; }
        internal List<MissionTemplatePackage> AircraftPackages { get; init; }
        internal AmountNR SituationEnemySkill { get; init; }
        internal AmountNR SituationEnemyAirDefense { get; init; }
        internal AmountNR SituationEnemyAirForce { get; init; }
        internal AmountNR SituationFriendlySkill { get; init; }
        internal AmountNR SituationFriendlyAirDefense { get; init; }
        internal AmountNR SituationFriendlyAirForce { get; init; }

        internal MissionTemplateRecord(MissionTemplate template)
        {
            BriefingMissionName = template.BriefingMissionName;
            BriefingMissionDescription = template.BriefingMissionDescription;
            ContextCoalitionBlue = template.ContextCoalitionBlue;
            ContextCoalitionRed = template.ContextCoalitionRed;
            ContextDecade = template.ContextDecade;
            ContextPlayerCoalition = template.ContextPlayerCoalition;
            ContextTheater = template.ContextTheater;
            ContextSituation = template.ContextSituation;
            EnvironmentSeason = template.EnvironmentSeason;
            EnvironmentTimeOfDay = template.EnvironmentTimeOfDay;
            EnvironmentWeatherPreset = template.EnvironmentWeatherPreset;
            EnvironmentWind = template.EnvironmentWind;
            FlightPlanObjectiveDistance = template.FlightPlanObjectiveDistance;
            FlightPlanObjectiveSeperation = template.FlightPlanObjectiveSeperation;
            FlightPlanTheaterStartingAirbase = template.FlightPlanTheaterStartingAirbase;
            MissionFeatures = template.MissionFeatures;
            Mods = template.Mods;
            Objectives = template.Objectives.Select(x => new MissionTemplateObjectiveRecord(x)).ToList();
            OptionsFogOfWar = template.OptionsFogOfWar;
            OptionsMission = template.OptionsMission;
            OptionsRealism = template.OptionsRealism;
            PlayerFlightGroups = template.PlayerFlightGroups.Select(x => new MissionTemplateFlightGroupRecord(x)).ToList();
            AircraftPackages = template.AircraftPackages;
            SituationEnemySkill = template.SituationEnemySkill;
            SituationEnemyAirDefense = template.SituationEnemyAirDefense;
            SituationEnemyAirForce = template.SituationEnemyAirForce;
            SituationFriendlySkill = template.SituationFriendlySkill;
            SituationFriendlyAirDefense = template.SituationFriendlyAirDefense;
            SituationFriendlyAirForce = template.SituationFriendlyAirForce;
        }

        internal string GetCoalitionID(Coalition coalition)
        {
            if (coalition == Coalition.Red) return ContextCoalitionRed;
            return ContextCoalitionBlue;
        }

        internal string GetCoalitionID(Side side)
        {
            return GetCoalitionID((side == Side.Ally) ? ContextPlayerCoalition : ContextPlayerCoalition.GetEnemy());
        }

        internal int GetPlayerSlotsCount()
        {
            return PlayerFlightGroups.Aggregate(0, (acc, x) => acc + (x.AIWingmen ? 1 : x.Count));
        }
    }
}
