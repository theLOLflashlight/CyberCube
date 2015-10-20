using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{
    public static class Utils
    {

        public static ButtonState[] GetPressedButtons( this GamePadButtons gpButtons )
        {
            ButtonState[] buttons = {
                gpButtons.A,
                gpButtons.B,
                gpButtons.Back,
                gpButtons.BigButton,
                gpButtons.LeftShoulder,
                gpButtons.LeftStick,
                gpButtons.RightShoulder,
                gpButtons.RightStick,
                gpButtons.Start,
                gpButtons.X,
                gpButtons.Y
            };

            return buttons;
        }

        public static Vector2 Measure( this SpriteFont font )
        {
            return font.MeasureString( "_" );
        }

        public static void DrawRect( this SpriteBatch batch, Rectangle rect, Color color )
        {
            Texture2D texture = new Texture2D( batch.GraphicsDevice, 1, 1 );
            texture.SetData( new Color[] { Color.White } );
            batch.Draw( texture, rect, color );
        }

        public static bool Contains( this Rectangle rec, Vector2 vec, float e = 0 )
        {
            return vec.X + e >= rec.Left && vec.X - e <= rec.Right
                 && vec.Y + e >= rec.Top && vec.Y - e <= rec.Bottom;
        }

        public static float Clamp( float n, float min, float max )
        {
            return Math.Max( min, Math.Min( max, n ) );
        }

        public static Vector2 NearestPointOn( this Vector2 vec, Rectangle rec )
        {
            var x = vec.X;
            var y = vec.Y;

            var l = rec.Left;
            var t = rec.Top;
            var r = rec.Right;
            var b = rec.Bottom;

            x = Clamp( x, l, r );
            y = Clamp( y, t, b );

            var dl = Math.Abs( x - l );
            var dr = Math.Abs( x - r );
            var dt = Math.Abs( y - t );
            var db = Math.Abs( y - b );

            var m = Math.Min( Math.Min( dl, dr ), Math.Min( dt, db ) );

            if ( m == dt )
                return new Vector2( x, t );

            if ( m == db )
                return new Vector2( x, b );

            if ( m == dl )
                return new Vector2( l, y );

            return new Vector2( r, y );
        }

        public static void FloatApproach( ref float f0, float f1, float step )
        {
            if ( f0 > f1 )
                f0 -= Math.Min( f0 - f1, step );

            else if ( f0 < f1 )
                f0 += Math.Min( f1 - f0, step );
        }

        public static float Lerp( this float f0, float f1, float step )
        {
            if ( f0 > f1 )
                return f0 - Math.Min( f0 - f1, step );

            if ( f0 < f1 )
                return f0 + Math.Min( f1 - f0, step );

            return f0;
        }

        /// <summary>
        /// Performs a linear interpolation between another vector.
        /// </summary>
        public static Vector3 Lerp( this Vector3 v0, Vector3 v1, float amount )
        {
            return Vector3.Lerp( v0, v1, amount );
        }

        /// <summary>
        /// Performs a spherical linear interpolation between another vector, using 
        /// a quaternion transformation.
        /// </summary>
        public static Vector3 Slerp( this Vector3 v0, Vector3 v1, float amount )
        {
            return Vector3.Transform(
                       v0,
                       Quaternion.Slerp(
                           Quaternion.Identity,
                           v0.RotateOntoQ( v1 ),
                           amount ) );
        }

        public static Vector2 Rotate( this Vector2 position, float radians )
        {
            return Vector2.Transform( position, Matrix.CreateRotationZ( radians ) );
        }

        public static Vector3 Rotate( this Vector3 position, Vector3 normal, float radians )
        {
            return Vector3.Transform( position, Quaternion.CreateFromAxisAngle( normal, radians ) );
        }

        /// <summary>
        /// Computes the matrix required to rotate the vector so it is parallel with another.
        /// </summary>
        public static Matrix RotateOntoM( this Vector3 v1, Vector3 v2 )
        {
            if ( v1 == v2 )
                return Matrix.Identity;

            v1.Normalize();
            v2.Normalize();

            double angle = Math.Acos( Vector3.Dot( v1, v2 ) );
            if ( angle == 0 )
                return Matrix.Identity;

            var axis = Vector3.Cross( v1, angle != Math.PI
                                          ? v2
                                          : v1 != Vector3.One
                                            ? Vector3.One
                                            : Vector3.Up );
            axis.Normalize();

            return Matrix.CreateFromAxisAngle( axis, (float) angle );
        }

        /// <summary>
        /// Computes the quaternion required to rotate the vector so it is parallel with another.
        /// </summary>
        public static Quaternion RotateOntoQ( this Vector3 v1, Vector3 v2 )
        {
            if ( v1 == v2 )
                return Quaternion.Identity;

            v1.Normalize();
            v2.Normalize();

            double angle = Math.Acos( Vector3.Dot( v1, v2 ) );
            if ( angle == 0 )
                return Quaternion.Identity;

            var axis = Vector3.Cross( v1, angle != Math.PI
                                          ? v2
                                          : v1 != Vector3.One
                                            ? Vector3.One
                                            : Vector3.Up );
            axis.Normalize();

            return Quaternion.CreateFromAxisAngle( axis, (float) angle );
        }

        public static void RotateOnto( this Vector3 v1, Vector3 v2, out Matrix result )
        {
            if ( v1 == v2 )
            {
                result = Matrix.Identity;
                return;
            }

            v1.Normalize();
            v2.Normalize();

            double angle = Math.Acos( Vector3.Dot( v1, v2 ) );
            if ( angle == 0 )
            {
                result = Matrix.Identity;
                return;
            }

            var axis = Vector3.Cross( v1, angle != Math.PI
                                          ? v2
                                          : v1 != Vector3.One
                                            ? Vector3.One
                                            : Vector3.Up );
            axis.Normalize();

            Matrix.CreateFromAxisAngle( ref axis, (float) angle, out result );
        }

        public static void RotateOnto( this Vector3 v1, Vector3 v2, out Quaternion result )
        {
            if ( v1 == v2 )
            {
                result = Quaternion.Identity;
                return;
            }

            v1.Normalize();
            v2.Normalize();

            double angle = Math.Acos( Vector3.Dot( v1, v2 ) );
            if ( angle == 0 )
            {
                result = Quaternion.Identity;
                return;
            }

            var axis = Vector3.Cross( v1, angle != Math.PI
                                          ? v2
                                          : v1 != Vector3.One
                                            ? Vector3.One
                                            : Vector3.Up );
            axis.Normalize();

            Quaternion.CreateFromAxisAngle( ref axis, (float) angle, out result );
        }


        public static Vector2 Rounded( this Vector2 vec )
        {
            return RoundVector( vec );
        }

        public static Vector3 Rounded( this Vector3 vec )
        {
            return RoundVector( vec );
        }

        public static Vector4 Rounded( this Vector4 vec )
        {
            return RoundVector( vec );
        }

        public static Vector2 RoundVector( Vector2 vec )
        {
            vec.X = (float) Math.Round( vec.X );
            vec.Y = (float) Math.Round( vec.Y );
            return vec;
        }

        public static Vector3 RoundVector( Vector3 vec )
        {
            vec.X = (float) Math.Round( vec.X );
            vec.Y = (float) Math.Round( vec.Y );
            vec.Z = (float) Math.Round( vec.Z );
            return vec;
        }

        public static Vector4 RoundVector( Vector4 vec )
        {
            vec.W = (float) Math.Round( vec.W );
            vec.X = (float) Math.Round( vec.X );
            vec.Y = (float) Math.Round( vec.Y );
            vec.Z = (float) Math.Round( vec.Z );
            return vec;
        }

        public static Vector2 Transform( this Vector2 position, Matrix matrix )
        {
            return Vector2.Transform( position, matrix );
        }

        public static Vector2 Transform( this Vector2 position, Quaternion rotation )
        {
            return Vector2.Transform( position, rotation );
        }

        public static Vector3 Transform( this Vector3 position, Matrix matrix )
        {
            return Vector3.Transform( position, matrix );
        }

        public static Vector3 Transform( this Vector3 position, Quaternion rotation )
        {
            return Vector3.Transform( position, rotation );
        }

    }
}
