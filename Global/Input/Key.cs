using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace ShootCube.Global.Input
{
    public class Key
    {
        public Keys KeyId;

        public Action Pressed;
        public Action Released;

        public bool RequiresAction;

        internal bool pressState;
        

        public Key(Keys key, Action pressed, bool req, Action released = null)
        {
            KeyId = key;
            Pressed = pressed;
            Released = released;

            RequiresAction = req;
        }

 
    }
}
