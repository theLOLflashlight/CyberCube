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
            : base( content.Load<Effect>( "TexturesAndColors" ) )
        {
        }

        public Matrix World
        {
            get {
                return Parameters[ "world" ].GetValueMatrix();
            }
            set {
                Parameters[ "world" ].SetValue( value );
            }
        }

        public Matrix View
        {
            get {
                return Parameters[ "view" ].GetValueMatrix();
            }
            set {
                Parameters[ "view" ].SetValue( value );
            }
        }

        public Matrix Projection
        {
            get {
                return Parameters[ "projection" ].GetValueMatrix();
            }
            set {
                Parameters[ "projection" ].SetValue( value );
            }
        }

        public Vector4 LightColor
        {
            get {
                return Parameters[ "lightColor" ].GetValueVector4();
            }
            set {
                Parameters[ "lightColor" ].SetValue( value );
            }
        }

        public Vector3 LightDirection
        {
            get {
                return Parameters[ "lightDirection" ].GetValueVector3();
            }
            set {
                Parameters[ "lightDirection" ].SetValue( value );
            }
        }

        public Vector4 AmbientColor
        {
            get {
                return Parameters[ "ambientColor" ].GetValueVector4();
            }
            set {
                Parameters[ "ambientColor" ].SetValue( value );
            }
        }

        public Texture2D Texture
        {
            get {
                return Parameters[ "modelTexture" ].GetValueTexture2D();
            }
            set {
                Parameters[ "modelTexture" ].SetValue( value );
            }
        }

    }
}