using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShootCube.Declaration;
using ShootCube.Global;
using ShootCube.Global.Picking;

namespace ShootCube.World.Chunk.Model
{
    public class Chunk
    {
        private static object _syncLock = new object();
        public Dictionary<BoundingBox, Cube> CubeMap = new Dictionary<BoundingBox, Cube>(); // FRAGILE !

        private int _offSet;

        public List<IFlat> Tiles = new List<IFlat>();

        public Chunk(Vector3 translation)
        {
            ChunkId = (short) Globals.Random.Next(0, short.MaxValue);
            LocalPosition = translation;

            BoundingBoxes = new List<BoundingBox>();


            ChunkBox = new BoundingBox(
                new Vector3(LocalPosition.X + ChunkManager.GlobalTranslation.X - 0.5f, 0,
                    LocalPosition.Z + ChunkManager.GlobalTranslation.Z - .5f),
                new Vector3(LocalPosition.X + ChunkManager.GlobalTranslation.X + Width - 0.5f, Height * 2,
                    LocalPosition.Z + ChunkManager.GlobalTranslation.Z + Depth - 0.5f));

            Neighbours = new Chunk[9];
        }

        public short ChunkId { get; }
        public Vector3 LocalPosition { get; set; }

        public static int Width { get; set; }
        public static int Depth { get; set; }
        public static int Height { get; set; }

        public BoundingBox ChunkBox { get; set; }
        public Chunk[] Neighbours { get; set; }

        public List<BoundingBox> BoundingBoxes { get; set; }

        public VertexBuffer VertexBuffer { get; set; }
        public VptvDeclaration[] Vertices { get; set; }

        public int VerticesCount { get; private set; }


        public void Generate()
        {
            var x = (int) (LocalPosition.X - ChunkManager.GlobalTranslation.X);
            var z = (int) (LocalPosition.Z - ChunkManager.GlobalTranslation.Z);
            x /= 16;
            z /= 16;

            if (x - 1 >= 0 && z - 1 >= 0)
                Neighbours[0] = ChunkManager.Chunks[x - 1 + (z - 1) * ChunkManager.Width];
            if (z - 1 >= 0)
                Neighbours[1] = ChunkManager.Chunks[x + (z - 1) * ChunkManager.Width];
            if (x + 1 < ChunkManager.Width && z - 1 >= 0)
                Neighbours[2] = ChunkManager.Chunks[x + 1 + (z - 1) * ChunkManager.Width];

            if (x - 1 >= 0)
                Neighbours[3] = ChunkManager.Chunks[x - 1 + z * ChunkManager.Width];
            Neighbours[4] = ChunkManager.Chunks[x + z * ChunkManager.Width];
            if (x + 1 < ChunkManager.Width)
                Neighbours[5] = ChunkManager.Chunks[x + 1 + z * ChunkManager.Width];

            if (x - 1 >= 0 && z + 1 < ChunkManager.Depth)
                Neighbours[6] = ChunkManager.Chunks[x - 1 + (z + 1) * ChunkManager.Width];
            if (z + 1 < ChunkManager.Depth)
                Neighbours[7] = ChunkManager.Chunks[x + (z + 1) * ChunkManager.Width];
            if (x + 1 < ChunkManager.Width && z + 1 < ChunkManager.Depth)
                Neighbours[8] = ChunkManager.Chunks[x + 1 + (z + 1) * ChunkManager.Width];

            Tiles.Clear();

            Analyze(0, 0, Width, Depth);

            GenerateVertexBuffer();
        }

