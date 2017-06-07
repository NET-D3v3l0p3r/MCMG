using Microsoft.Xna.Framework;
using ShootCube.Declaration;
using ShootCube.Global;
using ShootCube.World.Chunk.Model;

namespace ShootCube.World.Chunk
{
    public class XFlat : IFlat
    {
        public XFlat(Vector3 world, int x, int y, int z, byte id, float value)
        {
            Vertices = new VptvDeclaration[4];
            Id = id;

            Vertices[0] = new VptvDeclaration(world + new Vector3(x, y, z),
                new Vector2(0 + ChunkManager.TextureAtlasCoordinates[id].X,
                    ChunkManager.TextureAtlasCoordinates[id].Y + ChunkManager.RatioY), value);
            Vertices[1] = new VptvDeclaration(world + new Vector3(x + 1, y, z),
                new Vector2(ChunkManager.RatioX + ChunkManager.TextureAtlasCoordinates[id].X,
                    ChunkManager.TextureAtlasCoordinates[id].Y + ChunkManager.RatioY), value);
            Vertices[2] = new VptvDeclaration(world + new Vector3(x + 1, y + 1, z),
                new Vector2(ChunkManager.RatioX + ChunkManager.TextureAtlasCoordinates[id].X,
                    ChunkManager.TextureAtlasCoordinates[id].Y + 0), value);
            Vertices[3] = new VptvDeclaration(world + new Vector3(x, y + 1, z),
                new Vector2(0 + ChunkManager.TextureAtlasCoordinates[id].X,
                    ChunkManager.TextureAtlasCoordinates[id].Y + 0), value);
        }

        public VptvDeclaration[] Vertices { get; set; }
        public Vector2 TextureAtlasCoordinates { get; set; }
        public byte Id { get; set; }

        public Globals.Side Side { get; set; }
    }
}