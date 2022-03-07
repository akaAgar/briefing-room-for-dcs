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
    internal struct ImageMakerLayer
    {
        internal Image ImageData { get; }

        internal ContentAlignment Alignment { get; }

        internal Point Offset { get; }

        internal int Rotation { get; }

        internal double Scale { get; }

        internal ImageMakerLayer(
            Image imageData,
            ContentAlignment alignment = ContentAlignment.MiddleCenter,
            int offsetX = 0, int offsetY = 0, int rotation = 0, double scale = 1.0)
        {
            ImageData = imageData;
            Alignment = alignment;
            Offset = new Point(offsetX, offsetY);
            Rotation = rotation;
            Scale = scale;
        }

        internal ImageMakerLayer(
            string filePath,
            ContentAlignment alignment = ContentAlignment.MiddleCenter,
            int offsetX = 0, int offsetY = 0, int rotation = 0, double scale = 1.0)
        {
            ImageData = Image.FromFile($"{BRPaths.INCLUDE_JPG}{filePath}");
            Alignment = alignment;
            Offset = new Point(offsetX, offsetY);
            Rotation = rotation;
            Scale = scale;
        }
    }
}
