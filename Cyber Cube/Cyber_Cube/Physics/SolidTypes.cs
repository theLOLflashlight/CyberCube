using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Physics
{
    public abstract class Ground
    { }

    public class Flat : Ground
    { }

    public class Curved : Ground
    { }

    public class Convex : Curved
    { }

    public class Concave : Curved
    { }
}
