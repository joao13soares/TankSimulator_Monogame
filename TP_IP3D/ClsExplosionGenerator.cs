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
    class ClsExplosionGenerator
    {
        BasicEffect effect;
        GraphicsDevice device;

        Vector3 explosionOrigin;

        int numExplosionParticles = 20;
        List<ClsDustParticle> explosionParticles;

        public ClsExplosionGenerator(GraphicsDevice device, GameTime gt, Vector3 explosionOrigin)
        {
            this.device = device;
            effect = new BasicEffect(device);

            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = false;

            this.explosionOrigin = explosionOrigin;
            explosionParticles = new List<ClsDustParticle>();

            // Generate newExplosionParticle *(times) numExplosionParticles
            for (int i = 0; i < numExplosionParticles; i++)
            {
                // random directional initialVelocity
                float yaw = GameSettings.RandomFloat(-1.0f, 1.0f) * MathHelper.ToRadians(180.0f); // -180.0 < yaw < 180.0
                float pitch = GameSettings.RandomFloat(-1.0f, 1.0f) * MathHelper.ToRadians(45.0f); // -45.0 < pitch < 45.0
                Matrix rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0.0f);
                Vector3 initialVelocity = Vector3.Transform(Vector3.Up, rotation) * GameSettings.RandomFloat(1.5f, 2.0f);

                // create newExplosionParticle
                ClsDustParticle newExplosionParticle = new ClsDustParticle(device, gt, explosionOrigin, initialVelocity, Color.Yellow);

                // add newExplosionParticle to the List
                explosionParticles.Add(newExplosionParticle);
            }
        }

        public void Update(GameTime gt, Game1 game)
        {
            // For each explosionParticle
            for (int i = 0; i < explosionParticles.Count; i++)
            {
                // if bottom vertex is beyond Terrain limits
                if (game.Terrain.CheckIfBeyondTerrainBoundaries(explosionParticles[i].ContactPoint))
                    explosionParticles.RemoveAt(i); // delete(explosionParticle);
                else
                {
                    // safe to check CalcHeightByInterpolation() function, now that we know the particle is inside Terrain limits
                    float terrainHeight = game.Terrain.CalcHeightByInterpolation(explosionParticles[i].ContactPoint.X, explosionParticles[i].ContactPoint.Z);

                    // if (bottom vertex collided with terrain || its lifetime reached max)
                    if (explosionParticles[i].ContactPoint.Y <= terrainHeight || explosionParticles[i].IsDead)
                        explosionParticles.RemoveAt(i); // delete(explosionParticle);
                    else
                        explosionParticles[i].Update(gt); // update(explosionParticle);
                }
            }
        }

        public void Draw(GraphicsDevice device, ICamera camera)
        {
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            effect.CurrentTechnique.Passes[0].Apply();

            foreach (ClsDustParticle explosionParticle in explosionParticles)
                explosionParticle.Draw();
        }
        
        public bool explosionGeneratorEmpty { get { return explosionParticles.Count <= 0; } }
    }
}