using Microsoft.Xna.Framework;
using ShootCube.World.Chunk;
using ShootCube.World.Chunk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootCube.Global.Picking
{
    public class  Cube
    {
        public IFlat[] Faces = new IFlat[6];
        public ChunkEditable Chunk;

        public static Cube LoadFromBoundingBox(BoundingBox bb)
        {
            Cube cube = new Cube();

            Vector3 min = bb.Min;
            Vector3 max = bb.Max;
            Vector3 initial = new Vector3(min.X, max.Y, min.Z);
            cube.Chunk = ChunkManager.GetChunkForPosition(initial);
            if (cube.Chunk == null)
                return null;

            if (cube.Chunk.CubeMap.Keys.ToList().Exists(p => p!= null && p.Min == min))
                return cube.Chunk.CubeMap[bb];
            else return null;


            //Vector3 min = bb.Min;
            //Vector3 max = bb.Max;

            //cube.Faces = new IFlat[6];
            //int counter = 0;

            //Vector3 initial = new Vector3(min.X, max.Y, min.Z);
            //cube.Chunk = ChunkManager.GetChunkForPosition(initial);

            //var a = ChunkManager.ExtractFlat(initial + new Vector3(0, -1, 0)).ToList();
            //for (int i = 0; i < a.Count; i++)
            //{
            //    cube.Faces[counter++] = a[i];
            //}

            //var b = ChunkManager.ExtractFlat(initial + new Vector3(1, -1, 0)).ToList();
            //for (int i = 0; i < b.Count; i++)
            //{
            //    if (b[i] is ZFlat)
            //        cube.Faces[counter++] = b[i];
            //}

            //var d = ChunkManager.ExtractFlat(initial + new Vector3(0, -1, 1)).ToList();
            //for (int i = 0; i < d.Count; i++)
            //{
            //    if (d[i] is XFlat)
            //        cube.Faces[counter++] = d[i];
            //}

            //var e = ChunkManager.ExtractFlat(initial + new Vector3(0, 0, 0)).ToList();
            //for (int i = 0; i < e.Count; i++)
            //{
            //    if (e[i] is Horizontal)
            //        cube.Faces[counter++] = e[i];
            //}

            //return cube;
        }
    }
}
