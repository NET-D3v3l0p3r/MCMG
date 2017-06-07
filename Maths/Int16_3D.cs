using System;

namespace ShootCube.Maths
{
    [Serializable]
    public struct Int16_3D
    {
        public ushort X;
        public ushort Y;
        public ushort Z;


        public Int16_3D(ushort x, ushort y, ushort z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}