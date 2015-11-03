using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Tools
{
    public class AnimatedVariable< T, Diff >
        where T : IEquatable< T >
        where Diff : struct, IEquatable< Diff >, IComparable< Diff >
    {
        public delegate void ValueChangedListener( AnimatedVariable< T, Diff > sender, T value );

        public delegate T Interpolator( T t0, T t1, Diff amount );
        public delegate Diff Difference( T t0, T t1 );


        private T    mValue0;
        private T    mValue1;
        private Diff mAnimSpeed;

        public event ValueChangedListener OnValueChanged;

        private readonly Interpolator mInterpolator;
        private readonly Difference   mDifference;

        public AnimatedVariable( T value, Interpolator interpolator, Difference difference )
        {
            Value = value;
            mInterpolator = interpolator;
            mDifference = difference;
        }

        public T Value
        {
            get {
                return mValue0;
            }
            set {
                if ( value == null )
                    throw new ArgumentNullException( "Value cannot be null." );

                bool changed = !mValue0.Equals( value );
                mValue0 = mValue1 = value;

                if ( changed )
                    OnValueChanged?.Invoke( this, mValue0 );
            }
        }

        public Diff AnimSpeed
        {
            get {
                return mAnimSpeed;
            }
            set {
                if ( value.CompareTo( default( Diff ) ) < 0 )
                    throw new ArgumentOutOfRangeException( "AnimSpeed must be positive" );

                mAnimSpeed = value;
            }
        }

        public void AnimateValue( T value )
        {
            if ( value == null )
                throw new ArgumentNullException( "Value cannot be null." );

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

        public void Update( Diff amount )
        {
            //Diff amount = ((dynamic) AnimSpeed) * seconds;

            if ( !mValue0.Equals( mValue1 ) )
            {
                mValue0 = mDifference( mValue0, mValue1 ).CompareTo( amount ) > 0
                          ? mInterpolator( mValue0, mValue1, amount )
                          : mValue1;

                OnValueChanged?.Invoke( this, mValue0 );
            }
        }

    }

    public class AnimatedProperty<T>
    {
        public delegate T Getter();
        public delegate void Setter( T value );

        private T mValue1;
        private readonly Getter mGet;
        private readonly Setter mSet;

        public AnimatedProperty( Getter getter, Setter setter, T value )
        {
            mGet = getter;
            mSet = setter;
            mValue1 = value;
        }

        public T Value
        {
            get {
                return mGet();
            }
            set {
                mSet( value );
            }
        }

    }
}
