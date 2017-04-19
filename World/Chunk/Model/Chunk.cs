using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShootCube.Global;

namespace ShootCube.World.Chunk.Model
{
    public class Chunk
    {
        public Vector3 LocalPosition { get; set; }

        public static int Width { get; set; }
        public static int Depth { get; set; }
        public static int Height { get; set; }

        public BoundingBox ChunkBox { get; set; }

        public List<BoundingBox> BoundingBoxes { get; set; }

        public VertexBuffer VertexBuffer { get; set; }
        public VertexPositionNormalTexture[] Vertices { get; set; }

        public int VerticesCount { get; private set; }

        public delegate void GlobalChange();
        public event GlobalChange OnGlobalChange;

        private List<IFlat> Tiles = new List<IFlat>();
        private int offSet = 0;


        private static object SYNC_LOCK = new object();

        public Chunk(Vector3 translation)
        {
            LocalPosition = translation;
            
            BoundingBoxes = new List<BoundingBox>();


            ChunkBox = new BoundingBox(
                new Vector3(LocalPosition.X + ChunkManager.GLOBAL_TRANSLATION.X - 0.5f, 0, LocalPosition.Z + ChunkManager.GLOBAL_TRANSLATION.Z - .5f), new Vector3(LocalPosition.X + ChunkManager.GLOBAL_TRANSLATION.X + Width - 0.5f, Height * 2, LocalPosition.Z + ChunkManager.GLOBAL_TRANSLATION.Z + Depth - 0.5f));


        }


        public void Generate()
        {

            Tiles.Clear();
            Analyze(0, 0, Width, Depth);

            generateVertexBuffer();
        }

        public void Analyze(int startX, int startZ, int width, int depth)
        {
            for (int j = startZ; j < startZ + depth; j++)
            {
                for (int i = startX; i < startX + width; i++)
                {
                    for (int k = 0; k < Height; k++)
                    {
                        bool add_bb = false;
                        int id = ChunkManager.CubeMap[k, i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, (j) + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z];
                        if ((j - 1) + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z >= 0)
                            if (ChunkManager.CubeMap[k, i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, (j - 1) + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z] == 0 &&
                               id != 0)
                            {
                                Tiles.Add(new XFlat(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k - 1, j, (byte)2));
                                add_bb = true;
                            }

                        if ((j + 1) + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z < Depth * ChunkManager.Depth)
                            if (ChunkManager.CubeMap[k, i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, (j + 1) + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z] == 0 &&
                               id != 0)
                            {
                                Tiles.Add(new XFlat(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k - 1, j + 1, (byte)2));
                                add_bb = true;
                            }

                        if ((i + 1) + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X < Width * ChunkManager.Width)
                            if (ChunkManager.CubeMap[k, (i + 1) + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, j + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z] == 0 &&
                               id != 0)
                            {
                                Tiles.Add(new ZFlat(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i + 1, k - 1, j, (byte)2));
                                add_bb = true;
                            }

                        if ((i - 1) + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X >= 0)
                            if (ChunkManager.CubeMap[k, (i - 1) + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, j + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z] == 0 &&
                               id != 0)
                            {
                                Tiles.Add(new ZFlat(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k - 1, j, (byte)2));
                                add_bb = true;
                            }


                        if ((k - 1) + (int)LocalPosition.Y + (int)ChunkManager.GLOBAL_TRANSLATION.Y >= 0)
                            if (ChunkManager.CubeMap[k - 1, i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, j + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z] == 0 &&
                                id != 0)
                            {
                                Tiles.Add(new Horizontal(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k - 1, j, (byte)id));

                                Vector3 min = new Vector3(i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, k, j + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z);
                                Vector3 max = new Vector3(i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X + 1, k + 1, j + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z + 1);
                                BoundingBox bb = new BoundingBox(min, max);
                                if (!BoundingBoxes.Contains(bb))
                                    BoundingBoxes.Add(bb);
                                add_bb = false;
                            }
                        if ((k + 1) + (int)LocalPosition.Y + (int)ChunkManager.GLOBAL_TRANSLATION.Y < Height)
                            if (ChunkManager.CubeMap[k + 1, i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, j + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z] == 0 &&
                                id != 0)
                            {
                                Tiles.Add(new Horizontal(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k, j, (byte)id));

                                Vector3 min = new Vector3(i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, k - 1, j + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z);
                                Vector3 max = new Vector3(i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X + 1, k, j + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z + 1);
                                BoundingBox bb = new BoundingBox(min, max);
                                if (!BoundingBoxes.Contains(bb))
                                    BoundingBoxes.Add(bb);

                                add_bb = false;
                            }


                        if (add_bb)
                        {
                            Vector3 min = new Vector3(i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, k - 1, j + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z);
                            Vector3 max = new Vector3(i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X + 1, k, j + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z + 1);
                            BoundingBox bb = new BoundingBox(min, max);
                            if (!BoundingBoxes.Contains(bb))
                                BoundingBoxes.Add(bb);
                        }
                    }
                }
            }
        }
        
        private void generateVertexBuffer()
        {

            offSet = 0;
            Vertices = new VertexPositionNormalTexture[Tiles.Count * 4];

            for (int i = 0; i < Tiles.Count; i++)
            {
                IFlat flat = Tiles[i];
                if (flat == null)
                    continue;
                for (int j = 0; j < flat.Vertices.Length; j++)
                {
                    if (j + offSet < Vertices.Length)
                        Vertices[j + offSet] = flat.Vertices[j];
                }
                offSet += 4;
            }

            VertexBuffer = new VertexBuffer(Globals.GraphicsDevice, typeof(VertexPositionNormalTexture), Vertices.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData<VertexPositionNormalTexture>(Vertices);

            VerticesCount = Tiles.Count;
            Tiles.Clear();


        }

        public void RemoveCube(BoundingBox box)
        {
            BoundingBoxes.Remove(box);
            int i = (int)box.Min.X;
            int j = (int)box.Min.Z;
            int y = (int)box.Max.Y;

            ChunkManager.CubeMap[y, i, j] = 0;

            Task.Run(() =>
            {
                lock (SYNC_LOCK)
                    Generate();
            });

        }

        public void Update(GameTime gTime)
        {

         
        }


        public void Render()
        {

            if (Camera.ViewFrustum == null)
                return;
            if (Camera.ViewFrustum.Contains(ChunkBox) == ContainmentType.Disjoint)
                return;

            if (VertexBuffer == null)
                return;

            Globals.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            Globals.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VerticesCount * 2);
        }

    }
}
