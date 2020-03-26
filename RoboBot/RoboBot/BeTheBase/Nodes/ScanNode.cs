using System;
using System.Collections.Generic;
using System.Text;
using Robocode;
using Robocode.Util;

namespace BCDK
{
    public class ScanNode : BaseNode
    {
        private double scanRadians;
        public ScanNode(BlackBoard bb, double scanRadians)
        {
            this.bb = bb;
            this.scanRadians = scanRadians;
        }

        public override BehaviourTreeStatus Tick()
        {
            //if (this.bb.Robot.RadarTurnRemaining == 0.0)
                //this.bb.Robot.SetTurnRadarRightRadians(scanRadians);
            this.bb.Robot.SetTurnRadarRight(scanRadians);
            //this.bb.Robot.Scan();
            return BehaviourTreeStatus.Success;
        }       
    }
}
