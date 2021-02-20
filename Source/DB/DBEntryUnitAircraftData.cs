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


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BriefingRoom4DCSWorld.DB
{
    /// <summary>
    /// Stores data for an aircraft unit.
    /// </summary>
    public class DBEntryUnitAircraftData
    {
        /// <summary>
        /// Maximum number of payload pylons per aircraft.
        /// </summary>
        public const int MAX_PYLONS = 24;

        /// <summary>
        /// How powerful this aircraft is relative to other aircraft.
        /// Used to know how many enemy aircraft should be spawned.
        /// Index #0 is the rating with a non air-to-air loadout, index #1 is the rating with an air-to-air loadout.
        /// </summary>
        public int[] AirToAirRating { get; private set; } = new int[] { 1, 1 };

        /// <summary>
        /// Type of carrier ships this aircraft can take off from/land on.
        /// </summary>
        public CarrierType[] CarrierTypes { get; private set; } = new CarrierType[0];

        /// <summary>
        /// Cruise speed, relative to this unit category.
        /// </summary>
        public Amount CruiseSpeed { get; private set; } = Amount.Average;

        /// <summary>
        /// Cruise altitude, relative to this unit category.
        /// </summary>
        public Amount CruiseAltitude { get; private set; } = Amount.Average;

        /// <summary>
        /// Common payload (fuel, chaff, flares, cannon...) for this aircraft.
        /// </summary>
        public string PayloadCommon { get; private set; } = "";

        /// <summary>
        /// Payload for each pylon and each mission type.
        /// </summary>
        private Dictionary<Decade,Dictionary<UnitTaskPayload,string[]>> PayloadTasks { get; set; } = new Dictionary<Decade,Dictionary<UnitTaskPayload,string[]>>();

        /// <summary>
        /// Is this aircraft player-controllable?
        /// </summary>
        public bool PlayerControllable { get; private set; } = false;

        /// <summary>
        /// Default radio frequency for this aircraft.
        /// </summary>
        public float RadioFrequency { get; private set; } = 127.0f;

        /// <summary>
        /// Default radio modulation for this aircraft.
        /// </summary>
        public RadioModulation RadioModulation { get; private set; } = RadioModulation.AM;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DBEntryUnitAircraftData() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ini">INI file</param>
        public DBEntryUnitAircraftData(INIFile ini)
        {

            AirToAirRating[0] = Math.Max(1, ini.GetValue<int>("Aircraft", "A2ARating.Default"));
            AirToAirRating[1] = Math.Max(1, ini.GetValue<int>("Aircraft", "A2ARating.AirToAir"));

            CarrierTypes = ini.GetValueArray<CarrierType>("Aircraft", "CarrierTypes").Distinct().ToArray();

            CruiseAltitude = ini.GetValue<Amount>("Aircraft", "CruiseAltitude");
            CruiseSpeed = ini.GetValue<Amount>("Aircraft", "CruiseSpeed");

            PlayerControllable = ini.GetValue<bool>("Aircraft", "PlayerControllable");

            RadioFrequency = ini.GetValue<float>("Aircraft", "Radio.Frequency");
            RadioModulation = ini.GetValue<RadioModulation>("Aircraft", "Radio.Modulation");

            PayloadCommon = ini.GetValue<string>("Aircraft", "Payload.Common");
            foreach (Decade decade in Enum.GetValues(typeof(Decade))){
                PayloadTasks.Add(decade, new Dictionary<UnitTaskPayload, string[]>());
                foreach (UnitTaskPayload task in Enum.GetValues(typeof(UnitTaskPayload)))
                {
                    PayloadTasks[decade].Add(task, new string[MAX_PYLONS]);
                    for (var pylonIndex = 0; pylonIndex < MAX_PYLONS; pylonIndex++)
                        PayloadTasks[decade][task][pylonIndex] = ini.GetValue<string>("Aircraft", $"Payload.{decade}.Task.{task}.Pylon{pylonIndex + 1:00}");
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="RadioFrequency"/> and <see cref="RadioModulation"/> as a nice clean string for briefings.
        /// </summary>
        /// <returns>A string with radio frequency and modulation</returns>
        public string GetRadioAsString()
        {
            return $"{RadioFrequency.ToString("F1", NumberFormatInfo.InvariantInfo)} {RadioModulation}";
        }

        /// <summary>
        /// Returns the proper payload for a given task, or the default payload if required payload is not available.
        /// </summary>
        /// <param name="taskPayload">Task the aircraft should perform</param>
        /// <returns>An array of strings describing the weapon on each pylon</returns>
        public string[] GetPayload(UnitTaskPayload taskPayload, Decade decade)
        {
            if (TaskPayloadExists(taskPayload, decade))
                return PayloadTasks[decade][taskPayload];
            
            if (TaskPayloadExists(UnitTaskPayload.Default, decade))
                return PayloadTasks[decade][UnitTaskPayload.Default];

            return PayloadTasks[Decade.Decade2000][UnitTaskPayload.Default];
        }

        /// <summary>
        /// Is a payload available for the given task?
        /// Basically used to know if the default task should be used instead of the required task.
        /// </summary>
        /// <param name="task">A task</param>
        /// <returns>True if a payload have been defined for this task, false otherwise</returns>
        private bool TaskPayloadExists(UnitTaskPayload task, Decade decade)
        {
            // If at least one pylon has been defined for this task, return true
            for (int i = 0; i < MAX_PYLONS; i++)
                if (!string.IsNullOrEmpty(PayloadTasks[decade][task][i]))
                    return true;

            return false;
        }
    }
}

