using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShootCube.Global;
using System.Linq;
using ShootCube.World.Chunk.Model;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using ShootCube.World.Chunk;

namespace ShootCube.Sky
{
    public class Sun
    {
        public Vector3 Position { get; private set; }
        public Vector3 ReferencePosition { get; private set; }
        public Vector3 CorrespondingDirection { get; private set; }

        public int Radius { get; set; }

        public Texture2D Texture { get; private set; }
        public BasicEffect Effect { get; private set; }

        private VertexBuffer vertexBuffer;
        private VertexPositionColor[] vertices = new VertexPositionColor[4];

        private float angle;
        private Color color = new Color(255, 255, 0, 255);

        public Sun(Vector3 startPosition)
        {
            ReferencePosition = startPosition;
            

            vertices[0].Position = new Vector3(0, 0, 0);
            vertices[1].Position = new Vector3(1, 0, 0);
            vertices[2].Position = new Vector3(1, 1, 0);
            vertices[3].Position = new Vector3(0, 1, 0);
            
            vertexBuffer = new VertexBuffer(Globals.GraphicsDevice, typeof(VertexPositionColor), 4, BufferUsage.WriteOnly);

            Effect = new BasicEffect(Globals.GraphicsDevice);
        }

        public void Update()
        {
            float last = Position.Y;

            float z = Radius * (float)Math.Cos(Sky.Time) + ReferencePosition.Z;
            float y = Radius * (float)Math.Sin(Sky.Time) + ReferencePosition.Y;
            float x = ReferencePosition.X;

            Position = new Vector3(x, y, z);
            Position = Position ;
            Vector3 dir = Position - Camera.CameraPosition;
            angle = -(float)Math.Atan2(dir.Y, dir.Z);
          
            if(Position.Y  < last)
            {
                if(color.G > 165)
                color.G--;
                
            }else if(Position.Y > last)
            {
                if (color.G < 255)
                    color.G++;
            }

            uploadVertices(color);
        }

        private void uploadVertices(Color color)
        {
            vertices[0].Color = color;
            vertices[1].Color = color;
            vertices[2].Color = color;
            vertices[3].Color = color;

            vertexBuffer.SetData<VertexPositionColor>(vertices);
        }


        public void Render()
        {
            Globals.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            Effect.World = Matrix.CreateScale(50) * Matrix.CreateRotationX(angle) * Matrix.CreateTranslation(Position);
            Effect.View = Camera.View;
            Effect.Projection = Camera.Projection;
            Effect.VertexColorEnabled = true;
            Effect.CurrentTechnique.Passes[0].Apply();

            Globals.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
        }
    }
}
