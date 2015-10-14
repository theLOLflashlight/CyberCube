using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{
    public class InputState
    {

        public GamePadState GamePad { get; private set; }
        public GamePadState OldGamePad { get; private set; }

        public KeyboardState Keyboard { get; private set; }
        public KeyboardState OldKeyboard { get; private set; }

        public MouseState Mouse { get; private set; }
        public MouseState OldMouse { get; private set; }


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

        public bool Keyboard_WasKeyPressed( Keys key )
        {
            return Keyboard.IsKeyDown( key ) && OldKeyboard.IsKeyUp( key );
        }

        public bool Keyboard_WasKeyReleased( Keys key )
        {
            return Keyboard.IsKeyUp( key ) && OldKeyboard.IsKeyDown( key );
        }

        public bool Keyboard_WasAnyKeyPressed( Keys[] keys )
        {
            return keys.Any( k => { return Keyboard_WasKeyPressed( k ); } );
        }

        public bool Keyboard_WasAnyKeyReleased( Keys[] keys )
        {
            return keys.Any( k => { return Keyboard_WasKeyReleased( k ); } );
        }

        public bool GamePad_WasButtonPressed( Buttons button )
        {
            return GamePad.IsButtonDown( button ) && OldGamePad.IsButtonUp( button );
        }

        public bool GamePad_WasButtonReleased( Buttons button )
        {
            return GamePad.IsButtonUp( button ) && OldGamePad.IsButtonDown( button );
        }

        public bool GamePad_WasAnyButtonPressed( Buttons[] buttons )
        {
            return buttons.Any( b => { return GamePad_WasButtonPressed( b ); } );
        }

        public bool GamePad_WasAnyButtonReleased( Buttons[] buttons )
        {
            return buttons.Any( b => { return GamePad_WasButtonReleased( b ); } );
        }

    }
}
