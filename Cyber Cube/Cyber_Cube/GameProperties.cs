using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CyberCube
{
    public abstract class GameProperties
    {

        public struct EvaluateResult
        {
            public readonly bool Success;
            public readonly object Result;

            public EvaluateResult( bool success, object result )
            {
                Success = success;
                Result = result;
            }
        }


        public EvaluateResult Evaluate( string expression )
        {
            string[] tokens = expression.Trim().Split( ' ' );

            if ( tokens.Length == 0 )
                return new EvaluateResult( true, null );

            var properties = this.GetType().GetProperties();

            foreach ( var property in properties )
            {
                if ( tokens[ 0 ].ToLower() == property.Name.ToLower() )
                {
                    if ( tokens.Length == 1 )
                    {
                        return new EvaluateResult( true, property.GetValue( this, null ) );
                    }
                    else if ( tokens.Length == 3 && tokens[ 1 ] == "=" )
                    {
                        try {
                            property.SetValue(
                                this,
                                Parse( tokens[ 2 ], property.PropertyType ),
                                null );

                            return new EvaluateResult( true, null );
                        }
                        catch ( TargetException )
                        {
                            throw new Exception(
                                $"{property.Name} must be of type {property.PropertyType.Name}." );
                        }
                    }
                    throw new ArgumentException( "Expression malformed." );
                }
            }
            return new EvaluateResult( false, null );
        }


        public static object Parse( string value, Type type )
        {
            if ( type == typeof( string ) )
                return value;

            if ( type == typeof( bool ) )
                return bool.Parse( value );

            if ( type == typeof( int ) )
                return int.Parse( value );

            if ( type == typeof( float ) )
                return float.Parse( value );

            if ( type == typeof( Color ) )
                return Color_Parse( value );

            return null;
        }

        private static Color Color_Parse( string value )
        {
            var properties = typeof( Color ).GetProperties( BindingFlags.Public | BindingFlags.Static );

            foreach ( var property in properties )
                if ( property.Name.ToLower() == value.ToLower()
                     && property.PropertyType == typeof( Color ) )
                    return (Color) property.GetValue( null, null );

            throw new FormatException( $"'{value}' is not a color." );
        }
    }
}
