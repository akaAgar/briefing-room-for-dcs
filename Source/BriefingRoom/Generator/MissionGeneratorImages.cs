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
using BriefingRoom4DCS.Media;
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace BriefingRoom4DCS.Generator
{
    internal class MissionGeneratorImages : IDisposable
    {
        private readonly ImageMaker ImageMaker;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal MissionGeneratorImages()
        {
            ImageMaker = new ImageMaker();
        }

        internal void GenerateTitle(DCSMission mission, MissionTemplate template)
        {
            ImageMaker.TextOverlay.Alignment = ContentAlignment.MiddleCenter;
            ImageMaker.TextOverlay.Text = mission.Name;

            byte[] imageBytes = ImageMaker.GetImageBytes($"{template.ContextTheater}{Toolbox.RandomInt(1, 6):00}");

            mission.AddMediaFile($"title_{mission.UniqueID}.jpg", imageBytes);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {
            ImageMaker.Dispose();
        }
    }
}