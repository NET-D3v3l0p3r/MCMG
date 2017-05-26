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

namespace ShootCube.Dynamics
{
    public class LightManager
    {
        public List<LightSource> LightSources { get; set; }
    }
}
