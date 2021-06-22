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
    class ClsCameraSurfaceFollow : ICamera
    {
        Game1 game;

        Matrix viewMatrix;

        Vector3 position;

        Vector3 _normal;
        Matrix projectionMatrix;
        float _fov;
        float _aspectRatio;
        float _nearPlane;
        float _farPlane;
        float _yaw, _pitch;

        float _movementSpeed = 10.0f;
        float _rotationSpeed = 1.5f;

        float _distToGround = 5.0f;

        Vector2 viewCenter;

        public ClsCameraSurfaceFollow(Game1 game, GraphicsDevice device)
        {
            this.game = game;

            viewCenter = new Vector2(device.Viewport.Width / 2, device.Viewport.Height / 2);

            #region ViewMatrix
            // Camera directionH
            _yaw = 0.0f;
            _pitch = 0.0f;
            Vector3 defaultDirection = new Vector3(0.0f, 0.0f, -1.0f);
            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(_yaw, _pitch, 0.0f);
            Vector3 direction = Vector3.Transform(defaultDirection, cameraRotation);

            position = new Vector3(0.0f, _distToGround * game.Terrain.CalcHeightByInterpolation(0.0f, 0.0f), 0.0f);
            Vector3 target = position + direction;
            _normal = Vector3.Up;
            viewMatrix = Matrix.CreateLookAt(position, target, _normal);
            #endregion

            #region ProjectionMatrix
            _fov = MathHelper.ToRadians(45.0f);
            _aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            _nearPlane = 0.1f;
            _farPlane = 1000.0f;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(_fov, _aspectRatio, _nearPlane, _farPlane);
            #endregion
        }

        public void Update(KeyboardState ks, MouseState ms, GameTime gt)
        {
            float delta_X = ms.X - viewCenter.X;
            float delta_Y = ms.Y - viewCenter.Y;

            _yaw -= _rotationSpeed * (delta_X) * MathHelper.ToRadians(2f) * (float)gt.ElapsedGameTime.TotalSeconds;
            _pitch += _rotationSpeed * (delta_Y) * MathHelper.ToRadians(2f) * (float)gt.ElapsedGameTime.TotalSeconds;
            // adjut pitch to avoid camera flip
            if (_pitch < -1.5f) _pitch = -1.5f + Single.Epsilon;
            if (_pitch >  1.5f) _pitch = 1.5f - Single.Epsilon;

            Vector3 defaultDirection = new Vector3(0.0f, 0.0f, -1.0f);
            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(_yaw, _pitch, 0.0f);
            Vector3 direction = Vector3.Transform(defaultDirection, cameraRotation);

            Vector3 novaPos = position;
            Vector3 right = Vector3.Cross(direction, Vector3.Up);
            right.Normalize();
            if (ks.IsKeyDown(Keys.NumPad8))
                novaPos = position + direction * _movementSpeed * (float)gt.ElapsedGameTime.TotalSeconds;
            if (ks.IsKeyDown(Keys.NumPad5))
                novaPos = position - direction * _movementSpeed * (float)gt.ElapsedGameTime.TotalSeconds;
            if (ks.IsKeyDown(Keys.NumPad4))
                novaPos = position - right * _movementSpeed * (float)gt.ElapsedGameTime.TotalSeconds;
            if (ks.IsKeyDown(Keys.NumPad6))
                novaPos = position + right * _movementSpeed * (float)gt.ElapsedGameTime.TotalSeconds;

            // evitar exception quando, na interpolação de alturas, calcula coordenadas que estão para além dos limites do mapa
            novaPos = game.Terrain.CorrectPosition(novaPos);

            //Console.WriteLine("position (" + novaPos.X + ", " + game.GetTerrain().CalcHeightByInterpolation(novaPos.X, novaPos.Z) + ", " + novaPos.Z + ")");
            position = new Vector3(novaPos.X, _distToGround + game.Terrain.CalcHeightByInterpolation(novaPos.X, novaPos.Z), novaPos.Z);

            Vector3 target = position + direction;
            viewMatrix = Matrix.CreateLookAt(position, target, _normal);

            Mouse.SetPosition((int)viewCenter.X, (int)viewCenter.Y);
        }

        public Matrix ViewMatrix { get { return viewMatrix; } }
        public Vector3 Position { get { return position; } }
        public Matrix ProjectionMatrix { get { return projectionMatrix; } }
    }
}
