using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ShootCube.Network;
using ShootCube.Global;
using ShootCube.Global.Input;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using ShootCube.Dynamics.Ambient.Gravitation;

namespace ShootCube.Dynamics.Player
{
    [Serializable]
    public class MainPlayer
    {
        public string Id { get; private set; }
        public Vector3 Position { get; set; }
        public Vector3 LookingAt { get; set; }

        public BoundingBox HitBox { get; set; }
        public NetworkManager Network { get; set; }

        public int Fuel { get; set; }
        public bool IsJumping { get; private set; }

        public GravitationManager GravitationManager { get; set; }

        private Vector3 _lastPosition, _lastDirection;

        public bool isPlaying;
        private SoundEffect soundEffect;

        public SoundEffectInstance SoundEffectInstance;

        public MainPlayer(string id)
        {
            Id = id;

            soundEffect = Globals.Content.Load<SoundEffect>("jetpack");
            SoundEffectInstance = soundEffect.CreateInstance();
  

            Fuel = 200;

            KeyboardControl.AddKey(new Key(Keys.W, new Action(() =>
            {
                Camera.Move(new Vector3(0, 0, -1));
            }), false));
            KeyboardControl.AddKey(new Key(Keys.A, new Action(() =>
            {
                Camera.Move(new Vector3(-1, 0, 0));
            }), false));
            KeyboardControl.AddKey(new Key(Keys.S, new Action(() =>
            {
                Camera.Move(new Vector3(0, 0, 1));
            }), false));
            KeyboardControl.AddKey(new Key(Keys.D, new Action(() =>
            {
                Camera.Move(new Vector3(1, 0, 0));
            }), false));
            KeyboardControl.AddKey(new Key(Keys.Space, new Action(() =>
            {
                IsJumping = false;
                if (Fuel-- < 0)
                {
                    SoundEffectInstance.Stop();
                    return;
                }
                IsJumping = true;
                GravitationManager.Reset();
                if (!isPlaying)
                {
                    SoundEffectInstance.Play();
                    isPlaying = true;
                }
                Camera.Move(new Vector3(0, 1.85f, 0));
            }), false, new Action(() =>
            {
                IsJumping = false;
                isPlaying = false;
                SoundEffectInstance.Stop();

                 
            })));
            KeyboardControl.AddKey(new Key(Keys.LeftShift, new Action(() =>
            {
                Camera.Move(new Vector3(0, -1, 0));
            }), false));


        }

        public void Update()
        {
            if(Camera.CameraPosition != _lastPosition)
            {
                _lastPosition = Camera.CameraPosition;
                Network?.Client.Send(_lastPosition);
            } 

            Position = Camera.CameraPosition;
            LookingAt = Camera.Direction;

            HitBox = new BoundingBox(Position, Position + new Vector3(1, 1, 1));

            
        }

        
    }
}
