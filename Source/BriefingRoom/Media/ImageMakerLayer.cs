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

namespace BriefingRoom.Media
{
    /// <summary>
    /// Describes a layer of image to draw with <see cref="ImageMaker"/>
    /// </summary>
    public struct ImageMakerLayer
    {
        /// <summary>
        /// Filename of the image to draw on this layer (from <see cref="BRPaths.INCLUDE_JPG"/>
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Alignment of the image on the layer.
        /// </summary>
        public ContentAlignment Alignment { get; }

        /// <summary>
        /// Offset of the image.
        /// </summary>
        public Point Offset { get; }

        /// <summary>
        /// Rotation of the image, in degrees.
        /// </summary>
        public int Rotation { get; }

        /// <summary>
        /// Scale of the image
        /// </summary>
        public double Scale { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="alignment">Alignment of the image on the layer</param>
        /// <param name="offsetX">X-offset of the image</param>
        /// <param name="offsetY">Y-offset of the image</param>
        /// <param name="rotation">Rotation of the image, in degrees</param>
        /// <param name="scale">Scale of the image</param>
        public ImageMakerLayer(
            string fileName,
            ContentAlignment alignment = ContentAlignment.MiddleCenter,
            int offsetX = 0, int offsetY = 0, int rotation = 0, double scale = 1.0)
        {
            FileName = fileName;
            Alignment = alignment;
            Offset = new Point(offsetX, offsetY);
            Rotation = rotation;
            Scale = scale;
        }
    }
}
