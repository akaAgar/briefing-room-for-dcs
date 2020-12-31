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

using BriefingRoom4DCSWorld.DB;
using BriefingRoom4DCSWorld.TypeConverters;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;

namespace BriefingRoom4DCSWorld.Template
{
    /// <summary>
    /// A campaign template
    /// </summary>
    public class CampaignTemplate : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public CampaignTemplate()
        {
            Clear();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filePath">Path to the .ini file the campaign template should be read from.</param>
        public CampaignTemplate(string filePath)
        {
            LoadFromFile(filePath);
        }

        [TypeConverter(typeof(DBEntryTypeConverter<DBEntryCoalition>))]
        public string CoalitionBlue { get { return CoalitionBlue_; } set { CoalitionBlue_ = TemplateTools.CheckValue<DBEntryCoalition>(value); } }
        private string CoalitionBlue_;

        [TypeConverter(typeof(EnumTypeConverter<Coalition>))]
        public Coalition CoalitionPlayer { get; set; }

        [TypeConverter(typeof(DBEntryTypeConverter<DBEntryCoalition>))]
        public string CoalitionRed { get { return CoalitionRed_; } set { CoalitionRed_ = TemplateTools.CheckValue<DBEntryCoalition>(value); } }
        private string CoalitionRed_;

        [TypeConverter(typeof(BooleanYesNoTypeConverter))]
        public bool AllowNightMissions { get; set; }

        [TypeConverter(typeof(BooleanYesNoTypeConverter))]
        public bool AllowBadWeather { get; set; }

        [TypeConverter(typeof(StringArrayTypeConverter))]
        [Editor(typeof(CheckedListBoxUIEditorDBEntry<DBEntryObjective>), typeof(UITypeEditor))]
        public string[] ObjectiveTypes { get { return ObjectiveTypes_; } set { ObjectiveTypes_ = value.Distinct().OrderBy(x => x).ToArray(); } }
        public string[] ObjectiveTypes_;

        [TypeConverter(typeof(BooleanYesNoTypeConverter))]
        public bool OptionsShowEnemyUnits { get; set; }

        [TypeConverter(typeof(DBEntryPlayerAircraftTypeConverter))]
        public string PlayerAircraft { get { return PlayerSPAircraft_; } set { PlayerSPAircraft_ = TemplateTools.CheckValuePlayerAircraft(value); } }
        private string PlayerSPAircraft_;

        [TypeConverter(typeof(EnumTypeConverter<PlayerStartLocation>))]
        public PlayerStartLocation PlayerStartLocation { get; set; }

        [TypeConverter(typeof(DBEntryTypeConverter<DBEntryTheater>))]
        public string TheaterID { get { return TheaterID_; } set { TheaterID_ = TemplateTools.CheckValue<DBEntryTheater>(value); } }
        private string TheaterID_;

        [TypeConverter(typeof(BooleanYesNoTypeConverter))]
        public bool InvertTheaterCoalitionCountries { get; set; }

        public int NumberOfMissions { get { return NumberOfMissions_; } set { NumberOfMissions_ = Toolbox.Clamp(value, TemplateTools.MIN_CAMPAIGN_MISSIONS, TemplateTools.MAX_CAMPAIGN_MISSIONS); } }
        private int NumberOfMissions_;

        public CampaignDifficulty DifficultyFirstMission { get; set; }

        public CampaignDifficulty DifficultyLastMission { get; set; }

        /// <summary>
        /// Resets all properties to their default values.
        /// </summary>
        public void Clear()
        {
     
        }

        /// <summary>
        /// Loads a campaign template from an .ini file.
        /// </summary>
        /// <param name="filePath">Path to the .ini file</param>
        /// <returns></returns>
        public bool LoadFromFile(string filePath)
        {
            Clear();
            if (!File.Exists(filePath)) return false;

            using (INIFile ini = new INIFile(filePath))
            {
            
            }

            return true;
        }

        /// <summary>
        /// Save the campaign template to an .ini file.
        /// </summary>
        /// <param name="filePath">Path to the .ini file.</param>
        public void SaveToFile(string filePath)
        {
            using (INIFile ini = new INIFile())
            {
                ini.SaveToFile(filePath);
            }
        }

        /// <summary>
        /// "Shortcut" method to get <see cref="CoalitionBlue"/> or <see cref="CoalitionRed"/> by using a <see cref="Coalition"/> parameter.
        /// </summary>
        /// <param name="coalition">Color of the coalition to return</param>
        /// <returns><see cref="CoalitionBlue"/> or <see cref="CoalitionRed"/></returns>
        public string GetCoalition(Coalition coalition)
        {
            if (coalition == Coalition.Red) return CoalitionRed;
            return CoalitionBlue;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
