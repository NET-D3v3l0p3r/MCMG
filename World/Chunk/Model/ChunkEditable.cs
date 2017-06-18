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
using static ShootCube.Global.Globals;
using ShootCube.Global.Utilities;
using ShootCube.World.Structure.Cave;

namespace ShootCube.World.Chunk.Model
{
    /*
     * TODO: Re-implement chunk generation:
     * Renew just the area of edited cube, not whole chunk.
     * PRIORITY HIGH!
     * 
     */
    public class ChunkEditable
    {
        public short ChunkId { get; private set; }
        public Vector3 LocalPosition { get; set; }

        public static int Width { get; set; }
        public static int Depth { get; set; }
        public static int Height { get; set; }

        public BoundingBox ChunkBox { get; set; }
        public ChunkEditable[] Neighbours { get; set; }

        public List<BoundingBox> BoundingBoxes { get; set; }

        public VertexBuffer VertexBuffer { get; set; }
        public VPTVDeclaration[] Vertices { get; set; }

        public int VerticesCount { get; private set; }

        public List<IFlat> Tiles = new List<IFlat>();
        public Dictionary<BoundingBox, Cube> CubeMap = new Dictionary<BoundingBox, Cube>(); // FRAGILE !

        public bool IsUpdated { get; set; }

        private int offSet = 0;

        private static object SYNC_LOCK = new object();
        private static short NUMBER = 0;
        public ChunkEditable(Vector3 translation)
        {
            ChunkId = NUMBER++;
            LocalPosition = translation;

            BoundingBoxes = new List<BoundingBox>();


            ChunkBox = new BoundingBox(
                new Vector3(LocalPosition.X + ChunkManager.GLOBAL_TRANSLATION.X - 0.5f, 0, LocalPosition.Z + ChunkManager.GLOBAL_TRANSLATION.Z - .5f), new Vector3(LocalPosition.X + ChunkManager.GLOBAL_TRANSLATION.X + Width - 0.5f, Height * 2, LocalPosition.Z + ChunkManager.GLOBAL_TRANSLATION.Z + Depth - 0.5f));

            Neighbours = new ChunkEditable[9];

            int cavePossibility = Globals.Random.Next(0, 50);
            if (cavePossibility > 15 && cavePossibility <= 35)
                CaveGenerator.Nodes.Add(new Node(new Vector3(Globals.Random.Next(0, Width), Globals.Random.Next(Height - (90*2), Height), Globals.Random.Next(0, Depth)) + LocalPosition, Globals.Random.Next(5, 15)));
        }

        #region "FIRST START"
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

            // THIS NEEDS TO BE REMOVED!
            //Tiles.Clear();
            //CubeMap.Clear();
            //BoundingBoxes.Clear();

