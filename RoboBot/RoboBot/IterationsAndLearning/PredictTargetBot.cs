using System;
using System.Collections.Generic;
using System.Text;
using Robocode;
using Robocode.Util;

namespace BCDK
{
    class PredictTargetBot : AdvancedRobot
    {
        double turn = 1;
        double targetBearing = -200;
        double targetDistance = -1;
        double targetHeading = 0;
        double targetVelocity = 0;
        double lastTargetHeading = 0;
        double lastTargetVelocity = 0;
        double targetX = 0;
        double targetY = 0;
        double myX;
        double myY;
        double noTargetTurn = 1000;
        double estBulletX = 0;
        double estBulletY = 0;
        double fieldWidth = 0;
        double fieldHeight = 0;
        double scanNoTargetTurn = 0;
        double lastTargetX = 0;
        double lastTargetY = 0;
        bool movingForward = true;
        double hitTimer = 10;
        double turnDirection = 1;
        double radarDirection = 1;


        public override void Run()
        {
            base.Run();
            fieldWidth = BattleFieldWidth;
            fieldHeight = BattleFieldHeight;
            IsAdjustRadarForRobotTurn = true;
            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;
            myX = X;
            myY = Y;

            AddCustomEvent(new Condition("Move", (c) =>
            {
                 return (myX != X || myY != Y);               
            }));

            AddCustomEvent(new Condition("NearWall", (c) =>
            {
                 return (X < 100 || Y < 100
                        ||X > fieldWidth - 100 || Y > fieldHeight - 100);
            }));

            while (true)
            {
                TurnLeft(10000);
                MaxVelocity =5;
                Ahead(10000);
            }

        }

        public double getX()
        {
            return X;
        }

        public double getY()
        {
            return Y;
        }

        public void setTargetXY(double targetBearing, double targetDistance)
        {
            this.targetBearing = targetBearing;
            this.targetDistance = targetDistance;
            double alpha = (90
                        - (this.Heading - targetBearing));
            targetX = ((targetDistance * Math.Cos(Utils.ToRadians(alpha)))
                        + this.X);
            targetY = ((targetDistance * Math.Sin(Utils.ToRadians(alpha)))
                        + this.Y);
            scanNoTargetTurn = noTargetTurn;
        }

        public double getTargetBearing(String type)
        {
            double alpha = Utils.ToDegrees(Math.Atan(((targetY - this.Y)
                                / (targetX - this.X))));
            if ((((targetY - this.Y)
                        < 0)
                        && ((targetX - this.X)
                        < 0)))
            {
                alpha = (180 + alpha);
            }
            else if ((((targetY - this.getY())
                        < 0)
                        && ((targetX - this.getX())
                        > 0)))
            {
                alpha = (360 + alpha);
            }
            else if ((((targetY - this.getY())
                        > 0)
                        && ((targetX - this.getX())
                        < 0)))
            {
                alpha = (180 + alpha);
            }

            double bearing = (90
                        - (this.GunHeading - alpha));
            if (type.Equals("heading"))
            {
                bearing = (90
                            - (this.Heading - alpha));
            }

            
            return bearing;
        }

        public double getCenterBearing()
        {
            double alpha = Utils.ToDegrees(Math.Atan((((fieldHeight / 2)
                                - this.getY())
                                / ((fieldWidth / 2)
                                - this.getX()))));
            if (((((fieldHeight / 2)
                        - this.getY())
                        < 0)
                        && (((fieldWidth / 2)
                        - this.getX())
                        < 0)))
            {
                alpha = (180 + alpha);
            }
            else if (((((fieldHeight / 2)
                        - this.getY())
                        < 0)
                        && (((fieldWidth / 2)
                        - this.getX())
                        > 0)))
            {
                alpha = (360 + alpha);
            }
            else if (((((fieldHeight / 2)
                        - this.getY())
                        > 0)
                        && (((fieldWidth / 2)
                        - this.getX())
                        < 0)))
            {
                alpha = (180 + alpha);
            }

            double bearing = (90
                        - (this.Heading - alpha));

            return bearing;
        }

        public double predictionAngle(double bulletSpeed)
        {
            double angle = 0;
            double bearing = (Heading + targetBearing);
            double tankWidth = this.Width;
            double toTurn = ((Heading - GunHeading)
                        + targetBearing);
            double angularChange = 0;
            double velocityChange = 0;
            if (((scanNoTargetTurn < 3)
                        && (scanNoTargetTurn != 0)))
            {
                angularChange = ((targetHeading - lastTargetHeading)
                            / scanNoTargetTurn);
                velocityChange = ((targetVelocity - lastTargetVelocity)
                            / scanNoTargetTurn);
                double dTravel = (Math.Sqrt((((targetX - lastTargetX)
                                * (targetX - lastTargetX))
                                + ((targetY - lastTargetY)
                                * (targetY - lastTargetY)))) / scanNoTargetTurn);
                angularChange = (angularChange
                            / (dTravel * targetVelocity));
                velocityChange = (velocityChange
                            / (dTravel * targetVelocity));
            }

            double diagonal = Math.Sqrt(((fieldWidth * fieldWidth)
                            + (fieldHeight * fieldHeight)));
            for (angle = -45; (angle < 45); angle++)
            {
                double gunTurn = Math.Abs(((toTurn + angle)
                                % 180));
                double estTargetX = targetX;
                double estTargetY = targetY;
                for (double time = 1; (time
                            < (diagonal / bulletSpeed)); time++)
                {
                    double myAngle = (bearing + angle);
                    estTargetX = (estTargetX
                                + (Math.Sin(Utils.ToRadians((targetHeading
                                        + (time * angularChange))))
                                * (targetVelocity
                                + (time * velocityChange))));
                    estTargetY = (estTargetY
                                + (Math.Cos(Utils.ToRadians((targetHeading
                                        + (time * angularChange))))
                                * (targetVelocity
                                + (time * velocityChange))));
                    estBulletX = (myX
                                + (Math.Sin(Utils.ToRadians(myAngle))
                                * ((time
                                - (gunTurn / Rules.GUN_TURN_RATE))
                                * bulletSpeed)));
                    estBulletY = (myY
                                + (Math.Cos(Utils.ToRadians(myAngle))
                                * ((time
                                - (gunTurn / Rules.GUN_TURN_RATE))
                                * bulletSpeed)));
                    if (((estBulletX < 0)
                                || ((estBulletY < 0)
                                || ((estBulletX > fieldWidth)
                                || (estBulletY > fieldHeight)))))
                    {
                        // TODO: Warning!!! continue If
                    }

                    double estDistance = Math.Sqrt((((estTargetX - estBulletX)
                                    * (estTargetX - estBulletX))
                                    + ((estTargetY - estBulletY)
                                    * (estTargetY - estBulletY))));
                    if ((estDistance < 5))
                    {
                        return angle;
                    }

                }

            }

            return 1000;
        }

