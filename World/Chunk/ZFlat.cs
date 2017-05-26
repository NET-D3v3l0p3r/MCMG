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
using ShootCube.Declaration;

namespace ShootCube.World.Chunk
{
    public class ZFlat : IFlat
    {
        public VPTVDeclaration[] Vertices { get; set; }
        public Vector2 TextureAtlasCoordinates { get; set; }
        public byte Id { get; set; }

        public Globals.Side Side { get; set; }


        public ZFlat(Vector3 world, int x, int y, int z, byte id, float value)
        {
            Vertices = new VPTVDeclaration[4];

            Id = id;

            //Vertices[0] = new VertexPositionTexture(world + new Vector3(x, y, z), color);
            //Vertices[1] = new VertexPositionTexture(world + new Vector3(x, y, z + 1), color);
            //Vertices[2] = new VertexPositionTexture(world + new Vector3(x, y + 1, z + 1), color);
            //Vertices[3] = new VertexPositionTexture(world + new Vector3(x, y + 1, z), color);

            Vertices[0] = new VPTVDeclaration(world + new Vector3(x, y, z), new Vector2(0 + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + ChunkManager.RatioY), value);
            Vertices[1] = new VPTVDeclaration(world + new Vector3(x, y, z + 1), new Vector2(ChunkManager.RatioX + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + ChunkManager.RatioY), value);
            Vertices[2] = new VPTVDeclaration(world + new Vector3(x, y + 1, z + 1), new Vector2(ChunkManager.RatioX + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + 0), value);
            Vertices[3] = new VPTVDeclaration(world + new Vector3(x, y + 1, z), new Vector2(0 + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + 0), value);


        }
    }
}
