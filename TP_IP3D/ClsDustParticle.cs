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
    class ClsDustParticle
    {
        GraphicsDevice device;

        VertexPositionColor[] vertices;
        short[] sideIndices, topIndices, bottomIndices;
        VertexBuffer vertexBuffer;
        IndexBuffer sideIndexBuffer, topIndexBuffer, bottomIndexBuffer;
        float dustParticleSize;
        Color color;

        Vector3 initialPosition;
        Vector3 initialVelocity;
        Vector3 velocity;
        float gravityAcceleration = 9.8f;
        float creationTime;
        float lifeTime;
        float maxLifeTime = 5.0f;
        bool isDead = false;

        public ClsDustParticle(GraphicsDevice device, GameTime gt, Vector3 initialPosition, Vector3 initialVelocity, Color color)
        {
            this.device = device;

            this.initialPosition = initialPosition; // p0
            this.initialVelocity = initialVelocity; // v0
            velocity = initialVelocity; // v = v0
            creationTime = (float)gt.TotalGameTime.TotalSeconds; // t0

            this.color = color;

            CreateDustParticle();
        }

        // cubed dustParticle
        private void CreateDustParticle()
        {
            CreateVertices();
            IndexVertices();
            CreateBuffers();
        }

        private void CreateVertices()
        {
            dustParticleSize = GameSettings.RandomFloat(0.01f, 0.07f); // 0.01 < dustParticleSize < 0.07

            // horRingVertices
            vertices = new VertexPositionColor[2 * 4];
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = new VertexPositionColor(initialPosition, color);
        }

        private void IndexVertices()
        {
            // sideIndices
            sideIndices = new short[2 * 4 + 2];
            for (int i = 0; i < vertices.Length; i++)
                sideIndices[i] = (short)i;
            sideIndices[8] = sideIndices[0];
            sideIndices[9] = sideIndices[1];

            // topIndices
            topIndices = new short[4];
            topIndices[0] = (short)7;
            topIndices[1] = (short)5;
            topIndices[2] = (short)1;
            topIndices[3] = (short)3;

            // horRingIndices
            bottomIndices = new short[4];
            bottomIndices[0] = (short)0;
            bottomIndices[1] = (short)2;
            bottomIndices[2] = (short)6;
            bottomIndices[3] = (short)4;
        }

        private void CreateBuffers()
        {
            // horRingVertices
            vertexBuffer = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, vertices.Length, BufferUsage.None);
            vertexBuffer.SetData(vertices);

            // sideIndices
            sideIndexBuffer = new IndexBuffer(device, typeof(short), sideIndices.Length, BufferUsage.None);
            sideIndexBuffer.SetData(sideIndices);

            // topIndices
            topIndexBuffer = new IndexBuffer(device, typeof(short), topIndices.Length, BufferUsage.None);
            topIndexBuffer.SetData(topIndices);

            // horRingIndices
            bottomIndexBuffer = new IndexBuffer(device, typeof(short), bottomIndices.Length, BufferUsage.None);
            bottomIndexBuffer.SetData(bottomIndices);
        }

        public void Update(GameTime gt)
        {
            // Δt - total time (in seconds) that the dustParticle has been "alive"
            lifeTime = (float)gt.TotalGameTime.TotalSeconds - creationTime;
            if (lifeTime > maxLifeTime) isDead = true;

            // gravity effect
            velocity.Y = initialVelocity.Y - 0.05f * gravityAcceleration * lifeTime; // vy = v0y - g*Δt

            // vertices.Position
            for (int i = 0; i < vertices.Length / 2; i++)
            {
                // posição x & z para os vértices da esquerda
                float angle = (float)i * (MathHelper.Pi / 2);
                float x = dustParticleSize / 2 * (float)Math.Cos(angle);
                float z = dustParticleSize / 2 * -(float)Math.Sin(angle);

                vertices[2 * i + 0].Position = initialPosition + velocity * lifeTime + new Vector3(x, 0, z); // p = p0 + v0*Δt
                vertices[2 * i + 1].Position = vertices[2 * i + 0].Position + Vector3.Up * dustParticleSize;
            }

            vertexBuffer.SetData(vertices);
        }

        public void Draw()
        {
            device.SetVertexBuffer(vertexBuffer);

            // sideIndices
            device.Indices = sideIndexBuffer;
            device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, sideIndices.Length - 2);

            // topIndices
            device.Indices = topIndexBuffer;
            device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, topIndices.Length - 2);

            // horRingIndices
            device.Indices = bottomIndexBuffer;
            device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, bottomIndices.Length - 2);
        }

        // return bottom vertex
        public Vector3 ContactPoint { get { return vertices[0].Position; } }
        public bool IsDead { get { return isDead; } }
    }
}