        public void Analyze(int startX, int startZ, int width, int depth)
        {
            for (var j = startZ; j < startZ + depth; j++)
            for (var i = startX; i < startX + width; i++)
            for (var k = 0; k < Height; k++)
            {
                int id = ChunkManager.Cubes[k, i + (int) LocalPosition.X + (int) ChunkManager.GlobalTranslation.X,
                    j + (int) LocalPosition.Z + (int) ChunkManager.GlobalTranslation.Z];

                #region "Correspoding neighbours"

                byte back = 255;
                byte forward = 255;

                byte left = 255;
                byte right = 255;

                byte up = 255;
                byte down = 255;

                if (j - 1 + (int) LocalPosition.Z + (int) ChunkManager.GlobalTranslation.Z >= 0)
                    back = ChunkManager.Cubes[k, i + (int) LocalPosition.X + (int) ChunkManager.GlobalTranslation.X,
                        j - 1 + (int) LocalPosition.Z + (int) ChunkManager.GlobalTranslation.Z];
                if (j + 1 + (int) LocalPosition.Z + (int) ChunkManager.GlobalTranslation.Z <
                    Depth * ChunkManager.Depth)
                    forward = ChunkManager.Cubes[k, i + (int) LocalPosition.X + (int) ChunkManager.GlobalTranslation.X,
                        j + 1 + (int) LocalPosition.Z + (int) ChunkManager.GlobalTranslation.Z];

                if (i + 1 + (int) LocalPosition.X + (int) ChunkManager.GlobalTranslation.X <
                    Width * ChunkManager.Width)
                    right = ChunkManager.Cubes[k,
                        i + 1 + (int) LocalPosition.X + (int) ChunkManager.GlobalTranslation.X,
                        j + (int) LocalPosition.Z + (int) ChunkManager.GlobalTranslation.Z];
                if (i - 1 + (int) LocalPosition.X + (int) ChunkManager.GlobalTranslation.X >= 0)
                    left = ChunkManager.Cubes[k,
                        i - 1 + (int) LocalPosition.X + (int) ChunkManager.GlobalTranslation.X,
                        j + (int) LocalPosition.Z + (int) ChunkManager.GlobalTranslation.Z];

                if (k - 1 + (int) LocalPosition.Y + (int) ChunkManager.GlobalTranslation.Y >= 0)
                    down = ChunkManager.Cubes[k - 1,
                        i + (int) LocalPosition.X + (int) ChunkManager.GlobalTranslation.X,
                        j + (int) LocalPosition.Z + (int) ChunkManager.GlobalTranslation.Z];
                if (k + 1 + (int) LocalPosition.Y + (int) ChunkManager.GlobalTranslation.Y < Height)
                    up = ChunkManager.Cubes[k + 1, i + (int) LocalPosition.X + (int) ChunkManager.GlobalTranslation.X,
                        j + (int) LocalPosition.Z + (int) ChunkManager.GlobalTranslation.Z];

                #endregion


                #region "X"

                if (back == 0 &&
                    id != 0)
                {
                    Tiles.Add(new XFlat(LocalPosition + ChunkManager.GlobalTranslation, i, k - 1, j,
                        ChunkManager.CorrespondingSideIds[(byte) id][Globals.Side.XBackward], .5f)
                    {
                        Side = Globals.Side.XBackward
                    });
                    CubeMap[AddBoundingBox(i, k, j)].Faces[0] = Tiles[Tiles.Count - 1];
                }
                if (forward == 0 &&
                    id != 0)
                {
                    Tiles.Add(new XFlat(LocalPosition + ChunkManager.GlobalTranslation, i, k - 1, j + 1,
                        ChunkManager.CorrespondingSideIds[(byte) id][Globals.Side.XForward], .5f)
                    {
                        Side = Globals.Side.XForward
                    });
                    CubeMap[AddBoundingBox(i, k, j)].Faces[1] = Tiles[Tiles.Count - 1];
                }

                #endregion

                #region "Z"

                if (right == 0 &&
                    id != 0)
                {
                    Tiles.Add(new ZFlat(LocalPosition + ChunkManager.GlobalTranslation, i + 1, k - 1, j,
                        ChunkManager.CorrespondingSideIds[(byte) id][Globals.Side.ZRight], .5f)
                    {
                        Side = Globals.Side.ZRight
                    });
                    CubeMap[AddBoundingBox(i, k, j)].Faces[2] = Tiles[Tiles.Count - 1];
                }
                if (left == 0 &&
                    id != 0)
                {
                    Tiles.Add(new ZFlat(LocalPosition + ChunkManager.GlobalTranslation, i, k - 1, j,
                        ChunkManager.CorrespondingSideIds[(byte) id][Globals.Side.ZLeft], .5f)
                    {
                        Side = Globals.Side.ZLeft
                    });
                    CubeMap[AddBoundingBox(i, k, j)].Faces[3] = Tiles[Tiles.Count - 1];
                }

                #endregion

                #region "Y"

                if (down == 0 &&
                    id != 0)
                {
                    Tiles.Add(new Horizontal(LocalPosition + ChunkManager.GlobalTranslation, i, k - 1, j,
                        ChunkManager.CorrespondingSideIds[(byte) id][Globals.Side.HorizontalDown], .5f)
                    {
                        Side = Globals.Side.HorizontalDown
                    });
                    CubeMap[AddBoundingBox(i, k, j)].Faces[4] = Tiles[Tiles.Count - 1];
                }
                if (up == 0 &&
                    id != 0)
                {
                    Tiles.Add(new Horizontal(LocalPosition + ChunkManager.GlobalTranslation, i, k, j,
                        ChunkManager.CorrespondingSideIds[(byte) id][Globals.Side.HorizontalUp], .5f)
                    {
                        Side = Globals.Side.HorizontalUp
                    });
                    CubeMap[AddBoundingBox(i, k, j)].Faces[5] = Tiles[Tiles.Count - 1];
                }

                #endregion
            }
        }