        public void shoot()
        {
            myX = this.getX();
            myY = this.getY();
            double angle = predictionAngle(Rules.GetBulletSpeed(2));
            if ((angle != 1000))
            {
                double gunTurn = (((Heading - GunHeading)
                            + (targetBearing + angle))
                            % 360);



                TurnGunRight(gunTurn);
                Fire(2);
            }
            else
            {
                double gunTurn = (((Heading - GunHeading)
                            + targetBearing)
                            % 360);

                TurnGunRight(gunTurn);
            }

        }

        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            lastTargetHeading = targetHeading;
            lastTargetVelocity = targetVelocity;
            lastTargetX = targetX;
            lastTargetY = targetY;
            targetBearing = e.Bearing;
            targetDistance = e.Distance;
            targetHeading = e.Heading;
            targetVelocity = e.Velocity;
            double radarTurn = (((Heading - RadarHeading)
                        + targetBearing)
                        % 360);

            if (radarTurn > 180)
                radarTurn = 179;
            else if (radarTurn < -180)
                radarTurn = -179;

            TurnRadarRight(radarTurn);
            radarDirection = (radarDirection * -1);
            setTargetXY(targetBearing, targetDistance);
            if ((GunHeat == 0))
            {
                shoot();
            }

            noTargetTurn = 0;
            Scan();
        }

        public override void OnHitByBullet(HitByBulletEvent e)
        {
            hitTimer = 0;
        }

        public override void OnHitWall(HitWallEvent e)
        {
            reverseDirection();
        }

        public override void OnHitRobot(HitRobotEvent e)
        {
            reverseDirection();
        }

        public override void OnCustomEvent(CustomEvent e)
        {
            if (e.Condition.Name.Equals("Move"))
            {
                double bearing = getTargetBearing("heading");
                hitTimer++;
                if ((targetDistance > 400))
                {
                    if (((bearing <= 45)
                                && ((bearing >= -45)
                                && (hitTimer >= 10))))
                    {
                        MaxVelocity=8;
                    }
                    else if ((hitTimer >= 10))
                    {
                        MaxVelocity=5;
                    }

                }
                else if (((hitTimer >= 10)
                            && ((hitTimer % 4)
                            == 0)))
                {
                    bearing = ((bearing
                                - (turnDirection * 90))
                                % 360);

                    TurnRight(bearing);
                    Ahead(100000);
                    MaxVelocity = 8;
                }
                else if ((((turnDirection == 1)
                            && ((bearing <= 135)
                            && (bearing >= 45)))
                            || (((turnDirection == -1)
                            && ((bearing <= -45)
                            && (bearing >= -35)))
                            && (hitTimer >= 10))))
                {
                    MaxVelocity = 8;
                    MaxTurnRate = 2;
                }
                else if ((hitTimer >= 10))
                {
                    MaxVelocity = 5;
                    MaxTurnRate = 10;
                }

                if ((hitTimer == 1))
                {
                    turnDirection = ((1 * turnDirection)
                                * -1);
                    turn = 1;
                    TurnRight(180);
                    MaxVelocity = 8;
                }

                noTargetTurn++;
                if (((noTargetTurn > 6)
                            && ((noTargetTurn % 7)
                            == 0)))
                {
                    TurnRadarRight(360);
                    noTargetTurn = 0;
                }

            }
            else if (e.Condition.Name.Equals("NearWall"))
            {
                double bearing = getCenterBearing();
                MaxTurnRate=Rules.MAX_TURN_RATE;
                TurnRight(bearing);
                MaxVelocity=8;
                Ahead(40000);
                hitTimer = 1;
            }

        }

        public void reverseDirection()
        {
            if (movingForward)
            {
                MaxVelocity=6;
                MaxTurnRate=Rules.MAX_TURN_RATE;
                Back(40000);
                movingForward = false;
            }
            else
            {
                MaxVelocity = 6;
                MaxTurnRate = Rules.MAX_TURN_RATE;
                Ahead(40000);
                movingForward = true;
            }

        }
        /*
        public override void OnPaint(IGraphics g)
        {
            base.OnPaint(g);
            
            g.DrawLine(((int)(myX)), ((int)(myY)), ((int)(estBulletX)), ((int)(estBulletY)));
            g.fillOval((((int)(estBulletX)) - 3), (((int)(estBulletY)) - 3), 6, 6);
        }*/
    }
}
