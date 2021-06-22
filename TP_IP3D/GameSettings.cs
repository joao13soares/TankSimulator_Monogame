using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TP_IP3D
{
    static class GameSettings
    {
        static Random r = new Random();
        public static float RandomFrom2Numbers(float opt1, float opt2) => (float)r.Next(2) == 0 ? opt1 : opt2;
        public static float RandomFloat(float minimum, float maximum) => (float)r.NextDouble() * (maximum - minimum) + minimum;

        // Lighting Settings
        static Vector3 ambientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
        static Vector3 diffuseColor = new Vector3(0.6f, 0.6f, 0.6f);
        static bool directionalLight0_Enabled = true;
        static Vector3 directionalLight0_DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
        static Vector3 directionalLight0_Direction = new Vector3(1.0f, -1.0f, 1.0f);

        public static Vector3 AmbientLightColor { get { return ambientLightColor; } }
        public static Vector3 DiffuseColor { get { return diffuseColor; } }
        public static bool DirectionalLight0_Enabled { get { return directionalLight0_Enabled; } }
        public static Vector3 DirectionalLight0_DiffuseColor { get { return directionalLight0_DiffuseColor; } }
        public static Vector3 DirectionalLight0_Direction { get { return directionalLight0_Direction; } }

        // Shoot
        static Keys shootP1 = Keys.Space;
        static Keys shootP2 = Keys.O;
        // Cannon
        static Keys cannonUpP1 = Keys.Up;
        static Keys cannonDownP1 = Keys.Down;
        static Keys cannonUpP2 = Keys.T;
        static Keys cannonDownP2 = Keys.G;
        // Turret
        static Keys turretLeftP1 = Keys.Left;
        static Keys turretRightP1 = Keys.Right;
        static Keys turretLeftP2 = Keys.F;
        static Keys turretRightP2 = Keys.H;
        // Wheels
        static Keys wheelsForwardP1 = Keys.W;
        static Keys wheelsBackwardsP1 = Keys.S;
        static Keys wheelsForwardP2 = Keys.I;
        static Keys wheelsBackwardsP2 = Keys.K;
        // Steer
        static Keys steerLeftP1 = Keys.A;
        static Keys steerRightP1 = Keys.D;
        static Keys steerLeftP2 = Keys.J;
        static Keys steerRightP2 = Keys.L;
        // Hatch
        static Keys hatchP1 = Keys.Q;
        static Keys hatchP2 = Keys.U;

        public static Keys Shoot(bool playerOne) => playerOne == true ? shootP1 : shootP2;
        public static Keys CannonUp(bool playerOne) => playerOne == true ? cannonUpP1 : cannonUpP2;
        public static Keys CannonDown(bool playerOne) => playerOne == true ? cannonDownP1 : cannonDownP2;
        public static Keys TurretLeft(bool playerOne) => playerOne == true ? turretLeftP1 : turretLeftP2;
        public static Keys TurretRight(bool playerOne) => playerOne == true ? turretRightP1 : turretRightP2;
        public static Keys WheelsForward(bool playerOne) => playerOne == true ? wheelsForwardP1 : wheelsForwardP2;
        public static Keys WheelsBackwards(bool playerOne) => playerOne == true ? wheelsBackwardsP1 : wheelsBackwardsP2;
        public static Keys SteerLeft(bool playerOne) => playerOne == true ? steerLeftP1 : steerLeftP2;
        public static Keys SteerRight(bool playerOne) => playerOne == true ? steerRightP1 : steerRightP2;
        public static Keys Hatch(bool playerOne) => playerOne == true ? hatchP1 : hatchP2;

        // Other controls
        static Keys cameraGhostMode = Keys.F1;
        static Keys cameraSurfaceFollow = Keys.F2;
        static Keys cameraTankFollowP1 = Keys.F3;
        static Keys cameraTankFollowP2 = Keys.F4;
        static Keys bothTanksPlayerMode = Keys.F10;
        static Keys bothTanksCPUMode = Keys.F11;
        static Keys tank2CPUMode = Keys.F12;
        static Keys axis = Keys.X;
        static Keys normalsLines = Keys.N;
        static Keys colliders = Keys.C;
        static Keys seeHealth = Keys.V;

        public static Keys CameraGhostMode { get { return cameraGhostMode; } }
        public static Keys CameraSurfaceFollow { get { return cameraSurfaceFollow; } }
        public static Keys CameraTankFollow(bool playerOne) => playerOne == true ? cameraTankFollowP1 : cameraTankFollowP2;
        public static Keys BothTanksPlayerMode { get { return bothTanksPlayerMode; } }
        public static Keys BothTanksCPUMode { get { return bothTanksCPUMode; } }
        public static Keys Tank2CPUMode { get { return tank2CPUMode; } }
        public static Keys Axis { get { return axis; } }
        public static Keys NormalsLines { get { return normalsLines; } }
        public static Keys Colliders { get { return colliders; } }
        public static Keys SeeHealth { get { return seeHealth; } }
    }
}
