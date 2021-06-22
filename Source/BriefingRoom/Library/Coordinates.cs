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
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCS
{
    /// <summary>
    /// Stores a set of X, Y map coordinates as doubles.
    /// </summary>
    public struct Coordinates : ICloneable
    {
        /// <summary>
        /// Constant "zero" coordinates, with both X and Y set to zero.
        /// </summary>
        public static readonly Coordinates Zero = new Coordinates(0, 0);

        /// <summary>
        /// The X coordinate.
        /// </summary>
        public readonly double X;

        /// <summary>
        /// The Y coordinate.
        /// </summary>
        public readonly double Y;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="both">X and Y coordinate.</param>
        public Coordinates(double both) { X = both; Y = both; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public Coordinates(double x, double y) { X = x; Y = y; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="source">Source coordinates set to duplicate.</param>
        public Coordinates(Coordinates source) { X = source.X; Y = source.Y; }

        /// <summary>
        /// Constructor. Parses the X and Y coordinates from a string (format is "1.2345,6.7890")
        /// </summary>
        /// <param name="coordinatesString">The string containing the coordinates.</param>
        public Coordinates(string coordinatesString)
        {
            try
            {
                string[] xAndYStrings = coordinatesString.Split(',');

                X = Toolbox.StringToDouble(xAndYStrings[0]);
                Y = Toolbox.StringToDouble(xAndYStrings[1]);
            }
            catch (Exception)
            {
                X = 0; Y = 0;
            }
        }

        /// <summary>
        /// + operator.
        /// </summary>
        /// <param name="coo1">A set of coordinates</param>
        /// <param name="coo2">Another set of coordinates</param>
        /// <returns>The sum of both coordinates</returns>
        public static Coordinates operator +(Coordinates coo1, Coordinates coo2) { return new Coordinates(coo1.X + coo2.X, coo1.Y + coo2.Y); }

        /// <summary>
        /// - operator.
        /// </summary>
        /// <param name="coo1">A set of coordinates</param>
        /// <param name="coo2">Another set of coordinates</param>
        /// <returns>The subtraction of both coordinates</returns>
        public static Coordinates operator -(Coordinates coo1, Coordinates coo2){ return new Coordinates(coo1.X - coo2.X, coo1.Y - coo2.Y); }

        /// <summary>
        /// * operator.
        /// </summary>
        /// <param name="coo1">A set of coordinates</param>
        /// <param name="mul">A multiplier</param>
        /// <returns>A set of coordinates with X and Y components multiplied by mul</returns>
        public static Coordinates operator *(Coordinates coo, double mul) { return new Coordinates(coo.X * mul, coo.Y * mul); }

        /// <summary>
        /// * operator.
        /// </summary>
        /// <param name="coo1">A set of coordinates</param>
        /// <param name="coo2">Another set of coordinates</param>
        /// <returns>The multiplcation of both coordinates</returns>
        public static Coordinates operator *(Coordinates coo1, Coordinates coo2) { return new Coordinates(coo1.X * coo2.X, coo1.Y * coo2.Y); }

        /// <summary>
        /// / operator.
        /// </summary>
        /// <param name="coo1">A set of coordinates</param>
        /// <param name="coo2">Another set of coordinates</param>
        /// <returns>The division of both coordinates</returns>
        public static Coordinates operator /(Coordinates coo1, Coordinates coo2) { return new Coordinates(coo1.X / coo2.X, coo1.Y / coo2.Y); }

        /// <summary>
        /// / operator.
        /// </summary>
        /// <param name="coo1">A set of coordinates</param>
        /// <param name="div">A divider</param>
        /// <returns>A set of coordinates with X and Y components divided by div</returns>
        public static Coordinates operator /(Coordinates coo, double div) { return new Coordinates(coo.X / div, coo.Y / div); }

        /// <summary>
        /// Converts the coordinates to a string in the "X,Y" format in the invariant culture.
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString() { return Toolbox.ValToString(X) + "," + Toolbox.ValToString(Y); }

        /// <summary>
        /// Creates a set of normalized coordinates from an angle.
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        /// <returns>A set of coordinates</returns>
        public static Coordinates FromAngleInDegrees(double angle)
        {
            return FromAngleInRadians(angle * Toolbox.DEGREES_TO_RADIANS);
        }

        /// <summary>
        /// Creates a set of normalized coordinates from an angle.
        /// </summary>
        /// <param name="angle">Angle in radians</param>
        /// <returns>A set of coordinates</returns>
        public static Coordinates FromAngleInRadians(double angle)
        {
            return new Coordinates(Math.Cos(angle), Math.Sin(angle));
        }

        /// <summary>
        /// Gets Angle in Radias from center coordinate to 
        /// </summary>
        /// <param name="center">Coordinates of center</param>
        /// <param name="waypoint">Coordinates of waypoint</param>
        /// <returns>Angle in Radians (always positive)</returns>
        public static double ToAngleInRadians(Coordinates center, Coordinates waypoint)
        {
            var delta = waypoint - center;
            var rawRads =  Math.Atan2(delta.Y, delta.X);
            return rawRads > 0? rawRads : Toolbox.TWO_PI + rawRads;
        }

        /// <summary>
        /// Interpolates two sets of coordinates.
        /// </summary>
        /// <param name="coo1">A set of coordinates</param>
        /// <param name="coo2">Another set of coordinates</param>
        /// <param name="value">A value between</param>
        /// <returns>The interpolated set of coordinates</returns>
        public static Coordinates Lerp(Coordinates coo1, Coordinates coo2, double value)
        {
            return new Coordinates(Toolbox.Lerp(coo1.X, coo2.X, value), Toolbox.Lerp(coo1.Y, coo2.Y, value));
        }

        /// <summary>
        /// Interpolates two sets of coordinates.
        /// </summary>
        /// <param name="coo1">A set of coordinates</param>
        /// <param name="coo2">Another set of coordinates</param>
        /// <param name="value">A value between</param>
        /// <returns>The interpolated set of coordinates</returns>
        public static Coordinates FromAngleAndDistance(Coordinates coordinates, double distance/*, double angle*/)
        {
            return new Coordinates(
                coordinates.X + distance * Math.Cos(distance * Toolbox.DEGREES_TO_RADIANS),
                coordinates.Y + distance * Math.Sin(distance * Toolbox.DEGREES_TO_RADIANS));
        }


        /// <summary>
        /// Creates a random coordinates set at a distance between min and max from the zero point.
        /// Mostly used to create random inaccuracy from waypoints, etc.
        /// </summary>
        /// <param name="min">Minimum distance (in meters) from 0,0</param>
        /// <param name="max">Maximum distance (in meters) from 0,0</param>
        /// <returns>A set of coordinates</returns>
        public static Coordinates CreateRandom(double min, double max)
        {
            return CreateRandom(new MinMaxD(min, max));
        }

        /// <summary>
        /// Creates a random coordinates set at a distance between minmax.Min and minMax.Max from the zero point.
        /// Mostly used to create random inaccuracy from waypoints, etc.
        /// </summary>
        /// <param name="minMax">Minimum and maximum distance from 0,0</param>
        /// <returns>A set of coordinates</returns>
        internal static Coordinates CreateRandom(MinMaxD minMax)
        {
            return FromAngleInDegrees(Toolbox.RandomDouble(360.0)) * minMax.GetValue();
        }

        /// <summary>
        /// Returns the center of all coordintes passed as parameters.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <returns>The center, or 0,0 if no coordinates were passed as parameters.</returns>
        public static Coordinates GetCenter(params Coordinates[] coordinates)
        {
            if ((coordinates == null) || (coordinates.Length == 0)) return new Coordinates();

            Coordinates center = new Coordinates();
            for (int i = 0; i < coordinates.Length; i++) center += coordinates[i];

            center /= coordinates.Length;

            return center;
        }

        /// <summary>
        /// Returns the distance between this set of coordinates and another.
        /// </summary>
        /// <param name="other">The other set of coordinates.</param>
        /// <returns>The distance.</returns>
        public double GetDistanceFrom(Coordinates other) { return Math.Sqrt(GetSquaredDistanceFrom(other)); }

        /// <summary>
        /// Returns the square of the distance between this set of coordinates and another (for quick distance comparison withtout square root).
        /// </summary>
        /// <param name="other">The other set of coordinates.</param>
        /// <returns>The square of the distance.</returns>
        public double GetSquaredDistanceFrom(Coordinates other) { return Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2); }

        /// <summary>
        /// ICloneable implementation.
        /// </summary>
        /// <returns>A new Coordinates structure with the same X and Y values.</returns>
        public object Clone() { return new Coordinates(X, Y); }

        /// <summary>
        /// Returns a string describing the coordinates as a Lua table of format "{ x = <see cref="X"/>, y = <see cref="Y"/> }"
        /// </summary>
        /// <returns>A string</returns>
        public string ToLuaTable()
        {
            return $"{{ [\"x\"] = {Toolbox.ValToString(X)}, [\"y\"] = {Toolbox.ValToString(Y)} }}";
        }

        /// <summary>
        /// Creates a random coordinates set at a distance between min and max from the zero point.
        /// Mostly used to create random inaccuracy from waypoints, etc.
        /// </summary>
        /// <param name="min">Minimum distance (in meters) from 0,0</param>
        /// <param name="max">Maximum distance (in meters) from 0,0</param>
        /// <returns>A set of coordinates</returns>
        public Coordinates CreateNearRandom(double min, double max)
        {
            return CreateNearRandom(new MinMaxD(min, max));
        }

        internal double GetLength()
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }

        internal Coordinates Normalize()
        {
            double length = GetLength();
            if (length == 0) return Zero;
            return new Coordinates(X / length, Y / length);
        }

        /// <summary>
        /// Creates a random coordinates set at a distance between minmax.Min and minMax.Max from the zero point.
        /// Mostly used to create random inaccuracy from waypoints, etc.
        /// </summary>
        /// <param name="minMax">Minimum and maximum distance from 0,0</param>
        /// <returns>A set of coordinates</returns>
        internal Coordinates CreateNearRandom(MinMaxD minMax)
        {
            double randX = minMax.GetValue() * Toolbox.RandomFrom(1 , -1); 
            double randY = minMax.GetValue() * Toolbox.RandomFrom(1 , -1);
            
            return new Coordinates(X + randX, Y + randY);
        }

        /// <summary>
        /// Returns the sum of a series of coordinates.
        /// </summary>
        /// <param name="coordinates">Coordinates to sum.</param>
        /// <returns>The sum of all coordinates</returns>
        internal static Coordinates Sum(IEnumerable<Coordinates> coordinates)
        {
            return new Coordinates(
                (from coordinate in coordinates select coordinate.X).Sum(),
                (from coordinate in coordinates select coordinate.Y).Sum());
        }
    }
}
