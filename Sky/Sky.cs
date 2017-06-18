using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShootCube.Global;
using System.Linq;
using ShootCube.World.Chunk.Model;
using System;
using System.Threading.Tasks;
using System.Diagnostics;


namespace ShootCube.Sky
{
    public class Sky
    {
        public SkyBox SkyBox { get; private set; }
        public Sun Sun { get; private set; }

        public static float Time { get; private set; }
        public static float Divisor { get; private set; }

        public enum SunStateEnum
        {
            Sunrise,
            Sunset
        }

        public static SunStateEnum SunState { get; private set; }

        

        public Sky(float divisor)
        {
            Divisor = divisor;

            Sun = new Sun(new Vector3((ChunkManager.Width * ChunkEditable.Width) / 2.0f, 0, 0));
            Sun.Radius = 500;
            //SkyBox = new SkyBox(2000);
            SunState = SunStateEnum.Sunset;

        }

        public void Update(GameTime gTime)
        {
            Time = MathHelper.ToRadians(45) + (float)gTime.TotalGameTime.TotalSeconds / Divisor;
            Sun.Update();


            // TODO!
            if (Math.Round(MathHelper.ToDegrees(Time)) % 150 == 0)
                SunState = SunState == SunStateEnum.Sunrise ? SunStateEnum.Sunset : SunStateEnum.Sunrise;

        }

        public void Render()
        {
            Sun.Render();
            //SkyBox.Draw();
        }
    }
}
