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
    class ClsCameraGhostMode : ICamera
    {
        Matrix viewMatrix;

        Vector3 position;

        Vector3 _normal;
        Matrix projectionMatrix;
        float _fov;
        float _aspectRatio;
        float _nearPlane;
        float _farPlane;
        float _yaw, _pitch;

        float _movementSpeed = 50.0f;
        float _rotationSpeed = 1.5f;

        Vector2 viewCenter;

        public ClsCameraGhostMode(GraphicsDevice device)
        {
            viewCenter = new Vector2(device.Viewport.Width / 2, device.Viewport.Height / 2);

            #region ViewMatrix
            // Camera directionH
            _yaw = 0.0f;
            _pitch = 0.0f;
            Vector3 defaultDirection = new Vector3(-1.0f, 0.0f, -1.0f);
            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(_yaw, _pitch, 0.0f);
            Vector3 direction = Vector3.Transform(defaultDirection, cameraRotation);

            position = new Vector3(100.0f, 50.0f, 100.0f);
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

        public void Update(KeyboardState kb, MouseState ms, GameTime gt)
        {
            float delta_X = ms.X - viewCenter.X;
            float delta_Y = ms.Y - viewCenter.Y;

            _yaw -= _rotationSpeed * (delta_X) * MathHelper.ToRadians(2f) * (float)gt.ElapsedGameTime.TotalSeconds;
            _pitch += _rotationSpeed * (delta_Y) * MathHelper.ToRadians(2f) * (float)gt.ElapsedGameTime.TotalSeconds;

            Vector3 defaultDirection = new Vector3(-1.0f, 0.0f, -1.0f);
            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(_yaw, _pitch, 0.0f);
            Vector3 direction = Vector3.Transform(defaultDirection, cameraRotation);

            Vector3 novaPos = position;
            Vector3 right = Vector3.Cross(direction, Vector3.Up);
            right.Normalize();
            if (kb.IsKeyDown(Keys.NumPad8))
                novaPos = position + direction * _movementSpeed * (float)gt.ElapsedGameTime.TotalSeconds;
            if (kb.IsKeyDown(Keys.NumPad5))
                novaPos = position - direction * _movementSpeed * (float)gt.ElapsedGameTime.TotalSeconds;
            if (kb.IsKeyDown(Keys.NumPad4))
                novaPos = position - right * _movementSpeed * (float)gt.ElapsedGameTime.TotalSeconds;
            if (kb.IsKeyDown(Keys.NumPad6))
                novaPos = position + right * _movementSpeed * (float)gt.ElapsedGameTime.TotalSeconds;

            // CameraGhostMode exclusive "up&down" controls
            if (kb.IsKeyDown(Keys.NumPad1))
                novaPos = position - Vector3.Up * _movementSpeed * (float)gt.ElapsedGameTime.TotalSeconds;
            if (kb.IsKeyDown(Keys.NumPad7))
                novaPos = position + Vector3.Up * _movementSpeed * (float)gt.ElapsedGameTime.TotalSeconds;

            position = novaPos;

            Vector3 target = position + direction;
            viewMatrix = Matrix.CreateLookAt(position, target, _normal);

            Mouse.SetPosition((int)viewCenter.X, (int)viewCenter.Y);
        }

        public Matrix ViewMatrix { get { return viewMatrix; } }
        public Vector3 Position { get { return position; } }
        public Matrix ProjectionMatrix { get { return projectionMatrix; } }
    }
}
