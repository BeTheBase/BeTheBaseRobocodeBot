using System;
using System.Collections.Generic;
using System.Text;
using Robocode;

namespace BCDK
{
    public class BlackBoard
    {
        public AdvancedRobot Robot;
        public ScannedRobotEvent ScannendEvent;
        public List<EnemyData> EnemyLogger = new List<EnemyData>();
        public Dictionary<int, double> Match = new Dictionary<int, double>();
        public EnemyData PredictedEnemyDataStart;
        public EnemyData CurrentEnemy;
        public float MovementDirection = 1;
        public double PreviousEnergy = 100;
        public int GunDirection = 1;
        public double FireRange = 200;
        public bool DirectHit = false;
        public double Power = 2;
    }
}