        public BoundingBox AddBoundingBox(int i, int k, int j)
        {
            var min = new Vector3(i + (int) LocalPosition.X + (int) ChunkManager.GlobalTranslation.X, k - 1,
                j + (int) LocalPosition.Z + (int) ChunkManager.GlobalTranslation.Z);
            var max = new Vector3(i + (int) LocalPosition.X + (int) ChunkManager.GlobalTranslation.X + 1, k,
                j + (int) LocalPosition.Z + (int) ChunkManager.GlobalTranslation.Z + 1);
            var bb = new BoundingBox(min, max);
            if (!BoundingBoxes.Contains(bb))
            {
                BoundingBoxes.Add(bb);
                CubeMap.Add(bb, new Cube {Chunk = this});
            }
            return bb;
        }

        private void GenerateVertexBuffer()
        {
            _offSet = 0;
            Vertices = new VptvDeclaration[Tiles.Count * 4];

            for (var i = 0; i < Tiles.Count; i++)
            {
                var flat = Tiles[i];
                if (flat == null)
                    continue;
                for (var j = 0; j < flat.Vertices.Length; j++)
                    if (j + _offSet < Vertices.Length)
                        Vertices[j + _offSet] = flat.Vertices[j];
                _offSet += 4;
            }

            VertexBuffer = new VertexBuffer(Globals.GraphicsDevice, typeof(VptvDeclaration), Vertices.Length,
                BufferUsage.WriteOnly);
            VertexBuffer.SetData(Vertices);

            VerticesCount = Tiles.Count;
        }

        private void EditVertexBuffer(int listIndex)
        {
            var offSet = listIndex * 4 * VptvDeclaration.vertexDeclaration.VertexStride;
            VertexBuffer.SetData(offSet, Tiles[listIndex].Vertices, 0, 4,
                VptvDeclaration.vertexDeclaration.VertexStride);
        }

        public IFlat GetFlatVec(Vector3 v)
        {
            return Tiles.Find(p => p.Vertices[0].Position.Equals(v));
        }

        public void RemoveCube(BoundingBox box)
        {
            BoundingBoxes.Remove(box);
            var i = (int) box.Min.X;
            var j = (int) box.Min.Z;
            var y = (int) box.Max.Y;

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

        public void EditFully(IFlat old, IFlat _new)
        {
            var index = Tiles.IndexOf(old);
            if (index == -1)
                return;

            Tiles[index] = _new;
            EditVertexBuffer(index);
        }

        public void EditLight(IFlat of, float value)
        {
            var index = Tiles.IndexOf(of);
            if (index == -1)
                return;

            var flat = Tiles[index];

            flat.Vertices[0].Value = new Vector2(value);
            flat.Vertices[1].Value = new Vector2(value);
            flat.Vertices[2].Value = new Vector2(value);
            flat.Vertices[3].Value = new Vector2(value);

            Tiles[index] = flat;
            EditVertexBuffer(index);
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
            var c = (Chunk) obj;
            return c != null && c.ChunkId == ChunkId;
        }
    }
}