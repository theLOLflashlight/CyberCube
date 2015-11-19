using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Tools
{
    /// <summary>
    /// What a Terrible Failure. Represents an error that should never happen.
    /// </summary>
    public class WtfException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the WtfException class.
        /// </summary>
        public WtfException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the WtfException class with a specified error 
        /// message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public WtfException( string message )
            : base( message )
        {
        }
    }

    public abstract class EnumException : ArgumentOutOfRangeException
    {
#if !XBOX
        public EnumException( string paramName, object actualValue, string message )
            : base( paramName, actualValue, message )
        {
        }
#else
        public EnumException( string paramName, object actualValue, string message )
            : base( paramName, message )
        {
        }
#endif

        public EnumException( string paramName, string message )
            : base( paramName, message )
        {
        }

        public abstract Type EnumType
        {
            get;
        }
    }

    public class EnumException<E> : EnumException
    {
        public EnumException( string paramName, E actualValue )
            : base( paramName, actualValue, $"Invalid value for {nameof( E )}" )
        {
        }

        public EnumException( string paramName )
            : base( paramName, $"Invalid value for {nameof( E )}" )
        {
        }

        public sealed override Type EnumType
        {
            get {
                return typeof( E );
            }
        }
    }
}
