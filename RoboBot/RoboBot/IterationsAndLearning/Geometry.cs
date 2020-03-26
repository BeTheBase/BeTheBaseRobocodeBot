using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BCDK
{
    public static class Geometry
    {
        /**
    * Calculates distance between two 2D points.
    * @param p1 First point.
    * @param p2 Second point.
    * @return Distance between points.
    */
        public static double distanceBetween(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        /**
         * Normalizes angle (i.e. converts it to range [-PI; PI)).
         * @param angle Angle in radians to be normalized.
         * @return Normalized angle in range [-PI; PI).
         */
        public static double normalizeAngle(double angle)
        {
            return Robocode.Util.Utils.NormalRelativeAngle(angle);
        }

        /**
         * Moves point by specified vector.
         * @param p Point object to be moved.
         * @param length Length of vector.
         * @param angle Angle of vector bearing in radians.
         * @return Copy of input Point object with changed coordinates.
         */
        public static Point movePointByVector(Point p, double length, double angle)
        {
            double x2 = p.X + length * Math.Sin(angle);
            double y2 = p.Y + length * Math.Cos(angle);

            return new Point((int)x2, (int)y2);
        }

        /**
         * Converts radians to degrees.
         * @param radians Angle in radians to be converted.
         * @return Angle in degrees.
         */
        public static double radiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        /**
         * Converts degrees to radians.
         * @param degrees Angle in degrees to be converted.
         * @return Angle in radians.
         */
        public static double degreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        /**
         * Returns absolute bearing from point p1 to point p2.
         * @param p1
         * @param p2
         * @return Absolute bearing in radians.
         */
        public static double getBearing(Point p1, Point p2)
        {
            return Math.Atan2(p2.X - p1.X, p2.Y - p1.Y);
        }
    }
}
