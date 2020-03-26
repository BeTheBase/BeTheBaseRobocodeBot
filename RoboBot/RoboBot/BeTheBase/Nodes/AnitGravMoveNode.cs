using System;
using System.Collections.Generic;
using System.Text;
using Robocode;
using Robocode.Util;

namespace BCDK
{
    public class AnitGravMoveNode : BaseNode
    {
        private double distance;
        double midPointPower = 0;
        int midPointCount = 0;
        public AnitGravMoveNode(BlackBoard bb, double distance)
        {
            this.bb = bb;
            this.distance = distance;
            //this.gravpoints = inputPoints;
        }

        public override BehaviourTreeStatus Tick()
        {
            if(this.bb.CurrentEnemy == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            List<GravPoint> gravpoints = new List<GravPoint>();

            AdvancedRobot r = this.bb.Robot;

            gravpoints.Add(new GravPoint(this.bb.CurrentEnemy.x, this.bb.CurrentEnemy.y, 1000));

            double changeInEnergy = this.bb.PreviousEnergy - this.bb.CurrentEnemy.E.Energy;

            if (changeInEnergy >= -3 && changeInEnergy <= 3)
            {
                //Detect if enemy energy has changed
                //Now we need to "predict" where the bot will fire its bullets and assign grav points to them
                gravpoints.Add(new GravPoint(r.X + 15, 0, RandomNumber(-1000, 1000)));
                gravpoints.Add(new GravPoint(r.X - 15, 0, RandomNumber(-1000, 1000)));
            }

            double xforce = 0;
            double yforce = 0;
            double force;
            double ang;

            GravPoint p;

            for (int i = 0; i < gravpoints.Count; i++)
            {
                p = (GravPoint)gravpoints[i];
                //Calculate the total force from this point on us
                force = p.power / Math.Pow(GetRange(r.X, r.Y, p.x, p.y), 1.5);
                //Find the bearing from the point to us
                ang =
            NormaliseBearing(Math.PI / 2 - Math.Atan2(r.Y - p.y, r.X - p.x));
                //Add the components of this force to the total force in their 
                //respective directions
                xforce += Math.Sin(ang) * force;
                yforce += Math.Cos(ang) * force;
            }

                /**The next section adds a middle point with a random (positive or negative) strength.
                The strength changes every 5 turns, and goes between -1000 and 1000.  This gives a better
                overall movement.**/
            midPointCount++;
            if(midPointCount > 5)
            {
                midPointCount = 0;
                midPointPower = RandomNumber(-1000, 1000);
            }
            p = new GravPoint(this.bb.Robot.BattleFieldWidth / 2, this.bb.Robot.BattleFieldHeight/2, midPointPower);
            force = p.power / Math.Pow(GetRange(r.X, r.Y, p.x, p.y), 1.5);
            ang = NormaliseBearing(Math.PI / 2 - Math.Atan2(r.Y - p.y, r.X - p.x));
            /*
            double TOP = r.BattleFieldWidth - 18.0;
            double RIGHT = r.BattleFieldHeight - 18.0;
            double BOTTOM = 18.0;
            double LEFT = 18.0;

            double N = 2.0 * Math.PI;
            double E = Math.PI / 2.0;
            double S = Math.PI;
            double W = 3.0 * Math.PI / 2.0;

            double s = 8.0;
            double x = r.X;
            double y = r.Y;
            double a = ang; // whatever angle you wish to travel, we can smooth it!
            bool clockwise = true; // your choice!

            if (clockwise)
            {
                if (S < a)
                { // left wall
                    if (shouldSmooth(a - S, LEFT - x, s))
                    {
                        a = smooth(a - S, LEFT - x, s) + S;
                    }
                }
                else if (a < S)
                { // right wall
                    if (shouldSmooth(a, x - RIGHT, s))
                    {
                        a = smooth(a, x - RIGHT, s);
                    }
                }
                if (W < a || a < E)
                { // top wall
                    if (shouldSmooth(a + E, y - TOP, s))
                    {
                        a = smooth(a + E, y - TOP, s) - E;
                    }
                }
                else if (E < a && a < W)
                { // bottom wall
                    if (shouldSmooth(a - E, BOTTOM - y, s))
                    {
                        a = smooth(a - E, BOTTOM - y, s) + E;
                    }
                }
            }
            else
            {
                if (S < a)
                { // left wall
                    if (shouldSmooth(N - a, LEFT - x, s))
                    {
                        a = N - smooth(N - a, LEFT - x, s);
                    }
                }
                else if (a < S)
                { // right wall
                    if (shouldSmooth(S - a, x - RIGHT, s))
                    {
                        a = S - smooth(S - a, x - RIGHT, s);
                    }
                }
                if (W < a || a < E)
                { // top wall
                    if (shouldSmooth(E - a, y - TOP, s))
                    {
                        a = E - smooth(E - a, y - TOP, s);
                    }
                }
                else if (E < a && a < W)
                { // bottom wall
                    if (shouldSmooth(W - a, BOTTOM - y, s))
                    {
                        a = W - smooth(W - a, BOTTOM - y, s);
                    }
                }
            }*/
            xforce += Math.Sin(ang) * force;
            yforce += Math.Cos(ang) * force;
            /**The following four lines add wall avoidance.  They will only 
            affect us if the bot is close to the walls due to the
            force from the walls decreasing at a power 3.**/
            xforce += 5000 / Math.Pow(GetRange(r.X,r.Y, r.BattleFieldWidth, r.Y), 3);
            xforce -= 5000 / Math.Pow(GetRange(r.X,r.Y, 0, r.Y), 3);
            yforce += 5000 / Math.Pow(GetRange(r.X,r.Y, r.X, r.BattleFieldHeight), 3);
            yforce -= 5000 / Math.Pow(GetRange(r.X,r.Y, r.X, 0), 3);


           

            //Move in the direction of our resolved force.
            GoTo(this.bb.Robot.X - xforce, this.bb.Robot.Y - yforce);

            //if(Math.Abs(r.X) != Math.Abs(xforce) && Math.Abs(r.Y) != Math.Abs(yforce))
            //{
            //    return BehaviourTreeStatus.Running;
            //}
            //else
            return BehaviourTreeStatus.Success;
        }

        // Generate a random number between two numbers
        public int RandomNumber(double min, double max)
        {
            Random random = new Random();
            return random.Next((int)min, (int)max);
        }

        /**Move in the direction of an x and y coordinate**/
        void GoTo(double x, double y)
        {
            double dist = distance;
            double angle = Utils.ToDegrees(AbsBearing(this.bb.Robot.X, this.bb.Robot.Y, x, y));
            double r = TurnTo(angle);
            this.bb.Robot.SetAhead(dist * r);
        }

        double _r = 114.5450131316624;

        private bool shouldSmooth(double a, double x, double s)
        {
            double d = 2 * _r; // turning diameter
            double nextX = x + s * Math.Sin(a);
            if (nextX < -d)
            { // shortcut, unnecessary code
                return false;
            }
            if (0.0 <= nextX)
            { // shortcut, unnecessary code
                return true;
            }
            return 0.0 < nextX + _r * (Math.Cos(a) + 1.0);
        }

        private double smooth(double a, double x, double s)
        {
            double nextX = x + s * Math.Sin(a);
            if (0.0 <= nextX)
            { // NOT a shortcut, necessary code
                return Math.PI;
            }
            return Math.Acos(-nextX / _r - 1.0);
        }

        /**Turns the shortest angle possible to come to a heading, then returns 
the direction the bot needs to move in.**/
        int TurnTo(double angle)
        {
            double ang;
            int dir;
            ang = NormaliseBearing(this.bb.Robot.Heading - angle);
            if (ang > 90)
            {
                ang -= 180;
                dir = -1;
            }
            else if (ang < -90)
            {
                ang += 180;
                dir = -1;
            }
            else
            {
                dir = 1;
            }
            this.bb.Robot.SetTurnLeft(ang);
            return dir;
        }

        // if a bearing is not within the -pi to pi range, alters it to provide the shortest angle
        double NormaliseBearing(double ang)
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
        double GetRange(double x1, double y1, double x2, double y2)
        {
            double x = x2 - x1;
            double y = y2 - y1;
            double range = Math.Sqrt(x * x + y * y);
            return range;
        }

        // gets the absolute bearing between to x,y coordinates
        public double AbsBearing(double x1, double y1, double x2, double y2)
        {
            double xo = (x2 - x1);
            double yo = (y2 - y1);
            double h = GetRange(x1, y1, x2, y2);
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
    }
}
