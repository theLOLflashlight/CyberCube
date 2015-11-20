using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Tools
{
    public static class MathTools
    {

        public static T Min< T >( T arg1, params T[] args )
            where T : IComparable< T >
        {
            T min = arg1;

            foreach ( T arg in args )
                if ( arg.CompareTo( min ) < 0 )
                    min = arg;

            return min;
        }

        public static T Max< T >( T arg1, params T[] args )
            where T : IComparable< T >
        {
            T max = arg1;

            foreach ( T arg in args )
                if ( arg.CompareTo( max ) > 0 )
                    max = arg;

            return max;
        }

        public static bool AnglesWithinRange( float angleA, float angleB, float range )
        {
            return Math.Abs( MathHelper.WrapAngle( angleA - angleB ) ) <= range;
        }

        public static float TransformRange( float x, float mina, float maxa, float minb, float maxb, bool clamp = false )
        {
            return !clamp ? TransformRange( x, mina, maxa, minb, maxb )
                : MathHelper.Clamp( TransformRange( x, mina, maxa, minb, maxb ), minb, maxb );
        }

        public static IEnumerable<float> TransformRange( IEnumerable<float> range, float mina, float maxa, float minb, float maxb, bool clamp = false )
        {
            foreach ( float x in range )
                yield return TransformRange( x, mina, maxa, minb, maxb, clamp );
        }

        private static float TransformRange( float x, float mina, float maxa, float minb, float maxb )
        {
            return (x - mina) * ((maxb - minb) / (maxa - mina)) + minb;
        }
    }
}
