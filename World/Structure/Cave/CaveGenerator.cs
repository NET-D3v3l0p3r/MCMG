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

         

            for (int i = 0; i < Nodes.Count - 1; i++)
            {
                Nodes[i].ConnectedTo = Nodes[i + 1];
            }
            foreach (var node in Nodes)
            {
                node.RunOperation();
            }

            Nodes.Clear();
        }

    }
}
