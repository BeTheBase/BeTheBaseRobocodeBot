using Robocode.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Drawing;

namespace BCDK
{
    public class FireNode : BaseNode
    {
        private double power;

        public FireNode(BlackBoard bb, double power)
        {
            this.bb = bb;
            this.power = power;
        }

       

        public override BehaviourTreeStatus Tick()
        {
            
            //if (this.bb.FireRange > this.bb.ScannendEvent?.Distance) return BehaviourTreeStatus.Success;
            //SetFirePower();

            /*
            
            EnemyData predictEnemyData = this.bb.PredictedEnemyDataStart;
            double predictedEnemyVelocity = predictEnemyData.EnemyVelocity;
            double predictedEnemyHeading = predictEnemyData.EnemyHeading;
            double gunTurnAmt;
            double absbearing = (this.bb.ScannendEvent.BearingRadians + predictedEnemyHeading);

            gunTurnAmt = Utils.NormalRelativeAngle(absbearing - this.bb.Robot.GunHeading + predictedEnemyVelocity);//amount to turn our gun, lead just a little bit
                                                                                                                   //this.bb.Robot.TurnGunRight(gunTurnAmt);
                                                                                                                   //double absoluteBearing = this.bb.Robot.Heading + this.bb.ScannendEvent.Bearing;
                                                                                                                   //this.bb.Robot.TurnGunRight(Utils.ToDegrees(Utils.NormalRelativeAngle(Utils.ToRadians(absoluteBearing - this.bb.Robot.GunHeading))));
                                                                                                                   //this.bb.Robot.Fire(power);
                                                                                                                   // calculate speed of bullet

            // non-predictive firing can be done like this:
            //double absDeg = AbsoluteBearing(this.bb.Robot.X, this.bb.Robot.Y, this.bb.currentEnemy.X, this.bb.currentEnemy.Y);


            */
            //double absoluteBearing = this.bb.Robot.Heading + this.bb.ScannendEvent.Bearing;
            //this.bb.Robot.TurnGunRight(Utils.ToDegrees(Utils.NormalRelativeAngle(Utils.ToRadians(absoluteBearing - this.bb.Robot.GunHeading))));
            this.power = (400 / this.bb.ScannendEvent.Distance);
            // selects a bullet power based on our distance away from the target

            double bulletSpeed = 20 - power * 3;
            // distance = rate * time, solved for time
            long time = (long)Math.Ceiling(this.bb.ScannendEvent.Distance / Robocode.Rules.GetBulletSpeed(power));
            //long time = (long)(this.bb.currentEnemy.GetDistance() / bulletSpeed);
            // calculate gun turn to predicted x,y location
            if (this.bb.PredictedEnemyDataStart == null)
                this.bb.PredictedEnemyDataStart = this.bb.CurrentEnemy;
            double futureX = this.bb.CurrentEnemy.getFutureX((long)time,this.bb.CurrentEnemy.EnemyHeading, this.bb.CurrentEnemy.EnemyVelocity);
            double futureY = this.bb.CurrentEnemy.getFutureY((long)time, this.bb.CurrentEnemy.EnemyHeading, this.bb.CurrentEnemy.EnemyVelocity);
            double absDeg = AbsoluteBearing(this.bb.Robot.X, this.bb.Robot.Y, futureX, futureY);
            // turn the gun to the predicted x,y location
            this.bb.Robot.TurnGunRight(normalizeBearing(absDeg - this.bb.Robot.GunHeading));
            // if the gun is cool and we're pointed in the right direction, shoot!



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
