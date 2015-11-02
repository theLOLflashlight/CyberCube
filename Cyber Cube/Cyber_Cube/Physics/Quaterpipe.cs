using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common.Decomposition;

namespace CyberCube.Physics
{
    public class Quarterpipe : Solid
    {
        public enum Type
        {
            SE, SW, NW, NE
        }

        private static float AngleFromType( Type type )
        {
            switch ( type )
            {
            default:
            case Type.SE:
                return 0;

            case Type.SW:
                return MathHelper.PiOver2;

            case Type.NW:
                return MathHelper.Pi;

            case Type.NE:
                return -MathHelper.PiOver2;
            }
        }

        private static Vertices MakeArc( int sides, float radius, float angle )
        {
            Vertices arc = PolygonTools.CreateArc( MathHelper.PiOver2, sides, radius );
            arc.Add( new Vector2( arc.Last().X, arc.First().Y ) );
            arc.Add( arc.First() );
            arc.Rotate( angle );

            return arc;
        }

        private float     mRadius;
        private Type      mType;
        private Texture2D mInvCircleTex;

        public Quarterpipe( CubeGame game,
                            World world,
                            float radius,
                            Vector2 position,
                            Type type,
                            BodyType bodyType = BodyType.Static,
                            float mass = 1,
                            Category categories = Category.Cat1 )
            : base( game, world )
        {
            mRadius = radius;
            mType = type;
            float angle = AngleFromType( mType );

            Body = BodyFactory.CreateChainShape(
                world,
                MakeArc( 100, radius.ToUnits(), angle ),
                position.ToUnits() );

            Body.BodyType = bodyType;
            Body.Mass = mass;
            Body.CollisionCategories = categories;

            var fs = FixtureFactory.AttachCompoundPolygon(
                Triangulate.ConvexPartition(
                    MakeArc( 10, radius.ToUnits(), angle ),
                    TriangulationAlgorithm.Earclip ),
                1,
                Body );

            foreach ( var f in fs )
                f.CollidesWith = Category.None;
        }

        public override Solid Clone( World world )
        {
            return DefaultDeepClone( base.Clone( world ) );
        }

        private Texture2D CreateCircleTexture( int diam )
        {
            int radius = diam / 2;
            float radiussq = radius * radius;

            Texture2D texture = new Texture2D( GraphicsDevice, diam, diam );
            Color[] colorData = new Color[ diam * diam ];

            for ( int x = 0; x < diam; x++ )
            {
                for ( int y = 0; y < diam; y++ )
                {
                    int index = x * diam + y;
                    Vector2 pos = new Vector2( x - radius, y - radius );

                    colorData[ index ] = pos.LengthSquared() <= radiussq
                                         ? Color.Transparent
                                         : Color.White;
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        public override void Initialize()
        {
            base.Initialize();

            mInvCircleTex = CreateCircleTexture( (int) mRadius );
        }

        public override void Draw( GameTime gameTime )
        {
            Vector2 position = Body.Position.ToPixels();
            Vector2 scale = new Vector2(
                mRadius / mInvCircleTex.Width,
                mRadius / mInvCircleTex.Height );

            int x = mInvCircleTex.Width;
            int y = mInvCircleTex.Height;
            switch ( mType )
            {
            case Type.SE:
                break;
            case Type.SW:
                x = 0;
                break;
            case Type.NW:
                x = 0;
                y = 0;
                break;
            case Type.NE:
                y = 0;
                break;
            }

            mSpriteBatch.Begin();

            mSpriteBatch.Draw(
                mInvCircleTex,
                new Rectangle(
                    (int) position.X + x,
                    (int) position.Y + y,
                    (int) mRadius,
                    (int) mRadius ),
                new Rectangle(
                    x / 2,
                    y / 2,
                    mInvCircleTex.Width / 2,
                    mInvCircleTex.Height / 2 ),
                BodyType == BodyType.Static ? Color.Black : Color.White,
                Body.Rotation,
                new Vector2(
                    mInvCircleTex.Width / 2.0f,
                    mInvCircleTex.Height / 2.0f ),
                SpriteEffects.None,
                0 );
            
            mSpriteBatch.End();
        }
    }
}
