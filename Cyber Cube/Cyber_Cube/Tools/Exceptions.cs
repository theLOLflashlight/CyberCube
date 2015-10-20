using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube.Tools
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
}
