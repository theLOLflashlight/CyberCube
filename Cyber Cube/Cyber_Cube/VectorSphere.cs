using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{
    public struct VectorSphere
    {
        /// <summary>
        /// Radial coordinate.
        /// </summary>
        public float R;

        /// <summary>
        /// Azimuthal coordnate.
        /// </summary>
        public float A;

        /// <summary>
        /// Polar coordinate.
        /// </summary>
        public float P;

        public VectorSphere( float radial, float azimuthal, float polar )
        {
            R = radial;
            A = azimuthal;
            P = polar;
        }

        public static explicit operator VectorSphere( Vector3 v )
        {
            VectorSphere outVec = new VectorSphere();

            outVec.R = v.Length(); //(float) Math.Sqrt( (v.X * v.X) + (v.Y * v.Y) + (v.Z * v.Z) );

            if ( v.X == 0 )
                v.X = float.Epsilon;

            outVec.P = (float) Math.Atan( v.Z / v.X );
            if ( v.X < 0 )
                outVec.P += MathHelper.Pi;

            outVec.A = (float) Math.Asin( v.Y / outVec.R );

            return outVec;
        }

        public static explicit operator Vector3( VectorSphere v )
        {
            float a = v.R * (float) Math.Cos( v.A );

            return new Vector3( a * (float) Math.Cos( v.P ),
                                v.R * (float) Math.Sin( v.A ),
                                a * (float) Math.Sin( v.P ) );
        }
    }
}
