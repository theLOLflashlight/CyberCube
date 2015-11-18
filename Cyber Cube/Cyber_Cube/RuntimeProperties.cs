using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CyberCube
{
    public abstract class RuntimeProperties
    {

        public struct EvaluateResult
        {
            public readonly bool Success;
            public readonly object Result;

            public EvaluateResult( object result )
                : this( true, result )
            {
            }

            public EvaluateResult( bool success, object result )
            {
                Success = success;
                Result = result;
            }
        }


        public EvaluateResult Evaluate( string expression )
        {
            string[] tokens = expression.Split( ' ' );

            if ( tokens.Length == 0 )
                return new EvaluateResult( null );

            foreach ( var property in GetType().GetProperties() )
            {
                if ( tokens[ 0 ].ToLower() == property.Name.ToLower() )
                {
                    if ( tokens.Length == 1 )
                    {
                        return new EvaluateResult( property.GetValue( this, null ) );
                    }
                    else if ( tokens.Length >= 3 && tokens[ 1 ] == "=" )
                    {
                        try {
                            string subExpression = "";
                            for ( int i = 2; i < tokens.Length; ++i )
                                subExpression += tokens[ i ] + " ";

                            property.SetValue(
                                this,
                                Parse( subExpression.TrimEnd(), property.PropertyType ),
                                null );

                            return new EvaluateResult( null );
                        }
                        catch ( TargetException )
                        {
                            throw new Exception(
                                $"{property.Name} must be of type {property.PropertyType.Name}." );
                        }
                    }
                    throw new ArgumentException( "Malformed expression." );
                }
            }
            return new EvaluateResult();
        }


        protected virtual object Parse( string value, Type type )
        {
            if ( value == null )
                throw new ArgumentNullException( nameof( value ) );

            if ( type == null )
                throw new ArgumentNullException( nameof( type ) );


            if ( type == typeof( string ) )
                return value;

            if ( type == typeof( char ) )
            {
                char c;
                if ( char.TryParse( value, out c ) )
                    return c;
            }

            if ( type == typeof( DateTime ) )
                return DateTime.Parse( value );


            if ( type == typeof( bool ) )
                return bool.Parse( value );


            if ( type == typeof( byte ) )
                return byte.Parse( value );

            if ( type == typeof( short ) )
                return short.Parse( value );

            if ( type == typeof( int ) )
                return int.Parse( value );

            if ( type == typeof( long ) )
                return long.Parse( value );


            if ( type == typeof( float ) )
                return float.Parse( value );

            if ( type == typeof( double ) )
                return double.Parse( value );

            if ( type == typeof( decimal ) )
                return decimal.Parse( value );


            return null;
        }
    }
}
