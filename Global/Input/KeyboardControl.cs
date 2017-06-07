using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace ShootCube.Global.Input
{
    public static class KeyboardControl
    {
        private static readonly List<Key> KeyList = new List<Key>();

        public static void AddKey(Key key)
        {
            if (KeyList.Exists(p => p.KeyId == key.KeyId))
                return;

            KeyList.Add(key);
        }

        public static void RemoveKey(Keys key)
        {
            KeyList.Remove(KeyList.Find(p => p.KeyId == key));
        }

        public static void Update()
        {
            for (var i = 0; i < KeyList.Count; i++)
                switch (KeyList[i].RequiresAction)
                {
                    case false:
                        if (Keyboard.GetState().IsKeyDown(KeyList[i].KeyId))
                            KeyList[i].Pressed.Invoke();
                        if (Keyboard.GetState().IsKeyUp(KeyList[i].KeyId))
                            if (KeyList[i].Released != null)
                                KeyList[i].Released.Invoke();
                        break;

                    case true:

                        if (Keyboard.GetState().IsKeyDown(KeyList[i].KeyId) && !KeyList[i].PressState)
                        {
                            KeyList[i].Pressed.Invoke();
                            KeyList[i].PressState = true;
                        }

                        if (Keyboard.GetState().IsKeyUp(KeyList[i].KeyId) && KeyList[i].PressState)
                        {
                            if (KeyList[i].Released != null)
                                KeyList[i].Released.Invoke();
                            KeyList[i].PressState = false;
                        }

                        break;
                }
        }
    }
}