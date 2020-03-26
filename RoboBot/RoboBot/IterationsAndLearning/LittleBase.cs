using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Robocode;
using Robocode.Util;

namespace BCDK
{
    [System.Serializable]
    public class BaseStats
    {
        public double Power = 1;
        public double MovementSpeed = 100;
        public int MoveDirection = 1;
        
        public BaseStats(double power, double movementSpeed, int moveDirection)
        {
            Power = power;
            MovementSpeed = movementSpeed;
            MoveDirection = moveDirection;
        }
    }

    public class Vector
    {
        public double x;
        public double y;

        public Vector(double _x, double _y)
        {
            x = _x;
            y = _y;
        }
    }

class LittleBase : Robot
    {
        private BaseStats myFirstStats;
        private ScannedRobotEvent nextVictim;
        private int wallMargin = 60;
        private int toCloseToWall = 0;
        private bool peek = false;
        private bool scannReady = true;
        private Dictionary<string, ScannedRobotEvent> enemyData = new Dictionary<string, ScannedRobotEvent>();
        private int scannCheckTime = 5;
        private int counter = 0;
        double previousEnergy = 100;
        int movementDirection = 1;
        int gunDirection = 1;

        double enemyCloseRange = 50;
        double enemyMediumRange = 100;
        double enemyFarRange = 300;

        //Movement

        //We need a path
        List<PathNode> path = new List<PathNode>();
        Astar aStar = new Astar();
        bool walkPath = false;
        int scannedX = 0;
        int scannedY = 0;
        Grid<PathNode> myGrid;

        List<GravPoint> gravpoints;

        private void AntiGravMove()
        {
            double xforce = 0;
            double yforce = 0;
            double force;
            double ang;
            GravPoint p;

            for (int i = 0; i < gravpoints.Count; i++)
            {
                p = (GravPoint)gravpoints[i];
                //Calculate the total force from this point on us
                force = p.power / Math.Pow(getRange(X, Y, p.x, p.y), 2);
                //Find the bearing from the point to us
                ang = normaliseBearing(Math.PI / 2 - Math.Atan2(X - p.y, Y - p.x));
                //Add the components of this force to the total force in their 
                //respective directions
                xforce += Math.Sin(ang) * force;
                yforce += Math.Cos(ang) * force;
            }

            /**The following four lines add wall avoidance.  They will only 
            affect us if the bot is close to the walls due to the
            force from the walls decreasing at a power 3.**/
            xforce += 5000 / Math.Pow(getRange(X,
              Y, BattleFieldWidth, Y), 3);
            xforce -= 5000 / Math.Pow(getRange(X,
              Y, 0, X), 3);
            yforce += 5000 / Math.Pow(getRange(X,
              Y, X, BattleFieldHeight), 3);
            yforce -= 5000 / Math.Pow(getRange(X,
              Y, X, 0), 3);

            //Move in the direction of our resolved force.
            goTo(X - xforce, Y - yforce);
        }

        /**Move in the direction of an x and y coordinate**/
        void goTo(double x, double y)
        {
            double dist = 20; 
            double angle = Utils.ToDegrees(absbearing(X,Y,x,y));
            double r = turnTo(angle);
            Ahead(dist * r);
        }

        // if a bearing is not within the -pi to pi range, alters it to provide the shortest angle
        double normaliseBearing(double ang)
        {
            if ((ang > Math.PI))
            {
                ang = (ang - (2 * Math.PI));
            }

            if ((ang
                        < (Math.PI * -1)))
            {
                ang = (ang + (2 * Math.PI));
            }

            return ang;
        }


        /**Turns the shortest angle possible to come to a heading, then returns 
the direction the bot needs to move in.**/
        int turnTo(double angle)
        {
            double ang;
            int dir;
            ang = normaliseBearing(Heading - angle);
            if (ang > 90) {
                ang -= 180;
                dir = -1;
            }
            else if (ang < -90) {
                ang += 180;
                dir = -1;
            }
            else {
                dir = 1;
            }
            TurnLeft(ang);
            return dir;
        }


        // if a heading is not within the 0 to 2pi range, alters it to provide the shortest angle
        double normaliseHeading(double ang)
        {
            if ((ang > (2 * Math.PI)))
            {
                ang = (ang - (2 * Math.PI));
            }

            if ((ang < 0))
            {
                ang = (ang + (2 * Math.PI));
            }

            return ang;
        }

