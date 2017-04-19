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
    class ZFlat : IFlat
    {
        public VertexPositionNormalTexture[] Vertices { get; set; }
        public Vector2 TextureAtlasCoordinates { get; set; }

        public ZFlat(Vector3 world, int x, int y, int z, byte id)
        {
            Vertices = new VertexPositionNormalTexture[4];

            //Vertices[0] = new VertexPositionTexture(world + new Vector3(x, y, z), color);
            //Vertices[1] = new VertexPositionTexture(world + new Vector3(x, y, z + 1), color);
            //Vertices[2] = new VertexPositionTexture(world + new Vector3(x, y + 1, z + 1), color);
            //Vertices[3] = new VertexPositionTexture(world + new Vector3(x, y + 1, z), color);

            Vertices[0] = new VertexPositionNormalTexture(world + new Vector3(x, y, z), new Vector3(1, 0, 0), new Vector2(0 + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + ChunkManager.RatioY));
            Vertices[1] = new VertexPositionNormalTexture(world + new Vector3(x, y, z + 1), new Vector3(1, 0, 0), new Vector2(ChunkManager.RatioX + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + ChunkManager.RatioY));
            Vertices[2] = new VertexPositionNormalTexture(world + new Vector3(x, y + 1, z + 1), new Vector3(1, 0, 0), new Vector2(ChunkManager.RatioX + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + 0));
            Vertices[3] = new VertexPositionNormalTexture(world + new Vector3(x, y + 1, z), new Vector3(1, 0, 0), new Vector2(0 + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + 0));


        }
    }
}
