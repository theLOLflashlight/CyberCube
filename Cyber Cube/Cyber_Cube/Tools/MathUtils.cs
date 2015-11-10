using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Tools
{
    public static class MathUtils
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

    }
}
