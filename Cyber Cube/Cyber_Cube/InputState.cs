using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{
    public delegate float AnalogInput();

    public enum Actions
    {
        MoveLeft, MoveRight, MoveUp, MoveDown
    }

    public struct InputAction
    {
        public readonly Actions Action;
        public readonly float Value;

        public InputAction( Actions action, float value )
        {
            Action = action;
            Value = value;
        }

        public InputAction( Actions action, bool value )
            : this( action, value ? 1f : 0f )
        {
        }

        public static implicit operator Actions( InputAction pAction )
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

        private Dictionary<Actions, Keys> mKeyBinds = new Dictionary<Actions, Keys>();
        private Dictionary<Actions, Buttons> mButtonBinds = new Dictionary<Actions, Buttons>();
        private Dictionary<Actions, AnalogInput> mAnalogBinds = new Dictionary<Actions, AnalogInput>();

        public InputState()
        {
            GamePad = Microsoft.Xna.Framework.Input.GamePad.GetState( PlayerIndex.One );
            Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();

            mKeyBinds[ Actions.MoveLeft ] = Keys.Left;
            mKeyBinds[ Actions.MoveRight ] = Keys.Right;
            mKeyBinds[ Actions.MoveUp ] = Keys.Up;
            mKeyBinds[ Actions.MoveDown ] = Keys.Down;

            mButtonBinds[ Actions.MoveLeft ] = Buttons.DPadLeft;
            mButtonBinds[ Actions.MoveRight ] = Buttons.DPadRight;
            mButtonBinds[ Actions.MoveUp ] = Buttons.DPadUp;
            mButtonBinds[ Actions.MoveDown ] = Buttons.DPadDown;

            mAnalogBinds[ Actions.MoveLeft ] = () => { return -GamePad.ThumbSticks.Left.X; };
            mAnalogBinds[ Actions.MoveRight ] = () => { return GamePad.ThumbSticks.Left.X; };
            mAnalogBinds[ Actions.MoveUp ] = () => { return GamePad.ThumbSticks.Left.Y; };
            mAnalogBinds[ Actions.MoveDown ] = () => { return -GamePad.ThumbSticks.Left.Y; };
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

        public InputAction this[ Actions action ]
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

    }
}
