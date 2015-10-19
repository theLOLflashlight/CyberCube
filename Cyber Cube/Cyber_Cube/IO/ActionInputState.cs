using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube.IO
{

    public class InputState< Action > : InputState
        where Action : struct, IComparable, IFormattable, IConvertible
    {
        public delegate float DynamicInput( InputState< Action > input );

        public struct ActionState
        {
            public readonly Action Action;
            public readonly float Value;

            public ActionState( Action action, float value )
            {
                Action = action;
                Value = value;
            }

            public ActionState( Action action, bool value )
                : this( action, value ? 1f : 0f )
            {
            }

            public static implicit operator Action( ActionState input )
            {
                return input.Action;
            }

            public static implicit operator bool ( ActionState input )
            {
                return input.Value != 0;
            }
        }

        private Dictionary<Action, Keys> mKeyBinds = new Dictionary<Action, Keys>();
        private Dictionary<Action, Keys> mKeyPressedBinds = new Dictionary<Action, Keys>();
        private Dictionary<Action, Keys> mKeyReleasedBinds = new Dictionary<Action, Keys>();

        private Dictionary<Action, Buttons> mButtonBinds = new Dictionary<Action, Buttons>();
        private Dictionary<Action, Buttons> mButtonPressedBinds = new Dictionary<Action, Buttons>();
        private Dictionary<Action, Buttons> mButtonReleasedBinds = new Dictionary<Action, Buttons>();

        private Dictionary<Action, DynamicInput> mDynamicBinds = new Dictionary<Action, DynamicInput>();

        public InputState()
            : base()
        {
            
        }

        public ActionState GetAction( Action action )
        {
            if ( mKeyPressedBinds.ContainsKey( action )
                 && Keyboard_WasKeyPressed( mKeyPressedBinds[ action ] ) )
                return new ActionState( action, true );

            if ( mKeyReleasedBinds.ContainsKey( action )
                 && Keyboard_WasKeyReleased( mKeyReleasedBinds[ action ] ) )
                return new ActionState( action, true );

            if ( mKeyBinds.ContainsKey( action )
                 && Keyboard.IsKeyDown( mKeyBinds[ action ] ) )
                return new ActionState( action, true );


            if ( mButtonPressedBinds.ContainsKey( action )
                 && GamePad_WasButtonPressed( mButtonPressedBinds[ action ] ) )
                return new ActionState( action, true );

            if ( mButtonReleasedBinds.ContainsKey( action )
                 && GamePad_WasButtonReleased( mButtonReleasedBinds[ action ] ) )
                return new ActionState( action, true );

            if ( mButtonBinds.ContainsKey( action )
                 && GamePad.IsButtonDown( mButtonBinds[ action ] ) )
                return new ActionState( action, true );


            if ( mDynamicBinds.ContainsKey( action ) )
            {
                var analogResult = mDynamicBinds[ action ]( this );
                if ( analogResult > 0 )
                    return new ActionState( action, analogResult );
            }

            return new ActionState( action, false );
        }


        // Key

        public void AddBinding( Action action, Keys key )
        {
            AddBindings( new KeyValuePair<Action, Keys>( action, key ) );
        }

        public void AddBindings( params KeyValuePair<Action, Keys>[] bindings )
        {
            AddBindings( bindings.AsEnumerable() );
        }

        public void AddBindings( IEnumerable<KeyValuePair<Action, Keys>> bindings )
        {
            foreach ( var binding in bindings )
                mKeyBinds.Add( binding.Key, binding.Value );
        }

        // Key Pressed

        public void AddPressedBinding( Action action, Keys key )
        {
            AddPressedBindings( new KeyValuePair<Action, Keys>( action, key ) );
        }

        public void AddPressedBindings( params KeyValuePair<Action, Keys>[] bindings )
        {
            AddPressedBindings( bindings.AsEnumerable() );
        }

        public void AddPressedBindings( IEnumerable<KeyValuePair<Action, Keys>> bindings )
        {
            foreach ( var binding in bindings )
                mKeyPressedBinds.Add( binding.Key, binding.Value );
        }

        // Key Released

        public void AddReleasedBinding( Action action, Keys key )
        {
            AddReleasedBindings( new KeyValuePair<Action, Keys>( action, key ) );
        }

        public void AddReleasedBindings( params KeyValuePair<Action, Keys>[] bindings )
        {
            AddReleasedBindings( bindings.AsEnumerable() );
        }

        public void AddReleasedBindings( IEnumerable<KeyValuePair<Action, Keys>> bindings )
        {
            foreach ( var binding in bindings )
                mKeyReleasedBinds.Add( binding.Key, binding.Value );
        }


        // Button

        public void AddBinding( Action action, Buttons button )
        {
            AddBindings( new KeyValuePair<Action, Buttons>( action, button ) );
        }

        public void AddBindings( params KeyValuePair<Action, Buttons>[] bindings )
        {
            AddBindings( bindings.AsEnumerable() );
        }

        public void AddBindings( IEnumerable<KeyValuePair<Action, Buttons>> bindings )
        {
            foreach ( var binding in bindings )
                mButtonBinds.Add( binding.Key, binding.Value );
        }

        // Button Pressed

        public void AddPressedBinding( Action action, Buttons button )
        {
            AddPressedBindings( new KeyValuePair<Action, Buttons>( action, button ) );
        }

        public void AddPressedBindings( params KeyValuePair<Action, Buttons>[] bindings )
        {
            AddPressedBindings( bindings.AsEnumerable() );
        }

        public void AddPressedBindings( IEnumerable<KeyValuePair<Action, Buttons>> bindings )
        {
            foreach ( var binding in bindings )
                mButtonPressedBinds.Add( binding.Key, binding.Value );
        }

        // Button Released

        public void AddReleasedBinding( Action action, Buttons button )
        {
            AddReleasedBindings( new KeyValuePair<Action, Buttons>( action, button ) );
        }

        public void AddReleasedBindings( params KeyValuePair<Action, Buttons>[] bindings )
        {
            AddReleasedBindings( bindings.AsEnumerable() );
        }

        public void AddReleasedBindings( IEnumerable<KeyValuePair<Action, Buttons>> bindings )
        {
            foreach ( var binding in bindings )
                mButtonReleasedBinds.Add( binding.Key, binding.Value );
        }


        // Dynamic Input

        public void AddBinding( Action action, DynamicInput dynamicInput )
        {
            AddBindings( new KeyValuePair<Action, DynamicInput>( action, dynamicInput ) );
        }

        public void AddBindings( params KeyValuePair<Action, DynamicInput>[] bindings )
        {
            AddBindings( bindings.AsEnumerable() );
        }

        public void AddBindings( IEnumerable<KeyValuePair<Action, DynamicInput>> bindings )
        {
            foreach ( var binding in bindings )
                mDynamicBinds.Add( binding.Key, binding.Value );
        }

    }
}
