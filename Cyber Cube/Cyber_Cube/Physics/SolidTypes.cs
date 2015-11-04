using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Physics
{
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
