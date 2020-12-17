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

using BriefingRoom4DCSWorld.Mission;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace BriefingRoom4DCSWorld.Miz
{
    /// <summary>
    /// Creates all images file required for the mission except for Kneeboard makers, which are handled differently (see MIZMediaKneeboardMaker).
    /// </summary>
    public class MizMakerMediaImages : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MizMakerMediaImages() { }

        /// <summary>
        /// IDispose implementation.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Generates the title image for the mission.
        /// </summary>
        /// <param name="missionName">The name of the mission.</param>
        /// <returns>The mission title image, as an array of bytes for a JPEG file.</returns>
        private byte[] GetTitleImage(string missionName)
        {
            byte[] imageBytes;
            Rectangle rect;
            int x, y;

            using (Image titleImage = GetImageIfItExists("Jpg\\_default.jpg"))
            {
                using (Graphics g = Graphics.FromImage(titleImage))
                {
                    using (Font font = new Font("Arial", 48, FontStyle.Regular, GraphicsUnit.Point))
                    {
                        for (x = -1; x <= 1; x++)
                            for (y = -1; y <= 1; y++)
                            {
                                if ((x == 0) && (y == 0)) continue;
                                rect = new Rectangle(x * 1, y * 1, 512, 512);
                                TextRenderer.DrawText(g, missionName, font, rect, Color.Black, Toolbox.CENTER_TEXT_FLAGS);
                            }

                        rect = new Rectangle(0, 0, 512, 512);
                        TextRenderer.DrawText(g, missionName, font, rect, Color.White, Toolbox.CENTER_TEXT_FLAGS);
                    }
                }

                imageBytes = Toolbox.ImageToBytes(titleImage, ImageFormat.Jpeg);
            }

            return imageBytes;
        }

        public void AddMediaFiles(MizFile miz, DCSMission mission)
        {
            miz.AddEntry("l10n/DEFAULT/title.jpg", GetTitleImage(mission.MissionName));
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
