using System;
using System.Collections.Generic;
using System.Text;

namespace BCDK
{
    /// <summary>
    /// Decorator node that inverts the success/failure of its child.
    /// </summary>
    public class InverterNode : BaseNode
    {
        /// <summary>
        /// The child to be inverted.
        /// </summary>
        private BaseNode childNode;

        public InverterNode(BlackBoard bb, BaseNode child)
        {
            this.bb = bb;
            this.childNode = child;
        }

        public override BehaviourTreeStatus Tick()
        {
            if (childNode == null)
            {
                throw new ApplicationException("InverterNode must have a child node!");
            }

            var result = childNode.Tick();
            switch(result)
            {
                case BehaviourTreeStatus.Failure:
                    return BehaviourTreeStatus.Success;
                case BehaviourTreeStatus.Success:
                    return BehaviourTreeStatus.Failure;
                default:
                    return result;
            }
            /*
            if (result == BehaviourTreeStatus.Failure)
            {
                return BehaviourTreeStatus.Success;
            }
            else if (result == BehaviourTreeStatus.Success)
            {
                return BehaviourTreeStatus.Failure;
            }
            else
            {
                return result;
            }*/
        }
    }
}
