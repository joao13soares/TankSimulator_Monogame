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
    public interface ICollider
    {
        string Name();

        // notifies object that there was a collision
        void CollidedWith(ICollider other);

        // check if collisions exist
        bool CheckIfCollidesWith(ICollider other);

        // returns itself (collider)
        ICollider GetCollider();

        // show colliders in 3D space
        void DrawCollider(ICamera camera);
    }
}
