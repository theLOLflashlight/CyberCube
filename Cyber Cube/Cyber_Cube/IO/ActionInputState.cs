﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.IO
{

    public class InputState< BindTarget > : InputState
        where BindTarget : struct, IComparable, IFormattable, IConvertible
    {
        public delegate float DynamicInput( InputState< BindTarget > input );

        public struct ActionState
        {
            public readonly BindTarget Action;
            public readonly float Value;

            public ActionState( BindTarget action, float value )
            {
                Action = action;
                Value = value;
            }

            public ActionState( BindTarget action, bool value )
                : this( action, value ? 1f : 0f )
            {
            }

            public static implicit operator float( ActionState input )
            {
                return input.Value;
            }

            public static implicit operator bool( ActionState input )
            {
                return input.Value != 0;
            }
        }

        private Dictionary<BindTarget, Keys> mKeyBinds = new Dictionary<BindTarget, Keys>();
        private Dictionary<BindTarget, Keys> mKeyPressedBinds = new Dictionary<BindTarget, Keys>();
        private Dictionary<BindTarget, Keys> mKeyReleasedBinds = new Dictionary<BindTarget, Keys>();

        private Dictionary<BindTarget, Buttons> mButtonBinds = new Dictionary<BindTarget, Buttons>();
        private Dictionary<BindTarget, Buttons> mButtonPressedBinds = new Dictionary<BindTarget, Buttons>();
        private Dictionary<BindTarget, Buttons> mButtonReleasedBinds = new Dictionary<BindTarget, Buttons>();

        private Dictionary<BindTarget, DynamicInput> mDynamicBinds = new Dictionary<BindTarget, DynamicInput>();

        public InputState()
            : base()
        {
        }

        /// <summary>
        /// Gets the action state for the supplied action. If the current input state 
        /// has a focus, this method will return a default action state if the sender 
        /// is not that focus.
        /// </summary>
        /// <param name="action">The action to check.</param>
        /// <param name="sender">The user of this method.</param>
        public ActionState GetAction( BindTarget action, object sender )
        {
            if ( !HasFocus || CheckFocus( sender ) )
                return GetAction( action );

            return default( ActionState );
        }

        public ActionState GetAction( BindTarget action )
        {
            #region Key Binds
            if ( mKeyPressedBinds.ContainsKey( action )
                 && Keyboard_WasKeyPressed( mKeyPressedBinds[ action ] ) )
                return new ActionState( action, true );

            if ( mKeyReleasedBinds.ContainsKey( action )
                 && Keyboard_WasKeyReleased( mKeyReleasedBinds[ action ] ) )
                return new ActionState( action, true );
            #endregion

            if ( mKeyBinds.ContainsKey( action )
                 && Keyboard.IsKeyDown( mKeyBinds[ action ] ) )
                return new ActionState( action, true );

            #region Button Binds
            if ( mButtonPressedBinds.ContainsKey( action )
                 && GamePad_WasButtonPressed( mButtonPressedBinds[ action ] ) )
                return new ActionState( action, true );

            if ( mButtonReleasedBinds.ContainsKey( action )
                 && GamePad_WasButtonReleased( mButtonReleasedBinds[ action ] ) )
                return new ActionState( action, true );
            #endregion

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


        #region Key Bindings
        public void AddBinding( BindTarget action, Keys key )
        {
            AddBindings( new KeyValuePair<BindTarget, Keys>( action, key ) );
        }

        public void AddBindings( params KeyValuePair<BindTarget, Keys>[] bindings )
        {
            AddBindings( bindings.AsEnumerable() );
        }

        public void AddBindings( IEnumerable<KeyValuePair<BindTarget, Keys>> bindings )
        {
            foreach ( var binding in bindings )
                mKeyBinds.Add( binding.Key, binding.Value );
        }
        #endregion

        #region Key Pressed Bindings
        public void AddPressedBinding( BindTarget action, Keys key )
        {
            AddPressedBindings( new KeyValuePair<BindTarget, Keys>( action, key ) );
        }

        public void AddPressedBindings( params KeyValuePair<BindTarget, Keys>[] bindings )
        {
            AddPressedBindings( bindings.AsEnumerable() );
        }

        public void AddPressedBindings( IEnumerable<KeyValuePair<BindTarget, Keys>> bindings )
        {
            foreach ( var binding in bindings )
                mKeyPressedBinds.Add( binding.Key, binding.Value );
        }
        #endregion

        #region Key Released Bindings
        public void AddReleasedBinding( BindTarget action, Keys key )
        {
            AddReleasedBindings( new KeyValuePair<BindTarget, Keys>( action, key ) );
        }

        public void AddReleasedBindings( params KeyValuePair<BindTarget, Keys>[] bindings )
        {
            AddReleasedBindings( bindings.AsEnumerable() );
        }

        public void AddReleasedBindings( IEnumerable<KeyValuePair<BindTarget, Keys>> bindings )
        {
            foreach ( var binding in bindings )
                mKeyReleasedBinds.Add( binding.Key, binding.Value );
        }
        #endregion


        #region Button Bindings
        public void AddBinding( BindTarget action, Buttons button )
        {
            AddBindings( new KeyValuePair<BindTarget, Buttons>( action, button ) );
        }

        public void AddBindings( params KeyValuePair<BindTarget, Buttons>[] bindings )
        {
            AddBindings( bindings.AsEnumerable() );
        }

        public void AddBindings( IEnumerable<KeyValuePair<BindTarget, Buttons>> bindings )
        {
            foreach ( var binding in bindings )
                mButtonBinds.Add( binding.Key, binding.Value );
        }
        #endregion

        #region Button Pressed Bindings
        public void AddPressedBinding( BindTarget action, Buttons button )
        {
            AddPressedBindings( new KeyValuePair<BindTarget, Buttons>( action, button ) );
        }

        public void AddPressedBindings( params KeyValuePair<BindTarget, Buttons>[] bindings )
        {
            AddPressedBindings( bindings.AsEnumerable() );
        }

        public void AddPressedBindings( IEnumerable<KeyValuePair<BindTarget, Buttons>> bindings )
        {
            foreach ( var binding in bindings )
                mButtonPressedBinds.Add( binding.Key, binding.Value );
        }
        #endregion

        #region Button Released Bindings
        public void AddReleasedBinding( BindTarget action, Buttons button )
        {
            AddReleasedBindings( new KeyValuePair<BindTarget, Buttons>( action, button ) );
        }

        public void AddReleasedBindings( params KeyValuePair<BindTarget, Buttons>[] bindings )
        {
            AddReleasedBindings( bindings.AsEnumerable() );
        }

        public void AddReleasedBindings( IEnumerable<KeyValuePair<BindTarget, Buttons>> bindings )
        {
            foreach ( var binding in bindings )
                mButtonReleasedBinds.Add( binding.Key, binding.Value );
        }
        #endregion


        #region Dynamic Input Bindings
        public void AddBinding( BindTarget action, DynamicInput dynamicInput )
        {
            AddBindings( new KeyValuePair<BindTarget, DynamicInput>( action, dynamicInput ) );
        }

        public void AddBindings( params KeyValuePair<BindTarget, DynamicInput>[] bindings )
        {
            AddBindings( bindings.AsEnumerable() );
        }

        public void AddBindings( IEnumerable<KeyValuePair<BindTarget, DynamicInput>> bindings )
        {
            foreach ( var binding in bindings )
                mDynamicBinds.Add( binding.Key, binding.Value );
        }
        #endregion
    }
}
