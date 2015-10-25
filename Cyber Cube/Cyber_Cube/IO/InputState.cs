﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.IO
{

    public class InputState
    {

        public object Focus { private get; set; }

        public bool CheckFocus( object obj )
        {
            return ReferenceEquals( Focus, obj );
        }

        public bool HasFocus
        {
            get {
                return Focus != null;
            }
        }

        public MouseState Mouse { get; private set; }
        public MouseState OldMouse { get; private set; }

        public KeyboardState Keyboard { get; private set; }
        public KeyboardState OldKeyboard { get; private set; }

        public GamePadState GamePad { get; private set; }
        public GamePadState OldGamePad { get; private set; }


        public InputState()
        {
            Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
            Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            GamePad = Microsoft.Xna.Framework.Input.GamePad.GetState( PlayerIndex.One );
        }

        public void Refresh()
        {
            OldMouse = Mouse;
            OldKeyboard = Keyboard;
            OldGamePad = GamePad;

            Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
            Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            GamePad = Microsoft.Xna.Framework.Input.GamePad.GetState( PlayerIndex.One );
        }
        
        //public bool Mouse_WasLeftClicked()
        //{
        //    return Mouse.LeftButton == ButtonState.Released && OldMouse.LeftButton == ButtonState.Pressed;
        //}
        //
        //public bool Mouse_WasRightClicked()
        //{
        //    return Mouse.RightButton == ButtonState.Released && OldMouse.RightButton == ButtonState.Pressed;
        //}
        //
        //public bool Mouse_WasMiddleClicked()
        //{
        //    return Mouse.MiddleButton == ButtonState.Released && OldMouse.MiddleButton == ButtonState.Pressed;
        //}

        // Mouse Was Pressed

        public bool Mouse_WasLeftPressed()
        {
            return Mouse.LeftButton == ButtonState.Pressed && OldMouse.LeftButton != ButtonState.Pressed;
        }

        public bool Mouse_WasRightPressed()
        {
            return Mouse.RightButton == ButtonState.Pressed && OldMouse.RightButton != ButtonState.Pressed;
        }

        public bool Mouse_WasMiddlePressed()
        {
            return Mouse.MiddleButton == ButtonState.Pressed && OldMouse.MiddleButton != ButtonState.Pressed;
        }

        // Mouse Was Released

        public bool Mouse_WasLeftReleased()
        {
            return Mouse.LeftButton == ButtonState.Released && OldMouse.LeftButton != ButtonState.Released;
        }

        public bool Mouse_WasRightReleased()
        {
            return Mouse.RightButton == ButtonState.Released && OldMouse.RightButton != ButtonState.Released;
        }

        public bool Mouse_WasMiddleReleased()
        {
            return Mouse.MiddleButton == ButtonState.Released && OldMouse.MiddleButton != ButtonState.Released;
        }


        public bool IsShiftDown()
        {
            return Keyboard.IsKeyDown( Keys.LeftShift ) || Keyboard.IsKeyDown( Keys.RightShift );
        }

        public bool IsCtrlDown()
        {
            return Keyboard.IsKeyDown( Keys.LeftControl ) || Keyboard.IsKeyDown( Keys.RightControl );
        }

        public bool IsAltDown()
        {
            return Keyboard.IsKeyDown( Keys.LeftAlt ) || Keyboard.IsKeyDown( Keys.RightAlt );
        }

        // Was Key

        public bool Keyboard_WasKeyPressed( Keys key )
        {
            return Keyboard.IsKeyDown( key ) && OldKeyboard.IsKeyUp( key );
        }

        public bool Keyboard_WasKeyReleased( Keys key )
        {
            return Keyboard.IsKeyUp( key ) && OldKeyboard.IsKeyDown( key );
        }

        // Was *Any* Key

        public bool Keyboard_WasAnyKeyPressed( IEnumerable<Keys> keys )
        {
            return keys.Any( k => { return Keyboard_WasKeyPressed( k ); } );
        }

        public bool Keyboard_WasAnyKeyReleased( IEnumerable<Keys> keys )
        {
            return keys.Any( k => { return Keyboard_WasKeyReleased( k ); } );
        }
        
        // Was Button

        public bool GamePad_WasButtonPressed( Buttons button )
        {
            return GamePad.IsButtonDown( button ) && OldGamePad.IsButtonUp( button );
        }

        public bool GamePad_WasButtonReleased( Buttons button )
        {
            return GamePad.IsButtonUp( button ) && OldGamePad.IsButtonDown( button );
        }

        // Was *Any* Button

        public bool GamePad_WasAnyButtonPressed( IEnumerable<Buttons> buttons )
        {
            return buttons.Any( b => { return GamePad_WasButtonPressed( b ); } );
        }

        public bool GamePad_WasAnyButtonReleased( IEnumerable<Buttons> buttons )
        {
            return buttons.Any( b => { return GamePad_WasButtonReleased( b ); } );
        }

    }
}
