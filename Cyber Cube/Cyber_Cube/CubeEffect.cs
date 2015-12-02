using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CyberCube
{
    public class CubeEffect : Effect
    {
        public CubeEffect( ContentManager content )
            : base( content.Load<Effect>( "CubeEffect" ) )
        {
        }

        public Matrix World
        {
            get {
                return Parameters[ "World" ].GetValueMatrix();
            }
            set {
                Parameters[ "World" ].SetValue( value );
            }
        }

        public Matrix View
        {
            get {
                return Parameters[ "View" ].GetValueMatrix();
            }
            set {
                Parameters[ "View" ].SetValue( value );
            }
        }

        public Matrix Projection
        {
            get {
                return Parameters[ "Projection" ].GetValueMatrix();
            }
            set {
                Parameters[ "Projection" ].SetValue( value );
            }
        }

        public Vector4 LightColor
        {
            get {
                return Parameters[ "LightColor" ].GetValueVector4();
            }
            set {
                Parameters[ "LightColor" ].SetValue( value );
            }
        }

        public Vector3 LightDirection
        {
            get {
                return Parameters[ "LightDirection" ].GetValueVector3();
            }
            set {
                Parameters[ "LightDirection" ].SetValue( value );
            }
        }

        public Vector4 AmbientColor
        {
            get {
                return Parameters[ "AmbientColor" ].GetValueVector4();
            }
            set {
                Parameters[ "AmbientColor" ].SetValue( value );
            }
        }

        public Texture2D ForeTexture
        {
            get {
                return Parameters[ "ForeTexture" ].GetValueTexture2D();
            }
            set {
                Parameters[ "ForeTexture" ].SetValue( value );
            }
        }

        public Texture2D BackTexture
        {
            get {
                return Parameters[ "BackTexture" ].GetValueTexture2D();
            }
            set {
                Parameters[ "BackTexture" ].SetValue( value );
            }
        }

        public Vector4 TransparentColor
        {
            get {
                return Parameters[ "TransparentColor" ].GetValueVector4();
            }
            set {
                Parameters[ "TransparentColor" ].SetValue( value );
            }
        }

    }
}