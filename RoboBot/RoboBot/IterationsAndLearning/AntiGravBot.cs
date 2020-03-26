using System;
using System.Collections.Generic;
using System.Text;
using Robocode;
using System.Linq;

using System.Collections;
using System.Drawing;
using Robocode.Util;

namespace BCDK
{
    public class AntiGravBot : AdvancedRobot
    {
        Hashtable targets;

        // all enemies are stored in the hashtable
        Enemy target;

        // our current enemy
        double PI = Math.PI;

        // just a constant
        int direction = 1;

        // direction we are heading... 1 = forward, -1 = backwards
        double firePower;

        // the power of the shot we will be using
        double midpointstrength = 0;

        // The strength of the gravity point in the middle of the field
        int midpointcount = 0;

        // Number of turns since that strength was changed.
        public override void Run()
        {
            this.targets = new Hashtable();
            this.target = new Enemy();
            this.target.distance = 100000;
            // initialise the distance so that we can select a target
            SetColors(Color.Red, Color.Blue, Color.Green);
            
            // sets the colours of the robot
            // the next two lines mean that the turns of the robot, gun and radar are independant
           // IsAdjustGunForRobotTurn(true);
            //setAdjustRadarForGunTurn(true);
            TurnRadarRightRadians((2 * this.PI));
            // turns the radar right around to get a view of the field
            while (true)
            {
                this.antiGravMove();
                // Move the bot
                this.doFirePower();
                // select the fire power to use
                this.doScanner();
                // Oscillate the scanner over the bot
                this.doGun();
                //println(this.target.distance);
                // move the gun to predict where the enemy will be
               
                Fire(this.firePower);
                Execute();
                // execute all commands
            }

        }

        void doFirePower()
        {
            this.firePower = (400 / this.target.distance);
            // selects a bullet power based on our distance away from the target
            if ((this.firePower > 3))
            {
                this.firePower = 3;
            }

        }

        void antiGravMove()
        {
            double xforce = 0;
            double yforce = 0;
            double force;
            double ang;
            GravPoint p;
            Enemy en;
            // cycle through all the enemies.  If they are alive, they are repulsive.  Calculate the force on us
            if (targets.Count > 0)
            {
                foreach (DictionaryEntry pair in targets)
                {
                    en = ((Enemy)(pair.Value));
                    if (en.live)
                    {
                        p = new GravPoint(en.x, en.y, -1000);
                        force = (p.power / Math.Pow(this.getRange(X, Y, p.x, p.y), 2));
                        // Find the bearing from the point to us
                        ang = this.normaliseBearing(((Math.PI / 2)
                                        - Math.Atan2((Y - p.y), (X - p.x))));
                        // Add the components of this force to the total force in their respective directions
                        xforce = (xforce
                                    + (Math.Sin(ang) * force));
                        yforce = (yforce
                                    + (Math.Cos(ang) * force));
                    }
                }
            }
            /*
            while (targets.Count > 0)
            {
                en = ((Enemy)(e.MoveNext()));
                if (en.live)
                {
                    p = new GravPoint(en.x, en.y, -1000);
                    force = (p.power / Math.Pow(this.getRange(X, Y, p.x, p.y), 2));
                    // Find the bearing from the point to us
                    ang = this.normaliseBearing(((Math.PI / 2)
                                    - Math.Atan2((Y - p.y), (X - p.x))));
                    // Add the components of this force to the total force in their respective directions
                    xforce = (xforce
                                + (Math.Sin(ang) * force));
                    yforce = (yforce
                                + (Math.Cos(ang) * force));
                }

            }*/

            this.midpointcount++;
            if ((this.midpointcount > 5))
            {
                this.midpointcount = 0;
                this.midpointstrength = ((new Random().Next(0,100) * 2000)
                            - 1000);
            }

            p = new GravPoint((BattleFieldWidth / 2), (BattleFieldHeight / 2), this.midpointstrength);
            force = (p.power / Math.Pow(this.getRange(X, Y, p.x, p.y), 1.5));
            ang = this.normaliseBearing(((Math.PI / 2)
                            - Math.Atan2((Y - p.y), (X - p.x))));
            xforce = (xforce
                        + (Math.Sin(ang) * force));
            yforce = (yforce
                        + (Math.Cos(ang) * force));
            xforce = (xforce + (5000 / Math.Pow(this.getRange(X, Y, BattleFieldWidth, Y), 3)));
            xforce = (xforce - (5000 / Math.Pow(this.getRange(X, Y, 0, Y), 3)));
            yforce = (yforce + (5000 / Math.Pow(this.getRange(X, Y, X, BattleFieldHeight), 3)));
            yforce = (yforce - (5000 / Math.Pow(this.getRange(X, Y, X, 0), 3)));
            // Move in the direction of our resolved force.
            this.goTo((X - xforce), (Y - yforce));
        }

