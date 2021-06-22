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
    class ClsSphereCollider : ICollider
    {
        public Vector3 Center { get; set; }
        public float Radius { get; set; }
        Matrix rotation;

        BasicEffect effect;
        GraphicsDevice device;

        int nSides = 50;
        VertexPositionColor[] horRingVertices, vertRingVertices;
        short[] horRingIndices, vertRingIndices;
        int vertexCount;
        int indexCount;
        VertexBuffer horRingVertexBuffer, vertRingVertexBuffer;
        IndexBuffer horRingIndexBuffer, vertRingIndexBuffer;
        Color color;

        public ClsSphereCollider(GraphicsDevice device, Vector3 center, float radius, Color color)
        {
            Center = center; Radius = radius;

            this.color = color;

            this.device = device;
            effect = new BasicEffect(device);
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = false;

            CreateColliderGeometry();
        }

        private void CreateColliderGeometry()
        {
            CreateVertices();
            IndexVertices();
            CreateBuffers();
        }
        private void CreateVertices()
        {
            vertexCount = 2 * nSides;

            horRingVertices = new VertexPositionColor[vertexCount];
            vertRingVertices = new VertexPositionColor[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                horRingVertices[i] = new VertexPositionColor(Center, color);
                vertRingVertices[i] = horRingVertices[i];
            }
        }
        private void IndexVertices()
        {
            indexCount = 2 * nSides + 2;
            
            horRingIndices = new short[indexCount];
            vertRingIndices = new short[indexCount];
            for (int i = 0; i < nSides; i++)
            {
                horRingIndices[i] = (short)i;
                vertRingIndices[i] = horRingIndices[i];
            }
            horRingIndices[nSides] = horRingIndices[0];
            vertRingIndices[nSides] = horRingIndices[0];
        }
        private void CreateBuffers()
        {
            // horRing
            horRingVertexBuffer = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, vertexCount, BufferUsage.None);
            horRingVertexBuffer.SetData(horRingVertices);

            horRingIndexBuffer = new IndexBuffer(device, typeof(short), indexCount, BufferUsage.None);
            horRingIndexBuffer.SetData(horRingIndices);


            // vertRing
            vertRingVertexBuffer = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, vertexCount, BufferUsage.None);
            vertRingVertexBuffer.SetData(vertRingVertices);

            vertRingIndexBuffer = new IndexBuffer(device, typeof(short), indexCount, BufferUsage.None);
            vertRingIndexBuffer.SetData(vertRingIndices);
        }

        public void UpdateColliderPosition(Vector3 center, Matrix rotation)
        {
            Center = center;
            this.rotation = rotation;

            float angle = - 2 * (float)Math.PI / nSides;

            // horRingVertices
            for (int i = 0; i < nSides; i++)
            {
                horRingVertices[i].Position = Center + Radius * new Vector3((float)Math.Cos(angle * i), 0f, (float)Math.Sin(angle * i));
                vertRingVertices[i].Position = Center + Radius * new Vector3((float)Math.Cos(angle * i), (float)Math.Sin(angle * i), 0f);
            }

            horRingVertexBuffer.SetData(horRingVertices);
            vertRingVertexBuffer.SetData(vertRingVertices);
        }

        public string Name() => "sphereCollider";

        public void CollidedWith(ICollider other) { }

        virtual public bool CheckIfCollidesWith(ClsSphereCollider other)
        {
            float dist1 = (Center - other.Center).LengthSquared();
            float dist2 = (float)Math.Pow(Radius + other.Radius, 2f);
            return dist1 < dist2;
        }

        public bool CheckIfCollidesWith(ICollider other)
        {
            ICollider collider = other.GetCollider();
            switch (collider)
            {
                case ClsSphereCollider s:
                    return CheckIfCollidesWith(s);
                default:
                    return false;
            }
        }

        public ICollider GetCollider() => this;

        public void DrawCollider(ICamera camera)
        {
            effect.World = Matrix.CreateTranslation(-Center) * rotation * Matrix.CreateTranslation(Center);
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            effect.CurrentTechnique.Passes[0].Apply();

            device.SetVertexBuffer(horRingVertexBuffer);
            device.Indices = horRingIndexBuffer;
            device.DrawIndexedPrimitives(PrimitiveType.LineStrip, 0, 0, nSides);

            device.SetVertexBuffer(vertRingVertexBuffer);
            device.Indices = vertRingIndexBuffer;
            device.DrawIndexedPrimitives(PrimitiveType.LineStrip, 0, 0, nSides);
        }
    }
}
