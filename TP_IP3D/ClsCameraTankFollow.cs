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
    class ClsCameraTankFollow : ICamera
    {
        ClsTank tank;

        Matrix viewMatrix;

        Vector3 position;

        Vector3 _normal;
        Matrix projectionMatrix;
        float _fov;
        float _aspectRatio;
        float _nearPlane;
        float _farPlane;

        float _distToTank = 5.0f;

        public ClsCameraTankFollow(GraphicsDevice device, ClsTank tank)
        {
            this.tank = tank;

            #region ViewMatrix
            viewMatrix = Matrix.Identity;
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
            #region ViewMatrix
            Vector3 direction = -tank.Rotation.Forward;

            _normal = tank.Rotation.Up;

            position = tank.Position + (_normal * 0.60f - direction * 1.80f) * _distToTank;

            Vector3 target = position + direction;
            viewMatrix = Matrix.CreateLookAt(position, target, _normal);
            #endregion
        }

        public Matrix ViewMatrix { get { return viewMatrix; } }
        public Vector3 Position { get { return position; } }
        public Matrix ProjectionMatrix { get { return projectionMatrix; } }
    }
}
