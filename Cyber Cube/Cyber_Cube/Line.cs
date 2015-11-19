using CyberCube.Tools;
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

        public Vector2 this[ float t ]
        {
            get {
                return P0 + (t * (P1 - P0));
            }
        }

        public float Length
        {
            get {
                return (P1 - P0).Length();
            }
        }

        public Vector2 Direction
        {
            get {
                return Vector2.Normalize( P1 - P0 );
            }
        }

        public Vector2 Center
        {
            get {
                return new Vector2( X0 + X1, Y0 + Y1 ) / 2;
            }
        }

        public bool IsHorizontal
        {
            get {
                return Y0 == Y1;
            }
        }

        public bool IsVertical
        {
            get {
                return X0 == X1;
            }
        }

        public void Reverse()
        {
            var tmp = P0;
            P0 = P1;
            P1 = tmp;
        }

        public void Reverse( out Line2 outLine )
        {
            outLine.P0 = P1;
            outLine.P1 = P0;
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

            var qp_s = (float) Math.Round( Utils.Cross( q - p, s ), 3 );
            var qp_r = (float) Math.Round( Utils.Cross( q - p, r ), 3 );
            var r_s = (float) Math.Round(  Utils.Cross( r, s ), 3 );

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

        public Line2 Rotate( float radians )
        {
            return new Line2( P0.Rotate( radians ),
                              P1.Rotate( radians ) );
        }

        public static Line2 operator +( Line2 line, Vector2 vec )
        {
            return new Line2( line.P0 + vec, line.P1 + vec );
        }

        public static Line2 operator +( Vector2 vec, Line2 line )
        {
            return new Line2( vec + line.P0, vec + line.P1 );
        }

        public static Line2 operator -( Line2 line, Vector2 vec )
        {
            return new Line2( line.P0 - vec, line.P1 - vec );
        }

        public static Line2 operator -( Vector2 vec, Line2 line )
        {
            return new Line2( vec - line.P0, vec - line.P1 );
        }


        public float X0
        {
            get {
                return P0.X;
            }
            set {
                P0.X = value;
            }
        }

        public float X1
        {
            get {
                return P1.X;
            }
            set {
                P1.X = value;
            }
        }

        public float Y0
        {
            get {
                return P0.Y;
            }
            set {
                P0.Y = value;
            }
        }

        public float Y1
        {
            get {
                return P1.Y;
            }
            set {
                P1.Y = value;
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

        public Line3( float x0, float y0, float z0, float x1, float y1, float z1 )
        {
            P0 = new Vector3( x0, y0, z0 );
            P1 = new Vector3( x1, y1, z1 );
        }

        public Vector3 this[ float t ]
        {
            get {
                return P0 + (t * (P1 - P0));
            }
        }

        public float Length
        {
            get {
                return (P1 - P0).Length();
            }
        }

        public Vector3 Direction
        {
            get {
                return Vector3.Normalize( P1 - P0 );
            }
        }

        public Vector3 Center
        {
            get {
                return new Vector3( X0 + X1, Y0 + Y1, Z0 + Z1 ) / 2;
            }
        }

        public void Reverse()
        {
            var tmp = P0;
            P0 = P1;
            P1 = tmp;
        }

        public void Reverse( out Line3 outLine )
        {
            outLine.P0 = P1;
            outLine.P1 = P0;
        }

        public static Line3 operator +( Line3 line, Vector3 vec )
        {
            return new Line3( line.P0 + vec, line.P1 + vec );
        }

        public static Line3 operator +( Vector3 vec, Line3 line )
        {
            return new Line3( vec + line.P0, vec + line.P1 );
        }

        public static Line3 operator -( Line3 line, Vector3 vec )
        {
            return new Line3( line.P0 - vec, line.P1 - vec );
        }

        public static Line3 operator -( Vector3 vec, Line3 line )
        {
            return new Line3( vec - line.P0, vec - line.P1 );
        }

        public float? Intersects( BoundingBox box )
        {
            return new Ray( P0, Direction ).Intersects( box );
        }

        public float? Intersects( BoundingSphere sphere )
        {
            return new Ray( P0, Direction ).Intersects( sphere );
        }

        public float? Intersects( BoundingFrustum frustum )
        {
            return new Ray( P0, Direction ).Intersects( frustum );
        }

        public float? Intersects( Plane plane )
        {
            return new Ray( P0, Direction ).Intersects( plane );
        }

        public float X0
        {
            get {
                return P0.X;
            }
            set {
                P0.X = value;
            }
        }

        public float X1
        {
            get {
                return P1.X;
            }
            set {
                P1.X = value;
            }
        }

        public float Y0
        {
            get {
                return P0.Y;
            }
            set {
                P0.Y = value;
            }
        }

        public float Y1
        {
            get {
                return P1.Y;
            }
            set {
                P1.Y = value;
            }
        }

        public float Z0
        {
            get {
                return P0.Z;
            }
            set {
                P0.Z = value;
            }
        }

        public float Z1
        {
            get {
                return P1.Z;
            }
            set {
                P1.Z = value;
            }
        }

    }
}
