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
    public class XFlat : IFlat
    {
        public VPTVDeclaration[] Vertices { get; set; }
        public Vector2 TextureAtlasCoordinates { get; set; }
        public byte Id { get; set; }

        public Globals.Side Side { get; set; }

        public XFlat(Vector3 world, int x, int y, int z, byte id, float value)
        {
            Vertices = new VPTVDeclaration[4];
            Id = id;

            Vertices[0] = new VPTVDeclaration(world + new Vector3(x, y, z), new Vector2(0 + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + ChunkManager.RatioY), value);
            Vertices[1] = new VPTVDeclaration(world + new Vector3(x + 1, y, z), new Vector2(ChunkManager.RatioX + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + ChunkManager.RatioY), value);
            Vertices[2] = new VPTVDeclaration(world + new Vector3(x + 1, y + 1, z), new Vector2(ChunkManager.RatioX + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + 0), value);
            Vertices[3] = new VPTVDeclaration(world + new Vector3(x, y + 1, z), new Vector2(0 + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + 0), value);

        }
    }
}
