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
            var length = v.Length();
            return new VectorSphere( length,
                                     v.X != 0 ? (float) Math.Atan( v.Y / v.X ) : 0,
                                     (float) Math.Acos( v.Z / length ) );
        }

        public static explicit operator Vector3( VectorSphere v )
        {
            return new Vector3( (float) (v.R * Math.Sin( v.A ) * Math.Cos( v.P )),
                                (float) (v.R * Math.Sin( v.A ) * Math.Sin( v.P )),
                                (float) (v.R * Math.Cos( v.A )) );
        }
    }
}
