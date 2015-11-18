using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube
{
    public struct VertexPositionNormalTextureColor : IVertexType
    {
        public static readonly VertexDeclaration sVertexDeclaration = new VertexDeclaration(
                new VertexElement( 0, VertexElementFormat.Color, VertexElementUsage.Color, 0 ),
                new VertexElement( 4, VertexElementFormat.Vector3, VertexElementUsage.Position, 0 ),
                new VertexElement( 16, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0 ),
                new VertexElement( 28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0 ) );

        public Color Color;
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get {
                return sVertexDeclaration;
            }
        }

        public VertexPositionNormalTextureColor( Vector3 position, Vector3 normal )
            : this( position, normal, Vector2.Zero )
        {
        }

        public VertexPositionNormalTextureColor( Vector3 position, Vector3 normal, Color color )
            : this( position, normal, Vector2.Zero, color )
        {
        }

        public VertexPositionNormalTextureColor( Vector3 position, Vector3 normal, Vector2 textureCoordinate )
            : this( position, normal, textureCoordinate, Color.White )
        {
        }

        public VertexPositionNormalTextureColor( Vector3 position, Vector3 normal, Vector2 textureCoordinate, Color color )
        {
            Position = position;
            Normal = normal;
            TextureCoordinate = textureCoordinate;
            Color = color;
        }

        public override bool Equals( object obj )
        {
            if ( !(obj is VertexPositionNormalTextureColor) )
                return false;

            var vpntc = (VertexPositionNormalTextureColor) obj;

            return Position == vpntc.Position
                   && Normal == vpntc.Normal
                   && TextureCoordinate == vpntc.TextureCoordinate
                   && Color == vpntc.Color;
        }
        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
        public override string ToString()
        {
            return $"{{{Position}, {Normal}, {TextureCoordinate}, {Color}}}";
        }

        public static bool operator ==( VertexPositionNormalTextureColor left, VertexPositionNormalTextureColor right )
        {
            return left.Equals( right );
        }
        public static bool operator !=( VertexPositionNormalTextureColor left, VertexPositionNormalTextureColor right )
        {
            return !left.Equals( right );
        }
    }
}
