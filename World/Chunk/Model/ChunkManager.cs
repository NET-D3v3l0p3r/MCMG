using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShootCube.MapGen;

namespace ShootCube.World.Chunk.Model
{
    public class ChunkManager
    {

        public static Vector3 GLOBAL_TRANSLATION { get; set; }

        public static int Width { get; set; }
        public static int Depth { get; set; }

        public static Chunk[] Chunks { get; set; }
        public static byte[,,] CubeMap { get; set; }

        public static SimplexNoiseGenerator SimplexNoise { get; private set; }

        #region "TextureAtlas"

        public static Texture2D TextureAtlas { get; set; }
        public static Dictionary<byte, Vector2> TextureAtlasCoordinates { get; set; }

        public static byte TileWidth, TileHeight;
        public static float RatioX, RatioY;

        #endregion

        public ChunkManager(int w, int h)
        {
            Width = w;
            Depth = h;

            Chunk.Width = 16;
            Chunk.Depth = 16;
            Chunk.Height = 55 + 55;

            Chunks = new Chunk[Width * Depth];

            CubeMap = new byte[Chunk.Height, Width * Chunk.Width, Depth * Chunk.Depth];

            TextureAtlas = Global.Globals.Content.Load<Texture2D>("atlas");
            TextureAtlasCoordinates = new Dictionary<byte, Vector2>();

            TileWidth = 64;
            TileHeight = 64;

            RatioX = (float)TileWidth / (float)TextureAtlas.Width;
            RatioY = (float)TileHeight / (float)TextureAtlas.Height;

            byte counter = 0;
            for (int j = 0; j < TextureAtlas.Height; j += TileHeight)
            {
                for (int i = 1; i < TextureAtlas.Width; i += TileWidth)
                {
                    TextureAtlasCoordinates.Add(counter++, new Vector2((i / TileWidth) * RatioX, (j / TileHeight) * RatioY));
                }
            }

        }



        public static void Start()
        {
            SimplexNoise = new SimplexNoiseGenerator(0, 1.0f / 512.0f, 1.0f / 512.0f, 1.0f / 512.0f, 1.0f / 512.0f)
            {
                Factor = 55,
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
                                CubeMap[y, x + q, z + p] = 1;
                            else CubeMap[y, x + q, z + p] = 4;
                        }
                        
                    }
                }

            }

            for (int i = 0; i < Chunks.Length; i++)
            {
                if (Chunks[i] != null)
                    Chunks[i].Generate();
            }
       
        }

        public static void Update(GameTime gTime)
        {
            Parallel.For(0, Chunks.Length, (i) =>
            {
                if (Chunks[i] != null)
                    Chunks[i].Update(gTime);
            });
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
