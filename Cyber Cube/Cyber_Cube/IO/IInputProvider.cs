using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube.IO
{
    public interface IInputProvider
    {
        InputState Input { get; }
    }

    public interface IInputProvider< Action > : IInputProvider
        where Action : struct, IComparable, IFormattable, IConvertible
    {
        new InputState< Action > Input { get; }
    }
}
