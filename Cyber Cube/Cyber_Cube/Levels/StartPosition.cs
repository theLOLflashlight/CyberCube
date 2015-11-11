using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Levels
{
    public struct StartPosition
    {
        public Vector3 Position;
        public float Rotation;

        public StartPosition( Vector3 pos, float rotation )
        {
            Position = pos;
            Rotation = rotation;
        }
    }
}
