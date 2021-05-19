///*
//==========================================================================
//This file is part of Briefing Room for DCS World, a mission
//generator for DCS World, by @akaAgar
//(https://github.com/akaAgar/briefing-room-for-dcs)

//Briefing Room for DCS World is free software: you can redistribute it
//and/or modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation, either version 3 of
//the License, or (at your option) any later version.

//Briefing Room for DCS World is distributed in the hope that it will
//be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
//of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Briefing Room for DCS World.
//If not, see https://www.gnu.org/licenses/
//==========================================================================
//*/

//using BriefingRoom4DCS.Data;
//using BriefingRoom4DCS.Mission;
//using BriefingRoom4DCS.Template;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace BriefingRoom4DCS.Miz
//{
//    /// <summary>
//    /// Creates the "Mission" entry in the MIZ file.
//    /// </summary>
//    internal class MizMakerLuaMission : IDisposable
//    {
//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        internal MizMakerLuaMission()
//        {

//        }

//        /// <summary>
//        /// <see cref="IDisposable"/> implementation.
//        /// </summary>
//        public void Dispose() { }

//        /// <summary>
//        /// Generates the content of the Lua file.
//        /// </summary>
//        /// <param name="mission">An HQ4DCS mission.</param>
//        /// <returns>The contents of the Lua file.</returns>
//        internal string MakeLua(DCSMission mission)
//        {
//            int i;

//            string lua = LuaTools.ReadIncludeLuaFile("Mission.lua");

//            GeneratorTools.ReplaceKey(ref lua, "TheaterID", Database.Instance.GetEntry<DBEntryTheater>(mission.Theater).DCSID);
//            GeneratorTools.ReplaceKey(ref lua, "PlayerCoalition", mission.CoalitionPlayer.ToString().ToLowerInvariant());

//            GeneratorTools.ReplaceKey(ref lua, "DateDay", mission.DateTime.Day);
//            GeneratorTools.ReplaceKey(ref lua, "DateMonth", (int)mission.DateTime.Month + 1);
//            GeneratorTools.ReplaceKey(ref lua, "DateYear", mission.DateTime.Year);
//            GeneratorTools.ReplaceKey(ref lua, "StartTime", mission.DateTime.GetTotalSecondsSinceMidnight());

//            GeneratorTools.ReplaceKey(ref lua, "WeatherCloudsBase", mission.Weather.CloudBase);
//            GeneratorTools.ReplaceKey(ref lua, "WeatherCloudsDensity", mission.Weather.CloudsDensity);
//            //GeneratorTools.ReplaceKey(ref lua, "WeatherCloudsPrecipitation", (int)mission.Weather.CloudsPrecipitation);
//            GeneratorTools.ReplaceKey(ref lua, "WeatherCloudsThickness", mission.Weather.CloudsThickness);
//            //GeneratorTools.ReplaceKey(ref lua, "WeatherCloudsPreset", mission.Weather.CloudsPreset.ToString());

