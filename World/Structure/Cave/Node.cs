using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShootCube.World.Chunk.Model;
using ShootCube.Maths;

namespace ShootCube.World.Structure.Cave
{
    public class Node
    {
        public Vector3 Position { get; set; }
        public int Radius { get; set; }
        public Node ConnectedTo { get; set; }

        public Node( Vector3 position, int radius)
        {
            Position = position;
            Radius = radius;
        }

        public void ConnectTo(Node neighbour)
        {
            ConnectedTo = neighbour;
        }

        public void RunOperation()
        {

            int decrementRadius = Radius;

            List<Value> listFinal = new List<Value>();
            List<Value> listFirst = new List<Value>();
            List<Value> listTemp = new List<Value>();

            listFirst.Add(new Value(new Int32_3D(Position)));

            while (decrementRadius > 0)
            {
                decrementRadius--;

                for (int i = 0; i < listFirst.Count; i++)
                {

                    Value light = listFirst[i];

                    int x = light.ArrayPosition.X;
                    int y = light.ArrayPosition.Y;
                    int z = light.ArrayPosition.Z;

                    if (!existsInList(listFinal, x - 1, y, z) && !existsInList(listTemp, x - 1, y, z) && !IsAir(x-1,y,z))
                        listTemp.Add(new Value(new Int32_3D(x - 1, y, z)));
                    if (!existsInList(listFinal, x + 1, y, z) && !existsInList(listTemp, x + 1, y, z) && !IsAir(x + 1, y, z))
                        listTemp.Add(new Value(new Int32_3D(x + 1, y, z)));


                    if (!existsInList(listFinal, x, y, z - 1) && !existsInList(listTemp, x, y, z - 1) && !IsAir(x, y, z - 1))
                        listTemp.Add(new Value(new Int32_3D(x, y, z - 1)));
                    if (!existsInList(listFinal, x, y, z + 1) && !existsInList(listTemp, x, y, z + 1) && !IsAir(x, y, z + 1))
                        listTemp.Add(new Value(new Int32_3D(x, y, z + 1)));


                    if (!existsInList(listFinal, x, y - 1, z) && !existsInList(listTemp, x, y - 1, z) && !IsAir(x, y - 1, z))
                        listTemp.Add(new Value(new Int32_3D(x, y - 1, z)));
                    if (!existsInList(listFinal, x, y + 1, z) && !existsInList(listTemp, x, y + 1, z) && !IsAir(x, y + 1, z))
                        listTemp.Add(new Value(new Int32_3D(x, y + 1, z)));


                    if (!existsInList(listFinal, x, y, z))
                        listFinal.Add(new Value(new Int32_3D(x, y, z)));
                }

                listFirst.Clear();
                listFirst = new List<Value>(listTemp);
                listTemp.Clear();
            }

            listTemp.Clear();
            listFirst.Clear();

            for (int i = 0; i < listFinal.Count; i++)
                listFinal[i].MakeAir();

            listFinal.Clear();

            // TODO: ADD MATHEMATICAL FORMULA EVALUATION TO 
            // ADJUST HEURISTICS

            if (ConnectedTo == null)
                return;

            Vector3 from = Position;
            Vector3 to = ConnectedTo.Position;

            // CREATE RAY [BETA] 

            AdvancedRay ray = new AdvancedRay(from, to - from);

            for (int i = 0; i < ray.InitiatedLambda; i++)
            {
                new Node(ray.Move(i), Global.Globals.Random.Next(5, 12)).RunOperation();
            }

        }

        private bool existsInList(List<Value> list, int x, int y, int z)
        {
            return list.Exists(p => p.ArrayPosition.X == x && p.ArrayPosition.Y == y && p.ArrayPosition.Z == z);
        }

        public bool IsAir(int x, int y, int z)
        {
            if (y < 0 || y >= ChunkEditable.Height || x < 0 || x >= ChunkManager.Width * ChunkEditable.Width || z < 0 || z >= ChunkManager.Depth * ChunkEditable.Depth)
                return true;

            if (ChunkManager.Cubes[y, x, z] == 8)
                return true;

            return ChunkManager.Cubes[y, x, z] == 0;
        }

    }
}
