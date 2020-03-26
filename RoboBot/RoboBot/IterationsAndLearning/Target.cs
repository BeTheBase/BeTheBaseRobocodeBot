using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BCDK
{
    public class Target
    {
        public string name;
        private List<TargetPosition> positions = new List<TargetPosition>();

        /**
         * Creates a radar target with specified parameters.
         * @param name Name of the target.
         * @param position TargetPosition objects representing the target.
         */
        public Target(string name, TargetPosition position)
        {
            this.name = name;
            positions = new List<TargetPosition>();
            positions.Add(position);
        }

        /**
         * Adds position to target positions list. Deletes the oldest positions if
         * needed.
         * @param position Position to add.
         */
        public void addPosition(TargetPosition position)
        {
            if (positions.Count > 0)
            {
                positions.RemoveAt(0);
            }
            positions.Add(position);
        }

        /**
         * Returns last known coordinates of target.
         * @return Coordinates of last position.
         */
        public Point lastCoords()
        {
            if (positions.Count <1)
            {
                return new Point();
            }

            long lastTime = positions[0].time;
            Point lastCoords = positions[0].coords;
            for (int i = 1; i < positions.Count; ++i)
            {
                TargetPosition position = positions[i];
                if (position.time > lastTime)
                {
                    lastTime = position.time;
                    lastCoords = position.coords;
                }
            }

            return lastCoords;
        }

        /**
         * Calculates estimated target position at some moment of time.
         * @param atTime Time when target position must be calculated.
         * @return Point object with coordinates at given time.
         */
        public Point estimatePositionAt(long atTime)
        {
            if (positions.Count > 0)
            {
                TargetPosition position = positions[0];
                return Geometry.movePointByVector(position.coords, position.velocity *
                        (atTime - position.time), position.heading);
            }
            else
            {
                return new Point();
            }
        }
    }
}
