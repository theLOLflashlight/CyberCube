using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{
    public static class Utils
    {

        public delegate Vector3 Vector3Interpolator( Vector3 v0, Vector3 v1, float amount );
        public delegate void Vector3InterpolatorRef( ref Vector3 v0, Vector3 v1, float amount );

        public static void FloatApproach( ref float variable, float target, float step )
        {
            if ( variable > target )
                variable -= Math.Min( variable - target, step );

            else if ( variable < target )
                variable += Math.Min( target - variable, step );
        }

        public static void Vector3Approach( ref Vector3 variable, Vector3 target, Vector3 step )
        {
            FloatApproach( ref variable.X, target.X, step.X );
            FloatApproach( ref variable.Y, target.Y, step.Y );
            FloatApproach( ref variable.Z, target.Z, step.Z );
        }

        public static Vector3 VectorLerp( Vector3 v0, Vector3 v1, float amount )
        {
            return Vector3.Lerp( v0, v1, amount );
        }

        public static void VectorLerp( ref Vector3 v0, Vector3 v1, float amount )
        {
            v0 = VectorLerp( v0, v1, amount );
        }

        public static Vector3 VectorSlerp( Vector3 v0, Vector3 v1, float amount )
        {
            return Vector3.Transform(
                       v0,
                       Quaternion.Slerp(
                           Quaternion.Identity,
                           Quaternion.CreateFromRotationMatrix(
                               Utils.RotateVecToVec(
                                   v0,
                                   v1 ) ),
                           amount ) );
        }

        public static void VectorSlerp( ref Vector3 v0, Vector3 v1, float amount )
        {
            v0 = VectorSlerp( v0, v1, amount );
        }

        public static Vector3 Slerp( this Vector3 v0, Vector3 v1, float amount )
        {
            return VectorSlerp( v0, v1, amount );
        }

        public static Vector3 RotateVector( Vector3 position, Vector3 normal, float radians )
        {
            return Vector3.Transform( position, Matrix.CreateFromAxisAngle( normal, radians ) );
        }

        public static Vector3 Rotate( this Vector3 position, Vector3 normal, float radians )
        {
            return RotateVector( position, normal, radians );
        }


        public static void RoundVector( ref Vector3 vec )
        {
            vec.X = (float) Math.Round( vec.X );
            vec.Y = (float) Math.Round( vec.Y );
            vec.Z = (float) Math.Round( vec.Z );
        }

        public static Vector3 RoundVector( Vector3 vec )
        {
            RoundVector( ref vec );
            return vec;
        }

        public static Vector3 Round( this Vector3 vec )
        {
            return RoundVector( vec );
        }

        public static void RoundVector( ref Vector2 vec )
        {
            vec.X = (float) Math.Round( vec.X );
            vec.Y = (float) Math.Round( vec.Y );
        }

        public static Vector2 RoundVector( Vector2 vec )
        {
            RoundVector( ref vec );
            return vec;
        }

        public static Vector2 Round( this Vector2 vec )
        {
            return RoundVector( vec );
        }

        public static Matrix RotateVecToVec( Vector3 v1, Vector3 v2 )
        {
            if ( v1 == v2 )
                return Matrix.Identity;

            v1.Normalize();
            v2.Normalize();

            var angle = Math.Acos( Vector3.Dot( v1, v2 ) );
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

        public static Matrix RotateOnto( this Vector3 v1, Vector3 v2 )
        {
            return RotateVecToVec( v1, v2 );
        }

        public static void Vector3SphereApproach( ref Vector3 v0, Vector3 v1, float step )
        {
            VectorSphere s0 = (VectorSphere) v0;
            VectorSphere s1 = (VectorSphere) v1;

            float thetaStep = (float) Math.Tan( step / s0.R );

            Utils.FloatApproach( ref s0.R, s1.R, step );
            Utils.FloatApproach( ref s0.A, s1.A, thetaStep );
            Utils.FloatRadianApproach( ref s0.P, s1.P, thetaStep );

            v0 = (Vector3) s0;
        }

        public static void FloatRadianApproach( ref float variable, float target, float step )
        {
            if ( variable > target )
                variable += variable <= target + MathHelper.Pi
                            ? -Math.Min( variable - target, step )
                            : Math.Min( target - variable, step );

            else if ( variable < target )
                variable += variable >= target - MathHelper.Pi
                            ? Math.Min( target - variable, step )
                            : -Math.Min( variable - target, step );
        }

    }
}
