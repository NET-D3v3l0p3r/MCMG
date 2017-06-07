using System.Linq;
using Microsoft.Xna.Framework;
using ShootCube.World.Chunk;
using ShootCube.World.Chunk.Model;

namespace ShootCube.Global.Picking
{
    public class Cube
    {
        public Chunk Chunk;
        public IFlat[] Faces = new IFlat[6];

        public static Cube LoadFromBoundingBox(BoundingBox bb)
        {
            var cube = new Cube();

            var min = bb.Min;
            var max = bb.Max;

            cube.Faces = new IFlat[6];
            var counter = 0;

            var initial = new Vector3(min.X, max.Y, min.Z);
            cube.Chunk = ChunkManager.GetChunkForPosition(initial);

            var a = ChunkManager.ExtractFlat(initial + new Vector3(0, -1, 0)).ToList();
            for (var i = 0; i < a.Count; i++)
                cube.Faces[counter++] = a[i];

            var b = ChunkManager.ExtractFlat(initial + new Vector3(1, -1, 0)).ToList();
            for (var i = 0; i < b.Count; i++)
                if (b[i] is ZFlat)
                    cube.Faces[counter++] = b[i];

            var d = ChunkManager.ExtractFlat(initial + new Vector3(0, -1, 1)).ToList();
            for (var i = 0; i < d.Count; i++)
                if (d[i] is XFlat)
                    cube.Faces[counter++] = d[i];

            var e = ChunkManager.ExtractFlat(initial + new Vector3(0, 0, 0)).ToList();
            for (var i = 0; i < e.Count; i++)
                if (e[i] is Horizontal)
                    cube.Faces[counter++] = e[i];

            return cube;
        }
    }
}