using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Tools
{
    public enum Vector2Component
    {
        X, Y
    }

    public enum Vector3Component
    {
        X, Y, Z
    }

    public enum Vector4Component
    {
        X, Y, Z, W
    }

    public static class VectorUtils
    {
        #region GetComponent
        public static float GetComponent( this Vector2 vec2, Vector2Component com )
        {
            switch ( com )
            {
            case Vector2Component.X:
                return vec2.X;

            case Vector2Component.Y:
                return vec2.Y;

            default:
                throw new EnumException<Vector2Component>( nameof( com ), com );
            }
        }

        public static float GetComponent( this Vector3 vec3, Vector3Component com )
        {
            switch ( com )
            {
            case Vector3Component.X:
                return vec3.X;

            case Vector3Component.Y:
                return vec3.Y;

            case Vector3Component.Z:
                return vec3.Z;

            default:
                throw new EnumException<Vector3Component>( nameof( com ), com );
            }
        }

        public static float GetComponent( this Vector4 vec4, Vector4Component com )
        {
            switch ( com )
            {
            case Vector4Component.X:
                return vec4.X;

            case Vector4Component.Y:
                return vec4.Y;

            case Vector4Component.Z:
                return vec4.Z;

            case Vector4Component.W:
                return vec4.W;

            default:
                throw new EnumException<Vector4Component>( nameof( com ), com );
            }
        }
        #endregion

        #region ChangeComponent
        public static Vector2 ChangeComponent( this Vector2 vec2, Vector2Component com, float value )
        {
            switch ( com )
            {
            case Vector2Component.X:
                vec2.X = value;
                break;

            case Vector2Component.Y:
                vec2.Y = value;
                break;

            default:
                throw new EnumException<Vector2Component>( nameof( com ), com );
            }

            return vec2;
        }

        public static Vector3 ChangeComponent( this Vector3 vec3, Vector3Component com, float value )
        {
            switch ( com )
            {
            case Vector3Component.X:
                vec3.X = value;
                break;

            case Vector3Component.Y:
                vec3.Y = value;
                break;

            case Vector3Component.Z:
                vec3.Z = value;
                break;

            default:
                throw new EnumException<Vector3Component>( nameof( com ), com );
            }

            return vec3;
        }

        public static Vector4 ChangeComponent( this Vector4 vec4, Vector4Component com, float value )
        {
            switch ( com )
            {
            case Vector4Component.X:
                vec4.X = value;
                break;

            case Vector4Component.Y:
                vec4.Y = value;
                break;

            case Vector4Component.Z:
                vec4.Z = value;
                break;

            case Vector4Component.W:
                vec4.W = value;
                break;

            default:
                throw new EnumException<Vector4Component>( nameof( com ), com );
            }

            return vec4;
        }
        #endregion

        #region LargestComponent
        public static Vector2Component LargestComponent( this Vector2 vec2 )
        {
            float x = Math.Abs( vec2.X );
            float y = Math.Abs( vec2.Y );

            float max = MathTools.Max( x, y );

            if ( x == max )
                return Vector2Component.X;

            if ( y == max )
                return Vector2Component.Y;

            throw new Tools.WtfException();
        }

        public static Vector3Component LargestComponent( this Vector3 vec3 )
        {
            float x = Math.Abs( vec3.X );
            float y = Math.Abs( vec3.Y );
            float z = Math.Abs( vec3.Z );

            float max = MathTools.Max( x, y, z );

            if ( x == max )
                return Vector3Component.X;

            if ( y == max )
                return Vector3Component.Y;

            if ( z == max )
                return Vector3Component.Z;

            throw new Tools.WtfException();
        }

        public static Vector4Component LargestComponent( this Vector4 vec4 )
        {
            float x = Math.Abs( vec4.X );
            float y = Math.Abs( vec4.Y );
            float z = Math.Abs( vec4.Z );
            float w = Math.Abs( vec4.W );

            float max = MathTools.Max( x, y, z, w );

            if ( x == max )
                return Vector4Component.X;

            if ( y == max )
                return Vector4Component.Y;

            if ( z == max )
                return Vector4Component.Z;

            if ( w == max )
                return Vector4Component.W;

            throw new Tools.WtfException();
        }
        #endregion

        #region SmallestComponent
        public static Vector2Component SmallestComponent( this Vector2 vec2 )
        {
            float x = Math.Abs( vec2.X );
            float y = Math.Abs( vec2.Y );

            float min = MathTools.Min( x, y );

            if ( x == min )
                return Vector2Component.X;

            if ( y == min )
                return Vector2Component.Y;

            throw new Tools.WtfException();
        }

        public static Vector3Component SmallestComponent( this Vector3 vec3 )
        {
            float x = Math.Abs( vec3.X );
            float y = Math.Abs( vec3.Y );
            float z = Math.Abs( vec3.Z );

            float min = MathTools.Min( x, y, z );

            if ( x == min )
                return Vector3Component.X;

            if ( y == min )
                return Vector3Component.Y;

            if ( z == min )
                return Vector3Component.Z;

            throw new Tools.WtfException();
        }

        public static Vector4Component SmallestComponent( this Vector4 vec4 )
        {
            float x = Math.Abs( vec4.X );
            float y = Math.Abs( vec4.Y );
            float z = Math.Abs( vec4.Z );
            float w = Math.Abs( vec4.W );

            float min = MathTools.Min( x, y, z, w );

            if ( x == min )
                return Vector4Component.X;

            if ( y == min )
                return Vector4Component.Y;

            if ( z == min )
                return Vector4Component.Z;

            if ( w == min )
                return Vector4Component.W;

            throw new Tools.WtfException();
        }
        #endregion

        /// <summary>
        /// Performs a linear interpolation between another vector.
        /// </summary>
        public static Vector3 Lerp( this Vector3 v0, Vector3 v1, float amount )
        {
            Utils.Lerp( ref v0.X, v1.X, amount );
            Utils.Lerp( ref v0.Y, v1.Y, amount );
            Utils.Lerp( ref v0.Z, v1.Z, amount );
            return v0;
            //return Vector3.Lerp( v0, v1, amount );
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
                           v0.RotateOnto_Q( v1 ),
                           amount ) );
        }

        public static Vector2 Rotate( this Vector2 position, float radians )
        {
            return Vector2.Transform( position, Quaternion.CreateFromAxisAngle( Vector3.UnitZ, radians ) );
        }

        public static Vector3 Rotate( this Vector3 position, Vector3 normal, float radians )
        {
            return Vector3.Transform( position, Quaternion.CreateFromAxisAngle( normal, radians ) );
        }

        /// <summary>
        /// Computes the matrix required to rotate the vector so it is parallel with another.
        /// </summary>
        public static Matrix RotateOnto_M( this Vector3 v1, Vector3 v2 )
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
        public static Quaternion RotateOnto_Q( this Vector3 v1, Vector3 v2 )
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

    public struct HyperVector3
    {
        public delegate float CoalesceFunc( Vector3 vec3 );

        public Vector3 X, Y, Z;

        public HyperVector3( Vector3 vec3 )
        {
            Z = Y = X = vec3;
        }

        public void Cascade( Vector3 vec3 )
        {
            X.X = vec3.X;
            Y.Y = vec3.Y;
            Z.Z = vec3.Z;
        }

        public Vector3 Coalesce( CoalesceFunc coalesce )
        {
            Vector3 vec = default( Vector3 );
            vec.X = coalesce( X );
            vec.Y = coalesce( Y );
            vec.Z = coalesce( Z );
            return vec;
        }

        public void Normalize()
        {
            X.Normalize();
            Y.Normalize();
            Z.Normalize();
        }

        public static HyperVector3 Normalize( HyperVector3 hvec )
        {
            hvec.Normalize();
            return hvec;
        }

        public static HyperVector3 operator *( HyperVector3 hvec, float f )
        {
            hvec.X *= f;
            hvec.Y *= f;
            hvec.Z *= f;
            return hvec;
        }

        public static HyperVector3 operator *( HyperVector3 hvec, Vector3 vec )
        {
            hvec.X *= vec;
            hvec.Y *= vec;
            hvec.Z *= vec;
            return hvec;
        }
    }
}
