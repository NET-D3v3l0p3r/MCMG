using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace ShootCube.Global.Input
{
    public static class MouseControl
    {

        private static Action leftAct, rigthAct;
        private static bool leftReq, rightReq;

        private static bool leftPressed, rightPressed;
        private static int leftLenght, rightLength;

        private static long a = 0;
        private static long b = 0;

        public static void AddLeftAction(Action action, bool reqRelease, int ms = 1)
        {
            leftAct = action;
            leftReq = reqRelease;
            leftLenght = ms;
        }

        public static void AddRightAction(Action action, bool reqRelease, int ms = 1)
        {
            rigthAct = action;
            rightReq = reqRelease;
            rightLength = ms;
        }

 

        public static void Update(GameTime gTime)
        {
            switch (leftReq)
            {
                case true:
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && !leftPressed)
                    {
                        leftPressed = true;
                        leftAct?.Invoke();
                    }
                    if (Mouse.GetState().LeftButton == ButtonState.Released)
                        leftPressed = false;
                    break;

                case false:
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        if (gTime.TotalGameTime.TotalMilliseconds > a)
                        {
                            leftAct?.Invoke();
                            a = (long)gTime.TotalGameTime.TotalMilliseconds + leftLenght;
                        }
                    }
                    break;
            }

            switch (rightReq)
            {
                case true:
                    if (Mouse.GetState().RightButton == ButtonState.Pressed && !rightPressed)
                    {
                        rightPressed = true;
                        rigthAct?.Invoke();
                    }
                    if (Mouse.GetState().RightButton == ButtonState.Released)
                        rightPressed = false;
                    break;

                case false:
                    if (Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        if (gTime.TotalGameTime.TotalMilliseconds > b)
                        {
                            rigthAct?.Invoke();
                            b = (long)gTime.TotalGameTime.TotalMilliseconds + rightLength;
                        }
                    }
                    break;
            }



        }



    }
}
