using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Physics
{
    public enum SolidType
    {
        None = 0,
        Any = ~0,
        Point = 1,
        Flat = 2,
        Curved = 4,
        Convex = 4 + 8,
        Concave = 4 + 16,
    }

    public class SolidDescriptor : IEquatable< SolidDescriptor >
    {
        public string UserData { get; set; }

        public SolidDescriptor()
        {
        }

        public SolidDescriptor( string userData )
        {
            UserData = userData;
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as SolidDescriptor );
        }

        public bool Equals( SolidDescriptor other )
        {
            return other != null
                   && GetType().IsAssignableFrom( other.GetType() )
                   && UserData == other.UserData;
        }

        public static bool operator ==( SolidDescriptor a, SolidDescriptor b )
        {
            return a?.Equals( b ) ?? b?.Equals( a ) ?? true;
        }

        public static bool operator !=( SolidDescriptor a, SolidDescriptor b )
        {
            return !(a == b);
        }
    }

    public class Hazard : SolidDescriptor
    {
        public Hazard()
            : this( null )
        {
        }

        public Hazard( string userData )
            : base( userData )
        {
        }
    }

    public class Door : SolidDescriptor
    {
        public Door()
            : this( null )
        {
        }

        public Door( string userData )
            : base( userData )
        {
        }
    }

    public class Flat : SolidDescriptor
    {
        public Flat()
            : this( null )
        {
        }

        public Flat( string userData )
            : base( userData )
        {
        }
    }

    public abstract class Curved : SolidDescriptor
    {
        public Curved( string userData )
            : base( userData )
        {
        }
    }

    public class Convex : Curved
    {
        public Convex()
            : this( null )
        {
        }

        public Convex( string userData )
            : base( userData )
        {
        }
    }

    public class Concave : Curved
    {
        public Concave()
            : this( null )
        {
        }

        public Concave( string userData )
            : base( userData )
        {
        }
    }
}