//            GeneratorTools.ReplaceKey(ref lua, "WeatherDustDensity", mission.Weather.DustDensity);
//            GeneratorTools.ReplaceKey(ref lua, "WeatherDustEnabled", mission.Weather.DustEnabled);
//            GeneratorTools.ReplaceKey(ref lua, "WeatherFogEnabled", mission.Weather.Turbulence);
//            GeneratorTools.ReplaceKey(ref lua, "WeatherFogThickness", mission.Weather.FogThickness);
//            GeneratorTools.ReplaceKey(ref lua, "WeatherFogVisibility", mission.Weather.FogVisibility);
//            GeneratorTools.ReplaceKey(ref lua, "WeatherGroundTurbulence", mission.Weather.Turbulence);
//            GeneratorTools.ReplaceKey(ref lua, "WeatherQNH", mission.Weather.QNH);
//            GeneratorTools.ReplaceKey(ref lua, "WeatherTemperature", mission.Weather.Temperature);
//            GeneratorTools.ReplaceKey(ref lua, "WeatherVisibilityDistance", mission.Weather.Visibility);
//            for (i = 0; i < 3; i++)
//            {
//                GeneratorTools.ReplaceKey(ref lua, $"WeatherWind{i + 1}", mission.Weather.WindSpeed[i]);
//                GeneratorTools.ReplaceKey(ref lua, $"WeatherWind{i + 1}Dir", mission.Weather.WindDirection[i]);
//            }
//            var neutrals = Enum.GetValues(typeof(Country)).Cast<Country>().Where(x => !mission.CoalitionCountries[(int)Coalition.Blue].Contains(x) && !mission.CoalitionCountries[(int)Coalition.Red].Contains(x));
//            GeneratorTools.ReplaceKey(ref lua, "Neutrals", Toolbox.ListToLuaString(neutrals.Cast<int>()));
//            GeneratorTools.ReplaceKey(ref lua, "Reds", Toolbox.ListToLuaString(mission.CoalitionCountries[(int)Coalition.Red].Cast<int>()));
//            GeneratorTools.ReplaceKey(ref lua, "Blues", Toolbox.ListToLuaString(mission.CoalitionCountries[(int)Coalition.Blue].Cast<int>()));

//            GeneratorTools.ReplaceKey(ref lua, "BullseyeBlueX", mission.Bullseye[(int)Coalition.Blue].X);
//            GeneratorTools.ReplaceKey(ref lua, "BullseyeBlueY", mission.Bullseye[(int)Coalition.Blue].Y);
//            GeneratorTools.ReplaceKey(ref lua, "BullseyeRedX", mission.Bullseye[(int)Coalition.Red].X);
//            GeneratorTools.ReplaceKey(ref lua, "BullseyeRedY", mission.Bullseye[(int)Coalition.Red].Y);


//            GeneratorTools.ReplaceKey(ref lua, "MissionName", mission.MissionName);
//            GeneratorTools.ReplaceKey(ref lua, "BriefingDescription", mission.BriefingTXT, true);

//            CreateUnitGroups(ref lua, mission); // Create unit groups
    
//            //BriefingRoom.PrintToLog("Building Airbase");
//            //DBEntryTheaterAirbase airbase =
//            //    (from DBEntryTheaterAirbase ab in Database.Instance.GetEntry<DBEntryTheater>(mission.Theater).Airbases
//            //    where ab.DCSID == mission.InitialAirbaseID select ab).FirstOrDefault();
//            //GeneratorTools.ReplaceKey(ref lua, "MissionAirbaseID", mission.InitialAirbaseID);
//            //GeneratorTools.ReplaceKey(ref lua, "MissionAirbaseX", airbase.Coordinates.X);
//            //GeneratorTools.ReplaceKey(ref lua, "MissionAirbaseY", airbase.Coordinates.Y);

//            // The following replacements must be performed after unit groups and player waypoints have been added
//            GeneratorTools.ReplaceKey(ref lua, "PlayerGroupID", mission.PlayerGroupID);
//            GeneratorTools.ReplaceKey(ref lua, "InitialWPName", Database.Instance.Common.Names.WPNameInitial);
//            GeneratorTools.ReplaceKey(ref lua, "FinalWPName", Database.Instance.Common.Names.WPNameFinal);

//            switch (mission.UnitGroups.Find(x => x.IsAPlayerGroup()).StartLocation)
//            {
//                default: // case PlayerStartLocation.ParkingCold
//                    GeneratorTools.ReplaceKey(ref lua, "PlayerEscortStartingAction", "From Parking Area"); // Player starts cold, AI escorts start cold too
//                    GeneratorTools.ReplaceKey(ref lua, "PlayerEscortStartingType", "TakeOffParking");
//                    GeneratorTools.ReplaceKey(ref lua, "PlayerEscortStartingAltitude", 13);
//                    break;
//                case PlayerStartLocation.ParkingHot:

