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
    public class ClsTerrain
    {
        Texture2D heightsMap;
        Color[] dataRGB;
        List<float> heights;

        BasicEffect effect;
        GraphicsDevice device;

        Texture2D terrainTexture;
        VertexPositionNormalTexture[] vertices;
        short[] indices;
        int vertexCount;
        int indexCount;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        float heightFactor = 5.0f;
        float planeLength = 1.0f;

        public ClsTerrain(GraphicsDevice device, Texture2D heightsMap, Texture2D terrainTexture)
        {
            this.heightsMap = heightsMap;
            this.device = device;
            this.terrainTexture = terrainTexture;

            #region Load Heights
            dataRGB = new Color[heightsMap.Width * heightsMap.Height];
            heightsMap.GetData<Color>(dataRGB);

            heights = new List<float>();
            foreach (Color c in dataRGB)
                heights.Add((float)c.R);
            #endregion

            #region Normalize heights
            float averageHeight = 0;
            foreach (float h in heights)
                averageHeight += h;
            averageHeight /= heights.Count; // averageHeight = averageHeight / heights.Count

            for (int i = 0; i < heights.Count; i++)
            {
                heights[i] /= (averageHeight / heightFactor);
                //Console.WriteLine("[" + i + "] height = " + heights[i] + " m");
            }
            #endregion

            #region Create Terrain
            //  Vamos usar um efeito básico
            effect = new BasicEffect(device);
            
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Texture = this.terrainTexture;

            effect.LightingEnabled = true;
            effect.AmbientLightColor = GameSettings.AmbientLightColor;
            effect.DiffuseColor = GameSettings.DiffuseColor;
            effect.DirectionalLight0.Enabled = GameSettings.DirectionalLight0_Enabled;
            effect.DirectionalLight0.DiffuseColor = GameSettings.DirectionalLight0_DiffuseColor;
            effect.DirectionalLight0.Direction = GameSettings.DirectionalLight0_Direction;
            effect.DirectionalLight0.Direction.Normalize();

            CreateGeometry();
            #endregion
        }

        public void CreateGeometry()
        {
            vertexCount = heightsMap.Width * heightsMap.Height;
            vertices = new VertexPositionNormalTexture[vertexCount];
            Vector2 textCoord = new Vector2();
            for (int row = 0; row < heightsMap.Height; row++)
            {
                for (int column = 0; column < heightsMap.Width; column++)
                {
                    if (row % 2 == 0 && column % 2 == 0) textCoord = new Vector2(0f, 0f);
                    else if (row % 2 == 0 && column % 2 != 0) textCoord = new Vector2(0f, 1f);
                    else if (row % 2 != 0 && column % 2 == 0) textCoord = new Vector2(1f, 0f);
                    else if (row % 2 != 0 && column % 2 != 0) textCoord = new Vector2(1f, 1f);

                    vertices[row * heightsMap.Width + column] = new VertexPositionNormalTexture(
                        new Vector3(column, heights[row * heightsMap.Width + column], row) * new Vector3(planeLength, 1, planeLength),
                        Vector3.Zero,
                        textCoord);
                }
            }

            CalcNormals();

            indexCount = (heightsMap.Width - 1) * (heightsMap.Height - 1) * 4;
            indices = new short[indexCount];
            int counter = 0;
            for (int row = 0; row < heightsMap.Height - 1; row++)
            {
                for (int column = 0; column < heightsMap.Width; column++)
                {
                    short left = (short)(row + column * heightsMap.Height);
                    short right = (short)((row + 1) + column * heightsMap.Height);

                    indices[counter++] = left;
                    indices[counter++] = right;
                }
            }

            vertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertexCount, BufferUsage.None);
            vertexBuffer.SetData(vertices);

            indexBuffer = new IndexBuffer(device, typeof(short), indexCount, BufferUsage.None);
            indexBuffer.SetData(indices);
        }

        private void CalcNormals()
        {
            Vector3 vUp, vRight, vDown, vLeft;
            Vector3 n1, n2, n3, n4;
            int currentVertex;

            // middle horRingVertices
            for (int row = 1; row < heightsMap.Height - 1; row++)
            {
                for (int column = 1; column < heightsMap.Width - 1; column++)
                {
                    currentVertex = row + column * heightsMap.Width;

                    vUp = vertices[currentVertex - heightsMap.Width].Position - vertices[currentVertex].Position;
                    vRight = vertices[currentVertex + 1].Position - vertices[currentVertex].Position;
                    vDown = vertices[currentVertex + heightsMap.Width].Position - vertices[currentVertex].Position;
                    vLeft = vertices[currentVertex - 1].Position - vertices[currentVertex].Position;

                    n1 = Vector3.Cross(vRight, vUp);
                    n2 = Vector3.Cross(vDown, vRight);
                    n3 = Vector3.Cross(vLeft, vDown);
                    n4 = Vector3.Cross(vUp, vLeft);

                    vertices[currentVertex].Normal = (n1 + n2 + n3 + n4) / 4;
                    vertices[currentVertex].Normal.Normalize();
                }
            }

            // top horRingVertices (except corner horRingVertices)
            for (int column = 1; column < heightsMap.Width - 1; column++)
            {
                currentVertex = column;

                vRight = vertices[currentVertex + 1].Position - vertices[currentVertex].Position;
                vDown = vertices[currentVertex + heightsMap.Width].Position - vertices[currentVertex].Position;
                vLeft = vertices[currentVertex - 1].Position - vertices[currentVertex].Position;

                n1 = Vector3.Cross(vDown, vRight);
                n2 = Vector3.Cross(vLeft, vDown);

                vertices[currentVertex].Normal = (n1 + n2) / 2;
                vertices[currentVertex].Normal.Normalize();
            }
            // bottom horRingVertices (except corner horRingVertices)
            for (int column = 1; column < heightsMap.Width - 1; column++)
            {
                currentVertex = heightsMap.Height * heightsMap.Width - heightsMap.Width + column;

                vLeft = vertices[currentVertex - 1].Position - vertices[currentVertex].Position;
                vUp = vertices[currentVertex - heightsMap.Width].Position - vertices[currentVertex].Position;
                vRight = vertices[currentVertex + 1].Position - vertices[currentVertex].Position;

                n1 = Vector3.Cross(vUp, vLeft);
                n2 = Vector3.Cross(vRight, vUp);

                vertices[currentVertex].Normal = (n1 + n2) / 2;
                vertices[currentVertex].Normal.Normalize();
            }
            // left horRingVertices (except corner horRingVertices)
            for (int row = 1; row < heightsMap.Height - 1; row++)
            {
                currentVertex = row * heightsMap.Height;

                vUp = vertices[currentVertex - heightsMap.Height].Position - vertices[currentVertex].Position;
                vRight = vertices[currentVertex + 1].Position - vertices[currentVertex].Position;
                vDown = vertices[currentVertex + heightsMap.Height].Position - vertices[currentVertex].Position;

                n1 = Vector3.Cross(vRight, vUp);
                n2 = Vector3.Cross(vDown, vRight);

                vertices[currentVertex].Normal = (n1 + n2) / 2;
                vertices[currentVertex].Normal.Normalize();
            }
            // right horRingVertices (except corner horRingVertices)
            for (int row = 1; row < heightsMap.Height - 1; row++)
            {
                currentVertex = row * heightsMap.Height + (heightsMap.Width - 1);

                vUp = vertices[currentVertex - heightsMap.Height].Position - vertices[currentVertex].Position;
                vLeft = vertices[currentVertex - 1].Position - vertices[currentVertex].Position;
                vDown = vertices[currentVertex + heightsMap.Height].Position - vertices[currentVertex].Position;

                n1 = Vector3.Cross(vUp, vLeft);
                n2 = Vector3.Cross(vLeft, vDown);

                vertices[currentVertex].Normal = (n1 + n2) / 2;
                vertices[currentVertex].Normal.Normalize();
            }

            // top left vertex (first)
            currentVertex = 0;
            vRight = vertices[currentVertex + 1].Position - vertices[currentVertex].Position;
            vDown = vertices[currentVertex + heightsMap.Width].Position - vertices[currentVertex].Position;
            vertices[currentVertex].Normal = Vector3.Cross(vDown, vRight);
            vertices[currentVertex].Normal.Normalize();

            // top right vertex
            currentVertex = heightsMap.Width - 1;
            vLeft = vertices[currentVertex - 1].Position - vertices[currentVertex].Position;
            vDown = vertices[currentVertex + heightsMap.Width].Position - vertices[currentVertex].Position;
            vertices[currentVertex].Normal = Vector3.Cross(vLeft, vDown);
            vertices[currentVertex].Normal.Normalize();

            // bottom left vertex
            currentVertex = vertexCount - heightsMap.Width;
            vRight = vertices[currentVertex + 1].Position - vertices[currentVertex].Position;
            vUp = vertices[currentVertex - heightsMap.Width].Position - vertices[currentVertex].Position;
            vertices[currentVertex].Normal = Vector3.Cross(vRight, vUp);
            vertices[currentVertex].Normal.Normalize();

            // bottom right vertex (last)
            currentVertex = vertexCount - 1;
            vLeft = vertices[currentVertex - 1].Position - vertices[currentVertex].Position;
            vUp = vertices[currentVertex - heightsMap.Width].Position - vertices[currentVertex].Position;
            vertices[currentVertex].Normal = Vector3.Cross(vUp, vLeft);
            vertices[currentVertex].Normal.Normalize();
        }

        public float CalcHeightByInterpolation(float xP, float zP)
        {
            /*
                    dA     dB
            .A<---------><--->.B
                        ^
                        | dAB
                        v
                        .p
                        ^
                        | dCD
                        v
            .C<---------><--->.D
                    dC     dD
            
            (xP, zP) -> yP?

            xA = (int)xP
            zA = (int)zP
            yA = arrayAlturas[zA*w + xA], w = 128, etc.

            dA = |xP - xA|
            dB = |xP - xB|

            yAB = dA*yB + dB*yA
            yCD = dC*yD + dD*yC

            yP = dAB*yCD + dCD*yAB
            */

            Vector3 P = new Vector3(xP, 0, zP);

            Vector3 A = new Vector3((int)P.X, 0, (int)P.Z); A.Y = GetHeightAt((int)A.X, (int)A.Z);
            Vector3 B = A + new Vector3(0, 0, P.Z >= 0 ? 1 : -1); B.Y = GetHeightAt((int)B.X, (int)B.Z);
            Vector3 C = A + new Vector3(P.X >= 0 ? 1 : -1, 0, 0); C.Y = GetHeightAt((int)C.X, (int)C.Z);
            Vector3 D = A + new Vector3(P.X >= 0 ? 1 : -1, 0, P.Z >= 0 ? 1 : -1); D.Y = GetHeightAt((int)D.X, (int)D.Z);

            float dA = Math.Abs(P.Z - A.Z);
            float dB = Math.Abs(P.Z - B.Z);
            float dC = Math.Abs(P.Z - C.Z);
            float dD = Math.Abs(P.Z - D.Z);
            float dAB = Math.Abs(P.X - A.X);
            float dCD = Math.Abs(P.X - C.X);

            float yAB = dA * B.Y + dB * A.Y;
            float yCD = dC * D.Y + dD * C.Y;
            float yP = dAB * yCD + dCD * yAB;

            return yP;
        }

        public Vector3 CalcNormalByInterpolation(float xP, float zP)
        {
            Vector3 P = new Vector3(xP, 0, zP);

            Vector3 A = new Vector3((int)P.X, 0, (int)P.Z);
            Vector3 B = A + new Vector3(0, 0, P.Z >= 0 ? 1 : -1);
            Vector3 C = A + new Vector3(P.X >= 0 ? 1 : -1, 0, 0);
            Vector3 D = A + new Vector3(P.X >= 0 ? 1 : -1, 0, P.Z >= 0 ? 1 : -1);

            float dA = Math.Abs(P.Z - A.Z);
            float dB = Math.Abs(P.Z - B.Z);
            float dC = Math.Abs(P.Z - C.Z);
            float dD = Math.Abs(P.Z - D.Z);
            float dAB = Math.Abs(P.X - A.X);
            float dCD = Math.Abs(P.X - C.X);

            Vector3 nAB = dA * GetNormalAt((int)B.X, (int)B.Z) + dB * GetNormalAt((int)A.X, (int)A.Z); nAB.Normalize();
            Vector3 nCD = dC * GetNormalAt((int)D.X, (int)D.Z) + dD * GetNormalAt((int)C.X, (int)C.Z); nCD.Normalize();
            Vector3 nP = dAB * nCD + dCD * nAB; nP.Normalize();

            return nP;
        }

        // check if coordinates are beyond Terrain limits
        public bool CheckIfBeyondTerrainBoundaries(Vector3 position)
        {
            return position.X < -GetMapWidth() / 2 + 1
                || position.X > GetMapWidth() / 2 - 1.1f
                || position.Z < -GetMapHeight() / 2 + 1
                || position.Z > GetMapHeight() / 2 - 1.1f;
        }

        // correct position in order to avoid throwing an exception, whenever are used coordinates beyond Terrain limits to calc horRingVertices' normals
        public Vector3 CorrectPosition(Vector3 position)
        {
            if (position.X < -GetMapWidth() / 2 + 1)
                position.X = -GetMapWidth() / 2 + 1 + Single.Epsilon;
            if (position.X > GetMapWidth() / 2 - 1.1f)
                position.X = GetMapWidth() / 2 - 1.1f - Single.Epsilon;

            if (position.Z < -GetMapHeight() / 2 + 1)
                position.Z = -GetMapHeight() / 2 + 1 + Single.Epsilon;
            if (position.Z > GetMapHeight() / 2 - 1.1f)
                position.Z = GetMapHeight() / 2 - 1.1f - Single.Epsilon;

            return position;
        }

        // get random position inside the map
        public Vector3 GetRandomPosition()
        {
            Vector3 randomPosition = Vector3.Zero;
            randomPosition.X = GameSettings.RandomFloat(-heightsMap.Width / 2, heightsMap.Width / 2);
            randomPosition.Z = GameSettings.RandomFloat(-heightsMap.Height / 2, heightsMap.Height / 2);
            CorrectPosition(randomPosition);
            //randomPosition.Y = CalcHeightByInterpolation(randomPosition.X, randomPosition.Z);

            return randomPosition;
        }

        public void Draw(GraphicsDevice device, ICamera camera)
        {
            // WireFrame FillMode
            // http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series1/Indices.php
            //RasterizerState rs = new RasterizerState();
            //rs.FillMode = FillMode.WireFrame;
            //device.RasterizerState = rs;

            // World Matrix
            effect.World = Matrix.CreateTranslation(-heightsMap.Width / 2, 0, -heightsMap.Height / 2);
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            // Indica o efeito para desenhar os eixos
            effect.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;

            for (int i = 0; i < heightsMap.Width - 1; i++)
            {
                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    0,
                    i * heightsMap.Height * 2,
                    (heightsMap.Height - 1) * 2);
            }
        }

        private float GetHeightAt(int row, int column) => heights[(row + heightsMap.Width / 2) + (column + heightsMap.Height / 2) * heightsMap.Height];
        public float GetMapWidth() => heightsMap.Width;
        public float GetMapHeight() => heightsMap.Height;
        public Vector3 GetNormalAt(int row, int column) => vertices[(row + heightsMap.Width / 2) + (column + heightsMap.Height / 2) * heightsMap.Height].Normal;
        public VertexPositionNormalTexture[] GetTerrainVertices() => vertices;
    }
}
