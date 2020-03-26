using System;
using System.Collections.Generic;
using System.Text;
using Robocode;
using Robocode.Util;

namespace BCDK
{
    public class MoveToWardsNode : BaseNode
    {
        private double distance;

        public MoveToWardsNode(BlackBoard bb, double distance)
        {
            this.bb = bb;
            this.distance = distance;
        }

        public override BehaviourTreeStatus Tick()
        {
            AdvancedRobot tmpBot = this.bb.Robot;
            ScannedRobotEvent e = this.bb.CurrentEnemy.E;

            if(tmpBot.Time % 50==0)
            {
                //this.bb.MovementDirection *= -1;
            }
            if(this.bb.DirectHit)
            {
                tmpBot.SetBack(distance);
                this.bb.DirectHit = false;
            }

            if (e == null)
            {
                this.bb.Robot.SetTurnRadarRight(360);
                return BehaviourTreeStatus.Success;
            }

            var gunTurnAmt = Utils.NormalRelativeAngleDegrees(e.Bearing + (this.bb.Robot.Heading - this.bb.Robot.RadarHeading));

            // If our target is too far away, turn and move toward it.
            if (e.Distance > 150)
            {
                gunTurnAmt = Utils.NormalRelativeAngleDegrees(e.Bearing + (this.bb.Robot.Heading - this.bb.Robot.RadarHeading));
                //tmpBot.SetTurnGunRight(gunTurnAmt); // Try changing these to setTurnGunRight,
                tmpBot.SetTurnLeft(normaliseBearing(e.Bearing + 90 - (15 * this.bb.MovementDirection))); // and see how much Tracker improves...
                                           // (you'll have to make Tracker an AdvancedRobot)
                tmpBot.SetAhead((e.Distance - 140) * this.bb.MovementDirection);
                return BehaviourTreeStatus.Success;
            }

            // Our target is close.
            gunTurnAmt = Utils.NormalRelativeAngleDegrees(e.Bearing + (this.bb.Robot.Heading - this.bb.Robot.RadarHeading));
            //tmpBot.SetTurnGunRight(gunTurnAmt);

            // Our target is too close! Back up.
            if (e.Distance < 100)
            {
                tmpBot.SetTurnLeft(normaliseBearing(e.Bearing + 90 - (15 * this.bb.MovementDirection))); // and see how much Tracker improves...
                this.bb.MovementDirection = 1;
                if (e.Bearing > -90 && e.Bearing <= 90)
                {
                    this.bb.Robot.SetBack(distance * this.bb.MovementDirection);
                }
                else
                {
                    this.bb.Robot.SetAhead(distance * this.bb.MovementDirection);
                }
            }
            //tmpBot.Ahead(distance * r);


            return BehaviourTreeStatus.Success;
        }


        // if a bearing is not within the -pi to pi range, alters it to provide the shortest angle
        double normaliseBearing(double ang)
        {
            if ((ang > Math.PI))
            {
                ang = (ang - (2 * Math.PI));
            }

            if ((ang
                        < (Math.PI * -1)))
            {
                ang = (ang + (2 * Math.PI));
            }

            return ang;
        }


        /**Turns the shortest angle possible to come to a heading, then returns 
the direction the bot needs to move in.**/
        int turnTo(double angle)
        {
            double ang;
            int dir;
            ang = normaliseBearing(this.bb.Robot.Heading - angle);
            if (ang > 90)
            {
                ang -= 180;
                dir = -1;
            }
            else if (ang < -90)
            {
                ang += 180;
                dir = -1;
            }
            else
            {
                dir = 1;
            }
            this.bb.Robot.TurnLeft(ang);
            return dir;
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

        // gets the absolute bearing between to x,y coordinates
        public double absbearing(double x1, double y1, double x2, double y2)
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
