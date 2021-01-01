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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace BriefingRoom4DCSWorld
{
    public class ImageMaker : IDisposable
    {
        private const int IMAGE_SIZE = 512;

        public Color BackgroundColor { get; set; } = Color.Black;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ImageMaker()
        {

        }

        public byte[] GetImageBytes(params string[] imageFiles)
        {
            return GetImageBytes((from string f in imageFiles select new ImageMakerLayer(f)).ToArray());
        }

        public byte[] GetImageBytes(params ImageMakerLayer[] imageLayers)
        {
            Bitmap bitmap = new Bitmap(IMAGE_SIZE, IMAGE_SIZE);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(BackgroundColor);

                foreach (ImageMakerLayer layer in imageLayers)
                {
                    string filePath = $"{BRPaths.INCLUDE_JPG}{layer.FileName}";
                    if (!File.Exists(filePath)) continue;

                    using (Image layerImage = Image.FromFile(filePath))
                    {
                        Point position = GetImagePosition(layer.Alignment, layer.OffsetX, layer.OffsetY, layerImage.Size);
                        
                        graphics.DrawImage(layerImage,
                                new Rectangle(position, layerImage.Size),
                                new Rectangle(Point.Empty, layerImage.Size),
                                GraphicsUnit.Pixel);
                    }
                }
            }

            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Jpeg);
                bytes = ms.ToArray();
            }

            return bytes;
        }

        private Point GetImagePosition(ContentAlignment alignment, int offsetX, int offsetY, Size size)
        {
            int x, y;

            if (alignment.ToString().EndsWith("Left")) x = 0;
            else if (alignment.ToString().EndsWith("Right")) x = IMAGE_SIZE - size.Width;
            else x = (IMAGE_SIZE - size.Width) / 2;

            if (alignment.ToString().StartsWith("Top")) y = 0;
            else if (alignment.ToString().StartsWith("Bottom")) y = IMAGE_SIZE - size.Height;
            else y = (IMAGE_SIZE - size.Height) / 2;

            return new Point(x + offsetX, y + offsetY);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