//                    GeneratorTools.ReplaceKey(ref lua, "PlayerEscortStartingAction", "From Parking Area Hot"); // Player starts hot, AI escorts start hot too
//                    GeneratorTools.ReplaceKey(ref lua, "PlayerEscortStartingType", "TakeOffParkingHot");
//                    GeneratorTools.ReplaceKey(ref lua, "PlayerEscortStartingAltitude", 13);
//                    break;
//                case PlayerStartLocation.Runway:

//                    GeneratorTools.ReplaceKey(ref lua, "PlayerEscortStartingAction", "Turning Point"); // Player starts on runway, AI escorts start in air above him
//                    GeneratorTools.ReplaceKey(ref lua, "PlayerEscortStartingType", "Turning Point");
//                    GeneratorTools.ReplaceKey(ref lua, "PlayerEscortStartingAltitude", 10000 * Toolbox.FEET_TO_METERS);
//                    break;
//            }

//            GeneratorTools.ReplaceKey(ref lua, "MissionID", mission.UniqueID);
//            GeneratorTools.ReplaceKey(ref lua, "ForcedOptions", GetForcedOptionsLua(mission));

//            return lua;
//        }

//        /// <summary>
//        /// Returns a string of Lua containing the forced mission options.
//        /// </summary>
//        /// <param name="mission">The DCS mission for which forced options should be generated</param>
//        /// <returns>Values for a Lua table, as a string</returns>
//        private string GetForcedOptionsLua(DCSMission mission)
//        {
//            string forcedOptionsLua = "";

//            foreach (RealismOption realismOption in mission.OptionsRealism)
//            {
//                switch (realismOption)
//                {
//                    case RealismOption.BirdStrikes: forcedOptionsLua += "[\"birds\"] = 300,"; break;
//                    case RealismOption.HideLabels: forcedOptionsLua += "[\"labels\"] = 0,"; break;
//                    case RealismOption.NoBDA: forcedOptionsLua += "[\"RBDAI\"] = false,\r\n"; break;
//                    case RealismOption.NoCheats: forcedOptionsLua += "[\"immortal\"] = false, [\"fuel\"] = false, [\"weapons\"] = false,"; break;
//                    case RealismOption.NoCrashRecovery: forcedOptionsLua += "[\"permitCrash\"] = false,"; break;
//                    case RealismOption.NoEasyComms: forcedOptionsLua += "[\"easyCommunication\"] = false,"; break;
//                    case RealismOption.NoExternalViews: forcedOptionsLua += "[\"externalViews\"] = false,"; break;
//                    case RealismOption.NoGameMode: forcedOptionsLua += "[\"easyFlight\"] = false, [\"easyRadar\"] = false,"; break;
//                    case RealismOption.NoOverlays: forcedOptionsLua += "[\"miniHUD\"] = false, [\"cockpitStatusBarAllowed\"] = false,"; break;
//                    case RealismOption.NoPadlock: forcedOptionsLua += "[\"padlock\"] = false,"; break;
//                    case RealismOption.RandomFailures: forcedOptionsLua += "[\"accidental_failures\"] = true,"; break;
//                    case RealismOption.RealisticGEffects: forcedOptionsLua += "[\"geffect\"] = \"realistic\","; break;
//                    case RealismOption.WakeTurbulence: forcedOptionsLua += "[\"wakeTurbulence\"] = true,"; break;
//                }
//            }

//            // Some realism options are forced OFF when not explicitely enabled
//            if (!mission.OptionsRealism.Contains(RealismOption.BirdStrikes))
//                forcedOptionsLua += "[\"birds\"] = 0,";
//            else if (!mission.OptionsRealism.Contains(RealismOption.RandomFailures))
//                forcedOptionsLua += "[\"accidental_failures\"] = false,";
//            else if (!mission.OptionsRealism.Contains(RealismOption.NoBDA))
//                forcedOptionsLua += "[\"RBDAI\"] = true,";

//            forcedOptionsLua += $"[\"radio\"] = {(mission.OptionsRealism.Contains(RealismOption.DisableDCSRadioAssists) ? "false" : "true")},";

