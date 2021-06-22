using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP_IP3D
{
    enum Mode
    {
        BothTanksPlayerMode,
        BothTanksCPUMode,
        Tank2CPUMode
    }

    class ClsTanksManager
    {
        Game1 game;

        ClsTank tank1, tank2;
        float radius = 20f;
        Mode mode = Mode.Tank2CPUMode;
        float coolDownTimer = 0f;

        public ClsTanksManager(Game1 game, GraphicsDevice device, Model tankModel, Model cannonBallModel)
        {
            this.game = game;

            tank1 = new ClsTank(game, device, tankModel, cannonBallModel, true, new Vector2(50f, 50f), Vector3.Backward);
            tank2 = new ClsTank(game, device, tankModel, cannonBallModel, false, new Vector2(40f, 40f), Vector3.Forward);
            game.Colliders.Add(tank1);
            game.Colliders.Add(tank2);
        }

        public void Update(GameTime gt)
        {
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(GameSettings.BothTanksPlayerMode) && !ks.IsKeyDown(GameSettings.BothTanksCPUMode) && !ks.IsKeyDown(GameSettings.Tank2CPUMode))
                mode = Mode.BothTanksPlayerMode;
            if (!ks.IsKeyDown(GameSettings.BothTanksPlayerMode) && ks.IsKeyDown(GameSettings.BothTanksCPUMode) && !ks.IsKeyDown(GameSettings.Tank2CPUMode))
                mode = Mode.BothTanksCPUMode;
            if (!ks.IsKeyDown(GameSettings.BothTanksPlayerMode) && !ks.IsKeyDown(GameSettings.BothTanksCPUMode) && ks.IsKeyDown(GameSettings.Tank2CPUMode))
                mode = Mode.Tank2CPUMode;

            if (tank1.Health > 0f && tank2.Health > 0f)
            {
                if (mode == Mode.BothTanksPlayerMode)
                {
                    tank1.PlayerTankUpdate(gt);
                    tank2.PlayerTankUpdate(gt);
                }
                else if (mode == Mode.BothTanksCPUMode)
                {
                    CalcTargetPosition(gt, tank1, tank2);
                    tank1.CPUTankUpdate(gt);

                    CalcTargetPosition(gt, tank2, tank1);
                    tank2.CPUTankUpdate(gt);
                }
                else if (mode == Mode.Tank2CPUMode)
                {
                    tank1.PlayerTankUpdate(gt);

                    CalcTargetPosition(gt, tank2, tank1);
                    tank2.CPUTankUpdate(gt);
                }
            }
        }

        private void CalcTargetPosition(GameTime gt, ClsTank seekerTank, ClsTank targetTank)
        {
            float distTanks = (seekerTank.Position - targetTank.Position).LengthSquared();
            if (distTanks < (float)Math.Pow(radius, 2f))
            {
                // SEEK: targetPosition aims for the back of the targetTank
                // (Forward and Backward vectors on tank Rotation Matrix are somehow flipped)
                seekerTank.CPUTargetPosition = targetTank.Position + targetTank.Rotation.Forward * 5f;
                seekerTank.CPUShootTargetPosition = targetTank.Position + targetTank.Rotation.Backward;
                seekerTank.CPUCanShoot = true;
                coolDownTimer = 0;
            }
            else
            {
                seekerTank.CPUCanShoot = false;
                if (coolDownTimer > 3f)
                {
                    // WANDER: targetPosition is a random position in the map
                    seekerTank.CPUTargetPosition = game.Terrain.GetRandomPosition();
                    seekerTank.CPUShootTargetPosition = Vector3.Zero;
                    coolDownTimer = 0;
                }
                else
                    coolDownTimer += (float)gt.ElapsedGameTime.TotalSeconds;
            }
        }

        public void Draw(GraphicsDevice device, ICamera camera)
        {
            tank1.Draw(device, camera);
            tank2.Draw(device, camera);
        }

        public ClsTank Tank1 { get { return tank1; } }
        public ClsTank Tank2 { get { return tank2; } }
    }
}
