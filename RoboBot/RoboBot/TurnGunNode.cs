using Robocode.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace BCDK
{
    public class TurnGunNode : BaseNode
    {
        public TurnGunNode(BlackBoard bb)
        {
            this.bb = bb;
        }

        public double GetAngleOfGunHeading()
        {
            BlackBoard blackBoard = this.bb;
            if (this.bb.ScannendEvent == null)
            {
                return 0;
            }

            double angle = blackBoard.ScannendEvent.Bearing -
                    (blackBoard.Robot.GunHeading - blackBoard.Robot.Heading);

            if (angle < -180)
            {
                angle += 360;
            }
            return angle;

        }

        public override BehaviourTreeStatus Tick()
        {
            BlackBoard blackBoard = this.bb;
            double rotation = 0;
            double accuracy = 5;

            rotation = GetAngleOfGunHeading();
            double distance = blackBoard.ScannendEvent.Distance;
            accuracy = 180 / (distance * 0.1f);
            double absoluteBearing = this.bb.Robot.Heading + this.bb.ScannendEvent.Bearing;
            this.bb.Robot.TurnGunRight(Utils.ToDegrees(Utils.NormalRelativeAngle(Utils.ToRadians(absoluteBearing - this.bb.Robot.GunHeading))));
            //blackBoard.Robot.TurnGunRight(rotation);

            return BehaviourTreeStatus.Success;


            if (Math.Abs(rotation) < accuracy)
            {
                return BehaviourTreeStatus.Success;
            }
            else
            {
                return BehaviourTreeStatus.Running;
            }
        }
    }
}
