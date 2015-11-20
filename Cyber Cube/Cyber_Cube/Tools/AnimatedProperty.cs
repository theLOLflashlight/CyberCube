using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CyberCube.Tools
{
    public class AnimatedProperty< T, Delta >
        where T : IEquatable< T >
    {
        public delegate void ValueChangedEventHandler( AnimatedProperty< T, Delta > sender, T value );

        public delegate T ValueInterpolator( T t0, T t1, Delta amount );

        public delegate T Getter();
        public delegate void Setter( T value );


        private T mValue0
        {
            get {
                return mGet();
            }
            set {
                mSet( value );
            }
        }
        private T mValue1;
        private ValueInterpolator mInterpolator;

        private readonly Getter mGet;
        private readonly Setter mSet;

        public event ValueChangedEventHandler ValueChanged;

        #region Constructors
        public AnimatedProperty( ValueInterpolator interpolator, Getter getter, Setter setter )
        {
            if ( interpolator == null )
                throw new ArgumentNullException( nameof( interpolator ) );
            if ( getter == null )
                throw new ArgumentNullException( nameof( getter ) );
            if ( setter == null )
                throw new ArgumentNullException( nameof( setter ) );

            mGet = getter;
            mSet = setter;
            mValue1 = mValue0;
            mInterpolator = interpolator;
        }

        public AnimatedProperty( T value, ValueInterpolator interpolator, Getter getter, Setter setter )
            : this( interpolator, getter, setter )
        {
            if ( value == null )
                throw new ArgumentNullException( nameof( value ) );

            mValue0 = mValue1 = value;
        }

        public AnimatedProperty( ValueInterpolator interpolator, PropertyInfo property, object obj, object[] index = null )
            : this( interpolator,
                    () => (T) property.GetValue( obj, index ),
                    v => property.SetValue( obj, v, index ) )
        {
        }
        #endregion

        public ValueInterpolator Interpolator
        {
            set {
                if ( value == null )
                    throw new ArgumentNullException( nameof( value ) );

                mInterpolator = value;
            }
        }

        private void OnValueChanged( T value )
        {
            ValueChanged?.Invoke( this, value );
        }

        public static implicit operator T( AnimatedProperty<T, Delta> animProp )
        {
            return animProp.Value;
        }

        public T Value
        {
            get {
                return mValue0;
            }
            set {
                if ( value == null )
                    throw new ArgumentNullException( nameof( value ) );

                bool changed = !mValue0.Equals( value );
                mValue0 = mValue1 = value;

                if ( changed )
                    OnValueChanged( value );
            }
        }

        public void AnimateValue( T value )
        {
            if ( value == null )
                throw new ArgumentNullException( nameof( value ) );

            mValue1 = value;
        }

        public void SkipAnimation()
        {
            mValue0 = mValue1;
        }

        public bool IsAnimating
        {
            get {
                return !mValue0.Equals( mValue1 );
            }
        }

        public void Step( Delta amount )
        {
            if ( !mValue0.Equals( mValue1 ) )
                OnValueChanged( mValue0 = mInterpolator( mValue0, mValue1, amount ) );
        }

    }
}
