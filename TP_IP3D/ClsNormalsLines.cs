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
    class ClsNormalsLines
    {
        Game1 game;

        BasicEffect effect;

        VertexPositionColor[] normalsLinesVertices;
        bool normalsLinesOn = false;
        bool isNormalsLinesKeyPressed = false;

        public ClsNormalsLines(GraphicsDevice device, Game1 game)
        {
            this.game = game;

            effect = new BasicEffect(device);

            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;

            CreateGeometry();
        }

        private void CreateGeometry()
        {
            VertexPositionNormalTexture[] terrainVertices = game.Terrain.GetTerrainVertices();
            normalsLinesVertices = new VertexPositionColor[terrainVertices.Length * 2];
            float linesLength = 1;
            for (int i = 0; i < terrainVertices.Length; i++)
            {
                normalsLinesVertices[i * 2 + 0] = new VertexPositionColor(terrainVertices[i].Position, Color.Blue);
                normalsLinesVertices[i * 2 + 1] = new VertexPositionColor(terrainVertices[i].Position + linesLength * terrainVertices[i].Normal, Color.Blue);
            }
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();

            // algoritmo para evitar detetar tecla primida
            if (ks.IsKeyDown(GameSettings.NormalsLines))
                isNormalsLinesKeyPressed = true;
            if (ks.IsKeyUp(GameSettings.NormalsLines) && isNormalsLinesKeyPressed)
            {
                normalsLinesOn = !normalsLinesOn;
                isNormalsLinesKeyPressed = false;
            }
        }

        public void Draw(GraphicsDevice device, ICamera camera)
        {
            effect.World = Matrix.CreateTranslation(-game.Terrain.GetMapWidth() / 2, 0, -game.Terrain.GetMapHeight() / 2);
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            // Indica o efeito para desenhar os eixos
            effect.CurrentTechnique.Passes[0].Apply();


            if (normalsLinesOn)
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, normalsLinesVertices, 0, game.Terrain.GetTerrainVertices().Length);
        }
    }
}
