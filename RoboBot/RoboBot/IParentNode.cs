using System;
using System.Collections.Generic;
using System.Text;

namespace BCDK
{
    interface IParentNode : INodeBase
    {
        void AddChild(INodeBase child);
    }
}
