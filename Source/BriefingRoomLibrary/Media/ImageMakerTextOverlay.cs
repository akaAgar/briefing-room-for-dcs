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
using System.Drawing;

namespace BriefingRoom.Media
{
    /// <summary>
    /// Describes a layer of text to draw in a <see cref="ImageMaker"/>
    /// </summary>
    public class ImageMakerTextOverlay
    {
        /// <summary>
        /// Alignment of the text overlay.
        /// </summary>
        public ContentAlignment Alignment { get; set; } = ContentAlignment.MiddleCenter;

        /// <summary>
        /// Color to use fot the text overlay.
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Font to use for the text overlay.
        /// </summary>
        public string FontFamily { get; set; } = "Arial";

        /// <summary>
        /// Size (in pixels) of the font to use.
        /// </summary>
        public float FontSize { get; set; } = 36.0f;

        /// <summary>
        /// Syle to use for the overlay text font.
        /// </summary>
        public FontStyle FontStyle { get; set; } = FontStyle.Regular;

        /// <summary>
        /// Rotation of the image, in degrees.
        /// </summary>
        public int Rotation { get; set; } = 0;

        /// <summary>
        /// Text to draw over all generated images.
        /// </summary>
        public string Text { get; set; } = "";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rotationInDegrees">Rotation of the image, in degrees</param>
        public ImageMakerTextOverlay() { }

        /// <summary>
        /// Draw the text overlay on the provided <see cref="Graphics"/>.
        /// </summary>
        /// <param name="graphics"><see cref="Graphics"/> to use to draw the text overlay</param>
        public void Draw(Graphics graphics)
        {
            if (string.IsNullOrEmpty(Text)) return; // No text, nothing to draw
         
            using (Font font = new Font(FontFamily, FontSize, FontStyle))
            {
                Size textSize = graphics.MeasureString(Text, font).ToSize();
                StringFormat stringFormat = GetStringFormat();

                // Draw text black "shadow" first to make sure it is readable on any background
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                        graphics.DrawString(Text, font, Brushes.Black,
                            new RectangleF(3 * x, 3 * y, ImageMaker.IMAGE_SIZE, ImageMaker.IMAGE_SIZE), stringFormat);

                using (SolidBrush brush = new SolidBrush(Color))
                    graphics.DrawString(Text, font, brush,
                        new RectangleF(0, 0, ImageMaker.IMAGE_SIZE, ImageMaker.IMAGE_SIZE), stringFormat);
            }
        }

        /// <summary>
        /// Returns string format information generated from <see cref="this.ContentAlignment"/>.
        /// </summary>
        /// <returns>A string format</returns>
        private StringFormat GetStringFormat()
        {
            StringFormat stringFormat = new StringFormat();

            if (Alignment.ToString().EndsWith("Left")) stringFormat.Alignment = StringAlignment.Near;
            else if (Alignment.ToString().EndsWith("Right")) stringFormat.Alignment = StringAlignment.Far;
            else stringFormat.Alignment = StringAlignment.Center;

            if (Alignment.ToString().StartsWith("Top")) stringFormat.LineAlignment = StringAlignment.Near;
            else if (Alignment.ToString().StartsWith("Bottom")) stringFormat.LineAlignment = StringAlignment.Far;
            else stringFormat.LineAlignment = StringAlignment.Center;

            return stringFormat;
        }
    }
}
