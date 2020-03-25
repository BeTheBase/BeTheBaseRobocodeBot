using Robocode;
using Robocode.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Drawing;

namespace BCDK
{
    public class EnemyData : Robot
    {
        public ScannedRobotEvent E;
        public double EnemyVelocity;
        public double EnemyHeading;

        public double x;
        public double y;


        private double bearing;
        private double distance;
        private double energy;
        private double heading;
        private string name = "";
        private double velocity;
        private long time;
        private Robot r;

        public EnemyData(ScannedRobotEvent e, double enemyVel, double enemyHeading)
        {
            this.E = e;
            this.EnemyVelocity = enemyVel;
            this.EnemyHeading = enemyHeading;
        }

        public void Update(ScannedRobotEvent e, Robot robot)
        {
            E = e;
            bearing = e.Bearing;
            distance = e.Distance;
            energy = e.Energy;
            heading = e.Heading;
            name = e.Name;
            velocity = e.Velocity;
            time = robot.Time;
            r = robot;
            double absBearingDeg = (robot.Heading + e.Bearing);
            if (absBearingDeg < 0) absBearingDeg += 360;

            // yes, you use the _sine_ to get the X value because 0 deg is North
            x = robot.X + Math.Sin(Utils.ToRadians(absBearingDeg)) * e.Distance;

            // yes, you use the _cosine_ to get the Y value because 0 deg is North
            y = robot.Y + Math.Cos(Utils.ToRadians(absBearingDeg)) * e.Distance;
        }

        public Point guessPosition(long when)
        {
            /**time is when our scan data was produced.  when is the time 
            that we think the bullet will reach the target.  diff is the 
            difference between the two **/
            double diff = when - time;
            double newX, newY;
            /**if there is a significant change in heading, use circular 
            path prediction**/
            if (Math.Abs(heading) > 0.00001)
            {
                double radius = velocity / heading;
                double tothead = diff * heading;
                newY = y + (Math.Sin(heading + tothead) * radius) -
                              (Math.Sin(heading) * radius);
                newX = x + (Math.Cos(heading) * radius) -
                              (Math.Cos(heading + tothead) * radius);
            }
            /**if the change in heading is insignificant, use linear 
            path prediction**/
            else
            {
                newY = y + Math.Cos(heading) * velocity * diff;
                newX = x + Math.Sin(heading) * velocity * diff;
            }
            return new Point((int)newX, (int)newY);
        }

        public double getFutureX(long when, double heading, double velocity)
        {
            return x + Math.Sin(Utils.ToRadians(heading)) * velocity * when;
        }

        public double GetFutureX(long when)
        {
            return x + Math.Sin(Utils.ToRadians(heading)) * velocity * when;
        }

        public double GetFutureY(long when)
        {
            return y + Math.Cos(Utils.ToRadians(heading)) * velocity * when;
        }

        public double getFutureY(long when, double heading, double velocity)
        {
            return y + Math.Cos(Utils.ToRadians(heading)) * velocity * when;
        }

        public double GetDistance()
        {
            return distance;
        }

        public double GetHeading()
        {
            return heading;
        }

        public double GetVelocity()
        {
            return velocity;
        }
    }

    public class FindPattern : BaseNode
    {
        private List<EnemyData> EnemyKeeper = new List<EnemyData>();
        public FindPattern(BlackBoard bb)
        {
            this.bb = bb;
        }

        public override BehaviourTreeStatus Tick()
        {
            ScannedRobotEvent e = this.bb.ScannendEvent;
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
