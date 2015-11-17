using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Levels
{
    public struct CubePosition
    {
        public Vector3 Position;
        public float Rotation;

        public CubePosition( Vector3 pos, float rotation )
        {
            Position = pos;
            Rotation = rotation;
        }
    }
}
