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
    static class ClsGUI
    {
        public static void Draw(GraphicsDevice device, SpriteFont font, float health1, float health2)
        {
            SpriteBatch spriteBatch = new SpriteBatch(device);

            spriteBatch.Begin();

            string text = String.Format("Tank 1: " + health1 + "/100");
            Vector2 dim = font.MeasureString(text);
            spriteBatch.DrawString(font,
                        text,
                        new Vector2(5f, device.Viewport.Height - dim.Y),
                        Color.Blue);

            text = String.Format("Tank 2: " + health2 + "/100");
            dim = font.MeasureString(text);
            spriteBatch.DrawString(font,
                        text,
                        new Vector2(device.Viewport.Width - dim.X - 5f, device.Viewport.Height - dim.Y),
                        Color.Red);

            spriteBatch.End();
        }
    }
}
