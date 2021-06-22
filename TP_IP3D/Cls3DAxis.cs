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
    class Cls3DAxis
    {
        BasicEffect effect;

        VertexPositionColor[] vertices;
        bool axisOn = true;
        bool isAxisKeyPressed = false;

        public Cls3DAxis(GraphicsDevice device)
        {
            //  Vamos usar um efeito básico
            effect = new BasicEffect(device);
            //  Calcula a aspectRatio, a view matrix e a projeção
            //float aspectRatio = (float)device.Viewport.Width /
            //               device.Viewport.Height;
            /*effect.View = Matrix.CreateLookAt(
                                new Vector3(50, 50, 100),
                                Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45.0f),
                               aspectRatio, 1.0f, 300.0f);*/
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;

            //  Cria os eixos 3D
            CreateGeometry();
        }

        private void CreateGeometry()
        {
            float axisLenght = 1000f; //  Tamanho da linha em cada sinal do eixo 
            int vertexCount = 6;   //  Vamos usar 6 vértices
            vertices = new VertexPositionColor[vertexCount];
            // Linha sobre o eixo X
            vertices[0] = new VertexPositionColor(new Vector3(-axisLenght, 0.0f, 0.0f), Color.Blue);
            vertices[1] = new VertexPositionColor(new Vector3(axisLenght, 0.0f, 0.0f), Color.Blue);
            // Linha sobre o eixo Y
            vertices[2] = new VertexPositionColor(new Vector3(0.0f, -axisLenght, 0.0f), Color.Green);
            vertices[3] = new VertexPositionColor(new Vector3(0.0f, axisLenght, 0.0f), Color.Green);
            // Linha sobre o eixo Z
            vertices[4] = new VertexPositionColor(new Vector3(0.0f, 0.0f, -axisLenght), Color.Red);
            vertices[5] = new VertexPositionColor(new Vector3(0.0f, 0.0f, axisLenght), Color.Red);
        }


        public void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();

            // algoritmo para evitar detetar tecla primida
            if (ks.IsKeyDown(GameSettings.Axis))
                isAxisKeyPressed = true;
            if (ks.IsKeyUp(GameSettings.Axis) && isAxisKeyPressed)
            {
                axisOn = !axisOn;
                isAxisKeyPressed = false;
            }
        }

        public void Draw(GraphicsDevice device, ICamera camera)
        {
            effect.World = Matrix.Identity;
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            // Indica o efeito para desenhar os eixos
            effect.CurrentTechnique.Passes[0].Apply();

            if (axisOn)
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 3);
        }

    }
}
