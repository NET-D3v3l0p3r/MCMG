using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootCube.World.Structure.Cave
{
    public class CaveGenerator
    {
        public static List<Node> Nodes = new List<Node>();


        public static void Generate()
        {

            for (int i = 0; i < Nodes.Count; i++)
                Nodes[i].ConnectedTo = Nodes[GetRandomExDefined(i, Nodes.Count)];

            foreach (var node in Nodes)
                node.RunOperation();

            Nodes.Clear();
        }

        private static int GetRandomExDefined(int i, int max)
        {
            int random = Global.Globals.Random.Next(0, max);
            if (random == i)
                return GetRandomExDefined(i, max);
            else return random;
        }

    }
}
