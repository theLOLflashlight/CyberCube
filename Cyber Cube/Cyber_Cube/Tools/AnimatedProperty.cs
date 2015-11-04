using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Tools
{
    public class AnimatedProperty< T, Delta >
        where T : IEquatable< T >
        //where Delta : struct, IEquatable< Delta >, IComparable< Delta >
    {
        public delegate void ValueChangedListener( AnimatedProperty< T, Delta > sender, T value );

        public delegate T Interpolator( T t0, T t1, Delta amount );

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
        private readonly Interpolator mInterpolator;

        private readonly Getter mGet;
        private readonly Setter mSet;

        public event ValueChangedListener OnValueChanged;

        public AnimatedProperty( T value, Interpolator interpolator, Getter getter, Setter setter )
        {
            if ( value == null )
                throw new ArgumentNullException( nameof( value ) );
            if ( interpolator == null )
                throw new ArgumentNullException( nameof( interpolator ) );
            if ( getter == null )
                throw new ArgumentNullException( nameof( getter ) );
            if ( setter == null )
                throw new ArgumentNullException( nameof( setter ) );

            mGet = getter;
            mSet = setter;
            mValue0 = mValue1 = value;
            mInterpolator = interpolator;
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
                    OnValueChanged?.Invoke( this, mValue0 );
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
                OnValueChanged?.Invoke( this, mValue0 );
            }
        }

    }
}
