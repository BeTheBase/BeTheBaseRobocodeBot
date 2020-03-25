using System;
using System.Collections.Generic;
using System.Text;
using Robocode.Util;
using Robocode;

namespace BCDK
{
    public class TrackNode : BaseNode
    {
        private double scanRadians;
        private double trackArea;
        public TrackNode(BlackBoard bb, double scanRadians, double trackArea)
        {
            this.bb = bb;
            this.scanRadians = scanRadians;
            this.trackArea = trackArea;
        }

        public override BehaviourTreeStatus Tick()
        {
            if (this.bb.CurrentEnemy == null)
            {
                return BehaviourTreeStatus.Failure;
            }
            /*
            ///Turn  multiplier lock
            // Absolute bearing to target
            // Subtract current radar heading to get turn required
            double radarTurn = this.bb.Robot.HeadingRadians + this.bb.ScannendEvent.BearingRadians - this.bb.Robot.RadarHeadingRadians;

            this.bb.Robot.TurnRadarRightRadians(Utils.NormalRelativeAngle(radarTurn));
            this.bb.Robot.SetTurnRadarRight(scanRadians);
            */
            //Width lock
            // Absolute angle towards target
            double angleToEnemy = this.bb.Robot.HeadingRadians + this.bb.CurrentEnemy.E.BearingRadians;

            // Subtract current radar heading to get the turn required to face the enemy, be sure it is normalized
            double radarTurn = Utils.NormalRelativeAngle(angleToEnemy - this.bb.Robot.RadarHeadingRadians);

            // Distance we want to scan from middle of enemy to either side
            // The 36.0 is how many units from the center of the enemy robot it scans.
            double extraTurn = Math.Min(Math.Atan(trackArea / this.bb.CurrentEnemy.GetDistance()), Rules.RADAR_TURN_RATE_RADIANS);

            // Adjust the radar turn so it goes that much further in the direction it is going to turn
            // Basically if we were going to turn it left, turn it even more left, if right, turn more right.
            // This allows us to overshoot our enemy so that we get a good sweep that will not slip.
            if (radarTurn < 0)
                radarTurn -= extraTurn;
            else
                radarTurn += extraTurn;

            this.bb.Robot.SetTurnRadarRightRadians(radarTurn);
            //this.bb.Robot.SetTurnGunRight(radarTurn);
            //Turn the radar
            /*
            ScannedRobotEvent e = this.bb.ScannendEvent;
            var gunTurnAmt = Utils.NormalRelativeAngleDegrees(e.Bearing + (this.bb.Robot.Heading - this.bb.Robot.RadarHeading));

            if (e.Distance > 150)
            {
                //this.bb.Robot.SetTurnGunRight(gunTurnAmt);
                this.bb.Robot.SetTurnRight(e.Bearing);
                this.bb.Robot.SetAhead(50* this.bb.MovementDirection);
            }
            if(e.Distance < 100)
            {
                if(e.Bearing > -90 && e.Bearing <= 90)
                {
                    this.bb.Robot.SetBack(50* this.bb.MovementDirection);
                }
                else
                {
                    this.bb.Robot.SetAhead(50);
                }
            }*/

            return BehaviourTreeStatus.Success;
        }
    }
}
