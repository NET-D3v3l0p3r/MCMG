using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using ShootCube.Global;
using static ShootCube.Global.Globals;

namespace ShootCube.World.Chunk
{
    public interface IFlat
    {
        VertexPositionNormalTexture[] Vertices { get; set; }
        Vector2 TextureAtlasCoordinates { get; set; }
    }
}
