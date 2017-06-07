using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShootCube.Global;
using ShootCube.World.Chunk.Model;

namespace ShootCube.Dynamics
{
    internal class Light
    {
        private Model _model;
        private readonly BoundingBox _boundingBox;

        internal int LightIntensity;
        internal int Value;

        internal int X;
        internal int Y;
        internal int Z;

        public Light(int value, int x, int y, int z, int intensity)
        {
            Value = value;

            X = x;
            Y = y;
            Z = z;

            LightIntensity = intensity;

            _model = Globals.Content.Load<Model>(@"light");

            _boundingBox = new BoundingBox(new Vector3(X, Y - 1, Z), new Vector3(X + 1, Y, Z + 1));
        }

        internal void LightUp()
        {
            var value = (float) (LightIntensity / (1.0f + Math.Pow(Math.E, -(Value - 7.0f)))) / LightIntensity + .5f;
            ;


            int leftValue = ChunkManager.Cubes[Y, X - 1, Z];
            int rightValue = ChunkManager.Cubes[Y, X + 1, Z];

            int frontValue = ChunkManager.Cubes[Y, X, Z + 1];
            int backValue = ChunkManager.Cubes[Y, X, Z - 1];

            int upValue = ChunkManager.Cubes[Y + 1, X, Z];
            int downValue = ChunkManager.Cubes[Y - 1, X, Z];

            if (leftValue != 0)
            {
                var left = new BoundingBox(new Vector3(X - 1, Y - 1, Z), new Vector3(X, Y, Z + 1));
                var c = ChunkManager.GetChunkForPosition(left.Min);
                c.EditLight(c.CubeMap[left].Faces.ToList().Find(p => p != null && p.Side == Globals.Side.ZRight),
                    1.0f + value);
            }

            if (rightValue != 0)
            {
                var right = new BoundingBox(new Vector3(X + 1, Y - 1, Z), new Vector3(X + 2, Y, Z + 1));
                var c = ChunkManager.GetChunkForPosition(right.Min);
                c.EditLight(c.CubeMap[right].Faces.ToList().Find(p => p != null && p.Side == Globals.Side.ZLeft),
                    1.0f + value);
            }

            if (frontValue != 0)
            {
                var front = new BoundingBox(new Vector3(X, Y - 1, Z + 1), new Vector3(X + 1, Y, Z + 2));
                var c = ChunkManager.GetChunkForPosition(front.Min);
                c.EditLight(c.CubeMap[front].Faces.ToList().Find(p => p != null && p.Side == Globals.Side.XBackward),
                    1.0f + value);
            }

            if (backValue != 0)
            {
                var back = new BoundingBox(new Vector3(X, Y - 1, Z - 1), new Vector3(X + 1, Y, Z));
                var c = ChunkManager.GetChunkForPosition(back.Min);
                c.EditLight(c.CubeMap[back].Faces.ToList().Find(p => p != null && p.Side == Globals.Side.XForward),
                    1.0f + value);
            }

            if (upValue != 0)
            {
                var top = new BoundingBox(new Vector3(X, Y, Z), new Vector3(X + 1, Y + 1, Z + 1));
                var c = ChunkManager.GetChunkForPosition(top.Min);
                c.EditLight(c.CubeMap[top].Faces.ToList().Find(p => p != null && p.Side == Globals.Side.HorizontalDown),
                    1.0f + value);
            }
            if (downValue != 0)
            {
                var down = new BoundingBox(new Vector3(X, Y - 2, Z), new Vector3(X + 1, Y - 1, Z + 1));
                var c = ChunkManager.GetChunkForPosition(down.Min);
                c.EditLight(c.CubeMap[down].Faces.ToList().Find(p => p != null && p.Side == Globals.Side.HorizontalUp),
                    1.0f + value);
            }
        }


        internal void Render()
        {
            BoundingBoxRenderer.Render(_boundingBox, Globals.GraphicsDevice, Camera.View, Camera.Projection,
                Color.Yellow);
        }
    }
}