        // returns the distance between two x,y coordinates
        public double getRange(double x1, double y1, double x2, double y2)
        {
            double xo = (x2 - x1);
            double yo = (y2 - y1);
            double h = Math.Sqrt(((xo * xo)
                            + (yo * yo)));
            return h;
        }

        // gets the absolute bearing between to x,y coordinates
        public double absbearing(double x1, double y1, double x2, double y2)
        {
            double xo = (x2 - x1);
            double yo = (y2 - y1);
            double h = getRange(x1, y1, x2, y2);
            if (((xo > 0)
                        && (yo > 0)))
            {
                return Math.Asin((xo / h));
            }

            if (((xo > 0)
                        && (yo < 0)))
            {
                return (Math.PI - Math.Asin((xo / h)));
            }

            if (((xo < 0)
                        && (yo < 0)))
            {
                return (Math.PI + Math.Asin(((xo / h)
                                * -1)));
            }

            if (((xo < 0)
                        && (yo > 0)))
            {
                return ((2 * Math.PI)
                            - Math.Asin(((xo / h)
                                * -1)));
            }

            return 0;
        }
        public override void Run()
        {
            base.Run();
            BodyColor = Color.Green;
            GunColor = Color.DarkRed;
            RadarColor = Color.Black;
            ScanColor = Color.White;
            BulletColor = Color.Red;
            TurnGunRight(360);
            Ahead(100 * movementDirection);

            while (true)
            {
                counter++;
                //Ahead(100);
                //TurnGunRight(360);
                //Back(100);
                //TurnGunRight(360);
                if (ScannReady(360))
                {
                    //myGrid = new Grid<PathNode>(, grid.GetLength(1), 10f, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
                    myGrid = new Grid<PathNode>((int)BattleFieldWidth, (int)BattleFieldHeight, 10, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
                    PathNode begin = new PathNode(myGrid, (int)X, (int)Y);
                    // Calculate the angle to the scanned robot
                    double angle = Utils.ToRadians((Heading + nextVictim.Bearing) % 360);

                    // Calculate the coordinates of the robot
                    scannedX = (int)(X + Math.Sin(angle) * nextVictim.Distance);
                    scannedY = (int)(Y + Math.Cos(angle) * nextVictim.Distance);
                    PathNode end = new PathNode(myGrid, scannedX, scannedY);

                    FindPath(begin, end, myGrid);


                    if (walkPath)
                    {
                        if (path != null && path.Count > 0)
                        {
                            Vector currentPosition = new Vector((float)X, (float)Y);
                            PathNode enemyPosition = myGrid.GetGridObject(path[0].x, path[0].y);
                            Vector enemyVector = new Vector(enemyPosition.x, enemyPosition.y);
                            if (currentPosition != enemyVector)
                            {
                                GoTo(enemyPosition.x, enemyPosition.y);
                            }
                            else
                            {
                                counter = 0;
                                path.RemoveAt(0);
                            }
                        }
                    }
                }             
            }
        }

        private void GoTo(double x, double y)
        {
            /* Transform our coordinates into a vector */
            x -= X;
            y -= Y;

            /* Calculate the angle to the target position */
            double angleToTarget = Math.Atan2(x, y);

            /* Calculate the turn required get there */
            double targetAngle = Utils.NormalRelativeAngle(angleToTarget - Heading);

            /* 
             * The Java Hypot method is a quick way of getting the length
             * of a vector. Which in this case is also the distance between
             * our robot and the target location.
             */
            double distance = Math.Max(x, y);

            /* This is a simple method of performing set front as back */
            double turnAngle = Math.Atan(Math.Tan(targetAngle));
            TurnRight(turnAngle);
            if (targetAngle == turnAngle)
            {
                Ahead(distance);
            }
            else
            {
                Back(distance);
            }
        }

        double Lerp(double firstFloat, double secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        Vector Lerp(Vector firstVector, Vector secondVector, float by)
        {
            double retX = Lerp(firstVector.x, secondVector.x, by);
            double retY = Lerp(firstVector.y, secondVector.y, by);
            return new Vector(retX, retY);
        }

        public bool SearchPathReady()
        {
            return true;
        }

        public bool ScannReady(double radians)
        {
            if (scannCheckTime <= counter)
            {
                TurnGunRight(radians);
                scannReady = false;
                return true;
            }
            else
            {
                return false;
            }
        }


        private void DodgeMovement(ScannedRobotEvent e)
        {
            // Stay at right angles to the opponent
            TurnRight(e.Bearing + 90 -
               30 * movementDirection);

            // If the bot has small energy drop,
            // assume it fired
            double changeInEnergy =
              previousEnergy - e.Energy;
            if (changeInEnergy > 0 &&
                changeInEnergy <= 3)
            {
                // Dodge!
                movementDirection =
                 -movementDirection;
                Ahead((e.Distance / 4 + 25)*movementDirection);
            }
            // When a bot is spotted,
            // sweep the gun and radar
            gunDirection = -gunDirection;
            TurnGunRight(360 * gunDirection);

            // Fire directly at target
            Fire(2);

            // Track the energy level
            previousEnergy = e.Energy;

        }

        public void FindPath(PathNode startNode, PathNode endNode, Grid<PathNode> grid)
        {
            path = aStar.FindPathNodes(startNode, endNode, grid);
            if(path != null)
            {
                walkPath = true;
            }
        }

        public override void OnScannedRobot(ScannedRobotEvent evnt)
        {
            base.OnScannedRobot(evnt);

            /*
             * Other ways:
             * Bereken haakse hoek op richting van de inkomende kogel en beweeg die kant op
             * /

            //Good to finish players off!
            if (!enemyData.ContainsKey(evnt.Name))
            {
                enemyData.Add(evnt.Name, evnt);
            }

            //Find the weakest robot ( least amount of energy )
            double minEnergy = Double.MaxValue;
            foreach (ScannedRobotEvent sre in enemyData.Values)
            {
                if (sre.Energy < minEnergy)
                {
                    minEnergy = sre.Energy;
                    //now we know who is the weakest player to finish
                    nextVictim = sre;
                }
            }

            if(enemyData.Count == 0 || nextVictim == null)
            {
                nextVictim = evnt;
                TurnGunRight(360);
            }

            if (!evnt.Equals(nextVictim)) nextVictim = evnt;

            //Close range Verry strong
            double absoluteBearing = Heading + nextVictim.Bearing;
            TurnGunRight(Utils.ToDegrees(Utils.NormalRelativeAngle(Utils.ToRadians(absoluteBearing - GunHeading))));
            double victimOffset = nextVictim.Distance;
            if(victimOffset <= enemyCloseRange)
            { 
                Fire(3);
            }
            else if(victimOffset <= enemyMediumRange && victimOffset > enemyCloseRange)
            {
                Fire(2);
            }
            else if(victimOffset <= enemyFarRange && victimOffset > enemyMediumRange)
            {
                Fire(1);
            }
            else
            {
                //Enemy to far away, don't waste bullets just scann again
                TurnGunRight(360);
            }
            //nextVictim = evnt;
            //Store the incoming enemy name 
            /*
            if (!enemyData.ContainsKey(evnt.Name))
            {
                enemyData.Add(evnt.Name, evnt);
            }

            //Remove old enemy data that is more than 10 turns old
            foreach (ScannedRobotEvent sre in enemyData.Values)
            {
                if (sre.Time <= Time - 10)
                {
                    //if(enemyData.ContainsKey(sre.Name))
                        //enemyData.Remove(sre.Name);
                }
            }

            //Find the weakest robot ( least amount of energy )
            double minEnergy = Double.MaxValue;
            foreach (ScannedRobotEvent sre in enemyData.Values)
            {
                if (sre.Energy < minEnergy)
                {
                    minEnergy = sre.Energy;
                    nextVictim = sre;
                }
            }

            if (evnt.Equals(nextVictim))
            {
                scannReady = true;
                //When the scanned enemy is the weakest, target it and we fire
                absoluteBearing = Heading + evnt.Bearing;
                TurnGunRight(Utils.ToDegrees(Utils.NormalRelativeAngle(Utils.ToRadians(absoluteBearing - GunHeading))));
                Fire(1);
            }
            else
            {
                Fire(1);

                TurnGunRight(360);
            }

            if (peek)
            {
                Scan();
            }*/
        }

        public override void OnHitByBullet(HitByBulletEvent evnt)
        {
            base.OnHitByBullet(evnt);
            TurnLeft(90 - evnt.Bearing);
        }
    }
}
