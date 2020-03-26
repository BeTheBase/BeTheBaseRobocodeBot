using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Robocode.Util;
using Robocode;
namespace BCDK
{
    public class AimNode : BaseNode
    {
        public AimNode(BlackBoard bb)
        {
            this.bb = bb;
        }

        public override BehaviourTreeStatus Tick()
        {
            if(this.bb.Robot == null|| this.bb.CurrentEnemy == null)
            {
                return BehaviourTreeStatus.Failure;
            }
            AimGun();
            /*
            AdvancedRobot r = this.bb.Robot;
            EnemyData e = this.bb.CurrentEnemy;
            Intercept intercept = new Intercept();
            intercept.Calculate
                (r.X,
                r.Y,
                e.x,
                e.y,
                e.GetHeading(),
                e.GetVelocity(),
                3,
                0
                );

            // Helper function that converts any angle into
            // an angle between +180 and -180 degrees.
            double turnAngle = Utils.NormalRelativeAngle
             (intercept.bulletHeading_deg - r.GunHeading);
            // Move gun to target angle
            r.SetTurnGunRight(normaliseBearing(turnAngle));
            if (Math.Abs(turnAngle) <= intercept.angleThreshold)
            {
                // Ensure that the gun is pointing at the correct angle
                if (
                (intercept.impactPoint.x > 0) &&
                (intercept.impactPoint.x < r.BattleFieldWidth) &&
                (intercept.impactPoint.y > 0) &&
                (intercept.impactPoint.y < r.BattleFieldHeight)
               )
                {
                    // Ensure that the predicted impact point is within
                    // the battlefield
                    r.Fire(this.bb.Power);
                }
            }
        */
            return BehaviourTreeStatus.Success;
        }

        /**This function predicts the time of the intersection between the 
            bullet and the target based on a simple iteration.  It then moves 
            the gun to the correct angle to fire on the target.**/
        public void AimGun()
        {
            EnemyData target = this.bb.CurrentEnemy;
            AdvancedRobot r = this.bb.Robot;
            long time;
            long nextTime;
            Point p;
            p = new Point((int)target.x, (int)target.y);
            for (int i = 0; i < 10; i++)
            {
                nextTime = (long)Math.Round((GetRange(r.X, r.Y, p.X, p.Y) / (20 - (3 * Rules.MAX_BULLET_POWER))));
                time = r.Time + nextTime;
                p = target.guessPosition(time);
            }
            /**Turn the gun to the correct angle**/
            double gunOffset = r.GunHeadingRadians -
                          (Math.PI / 2 - Math.Atan2(p.Y - r.Y, p.X - r.X));
            r.SetTurnGunLeftRadians(NormaliseBearing(gunOffset));
        }

        private double NormaliseBearing(double ang)
        {
            if (ang > Math.PI)
                ang -= 2 * Math.PI;
            if (ang < -Math.PI)
                ang += 2 * Math.PI;
            return ang;
        }

        private double GetRange(double x1, double y1, double x2, double y2)
        {
            double x = x2 - x1;
            double y = y2 - y1;
            double h = Math.Sqrt(x * x + y * y);
            return h;
        }
    }
}
