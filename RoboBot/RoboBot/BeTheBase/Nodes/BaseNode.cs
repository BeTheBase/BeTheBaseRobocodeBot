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

    public abstract class BaseNode
    {
        protected BlackBoard bb;
        public abstract BehaviourTreeStatus Tick();
    }
}
