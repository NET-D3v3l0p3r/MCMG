using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShootCube.Global;

namespace ShootCube.Sky
{
    public class Sun
    {
        private float _angle;
        private Color _color = new Color(255, 255, 0, 255);

        private readonly VertexBuffer _vertexBuffer;
        private readonly VertexPositionColor[] _vertices = new VertexPositionColor[4];

        public Sun(Vector3 startPosition)
        {
            ReferencePosition = startPosition;


            _vertices[0].Position = new Vector3(0, 0, 0);
            _vertices[1].Position = new Vector3(1, 0, 0);
            _vertices[2].Position = new Vector3(1, 1, 0);
            _vertices[3].Position = new Vector3(0, 1, 0);

            _vertexBuffer = new VertexBuffer(Globals.GraphicsDevice, typeof(VertexPositionColor), 4,
                BufferUsage.WriteOnly);

            Effect = new BasicEffect(Globals.GraphicsDevice);
        }

        public Vector3 Position { get; private set; }
        public Vector3 ReferencePosition { get; }
        public Vector3 CorrespondingDirection { get; private set; }

        public int Radius { get; set; }

        public Texture2D Texture { get; private set; }
        public BasicEffect Effect { get; }

        public void Update()
        {
            var last = Position.Y;

            var z = Radius * (float) Math.Cos(Sky.Time) + ReferencePosition.Z;
            var y = Radius * (float) Math.Sin(Sky.Time) + ReferencePosition.Y;
            var x = ReferencePosition.X;

            Position = new Vector3(x, y, z);
            Position = Position;
            var dir = Position - Camera.CameraPosition;
            _angle = -(float) Math.Atan2(dir.Y, dir.Z);

            if (Position.Y < last)
            {
                if (_color.G > 165)
                    _color.G--;
            }
            else if (Position.Y > last)
            {
                if (_color.G < 255)
                    _color.G++;
            }

            UploadVertices(_color);
        }

        private void UploadVertices(Color color)
        {
            _vertices[0].Color = color;
            _vertices[1].Color = color;
            _vertices[2].Color = color;
            _vertices[3].Color = color;

            _vertexBuffer.SetData(_vertices);
        }


        public void Render()
        {
            Globals.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            Effect.World = Matrix.CreateScale(50) * Matrix.CreateRotationX(_angle) * Matrix.CreateTranslation(Position);
            Effect.View = Camera.View;
            Effect.Projection = Camera.Projection;
            Effect.VertexColorEnabled = true;
            Effect.CurrentTechnique.Passes[0].Apply();

            Globals.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
        }
    }
}