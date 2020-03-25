using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Robocode;

namespace BCDK
{
    public class TargetPosition
    {


        /**
 * Initializes object.
 * @param time
 * @param coords
 * @param heading
 * @param velocity
 */
        private void init(long time, Point coords, double heading, double velocity)
        {
            this.time = time;
            this.coords = coords;
            this.heading = heading;
            this.velocity = velocity;
        }

        /**
         * Creates a TargetPosition object from all its' members' values.
         * @param time
         * @param coords
         * @param heading
         * @param velocity
         */
        public TargetPosition(long time, Point coords, double heading,
                double velocity)
        {
            init(time, coords, heading, velocity);
        }

        /**
         * Creates a target position using your robot position and
         * ScannedRobotEvent relative to it.
         * @param robot Your robot instance, used only for position retrieving.
         * @param event ScannedRobotEvent for enemy target.
         */
        public TargetPosition(Robot robot, ScannedRobotEvent e)
        {
            Point robotPos = new Point((int)robot.X, (int)robot.Y);
            Point enemyPos = Geometry.movePointByVector(robotPos, e.Distance,
                e.BearingRadians +
                Geometry.degreesToRadians(robot.Heading));

            init(e.Time, enemyPos, e.HeadingRadians,
                    e.Velocity);
        }

        public long time;
        public Point coords;
        public double heading;
        public double velocity;
    }
}
