using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube
{
    public struct Line2
    {
        public Vector2 P0;
        public Vector2 P1;

        public Line2( Vector2 p0, Vector2 p1 )
        {
            P0 = p0;
            P1 = p1;
        }

        public Line2( float x0, float y0, float x1, float y1 )
        {
            P0 = new Vector2( x0, y0 );
            P1 = new Vector2( x1, y1 );
        }

        private static float Cross( Vector2 v, Vector2 w )
        {
            return v.X * w.Y - v.Y * w.X;
        }

        public Vector2? Intersection( Vector2 p0, Vector2 p1 )
        {
            return Intersection( new Line2( p0, p1 ) );
        }

        public Vector2? Intersection( Line2 l )
        {
            var p = P0;
            var r = P1 - P0;
            var q = l.P0;
            var s = l.P1 - l.P0;

            var qp_s = (float) Math.Round( Cross( q - p, s ), 3 );
            var qp_r = (float) Math.Round( Cross( q - p, r ), 3 );
            var r_s = (float) Math.Round( Cross( r, s ), 3 );

            float t = qp_s / r_s;
            float u = qp_r / r_s;

            if ( r_s == 0 )
            {
                if ( qp_r == 0 )
                {
                    var t0 = Vector2.Dot( q - p, r ) / Vector2.Dot( r, r );
                    var t1 = t0 + Vector2.Dot( s, r ) / Vector2.Dot( r, r );

                    if ( Vector2.Dot( s, r ) >= 0 )
                    {
                        if ( t0 <= 1 && t1 >= 0 )
                            return q;
                    }
                    else if ( t1 <= 1 && t0 >= 0 )
                    {
                        return q;
                    }
                }
                else
                {
                    return null;
                }
            }
            else if ( (0 <= t && t <= 1) && (0 <= u && u <= 1) )
            {
                return p + t*r;
            }

            return null;
        }

        public Vector2 this[ float t ]
        {
            get {
                return P0 + (t * (P1 - P0));
            }
        }
    }

    public struct Line3
    {
        public Vector3 P0;
        public Vector3 P1;

        public Line3( Vector3 p0, Vector3 p1 )
        {
            P0 = p0;
            P1 = p1;
        }

        public Vector3 this[ float t ]
        {
            get {
                return P0 + (t * (P1 - P0));
            }
        }
    }
}
