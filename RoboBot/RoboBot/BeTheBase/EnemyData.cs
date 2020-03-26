using System;
using System.Collections.Generic;
using System.Text;
using Robocode;
using Robocode.Util;

namespace BCDK
{
    public class EnemyData : Robot
    {
        public ScannedRobotEvent E;

        public double x;
        public double y;

        private double bearing;
        private double distance;
        private double energy;
        private double heading;
        private string name = "";
        private double velocity;
        private long time;
        private Robot r;

        public EnemyData(ScannedRobotEvent e, double enemyVel, double enemyHeading)
        {
            this.E = e;
            this.velocity = enemyVel;
            this.heading = enemyHeading;
        }

        public void Update(ScannedRobotEvent e, Robot robot)
        {
            E = e;
            bearing = e.Bearing;
            distance = e.Distance;
            energy = e.Energy;
            heading = e.Heading;
            name = e.Name;
            velocity = e.Velocity;
            time = robot.Time;
            r = robot;
            double absBearingDeg = (robot.Heading + e.Bearing);
            if (absBearingDeg < 0) absBearingDeg += 360;

            // yes, you use the _sine_ to get the X value because 0 deg is North
            x = robot.X + Math.Sin(Utils.ToRadians(absBearingDeg)) * e.Distance;

            // yes, you use the _cosine_ to get the Y value because 0 deg is North
            y = robot.Y + Math.Cos(Utils.ToRadians(absBearingDeg)) * e.Distance;
        }

        public Point guessPosition(long when)
        {
            /**time is when our scan data was produced.  when is the time 
            that we think the bullet will reach the target.  diff is the 
            difference between the two **/
            double diff = when - time;
            double newX, newY;
            /**if there is a significant change in heading, use circular 
            path prediction**/
            if (Math.Abs(heading) > 0.00001)
            {
                double radius = velocity / heading;
                double tothead = diff * heading;
                newY = y + (Math.Sin(heading + tothead) * radius) -
                              (Math.Sin(heading) * radius);
                newX = x + (Math.Cos(heading) * radius) -
                              (Math.Cos(heading + tothead) * radius);
            }
            /**if the change in heading is insignificant, use linear 
            path prediction**/
            else
            {
                newY = y + Math.Cos(heading) * velocity * diff;
                newX = x + Math.Sin(heading) * velocity * diff;
            }
            return new Point((int)newX, (int)newY);
        }

        public double GetFutureX(long when, double heading, double velocity)
        {
            return x + Math.Sin(Utils.ToRadians(heading)) * velocity * when;
        }

        public double GetFutureX(long when)
        {
            return x + Math.Sin(Utils.ToRadians(heading)) * velocity * when;
        }

        public double GetFutureY(long when)
        {
            return y + Math.Cos(Utils.ToRadians(heading)) * velocity * when;
        }

        public double GetFutureY(long when, double heading, double velocity)
        {
            return y + Math.Cos(Utils.ToRadians(heading)) * velocity * when;
        }

        public double GetDistance()
        {
            return distance;
        }

        public double GetHeading()
        {
            return heading;
        }

        public double GetVelocity()
        {
            return velocity;
        }
    }
}
