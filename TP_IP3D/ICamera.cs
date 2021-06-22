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
    public interface ICamera
    {
        void Update(KeyboardState kb, MouseState ms, GameTime gt);

        Matrix ViewMatrix { get; }
        Vector3 Position { get; }
        Matrix ProjectionMatrix { get; }
    }
}