            Parse(0, 0, Width, Depth);
            generateVertexBuffer();
        }
        public void Parse(int startX, int startZ, int width, int depth)
        {
            for (int j = startZ; j < startZ + depth; j++)
            {
                for (int i = startX; i < startX + width; i++)
                {
                    for (int k = 0; k < Height; k++)
                    {
                        int id = ChunkManager.Cubes[k, i + (int)LocalPosition.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, (j) + (int)LocalPosition.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z];
                        if (id == 0)
                            continue;

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
                            Tiles.Add(new XFlat(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k - 1, j, ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.XBackward], 1.0f) { Side = Globals.Side.XBackward });
                            CubeMap[AddBoundingBox(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k, j)].Faces[0] = Tiles[Tiles.Count - 1];


                        }
                        if (forward == 0 &&
                           id != 0)
                        {
                            Tiles.Add(new XFlat(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k - 1, j + 1, ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.XForward], 1.0f) { Side = Globals.Side.XForward });
                            CubeMap[AddBoundingBox(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k, j)].Faces[1] = Tiles[Tiles.Count - 1];
                        }
                        #endregion
                        #region "Z"
                        if (right == 0 &&
                           id != 0)
                        {
                            Tiles.Add(new ZFlat(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i + 1, k - 1, j,
                                ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.ZRight], 1.0f)
                            { Side = Globals.Side.ZRight });
                            CubeMap[AddBoundingBox(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k, j)].Faces[2] = Tiles[Tiles.Count - 1];
                        }
                        if (left == 0 &&
                           id != 0)
                        {
                            Tiles.Add(new ZFlat(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k - 1, j,
                                ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.ZLeft], 1.0f)
                            { Side = Globals.Side.ZLeft });
                            CubeMap[AddBoundingBox(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k, j)].Faces[3] = Tiles[Tiles.Count - 1];
                        }

                        #endregion
                        #region "Y"
                        if (down == 0 &&
                            id != 0)
                        {
                            Tiles.Add(new Horizontal(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k - 1, j, ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.HorizontalDown], 1.0f) { Side = Globals.Side.HorizontalDown });
                            CubeMap[AddBoundingBox(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k, j)].Faces[4] = Tiles[Tiles.Count - 1];
                        }
                        if (up == 0 &&
                            id != 0)
                        {
                            Tiles.Add(new Horizontal(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k, j, ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.HorizontalUp], 1.0f) { Side = Globals.Side.HorizontalUp });
                            CubeMap[AddBoundingBox(LocalPosition + ChunkManager.GLOBAL_TRANSLATION, i, k, j)].Faces[5] = Tiles[Tiles.Count - 1];
                        }
                        #endregion

                    }
                }
            }
        }

        public BoundingBox AddBoundingBox(Vector3 translation, int i, int k, int j)
        {
            var bb = GetBoundingBox(translation, i, k, j);
            if (!BoundingBoxes.Contains(bb))
            {
                BoundingBoxes.Add(bb);
                CubeMap.Add(bb, new Cube());
            }
            return bb;
        }
        public static BoundingBox GetBoundingBox(Vector3 translation, int i, int k, int j)
        {
            Vector3 min = new Vector3(i + (int)translation.X + (int)ChunkManager.GLOBAL_TRANSLATION.X, k - 1, j + (int)translation.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z);
            Vector3 max = new Vector3(i + (int)translation.X + (int)ChunkManager.GLOBAL_TRANSLATION.X + 1, k, j + (int)translation.Z + (int)ChunkManager.GLOBAL_TRANSLATION.Z + 1);
            return new BoundingBox(min, max);
        }
        #endregion
        #region "VERTEX BUFFERS"
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
        public void EditMoisture(IFlat of, float value)
        {
            int index = Tiles.IndexOf(of);
            Vector2[] animationTextureCoords = Globals.GetTextureCoordinate(7);

            of.Vertices[0].AnimationCoordinate = animationTextureCoords[0];
            of.Vertices[1].AnimationCoordinate = animationTextureCoords[1];
            of.Vertices[2].AnimationCoordinate = animationTextureCoords[2];
            of.Vertices[3].AnimationCoordinate = animationTextureCoords[3];

            Tiles[index] = of;

            editVertexBuffer(index);
            EditLight(of, value);
        }

        #endregion
        #region "MISCELLANEOUS"
        public IFlat GetFlatVec(Vector3 v)
        {
            return Tiles.Find(p => p.Vertices[0].Position.Equals(v));
        }
        #endregion

        #region "NEEDS UPDATE"


        public void DestroyCube(Profile profile, WeaponStatistics.Utilities utility)
        {
            byte damage = WeaponStatistics.UtilityDamageRelation[utility];

            Cube cube = profile.Cube;

            int x = (int)profile.BoundingBox.Min.X;
            int z1 = (int)profile.BoundingBox.Min.Z;
            int y = (int)profile.BoundingBox.Max.Y;

            if (ChunkManager.Cubes[y, x, z1] == 8)
                return;
            if (cube == null)
                return;

            for (int i = 0; i < cube.Faces.Length; i++)
            {
                if (cube.Faces[i] == null)
                    continue;

                var flat = cube.Faces[i];
                var new_flat = flat;
                new_flat.Life -= damage;


                // calc damage

                double percents = (double)new_flat.Life / (50.0 * 2.0);

                int counter = 9;
                for (double k = .8; k >= .0; k -= .2)
                {
                    if (percents < k + .2 && percents >= k)
                    {
                        var tex = Globals.GetTextureCoordinate((byte)counter);
                        for (int z = 0; z < tex.Length; z++)
                        {
                            new_flat.Vertices[z].AnimationCoordinate = tex[z];
                        }
                    }
                    counter++;
                }

                EditFully(flat, new_flat);

                if (new_flat.Life <= 0)
                {
                    RemoveCube(profile.BoundingBox);
                    return;
                }

            }
        }


        /*
         * ALGORITHM: 
         * CREATE BOUNDINGBOXES OF NEIGHBOURS
         * ANALYZE 3x3x3 ARRAY WHETHER DELETE OR ADD FLAT
         *        => ADD: LOOK WHICH SIDE, ADD TO CUBEMAP, ADD TO TILES
         * 
         * DELETE ALL SIDES CONTAINED IN CUBE PROFILE FROM TILES
         * DELETE CUBE FROM BOUNDINGBOX AND CUBEMAP
         * REGENERATE VERTEXBUFFER ( CAN ALSO BE OPTIMIZED !)
         */

        #region "Remove"
        public void RemoveCube(BoundingBox box)
        {
            IsUpdated = true;
            Vector3 min = box.Min;
            Vector3 max = box.Max;

            int i = (int)min.X;
            int j = (int)min.Z;
            int k = (int)max.Y;

            ChunkManager.Cubes[k, i, j] = 0;

            Cube corespondingCube = CubeMap[box];

            foreach (var face in corespondingCube.Faces)
                if (face != null)
                    Tiles.Remove(face);

            CubeMap.Remove(box);
            BoundingBoxes.Remove(box);

            internalRemove(i, j, k);

            generateVertexBuffer();

        }
        private void internalRemove(int i, int j, int k)
        {
            internalRemoveCube(i, j + 1, k, Orientation.South);
            internalRemoveCube(i, j - 1, k, Orientation.North);

            internalRemoveCube(i + 1, j, k, Orientation.West);
            internalRemoveCube(i - 1, j, k, Orientation.East);

            internalRemoveCube(i, j, k + 1, Orientation.Down);
            internalRemoveCube(i, j, k - 1, Orientation.Up);


        }
        public void internalRemoveCube(int i, int j, int k, Orientation orientation)
        {

            if (i < 0)
                return;
            if (i >= Width * ChunkManager.Width)
                return;

            if (j < 0)
                return;
            if (j >= Depth * ChunkManager.Depth)
                return;

            var chunk = ChunkManager.GetChunkForPosition(new Vector3(i, k, j));

            if (ChunkId != chunk.ChunkId)
            {
                chunk.internalRemoveCube(i, j, k, orientation);
                chunk.generateVertexBuffer();
                return;
            }

            int id = ChunkManager.Cubes[k, i, j];

            if (id == 0)
                return;

            BoundingBox bb = new BoundingBox();
            bb = AddBoundingBox(Vector3.Zero, i, k, j);
            var cube = CubeMap[bb];
            var flat = cube.Faces.ToList().Find(p => p != null);

            var coord0 = new Vector2(-1f);
            var coord1 = new Vector2(-1f);
            var coord2 = new Vector2(-1f);
            var coord3 = new Vector2(-1f);

            byte life0 = 50 * 2;

            if (flat != null)
            {
                coord0 = flat.Vertices[0].AnimationCoordinate;
                coord1 = flat.Vertices[1].AnimationCoordinate;
                coord2 = flat.Vertices[2].AnimationCoordinate;
                coord3 = flat.Vertices[3].AnimationCoordinate;

                life0 = flat.Life;
            }

            switch (orientation)
            {
                case Orientation.Up:

                    Tiles.Add(new Horizontal(Vector3.Zero, i, k, j, ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.HorizontalUp], 1.0f,
                        coord0.X, coord0.Y,
                        coord1.X, coord1.Y,
                        coord2.X, coord2.Y,
                        coord3.X, coord3.Y)
                    {
                        Side = Side.HorizontalUp,
                        Life = life0
                    });
                    bb = AddBoundingBox(Vector3.Zero, i, k, j);
                    CubeMap[bb].Faces[5] = Tiles[Tiles.Count - 1];

                    break;

                case Orientation.Down:

                    Tiles.Add(new Horizontal(Vector3.Zero, i, k - 1, j, ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.HorizontalDown], 1.0f,
                        coord0.X, coord0.Y,
                        coord1.X, coord1.Y,
                        coord2.X, coord2.Y,
                        coord3.X, coord3.Y)
                    {
                        Side = Side.HorizontalDown,
                        Life = life0
                    });


                    CubeMap[bb].Faces[4] = Tiles[Tiles.Count - 1];
                    break;

                case Orientation.North:

                    Tiles.Add(new XFlat(Vector3.Zero, i, k - 1, j + 1, ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.XForward], 1.0f,
                        coord0.X, coord0.Y,
                        coord1.X, coord1.Y,
                        coord2.X, coord2.Y,
                        coord3.X, coord3.Y)
                    {
                        Side = Side.XForward,
                        Life = life0
                    });
                    bb = AddBoundingBox(Vector3.Zero, i, k, j);
                    CubeMap[bb].Faces[1] = Tiles[Tiles.Count - 1];
                    break;

                case Orientation.South:

                    Tiles.Add(new XFlat(Vector3.Zero, i, k - 1, j, ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.XBackward], 1.0f,
                        coord0.X, coord0.Y,
                        coord1.X, coord1.Y,
                        coord2.X, coord2.Y,
                        coord3.X, coord3.Y)
                    {
                        Side = Side.XBackward,
                        Life = life0
                    });
                    bb = AddBoundingBox(Vector3.Zero, i, k, j);
                    CubeMap[bb].Faces[0] = Tiles[Tiles.Count - 1];
                    break;

                case Orientation.West:
                    Tiles.Add(new ZFlat(Vector3.Zero, i, k - 1, j, ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.ZLeft], 1.0f,
                        coord0.X, coord0.Y,
                        coord1.X, coord1.Y,
                        coord2.X, coord2.Y,
                        coord3.X, coord3.Y)
                    {
                        Side = Side.ZLeft,
                        Life = life0
                    });
                    bb = AddBoundingBox(Vector3.Zero, i, k, j);
                    CubeMap[bb].Faces[3] = Tiles[Tiles.Count - 1];
                    break;

                case Orientation.East:
                    Tiles.Add(new ZFlat(Vector3.Zero, i + 1, k - 1, j, ChunkManager.CorrespondingSideIds[(byte)id][Globals.Side.ZRight], 1.0f,
                        coord0.X, coord0.Y,
                        coord1.X, coord1.Y,
                        coord2.X, coord2.Y,
                        coord3.X, coord3.Y)
                    {
                        Side = Side.ZRight,
                        Life = life0
                    });
                    bb = AddBoundingBox(Vector3.Zero, i, k, j);
                    CubeMap[bb].Faces[2] = Tiles[Tiles.Count - 1];
                    break;
            }

        }
        #endregion


        public void PlaceCube(BoundingBox box, Face face, byte id = 8)
        {
            int i = (int)box.Min.X;
            int j = (int)box.Min.Z;
            int y = (int)box.Max.Y;

            switch (face)
            {
                case Face.Top:
                    ChunkManager.Cubes[y + 1, i, j] = id;
                    break;

                case Face.Bottom:
                    ChunkManager.Cubes[y - 1, i, j] = id;
                    break;

                case Face.Left:
                    ChunkManager.Cubes[y, i - 1, j] = id;
                    break;

                case Face.Right:
                    ChunkManager.Cubes[y, i + 1, j] = id;
                    break;

                case Face.Back:
                    ChunkManager.Cubes[y, i, j - 1] = id;
                    break;

                case Face.Front:
                    ChunkManager.Cubes[y, i, j + 1] = id;
                    break;
            }


        }

        #endregion

        #region "MECHANICS"
        public void Update(GameTime gTime)
        {


        }
        public void Render()
        {

            if (Camera.ViewFrustum == null)
                return;
            if (Camera.ViewFrustum.Contains(ChunkBox) != ContainmentType.Intersects)
                return;

            if (VertexBuffer == null)
                return;

            Globals.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            Globals.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VerticesCount * 2);
        }
        #endregion
        #region "OVERRIDES"
        public override string ToString()
        {
            return ChunkId + "";
        }
        public override bool Equals(object obj)
        {
            ChunkEditable c = (ChunkEditable)obj;
            return c != null && c.ChunkId == ChunkId;
        }
        public override int GetHashCode()
        {
            return ChunkId;
        }
        #endregion
    }
}
