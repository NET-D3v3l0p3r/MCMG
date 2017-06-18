using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using ShootCube.Maths;
using ShootCube.World.Chunk.Model;

namespace ShootCube.World.Structure.Cave
{
    public class Value
    {
        public Int32_3D ArrayPosition { get; set; }

        public Value(Int32_3D position)
        {
            ArrayPosition = position;
        }

        public void MakeAir()
        {
            ChunkManager.Cubes[ArrayPosition.Y, ArrayPosition.X, ArrayPosition.Z] = 0;
        }
    }
}
