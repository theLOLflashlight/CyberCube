using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{
    public static class Utils
    {

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

        public static void Vector3Approach( ref Vector3 variable, Vector3 target, float step )
        {
            FloatApproach( ref variable.X, target.X, step );
            FloatApproach( ref variable.Y, target.Y, step );
            FloatApproach( ref variable.Z, target.Z, step );
        }

    }
}
