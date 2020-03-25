using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Robocode;

namespace BCDK
{
    public class Bullet
    {
        private Point firstCoords;
        private long firstSeenTime;
        private double heading;
        private double power;

        /**
         * Creates new object representing bullet on map.
         * @param coords Absolute coordinates of bullet.
         * @param power Energy of bullet (determines its damage and velocity).
         * @param heading Heading of bullet in radians.
         * @param time Time when bullet was seen.
         */
        public Bullet(Point coords, double heading, double power, long time)
        {
            firstCoords = coords;
            this.heading = heading;
            this.power = power;
            firstSeenTime = time;
        }

        /**
         * Calculates position of bullet at specified time.
         * @param time Absolute time value.
         * @return Point object points to estimated bullet position.
         */
        public Point getPositionAt(long time)
        {
            return Geometry.movePointByVector(firstCoords,
                    Robocode.Rules.GetBulletSpeed(power), heading);
        }
    }
}
