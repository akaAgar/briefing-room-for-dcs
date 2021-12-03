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

using System.Drawing;

namespace BriefingRoom4DCS.Media
{
    internal class ImageMakerTextOverlay
    {
        internal ContentAlignment Alignment { get; set; } = ContentAlignment.MiddleCenter;

        internal Color Color { get; set; } = Color.White;

        internal string FontFamily { get; set; } = "Arial";

        internal float FontSize { get; set; } = 36.0f;

        internal FontStyle FontStyle { get; set; } = FontStyle.Regular;

        internal int Rotation { get; set; } = 0;

        internal string Text { get; set; } = "";

        internal bool Shadow { get; set; } = true;

        private ImageMaker ImageMaker;

        internal ImageMakerTextOverlay(ImageMaker imageMaker)
        {
            ImageMaker = imageMaker;
        }

        internal void Draw(Graphics graphics)
        {
            if (string.IsNullOrEmpty(Text)) return; // No text, nothing to draw

            using (Font font = new Font(FontFamily, FontSize, FontStyle))
            {
                Size textSize = graphics.MeasureString(Text, font).ToSize();
                StringFormat stringFormat = GetStringFormat();

                // Draw text black "shadow" first to make sure it is readable on any background
                if (Shadow)
                {
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                            graphics.DrawString(Text, font, Brushes.Black,
                                new RectangleF(3 * x + 10, 3 * y + 10, ImageMaker.ImageSizeX - 10, ImageMaker.ImageSizeY - 10), stringFormat);
                }

                using (SolidBrush brush = new SolidBrush(Color))
                    graphics.DrawString(Text, font, brush,
                        new RectangleF(10, 10, ImageMaker.ImageSizeX - 10, ImageMaker.ImageSizeY - 10), stringFormat);
            }
        }

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
