using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Tools
{
    public class AnimatedVariable< T, Delta >
        where T : IEquatable< T >, new()
    {
        public delegate void ValueChangedEventHandler( AnimatedVariable< T, Delta > sender, T value );

        public delegate T Interpolator( T t0, T t1, Delta amount );


        private T mValue0;
        private T mValue1;
        private readonly Interpolator mInterpolator;

        public event ValueChangedEventHandler ValueChanged;

        public AnimatedVariable( Interpolator interpolator )
            : this( new T(), interpolator )
        {
        }

        public AnimatedVariable( T value, Interpolator interpolator )
        {
            if ( value == null )
                throw new ArgumentNullException( nameof( value ) );
            if ( interpolator == null )
                throw new ArgumentNullException( nameof( interpolator ) );

            mValue0 = mValue1 = value;
            mInterpolator = interpolator;
        }

        private void OnValueChanged( T value )
        {
            ValueChanged?.Invoke( this, value );
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
                    OnValueChanged( mValue0 );
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

        public void Update( Delta amount )
        {
            if ( !mValue0.Equals( mValue1 ) )
            {
                mValue0 = mInterpolator( mValue0, mValue1, amount );
                OnValueChanged( mValue0 );
            }
        }

    }
}
