using System;
using System.Collections.Generic;
using System.Text;
using Robocode;

namespace BCDK
{
    public class DodgeBot : AdvancedRobot
    { 
        double previousEnergy = 100;
        int movementDirection = 1;
        int gunDirection = 1;

        public override void Run()
        {
            TurnGunRight(99999);
            Ahead(1000 * movementDirection);
            Execute();
        }

        public override void OnScannedRobot(
          ScannedRobotEvent e)
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
            TurnGunRight(99999 * gunDirection);

            // Fire directly at target
            Fire(2);

            // Track the energy level
            previousEnergy = e.Energy;

            Execute();
        }
    }
}
