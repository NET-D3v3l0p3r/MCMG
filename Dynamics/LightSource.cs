using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShootCube.Global;
using ShootCube.Declaration;
using System.Runtime.InteropServices;
using ShootCube.World.Chunk.Model;

namespace ShootCube.Dynamics
{
    public class LightSource
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public int LightIntensity { get; set; }

        private List<Light> listFinal = new List<Light>();
        private List<Light> listFirst = new List<Light>();
        private List<Light> listTemp = new List<Light>();

        public LightSource(Vector3 pos, int intensity)
        {
            X = (int)pos.X;
            Y = (int)pos.Y;
            Z = (int)pos.Z;

            LightIntensity = intensity;
        }

        public void Emit()
        {
            Task.Run(() =>
            {
                int value = LightIntensity;

                listFirst.Add(new Light(value, X, Y, Z, LightIntensity));


                while (value != 0)
                {
                    value--;

                    for (int i = 0; i < listFirst.Count; i++)
                    {
                        Light light = listFirst[i];

                        int x = light.X;
                        int y = light.Y;
                        int z = light.Z;


                        if (!existsInList(listFinal, x - 1, y, z) && !existsInList(listTemp, x - 1, y, z) && !isObstacle(x - 1, y, z))
                        {
                            listTemp.Add(new Light(value, x - 1, y, z, LightIntensity));
                        }
                        if (!existsInList(listFinal, x + 1, y, z) && !existsInList(listTemp, x + 1, y, z) && !isObstacle(x + 1, y, z))
                        {
                            listTemp.Add(new Light(value, x + 1, y, z, LightIntensity));
                        }

                        if (!existsInList(listFinal, x, y, z - 1) && !existsInList(listTemp, x, y, z - 1) && !isObstacle(x, y, z - 1))
                        {
                            listTemp.Add(new Light(value, x, y, z - 1, LightIntensity));
                        }
                        if (!existsInList(listFinal, x, y, z + 1) && !existsInList(listTemp, x, y, z + 1) && !isObstacle(x, y, z + 1))
                        {
                            listTemp.Add(new Light(value, x, y, z + 1, LightIntensity));
                        }

                        if (!existsInList(listFinal, x, y + 1, z) && !existsInList(listTemp, x, y + 1, z) && !isObstacle(x, y + 1, z))
                        {
                            listTemp.Add(new Light(value, x, y + 1, z, LightIntensity));
                        }
                        if (!existsInList(listFinal, x, y - 1, z) && !existsInList(listTemp, x, y - 1, z) && !isObstacle(x, y - 1, z))
                        {
                            listTemp.Add(new Light(value, x, y - 1, z, LightIntensity));
                        }


                        if (!existsInList(listFinal, x, y, z) && !isObstacle(x, y, z))
                            listFinal.Add(new Light(value, x, y, z, LightIntensity));



                    }

                    listFirst.Clear();
                    listFirst = new List<Light>(listTemp);
                    listTemp.Clear();
                }

                listTemp.Clear();
                listFirst.Clear();

                for (int i = 0; i < listFinal.Count; i++)
                    listFinal[i].LightUp();

                listFinal.Clear();
            });
        }

        private bool existsInList(List<Light> list, int x, int y, int z)
        {
            return list.Exists(p => p.X == x && p.Y == y && p.Z == z);
        }
        private bool isObstacle(int x, int y, int z)
        {
            return ChunkManager.Cubes[y, x, z] != 0;
        }

        public void DebugDrawLight()
        {
            for (int i = 0; i < listFinal.Count; i++)
            {
                listFinal[i].Render();
            }
        }
    }
}
