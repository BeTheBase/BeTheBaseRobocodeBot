using System;
using System.Collections.Generic;
using System.Text;

namespace BCDK
{
    public class ActionNode : INodeBase
    {

        /// <summary>
        /// The name of the node.
        /// </summary>
        private string name;

        /// <summary>
        /// Function to invoke for the action.
        /// </summary>
        private Func<TimeData, BehaviourTreeStatus> fn;


        public ActionNode(string name, Func<TimeData, BehaviourTreeStatus> fn)
        {
            this.name = name;
            this.fn = fn;
        }

        public void OnInitialize(BlackBoard bb)
        {
            throw new NotImplementedException();
        }

        public BehaviourTreeStatus Tick(TimeData time)
        {
            return fn(time);
        }
    }
}
