using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShootCube.Global;
using ShootCube.World.Chunk.Model;

namespace ShootCube.World.Chunk
{
    public class XFlat : IFlat
    {
        public VertexPositionNormalTexture[] Vertices { get; set; }
        public Vector2 TextureAtlasCoordinates { get; set; }

        public XFlat(Vector3 world, int x, int y, int z, byte id)
        {
            Vertices = new VertexPositionNormalTexture[4];


            Vertices[0] = new VertexPositionNormalTexture(world + new Vector3(x, y, z), new Vector3(0, 0, 1), new Vector2(0 + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + ChunkManager.RatioY));
            Vertices[1] = new VertexPositionNormalTexture(world + new Vector3(x + 1, y, z), new Vector3(0, 0, 1), new Vector2(ChunkManager.RatioX + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + ChunkManager.RatioY));
            Vertices[2] = new VertexPositionNormalTexture(world + new Vector3(x + 1, y + 1, z), new Vector3(0, 0, 1), new Vector2(ChunkManager.RatioX + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + 0));
            Vertices[3] = new VertexPositionNormalTexture(world + new Vector3(x, y + 1, z), new Vector3(0, 0, 1), new Vector2(0 + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + 0));

        }
    }
}
