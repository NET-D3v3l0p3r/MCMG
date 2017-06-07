using System;
using Microsoft.Xna.Framework;

namespace ShootCube.Global
{
    public class FpsCounter
    {
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private int _frameCounter;
        private int _frameRate;


        public void Start(GameTime gTime)
        {
            _elapsedTime += gTime.ElapsedGameTime;
            if (_elapsedTime <= TimeSpan.FromSeconds(1))
                return;
            _elapsedTime -= TimeSpan.FromSeconds(1);
            _frameRate = _frameCounter;
            _frameCounter = 0;
        }

        public int End()
        {
            _frameCounter++;
            return _frameRate;
        }
    }
}