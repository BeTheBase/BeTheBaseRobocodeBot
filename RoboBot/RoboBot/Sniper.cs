using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Robocode;
using Robocode.Util;
using static System.Math;

namespace BCDK
{
    public class Sniper : AdvancedRobot
    {
        double DISTANCE_TO_ENEMY = 150;
        double DELTA_DISTANCE = 50;
        Map map = new Map();


        public override void Run()
        {
            SetColors(Color.Black, Color.Black, Color.Green, Color.Green, Color.Red);
            SetTurnRadarRightRadians(360);
            while (true)
            {
                Point currentPos = new Point((int)X, (int)Y);
                Target target = map.getNearestTarget(currentPos);
                if ((target != null))
                {
                    //  Determine moving.
                    double distanceToEnemy = Geometry.distanceBetween(target.lastCoords(), currentPos);
                    double bearingToEnemy = Geometry.getBearing(currentPos, target.lastCoords());
                    if (((distanceToEnemy
                                > (DISTANCE_TO_ENEMY + DELTA_DISTANCE))
                                || (distanceToEnemy < DISTANCE_TO_ENEMY)))
                    {
                        //  We have 2 points to move to: to the left side of enemy
                        //  and to the right side of him. We have to determine which
                        //  point to use.
                        Point leftPoint = Geometry.movePointByVector(target.lastCoords(), DISTANCE_TO_ENEMY, (bearingToEnemy
                                        - (PI / 2)));
                        Point rightPoint = Geometry.movePointByVector(target.lastCoords(), DISTANCE_TO_ENEMY, (bearingToEnemy
                                        + (PI / 2)));
                        Point pointToMove;
                        if (isPointOnTheBattlefield(leftPoint))
                        {
                            pointToMove = leftPoint;
                        }
                        else
                        {
                            pointToMove = rightPoint;
                        }

                        setMoveToPoint(pointToMove);
                    }
                    else
                    {
                        //  Cycling maneuver.
                        //  Again, we have 2 possible bearings to turn to: clockwise
                        //  or counter-clockwise.
                        double bearingCW = Geometry.normalizeAngle((bearingToEnemy
                                        - (HeadingRadians
                                        - (PI / 2))));
                        double bearingCCW = Geometry.normalizeAngle(((bearingToEnemy - HeadingRadians)
                                        + (PI / 2)));
                        if ((Math.Abs(bearingCW) < Math.Abs(bearingCCW)))
                        {
                            TurnRightRadians(bearingCW);
                        }
                        else
                        {
                            TurnRightRadians(bearingCCW);
                        }

                        //  Now determine turning speed for moving inside a circle
                        //  with radius = DISTANCE_TO_ENEMY.
                        double circleLength = (2
                                    * (PI * DISTANCE_TO_ENEMY));
                        double cycleTime = (circleLength / Rules.MAX_VELOCITY);
                        MaxTurnRate=(Geometry.radiansToDegrees((2
                                            * (PI / cycleTime))));
                        Ahead(circleLength);
                    }

                    double turnByAngle = Geometry.normalizeAngle((bearingToEnemy - GunHeadingRadians));
                    double bulletPower = firePower(target);
                    double bulletSpeed = Rules.GetBulletSpeed(bulletPower);
                    //  Enemy position modelling.
                    double bulletRadius = 0;
                    //  distance travelled by our bullet
                    double imaginaryDistanceToEnemy = distanceToEnemy;
                    for (long time = 0; ((bulletRadius < imaginaryDistanceToEnemy)
                                || (turnByAngle
                                > (Rules.GUN_TURN_RATE * time))); time++)
                    {
                        bulletRadius = (bulletRadius + bulletSpeed);
                        Point targetingPos = target.estimatePositionAt((Time + time));
                        imaginaryDistanceToEnemy = Geometry.distanceBetween(targetingPos, currentPos);
                        double imaginaryBearingToEnemy = Geometry.getBearing(currentPos, targetingPos);
                        turnByAngle = Geometry.normalizeAngle((imaginaryBearingToEnemy - GunHeadingRadians));
                        if (!isPointOnTheBattlefield(targetingPos))
                        {
                            break;
                        }

                    }

                    TurnGunRightRadians(turnByAngle);
                    if (((GunHeat == 0)
                                && (Math.Abs(GunTurnRemainingRadians)
                                < (Height / (2 * imaginaryDistanceToEnemy)))))
                    {
                        Fire(bulletPower);
                        //  TODO: Properly determine bullet coordinates.
                        Bullet bullet = new Bullet(currentPos, GunHeadingRadians, bulletPower, Time);
                        map.addBullet(bullet);
                    }
                    else
                    {
                        //doNothing();
                    }

                }
                else
                {
                    //doNothing();
                }
                Execute();
            }

        }

