using System;
using System.Collections.Generic;
using System.Text;

namespace BCDK
{
    public class SelectorNode : BaseNode
    {
        private BaseNode[] inputNodes;

        public SelectorNode(BlackBoard bb, params BaseNode[] _inputNodes)
        {
            this.bb = bb;
            this.inputNodes = _inputNodes;
        }

        public override BehaviourTreeStatus Tick()
        {
            foreach (BaseNode node in inputNodes)
            {
                var childStatus = node.Tick();
                if (childStatus != BehaviourTreeStatus.Failure)
                {
                    return childStatus;
                }
            }

            return BehaviourTreeStatus.Failure;
        }
    }
}
