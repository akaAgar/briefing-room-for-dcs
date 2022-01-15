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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BriefingRoom4DCS.Data
{
    internal class DBEntryUnitAircraftData
    {
        private const Decade DEFAULT_PAYLOAD_DECADE = Decade.Decade2000;

        internal const int MAX_PYLONS = 24;

        internal int[] AirToAirRating { get; private set; } = new int[] { 1, 1 };

        internal CarrierType[] CarrierTypes { get; private set; } = new CarrierType[0];

        internal int CruiseSpeed { get; private set; } = (int)(300 * Toolbox.KNOTS_TO_METERS_PER_SECOND);

        internal int CruiseAltitude { get; private set; } = (int)(15000 * Toolbox.FEET_TO_METERS);

        internal string PayloadCommon { get; private set; } = "";

        internal Dictionary<string, string[]> PayloadTasks { get; private set; } = new Dictionary<string, string[]>();

        internal bool PlayerControllable { get; private set; } = false;

        internal double RadioFrequency { get; private set; } = 127.0;

        internal int MinimumRunwayLengthFt { get; private set; }

        internal void Merge(DBEntryUnitAircraftData aircraftData)
        {
            Liveries = Liveries.Union(aircraftData.Liveries).ToList();
            PayloadTasks = PayloadTasks
                .Union(aircraftData.PayloadTasks)
                .GroupBy(g => g.Key)
                .ToDictionary(pair => pair.Key, pair => pair.Last().Value);
        }

        internal RadioModulation RadioModulation { get; private set; } = RadioModulation.AM;

        internal List<DBEntryUnitRadioPreset> RadioPresets { get; private set; }

        internal string PropsLua { get; private set; } = "";

        internal List<string> Liveries { get; private set; }
        internal DBEntryUnitAircraftData() { }

        internal DBEntryUnitAircraftData(INIFile ini, bool custom = false)
        {
            AirToAirRating[0] = Math.Max(1, ini.GetValue<int>("Aircraft", "A2ARating.Default"));
            AirToAirRating[1] = Math.Max(1, ini.GetValue<int>("Aircraft", "A2ARating.AirToAir"));

            CarrierTypes = ini.GetValueArray<CarrierType>("Aircraft", "CarrierTypes").Distinct().ToArray();

            CruiseAltitude = (int)(Math.Max(0, ini.GetValue<int>("Aircraft", "CruiseAltitude")) * Toolbox.FEET_TO_METERS);
            CruiseSpeed = (int)(Math.Max(0, ini.GetValue<int>("Aircraft", "CruiseSpeed")) * Toolbox.KNOTS_TO_METERS_PER_SECOND);

            PlayerControllable = ini.GetValue<bool>("Aircraft", "PlayerControllable");
            PropsLua = ini.GetValue<string>("Aircraft", "PropsLua");

            RadioFrequency = ini.GetValue<double>("Aircraft", "Radio.Frequency");
            RadioModulation = ini.GetValue<RadioModulation>("Aircraft", "Radio.Modulation");

            PayloadCommon = ini.GetValue<string>("Aircraft", "Payload.Common");

            MinimumRunwayLengthFt = ini.GetValue<int>("Aircraft", "MinimumRunwayLengthFt");

            var payloads = ini.GetKeysInSection("Aircraft").Where(x => x.StartsWith("payload.task")).Select(x => x.Split('.')[2]).Distinct().ToList();

            if (!custom)
                PayloadTasks.Add("default", new string[MAX_PYLONS]);

            foreach (string task in payloads)
            {
                if (task != "default" || custom)
                    PayloadTasks.Add(task, new string[MAX_PYLONS]);
                for (var pylonIndex = 0; pylonIndex < MAX_PYLONS; pylonIndex++)
                    PayloadTasks[task][pylonIndex] = ini.GetValue<string>("Aircraft", $"Payload.Task.{task}.Pylon{pylonIndex + 1:00}");
            }

            RadioPresets = new List<DBEntryUnitRadioPreset>();
            for (int i = 0; i < 4; i++)
            {
                if (ini.ValueExists("Aircraft", $"Radio.Presets.{i}.Channels"))
                    RadioPresets.Add(
                        new DBEntryUnitRadioPreset(
                            ini.GetValueArray<int>("Aircraft", $"Radio.Presets.{i}.Channels"),
                            ini.GetValueArray<int>("Aircraft", $"Radio.Presets.{i}.Modulations")));
            }
            Liveries = new List<string>{
                "default"
            };
            Liveries.AddRange(ini.GetValueArray<string>("Aircraft", "Liveries"));
        }

        internal string GetRadioAsString()
        {
            return $"{RadioFrequency.ToString("F1", NumberFormatInfo.InvariantInfo)} {RadioModulation}";
        }

        internal string GetPayloadLua(string aircraftPayload)
        {
            string[] payload;
            aircraftPayload = aircraftPayload.ToLower();
            if (TaskPayloadExists(aircraftPayload))
                payload = PayloadTasks[aircraftPayload];
            else
                payload = PayloadTasks["default"];

            string pylonsLua = "";
            for (int i = 0; i < MAX_PYLONS; i++)
            {
                string pylonCode = payload[i];
                if (!string.IsNullOrEmpty(pylonCode))
                    pylonsLua += $"[{i + 1}] = {{[\"CLSID\"] = \"{pylonCode}\" }},\r\n";
            }

            return pylonsLua;
        }

        private bool TaskPayloadExists(string task) => PayloadTasks.ContainsKey(task);
    }
}

