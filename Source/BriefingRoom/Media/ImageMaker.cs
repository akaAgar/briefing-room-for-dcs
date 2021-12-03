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

namespace BriefingRoom4DCS.Media
{
    internal class ImageMaker : IDisposable
    {
        internal int ImageSizeX { get; set; } = 512;
        internal int ImageSizeY { get; set; } = 512;

        internal Color BackgroundColor { get; set; } = Color.Black;

        internal ImageMakerTextOverlay TextOverlay { get; }

        internal ImageMaker()
        {
            TextOverlay = new ImageMakerTextOverlay(this);
        }

        internal byte[] GetImageBytes(params string[] imageFiles)
        {
            return GetImageBytes((from string f in imageFiles select new ImageMakerLayer(f)).ToArray());
        }

        internal byte[] GetImageBytes(params ImageMakerLayer[] imageLayers)
        {
            Bitmap bitmap = new Bitmap(ImageSizeX, ImageSizeY);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(BackgroundColor);

                // Draw all layers of the image
                foreach (ImageMakerLayer layer in imageLayers)
                {
                    string filePath = $"{BRPaths.INCLUDE_JPG}{layer.FileName}";
                    if (!File.Exists(filePath)) continue; // File doesn't exist, ignore it.

                    using (Image layerImage = Image.FromFile(filePath))
                    {
                        Point position = GetImagePosition(layer, layerImage.Size);

                        RotateGraphics(graphics, layer.Rotation);
                        graphics.DrawImage(layerImage,
                                new Rectangle(position, new Size((int)(layerImage.Size.Width * layer.Scale), (int)(layerImage.Size.Height * layer.Scale))),
                                new Rectangle(Point.Empty, layerImage.Size),
                                GraphicsUnit.Pixel);
                        RotateGraphics(graphics, -layer.Rotation);
                    }
                }

                // Draw the text overlay
                TextOverlay.Draw(graphics);
            }

            // Coverts the image to a JPG and get all bytes
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Jpeg);
                bytes = ms.ToArray();
            }

            return bytes;
        }

        private void RotateGraphics(Graphics graphics, int rotationInDegrees)
        {
            if (rotationInDegrees == 0) return;

            graphics.TranslateTransform(ImageSizeX / 2, ImageSizeY / 2);
            graphics.RotateTransform(rotationInDegrees);
            graphics.TranslateTransform(-ImageSizeX / 2, -ImageSizeY / 2);
        }

        private Point GetImagePosition(ImageMakerLayer layer, Size size)
        {
            int x, y;

            if (layer.Alignment.ToString().EndsWith("Left")) x = 0;
            else if (layer.Alignment.ToString().EndsWith("Right")) x = ImageSizeX - (int)(size.Width * layer.Scale);
            else x = (ImageSizeX - size.Width) / 2;

            if (layer.Alignment.ToString().StartsWith("Top")) y = 0;
            else if (layer.Alignment.ToString().StartsWith("Bottom")) y = ImageSizeY - (int)(size.Width * layer.Scale);
            else y = (ImageSizeY - size.Height) / 2;

            return new Point(x, y).Add(layer.Offset);
        }

        public void Dispose() { }
    }
}
