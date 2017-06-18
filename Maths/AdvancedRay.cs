using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ShootCube.Maths
{
    public class AdvancedRay
    {
        public Vector3 Start { get; set; }
        public Vector3 Direction { get; set; }

        public float InitiatedLambda { get; private set; }

        public AdvancedRay(Vector3 a, Vector3 d)
        {
            Start = a;
            Direction = d;

            InitiatedLambda = d.Length();

            Direction /= Direction.Length();
        }

        public Vector3 Move(float lambda)
        {
            return Start + lambda * Direction;
        }
    }
}
