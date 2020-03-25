using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Robocode.Util;
using Robocode;

namespace BCDK
{
    public class QSNode : BaseNode
    {
        public QSNode(BlackBoard bb)
        {
            this.bb = bb;
        }

        public override BehaviourTreeStatus Tick()
        {

            if (this.bb.Robot.GunHeat == 0 && Math.Abs(this.bb.Robot.GunTurnRemaining) < 10)
            {
                this.bb.Robot.Fire(this.bb.Power);
            }

            return BehaviourTreeStatus.Success;
        }
    }
}
