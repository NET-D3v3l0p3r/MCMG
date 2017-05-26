using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Microsoft.Xna.Framework.Content;
using ShootCube.Maths;
using ShootCube.World.Chunk.Model;

namespace ShootCube.Global
{
    public static class Globals
    {

        public static GraphicsDevice GraphicsDevice { get; set; }
        public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }
        public static ContentManager Content { get; set; }

        public static short[] IndicesDefinitionSprite = new short[] { 0, 1, 2, 2, 3, 0 };
        public static IndexBuffer IndexBuffer { get; private set; }

        public static Effect Effect { get; private set; }
        public static Random Random { get; private set; }

        public enum Side
        {
            HorizontalUp,
            HorizontalDown,

            XForward,
            XBackward,

            ZLeft,
            ZRight
        };

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

        public static void Initialize()
        {
            if (!Directory.Exists(@"data\chunks"))
                Directory.CreateDirectory(@"data\chunks");
            if (!File.Exists(@"data\chunks\indices.byte"))
            {
                int[] indices = new int[2500000 * 6];

                int offset = 0;
                int offsetVertices = 0;

                for (int i = 0; i < indices.Length / 6; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        indices[j + offset] = IndicesDefinitionSprite[j] + offsetVertices;
                    }
                    offset += 6;
                    offsetVertices += 4;
                }

                File.WriteAllBytes(@"data\chunks\indices.byte", indices.SerializeToByteArray());
                indices = new int[0];


            }

            int[] full = File.ReadAllBytes(@"data\chunks\indices.byte").DeserializeToDynamicType<int[]>();

            IndexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, full.Length, BufferUsage.WriteOnly);
            IndexBuffer.SetData<int>(full);

            full = new int[0];
            GC.Collect();
            Console.WriteLine("DONE LOADING INDICES!");
            Effect = Content.Load<Effect>(@"main");
            Console.WriteLine("DONE LOADING EFFECT!");

            Random = new Random();
        }
    }
}
