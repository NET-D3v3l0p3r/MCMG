using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShootCube.Global;
using ShootCube.Declaration;
using System.Runtime.InteropServices;
using ShootCube.Global.Picking;

namespace ShootCube.World.Chunk.Model
{
    public class Chunk
    {
        public short ChunkId { get; private set; }
        public Vector3 LocalPosition { get; set; }

        public static int Width { get; set; }
        public static int Depth { get; set; }
        public static int Height { get; set; }

        public BoundingBox ChunkBox { get; set; }
        public Chunk[] Neighbours { get; set; }

        public List<BoundingBox> BoundingBoxes { get; set; }

        public VertexBuffer VertexBuffer { get; set; }
        public VPTVDeclaration[] Vertices { get; set; }

        public int VerticesCount { get; private set; }

        public List<IFlat> Tiles = new List<IFlat>();
        public Dictionary<BoundingBox, Cube> CubeMap = new Dictionary<BoundingBox, Cube>(); // FRAGILE !

        private int offSet = 0;

        private static object SYNC_LOCK = new object();

        public Chunk(Vector3 translation)
        {
            ChunkId = (short)Globals.Random.Next(0, short.MaxValue);
            LocalPosition = translation;
            
            BoundingBoxes = new List<BoundingBox>();


            ChunkBox = new BoundingBox(
                new Vector3(LocalPosition.X + ChunkManager.GLOBAL_TRANSLATION.X - 0.5f, 0, LocalPosition.Z + ChunkManager.GLOBAL_TRANSLATION.Z - .5f), new Vector3(LocalPosition.X + ChunkManager.GLOBAL_TRANSLATION.X + Width - 0.5f, Height * 2, LocalPosition.Z + ChunkManager.GLOBAL_TRANSLATION.Z + Depth - 0.5f));

            Neighbours = new Chunk[9];
        }