        private bool isPointOnTheBattlefield(Point point)
        {
            return ((point.X >= 0)
                        && ((point.X <= BattleFieldWidth)
                        && ((point.Y >= 0)
                        && (point.Y <= BattleFieldHeight))));
        }

        private double firePower(Target t)
        {
            Point currentPos = new Point((int)X, (int)Y);
            double distance = Geometry.distanceBetween(t.lastCoords(), currentPos);
            double power;
            if ((distance
                        <= (DISTANCE_TO_ENEMY + DELTA_DISTANCE)))
            {
                power = Rules.MAX_BULLET_POWER;
            }
            else
            {
                power = ((DISTANCE_TO_ENEMY + DELTA_DISTANCE)
                            / (distance * Rules.MAX_BULLET_POWER));
            }

            return Math.Max(power, 0.5);
        }

        private void setMoveToPoint(Point p)
        {
            Point currentPos = new Point((int)X, (int)Y);
            double distanceToPoint = Geometry.distanceBetween(p, currentPos);
            double relativeBearingToPoint = Geometry.normalizeAngle((Geometry.getBearing(currentPos, p) - HeadingRadians));
            //  Set this to max because it might be set to lesser value by other
            //  methods:
            MaxTurnRate=Rules.MAX_TURN_RATE;
            TurnRightRadians(relativeBearingToPoint);
            Ahead(distanceToPoint);
        }

        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            map.setTarget(this, e);
        }

        public override void OnRobotDeath(RobotDeathEvent e)
        {
            map.removeTarget(e.Name);
        }

        /*
        public override void OnPaint(IGraphics g)
        {
            Point currentPos = new Point((int)X, (int)Y);
            Target currentTarget = map.getNearestTarget(currentPos);
            if ((currentTarget != null))
            {
                Point targetPos = currentTarget.estimatePositionAt(Time);
                g.setColor(Color.Green);
                g.DrawPolygon(((int)((targetPos.x
                                - (DISTANCE_TO_ENEMY - DELTA_DISTANCE)))), ((int)((targetPos.y
                                - (DISTANCE_TO_ENEMY - DELTA_DISTANCE)))), (((int)((DISTANCE_TO_ENEMY - DELTA_DISTANCE))) * 2), (((int)((DISTANCE_TO_ENEMY - DELTA_DISTANCE))) * 2));
                g.drawOval(((int)((targetPos.x
                                - (DISTANCE_TO_ENEMY + DELTA_DISTANCE)))), ((int)((targetPos.y
                                - (DISTANCE_TO_ENEMY + DELTA_DISTANCE)))), (((int)((DISTANCE_TO_ENEMY + DELTA_DISTANCE))) * 2), (((int)((DISTANCE_TO_ENEMY + DELTA_DISTANCE))) * 2));
            }

            foreach (Target target in map.targets)
            {
                //  Paint last seen position of target:
                g.setColor(Color.orange);
                g.drawOval(((int)((target.lastCoords().x - 25))), ((int)((target.lastCoords().y - 25))), 50, 50);
                //  Paint estimated position of target:
                Point p = target.estimatePositionAt(getTime());
                g.setColor(Color.blue);
                g.drawOval(((int)((p.x - 25))), ((int)((p.y - 25))), 50, 50);
            }

        }*/

        public override void OnHitWall(HitWallEvent e)
        {
            MaxTurnRate=Rules.MAX_TURN_RATE;
            double turning = ((PI / 2)
                        - Math.Abs(e.BearingRadians));
            if ((e.BearingRadians > 0))
            {
                TurnRightRadians(turning);
            }
            else
            {
                TurnLeftRadians(turning);
            }

            Back(25);
            //doNothing();
        }
    }
}
