using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShootCube.World.Chunk;
using ShootCube.Global;
using ShootCube.World.Chunk.Model;

namespace ShootCube.Dynamics.Ambient.Particle
{
    public class RainParticle : IParticle
    { 
        public XFlat Particle { get; set; }

        private byte corespondingHeight;
        private Vector2 initiatedVector;

        public RainParticle(Vector3 translation, int x, int y, int z)
        {
            Particle = new XFlat(translation, x, y, z, 3, 1.0f, scale_x: .15f, scale_y: .5f, transformation:
                Matrix.CreateRotationY((float)Global.Globals.Random.NextDouble()) *
                Matrix.CreateRotationX((float)Global.Globals.Random.NextDouble()));

            corespondingHeight = (byte)ChunkManager.SimplexNoise.GetNoise2D(x, z);
            initiatedVector = new Vector2(x, z);
        }

        public void LetFall(float value)
        {
            Particle.Vertices[0].Position -= new Vector3(0, value, 0);
            Particle.Vertices[1].Position -= new Vector3(0, value, 0);
            Particle.Vertices[2].Position -= new Vector3(0, value, 0);
            Particle.Vertices[3].Position -= new Vector3(0, value, 0);

            int y = (int)Particle.Vertices[0].Position.Y;


            if (y + Camera.CameraPosition.Y <= corespondingHeight)
                Reset();

            if (y < -5)
                Reset();


        }
        public void Reset()
        {
            Particle = new XFlat(Vector3.Zero, (int)initiatedVector.X, Globals.Random.Next(10, 25), (int)initiatedVector.Y, 3, 1.0f, scale_x: .15f, scale_y: .5f, transformation:
                  Matrix.CreateRotationY((float)Global.Globals.Random.NextDouble()) *
                  Matrix.CreateRotationX((float)Global.Globals.Random.NextDouble()));
            
        }

    }
}
