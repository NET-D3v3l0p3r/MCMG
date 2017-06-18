using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootCube.Maths
{
    [Serializable]
    public struct Int32_3D
    {
        public int X;
        public int Y;
        public int Z;


        public Int32_3D(float x, float y, float z)
            : this()
        {
            X = (int)x;
            Y = (int)y;
            Z = (int)z;
        }

        public Int32_3D(Vector3 position)
         : this()
        {
            X = (int)position.X;
            Y = (int)position.Y;
            Z = (int)position.Z;
        }

    }
}