//            //if (mission.CivilianTraffic == CivilianTraffic.Off)
//            //    forcedOptionsLua += "[\"civTraffic\"] = \"\",";
//            //else
//            //    forcedOptionsLua += $"[\"civTraffic\"] = \"{mission.CivilianTraffic.ToString().ToLowerInvariant()}\",";

//            //if (mission.RealismOptions.Contains(RealismOption.MapDisableAll))
//            //    forcedOptionsLua += "[\"optionsView\"] = \"optview_onlymap\",";
//            //else if(mission.RealismOptions.Contains(RealismOption.MapDisableAllButSelf))
//            //    forcedOptionsLua += "[\"optionsView\"] = \"optview_myaircraft\",";
//            //else if (mission.RealismOptions.Contains(RealismOption.MapDisableAllButUnitsKnown))
//            //    forcedOptionsLua += "[\"optionsView\"] = \"optview_allies\",";

//            return forcedOptionsLua;
//        }

//        private void CreatePlayerWaypoints(ref string groupLua, DCSMission mission, DBEntryUnit unitBP)
//        {
//            string waypointLua = LuaTools.ReadIncludeLuaFile("Mission\\WaypointPlayer.lua");
//            string allWaypointsLua = "";

//            for (int i = 0; i < mission.Waypoints.Count; i++)
//            {
//                string waypoint = waypointLua;
//                GeneratorTools.ReplaceKey(ref waypoint, "Index", i + 2);
//                GeneratorTools.ReplaceKey(ref waypoint, "Name", mission.Waypoints[i].Name);
//                GeneratorTools.ReplaceKey(ref waypoint, "X", mission.Waypoints[i].Coordinates.X);
//                GeneratorTools.ReplaceKey(ref waypoint, "Y", mission.Waypoints[i].Coordinates.Y);

//                double waypointAltitude, waypointSpeed;
//                if (unitBP == null) // Unit not found in the database, use default values for the unit category
//                {
//                    waypointAltitude = ((unitBP.Category == UnitCategory.Helicopter) ? 1500 : 20000) * Toolbox.FEET_TO_METERS;
//                    waypointSpeed = ((unitBP.Category == UnitCategory.Helicopter) ? 90 : 300) * Toolbox.KNOTS_TO_METERS_PER_SECOND;
//                }
//                else
//                {
//                    waypointAltitude = unitBP.AircraftData.CruiseAltitude;
//                    waypointSpeed = unitBP.AircraftData.CruiseAltitude;
//                }

//                waypointAltitude *= mission.Waypoints[i].AltitudeMultiplier;
//                waypointSpeed *= mission.Waypoints[i].SpeedMultiplier;

//                GeneratorTools.ReplaceKey(ref waypoint, "Altitude", waypointAltitude);
//                GeneratorTools.ReplaceKey(ref waypoint, "Speed", waypointSpeed);

//                allWaypointsLua += waypoint + "\n";
//            }

//            GeneratorTools.ReplaceKey(ref groupLua, "PlayerWaypoints", allWaypointsLua);
//            GeneratorTools.ReplaceKey(ref groupLua, "LastPlayerWaypointIndex", mission.Waypoints.Count + 2);
//        }

//        private void CreateUnitGroups(ref string lua, DCSMission mission)
//        {
//            int i, j;

//            for (i = 0; i < 2; i++){
//                string coalitionLua = "";
//                var k = 1;
//                foreach (var country in (Coalition)i == Coalition.Blue? mission.CoalitionCountries[(int)Coalition.Blue]: mission.CoalitionCountries[(int)Coalition.Red])
//                {
//                    string countryLua = LuaTools.ReadIncludeLuaFile($"Country");
//                    GeneratorTools.ReplaceKey(ref countryLua, "Index", k);
//                    GeneratorTools.ReplaceKey(ref countryLua, "DCSID", (int)country);
//                    GeneratorTools.ReplaceKey(ref countryLua, "Name", Enum.GetName(typeof(Country), country));
//                    for (j = 0; j < Toolbox.EnumCount<UnitCategory>(); j++)
//                        CreateUnitGroups(ref countryLua, mission, (Coalition)i, (UnitCategory)j, country);
//                    coalitionLua += countryLua;
//                    k++;
//                }
//                GeneratorTools.ReplaceKey(ref lua, $"COUNTRYS{(Coalition)i}", coalitionLua);
//            }
//        }

