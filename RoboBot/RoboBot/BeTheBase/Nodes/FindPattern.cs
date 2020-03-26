using Robocode;
using Robocode.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Drawing;

namespace BCDK
{
    public class FindPattern : BaseNode
    {
        private List<EnemyData> EnemyKeeper = new List<EnemyData>();
        public FindPattern(BlackBoard bb)
        {
            this.bb = bb;
        }

        public override BehaviourTreeStatus Tick()
        {
            ScannedRobotEvent e = this.bb.CurrentEnemy.E;
            double absbearing = (e.BearingRadians + e.HeadingRadians);
            double currentEnemyVel = e.Velocity;
            double currentEnemyHeading = e.Heading;
            double latVel = currentEnemyVel * Math.Sin(e.HeadingRadians - absbearing);
            double gunTurnAmt;
            gunTurnAmt = Utils.NormalRelativeAngle(absbearing - this.bb.Robot.GunHeading + latVel / 22);//amount to turn our gun, lead just a little bit
            //this.bb.Robot.TurnGunRight(gunTurnAmt);
            //this.bb.Robot.TurnLeft(-90 - e.Bearing);
            //this.bb.Robot.TurnRight(Utils.NormalRelativeAngle(absbearing - this.bb.Robot.Heading + latVel / currentEnemyVel));//drive towards the enemies predicted future location
            //this.bb.Robot.Ahead((e.Distance - 140) * 1);//move forward

            //We need to keep track of our enemy his velocity and heading change each tick
            EnemyKeeper.Add(new EnemyData(e, currentEnemyVel, currentEnemyHeading));
            if(EnemyKeeper.Count > 7)
            {
                for(int index = EnemyKeeper.Count-7; index < EnemyKeeper.Count; index++)
                {
                    //Add the last 7 movements of our enemy into the EnemyLogger so we can now use this later on
                    this.bb.EnemyLogger.Add(EnemyKeeper[index]);
                }
                if(this.bb.EnemyLogger.Count > 7)
                {
                    this.bb.EnemyLogger.RemoveAt(0);
                }
            }
            else
            {
                for (int index = 0; index < EnemyKeeper.Count; index++)
                {
                    //Add the last 7 movements of our enemy into the EnemyLogger so we can now use this later on
                    this.bb.EnemyLogger.Add(EnemyKeeper[index]);
                }
                if (this.bb.EnemyLogger.Count > 7)
                {
                    this.bb.EnemyLogger.RemoveAt(0);
                }
            }

            // find a stretch of 7 game turns that most closely resemble the opponent's behaviour in the last 7 turns
            int turns = 7;
            List<double> tracker = new List<double>();
            /*
            foreach(EnemyData enemyData in this.bb.EnemyLogger)
            {
                //var foundCloseMath = EnemyKeeper.Find(e => e.EnemyVelocity.CompareTo(enemyData.EnemyVelocity));

                //Look if we can find a pattern that happend in the past
                for(int keeperIndex = 0; keeperIndex < EnemyKeeper.Count-7; keeperIndex++)
                {
                    var S = Math.Abs(EnemyKeeper[keeperIndex].EnemyVelocity - enemyData.EnemyVelocity) + Math.Abs(EnemyKeeper[keeperIndex].EnemyHeading - enemyData.EnemyHeading);
                    match.Add(keeperIndex, S);
                }
            }*/
            Dictionary<int, double> match = new Dictionary<int, double>();
            for (int keeperIndex = 0; keeperIndex < EnemyKeeper.Count; keeperIndex++)
            {
                var S = Math.Abs(EnemyKeeper[keeperIndex].EnemyVelocity - this.bb.EnemyLogger[0].EnemyVelocity) + Math.Abs(EnemyKeeper[keeperIndex].EnemyHeading - this.bb.EnemyLogger[0].EnemyHeading);
                match.Add(keeperIndex, S);

                /*
                tracker.Add(S);
                if(keeperIndex == 0)
                {
                    match.Add(keeperIndex, S);
                }
                if(S < tracker[keeperIndex])
                {
                    if (S < match[keeperIndex])
                    {
                        match[keeperIndex] = S;
                    }
                    else
                    {
                        match.Add(keeperIndex, S);
                    }
                }*/
            }
            KeyValuePair<int, double> low = new KeyValuePair<int, double>(0,100);
            foreach(KeyValuePair<int, double> keyValuePair in match)
            {
                if(low.Value > keyValuePair.Value)
                {
                    low = keyValuePair;
                }
            }
            this.bb.PredictedEnemyDataStart = EnemyKeeper[low.Key];
            if(!this.bb.Match.ContainsKey(low.Key))
                this.bb.Match.Add(low.Key, low.Value);

            return BehaviourTreeStatus.Success;
        }
    }
}
