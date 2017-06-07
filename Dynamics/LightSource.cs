using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ShootCube.World.Chunk.Model;

namespace ShootCube.Dynamics
{
    public class LightSource
    {
        private readonly List<Light> _listFinal = new List<Light>();
        private List<Light> _listFirst = new List<Light>();
        private readonly List<Light> _listTemp = new List<Light>();

        public LightSource(Vector3 pos, int intensity)
        {
            X = (int) pos.X;
            Y = (int) pos.Y;
            Z = (int) pos.Z;

            LightIntensity = intensity;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public int LightIntensity { get; set; }

        public void Emit()
        {
            Task.Run(() =>
            {
                var value = LightIntensity;

                _listFirst.Add(new Light(value, X, Y, Z, LightIntensity));


                while (value != 0)
                {
                    value--;

                    for (var i = 0; i < _listFirst.Count; i++)
                    {
                        var light = _listFirst[i];

                        var x = light.X;
                        var y = light.Y;
                        var z = light.Z;


                        if (!ExistsInList(_listFinal, x - 1, y, z) && !ExistsInList(_listTemp, x - 1, y, z) &&
                            !IsObstacle(x - 1, y, z))
                            _listTemp.Add(new Light(value, x - 1, y, z, LightIntensity));
                        if (!ExistsInList(_listFinal, x + 1, y, z) && !ExistsInList(_listTemp, x + 1, y, z) &&
                            !IsObstacle(x + 1, y, z))
                            _listTemp.Add(new Light(value, x + 1, y, z, LightIntensity));

                        if (!ExistsInList(_listFinal, x, y, z - 1) && !ExistsInList(_listTemp, x, y, z - 1) &&
                            !IsObstacle(x, y, z - 1))
                            _listTemp.Add(new Light(value, x, y, z - 1, LightIntensity));
                        if (!ExistsInList(_listFinal, x, y, z + 1) && !ExistsInList(_listTemp, x, y, z + 1) &&
                            !IsObstacle(x, y, z + 1))
                            _listTemp.Add(new Light(value, x, y, z + 1, LightIntensity));

                        if (!ExistsInList(_listFinal, x, y + 1, z) && !ExistsInList(_listTemp, x, y + 1, z) &&
                            !IsObstacle(x, y + 1, z))
                            _listTemp.Add(new Light(value, x, y + 1, z, LightIntensity));
                        if (!ExistsInList(_listFinal, x, y - 1, z) && !ExistsInList(_listTemp, x, y - 1, z) &&
                            !IsObstacle(x, y - 1, z))
                            _listTemp.Add(new Light(value, x, y - 1, z, LightIntensity));


                        if (!ExistsInList(_listFinal, x, y, z) && !IsObstacle(x, y, z))
                            _listFinal.Add(new Light(value, x, y, z, LightIntensity));
                    }

                    _listFirst.Clear();
                    _listFirst = new List<Light>(_listTemp);
                    _listTemp.Clear();
                }

                _listTemp.Clear();
                _listFirst.Clear();

                for (var i = 0; i < _listFinal.Count; i++)
                    _listFinal[i].LightUp();

                _listFinal.Clear();
            });
        }

        private bool ExistsInList(List<Light> list, int x, int y, int z)
        {
            return list.Exists(p => p.X == x && p.Y == y && p.Z == z);
        }

        private bool IsObstacle(int x, int y, int z)
        {
            return ChunkManager.Cubes[y, x, z] != 0;
        }

        public void DebugDrawLight()
        {
            for (var i = 0; i < _listFinal.Count; i++)
                _listFinal[i].Render();
        }
    }
}