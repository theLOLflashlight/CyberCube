using FarseerPhysics.Dynamics;
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

        public static class Categories
        {
            public const Category DEFAULT = Category.Cat1;
            public const Category PLAYER = Category.Cat2;
            public const Category ENEMY = Category.Cat3;
            public const Category NPC = Category.Cat4;
            public const Category ACTORS = PLAYER | ENEMY | NPC;
        }


        /// <summary>
        /// Finds the first fixture attached to the body whose UserData matches the supplied value.
        /// </summary>
        /// <param name="body">The body to search</param>
        /// <param name="userData">The object to match against.</param>
        public static Fixture FindFixture( this Body body, object userData )
        {
            if ( userData == null )
                throw new ArgumentNullException( nameof( userData ) );

            foreach ( Fixture f in body.FixtureList )
                if ( userData.Equals( f.UserData ) )
                    return f;

            return null;
        }

        /// <summary>
        /// Finds all the fixtures attached to the body whose UserData matches the supplied value.
        /// </summary>
        /// <param name="body">The body to search</param>
        /// <param name="userData">The object to match against.</param>
        public static IEnumerable<Fixture> FindFixtures( this Body body, object userData )
        {
            if ( userData == null )
                throw new ArgumentNullException( nameof( userData ) );

            foreach ( Fixture f in body.FixtureList )
                if ( userData.Equals( f.UserData ) )
                    yield return f;
        }


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
