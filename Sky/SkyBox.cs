using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShootCube.Global;
using ShootCube.World.Chunk.Model;

namespace ShootCube.Sky
{
    public class SkyBox
    {
        public VertexPosition[] Vertices = new VertexPosition[24]; // 4 each side * 6

        public SkyBox(int scale)
        {
            Scale = scale;
            var counterVertices = 0;
            // Front

            Vertices[counterVertices++] = new VertexPosition(new Vector3(0, 0, 0) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(1, 0, 0) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(1, 1, 0) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(0, 1, 0) * scale);

            // BACK
            Vertices[counterVertices++] = new VertexPosition(new Vector3(0, 0, 1) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(1, 0, 1) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(1, 1, 1) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(0, 1, 1) * scale);

            // LEFT

            Vertices[counterVertices++] = new VertexPosition(new Vector3(0, 0, 0) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(0, 0, 1) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(0, 1, 1) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(0, 1, 0) * scale);

            // RIGHT

            Vertices[counterVertices++] = new VertexPosition(new Vector3(1, 0, 0) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(1, 0, 1) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(1, 1, 1) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(1, 1, 0) * scale);

            //BOTTOM

            Vertices[counterVertices++] = new VertexPosition(new Vector3(0, 0, 0) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(0, 0, 1) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(1, 0, 1) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(1, 0, 0) * scale);

            // TOP

            Vertices[counterVertices++] = new VertexPosition(new Vector3(0, 1, 0) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(0, 1, 1) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(1, 1, 1) * scale);
            Vertices[counterVertices++] = new VertexPosition(new Vector3(1, 1, 0) * scale);

            VertexBuffer = new VertexBuffer(Globals.GraphicsDevice, typeof(VertexPosition), Vertices.Length,
                BufferUsage.WriteOnly);
            VertexBuffer.SetData(Vertices);

            Effect = Globals.Content.Load<Effect>("shader");
        }

        public Effect Effect { get; }
        public VertexBuffer VertexBuffer { get; }

        public int Scale { get; }

        public void Draw()
        {
            Effect.Parameters["Luminity"].SetValue((float) Math.Sin(Sky.Time));
            Effect.Parameters["Projection"].SetValue(Camera.Projection);
            Effect.Parameters["View"].SetValue(Camera.View);
            Effect.Parameters["World"]
                .SetValue(Matrix.CreateTranslation(new Vector3(-((Scale - ChunkManager.Width * Chunk.Width) / 2), -300,
                    -((Scale - ChunkManager.Depth * Chunk.Depth) / 2))));
            Effect.CurrentTechnique.Passes[0].Apply();

            Globals.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            Globals.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
        }
    }
}