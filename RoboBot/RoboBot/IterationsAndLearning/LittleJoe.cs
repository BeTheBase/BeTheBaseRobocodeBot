using System;
using System.Collections.Generic;
using System.Linq;
using Robocode;
using Robocode.Util;
using System.Drawing;

namespace BCDK
{
    public class LittleJoe : AdvancedRobot
    {
        public Effect State;
        public INodeBase Tree;

        private bool robotInRange = false;

        // The coordinates of the last scanned robot
        int scannedX = 0;
        int scannedY = 0;

        double moveAmount =1;
        bool peek;
        double lowEnergy = 100.0;
        ScannedRobotEvent nextVictim;


        int wallMargin = 60;
        int toCloseToWall = 0;
        Dictionary<string, ScannedRobotEvent> enemyData = new Dictionary<string, ScannedRobotEvent>();
        bool createEvent = true;

        public override void OnScannedRobot(ScannedRobotEvent evnt)
        {
            nextVictim = evnt;
            //Store the incoming enemy name 
            if (!enemyData.ContainsKey(evnt.Name))
            {
                enemyData.Add(evnt.Name, evnt);
            }

            //Remove old enemy data that is more than 10 turns old
            foreach(ScannedRobotEvent sre in enemyData.Values)
            {
                if(sre.Time <= Time -10)
                {
                    enemyData.Remove(sre.Name);
                }
            }

            //Find the weakest robot ( least amount of energy )
            double minEnergy = Double.MaxValue;
            foreach(ScannedRobotEvent sre in enemyData.Values)
            {
                if(sre.Energy < minEnergy)
                {
                    minEnergy = sre.Energy;
                    nextVictim = sre;
                }               
            }

            if(evnt.Equals(nextVictim))
            {
                //When the scanned enemy is the weakest, target it and we fire
                double absoluteBearing = Heading + evnt.Bearing;
                TurnGunRight(Utils.ToDegrees(Utils.NormalRelativeAngle(Utils.ToRadians(absoluteBearing - GunHeading))));
                Fire(1);
            }

            if (evnt.Distance < 100)
            {
                robotInRange = true;
            }

            if(peek)
            {
                Scan();
            }
        }

        public void AddEffectsToState(List<EffectState> effects)
        {
            foreach (EffectState state in effects)
            {
                if (state.IsActive)
                {
                }
            }
        }

        public static void PlanActions(EffectState state, List<Action> availableActions, List<Goal> availableGoals, out Goal outGoal)
        {
            outGoal = null;
            availableGoals = ShuffleGoals(availableGoals, state); // Needs shuffleing wich goals are available
            foreach (Goal goal in availableGoals.OrderByDescending(g => g.Priority).ToList())
            {
                if(goal.IsViable(state))
                {
                    //Do plan with AStar search (goal, state, availableActions, ref outGoal)
                    if(outGoal != null)
                    {
                        return;
                    }
                }
            }
        }

        public static List<Goal> ShuffleGoals(List<Goal> Goals, EffectState state)
        {
            List<Goal> result = new List<Goal>();
            foreach(Goal goal in Goals.FindAll(g => g.IsViable(state)))
            {
                goal.Initialize(goal.agent);
                result.Add(goal);
            }

            return result;

        }

        public void Setup()
        {

        }

        public override void OnCustomEvent(CustomEvent evnt)
        {
            base.OnCustomEvent(evnt);

            if (evnt.Condition.name.Equals("too_close_to_walls"))
            {
                if (toCloseToWall <= 0)
                {
                    toCloseToWall += wallMargin;
                    MaxVelocity = 0;
                }

            }
        }

        Condition wallCondition;

        public override void Run()
        {
            BodyColor = Color.MediumBlue; 
            GunColor = Color.DarkRed;
            RadarColor = Color.Black;
            ScanColor = Color.White;
            BulletColor = Color.Green;

            if (createEvent)
            {
                wallCondition = new Condition("to_close_to_walls", (c) =>
                {
                    return (
                    // we're too close to the left wall
                    (X <= wallMargin ||
                     // or we're too close to the right wall
                     X >= BattleFieldWidth - wallMargin ||
                     // or we're too close to the bottom wall
                     Y <= wallMargin ||
                     // or we're too close to the top wall
                     Y >= BattleFieldHeight - wallMargin)
                    );
                });

                AddCustomEvent(wallCondition);

                createEvent = false;
            }
            //this.Tree.Tick(new TimeData(1.14f));

            TurnGunRight(360);
                // always square off against our enemy, turning slightly toward him
                //if (nextVictim == null) return;
                TurnRight(nextVictim.Bearing + 90 - (10 * moveAmount));

            // if we're close to the wall, eventually, we'll move away
            if (toCloseToWall > 0)
            {
                toCloseToWall--;
            }

                // normal movement: switch directions if we've stopped
                if (Velocity == 0)
                {
                    moveAmount *= -1;
                    Ahead(10000 * moveAmount);
                }

            Execute();

            while(true)
            {
                TurnGunRight(360);
                
            }


        }

        public override void OnHitRobot(HitRobotEvent evnt)
        {
            base.OnHitRobot(evnt);
            if(evnt.Bearing > -90 && evnt.Bearing < 90)
            {
                Back(100);
            }
            else
            {
                Ahead(100);
            }
        }



        public void Move(int dir)
        {

        }

        public void OnFire(double power)
        {
            if(robotInRange)
            {
                //Note that the power has a cost of the robot his Health
                Fire(power);
            }
        }

        public override void OnHitWall(HitWallEvent evnt)
        {
            base.OnHitWall(evnt);
        }

        public override void OnBulletMissed(BulletMissedEvent evnt)
        {
            base.OnBulletMissed(evnt);
        }

        public override void OnBulletHit(BulletHitEvent evnt)
        {
            base.OnBulletHit(evnt);
        }

        public override void OnHitByBullet(HitByBulletEvent evnt)
        {
            base.OnHitByBullet(evnt);
        }



    }
}
