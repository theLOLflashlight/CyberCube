using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{
    public enum CompassDirections
    {
        North, East, South, West
    }

    public struct Direction : IEquatable< Direction >, IEquatable< CompassDirections >
    {
        public readonly static Direction North = new Direction( CompassDirections.North );
        public readonly static Direction East = new Direction( CompassDirections.East );
        public readonly static Direction South = new Direction( CompassDirections.South );
        public readonly static Direction West = new Direction( CompassDirections.West );

        private readonly CompassDirections value;

        private Direction( CompassDirections direction )
        {
            value = direction;
        }

        public static implicit operator Direction( CompassDirections src )
        {
            return new Direction( src );
        }

        public static implicit operator CompassDirections( Direction src )
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
            return new Direction( (CompassDirections) (((int) direction.value + 2) % 4) );
        }

        /// <summary>
        /// Gets the next direction.
        /// </summary>
        /// <param name="direction">The source direction.</param>
        /// <returns>The result of rotating a direction 90deg clockwise.</returns>
        public static Direction operator +( Direction direction )
        {
            return new Direction( (CompassDirections) (((int) direction.value + 1) % 4) );
        }

        /// <summary>
        /// Gets the previous direction.
        /// </summary>
        /// <param name="direction">The source direction.</param>
        /// <returns>The result of rotating a direction 90deg anticlockwise.</returns>
        public static Direction operator -( Direction direction )
        {
            return new Direction( (CompassDirections) (((int) direction.value + 3) % 4) );
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

        public bool Equals( CompassDirections other )
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
