using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Tools
{
    public struct RectangleF : IEquatable< RectangleF >
    {
        public float Height;
        public float Width;
        public float X;
        public float Y;

        public RectangleF( float x, float y, float width, float height )
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public static implicit operator RectangleF( Rectangle rec )
        {
            return new RectangleF( rec.X, rec.Y, rec.Width, rec.Height );
        }

        public static explicit operator Rectangle( RectangleF rec )
        {
            return new Rectangle( (int) rec.X, (int) rec.Y, (int) rec.Width, (int) rec.Height );
        }

        public static RectangleF Empty
        {
            get {
                return default( RectangleF );
            }
        }

        public float Top
        {
            get {
                return Y;
            }
        }

        public float Bottom
        {
            get {
                return Y + Height;
            }
        }

        public float Left
        {
            get {
                return X;
            }
        }

        public float Right
        {
            get {
                return X + Width;
            }
        }

        public Vector2 Center
        {
            get {
                return new Vector2( X + Width / 2, Y + Height / 2 );
            }
        }

        public Vector2 Location
        {
            get {
                return new Vector2( X, Y );
            }
            set {
                X = value.X;
                Y = value.Y;
            }
        }

        public override bool Equals( object obj )
        {
            return obj is RectangleF && Equals( (RectangleF) obj);
        }

        public bool Equals( RectangleF other )
        {
            return Height == other.Height
                   && Width == other.Width
                   && X == other.X
                   && Y == other.Y;
        }

        public static bool operator ==( RectangleF a, RectangleF b )
        {
            return a.Equals( b );
        }

        public static bool operator !=( RectangleF a, RectangleF b )
        {
            return !a.Equals( b );
        }
    }
}
