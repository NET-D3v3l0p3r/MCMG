using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using ShootCube.World.Chunk.Model;
using ShootCube.World.Chunk;
using static ShootCube.Global.Globals;

namespace ShootCube.Global.Picking
{
    public struct Profile
    {
        public ChunkEditable Chunk;
        public Cube Cube;
        public BoundingBox BoundingBox;
        public Face Face;
        public float Distance;


    }
}
