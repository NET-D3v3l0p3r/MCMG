using Microsoft.Xna.Framework;
using ShootCube.Declaration;
using ShootCube.Global;
using ShootCube.World.Chunk.Model;

namespace ShootCube.World.Chunk
{
    public class ZFlat : IFlat
    {
        public ZFlat(Vector3 world, int x, int y, int z, byte id, float value)
        {
            Vertices = new VptvDeclaration[4];

            Id = id;

            //Vertices[0] = new VertexPositionTexture(world + new Vector3(x, y, z), color);
            //Vertices[1] = new VertexPositionTexture(world + new Vector3(x, y, z + 1), color);
            //Vertices[2] = new VertexPositionTexture(world + new Vector3(x, y + 1, z + 1), color);
            //Vertices[3] = new VertexPositionTexture(world + new Vector3(x, y + 1, z), color);

            Vertices[0] = new VptvDeclaration(world + new Vector3(x, y, z),
                new Vector2(0 + ChunkManager.TextureAtlasCoordinates[id].X,
                    ChunkManager.TextureAtlasCoordinates[id].Y + ChunkManager.RatioY), value);
            Vertices[1] = new VptvDeclaration(world + new Vector3(x, y, z + 1),
                new Vector2(ChunkManager.RatioX + ChunkManager.TextureAtlasCoordinates[id].X,
                    ChunkManager.TextureAtlasCoordinates[id].Y + ChunkManager.RatioY), value);
            Vertices[2] = new VptvDeclaration(world + new Vector3(x, y + 1, z + 1),
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