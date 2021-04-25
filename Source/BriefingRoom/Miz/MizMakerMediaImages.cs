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

using BriefingRoom4DCS.Data;
//using BriefingRoom4DCS.Generator;
using BriefingRoom4DCS.Media;
using BriefingRoom4DCS.Mission;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace BriefingRoom4DCS.Miz
{
    /// <summary>
    /// Creates all images file required for the mission except for Kneeboard makers, which are handled differently (see MIZMediaKneeboardMaker).
    /// </summary>
    internal class MizMakerMediaImages : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal MizMakerMediaImages()
        {

        }

        /// <summary>
        /// IDispose implementation.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Generates the title image for the mission.
        /// </summary>
        /// <param name="mission">The misison which requires a title image.</param>
        /// <returns>The mission title image, as an array of bytes for a JPEG file.</returns>
        private byte[] GetTitleImage(DCSMission mission)
        {
            byte[] imageBytes;

            using (ImageMaker imgMaker = new ImageMaker())
            {
                imgMaker.TextOverlay.Text = mission.MissionName;
                imgMaker.TextOverlay.Alignment = ContentAlignment.BottomCenter;

                List<ImageMakerLayer> layers = new List<ImageMakerLayer>();
                string[] theaterImages = Directory.GetFiles($"{BRPaths.INCLUDE_JPG}Theaters\\", $"{Database.Instance.GetEntry<DBEntryTheater>(mission.Theater).DCSID}*.jpg");
                if (theaterImages.Length == 0)
                    layers.Add(new ImageMakerLayer("_default.jpg"));
                else
                    layers.Add(new ImageMakerLayer("Theaters\\" + Path.GetFileName(Toolbox.RandomFrom(theaterImages))));

                //layers.Add(new ImageMakerLayer($"Flags\\{GeneratorTools.RemoveAfterComma(mission.Coalitions[(int)mission.CoalitionPlayer])}.png", ContentAlignment.TopLeft, 8, 8, 0, .5));
                layers.Add(new ImageMakerLayer($"Flags\\{mission.Coalitions[(int)mission.CoalitionPlayer]}.png", ContentAlignment.TopLeft, 8, 8, 0, .5));

                imageBytes = imgMaker.GetImageBytes(layers.ToArray());
            }

            return imageBytes;
        }

        internal void AddMediaFiles(MizFile miz, DCSMission mission)
        {
            miz.AddEntry($"l10n/DEFAULT/title_{mission.UniqueID}.jpg", GetTitleImage(mission));
        }

        /// <summary>
        /// Returns an image from the HQ4DCS\Include directory, if it exists.
        /// If it doesn't, return an solid black image of size defaultWidth by defaultHeight.
        /// </summary>
        /// <param name="imageFilePath">Relative ath to the image file from the HQ4DCS\Include directory.</param>
        /// <param name="defaultWidth">The default width, in case image doesn't exist.</param>
        /// <param name="defaultHeight">The default height, in case image doesn't exist.</param>
        /// <returns>A WinForm image.</returns>
        private Image GetImageIfItExists(string imageFilePath, int defaultWidth = 512, int defaultHeight = 512)
        {
            if (File.Exists(BRPaths.INCLUDE + imageFilePath))
                return Image.FromFile(BRPaths.INCLUDE + imageFilePath);

            Bitmap bitmap = new Bitmap(defaultWidth, defaultHeight);
            using (Graphics g = Graphics.FromImage(bitmap)) { g.Clear(Color.Black); }
            return bitmap;
        }
    }
}
