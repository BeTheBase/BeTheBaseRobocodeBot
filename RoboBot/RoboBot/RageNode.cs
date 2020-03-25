using System;
using System.Collections.Generic;
using System.Text;
using Robocode;
using Robocode.Util;

namespace BCDK
{
    public class RageNode : BaseNode
    {
        private double power;
        private int rageTime;

        public RageNode(BlackBoard bb, double power)
        {
            this.bb = bb;
            this.power = power;
        }

        public override BehaviourTreeStatus Tick()
        {
            double absoluteBearing = this.bb.Robot.Heading + this.bb.ScannendEvent.Bearing;
            AdvancedRobot myRobot = this.bb.Robot;
            //if(myRobot.GunHeading >= 90 && myRobot.GunHeading < 180)

            this.bb.Robot.TurnRightRadians(myRobot.GunHeading);
            //this.bb.Robot.TurnGunRight(Utils.ToDegrees(Utils.NormalRelativeAngle(Utils.ToRadians(absoluteBearing - this.bb.Robot.GunHeading))));
            //double absDeg = AbsoluteBearing(this.bb.Robot.X, this.bb.Robot.Y, this.bb.currentEnemy.x, this.bb.currentEnemy.y);

            //this.bb.Robot.TurnGunRight(normalizeBearing(absDeg - this.bb.Robot.GunHeading));

            // if the gun is cool and we're pointed in the right direction, shoot!
            if ((this.power >= 3) || this.bb.DirectHit)
            {
                this.power = 3;
                this.bb.Robot.Fire(power);
                return BehaviourTreeStatus.Running;
            }
            if (this.bb.Robot.GunHeat == 0 && Math.Abs(this.bb.Robot.GunTurnRemaining) < 10)
            {
                this.bb.Robot.Fire(power);
            }
            return BehaviourTreeStatus.Success;
        }

        private void SetFirePower()
        {
            this.power = (400 / this.bb.ScannendEvent.Distance);
            // selects a bullet power based on our distance away from the target
            if ((this.power > 3))
            {
                this.power = 3;
            }
        }

        // if a heading is not within the 0 to 2pi range, alters it to provide the shortest angle
        double normaliseHeading(double ang)
        {
            if ((ang > (2 * Math.PI)))
            {
                ang = (ang - (2 * Math.PI));
            }

            if ((ang < 0))
            {
                ang = (ang + (2 * Math.PI));
            }

            return ang;
        }

        // returns the distance between two x,y coordinates
        public double getRange(double x1, double y1, double x2, double y2)
        {
            double xo = (x2 - x1);
            double yo = (y2 - y1);
            double h = Math.Sqrt(((xo * xo)
                            + (yo * yo)));
            return h;
        }

        // normalizes a bearing to between +180 and -180
        double normalizeBearing(double angle)
        {
            while (angle > 180) angle -= 360;
            while (angle < -180) angle += 360;
            return angle;
        }
        private static double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        // computes the absolute bearing between two points
        double AbsoluteBearing(double x1, double y1, double x2, double y2)
        {
            double xo = x2 - x1;
            double yo = y2 - y1;


            double hyp = GetDistance(x1, y1, x2, y2);
            double arcSin = Utils.ToDegrees(Math.Asin(xo / hyp));
            double bearing = 0;

            if (xo > 0 && yo > 0)
            { // both pos: lower-Left
                bearing = arcSin;
            }
            else if (xo < 0 && yo > 0)
            { // x neg, y pos: lower-right
                bearing = 360 + arcSin; // arcsin is negative here, actually 360 - ang
            }
            else if (xo > 0 && yo < 0)
            { // x pos, y neg: upper-left
                bearing = 180 - arcSin;
            }
            else if (xo < 0 && yo < 0)
            { // both neg: upper-right
                bearing = 180 - arcSin; // arcsin is negative here, actually 180 + ang
            }

            return bearing;
        }

        // gets the absolute bearing between to x,y coordinates
        public double AbsBearing(double x1, double y1, double x2, double y2)
        {
            double xo = (x2 - x1);
            double yo = (y2 - y1);
            double h = getRange(x1, y1, x2, y2);
            if (((xo > 0)
                        && (yo > 0)))
            {
                return Math.Asin((xo / h));
            }

            if (((xo > 0)
                        && (yo < 0)))
            {
                return (Math.PI - Math.Asin((xo / h)));
            }

            if (((xo < 0)
                        && (yo < 0)))
            {
                return (Math.PI + Math.Asin(((xo / h)
                                * -1)));
            }

            if (((xo < 0)
                        && (yo > 0)))
            {
                return ((2 * Math.PI)
                            - Math.Asin(((xo / h)
                                * -1)));
            }

            return 0;
        }   
    }
}
