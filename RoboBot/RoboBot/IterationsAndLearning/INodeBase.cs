using System;
using System.Collections.Generic;
using System.Text;

namespace BCDK
{

    public interface INodeBase
    {
        void OnInitialize(BlackBoard bb);
        BehaviourTreeStatus Tick(TimeData timeData);
    }

}
