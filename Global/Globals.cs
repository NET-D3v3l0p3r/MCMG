using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShootCube.Global
{
    public static class Globals
    {
        public enum Orientation
        {
            North,
            East,
            South,
            West,

            NorthEast,
            SouthEast,
            SouthWest,
            NorthWest,

            Down,
            Up
        }

        public enum Side
        {
            HorizontalUp,
            HorizontalDown,

            XForward,
            XBackward,

            ZLeft,
            ZRight
        }

        public static short[] IndicesDefinitionSprite = {0, 1, 2, 2, 3, 0};

        public static GraphicsDevice GraphicsDevice { get; set; }
        public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }
        public static ContentManager Content { get; set; }
        public static IndexBuffer IndexBuffer { get; private set; }

        public static Effect Effect { get; private set; }
        public static Random Random { get; private set; }

        public static void Initialize()
        {
            if (!Directory.Exists(@"data\chunks"))
                Directory.CreateDirectory(@"data\chunks");
            if (!File.Exists(@"data\chunks\indices.byte"))
            {
                var indices = new int[2500000 * 6];

                var offset = 0;
                var offsetVertices = 0;

                for (var i = 0; i < indices.Length / 6; i++)
                {
                    for (var j = 0; j < 6; j++)
                        indices[j + offset] = IndicesDefinitionSprite[j] + offsetVertices;
                    offset += 6;
                    offsetVertices += 4;
                }

                File.WriteAllBytes(@"data\chunks\indices.byte", indices.SerializeToByteArray());
                indices = null;
            }

            var full = File.ReadAllBytes(@"data\chunks\indices.byte").DeserializeToDynamicType<int[]>();

            IndexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, full.Length,
                BufferUsage.WriteOnly);
            IndexBuffer.SetData(full);

            full = null;
            GC.Collect();
            Console.WriteLine("DONE LOADING INDICES!");
            Effect = Content.Load<Effect>(@"main");
            Console.WriteLine("DONE LOADING EFFECT!");

            Random = new Random();
        }
    }
}