using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{
    public enum CompassDirection
    {
        North, East, South, West
    }

    public struct Direction : IEquatable< Direction >, IEquatable< CompassDirection >
    {
        public readonly static Direction North = new Direction( CompassDirection.North );
        public readonly static Direction East = new Direction( CompassDirection.East );
        public readonly static Direction South = new Direction( CompassDirection.South );
        public readonly static Direction West = new Direction( CompassDirection.West );

        private readonly CompassDirection value;

        private Direction( CompassDirection direction )
        {
            value = direction;
        }

        public static implicit operator Direction( CompassDirection src )
        {
            return new Direction( src );
        }

        public static implicit operator CompassDirection( Direction src )
        {
            return src.value;
        }

        public float ToRadians()
        {
            return -MathHelper.PiOver2 * (int) value;
        }

        /// <summary>
        /// Gets the opposite direction.
        /// </summary>
        /// <param name="direction">The source direction.</param>
        /// <returns>The result of inverting a direction.</returns>
        public static Direction operator ~( Direction direction )
        {
            return new Direction( (CompassDirection) (((int) direction.value + 2) % 4) );
        }

        /// <summary>
        /// Gets the next direction.
        /// </summary>
        /// <param name="direction">The source direction.</param>
        /// <returns>The result of rotating a direction 90deg clockwise.</returns>
        public static Direction operator +( Direction direction )
        {
            return new Direction( (CompassDirection) (((int) direction.value + 1) % 4) );
        }

        /// <summary>
        /// Gets the previous direction.
        /// </summary>
        /// <param name="direction">The source direction.</param>
        /// <returns>The result of rotating a direction 90deg anticlockwise.</returns>
        public static Direction operator -( Direction direction )
        {
            return new Direction( (CompassDirection) (((int) direction.value + 3) % 4) );
        }

        /// <summary>
        /// Increments a direction object by rotating 90deg clockwise.
        /// </summary>
        /// <param name="direction">The source direction.</param>
        /// <returns>The source direction object.</returns>
        public static Direction operator ++( Direction direction )
        {
            return +direction;
        }

        /// <summary>
        /// Decrements a direction object by rotating 90deg anticlockwise.
        /// </summary>
        /// <param name="direction">The source direction.</param>
        /// <returns>The source direction object.</returns>
        public static Direction operator --( Direction direction )
        {
            return -direction;
        }

        public static bool operator ==( Direction a, Direction b )
        {
            return a.Equals( b );
        }

        public static bool operator !=( Direction a, Direction b )
        {
            return !a.Equals( b );
        }

        public bool Equals( Direction other )
        {
            return value == other.value;
        }

        public bool Equals( CompassDirection other )
        {
            return value == other;
        }

        public override bool Equals( object obj )
        {
            if ( obj is Direction )
                return this.Equals( (Direction) obj );

            return false;
        }

        public override int GetHashCode()
        {
            return (int) value;
        }

    }
}
