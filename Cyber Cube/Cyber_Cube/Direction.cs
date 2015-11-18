using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube
{
    /// <summary>
    /// Cardinal directions on a compass.
    /// </summary>
    public enum CompassDirection
    {
        North, East, South, West
    }

    /// <summary>
    /// Wrapper struct for the enumerated CompassDirection type. Supplies various operators for 
    /// working with directions.
    /// </summary>
    public struct Direction : IEquatable< Direction >, IEquatable< CompassDirection >
    {
        /// <summary>
        /// Static member representing the North direction.
        /// </summary>
        public readonly static Direction Up = new Direction( CompassDirection.North );
        /// <summary>
        /// Static member representing the East direction.
        /// </summary>
        public readonly static Direction Right = new Direction( CompassDirection.East );
        /// <summary>
        /// Static member representing the South direction.
        /// </summary>
        public readonly static Direction Down = new Direction( CompassDirection.South );
        /// <summary>
        /// Static member representing the West direction.
        /// </summary>
        public readonly static Direction Left = new Direction( CompassDirection.West );


        /// <summary>
        /// The internal state of the direction.
        /// </summary>
        private readonly CompassDirection value;

        /// <summary>
        /// Private constructor prevents instantiation by other classes.
        /// </summary>
        private Direction( CompassDirection direction )
        {
            value = direction;
        }

        /// <summary>
        /// Converts a direction enumerated type to its corresponding direction struct type.
        /// </summary>
        /// <param name="src">Enumerated direction to convert from.</param>
        public static implicit operator Direction( CompassDirection src )
        {
            return new Direction( src );
        }

        /// <summary>
        /// Converts a direction struct type to its corresponding direction enumerated type.
        /// </summary>
        /// <param name="src">Direction struct to convert from.</param>
        public static implicit operator CompassDirection( Direction src )
        {
            return src.value;
        }

        public static explicit operator Direction( float angle )
        {
            return Direction.FromAngle( angle );
        }

        public static Direction FromAngle( float angle )
        {
            angle = MathHelper.WrapAngle( angle );

            const float _45deg = MathHelper.PiOver4;
            const float _135deg = MathHelper.PiOver2 + MathHelper.PiOver4;

            if ( -_45deg < angle && angle <= _45deg )
                return Direction.Up;

            if ( _45deg < angle && angle <= _135deg )
                return Direction.Right;

            //if ( _135deg < angle || angle <= -_135deg )
            //    return Direction.Down;

            if ( -_135deg < angle && angle <= -_45deg )
                return Direction.Left;

            return Direction.Down;
        }

        /// <summary>
        /// Gets the direction expressed as radians, offset from the *north* position.
        /// </summary>
        public float Angle
        {
            get {
                return MathHelper.PiOver2 * (int) value;
            }
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
