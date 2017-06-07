using System;
using Microsoft.Xna.Framework.Input;

namespace ShootCube.Global.Input
{
    public class Key
    {
        public Keys KeyId;

        public Action Pressed;

        internal bool PressState;
        public Action Released;

        public bool RequiresAction;


        public Key(Keys key, Action pressed, bool req, Action released = null)
        {
            KeyId = key;
            Pressed = pressed;
            Released = released;

            RequiresAction = req;
        }
    }
}