using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShootCube.MapGen;
using ShootCube.Global;
using static ShootCube.Global.Globals;
using ShootCube.Global.Picking;

namespace ShootCube.World.Chunk.Model
{
    public class ChunkManager
    {

        public static Vector3 GLOBAL_TRANSLATION { get; set; }

        public static int Width { get; set; }
        public static int Depth { get; set; }

        public static Chunk[] Chunks { get; set; }
        public static byte[,,] Cubes { get; set; }
 


        public static SimplexNoiseGenerator SimplexNoise { get; private set; }

        public static Chunk CurrentChunk { get; private set; }

        #region "TextureAtlas"

        public static Texture2D TextureAtlas { get; set; }
        public static Dictionary<byte, Vector2> TextureAtlasCoordinates { get; set; }


        public static byte TileWidth, TileHeight;
        public static float RatioX, RatioY;

        public static Dictionary<byte, Dictionary<Side, byte>> CorrespondingSideIds { get; set; }

        #endregion


        public ChunkManager(int w, int h)
        {
            Width = w;
            Depth = h;

            Chunk.Width = 16;
            Chunk.Depth = 16;
            Chunk.Height = 55 + 55;

            Chunks = new Chunk[Width * Depth];

            Cubes = new byte[Chunk.Height, Width * Chunk.Width, Depth * Chunk.Depth];
 

            TextureAtlas = Global.Globals.Content.Load<Texture2D>("atlas");
            TextureAtlasCoordinates = new Dictionary<byte, Vector2>();

            TileWidth = 64;
            TileHeight = 64;

            RatioX = (float)TileWidth / (float)TextureAtlas.Width;
            RatioY = (float)TileHeight / (float)TextureAtlas.Height;

            CorrespondingSideIds = new Dictionary<byte, Dictionary<Side, byte>>();

            byte counter = 0;
            for (int j = 0; j < TextureAtlas.Height; j += TileHeight)
            {
                for (int i = 1; i < TextureAtlas.Width; i += TileWidth)
                {
                    TextureAtlasCoordinates.Add(counter++, new Vector2((i / TileWidth) * RatioX, (j / TileHeight) * RatioY));
                }
            }


            AddTexture(1, 1, 5, 2, 2, 2, 2);
            AddTexture(5, 5, 5, 5, 5, 5, 5);
            AddTexture(6, 6, 6, 6, 6, 6, 6);
            AddTexture(4, 4, 4, 4, 4, 4, 4);

        }

        private static void AddTexture(byte id, byte up, byte down, byte left, byte right, byte forward, byte backward)
        {
            CorrespondingSideIds.Add(id, new Dictionary<Side, byte>());
            CorrespondingSideIds[id].Add(Side.HorizontalUp, up);
            CorrespondingSideIds[id].Add(Side.HorizontalDown, down);
            CorrespondingSideIds[id].Add(Side.ZLeft, left);
            CorrespondingSideIds[id].Add(Side.ZRight, right);
            CorrespondingSideIds[id].Add(Side.XForward, forward);
            CorrespondingSideIds[id].Add(Side.XBackward, backward);
        }



        public static void Start()
        {
            SimplexNoise = new SimplexNoiseGenerator(0, 1.0f / 512.0f, 1.0f / 512.0f, 1.0f / 512.0f, 1.0f / 512.0f)
            {
                Factor = 55 - 55 ,
                Sealevel = 55,
                Octaves = 4
            };

            for (int j = 0; j < Depth; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    Chunk chunk = new Chunk(new Vector3(i * 16, 0, j * 16));
                    Chunks[i + j * Width] = chunk;
                }
            }
            for (int i = 0; i < Chunks.Length; i++)
            {
                int x = (int)Chunks[i].LocalPosition.X + (int)GLOBAL_TRANSLATION.X;
                int z = (int)Chunks[i].LocalPosition.Z + (int)GLOBAL_TRANSLATION.Z;
                for (int q = 0; q < Chunk.Width; q++)
                {
                    for (int p = 0; p < Chunk.Depth; p++)
                    {
                        int height = (int)SimplexNoise.GetNoise2D(x + q, z + p);
                        for (int y = 0; y <= height; y++)
                        {
                            if (y == height)
                                Cubes[y, x + q, z + p] = 1;
                            else if (y + 3 < height)
                                Cubes[y, x + q, z + p] = Globals.Random.NextDouble() > 0.5 ? (byte)6 : Globals.Random.NextDouble() > 0.35 ? (byte)6 : (byte)5;
                            else
                                Cubes[y, x + q, z + p] = 5;


                        }

       
                    }
                }

            }
        }

        public static void Run()
        {
            for (int i = 0; i < Chunks.Length; i++)
            {
                if (Chunks[i] != null)
                    Chunks[i].Generate();
            }
        }

        public static void Update(GameTime gTime)
        {
            for (int i = 0; i < Chunks.Length; i++)
            {
                if (Chunks[i].ChunkBox.Contains(Camera.CameraPosition) == ContainmentType.Contains)
                {
                    CurrentChunk = Chunks[i];
                    break;
                }
            }

        }

        public static Profile? Pick(float maxLength)
        {
            var cubes = getAllFocused(maxLength).OrderBy(x => x.Distance).ToArray();
            if(cubes.Length != 0)
                return cubes[0];
            
            return null;
        }
        private static IEnumerable<Profile> getAllFocused(float maxLength)
        {
            foreach (var chunk in CurrentChunk.Neighbours)
            {
                if (chunk == null)
                    continue;
                for (int i = 0; i < chunk.BoundingBoxes.Count; i++)
                {
                    var bb = chunk.BoundingBoxes[i];
                    var result = Camera.MouseRay.Intersects(bb);
                    if (result.HasValue)
                    {
                        float distance = result.Value;
                        if (distance < maxLength)
                        {
                            Profile p = new Profile()
                            {
                                Chunk = chunk,
                                BoundingBox = bb,
                                Cube = Cube.LoadFromBoundingBox(bb),
                                Distance = distance
                            };

                            yield return p;
                        }
                    }
                }
            }
        }
        public static List<IFlat> ExtractFlat(Vector3 position)
        {
            Chunk chunk = GetChunkForPosition(position);
            if (chunk == null)
                return null;

            return chunk.Tiles.FindAll(p => p.Vertices[0].Position == position);
        }

        public static IEnumerable<Cube> GetCubeByHit(BoundingBox hit)
        {
            foreach (var chunk in CurrentChunk.Neighbours)
            {
                if (chunk == null)
                    continue;
                for (int i = 0; i < chunk.BoundingBoxes.Count; i++)
                {
                    BoundingBox bb = chunk.BoundingBoxes[i];
                    if (hit.Intersects(bb))
                        yield return Cube.LoadFromBoundingBox(bb);
                }
            }
        }

        public static Chunk GetChunkForPosition(Vector3 position)
        {
            foreach (var chunk in CurrentChunk.Neighbours)
            {
                if (chunk == null)
                    continue;
                var bb = chunk.ChunkBox;
                if (bb.Contains(position) == ContainmentType.Contains)
                    return chunk;
            }
            return null;
        }
        public static void Render()
        {
            for (int i = 0; i < Chunks.Length; i++)
            {
                if (Chunks[i] != null)
                    Chunks[i].Render();
            }
        }

    }
}
