using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CyberCube
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

        public static object GetValue( this PropertyInfo property, object obj )
        {
            return property.GetValue( obj, null );
        }

        public static void SetValue( this PropertyInfo property, object obj, object value )
        {
            property.SetValue( obj, value, null );
        }

        public static Vector2 Measure( this SpriteFont font )
        {
            return font.MeasureString( "_" );
        }

        public static bool Contains( this Rectangle rec, Vector2 vec, float e = 0 )
        {
            return vec.X + e >= rec.Left && vec.X - e <= rec.Right
                 && vec.Y + e >= rec.Top && vec.Y - e <= rec.Bottom;
        }

        public static float Cross( Vector2 v, Vector2 w )
        {
            return v.X * w.Y - v.Y * w.X;
        }

        public static Vector2 NearestPointOn( this Vector2 vec, Rectangle rec )
        {
            var x = vec.X;
            var y = vec.Y;

            var l = rec.Left;
            var t = rec.Top;
            var r = rec.Right;
            var b = rec.Bottom;

            x = MathHelper.Clamp( x, l, r );
            y = MathHelper.Clamp( y, t, b );

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

        public static void Lerp( ref float f0, float f1, float step )
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

    }
}