        public void Generate()
        {
            int x = (int)(LocalPosition.X - ChunkManager.GLOBAL_TRANSLATION.X);
            int z = (int)(LocalPosition.Z - ChunkManager.GLOBAL_TRANSLATION.Z);
            x /= 16;
            z /= 16;

            if (x - 1 >= 0 && z - 1 >= 0)
                Neighbours[0] = ChunkManager.Chunks[(x - 1) + (z - 1) * ChunkManager.Width];
            if (z - 1 >= 0)
                Neighbours[1] = ChunkManager.Chunks[(x) + (z - 1) * ChunkManager.Width];
            if (x + 1 < ChunkManager.Width && z - 1 >= 0)
                Neighbours[2] = ChunkManager.Chunks[(x + 1) + (z - 1) * ChunkManager.Width];

            if (x - 1 >= 0)
                Neighbours[3] = ChunkManager.Chunks[(x - 1) + (z) * ChunkManager.Width];
            Neighbours[4] = ChunkManager.Chunks[(x) + (z) * ChunkManager.Width];
            if (x + 1 < ChunkManager.Width)
                Neighbours[5] = ChunkManager.Chunks[(x + 1) + (z) * ChunkManager.Width];

            if (x - 1 >= 0 && z + 1 < ChunkManager.Depth)
                Neighbours[6] = ChunkManager.Chunks[(x - 1) + (z + 1) * ChunkManager.Width];
            if (z + 1 < ChunkManager.Depth)
                Neighbours[7] = ChunkManager.Chunks[x + (z + 1) * ChunkManager.Width];
            if (x + 1 < ChunkManager.Width && z + 1 < ChunkManager.Depth)
                Neighbours[8] = ChunkManager.Chunks[(x + 1) + (z + 1) * ChunkManager.Width];

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
                        int id = ChunkManager.Cubes[k, i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, (j) + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z];

                        #region "Correspoding neighbours"
                        byte back = 255;
                        byte forward = 255;

                        byte left = 255;
                        byte right = 255;

                        byte up = 255;
                        byte down = 255;

                        if ((j - 1) + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z >= 0)
                            back = ChunkManager.Cubes[k, i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, (j - 1) + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z];
                        if ((j + 1) + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z < Depth * ChunkManager.Depth)
                            forward = ChunkManager.Cubes[k, i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, (j + 1) + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z];

                        if ((i + 1) + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X < Width * ChunkManager.Width)
                            right = ChunkManager.Cubes[k, (i + 1) + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, j + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z];
                        if ((i - 1) + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X >= 0)
                            left = ChunkManager.Cubes[k, (i - 1) + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, j + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z];

                        if ((k - 1) + (int)LocalPosition.Y + (int)ChunkManager.GLOBAL_TRANSLATION.Y >= 0)
                            down = ChunkManager.Cubes[k - 1, i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, j + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z];
                        if ((k + 1) + (int)LocalPosition.Y + (int)ChunkManager.GLOBAL_TRANSLATION.Y < Height)
                            up = ChunkManager.Cubes[k + 1, i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, j + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z];

                        #endregion


                        #region "X"
                        if (back == 0 &&
                           id != 0)
                        {
                            Tiles.Add(new XFlat(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k - 1, j, ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.XBackward], .5f) { Side = Globals.Side.XBackward });
                            CubeMap[AddBoundingBox(i, k, j)].Faces[0] = Tiles[Tiles.Count - 1];

                            
                        }
                        if (forward == 0 &&
                           id != 0)
                        {
                            Tiles.Add(new XFlat(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k - 1, j + 1, ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.XForward], .5f) { Side = Globals.Side.XForward });
                            CubeMap[AddBoundingBox(i, k, j)].Faces[1] = Tiles[Tiles.Count - 1];
                        }
                        #endregion
                        #region "Z"
                        if (right == 0 &&
                           id != 0)
                        {
                            Tiles.Add(new ZFlat(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i + 1, k - 1, j, ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.ZRight], .5f) { Side = Globals.Side.ZRight });
                            CubeMap[AddBoundingBox(i, k, j)].Faces[2] = Tiles[Tiles.Count - 1];
                        }
                        if (left == 0 &&
                           id != 0)
                        {
                            Tiles.Add(new ZFlat(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k - 1, j, ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.ZLeft], .5f) { Side = Globals.Side.ZLeft });
                            CubeMap[AddBoundingBox(i, k, j)].Faces[3] = Tiles[Tiles.Count - 1];
                        }

                        #endregion
                        #region "Y"
                        if (down == 0 &&
                            id != 0)
                        {
                            Tiles.Add(new Horizontal(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k - 1, j, ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.HorizontalDown], .5f) { Side = Globals.Side.HorizontalDown });
                            CubeMap[AddBoundingBox(i, k, j)].Faces[4] = Tiles[Tiles.Count - 1];
                        }
                        if (up == 0 &&
                            id != 0)
                        {
                            Tiles.Add(new Horizontal(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k, j, ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.HorizontalUp], .5f) { Side = Globals.Side.HorizontalUp });
                            CubeMap[AddBoundingBox(i, k, j)].Faces[5] = Tiles[Tiles.Count - 1];
                        }
                        #endregion

                    }
                }
            }
        }

        public BoundingBox AddBoundingBox(int i, int k, int j)
        {
            Vector3 min = new Vector3(i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, k - 1, j + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z);
            Vector3 max = new Vector3(i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X + 1, k, j + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z + 1);
            BoundingBox bb = new BoundingBox(min, max);
            if (!BoundingBoxes.Contains(bb))
            {
                BoundingBoxes.Add(bb);
                CubeMap.Add(bb, new Cube() { Chunk = this });
            }
            return bb;
        }
        
        private void generateVertexBuffer()
        {

            offSet = 0;
            Vertices = new VPTVDeclaration[Tiles.Count * 4];

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

            VertexBuffer = new VertexBuffer(Globals.GraphicsDevice, typeof(VPTVDeclaration), Vertices.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData<VPTVDeclaration>(Vertices);

            VerticesCount = Tiles.Count;


        }

        private void editVertexBuffer(int listIndex)
        {
            int offSet = listIndex * 4 * VPTVDeclaration.vertexDeclaration.VertexStride;
            VertexBuffer.SetData<VPTVDeclaration>(offSet, Tiles[listIndex].Vertices, 0, 4, VPTVDeclaration.vertexDeclaration.VertexStride);

        }

        public IFlat GetFlatVec(Vector3 v)
        {
            return Tiles.Find(p => p.Vertices[0].Position.Equals(v));
        }

        public void RemoveCube(BoundingBox box)
        {
            BoundingBoxes.Remove(box);
            int i = (int)box.Min.X;
            int j = (int)box.Min.Z;
            int y = (int)box.Max.Y;

            ChunkManager.Cubes[y, i, j] = 0;
            var chunk = ChunkManager.GetChunkForPosition(new Vector3(i, y, j));

            if (chunk == null)
                return;

            var chunk0 = ChunkManager.GetChunkForPosition(new Vector3(i - 1, y, j));
            var chunk1 = ChunkManager.GetChunkForPosition(new Vector3(i + 1, y, j));
            var chunk2 = ChunkManager.GetChunkForPosition(new Vector3(i, y, j - 1));
            var chunk3 = ChunkManager.GetChunkForPosition(new Vector3(i, y, j + 1));

            if (!chunk.Equals(chunk0))
                chunk0?.ReBuildAsync();
            if (!chunk.Equals(chunk1))
                chunk1?.ReBuildAsync();
            if (!chunk.Equals(chunk2))
                chunk2?.ReBuildAsync();
            if (!chunk.Equals(chunk3))
                chunk3?.ReBuildAsync();

            chunk.ReBuildAsync();
        }

        public void EditFully(IFlat _old, IFlat _new)
        {
            int index = Tiles.IndexOf(_old);
            if (index == -1)
                return;

            Tiles[index] = _new;
            editVertexBuffer(index);
        }

        public void EditLight(IFlat of, float value)
        {
            int index = Tiles.IndexOf(of);
            if (index == -1)
                return;

            var flat = Tiles[index];

            flat.Vertices[0].Value = new Vector2(value);
            flat.Vertices[1].Value = new Vector2(value);
            flat.Vertices[2].Value = new Vector2(value);
            flat.Vertices[3].Value = new Vector2(value);

            Tiles[index] = flat;
            editVertexBuffer(index);

        }
        
        public void ReBuildAsync()
        {
            //Task.Run(() =>
            //{
            //    lock (SYNC_LOCK)
            //    {
            //        Generate();
            //    }
            //});

            Generate();
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

        public override string ToString()
        {
            return ChunkId + "";
        }

        public override bool Equals(object obj)
        {
            Chunk c = (Chunk)obj;
            return c != null && c.ChunkId == ChunkId;
        }
    }
}
