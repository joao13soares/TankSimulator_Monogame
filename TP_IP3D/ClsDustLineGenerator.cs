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
    class ClsDustLineGenerator
    {
        BasicEffect effect;
        GraphicsDevice device;

        // Vector3 lineCenter;
        Vector3 lineDirection;
        Vector3 lineNormal;
        float lineWidth;

        int dustParticlesFrequency = 3;
        List<ClsDustParticle> dustParticles;

        bool stopAddingParticles = false;

        public ClsDustLineGenerator(GraphicsDevice device, float lineWidth)
        {
            this.device = device;
            effect = new BasicEffect(device);

            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = false;

            this.lineWidth = lineWidth;

            dustParticles = new List<ClsDustParticle>();
        }

        public void Update(Vector3 lineCenter, Matrix tankRotation, GameTime gt, Game1 game)
        {
            // Generate newDustParticle *(times) dustParticlesFrequency
            for (int i = 0; i < dustParticlesFrequency; i++)
            {
                // random initialPosition in the dust line generator
                float distToCenter = GameSettings.RandomFloat(-lineWidth / 2, lineWidth / 2); // -lineWidth / 2 < distToCenter < lineWidth / 2
                lineDirection = tankRotation.Right;
                Vector3 initalPosition = lineCenter + distToCenter * lineDirection;

                // random directional initialVelocity
                lineNormal = -tankRotation.Backward;
                float yaw = GameSettings.RandomFloat(-0.5f, 0.5f) * MathHelper.ToRadians(45.0f); // -45.0 / 2 < yaw < 45.0 / 2
                float pitch = -GameSettings.RandomFloat(0.5f, 1.0f) * MathHelper.ToRadians(50.0f); // 25.0f < pitch < 50.0
                Matrix rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0.0f);
                Vector3 initialVelocity = Vector3.Transform(lineNormal, rotation) * GameSettings.RandomFloat(1.5f, 2.0f);

                // create newDustParticle
                ClsDustParticle newDustParticle = new ClsDustParticle(device, gt, initalPosition, initialVelocity, Color.SaddleBrown);

                // add newDustParticle to the List, if this generator is still allowed/active
                if(!stopAddingParticles)
                    dustParticles.Add(newDustParticle);
            }

            // For each dustParticle
            for (int i = 0; i < dustParticles.Count; i++)
            {
                // if bottom vertex is beyond Terrain limits
                if (game.Terrain.CheckIfBeyondTerrainBoundaries(dustParticles[i].ContactPoint))
                    dustParticles.RemoveAt(i); // delete(dustParticle);
                else
                {
                    // safe to check CalcHeightByInterpolation() function, now that we know the particle is inside Terrain limits
                    float terrainHeight = game.Terrain.CalcHeightByInterpolation(dustParticles[i].ContactPoint.X, dustParticles[i].ContactPoint.Z);
                    
                    // if (bottom vertex collided with terrain || its lifetime reached max)
                    if (dustParticles[i].ContactPoint.Y <= terrainHeight || dustParticles[i].IsDead)
                        dustParticles.RemoveAt(i); // delete(dustParticle);
                    else
                        dustParticles[i].Update(gt); // update(dustParticle);
                }
            }
        }

        public void Draw(GraphicsDevice device, ICamera camera)
        {
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            effect.CurrentTechnique.Passes[0].Apply();

            foreach (ClsDustParticle dustParticle in dustParticles)
                dustParticle.Draw();
        }

        public bool StopAddingParticles { get { return stopAddingParticles; } set { stopAddingParticles = true; } }
        public bool DustLineGeneratorEmpty { get { return dustParticles.Count <= 0; } }
    }
}