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
    public class Horizontal : IFlat
    {
        public VPTVDeclaration[] Vertices { get; set; }
        public byte Id { get; set; }
        public byte Life { get; set; }

        public Globals.Side Side { get; set; }


        public Horizontal(Vector3 world, int x, int y, int z, byte id, float value, 
            float _x0 = -1, float _y0 = -1, 
            float _x1 = -1, float _y1 = -1, 
            float _x2 = -1, float _y2 = -1, 
            float _x3 = -1, float _y3 = -1)
        {
            Vertices = new VPTVDeclaration[4];

            Id = id;
            Life = 50 * 2;

            Vertices[0] = new VPTVDeclaration(world + new Vector3(x, y, z), 
                new Vector2(0 + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + ChunkManager.RatioY), 
                new Vector2(_x0, _y0), value);
            Vertices[1] = new VPTVDeclaration(world + new Vector3(x + 1, y, z), 
                new Vector2(ChunkManager.RatioX + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + ChunkManager.RatioY), 
                new Vector2(_x1, _y1), value);
            Vertices[2] = new VPTVDeclaration(world + new Vector3(x + 1, y, z + 1), 
                new Vector2(ChunkManager.RatioX + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + 0), 
                new Vector2(_x2, _y2), value);
            Vertices[3] = new VPTVDeclaration(world + new Vector3(x, y, z + 1), 
                new Vector2(0 + ChunkManager.TextureAtlasCoordinates[id].X, ChunkManager.TextureAtlasCoordinates[id].Y + 0), 
                new Vector2(_x3, _y3), value);


        }
    }
}
