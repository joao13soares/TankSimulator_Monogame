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
    enum State
    {
        Thrown,
        HitObject,
        Exploding,
        Dead
    }

    class ClsCannonBall : ICollider
    {
        GraphicsDevice device;

        Model cannonBallModel;
        float modelScale = 0.1f;

        // Keeps all transforms
        Matrix[] boneTransforms;

        Vector3 initialPosition;
        Vector3 position;
        Vector3 initialVelocity;
        Vector3 velocity;
        float gravityAcceleration = 9.8f;
        float creationTime;
        float lifeTime;
        public State state = State.Thrown;

        ICollider cannonBallCollider;

        ClsExplosionGenerator explosionGenerator;

        public ClsCannonBall(GraphicsDevice device, GameTime gt, Model cannonBallModel, Vector3 initialPosition, Vector3 initialVelocity)
        {
            GameSounds.CannonBallThrown();

            this.device = device;

            this.cannonBallModel = cannonBallModel;

            // Lighting settings
            foreach (ModelMesh mesh in this.cannonBallModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = GameSettings.AmbientLightColor;
                    effect.DiffuseColor = GameSettings.DiffuseColor;
                    effect.DirectionalLight0.Enabled = GameSettings.DirectionalLight0_Enabled;
                    effect.DirectionalLight0.DiffuseColor = GameSettings.DirectionalLight0_DiffuseColor;
                    effect.DirectionalLight0.Direction = GameSettings.DirectionalLight0_Direction;
                    effect.DirectionalLight0.Direction.Normalize();
                }
            }

            // create array to store final bone transforms
            boneTransforms = new Matrix[cannonBallModel.Bones.Count];

            this.initialPosition = initialPosition; // p0
            position = initialPosition; // p = p0
            this.initialVelocity = initialVelocity; // v0
            velocity = initialVelocity; // v = v0
            creationTime = (float)gt.TotalGameTime.TotalSeconds; // t0

            // create collider
            cannonBallCollider = new ClsSphereCollider(device, initialPosition, modelScale, Color.Yellow);
        }

        public void Update(GameTime gt, Game1 game)
        {
            if (state == State.Thrown)
            {
                // Δt - total time (in seconds) that the cannonBall has been "alive"
                lifeTime = (float)gt.TotalGameTime.TotalSeconds - creationTime;

                // gravity effect
                velocity.Y = initialVelocity.Y - 0.2f * gravityAcceleration * lifeTime; // vy = v0y - g*Δt

                // position
                position = initialPosition + velocity * lifeTime; // p = p0 + v0*Δt

                // update collider position
                ClsSphereCollider cannonBallSphereCollider = cannonBallCollider as ClsSphereCollider;
                cannonBallSphereCollider.UpdateColliderPosition(position, Matrix.Identity);
            }
            else if (state == State.HitObject)
            {
                GameSounds.CannonBallHitTank();
                explosionGenerator = new ClsExplosionGenerator(device, gt, position);
                state = State.Exploding;
            }
            else if (state == State.Exploding)
            {
                explosionGenerator.Update(gt, game);

                if (explosionGenerator.explosionGeneratorEmpty)
                    state = State.Dead;
            }
        }

        public void Draw(ICamera camera)
        {
            if (state == State.Thrown)
            {
                // Applies a transformations to ...
                // ... Root
                cannonBallModel.Root.Transform = Matrix.CreateScale(modelScale)
                                                * Matrix.CreateTranslation(position);

                // Applies transforms to bones in a cascade
                cannonBallModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

                // Draw the model
                foreach (ModelMesh mesh in cannonBallModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = boneTransforms[mesh.ParentBone.Index];
                        effect.View = camera.ViewMatrix;
                        effect.Projection = camera.ProjectionMatrix;
                    }
                    mesh.Draw();
                }
            }
            else if(state == State.Exploding)
            {
                explosionGenerator.Draw(device, camera);
            }
        }

        public Vector3 Position { get { return position; } }

        // collider
        public string Name() => "CannonBall";
        public void CollidedWith(ICollider other)
        {
            if (other.Name() == "Tank_playerTwo" || other.Name() == "Tank_playerOne" || other.Name() == "CannonBall")
            {
                state = State.HitObject;
            }
        }
        public bool CheckIfCollidesWith(ICollider other) => cannonBallCollider.CheckIfCollidesWith(other);
        public ICollider GetCollider() => cannonBallCollider;
        public void DrawCollider(ICamera camera) => cannonBallCollider.DrawCollider(camera);
    }
}
