using System;
using Microsoft.Xna.Framework;
using ShootCube.World.Chunk.Model;

namespace ShootCube.Sky
{
    public class Sky
    {
        public enum SunStateEnum
        {
            Sunrise,
            Sunset
        }


        public Sky(float divisor)
        {
            Divisor = divisor;

            Sun = new Sun(new Vector3(ChunkManager.Width * Chunk.Width / 2.0f, 0, 0)) {Radius = 500};
            //SkyBox = new SkyBox(2000);
            SunState = SunStateEnum.Sunset;
        }

        public SkyBox SkyBox { get; private set; }
        public Sun Sun { get; }

        public static float Time { get; private set; }
        public static float Divisor { get; private set; }

        public static SunStateEnum SunState { get; private set; }

        public void Update(GameTime gTime)
        {
            Time = MathHelper.ToRadians(45) + (float) gTime.TotalGameTime.TotalSeconds / Divisor;
            Sun.Update();


            // TODO!
            if (Math.Abs(Math.Round(MathHelper.ToDegrees(Time)) % 150) < 0.001f)
                SunState = SunState == SunStateEnum.Sunrise ? SunStateEnum.Sunset : SunStateEnum.Sunrise;
        }

        public void Render()
        {
            Sun.Render();
            //SkyBox.Draw();
        }
    }
}