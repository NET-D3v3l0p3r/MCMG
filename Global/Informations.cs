using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootCube.Global
{
    public static class Informations
    {
        public static Dictionary<byte, SoundEffectInstance> CorrespondingAudios { get; private set; }


        public static void Initialize()
        {
            CorrespondingAudios = new Dictionary<byte, SoundEffectInstance>()
            {
                { 1, Globals.Content.Load<SoundEffect>(@"sounds\cubes\dirt").CreateInstance() },
                { 2, Globals.Content.Load<SoundEffect>(@"sounds\cubes\dirt").CreateInstance() },
                { 5, Globals.Content.Load<SoundEffect>(@"sounds\cubes\dirt").CreateInstance() },
                { 6, Globals.Content.Load<SoundEffect>(@"sounds\cubes\stone").CreateInstance() },
                //{ 8, Globals.Content.Load<SoundEffect>(@"sounds\cubes\border") }
            };

            foreach (var item in CorrespondingAudios)
                item.Value.IsLooped = false;
        }

    }
}