//        private void CreateUnitGroups(ref string lua, DCSMission mission, Coalition coalition, UnitCategory unitCategory, Country country)
//        {
//            int i, j;
//            int groupIndex = 1;
//            string allGroupsLua = "";

//            foreach (DCSMissionUnitGroup group in mission.UnitGroups)
//            {
//                if ((group.Coalition != coalition) || // Group does not belong to this coalition
//                    (group.Country != country) || // Group does not match country
//                    (group.Category != unitCategory) || // Group does not belong to this unit category
//                    (group.Units.Length == 0)) // Group doesn't contain any units
//                    continue; // Ignore it

//                string groupLua = LuaTools.ReadIncludeLuaFile($"Units\\{group.LuaGroup}");
//                GeneratorTools.ReplaceKey(ref groupLua, "Index", groupIndex);
//                GeneratorTools.ReplaceKey(ref groupLua, "X", group.Coordinates.X);
//                GeneratorTools.ReplaceKey(ref groupLua, "Y", group.Coordinates.Y);
//                GeneratorTools.ReplaceKey(ref groupLua, "X2", group.Coordinates2.X);
//                GeneratorTools.ReplaceKey(ref groupLua, "Y2", group.Coordinates2.Y);
//                GeneratorTools.ReplaceKey(ref groupLua, "Hidden", group.Flags.HasFlag(DCSMissionUnitGroupFlags.Hidden));
//                GeneratorTools.ReplaceKey(ref groupLua, "ID", group.GroupID);
//                GeneratorTools.ReplaceKey(ref groupLua, "FirstUnitID", group.Units[0].ID);
//                GeneratorTools.ReplaceKey(ref groupLua, "Name", group.Name);
//                GeneratorTools.ReplaceKey(ref groupLua, "ObjectiveAirbaseID", group.AirbaseID);

//                if(group.CarrierId > 0)
//                    GeneratorTools.ReplaceKey(ref groupLua, "LinkUnit", group.CarrierId);
                
//                if (group.TACAN != null){
//                    GeneratorTools.ReplaceKey(ref groupLua, "TacanFrequency", group.TACAN.freqency);
//                    GeneratorTools.ReplaceKey(ref groupLua, "TacanCallSign", group.TACAN.callsign);
//                    GeneratorTools.ReplaceKey(ref groupLua, "TacanChannel", group.TACAN.channel);
//                    GeneratorTools.ReplaceKey(ref groupLua, "UnitID", group.Units[0].ID);
//                }
//                if(group.ILS > 0)
//                    GeneratorTools.ReplaceKey(ref groupLua, "ILS", group.ILS);


//                DBEntryUnit unitBP = Database.Instance.GetEntry<DBEntryUnit>(group.UnitID);
//                if (unitBP == null) continue; // TODO: error message?

//                // Group is a client group, requires player waypoints
//                if (group.IsAPlayerGroup())
//                {
//                    CreatePlayerWaypoints(ref groupLua, mission, unitBP);
//                    switch (group.StartLocation)
//                    {
//                        default: // case PlayerStartLocation.ParkingCold
//                            GeneratorTools.ReplaceKey(ref groupLua, "PlayerStartingAction", "From Parking Area");
//                            GeneratorTools.ReplaceKey(ref groupLua, "PlayerStartingType", "TakeOffParking");
//                            break;
//                        case PlayerStartLocation.ParkingHot:
//                            GeneratorTools.ReplaceKey(ref groupLua, "PlayerStartingAction", "From Parking Area Hot");
//                            GeneratorTools.ReplaceKey(ref groupLua, "PlayerStartingType", "TakeOffParkingHot");
//                            break;
//                        case PlayerStartLocation.Runway:
//                            GeneratorTools.ReplaceKey(ref groupLua, "PlayerStartingAction", "From Runway");
//                            GeneratorTools.ReplaceKey(ref groupLua, "PlayerStartingType", "TakeOff");
//                            break;
//                    }


