using System;
using System.Collections.Generic;
using System.Text;

namespace BCDK
{
    public class ConditionNode : BaseNode
    {
        private BaseNode[] inputNodes;
        private Func<bool> condition;

        public ConditionNode(BlackBoard blackBoard, Func<bool> condition, params BaseNode[] input)
        {
            this.bb = blackBoard;
            this.condition = condition;
            this.inputNodes = input;
        }

        public override BehaviourTreeStatus Tick()
        {
            if (!condition())
            {
                return BehaviourTreeStatus.Success;
            }
            foreach (BaseNode node in inputNodes)
            {
                BehaviourTreeStatus result = node.Tick();

                switch (result)
                {
                    case BehaviourTreeStatus.Failure:
                        return BehaviourTreeStatus.Failure;
                    case BehaviourTreeStatus.Running:
                        return BehaviourTreeStatus.Running;
                    case BehaviourTreeStatus.Success:
                        //only at succes it moves on to the next task
                        break;
                    default:
                        break;
                }
            }

            return BehaviourTreeStatus.Failure;
        }
    }
}
