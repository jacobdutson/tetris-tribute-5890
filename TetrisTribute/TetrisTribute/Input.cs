using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace TetrisTribute
{
    class Input
    {
        private KeyboardState oldKeyState;
        private GamePadState oldPadState;
     
        public bool up;
        public bool down;
        public bool right;
        public bool left;
        public bool enter;

        public bool oldUp;
        public bool oldDown;
        public bool oldRight;
        public bool oldLeft;
        public bool oldEnter;

        public Input()
        {
            up = false;
            down = false;
            right = false;
            left = false;
            enter = false;

            oldUp = false;
            oldDown = false;
            oldRight = false;
            oldLeft = false;
            oldEnter = false;
            
        }

        public void updateInput()
        {
            oldUp = up;
            oldDown = down;
            oldRight = right;
            oldLeft = left;
            oldEnter = enter;

            up = false;
            down = false;
            right = false;
            left = false;
            enter = false;

            KeyboardState keyState = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);

            Keys[] pressedKeys = keyState.GetPressedKeys();

            for (int i = 0; i < pressedKeys.Length; i++)
            {
                switch (pressedKeys[i])
                {
                    case Keys.Up:
                        up = true;
                        break;
                    case Keys.Down:
                        down = true;
                        break;
                    case Keys.Right:
                        right = true;
                        break;
                    case Keys.Left:
                        left = true;
                        break;
                    case Keys.Enter:
                        enter = true;
                        break;
                    default:
                        //TODO DO SOMETHING
                        break;
                }
            }

            if (padState.IsConnected)
            {
                //Dpad
                if (padState.DPad.Up == ButtonState.Pressed)
                {
                    up = true;
                }
                if (padState.DPad.Right == ButtonState.Pressed)
                {
                    right = true;
                }
                if (padState.DPad.Left == ButtonState.Pressed)
                {
                    left = true;
                }
                if (padState.DPad.Down == ButtonState.Pressed)
                {
                    down = true;
                }

                //left thumbstick
              /*  if (padState.ThumbSticks.Left.X == )
                {
                    up = true;
                }*/
            }
            

        }

        public bool Up
        {
            get { return up; }
        }

        public bool Right
        {
            get { return right; }
        }

        public bool Left
        {
            get { return left; }
        }

        public bool Down
        {
            get { return down; }
        }

        public bool Enter
        {
            get { return enter; }
        }

        public bool PreviousUp
        {
            get { return oldUp; }
        }

        public bool PreviousRight
        {
            get { return oldRight; }
        }

        public bool PreviousLeft
        {
            get { return oldLeft; }
        }

        public bool PreviousDown
        {
            get { return oldDown; }
        }

        public bool PreviousEnter
        {
            get { return oldEnter; }
        }
        
    }
}
