using Microsoft.Xna.Framework;
using ShootCube.Dynamics.Player;
using ShootCube.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootCube.Dynamics.Ambient.Gravitation
{
    public class GravitationManager
    {
        public MainPlayer Player { get; private set; }

        public float Velocity { get; set; }
        public float AccelerationValue { get; set; }

        public GravitationManager(MainPlayer player, float v, float a)
        {
            Player = player;
            Player.GravitationManager = this;

            Velocity = v;
            AccelerationValue = a;

        }

        private float _acceleration;
        public void Reset()
        {
            _acceleration = 0;
        }

        public void Update(GameTime gTime)
        {
            if (!Player.IsJumping)
                _acceleration -= AccelerationValue;

            Camera.Move(new Vector3(0, -Velocity + _acceleration, 0));
        }


    }
}
