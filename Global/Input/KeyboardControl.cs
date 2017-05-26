using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace ShootCube.Global.Input
{
    public static class KeyboardControl
    {

        private static List<Key> keyList = new List<Key>();

        public static void AddKey(Key key)
        {
            if (keyList.Exists(p => p.KeyId == key.KeyId))
                return;

            keyList.Add(key);
        }
        
        public static void RemoveKey(Keys key)
        {
            keyList.Remove(keyList.Find(p => p.KeyId == key));
        }

        public static void Update()
        {
            for (int i = 0; i < keyList.Count; i++)
            {
     
                switch (keyList[i].RequiresAction)
                {
                    case false:
                        if (Keyboard.GetState().IsKeyDown(keyList[i].KeyId))
                            keyList[i].Pressed.Invoke();
                        if (Keyboard.GetState().IsKeyUp(keyList[i].KeyId))
                            if(keyList[i].Released != null)
                                keyList[i].Released.Invoke();
                        break;

                    case true:

                        if (Keyboard.GetState().IsKeyDown(keyList[i].KeyId) && !keyList[i].pressState)
                        {
                            keyList[i].Pressed.Invoke();
                            keyList[i].pressState = true;
                        }

                        if (Keyboard.GetState().IsKeyUp(keyList[i].KeyId) && keyList[i].pressState)
                        {
                            if (keyList[i].Released != null)
                                keyList[i].Released.Invoke();
                            keyList[i].pressState = false;
                        }

                        break;
                }
            }
        }
    }
}
