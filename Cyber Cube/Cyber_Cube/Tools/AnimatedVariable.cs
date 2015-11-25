using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Tools
{
    public class AnimatedVariable< T >
        where T : IEquatable< T >, new()
    {
        public delegate void ValueChangedEventHandler( AnimatedVariable< T > sender, T value );

        public delegate T ValueInterpolator( T t0, T t1, float amount );


        private T mValue0;
        private T mValue1;
        private ValueInterpolator mInterpolator;

        public event ValueChangedEventHandler ValueChanged;

        #region Constructors
        public AnimatedVariable( T value, ValueInterpolator interpolator )
        {
            if ( value == null )
                throw new ArgumentNullException( nameof( value ) );
            if ( interpolator == null )
                throw new ArgumentNullException( nameof( interpolator ) );

            mValue0 = mValue1 = value;
            mInterpolator = interpolator;
        }

        public AnimatedVariable( ValueInterpolator interpolator )
            : this( new T(), interpolator )
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

        public static implicit operator T( AnimatedVariable<T> animVar )
        {
            return animVar.Value;
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

        public void Step( float amount )
        {
            if ( !mValue0.Equals( mValue1 ) )
                OnValueChanged( mValue0 = mInterpolator( mValue0, mValue1, amount ) );
        }

    }
}
