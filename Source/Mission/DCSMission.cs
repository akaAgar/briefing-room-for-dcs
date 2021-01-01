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

using BriefingRoom4DCSWorld.Miz;
using System;
using System.Collections.Generic;

namespace BriefingRoom4DCSWorld.Mission
{
    /// <summary>
    /// Stores a DCS World mission, with all its unit groups, Lua scripts and parameters.
    /// Can be exported to a <see cref="Miz.MizFile"/> through use of the <see cref="ExportToMiz"/> method.
    /// </summary>
    public class DCSMission : IDisposable
    {
        /// <summary>
        /// The coalition each airbase belongs to. Key is the airbase ID in DCS World, value is the coalition.
        /// </summary>
        public Dictionary<int, Coalition> AirbasesCoalition { get; set; } = new Dictionary<int, Coalition>();

        /// <summary>
        /// List of aircraft groups to spawn during the mission.
        /// Aircraft groups are activated later on because (1) they would run out of fuel if spawned on start and (2) it's nice to face new enemy CAP units as the mission goes on
        /// </summary>
        public List<DCSMissionAircraftSpawnQueueItem> AircraftSpawnQueue { get; set; } = new List<DCSMissionAircraftSpawnQueueItem>();

        /// <summary>
        /// Mission briefing, in HTML format.
        /// </summary>
        public string BriefingHTML { get; set; } = "";

        /// <summary>
        /// Mission briefing, in raw text format, as it will be displayed in DCS World.
        /// </summary>
        public string BriefingTXT { get; set; } = "";

        /// <summary>
        /// Location of blue (index #0) and red (index #1) coalition bullseye.
        /// </summary>
        public Coordinates[] Bullseye { get; set; } = new Coordinates[] { new Coordinates(), new Coordinates() };

        /// <summary>
        /// ID of the <see cref="DB.DBEntryCoalition"/> of blue (index #0) and red (index #1) coalitions.
        /// </summary>
        public string[] Coalitions { get; set; } = new string[] { "", "" };

        /// <summary>
        /// Enemy coalition. Read-only, generated from <see cref="CoalitionPlayer"/>.
        /// </summary>
        public Coalition CoalitionEnemy { get { return (Coalition)(1 - (int)CoalitionPlayer); } }

        /// <summary>
        /// Coalition the player(s) belongs to.
        /// </summary>
        public Coalition CoalitionPlayer { get; set; } = Coalition.Blue;

        /// <summary>
        /// Core Lua script containing tables into mission objectives, etc. Will replace $CORELUA$ in l10n\DEFAULT\script.lua.
        /// </summary>
        public string CoreLuaScript { get; set; } = "";

        /// <summary>
        /// Date and time during which this mission takes place.
        /// </summary>
        public DCSMissionDateTime DateTime { get; set; } = new DCSMissionDateTime();

        /// <summary>
        /// Starting time of the mission in seconds since midnight.
        /// </summary>
        public int DateTimeTotalSeconds { get { return Toolbox.Clamp(DateTime.Hour * 3600 + DateTime.Minute * 60, 0, Toolbox.SECONDS_PER_DAY - 1); } }

        /// <summary>
        /// Core Lua script containing tables into mission objectives, etc. Will replace RADIOSOUNDS in l10n\DEFAULT\script.lua.
        /// </summary>
        public bool RadioSounds { get; set; } = true;

        /// <summary>
        /// Lua script files to include in the mission script. Will replace $INCLUDEDLUA$ in l10n\DEFAULT\script.lua.
        /// </summary>
        public List<string> IncludedLuaScripts { get; set; } = new List<string>();

        /// <summary>
        /// The internal DCS ID of the airbase the player will take off from.
        /// </summary>
        public int InitialAirbaseID { get; set; }

        /// <summary>
        /// Initial coordinates of the players, used to measure the distance to objective.
        /// </summary>
        public Coordinates InitialPosition { get; set; }

        /// <summary>
        /// Lua code to include just before the included scripts, to set parameters, etc.
        /// </summary>
        public string LuaSettings { get; set; } = "";

