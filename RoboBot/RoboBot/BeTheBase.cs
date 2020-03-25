using System;
using System.Collections.Generic;
using System.Text;
using Robocode;
using Robocode.Util;

namespace BCDK
{
    public class BeTheBase : AdvancedRobot
    {
        BaseNode attackBehaviour;
        BaseNode moveBehaviour;
        BaseNode behaviourTree;
        BaseNode dodgeBehaviour;
        BaseNode currentBehaviour;
        BaseNode testBehaviour;
        BlackBoard blackBoard = new BlackBoard();
        bool scannReady = false;
        int bulletsMissed = 0;
        int bulletMissedMax = 0;
        int bulletHitBulletCount = 0;
        int incomingHitss = 0;
        int scannCheckTime = 3;
        double turnRadians = 360;
        bool move = false;
        bool attackReady = false;

        public override void Run()
        {
            base.Run();

            blackBoard.Robot = this;
            TurnRadarRight(360);
            dodgeBehaviour = new SequenceNode(blackBoard, new ScanNode(blackBoard, 360), new TrackNode(blackBoard, 360, 40), new AimNode(blackBoard), new CircleAroundNode(blackBoard));
            attackBehaviour = new SequenceNode(blackBoard, 
                new ScanNode(blackBoard, 360), 
                new TrackNode(blackBoard, 360, 30), 
                new TurnNode(blackBoard), 
                new AnitGravMoveNode(blackBoard, 200),
                new AimNode(blackBoard),
                new ConditionNode(blackBoard, new Func<bool>(() => AttackCheck()), new QSNode(blackBoard)));
            behaviourTree = new SequenceNode(blackBoard, new ScanNode(blackBoard, 360),
                new FindPattern(blackBoard),
                new FireNode(blackBoard, 2),
                new ConditionNode(blackBoard, new Func<bool>(() => blackBoard.DirectHit), new FireNode(blackBoard, 3)),
                new MoveNode(blackBoard, 200));
            //new ConditionNode(blackBoard, new Func<bool>(() => RangeCheck()), new RageNode(blackBoard, 3)));

            moveBehaviour = new SequenceNode(blackBoard, new ScanNode(blackBoard, 360), new TrackNode(blackBoard, 360, 40), new SetPowerNode(blackBoard), new AimNode(blackBoard), new ConditionNode(blackBoard, new Func<bool>(() => AttackCheck()),new QSNode(blackBoard)), new AnitGravMoveNode(blackBoard, 100));
            testBehaviour = new SequenceNode(blackBoard,
                new ScanNode(blackBoard, 360),              
                new AnitGravMoveNode(blackBoard,200),
                new TurnNode(blackBoard),
                new SetPowerNode(blackBoard),
                new TrackNode(blackBoard, 360, 40),
                new AimNode(blackBoard),
                //new FireNode(blackBoard, 3),
                new ConditionNode(blackBoard, new Func<bool>(()=> AttackCheck()), new QSNode(blackBoard)));

            currentBehaviour = testBehaviour;

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
            double changeInEnergy = this.blackBoard.PreviousEnergy - this.blackBoard.ScannendEvent.Energy;
            if (changeInEnergy >= -3 && changeInEnergy <= 3)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CheckEnemyDistance()
        {
            double distance = this.blackBoard.ScannendEvent.Distance;
            if (distance > 150)
                return true;
            else
                return false;
        }

        public bool RangeCheck()
        {
            double power = (400 / this.blackBoard.ScannendEvent.Distance);
            // selects a bullet power based on our distance away from the target
            bool check = false;
            if ((power >= 2))
            {
                power = 3;
                check = true;
            }
            else
            {
                check = false;
            }
            return check;
        }

        public override void OnScannedRobot(ScannedRobotEvent evnt)
        {
            base.OnScannedRobot(evnt);
            if(blackBoard.CurrentEnemy == null)
            {
                blackBoard.CurrentEnemy = new EnemyData(evnt, evnt.Velocity, evnt.Heading);
            }
            blackBoard.ScannendEvent = evnt;
            blackBoard.CurrentEnemy.Update(evnt, this);
            

            /*
            if (scannReady)
            {
                if (evnt != null)
                {
                    int scannedX = 0;
                    int scannedY = 0;
                    double angle = Utils.ToRadians((Heading + evnt.Bearing) % 360);
                    scannedX = (int)(X + Math.Sin(angle) * evnt.Distance);
                    scannedY = (int)(Y + Math.Cos(angle) * evnt.Distance);
                    //GoTo(scannedX, scannedY);
                    double absoluteBearing = Heading + evnt.Bearing;
                    TurnGunRight(Utils.ToDegrees(Utils.NormalRelativeAngle(Utils.ToRadians(absoluteBearing - GunHeading))));
                    Fire(1);
                    scannReady = false;
                }
            }*/
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
                currentBehaviour = attackBehaviour;
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
            else if(Time % 100 == 0)
            {
                blackBoard.DirectHit = true;
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
            currentBehaviour = attackBehaviour;
            Execute();
        }
    }
}
