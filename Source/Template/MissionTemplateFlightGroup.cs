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
using BriefingRoom4DCSWorld.GUI;
using System;
using System.Linq;
using System.Windows.Forms;

namespace BriefingRoom4DCSWorld.Template
{
    /// <summary>
    /// A player flight group, to be stored in <see cref="MissionTemplate.PlayerFlightGroups"/>
    /// </summary>
    public class MissionTemplateFlightGroup : ContextMenuExpandable
    {

        public string Aircraft { get; set; }

        public string Carrier { get; set; }

        public int Count { get { return _Count; } set { _Count = Toolbox.Clamp(value, 1, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE); } }
        private int _Count = 1;

        public MissionTemplateFlightGroupTask Tasking { get; set; }

        public Country Country { get; set; }

        public PlayerStartLocation StartLocation { get; set; }

        public MissionTemplateFlightGroup()
        {
            Clear();
        }

        public MissionTemplateFlightGroup(
            string aircraft,
            int count,
            MissionTemplateFlightGroupTask tasking,
            string carrier,
            Country country,
            PlayerStartLocation startLocation)
        {
            Aircraft = aircraft;
            Count = count;
            Tasking = tasking;
            Carrier = carrier;
            StartLocation = startLocation;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ini">The .ini file to load from</param>
        /// <param name="section">The ini section to load from</param>
        /// <param name="key">The ini key to load from</param>
        public MissionTemplateFlightGroup(INIFile ini, string section, string key)
        {
            Clear();
            
            Aircraft = Database.Instance.CheckValue<DBPseudoEntryPlayerAircraft>(ini.GetValue(section, $"{key}.AircraftType", Aircraft));
            Carrier = ini.GetValue(section, $"{key}.Carrier", Carrier);
            Count = ini.GetValue(section, $"{key}.Count", Count);
            Tasking = ini.GetValue(section, $"{key}.Task", Tasking);
            Country = ini.GetValue(section, $"{key}.Country", Country);
            StartLocation = ini.GetValue(section, $"{key}.StartLocation", StartLocation);
        }

        /// <summary>
        /// Resets all settings to their default values.
        /// </summary>
        private void Clear()
        {
            Aircraft = Database.Instance.CheckValue<DBPseudoEntryPlayerAircraft>("Su-25T", "Su-25T");
            Carrier = "";
            Count = 2;
            Tasking = MissionTemplateFlightGroupTask.Objectives;
            Country = Country.CJTFBlue;
            StartLocation = PlayerStartLocation.Runway;
        }

        /// <summary>
        /// Saves the flight group to an .ini file.
        /// </summary>
        /// <param name="ini"></param>
        /// <param name="section">The ini section to save to</param>
        /// <param name="key">The ini key to save to</param>
        public void SaveToFile(INIFile ini, string section, string key)
        {
            ini.SetValue(section, $"{key}.AircraftType", Aircraft);
            ini.SetValue(section, $"{key}.Carrier", Carrier);
            ini.SetValue(section, $"{key}.Count", Count);
            ini.SetValue(section, $"{key}.Task", Tasking);
            ini.SetValue(section, $"{key}.Country", Country);
            ini.SetValue(section, $"{key}.StartLocation", StartLocation);
        }

        /// <summary>
        /// ToString() override.
        /// </summary>
        /// <returns>A string representing this flight group to display in the PropertyGrid.</returns>
        public override string ToString()
        {
            string str = $"{Toolbox.ValToString(Count)}x {Aircraft}, from {Country}, tasked with {Toolbox.LowerCaseFirstLetter(GUITools.GetDisplayName(Tasking))}, starting {StartLocation}, take off from ";
            if (string.IsNullOrEmpty(Carrier)) str += "land airbase";
            else str += GUITools.GetDisplayName(typeof(DBPseudoEntryCarrier), Carrier);

            return str;
        }

        public override void CreateContextMenu(ContextMenuStrip contextMenu, ToolStripItemClickedEventHandler onClickEventHandler)
        {
            ToolStripMenuItem parentMenu;

            // Flyable aircraft
            parentMenu = (ToolStripMenuItem)contextMenu.Items.Add("Aircraft");
            GUITools.PopulateToolStripMenuWithDBEntries(parentMenu.DropDownItems, typeof(DBPseudoEntryPlayerAircraft), onClickEventHandler);

            // Aircraft count
            parentMenu = (ToolStripMenuItem)contextMenu.Items.Add("Aircraft count");
            GUITools.PopulateToolStripMenuWithIntegers(parentMenu.DropDownItems, 1, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE);

            // Flight group tasking
            parentMenu = (ToolStripMenuItem)contextMenu.Items.Add("Tasking");
            foreach (object enumValue in Enum.GetValues(typeof(MissionTemplateFlightGroupTask)))
            {
                GUITools.GetDisplayStrings(typeof(MissionTemplateFlightGroupTask), enumValue, out string enumDisplayName, out string enumDescription);

                ToolStripMenuItem item = new ToolStripMenuItem
                {
                    Text = enumDisplayName,
                    Tag = enumValue,
                    ToolTipText = enumDescription
                };

                parentMenu.DropDownItems.Add(item);
            }

            // Aircraft carriers
            parentMenu = (ToolStripMenuItem)contextMenu.Items.Add("Home carrier");
            parentMenu.DropDownItems.Add("(None, take off from land airbase)").Tag = "$";
            GUITools.PopulateToolStripMenuWithDBEntries(parentMenu.DropDownItems, typeof(DBPseudoEntryCarrier), onClickEventHandler, false, "$");

            // Country //TODO Grouping
            parentMenu = (ToolStripMenuItem)contextMenu.Items.Add("Country");
            foreach (object enumValue in Enum.GetValues(typeof(Country)))
            {
                GUITools.GetDisplayStrings(typeof(Country), enumValue, out string enumDisplayName, out string enumDescription);

                ToolStripMenuItem item = new ToolStripMenuItem
                {
                    Text = enumDisplayName,
                    Tag = enumValue,
                    ToolTipText = enumDescription
                };

                parentMenu.DropDownItems.Add(item);
            }

            // Country //TODO Grouping
            parentMenu = (ToolStripMenuItem)contextMenu.Items.Add("StartLocation");
            foreach (object enumValue in Enum.GetValues(typeof(PlayerStartLocation)))
            {
                GUITools.GetDisplayStrings(typeof(PlayerStartLocation), enumValue, out string enumDisplayName, out string enumDescription);

                ToolStripMenuItem item = new ToolStripMenuItem
                {
                    Text = enumDisplayName,
                    Tag = enumValue,
                    ToolTipText = enumDescription
                };

                parentMenu.DropDownItems.Add(item);
            }
        }

        public override void OnContextMenuItemClicked(object itemTag)
        {
            if (itemTag == null) return;
            
            if (itemTag is MissionTemplateFlightGroupTask groupTask)
                Tasking = groupTask;
            else if (itemTag is Country country)
                Country = country;
            else if (itemTag is PlayerStartLocation location)
                StartLocation = location;
            else if (itemTag is int)
                Count = (int)itemTag;
            else if (itemTag is string)
            {
                if (itemTag.ToString().StartsWith("$"))
                    Carrier = itemTag.ToString().Substring(1);
                else
                    Aircraft = itemTag.ToString();
            }
        }

        /// <summary>
        /// IDispose implementation.
        /// </summary>
        public void Dispose() { }
    }
}
