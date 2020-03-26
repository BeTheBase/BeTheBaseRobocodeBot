using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Robocode;
using Robocode.Util;

namespace BCDK
{
    public class Map
    {
        List<Target> targets;
        List<Bullet> bullets = new List<Bullet>();
        public Map()
        {
            targets = new List<Target>();
        }


        public void setTarget(Robot robot, ScannedRobotEvent e)
        {
            string targetName=e.Name;
            TargetPosition targetPosition = new TargetPosition(robot, e);
            for (int i = 0; (i < targets.Count); i++)
            {
                Target target = targets[i];
                if (target.name.Equals(targetName))
                {
                    target.addPosition(targetPosition);
                    return;
                }
            }
            targets.Add(new Target(targetName, targetPosition));
        }

        public void addBullet(Bullet bullet)
        {
            bullets.Add(bullet);
        }

        public Target getNearestTarget(Point coords)
        {
            if (targets.Count<1)
            {
                return null;
            }

            double min_distance = Geometry.distanceBetween(coords, targets[0].lastCoords());
            int index = 0;
            for (int i = 1; (i < targets.Count); i++)
            {
                double distance = Geometry.distanceBetween(coords, targets[i].lastCoords());
                if ((distance < min_distance))
                {
                    min_distance = distance;
                    index = i;
                }

            }

            return targets[index];
        }

        public void removeTarget(String targetName)
        {
            for (int i = 0; (i < targets.Count); i++)
            {
                if (targets[i].name.Equals(targetName))
                {
                    targets.RemoveAt(i);
                    return;
                }

            }

        }
    }
}