﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{
    public class InputState
    {
        public GamePadState OldGamePad { get; private set; }
        public KeyboardState OldKeyboard { get; private set; }
        public MouseState OldMouse { get; private set; }


        public GamePadState GamePad { get; private set; }
        public KeyboardState Keyboard { get; private set; }
        public MouseState Mouse { get; private set; }

        public InputState()
        {
            GamePad = Microsoft.Xna.Framework.Input.GamePad.GetState( PlayerIndex.One );
            Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
        }

        public void Update( GameTime gameTime )
        {
            OldGamePad = GamePad;
            OldKeyboard = Keyboard;
            OldMouse = Mouse;

            GamePad = Microsoft.Xna.Framework.Input.GamePad.GetState( PlayerIndex.One );
            Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
        }

    }
}
