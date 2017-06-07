using Microsoft.Xna.Framework;
using ShootCube.Declaration;
using static ShootCube.Global.Globals;

namespace ShootCube.World.Chunk
{
    public interface IFlat
    {
        VptvDeclaration[] Vertices { get; set; }
        Vector2 TextureAtlasCoordinates { get; set; }
        byte Id { get; set; }

        Side Side { get; set; }
    }
}