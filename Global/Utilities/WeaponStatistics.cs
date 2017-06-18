using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootCube.Global.Utilities
{
    public static class WeaponStatistics
    {
        public enum Utilities
        {
            Pickaxe_diamond,
            Pickaxe_iron,
            Pickaxe_stone,
            Pickaxe_wood
        }

        public static Dictionary<Utilities, byte> UtilityDamageRelation { get; set; }


        public static void Initialize()
        {
            UtilityDamageRelation = new Dictionary<Utilities, byte>();

            UtilityDamageRelation.Add(Utilities.Pickaxe_diamond, 20);
            UtilityDamageRelation.Add(Utilities.Pickaxe_iron, 10);
            UtilityDamageRelation.Add(Utilities.Pickaxe_stone, 5);
            UtilityDamageRelation.Add(Utilities.Pickaxe_wood, 1);

        }
    }
}
