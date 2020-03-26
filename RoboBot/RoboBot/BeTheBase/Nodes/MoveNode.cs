using Robocode.Util;
using System;
using System.Collections.Generic;
using System.Text;
using Robocode;

namespace BCDK
{
    public class MoveNode : BaseNode
    {
        private double distance;

        public MoveNode(BlackBoard bb, double distance)
        {
            this.bb = bb;
            this.distance = distance;
        }

        public override BehaviourTreeStatus Tick()
        {
            AdvancedRobot tmpBot = this.bb.Robot;
            ScannedRobotEvent e = this.bb.CurrentEnemy.E;


            if (e == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            // Stay at right angles to the opponent
            tmpBot.SetTurnRight(e.Bearing + 90 -
               30 * this.bb.MovementDirection);

            // If the bot has small energy drop,
            // assume it fired
            double changeInEnergy = this.bb.PreviousEnergy - e.Energy;
            if (changeInEnergy >= -3 && changeInEnergy <= 3)
            {
                // Dodge!
                this.bb.MovementDirection =
                 -this.bb.MovementDirection;
                tmpBot.SetAhead((e.Distance / 4 + 25) * this.bb.MovementDirection);
            }
            // When a bot is spotted,
            // sweep the gun and radar
            this.bb.GunDirection = -this.bb.GunDirection;
            //tmpBot.TurnGunRight(360 * this.bb.GunDirection);

            // Track the energy level
            this.bb.PreviousEnergy = e.Energy;

            double xforce = 0;
            double yforce = 0;
            double force;
            double ang;
            /**The following four lines add wall avoidance.  They will only 
             affect us if the bot is close to the walls due to the
             force from the walls decreasing at a power 3.**/
            xforce += 5000 / Math.Pow(getRange(tmpBot.X,
              tmpBot.Y, tmpBot.BattleFieldWidth, tmpBot.Y), 3);
            xforce -= 5000 / Math.Pow(getRange(tmpBot.X,
              tmpBot.Y, 0, tmpBot.X), 3);
            yforce += 5000 / Math.Pow(getRange(tmpBot.X,
              tmpBot.Y, tmpBot.X, tmpBot.BattleFieldHeight), 3);
            yforce -= 5000 / Math.Pow(getRange(tmpBot.X,
              tmpBot.Y, tmpBot.X, 0), 3);

            double angle = Utils.ToDegrees(absbearing(tmpBot.X, tmpBot.Y, tmpBot.X - xforce, tmpBot.Y - yforce));
            double r = turnTo(angle);
            //distance = RandomNumber(80, 300);
            tmpBot.SetAhead(distance * r);

            //this.bb.Robot.Ahead(distance * angle);

            return BehaviourTreeStatus.Success;
        }

        // Generate a random number between two numbers
        public int RandomNumber(double min, double max)
        {
            Random random = new Random();
            return random.Next((int)min, (int)max);
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
