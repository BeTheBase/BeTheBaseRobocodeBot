using System;
using System.Collections.Generic;
using System.Text;

namespace BCDK
{
    public enum BehaviourTreeStatus
    {
        Success,
        Failure,
        Running
    }

    public interface INodeBase
    {
        void OnInitialize(BlackBoard bb);
        BehaviourTreeStatus Tick(TimeData timeData);
    }

    public abstract class BaseNode
    {
        protected BlackBoard bb;
        public abstract BehaviourTreeStatus Tick();
    }
}
