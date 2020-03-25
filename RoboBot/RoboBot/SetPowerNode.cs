using System;
using System.Collections.Generic;
using System.Text;

namespace BCDK
{
    public class SetPowerNode : BaseNode
    {
        public SetPowerNode(BlackBoard bb)
        {
            this.bb = bb;
        }

        public override BehaviourTreeStatus Tick()
        {
            if (this.bb.Robot == null || this.bb.CurrentEnemy == null)
            {
                return BehaviourTreeStatus.Failure;
            }
            this.bb.Power = (400 / this.bb.CurrentEnemy.E.Distance);
            if (this.bb.Power >= 3)
                this.bb.Power = 3;
            return BehaviourTreeStatus.Success;
        }
    }
}
