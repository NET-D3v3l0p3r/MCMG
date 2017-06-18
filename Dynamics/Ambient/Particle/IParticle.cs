using Microsoft.Xna.Framework;
using ShootCube.World.Chunk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootCube.Dynamics.Ambient.Particle
{
    public interface IParticle
    {
        XFlat Particle { get; set; }

        void LetFall(float value);
        void Reset();
    }
}
