using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootCube.Maths
{
    [Serializable]
    public struct Int16_3D
    {
        public UInt16 X;
        public UInt16 Y;
        public UInt16 Z;


        public Int16_3D(UInt16 x, UInt16 y, UInt16 z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

    }
}
