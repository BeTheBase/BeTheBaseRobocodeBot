using System;
using System.Collections.Generic;
using System.Text;
using Robocode;

namespace BCDK
{
    class TestBot : Robot
    {
        public override void Run()
        {
            base.Run();
            while(true)
            {
                Ahead(100);
                TurnGunRight(360);
                Back(100);
                TurnGunRight(360);
                
            }
        }

        public override void OnScannedRobot(ScannedRobotEvent evnt)
        {
            base.OnScannedRobot(evnt);
            Fire(1);
        }

        public override void OnHitByBullet(HitByBulletEvent evnt)
        {
            base.OnHitByBullet(evnt);
            TurnLeft(90 - evnt.Bearing);
        }
    }
}
