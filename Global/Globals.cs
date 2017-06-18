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


        public enum Face
        {
            NULL = -1,
            Top = 3,
            Bottom = 2,
            Left = 0,
            Right = 1,
            Front = 5,
            Back = 4

        };

        public static bool IntersectRayVsBox(BoundingBox a_kBox,
                   Ray a_kRay,
                   out float a_fDist,
                   out Face a_nFace)
        {
            a_nFace = (Face)Enum.Parse(typeof(Face), "-1");
            a_fDist = float.MaxValue;

            // Preform the collision query  
            float? fParam = a_kRay.Intersects(a_kBox);

            // No collision, return false.  
            if (!fParam.HasValue)
                return false;

            // Asign the distance along the ray our intersection point is  
            a_fDist = fParam.Value;

            // Compute the intersection point  
            Vector3 vIntersection = a_kRay.Position + a_kRay.Direction * a_fDist;

            // Determine the side of the box the ray hit, this is slower than  
            // more obvious methods but it's extremely tolerant of numerical  
            // drift (aka rounding errors)  
            Vector3 vDistMin = vIntersection - a_kBox.Min;
            Vector3 vDistMax = vIntersection - a_kBox.Max;

            vDistMin.X = (float)Math.Abs(vDistMin.X);
            vDistMin.Y = (float)Math.Abs(vDistMin.Y);
            vDistMin.Z = (float)Math.Abs(vDistMin.Z);

            vDistMax.X = (float)Math.Abs(vDistMax.X);
            vDistMax.Y = (float)Math.Abs(vDistMax.Y);
            vDistMax.Z = (float)Math.Abs(vDistMax.Z);


            // Start off assuming that our intersection point is on the  
            // negative x face of the bounding box.  
            a_nFace = 0;
            float fMinDist = vDistMin.X;

            // +X  
            if (vDistMax.X < fMinDist)
            {
                a_nFace = (Face)1;
                fMinDist = vDistMax.X;
            }

            // -Y  
            if (vDistMin.Y < fMinDist)
            {
                a_nFace = (Face)2;
                fMinDist = vDistMin.Y;
            }

            // +Y  
            if (vDistMax.Y < fMinDist)
            {
                a_nFace = (Face)3;
                fMinDist = vDistMax.Y;
            }

            // -Z  
            if (vDistMin.Z < fMinDist)
            {
                a_nFace = (Face)4;
                fMinDist = vDistMin.Z;
            }

            // +Z  
            if (vDistMax.Z < fMinDist)
            {
                a_nFace = (Face)5;
                fMinDist = vDistMin.Z;
            }

            return true;
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

        public static Vector2[] GetTextureCoordinate(byte id)
        {
            Vector2[] temp = new Vector2[4];

            if (!ChunkManager.TextureAtlasCoordinates.Keys.Contains(id))
                return new Vector2[] { new Vector2(-1), new Vector2(-1), new Vector2(-1), new Vector2(-1) };

            temp[0] = new Vector2(0 + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + ChunkManager.RatioY);
            temp[1] = new Vector2(ChunkManager.RatioX + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + ChunkManager.RatioY);
            temp[2] = new Vector2(ChunkManager.RatioX + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + 0);
            temp[3] = new Vector2(0 + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + 0);

            return temp;
        }

        public static T[,,] CopyChunk3D<T>(T[,,] from, int x, int y, int z, int w, int h, int d)
        {
            T[,,] temp = new T[h, w, d];

            int x0 = 0;
            int x1 = 0;
            int x2 = 0;

            for (int i = x; i < x + w; i++)
            {
                for (int j = z; j < z + d; j++)
                {
                    for (int k = y; k < y + h; k++)
                    {
                        temp[x2, x0, x1] = from[k, i, j];
                        x2++;
                    }

                    x1++;
                }
                x0++;
            }

            return temp;
        }
    }
}
