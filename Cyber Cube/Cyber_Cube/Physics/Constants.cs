using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Physics
{
    public static class Constants
    {

        public const float UNIT_TO_PIXEL = 100;
        public const float PIXEL_TO_UNIT = 1 / UNIT_TO_PIXEL;

        /// <summary>
        /// Converts an int to world unit scale (meters).
        /// </summary>
        /// <param name="i">An int measured in pixels.</param>
        /// <returns>A float measured in meters.</returns>
        public static float ToUnits( this int i )
        {
            return i * PIXEL_TO_UNIT;
        }

        /// <summary>
        /// Converts an int to pixel scale.
        /// </summary>
        /// <param name="i">An int measured in world units (meters).</param>
        /// <returns>A float measured in pixels.</returns>
        public static float ToPixels( this int i )
        {
            return i * UNIT_TO_PIXEL;
        }

        /// <summary>
        /// Converts a float to world unit scale (meters).
        /// </summary>
        /// <param name="f">A float measured in pixels.</param>
        /// <returns>A float measured in meters.</returns>
        public static float ToUnits( this float f )
        {
            return f * PIXEL_TO_UNIT;
        }

        /// <summary>
        /// Converts a float to pixel scale.
        /// </summary>
        /// <param name="f">A float measured in world units (meters).</param>
        /// <returns>A float measured in pixels.</returns>
        public static float ToPixels( this float f )
        {
            return f * UNIT_TO_PIXEL;
        }

        /// <summary>
        /// Converts a double to world unit scale (meters).
        /// </summary>
        /// <param name="f">A double measured in pixels.</param>
        /// <returns>A float measured in meters.</returns>
        public static float ToUnits( this double d )
        {
            return (float) d * PIXEL_TO_UNIT;
        }

        /// <summary>
        /// Converts a double to pixel scale.
        /// </summary>
        /// <param name="d">A double measured in world units (meters).</param>
        /// <returns>A float measured in pixels.</returns>
        public static float ToPixels( this double d )
        {
            return (float) d * UNIT_TO_PIXEL;
        }

        /// <summary>
        /// Converts a Vector2 to world unit scale (meters).
        /// </summary>
        /// <param name="v">A vector measured in pixels.</param>
        /// <returns>A vector measured in meters.</returns>
        public static Vector2 ToUnits( this Vector2 v )
        {
            return v * PIXEL_TO_UNIT;
        }

        /// <summary>
        /// Converts a Vector2 to pixel scale.
        /// </summary>
        /// <param name="v">A vector measured in world units (meters).</param>
        /// <returns>A vector measured in pixels.</returns>
        public static Vector2 ToPixels( this Vector2 v )
        {
            return v * UNIT_TO_PIXEL;
        }

    }
}
