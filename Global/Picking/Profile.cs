using Microsoft.Xna.Framework;
using ShootCube.World.Chunk.Model;

namespace ShootCube.Global.Picking
{
    public struct Profile
    {
        public Chunk Chunk;
        public Cube Cube;
        public BoundingBox BoundingBox;
        public float Distance;
    }
}