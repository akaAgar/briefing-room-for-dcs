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

using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    internal class MissionGeneratorFrontLine
    {
        internal static void GenerateFrontLine(DCSMission mission, MissionTemplateRecord template, Coordinates playerAirbase, Coordinates objectiveCenter, Coalition playerCoalition, DBEntrySituation situationDB, ref UnitMaker unitMaker)
        {
            if (template.OptionsMission.Contains("SpawnAnywhere") || template.ContextSituation == "None" || template.OptionsMission.Contains("NoFrontLine"))
                return;
    
            var frontLineCenter = Coordinates.Lerp(playerAirbase, objectiveCenter, GetLerpDistance(template));

            var objectiveHeading = playerAirbase.GetHeadingFrom(objectiveCenter);
            var angleVariance = new MinMaxD(objectiveHeading + 45, objectiveHeading + 135);
            var frontLineList = new List<Coordinates> { frontLineCenter };

            var blueZones = situationDB.GetBlueZones(false);
            var redZones = situationDB.GetRedZones(false);
            var biasZones = ShapeManager.IsPosValid(frontLineCenter, blueZones) ? blueZones : (ShapeManager.IsPosValid(frontLineCenter, redZones) ? redZones : Toolbox.RandomFrom(blueZones, redZones));

            for (int i = 0; i < 10; i++)
            {
                frontLineList.Insert(0, CreatePoint(frontLineList, angleVariance, biasZones, true));
                frontLineList.Add(CreatePoint(frontLineList, angleVariance, biasZones, false));
            }

            mission.MapData.Add("FRONTLINE", frontLineList.Select(x => x.ToArray()).ToList());
            unitMaker.SpawnPointSelector.SetFrontLine(frontLineList, playerAirbase, playerCoalition);
        }

        private static Coordinates CreatePoint(List<Coordinates> frontLineList, MinMaxD angleVariance, List<List<Coordinates>> biasPoints, bool preCenter)
        {
            var angle = angleVariance.GetValue();
            if (preCenter)
                angle = angle - 180;
            var refPoint = preCenter ? frontLineList.First() : frontLineList.Last();
            var point = Coordinates.FromAngleAndDistance(refPoint, new MinMaxD(2.5 * Toolbox.NM_TO_METERS, 5 * Toolbox.NM_TO_METERS), angle);
            if (biasPoints.Count > 0)
            {
                var nearest = biasPoints.Select(x => ShapeManager.GetNearestPointBorder(point, x)).MinBy(x => x.Item1).Item2;
                point = Coordinates.Lerp(point, nearest, Toolbox.RandomDouble(0, 0.25));
            }
            return point;
        }

        private static double GetLerpDistance(MissionTemplateRecord template) {
            var friendlySideObjectivesCount = 0;
            var enemySideObjectivesCount = 0;

            template.Objectives.ForEach(x => {
                if(MissionGeneratorObjectives.GetObjectiveData(x).taskDB.TargetSide == Side.Ally)
                    friendlySideObjectivesCount++;
                else
                    enemySideObjectivesCount++;
                });

            var lerpDistance = Toolbox.RandomDouble(0.2, 1.1);
            var bias = friendlySideObjectivesCount - enemySideObjectivesCount;
            if(bias < 1)
                lerpDistance += bias * 0.05;
            else
                lerpDistance += bias * 0.1;

            return double.Min(double.Max(lerpDistance, 0.2), 1.3);
        }

    }
}
