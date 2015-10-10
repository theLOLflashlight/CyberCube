using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{

    public partial class Cube
    {
        public class Face
        {
            public Face NorthFace { get; internal set; }
            public Face EastFace { get; internal set; }
            public Face SouthFace { get; internal set; }
            public Face WestFace { get; internal set; }

            public string Name { get; private set; }

            public Vector3 Normal { get; private set; }
            public Vector3 Up { get; private set; }

            public Face( string name, Vector3 normal, Vector3 up )
                : this( null, null, null, null )
            {
                Name = name;
                Normal = normal;
                Up = up;
            }

            public Face( Face northFace, Face eastFace, Face southFace, Face westFace )
            {
                NorthFace = northFace;
                EastFace = eastFace;
                SouthFace = southFace;
                WestFace = westFace;
            }
        }
    }
}
