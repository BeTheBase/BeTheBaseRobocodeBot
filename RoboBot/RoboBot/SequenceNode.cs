using System;
using System.Collections.Generic;
using System.Text;

namespace BCDK
{
    /// <summary>
    /// Runs child nodes in sequence, until one fails.
    /// </summary>
    public class SequenceNode : BaseNode
    {
        /// <summary>
        /// List of child nodes.
        /// </summary>
        private BaseNode[] inputNodes;

        public SequenceNode(BlackBoard bb, params BaseNode[] _inputNodes)
        {
            this.inputNodes = _inputNodes;
            this.bb = bb;
        }

        public override BehaviourTreeStatus Tick()
        {
            foreach (BaseNode node in inputNodes)
            {
                var childStatus = node.Tick();
                if (childStatus != BehaviourTreeStatus.Success)
                {
                    return childStatus;
                }
            }

            return BehaviourTreeStatus.Success;
        }
    }
}
