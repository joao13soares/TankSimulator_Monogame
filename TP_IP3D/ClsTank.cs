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
    class ClsTank : ICollider
    {
        Game1 game;
        GraphicsDevice device;

        bool playerOne;

        Model tankModel;
        float modelScale = 0.005f;

        // Turret, Cannon, Wheels, Steer and Hatch bones
        ModelBone turretBone;
        ModelBone cannonBone;
        ModelBone[] wheelsBones;
        ModelBone rightSteerBone;
        ModelBone leftSteerBone;
        ModelBone hatchBone;

        // Default transforms
        Matrix cannonTransform;
        Matrix turretTransform;
        Matrix[] wheelsTransforms;
        Matrix leftSteerTransform;
        Matrix rightSteerTransform;
        Matrix hatchTransform;

        // Keeps all transforms
        Matrix[] boneTransforms;

        float cannonPitch = -0.075f;
        float turretYaw = 0f;
        float wheelsPitch = 0f;
        float steerYaw = 0f;
        bool hatchOpen = false;
        bool isHatchKeyPressed = false;

        Vector3 position;
        Vector3 directionH;
        Vector3 defaultDirection;
        float tankYaw = 0f;
        float tankYawFactor = 0.01f;
        Matrix rotationH;
        Matrix rotation = Matrix.Identity;
        float velocity = 4f;

        ClsCameraTankFollow cameraTankFollow;

        List<ClsDustLineGenerator>[] dustLineGenerators;
        bool tankMoved = false;
        bool tankMovedLastUpdate = false;

        ICollider tankCollider;

        float health = 100.0f;

        ClsCannonBallsManager cannonBallsManager;
        bool isShootKeyPressed = false;

        public Vector3 CPUTargetPosition;
        public Vector3 CPUShootTargetPosition;
        public bool CPUCanShoot = false;

        public ClsTank(Game1 game, GraphicsDevice device, Model tankModel, Model cannonBallModel, bool playerOne, Vector2 initialPos, Vector3 defaultDirection)
        {
            this.game = game;
            this.device = device;
            this.playerOne = playerOne;
            this.tankModel = tankModel;

            // Lighting settings
            foreach (ModelMesh mesh in tankModel.Meshes)
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

            // Read bones
            turretBone = tankModel.Bones["turret_geo"];
            cannonBone = tankModel.Bones["canon_geo"];
            wheelsBones = new ModelBone[4];
            wheelsBones[0] = tankModel.Bones["l_front_wheel_geo"];
            wheelsBones[1] = tankModel.Bones["r_front_wheel_geo"];
            wheelsBones[2] = tankModel.Bones["l_back_wheel_geo"];
            wheelsBones[3] = tankModel.Bones["r_back_wheel_geo"];
            leftSteerBone = tankModel.Bones["l_steer_geo"];
            rightSteerBone = tankModel.Bones["r_steer_geo"];
            hatchBone = tankModel.Bones["hatch_geo"];

            // Read bone default transforms
            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            wheelsTransforms = new Matrix[wheelsBones.Length];
            for (int i = 0; i < wheelsTransforms.Length; i++)
            {
                wheelsTransforms[i] = wheelsBones[i].Transform;
            }
            leftSteerTransform = leftSteerBone.Transform;
            rightSteerTransform = rightSteerBone.Transform;
            hatchTransform = hatchBone.Transform;

            // create array to store final bone transforms
            boneTransforms = new Matrix[tankModel.Bones.Count];

            position = new Vector3(initialPos.X, game.Terrain.CalcHeightByInterpolation(initialPos.X, initialPos.Y), initialPos.Y);
            this.defaultDirection = defaultDirection;

            cameraTankFollow = new ClsCameraTankFollow(device, this);

            dustLineGenerators = new List<ClsDustLineGenerator>[2];     // A list of dustLineGenerators for each back wheel ...
            dustLineGenerators[0] = new List<ClsDustLineGenerator>();   // Each wheel can have multiple "throws"/generations of particles depending if ...
            dustLineGenerators[1] = new List<ClsDustLineGenerator>();   // ... the tank started moving before all the previous generator's particles died

            tankCollider = new ClsSphereCollider(device, position, modelScale * 300, playerOne ? Color.Blue : Color.Red);

            cannonBallsManager = new ClsCannonBallsManager(game, device, cannonBallModel);
        }

        public void PlayerTankUpdate(GameTime gt)
        {
            KeyboardState ks = Keyboard.GetState();

            // Cannon
            if (ks.IsKeyDown(GameSettings.CannonUp(playerOne)) && cannonPitch > -1.0f)
                cannonPitch -= MathHelper.ToRadians(1f);
            if (ks.IsKeyDown(GameSettings.CannonDown(playerOne)) && cannonPitch < -0.075f)
                cannonPitch += MathHelper.ToRadians(1f);
            //Console.WriteLine("cannonPitch: " + cannonPitch);

            // Turret
            if (ks.IsKeyDown(GameSettings.TurretLeft(playerOne)))
                turretYaw += MathHelper.ToRadians(1f);
            if (ks.IsKeyDown(GameSettings.TurretRight(playerOne)))
                turretYaw -= MathHelper.ToRadians(1f);

            // Wheels
            if (ks.IsKeyDown(GameSettings.WheelsForward(playerOne)))
            {
                wheelsPitch += velocity * MathHelper.ToRadians(2f);
                position -= directionH * velocity * (float)gt.ElapsedGameTime.TotalSeconds;
                tankYaw += tankYawFactor * steerYaw;

                tankMoved = true;
            }
            else
                tankMoved = false;
            if (ks.IsKeyDown(GameSettings.WheelsBackwards(playerOne)))
            {
                wheelsPitch -= velocity * MathHelper.ToRadians(2f);
                position += directionH * velocity * (float)gt.ElapsedGameTime.TotalSeconds;
                tankYaw -= tankYawFactor * steerYaw;
            }
            //Console.WriteLine(position);

            // Steer
            if (ks.IsKeyDown(GameSettings.SteerLeft(playerOne)) && steerYaw < 0.75f)
                steerYaw += MathHelper.ToRadians(1f);
            if (ks.IsKeyDown(GameSettings.SteerRight(playerOne)) && steerYaw > -0.75f)
                steerYaw -= MathHelper.ToRadians(1f);
            // Steer automatically reset to 0
            if (!ks.IsKeyDown(GameSettings.SteerLeft(playerOne)) && steerYaw > 0)
            {
                steerYaw -= MathHelper.ToRadians(0.5f);
                if (steerYaw < 0)
                    steerYaw = 0;
            }
            if (!ks.IsKeyDown(GameSettings.SteerRight(playerOne)) && steerYaw < 0)
            {
                steerYaw += MathHelper.ToRadians(0.5f);
                if (steerYaw > 0)
                    steerYaw = 0;
            }
            //Console.WriteLine("steerYaw: " + steerYaw);

            // Hatch
            if (ks.IsKeyDown(GameSettings.Hatch(playerOne)))
                isHatchKeyPressed = true;
            if (ks.IsKeyUp(GameSettings.Hatch(playerOne)) && isHatchKeyPressed)
            {
                isHatchKeyPressed = false;
                hatchOpen = !hatchOpen;
            }

            // shoot cannonBall
            if (ks.IsKeyDown(GameSettings.Shoot(playerOne)))
                isShootKeyPressed = true;
            if (ks.IsKeyUp(GameSettings.Shoot(playerOne)) && isShootKeyPressed)
            {
                isShootKeyPressed = false;
                Vector3 cannonDirection = boneTransforms[cannonBone.Index].Backward;
                cannonDirection.Normalize();
                Vector3 cannonTipPosition = boneTransforms[cannonBone.Index].Translation + cannonDirection;

                cannonBallsManager.ShootCannonBall(gt, cannonTipPosition, cannonDirection);
            }

            GeneralUpdates(gt);
        }

        public void CPUTankUpdate(GameTime gt)
        {
            // define boneTransoforms Matrix before using it for using bones positions
            tankModel.Root.Transform = Matrix.CreateScale(modelScale)
                                        * rotation
                                        * Matrix.CreateTranslation(position);
            tankModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // Turret
            Vector3 directionToShootTarget = position - CPUShootTargetPosition; directionToShootTarget.Normalize();
            Vector3 currentTurretDirection = boneTransforms[turretBone.Index].Forward; currentTurretDirection.Normalize();
            // check if cross product between vectors generates an upwards vector, meaning a positive angle and vice versa
            float turretYawSignal = Vector3.Cross(currentTurretDirection, directionToShootTarget).Y > 0 ? 1 : -1;
            // increase/decrease turretYaw between the two vectors using the calculated signal and a constant value
            turretYaw += turretYawSignal * 0.01f;

            // Wheels (with half the normal velocity)
            wheelsPitch += velocity/2f * MathHelper.ToRadians(2f);
            position -= directionH * velocity / 2f * (float)gt.ElapsedGameTime.TotalSeconds;
            tankYaw += tankYawFactor * steerYaw;
            tankMoved = true;

            // Steer
            Vector3 directionToTarget = position - CPUTargetPosition; directionToTarget.Normalize();
            Vector3 currentDirection = rotation.Forward;
            // check if cross product between vectors generates an upwards vector, meaning a positive angle and vice versa
            float steerYawSignal = Vector3.Cross(currentDirection, directionToTarget).Y > 0 ? 1 : -1;
            // calc steerYaw using the calculated signal and the dot product
            steerYaw = steerYawSignal * (float)Math.Acos(Vector3.Dot(currentDirection, directionToTarget));
            // keep steerYaw between limits
            if (steerYaw < -0.75f) steerYaw = -0.75f;
            if (steerYaw > 0.75f) steerYaw = 0.75f;
            //Console.WriteLine("steerYaw = " + MathHelper.ToDegrees(steerYaw));

            // shoot cannonBall
            if (CPUCanShoot)
            {
                Vector3 cannonDirection = boneTransforms[cannonBone.Index].Backward;
                cannonDirection.Normalize();
                Vector3 cannonTipPosition = boneTransforms[cannonBone.Index].Translation + cannonDirection;
                cannonBallsManager.ShootCannonBall(gt, cannonTipPosition, cannonDirection);
            }

            GeneralUpdates(gt);
        }

        private void GeneralUpdates(GameTime gt)
        {
            // evitar exception quando, na interpolação de alturas e de normais, calcula coordenadas que estão para além dos limites do mapa
            position = game.Terrain.CorrectPosition(position);
            position.Y = game.Terrain.CalcHeightByInterpolation(position.X, position.Z);

            // calc rotation of the root of the tank model
            rotationH = Matrix.CreateFromYawPitchRoll(tankYaw, 0f, 0f);
            directionH = Vector3.Transform(defaultDirection, rotationH);
            Vector3 normal = game.Terrain.CalcNormalByInterpolation(position.X, position.Z); normal.Normalize();
            Vector3 right = Vector3.Cross(directionH, normal); right.Normalize();
            Vector3 dir = Vector3.Cross(normal, right); dir.Normalize();
            rotation.Up = normal;
            rotation.Right = right;
            rotation.Forward = dir;

            // update dustLineGenerators
            UpdateDustLineGenerators(gt);

            // update collider position
            ClsSphereCollider tankSphereCollider = tankCollider as ClsSphereCollider;
            tankSphereCollider.UpdateColliderPosition(position + Vector3.UnitY * modelScale * 100, rotation);

            // update cannonBallsManager
            cannonBallsManager.Update(gt);
        }

        private void UpdateDustLineGenerators(GameTime gt)
        {
            // approximated center position for each wheel (dust generators)
            Vector3[] center = { boneTransforms[wheelsBones[2].Index].Translation + new Vector3(110f, -75f, 0f) * modelScale,
                                 boneTransforms[wheelsBones[3].Index].Translation + new Vector3(-65f, -75f, 0f) * modelScale };

            // for each wheel
            for (int i = 0; i < dustLineGenerators.Length; i++)
            {
                // for each dust generator on each wheel
                for (int j = 0; j < dustLineGenerators[i].Count; j++)
                {
                    dustLineGenerators[i][j].Update(center[i], rotation, gt, game); // Update dust generators regardless (ClsDustLineGenerator.cs has ...
                                                                                // ... already a "stopAddingParticles" test in its own Update method

                    // if dust generator has stopped adding particles && it has no more particles to process
                    if (dustLineGenerators[i][j].StopAddingParticles && dustLineGenerators[i][j].DustLineGeneratorEmpty)
                        dustLineGenerators[i].RemoveAt(j); // remove dust generator
                }

                // if tank moved on this Update ...
                if (tankMoved)
                {
                    // ... AND didn't move in the last Update ...
                    if (!tankMovedLastUpdate)
                    {
                        // ... then, it means it was stopped and started moving again.
                        // So, add a new generator to each wheel
                        float dustLineWidth = 100 * modelScale;
                        ClsDustLineGenerator newDustLineGenerator = new ClsDustLineGenerator(device, dustLineWidth);
                        dustLineGenerators[i].Add(newDustLineGenerator);
                    }
                }
                else
                {
                    // stop all current generators from adding new particles 
                    for (int j = 0; j < dustLineGenerators[i].Count; j++)
                        dustLineGenerators[i][j].StopAddingParticles = true;
                }
                //Console.WriteLine("count " + i + " = " + dustLineGenerators[i].Count);
            }
            //Console.WriteLine("tankMovedLastUpdate " + tankMovedLastUpdate + "\ntankMoved " + tankMoved);
            tankMovedLastUpdate = tankMoved;
        }

        public void Draw(GraphicsDevice device, ICamera camera)
        {
            // Applies a transformations to ...
            // ... Root
            tankModel.Root.Transform = Matrix.CreateScale(modelScale)
                                        * rotation
                                        * Matrix.CreateTranslation(position);
            // ... Cannon
            cannonBone.Transform = Matrix.CreateRotationX(cannonPitch) * cannonTransform;
            // ... Turret
            turretBone.Transform = Matrix.CreateRotationY(turretYaw) * turretTransform;
            // ... Wheels
            for (int i = 0; i < wheelsBones.Length; i++)
            {
                wheelsBones[i].Transform = Matrix.CreateRotationX(wheelsPitch) * wheelsTransforms[i];
            }
            // ... Steer
            leftSteerBone.Transform = Matrix.CreateRotationY(steerYaw) * leftSteerTransform;
            rightSteerBone.Transform = Matrix.CreateRotationY(steerYaw) * rightSteerTransform;
            // ... Hatch
            hatchBone.Transform = (hatchOpen ? Matrix.CreateRotationX(-1.5f) : Matrix.CreateRotationX(0.0f)) * hatchTransform;

            // Applies transforms to bones in a cascade
            tankModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // Draw the model
            foreach (ModelMesh mesh in tankModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                }
                mesh.Draw();
            }

            // draw list of dustLineGenerators from both wheels
            for (int i = 0; i < dustLineGenerators.Length; i++)
                foreach (ClsDustLineGenerator dustLineGenerator in dustLineGenerators[i])
                    dustLineGenerator.Draw(device, camera);

            // draw cannonBalls
            cannonBallsManager.Draw(camera);
        }

        public ClsCameraTankFollow CameraTankFollow { get { return cameraTankFollow; } }
        public Vector3 Position { get { return position; } }
        public Vector3 DirectionH { get { return directionH; } }
        public Matrix Rotation { get { return rotation; } }
        public float Health { get { return health; } }

        // collider
        public string Name() => playerOne ? "Tank_playerOne" : "Tank_playerTwo";
        public void CollidedWith(ICollider other)
        {
            if ((playerOne && other.Name() == "Tank_playerTwo") || (!playerOne && other.Name() == "Tank_playerOne"))
            {
                //Console.WriteLine("Collision: " + Name() + " with " + other.Name());

                ClsSphereCollider tankSphereCollider = tankCollider as ClsSphereCollider;
                ClsSphereCollider otherTankSphereCollider = other.GetCollider() as ClsSphereCollider;

                // directional vector to bump into the other tank
                Vector3 dir = otherTankSphereCollider.Center - tankSphereCollider.Center;
                dir.Normalize();

                // applies 5% of the normalized direction so the tanks don't seem like teleporting
                position -= dir * 0.05f;
            }
            if (other.Name() == "CannonBall" && health > 0f)
            {
                health -= 10.0f;
                Console.WriteLine(Name() + "'s health = " + health);
            }
        }
        public bool CheckIfCollidesWith(ICollider other) => tankCollider.CheckIfCollidesWith(other);
        public ICollider GetCollider() => tankCollider;
        public void DrawCollider(ICamera camera) => tankCollider.DrawCollider(camera);
    }
}