//                }

//                string allUnitsLua = "";
//                for (i = 0; i < group.Units.Length; i++)
//                {
//                    string unitLua = LuaTools.ReadIncludeLuaFile($"Units\\{group.LuaUnit}");
//                    Coordinates unitCoordinates = new Coordinates(group.Coordinates);
//                    double unitHeading = 0;

//                    if ((group.Category == UnitCategory.Static) || (group.Category == UnitCategory.Vehicle))
//                    {
//                        double randomSpreadAngle = Toolbox.RandomDouble(Math.PI * 2);
//                        unitCoordinates += new Coordinates(Math.Cos(randomSpreadAngle) * 35, Math.Sin(randomSpreadAngle) * 35);
//                        unitHeading = Toolbox.RandomDouble(Math.PI * 2);
//                    }
//                    else
//                        unitCoordinates += new Coordinates(50 * i, 50 * i);

//                    if ((group.Category == UnitCategory.Helicopter) || (group.Category == UnitCategory.Plane)) // Unit is an aircraft
//                    {
//                        if (unitBP == null)
//                        {
//                            GeneratorTools.ReplaceKey(ref groupLua, "Altitude", ((group.Category == UnitCategory.Helicopter) ? 1500 : 4572) * Toolbox.RandomDouble(.8, 1.2));
//                            GeneratorTools.ReplaceKey(ref unitLua, "Altitude", ((group.Category == UnitCategory.Helicopter) ? 1500 : 4572) * Toolbox.RandomDouble(.8, 1.2));
//                            GeneratorTools.ReplaceKey(ref groupLua, "EPLRS", false);
//                            GeneratorTools.ReplaceKey(ref unitLua, "PayloadCommon", "");
//                            GeneratorTools.ReplaceKey(ref unitLua, "PayloadPylons", "");
//                            GeneratorTools.ReplaceKey(ref unitLua, "RadioPresetsLua", "");
//                            GeneratorTools.ReplaceKey(ref unitLua, "PropsLua", "");
//                            GeneratorTools.ReplaceKey(ref groupLua, "RadioBand", 0);
//                            GeneratorTools.ReplaceKey(ref groupLua, "RadioFrequency", 124.0f);
//                            GeneratorTools.ReplaceKey(ref groupLua, "Speed", ((group.Category == UnitCategory.Helicopter) ? 51 : 125) * Toolbox.RandomDouble(.9, 1.1));
//                        }
//                        else
//                        {
//                            GeneratorTools.ReplaceKey(ref groupLua, "Altitude", unitBP.AircraftData.CruiseAltitude);
//                            GeneratorTools.ReplaceKey(ref unitLua, "Altitude", unitBP.AircraftData.CruiseAltitude);
//                            GeneratorTools.ReplaceKey(ref groupLua, "EPLRS", unitBP.Flags.HasFlag(DBEntryUnitFlags.EPLRS));
//                            GeneratorTools.ReplaceKey(ref unitLua, "PayloadCommon", unitBP.AircraftData.PayloadCommon);
//                            if (group.IsAPlayerGroup()){
//                                GeneratorTools.ReplaceKey(ref unitLua, "RadioPresetsLua", string.Join("", unitBP.AircraftData.RadioPresets.Select((x, index) => $"[{index + 1}] = {x.ToLuaString()}")));
//                                GeneratorTools.ReplaceKey(ref unitLua, "PropsLua", unitBP.AircraftData.PropsLua);
//                            } else {
//                                GeneratorTools.ReplaceKey(ref unitLua, "RadioPresetsLua", "");
//                                GeneratorTools.ReplaceKey(ref unitLua, "PropsLua", "");
//                            }
//                            GeneratorTools.ReplaceKey(ref groupLua, "RadioBand", (int)unitBP.AircraftData.RadioModulation);
//                            GeneratorTools.ReplaceKey(ref groupLua, "RadioFrequency", unitBP.AircraftData.RadioFrequency);
//                            GeneratorTools.ReplaceKey(ref groupLua, "Speed", unitBP.AircraftData.CruiseSpeed);

