using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShootCube.Dynamics.Ambient.Particle;
using ShootCube.Global;
using ShootCube.Declaration;
using ShootCube.World.Chunk;
using ShootCube.World.Chunk.Model;

namespace ShootCube.Dynamics.Ambient
{
    public class WeatherManager
    {
        public VertexBuffer VertexBuffer { get; set; }
        public List<IParticle> Particles { get; set; }


        public static int WeatherParticleSpawningHeightAbsolute { get; set; }
        public static int WeatherParticleResetHeightAbsolute { get; set; }

        public static Effect WeatherEffect { get; set; }

        private int verticesCount;
        public enum ParticleType
        {
            Rain,
            Snowflakes
        }

        public WeatherManager()
        {
            Particles = new List<IParticle>();

            WeatherEffect = Globals.Content.Load<Effect>("main");

        }


        public void SpawnParticles(ParticleType type)
        {
            Particles.Clear();
            Vector3 playerPosition = Camera.CameraPosition;

            int x = (int)playerPosition.X;
            int y = (int)playerPosition.Y;
            int z = (int)playerPosition.Z;

            switch (type)
            {
                case ParticleType.Rain:

                    for (int i = -7; i < 7; i++)
                    {
                        for (int j = -7; j < 7; j++)
                        {
                            for (int k = 0; k < 7; k++)
                            {
                                Particles.Add(new RainParticle(Vector3.Zero, i, Globals.Random.Next(-5, 25), j));
                            }
                        }
                    }

                    break;
                case ParticleType.Snowflakes:

                    break;
            }
        }

        public void GenerateVertexBuffer()
        {
       
            int offSet = 0;
            VPTVDeclaration[] Vertices = new VPTVDeclaration[Particles.Count * 4];

            for (int i = 0; i < Particles.Count; i++)
            {
                IFlat flat = Particles[i].Particle;
                if (flat == null)
                    continue;
                for (int j = 0; j < flat.Vertices.Length; j++)
                {
                    if (j + offSet < Vertices.Length)
                        Vertices[j + offSet] = flat.Vertices[j];
                }
                offSet += 4;
            }

            VertexBuffer = new VertexBuffer(Globals.GraphicsDevice, typeof(VPTVDeclaration), Vertices.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData<VPTVDeclaration>(Vertices);

            verticesCount = Particles.Count;
            Vertices = new VPTVDeclaration[0];

        

        }

        public void Update()
        {
            if (Particles.Count == 0)
                return;
            for (int i = 0; i < Particles.Count; i++)
                Particles[i].LetFall(0.35f);

            GenerateVertexBuffer();
        }


        public void Render()
        { 
            if (verticesCount == 0 || VertexBuffer == null)
                return;


            

            WeatherEffect.Parameters["View"].SetValue(Camera.View);
            WeatherEffect.Parameters["World"].SetValue(Matrix.CreateTranslation(Camera.CameraPosition -new Vector3(0, 0, 4.5f)));
            WeatherEffect.Parameters["Projection"].SetValue(Camera.Projection);
            WeatherEffect.Parameters["TextureAtlas"].SetValue(ChunkManager.TextureAtlas);

            WeatherEffect.CurrentTechnique.Passes[0].Apply();

            Globals.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            Globals.GraphicsDevice.Indices = Globals.IndexBuffer;
            Globals.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, verticesCount * 2);
            


        }


    }
}
