using System;
using System.Collections.Generic;
using System.Text;
using Robocode.Util;
using Robocode;

namespace BCDK
{
    public class TurnNode : BaseNode
    {
        public TurnNode(BlackBoard bb)
        {
            this.bb = bb;
        }

        public override BehaviourTreeStatus Tick()
        {
            if (this.bb.CurrentEnemy == null)
            {
                return BehaviourTreeStatus.Failure;
            }
            AdvancedRobot r = this.bb.Robot;

            if (this.bb.MovementDirection > 0)
                r.SetTurnRight(Utils.NormalRelativeAngleDegrees(this.bb.CurrentEnemy.E.Bearing + 80));
            else
                r.SetTurnRight(Utils.NormalRelativeAngleDegrees(this.bb.CurrentEnemy.E.Bearing + 100));

            return BehaviourTreeStatus.Success;
        }
    }
}
