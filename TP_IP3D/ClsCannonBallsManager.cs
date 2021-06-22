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
    class ClsCannonBallsManager
    {
        Game1 game;
        GraphicsDevice device;

        List<ClsCannonBall> cannonBalls;
        Model cannonBallModel;

        float coolDownTimer = 0f;
        bool canShoot = true;

        public ClsCannonBallsManager(Game1 game, GraphicsDevice device, Model cannonBallModel)
        {
            this.game = game;
            this.device = device;

            cannonBalls = new List<ClsCannonBall>();
            this.cannonBallModel = cannonBallModel;
        }

        public void Update(GameTime gt)
        {
            for (int i = 0; i < cannonBalls.Count; i++)
            {
                // if cannonBall is beyond Terrain limits
                if (game.Terrain.CheckIfBeyondTerrainBoundaries(cannonBalls[i].Position))
                {
                    //explosionGenerators.Add(newExplosionGenerator(cannonBalls[i].Position)); <-----------------------
                    game.Colliders.Remove(cannonBalls[i]); // delete(cannonBallCollider)
                    cannonBalls.RemoveAt(i); // delete(cannonBall);
                }
                else
                {
                    // safe to check CalcHeightByInterpolation() function, now that we know the cannonBall is inside Terrain limits
                    float terrainHeight = game.Terrain.CalcHeightByInterpolation(cannonBalls[i].Position.X, cannonBalls[i].Position.Z);

                    // if (cannonBall collided with terrain || hit another object || is exploding)
                    if (cannonBalls[i].Position.Y <= terrainHeight || cannonBalls[i].state == State.HitObject || cannonBalls[i].state == State.Exploding)
                        game.Colliders.Remove(cannonBalls[i]); // delete(cannonBallCollider)
                    // if (cannonBall collided with terrain || is dead)
                    if (cannonBalls[i].Position.Y <= terrainHeight || cannonBalls[i].state == State.Dead)
                        cannonBalls.RemoveAt(i); // delete(cannonBall);
                    else
                        cannonBalls[i].Update(gt, game); // update(cannonBall);
                }
            }

            // avoid spamming cannonBalls
            if (coolDownTimer >= 2.0f)
            {
                canShoot = true;
                coolDownTimer = 0f;
            }
            else
                coolDownTimer += (float)gt.ElapsedGameTime.TotalSeconds;
        }

        public void ShootCannonBall(GameTime gt, Vector3 initialPosition, Vector3 cannonDirection)
        {
            if (canShoot)
            {
                // directional initialVelocity
                cannonDirection.Normalize();
                Vector3 initialVelocity = cannonDirection * 7f;

                // create newCannonBall
                ClsCannonBall newCannonBall = new ClsCannonBall(device, gt, cannonBallModel, initialPosition, initialVelocity);
                cannonBalls.Add(newCannonBall);
                game.Colliders.Add(newCannonBall);

                // avoid spamming cannonBalls (triggers timer)
                canShoot = false;
            }
        }

        public void Draw(ICamera camera)
        {
            foreach (ClsCannonBall cannonBall in cannonBalls)
                cannonBall.Draw(camera);
        }
    }
}
