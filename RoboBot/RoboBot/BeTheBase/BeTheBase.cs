using System;
using System.Collections.Generic;
using System.Text;
using Robocode;
using Robocode.Util;

namespace BCDK
{
    public class BeTheBase : AdvancedRobot
    {
        BaseNode pushBehaviour;
        BaseNode moveBehaviour;
        BaseNode paternBehaviour;
        BaseNode dodgeBehaviour;
        BaseNode currentBehaviour;
        BaseNode antiGravBehaviour;
        BlackBoard blackBoard = new BlackBoard();

        int bulletsMissed = 0;
        int bulletMissedMax = 0;
        int bulletHitBulletCount = 0;
        int incomingHitss = 0;
        int scannCheckTime = 3;

        double turnRadians = 360;

        bool scannReady = false;
        bool move = false;
        bool attackReady = false;

        public override void Run()
        {
            base.Run();

            blackBoard.Robot = this;
            TurnRadarRight(360);

            dodgeBehaviour = new SequenceNode(blackBoard, 
                new ScanNode(blackBoard, 360), 
                new TrackNode(blackBoard, 360, 40), 
                new AimNode(blackBoard), 
                new CircleAroundNode(blackBoard));

            pushBehaviour = new SequenceNode(blackBoard, 
                new ScanNode(blackBoard, 360), 
                new TrackNode(blackBoard, 360, 30), 
                new TurnNode(blackBoard), 
                new AnitGravMoveNode(blackBoard, 200),
                new AimNode(blackBoard),
                new ConditionNode(blackBoard, new Func<bool>(() => AttackCheck()), new QSNode(blackBoard)));

            paternBehaviour = new SequenceNode(blackBoard, 
                new ScanNode(blackBoard, 360),
                new FindPattern(blackBoard),
                new FireNode(blackBoard, 2),
                new ConditionNode(blackBoard, new Func<bool>(() => blackBoard.DirectHit), new FireNode(blackBoard, 3)),
                new MoveNode(blackBoard, 200));

            moveBehaviour = new SequenceNode(blackBoard, 
                new ScanNode(blackBoard, 360), 
                new TrackNode(blackBoard, 360, 40), 
                new SetPowerNode(blackBoard), 
                new AimNode(blackBoard), 
                new ConditionNode(blackBoard, new Func<bool>(() => AttackCheck()),new QSNode(blackBoard)), 
                new AnitGravMoveNode(blackBoard, 100));

            antiGravBehaviour = new SequenceNode(blackBoard,
                new ScanNode(blackBoard, 360),              
                new AnitGravMoveNode(blackBoard,200),
                new TurnNode(blackBoard),
                new SetPowerNode(blackBoard),
                new TrackNode(blackBoard, 360, 40),
                new AimNode(blackBoard),
                new ConditionNode(blackBoard, new Func<bool>(()=> AttackCheck()), new QSNode(blackBoard)));

            currentBehaviour = antiGravBehaviour;

            // divorce radar movement from gun movement
            IsAdjustRadarForGunTurn = true;
            // divorce gun movement from tank movement
            IsAdjustGunForRobotTurn=true;
            while (true)
            {
                currentBehaviour.Tick();               
                Execute();
            }
        }

        public bool AttackCheck()
        {
            if(this.blackBoard.CurrentEnemy.E.Distance > 400)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool MoveCheck()
        {
            double changeInEnergy = this.blackBoard.PreviousEnergy - this.blackBoard.CurrentEnemy.E.Energy;
            if (changeInEnergy >= -3 && changeInEnergy <= 3)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void OnScannedRobot(ScannedRobotEvent evnt)
        {
            base.OnScannedRobot(evnt);
            if(blackBoard.CurrentEnemy == null)
            {
                blackBoard.CurrentEnemy = new EnemyData(evnt, evnt.Velocity, evnt.Heading);
            }
            blackBoard.CurrentEnemy.Update(evnt, this);           
        }

        public override void OnHitWall(HitWallEvent evnt)
        {
            base.OnHitWall(evnt);

            blackBoard.MovementDirection *= -1;
        }

        public override void OnBulletMissed(BulletMissedEvent evnt)
        {
            base.OnBulletMissed(evnt);
            bulletMissedMax++;
            bulletsMissed++;
            if(bulletsMissed>2)
            {
                this.blackBoard.CurrentEnemy = null;
                //this.SetTurnRadarRightRadians(360);
                bulletsMissed = 0;
            }

            if(bulletMissedMax > 5)
            {
                currentBehaviour = pushBehaviour;
            }
        }

        public override void OnBulletHitBullet(BulletHitBulletEvent evnt)
        {
            base.OnBulletHitBullet(evnt);
            bulletHitBulletCount++;
            if(bulletHitBulletCount > 2)
            {
                this.blackBoard.Power = 1;
                bulletHitBulletCount = 0;
            }
        }

        public override void OnBulletHit(BulletHitEvent evnt)
        {
            base.OnBulletHit(evnt);
            bulletHitBulletCount = 0;
            bulletMissedMax = 0;
        }

        public override void OnHitByBullet(HitByBulletEvent evnt)
        {
            base.OnHitByBullet(evnt);
            //etTurnRadarRight(360);
            incomingHitss++;
            if (incomingHitss > 0&& !blackBoard.DirectHit || Time % 100 == 0)
            {
                // currentBehaviour = moveBehaviour;
                blackBoard.DirectHit = true;
                incomingHitss = 0;
            }
        }

        public override void OnHitRobot(HitRobotEvent evnt)
        {
            base.OnHitRobot(evnt);
            this.blackBoard.MovementDirection *= -1;
        }

        public override void OnRobotDeath(RobotDeathEvent evnt)
        {
            base.OnRobotDeath(evnt);
            SetTurnRadarRightRadians(360);
            blackBoard.CurrentEnemy = null;
            currentBehaviour = pushBehaviour;
            Execute();
        }
    }
}
