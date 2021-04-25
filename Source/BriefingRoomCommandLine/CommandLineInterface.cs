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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Reflection;

namespace BriefingRoom4DCS.CommandLineTool
{
    /// <summary>
    /// Tool to generate mission(s) from command-line parameters.
    /// </summary>
    public class CommandLineInterface : IDisposable
    {
		private MissionTemplate Template;

		public CommandLineInterface()
		{
			Template = new MissionTemplate();

			ShowMenu();
		}

		private void ShowMenu(string menuCategory = "")
        {
			Console.Clear();
			Console.WriteLine("MISSION TEMPLATE");
			Console.WriteLine();

			int index = 1;
			List<string> displayedCategories = new List<string>();
			foreach (PropertyInfo pi in typeof(MissionTemplate).GetProperties())
            {
				CategoryAttribute propertyCategory = pi.GetCustomAttribute<CategoryAttribute>();
				DisplayAttribute propertyDisplay = pi.GetCustomAttribute<DisplayAttribute>();

				if (string.IsNullOrEmpty(menuCategory))
                {

                }
				else
                {
					if (propertyCategory == null) continue;
					if (propertyCategory.Category != menuCategory) continue;

					Console.WriteLine($"{index}- {propertyDisplay.Name}: {pi.GetValue(Template)}");
				}

				if (propertyCategory != null)
                {
					if (displayedCategories.Contains(propertyCategory.Category)) continue;
					displayedCategories.Add(propertyCategory.Category);
					Console.WriteLine($"{index}- [{propertyCategory.Category}]");
					index++;
				}
				else
                {
					Console.WriteLine($"{index}- {propertyDisplay.Name}: {pi.GetValue(Template)}");
					index++;
				}
			}

			Console.WriteLine();
			Console.ReadLine();
        }
		
		public void Dispose()
		{
		}
    }
}