        /// <summary>
        /// Name/title of the mission/sortie.
        /// </summary >
        public string MissionName { get; set; } = "";

        /// <summary>
        /// An array of <see cref="DCSMissionObjective"/> describing all objectives in the mission.
        /// </summary>
        public DCSMissionObjective[] Objectives { get; set; } = new DCSMissionObjective[0];

        
        /// <summary>
        /// Time to end mission min after objective complete -1 no auto end, -2 command end
        /// </summary >
        public int EndMode {get; set;} = -1;

        /// <summary>
        /// "Center position" of the mission objectives (average of all objectives positions)
        /// </summary>
        public Coordinates ObjectivesCenter
        {
            get
            {
                if (Objectives.Length == 0) return InitialPosition;

                Coordinates center = new Coordinates();
                foreach (DCSMissionObjective objective in Objectives)
                    center += objective.Coordinates;

                return center / Objectives.Length;
            }
        }

        /// <summary>
        /// True if the objective is static object, false if it isn't (which means it's an unit)
        /// Replaces $STATICOBJECTIVE$ in Script.lua
        /// </summary>
        public bool ObjectiveIsStatic = false;

        /// <summary>
        /// Ogg vorbis audio files to include in the .miz file.
        /// </summary>
        public List<string> OggFiles { get; set; } = new List<string>();

        /// <summary>
        /// Returns the ID of the first group containing players.
        /// Mostly used to direct escort flight groups in single-player mission, should probably not be used in MP missions
        /// as not clients groups will always be populated.
        /// </summary>
        public int PlayerGroupID
        {
            get
            {
                foreach (DCSMissionUnitGroup g in UnitGroups)
                    if (g.IsAPlayerGroup()) return g.GroupID;

                return 0;
            }
        }

        /// <summary>
        /// Starting location (runway, ramp hot, ramp cold) of the player(s)
        /// </summary>
        public PlayerStartLocation PlayerStartLocation { get; set; } = PlayerStartLocation.Runway;

        /// <summary>
        /// The ID of the <see cref="TheaterBP"/> (NOT THE DCS WORLD INTERNAL THEATER ID!) for this mission.
        /// </summary>
        public string Theater { get; set; } = "";

        /// <summary>
        /// A list of <see cref="DCSMissionUnitGroup"/> describing all unit groups in the mission.
        /// </summary>
        public List<DCSMissionUnitGroup> UnitGroups { get; private set; } = new List<DCSMissionUnitGroup>();

        /// <summary>
        /// A list of <see cref="DCSMissionWaypoint"/> describing all waypoints in the mission.
        /// </summary>
        public List<DCSMissionWaypoint> Waypoints { get; private set; } = new List<DCSMissionWaypoint>();

        /// <summary>
        /// Total flight plan distance, in meters.
        /// </summary>
        public double WaypointsDistance
        {
            get
            {
                double distance = 0.0;
                Coordinates lastCoordinates = InitialPosition;
                for (int i = 0; i < Waypoints.Count; i++)
                {
                    distance += lastCoordinates.GetDistanceFrom(Waypoints[i].Coordinates);
                    lastCoordinates = Waypoints[i].Coordinates;
                }
                distance += lastCoordinates.GetDistanceFrom(InitialPosition); // Distance from last waypoint to home airbase

                return distance;
            }
        }

        /// <summary>
        /// Weather information for this mission.
        /// </summary>
        public DCSMissionWeatherInfo Weather { get; set; } = new DCSMissionWeatherInfo();

        /// <summary>
        /// Constructor.
        /// </summary>
        public DCSMission() { }

        /// <summary>
        /// Exports the mission to a MizFile, which can then be saved to a .miz file.
        /// </summary>
        /// <returns>A MizFile</returns>
        public MizFile ExportToMiz()
        {
            MizFile miz;

            using (MizMaker exporter = new MizMaker())
                miz = exporter.ExportToMizFile(this);

            return miz;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
