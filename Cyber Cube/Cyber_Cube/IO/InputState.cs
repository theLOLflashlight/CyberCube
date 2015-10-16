using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube.IO
{
    public delegate float AnalogInput();

    public enum Action
    {
        MoveLeft, MoveRight, MoveUp, MoveDown
    }

    public struct InputAction
    {
        public readonly Action Action;
        public readonly float Value;

        public InputAction( Action action, float value )
        {
            Action = action;
            Value = value;
        }

        public InputAction( Action action, bool value )
            : this( action, value ? 1f : 0f )
        {
        }

        public static implicit operator Action( InputAction pAction )
        {
            return pAction.Action;
        }

        public static implicit operator bool( InputAction pAction )
        {
            return pAction.Value != 0;
        }
    }

    public class InputState
    {

        public GamePadState GamePad { get; private set; }
        public GamePadState OldGamePad { get; private set; }

        public KeyboardState Keyboard { get; private set; }
        public KeyboardState OldKeyboard { get; private set; }

        public MouseState Mouse { get; private set; }
        public MouseState OldMouse { get; private set; }

        private Dictionary<Action, Keys> mKeyBinds = new Dictionary<Action, Keys>();
        private Dictionary<Action, Buttons> mButtonBinds = new Dictionary<Action, Buttons>();
        private Dictionary<Action, AnalogInput> mAnalogBinds = new Dictionary<Action, AnalogInput>();

        public InputState()
        {
            GamePad = Microsoft.Xna.Framework.Input.GamePad.GetState( PlayerIndex.One );
            Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();

            mKeyBinds[ Action.MoveLeft ] = Keys.Left;
            mKeyBinds[ Action.MoveRight ] = Keys.Right;
            mKeyBinds[ Action.MoveUp ] = Keys.Up;
            mKeyBinds[ Action.MoveDown ] = Keys.Down;

            mButtonBinds[ Action.MoveLeft ] = Buttons.DPadLeft;
            mButtonBinds[ Action.MoveRight ] = Buttons.DPadRight;
            mButtonBinds[ Action.MoveUp ] = Buttons.DPadUp;
            mButtonBinds[ Action.MoveDown ] = Buttons.DPadDown;

            mAnalogBinds[ Action.MoveLeft ] = () => { return -GamePad.ThumbSticks.Left.X; };
            mAnalogBinds[ Action.MoveRight ] = () => { return GamePad.ThumbSticks.Left.X; };
            mAnalogBinds[ Action.MoveUp ] = () => { return GamePad.ThumbSticks.Left.Y; };
            mAnalogBinds[ Action.MoveDown ] = () => { return -GamePad.ThumbSticks.Left.Y; };
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

        public InputAction this[ Action action ]
        {
            get {
                if ( mKeyBinds.ContainsKey( action )
                     && Keyboard.IsKeyDown( mKeyBinds[ action ] ) )
                    return new InputAction( action, true );

                if ( mButtonBinds.ContainsKey( action )
                     && GamePad.IsButtonDown( mButtonBinds[ action ] ) )
                    return new InputAction( action, true );

                if ( mAnalogBinds.ContainsKey( action ) )
                {
                    var analogResult = mAnalogBinds[ action ].Invoke();
                    if ( analogResult > 0 )
                        return new InputAction( action, analogResult );
                }
                return new InputAction( action, false );
            }
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

    }
}