//                            string pylonLua = "";
//                            string[] payload = unitBP.AircraftData.GetPayload(group.Payload, mission.DateTime.GetDecade());
//                            for (j = 0; j < DBEntryUnitAircraftData.MAX_PYLONS; j++)
//                            {
//                                string pylonCode = payload[j];
//                                if (!string.IsNullOrEmpty(pylonCode))
//                                    pylonLua += $"[{j + 1}] = {{[\"CLSID\"] = \"{pylonCode}\" }},\r\n";
//                            }

//                            GeneratorTools.ReplaceKey(ref unitLua, "PayloadPylons", pylonLua);
//                        }
//                    } else if((group.Category == UnitCategory.Ship)){
//                        if(group.RadioFrequency > 0){
//                        GeneratorTools.ReplaceKey(ref unitLua, "RadioBand", (int)group.RadioModulation);
//                        GeneratorTools.ReplaceKey(ref unitLua, "RadioFrequency", group.RadioFrequency * 1000000);
//                        } else {
//                            GeneratorTools.ReplaceKey(ref unitLua, "RadioBand", 0);
//                            GeneratorTools.ReplaceKey(ref unitLua, "RadioFrequency", 127.0f * 1000000);
//                        }
//                        GeneratorTools.ReplaceKey(ref groupLua, "Speed", group.Speed);
//                    }

//                    if (unitBP == null)
//                        GeneratorTools.ReplaceKey(ref unitLua, "ExtraLua", "");
//                    else
//                        GeneratorTools.ReplaceKey(ref unitLua, "ExtraLua", unitBP.ExtraLua);

//                    GeneratorTools.ReplaceKey(ref unitLua, "Callsign", group.CallsignLua); // Must be before "index" replacement, as is it is integrated in the callsign
//                    GeneratorTools.ReplaceKey(ref unitLua, "Index", i + 1);
//                    GeneratorTools.ReplaceKey(ref unitLua, "ID", group.Units[i].ID);
//                    GeneratorTools.ReplaceKey(ref unitLua, "Type", group.Units[i].Type);
//                    GeneratorTools.ReplaceKey(ref unitLua, "Name", $"{group.Name} {i + 1}");
//                    if (group.Flags.HasFlag(DCSMissionUnitGroupFlags.FirstUnitIsPlayer))
//                        GeneratorTools.ReplaceKey(ref unitLua, "Skill", (i == 0) ? DCSSkillLevel.Player : group.Skill, false);
//                    else
//                        GeneratorTools.ReplaceKey(ref unitLua, "Skill", group.Skill, false);
//                    GeneratorTools.ReplaceKey(ref unitLua, "X", group.Units[i].Coordinates.X);
//                    GeneratorTools.ReplaceKey(ref unitLua, "Y", group.Units[i].Coordinates.Y);
//                    GeneratorTools.ReplaceKey(ref unitLua, "Heading", group.Units[i].Heading);
//                    GeneratorTools.ReplaceKey(ref unitLua, "OnboardNumber", Toolbox.RandomInt(1, 1000).ToString("000"));
//                    GeneratorTools.ReplaceKey(ref unitLua, "ParkingID", group.Units[i].ParkingSpot);

//                    allUnitsLua += unitLua + "\n";
//                }
//                GeneratorTools.ReplaceKey(ref groupLua, "units", allUnitsLua);

//                allGroupsLua += groupLua + "\n";
                
//                groupIndex++;
//            }

//            GeneratorTools.ReplaceKey(ref lua, $"Groups{unitCategory}", allGroupsLua);
//        }
//    }
//}
