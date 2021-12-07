// A C# program to check if a given Coordinates
// lies inside a given polygon
// Refer https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
// for explanation of functions onSegment(),
// orientation() and doIntersect()
// This code is contributed by 29AjayKumar

using System;
using System.Collections.Generic;
namespace BriefingRoom4DCS
{


    internal class ShapeManager
    {

        // Define Infinite (Using INT_MAX
        // caused overflow problems)
        private static double INF = 10000000;

        // Given three collinear points p, q, r,
        // the function checks if point q lies
        // on line segment 'pr'
        private static bool onSegment(Coordinates p, Coordinates q, Coordinates r)
        {
            if (q.X <= Math.Max(p.X, r.X) &&
                q.X >= Math.Min(p.X, r.X) &&
                q.Y <= Math.Max(p.Y, r.Y) &&
                q.Y >= Math.Min(p.Y, r.Y))
            {
                return true;
            }
            return false;
        }

        // To find orientation of ordered triplet (p, q, r).
        // The function returns following values
        // 0 --> p, q and r are collinear
        // 1 --> Clockwise
        // 2 --> Counterclockwise
        private static double orientation(Coordinates p, Coordinates q, Coordinates r)
        {
            double val = (q.Y - p.Y) * (r.X - q.X) -
                    (q.X - p.X) * (r.Y - q.Y);

            if (val == 0)
            {
                return 0; // collinear
            }
            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        // The function that returns true if
        // line segment 'p1q1' and 'p2q2' intersect.
        private static bool doIntersect(Coordinates p1, Coordinates q1,
                                Coordinates p2, Coordinates q2)
        {
            // Find the four orientations needed for
            // general and special cases
            double o1 = orientation(p1, q1, p2);
            double o2 = orientation(p1, q1, q2);
            double o3 = orientation(p2, q2, p1);
            double o4 = orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
            {
                return true;
            }

            // Special Cases
            // p1, q1 and p2 are collinear and
            // p2 lies on segment p1q1
            if (o1 == 0 && onSegment(p1, p2, q1))
            {
                return true;
            }

            // p1, q1 and p2 are collinear and
            // q2 lies on segment p1q1
            if (o2 == 0 && onSegment(p1, q2, q1))
            {
                return true;
            }

            // p2, q2 and p1 are collinear and
            // p1 lies on segment p2q2
            if (o3 == 0 && onSegment(p2, p1, q2))
            {
                return true;
            }

            // p2, q2 and q1 are collinear and
            // q1 lies on segment p2q2
            if (o4 == 0 && onSegment(p2, q1, q2))
            {
                return true;
            }

            // Doesn't fall in any of the above cases
            return false;
        }

        // Returns true if the Coordinates p lies
        // inside the polygon[] with n vertices
        private static bool isInside(List<Coordinates> polygon, Coordinates p)
        {
            // There must be at least 3 vertices in polygon[]
            if (polygon.Count < 3)
            {
                return false;
            }

            // Create a Coordinates for line segment from p to infinite
            Coordinates extreme = new Coordinates(INF, p.Y);

            // Count intersections of the above line
            // with sides of polygon
            int count = 0, i = 0;
            do
            {
                int next = (i + 1) % polygon.Count;

                // Check if the line segment from 'p' to
                // 'extreme' intersects with the line
                // segment from 'polygon[i]' to 'polygon[next]'
                if (doIntersect(polygon[i],
                                polygon[next], p, extreme))
                {
                    // If the Coordinates 'p' is collinear with line
                    // segment 'i-next', then check if it lies
                    // on segment. If it lies, return true, otherwise false
                    if (orientation(polygon[i], p, polygon[next]) == 0)
                    {
                        return onSegment(polygon[i], p,
                                        polygon[next]);
                    }
                    count++;
                }
                i = next;
            } while (i != 0);

            // Return true if count is odd, false otherwise
            return (count % 2 == 1); // Same as (count%2 == 1)
        }

        internal static bool IsPosValid(Coordinates coords, List<Coordinates> InclusionShape, List<List<Coordinates>> exclusionShapes = null)
        {
            var outcome = isInside(InclusionShape, coords);
            if (exclusionShapes is not null)
            {
                foreach (var shape in exclusionShapes)
                {
                    if (isInside(shape, coords))
                        return false;
                }
            }
            return outcome;
        }
    }
}


