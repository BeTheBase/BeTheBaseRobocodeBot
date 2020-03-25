using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Robocode;
using Robocode.Util;

namespace BCDK
{
    public class CircleAroundNode : BaseNode
    {
        public CircleAroundNode(BlackBoard bb)
        {
            this.bb = bb;
        }

        // Generate a random number between two numbers
        public int RandomNumber(double min, double max)
        {
            Random random = new Random();
            return random.Next((int)min, (int)max);
        }

        public override BehaviourTreeStatus Tick()
        {
            if (this.bb.CurrentEnemy == null || this.bb.Robot == null)
            {
                return BehaviourTreeStatus.Failure;
            }
            ScannedRobotEvent e = this.bb.CurrentEnemy.E;
            AdvancedRobot r = this.bb.Robot;
            if (r.Time % RandomNumber(70,110) == 0)
            {
                this.bb.MovementDirection *= -1;
            }




            // Calculate exact location of the robot
            double absoluteBearing = r.Heading + e.Bearing;
            double bearingFromGun = Utils.NormalRelativeAngleDegrees(absoluteBearing - r.GunHeading);
            double bearingFromRadar = Utils.NormalRelativeAngleDegrees(absoluteBearing - r.RadarHeading);

            //Spiral around our enemy. 90 degrees would be circling it (parallel at all times)

            double goalDirection = absoluteBearing - Math.PI / 2 * this.bb.MovementDirection;
            RectangleF fieldRect = new RectangleF(18, 18, (float)r.BattleFieldWidth - 36,
                (float)r.BattleFieldHeight - 36);
            while (!fieldRect.Contains((float)r.X + (float)Math.Sin(goalDirection) * 120, (float)r.Y +
                    (float)Math.Cos(goalDirection) * 120))
            {
                goalDirection += this.bb.MovementDirection * .1;    //turn a little toward enemy and try again
            }
            double turn =
                Utils.NormalRelativeAngle(goalDirection - r.HeadingRadians);
            if (Math.Abs(turn) > Math.PI / 2)
            {
                turn = Utils.NormalRelativeAngle(turn + Math.PI);
                r.SetBack(100);
                //this.bb.MovementDirection = -1;
            }
            else
            {
                //this.bb.MovementDirection = 1;
                r.SetAhead(e.Distance-140);
            }
            //r.SetTurnRightRadians(turn);
            // 80 and 100 make that we move a bit closer every turn.

            //r.SetAhead((e.Distance - 140) * this.bb.MovementDirection);

            if (this.bb.MovementDirection > 0)
                r.SetTurnRight(Utils.NormalRelativeAngleDegrees(e.Bearing + 80));
            else
                r.SetTurnRight(Utils.NormalRelativeAngleDegrees(e.Bearing + 100));



            // If it's close enough, fire!
            if (Math.Abs(bearingFromGun) <= 4)
            {
                //r.SetTurnGunRight(bearingFromGun);
                //r.SetTurnRadarRight(bearingFromRadar); // keep the radar focussed on the enemy
                                                   // We check gun heat here, because calling fire()
                                                     // uses a turn, which could cause us to lose track
                                                     // of the other robot.

                // The close the enmy robot, the bigger the bullet. 
                // The more precisely aimed, the bigger the bullet.
                 //Don't fire us into disability, always save .1
                if (r.GunHeat == 0 && r.Energy > .2)
                {
                    //r.Fire(Math.Min(4.5 - Math.Abs(bearingFromGun) / 2 - e.Distance / 250, r.Energy - .1));
                }
            } // otherwise just set the gun to turn.
              // 
            else
            {
                r.SetTurnGunRight(bearingFromGun);
                r.SetTurnRadarRight(bearingFromRadar);
            }
            // Generates another scan event if we see a robot.
            // We only need to call this if the radar
            // is not turning.  Otherwise, scan is called automatically.
            if (bearingFromGun == 0)
            {
                r.Scan();
            }

            return BehaviourTreeStatus.Success;
        }

        double WallCloseCheck()
        {
            double minDistanceFromWall = 60;
            double width = this.bb.Robot.BattleFieldWidth;
            double height = this.bb.Robot.BattleFieldHeight;
            double x = this.bb.Robot.X;
            double y = this.bb.Robot.Y;
            if(x > width - minDistanceFromWall || x < minDistanceFromWall+ minDistanceFromWall)
            {
                //Our X position gets into the wall
                return this.bb.MovementDirection *=-1;
            }
            else if(y > height - minDistanceFromWall || y < minDistanceFromWall + minDistanceFromWall)
            {
                //Our Y position gets into the wall
                return this.bb.MovementDirection *=-1;
            }
            else
            {
                return this.bb.MovementDirection;
            }
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
    }
}
