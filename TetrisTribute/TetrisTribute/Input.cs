using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

//Author: Jason Newbold

namespace TetrisTribute
{
    class Input
    {
        // values that represent keys pressed from any input
        private bool up;
        private bool down;
        private bool right;
        private bool left;
        private bool enter;
        private bool pause;
        private bool inputString;
        private bool stringUpdated;
        private string name;
        private bool back;
        private bool space;
        private bool x;


        // values that represent the old key state from any input
        private bool oldUp;
        private bool oldDown;
        private bool oldRight;
        private bool oldLeft;
        private bool oldEnter;
        private bool oldPause;
        private bool oldInputString;
        private bool oldBack;
        private bool oldSpace;
        private bool oldx;

        private char xboxChar;

        //stores if the current buttons are in a menu state
        private bool menuControl;

        //initialize all buttons to false
        public Input()
        {
            name = "";
            xboxChar = (char)65;

            menuControl = true;
            up = false;
            down = false;
            right = false;
            left = false;
            enter = false;
            pause = false;
            back = false;
            space = false;
            inputString = false;
            x = false;

            oldUp = false;
            oldDown = false;
            oldRight = false;
            oldLeft = false;
            oldEnter = false;
            oldPause = false;
            oldBack = false;
            oldSpace = false;
            oldInputString = false;
            oldx = false;

            stringUpdated = false;
        }

        //updates the input keys pressed
        public void updateInput()
        {
            //store old values
            oldUp = up;
            oldDown = down;
            oldRight = right;
            oldLeft = left;
            oldEnter = enter;
            oldPause = pause;
            oldInputString = inputString;
            oldBack = back;
            oldSpace = space;
            oldx = x;

            //reset inputs
            up = false;
            down = false;
            right = false;
            left = false;
            enter = false;
            pause = false;
            back = false;
            space = false;
            inputString = false;
            x = false;

            KeyboardState keyState = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);

            //get keyboard input
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
                    case Keys.P:
                        pause = true;
                        if (!oldPause)
                        {
                            name += "P";
                        }
                        break;
                    case Keys.RightShift:
                        break;
                    case Keys.Space:
                        space = true;
                        if (!oldSpace)
                        {
                            name += " ";
                        }
                        break;
                    case Keys.LeftShift:
                        break;
                    case Keys.Back:
                        back = true;
                        if (name.Length == 0)
                        {
                            name = "";
                        }
                        else if (!oldBack)
                        {
                            name = name.Substring(0, (name.Length - 1));
                        }
                        break;
                    default:
                        inputString = true;
                        if (!oldInputString)
                        {
                            name += pressedKeys[i].ToString();
                        }
                        stringUpdated = true;
                        break;
                }
            }

            //get gamepad input
            if (padState.IsConnected)
            {
                //Dpad

                if (padState.DPad.Up == ButtonState.Pressed)
                {
                    if (menuControl)
                    {
                        up = true;
                    }
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
                if (padState.ThumbSticks.Left.X > 0)
                {
                    right = true;
                }
                if (padState.ThumbSticks.Left.X < 0)
                {
                    left = true;
                }
                if (padState.ThumbSticks.Left.Y > 0)
                {
                    if (menuControl)
                    {
                        up = true;
                    }
                }
                if (padState.ThumbSticks.Left.Y < 0)
                {
                    down = true;
                }

                if (padState.Buttons.X == ButtonState.Pressed)
                {
                    x = true;
                    if (!oldx)
                    {
                        name += xboxChar;
                    }
                }
                //a button will act like enter during menu and rotate during game
                if (padState.Buttons.A == ButtonState.Pressed)
                {
                    if (menuControl)
                    {
                        enter = true;
                        if (name.Length > 0)
                        {
                            name = name.Substring(0, name.Length - 1);
                        }
                    }
                    else
                    {
                        up = true;
                    }
                }

                if (padState.Buttons.B == ButtonState.Pressed)
                {
                    if (name.Length > 0)
                    {
                        name = name.Substring(0, (name.Length - 1));
                    }
                }

                if (up && !PreviousUp  && xboxChar < 90)
                {
                    xboxChar++;
                }
                else if (up && !PreviousUp)
                {
                    xboxChar = (char)65;
                }

                if (down && !PreviousDown && xboxChar > 65)
                {
                    xboxChar--;
                }
                else if (down && !PreviousDown)
                {
                    xboxChar = (char)90;
                }
                if (name.Length > 0)
                {
                    name = name.Substring(0, name.Length - 1);
                }
                name += xboxChar;
            }

            
            if (name.Length > 15)
            {
                name = name.Substring(0, 15);
            }

        }

        //set if the inputs are in a menu
        public void setMenuControl(bool value)
        {
            menuControl = value;
        }

        //gets the current up value
        public bool Up
        {
            get { return up; }
        }

        //gets the current right value
        public bool Right
        {
            get { return right; }
        }

        //gets the current left value
        public bool Left
        {
            get { return left; }
        }

        //gets the current down value
        public bool Down
        {
            get { return down; }
        }

        //gets the current enter value
        public bool Enter
        {
            get { return enter; }
        }

        //gets the current pause value
        public bool Pause
        {
            get { return pause; }
        }

        //gets the previous up value
        public bool PreviousUp
        {
            get { return oldUp; }
        }

        //gets the previous right value
        public bool PreviousRight
        {
            get { return oldRight; }
        }

        //gets the previous left value
        public bool PreviousLeft
        {
            get { return oldLeft; }
        }

        //gets the previous down value
        public bool PreviousDown
        {
            get { return oldDown; }
        }

        //gets the previous enter value
        public bool PreviousEnter
        {
            get { return oldEnter; }
        }

        //gets the previous pause value
        public bool PreviousPause
        {
            get { return oldPause; }
        }

        //gets the previous up value
        /* public string inputChar
         {
             get { stringUpdated = false; return inputString; }
         }

         //gets the previous up value
         public string oldInputChar
         {
             get { return oldInputString; }
         }*/

        //gets the previous up value
        public bool UpdatedString
        {
            get { return stringUpdated; }
        }

        //gets the previous up value
        public string Name
        {
            get { return name; }
        }

        public void clearName()
        {
            name = "";
        }
    }
}