        /**Move in the direction of an x and y coordinate**/
        void goTo(double x, double y)
        {
            double dist = 60;
            double angle = Utils.ToDegrees(absbearing(X, Y, x, y));
            double r = turnTo(angle);
            Ahead(dist * r);
        }

        int turnTo(double angle)
        {
            double ang;
            int dir;
            ang = this.normaliseBearing((Heading - angle));
            if ((ang > 90))
            {
                ang -= 180;
                dir = -1;
            }
            else if ((ang < -90))
            {
                ang += 180;
                dir = -1;
            }
            else
            {
                dir = 1;
            }

            TurnLeft(ang);
            return dir;
        }

        void doScanner()
        {
            TurnRadarLeftRadians((2 * this.PI));
        }

        void doGun()
        {
            long time = (Time + ((int)(Math.Round((this.getRange(X, Y, this.target.x, this.target.y) / (20 - (3 * this.firePower)))))));
            Vector p = this.target.guessPosition(time);
            // offsets the gun by the angle to the next shot based on linear targeting provided by the enemy class
            double gunOffset = (GunHeadingRadians
                        - ((Math.PI / 2)
                        - Math.Atan2((p.y - Y), (p.x - X))));
            TurnGunLeftRadians(this.normaliseBearing(gunOffset));
        }

        // if a bearing is not within the -pi to pi range, alters it to provide the shortest angle
        double normaliseBearing(double ang)
        {
            if ((ang > this.PI))
            {
                ang = (ang - (2 * this.PI));
            }

            if ((ang
                        < (this.PI * -1)))
            {
                ang = (ang + (2 * this.PI));
            }

            return ang;
        }

        // if a heading is not within the 0 to 2pi range, alters it to provide the shortest angle
        double normaliseHeading(double ang)
        {
            if ((ang > (2 * this.PI)))
            {
                ang = (ang - (2 * this.PI));
            }

            if ((ang < 0))
            {
                ang = (ang + (2 * this.PI));
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
            double h = this.getRange(x1, y1, x2, y2);
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

        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            Enemy en;
            if (this.targets.ContainsKey(e.Name))
            {
                en = ((Enemy)(this.targets[e.Name]));
            }
            else
            {
                en = new Enemy();
                this.targets.Add(e.Name, en);
            }

            // the next line gets the absolute bearing to the point where the bot is
            double absbearing_rad = ((HeadingRadians + e.BearingRadians) % (2 * this.PI));
            // this section sets all the information about our target
            en.name = e.Name;
            double h = this.normaliseBearing((e.HeadingRadians - en.heading));
            h = (h
                        / (Time - en.ctime));
            en.changehead = h;
            en.x = (X
                        + (Math.Sin(absbearing_rad) * e.Distance));
            // works out the x coordinate of where the target is
            en.y = (Y
                        + (Math.Cos(absbearing_rad) * e.Distance));
            // works out the y coordinate of where the target is
            en.bearing = e.BearingRadians;
            en.heading = e.HeadingRadians;
            en.ctime = Time;
            // game time at which this scan was produced
            en.speed = e.Velocity;
            en.distance = e.Distance;
            en.live = true;
            if (((en.distance < this.target.distance)
                        || (this.target.live == false)))
            {
                this.target = en;
            }

        }

        public override void OnRobotDeath(RobotDeathEvent e)
        {
            Enemy en = ((Enemy)(this.targets[e.Name]));
            en.live = false;
        }
    }
    class Enemy
    {

        public string name;

        public double bearing;

        public double heading;

        public double speed;

        public double x;

        public double y;

        public double distance;

        public double changehead;

        public long ctime;

        // game time that the scan was produced
        public bool live;

        // is the enemy alive?
        public Vector guessPosition(long when)
        {
            double diff = (when - this.ctime);
            double newY = (this.y
                        + (Math.Cos(this.heading)
                        * (this.speed * diff)));
            double newX = (this.x
                        + (Math.Sin(this.heading)
                        * (this.speed * diff)));
            return new Vector(newX, newY);
        }
    }
    class GravPoint
    {

        public double x;

        public double y;

        public double power;

        public GravPoint(double pX, double pY, double pPower)
        {
            this.x = pX;
            this.y = pY;
            this.power = pPower;
        }
    }
}
