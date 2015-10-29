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

        protected float mRadius;

        private Type mType;

        private Texture2D mOcclusionCir;

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

            float angle = 0;
            switch ( mType )
            {
            case Type.SE:
                angle = 0;
                break;
            case Type.SW:
                angle = MathHelper.PiOver2;
                break;
            case Type.NW:
                angle = MathHelper.Pi;
                break;
            case Type.NE:
                angle = -MathHelper.PiOver2;
                break;
            }

            Vertices arc = PolygonTools.CreateArc( MathHelper.PiOver2, 100, radius * Constants.PIXEL_TO_UNIT );
            arc.Add( new Vector2( arc.Last().X, arc.First().Y ) );
            arc.Add( arc.First() );
            arc.Rotate( angle );

            Body = BodyFactory.CreateChainShape( world, arc, position * Constants.PIXEL_TO_UNIT );

            List<Vertices> triangles = Triangulate.ConvexPartition( arc, TriangulationAlgorithm.Earclip );
            var fs = FixtureFactory.AttachCompoundPolygon(
                triangles,
                1,
                Body );

            foreach ( var f in fs )
                f.CollidesWith = Category.None;

            //Body = BodyFactory.CreateLineArc(
            //    world,
            //    MathHelper.PiOver2,
            //    100,
            //    radius * Constants.PIXEL_TO_UNIT,
            //    position * Constants.PIXEL_TO_UNIT,
            //    angle - MathHelper.PiOver4,
            //    false );

            Body.BodyType = bodyType;
            Body.Mass = mass;
            Body.CollisionCategories = categories;
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

            mOcclusionCir = CreateCircleTexture( (int) mRadius );
        }

        public override void Draw( GameTime gameTime )
        {
            mSpriteBatch.Begin();
            
            Vector2 position = Body.Position * Constants.UNIT_TO_PIXEL;
            Vector2 scale = new Vector2(
                mRadius / Texture.Width,
                mRadius / Texture.Height );
            
            //mSpriteBatch.Draw(
            //    Texture,
            //    position + new Vector2( mRadius / 2 ),
            //    null,
            //    BodyType == BodyType.Static ? Color.Black : Color.White,
            //    Body.Rotation,
            //    new Vector2(
            //        Texture.Width / 2.0f,
            //        Texture.Height / 2.0f ),
            //    scale,
            //    SpriteEffects.None,
            //    0 );

            scale = new Vector2(
                mRadius / mOcclusionCir.Width,
                mRadius / mOcclusionCir.Height );


            int x = mOcclusionCir.Width;
            int y = mOcclusionCir.Height;
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


            mSpriteBatch.Draw(
                mOcclusionCir,
                new Rectangle(
                    (int) position.X + x,
                    (int) position.Y + y,
                    (int) mRadius,
                    (int) mRadius ),
                new Rectangle(
                    x / 2,
                    y / 2,
                    mOcclusionCir.Width / 2,
                    mOcclusionCir.Height / 2 ),
                BodyType == BodyType.Static ? Color.Black : Color.White,
                Body.Rotation,
                //Vector2.Zero,
                new Vector2(
                    mOcclusionCir.Width / 2.0f,
                    mOcclusionCir.Height / 2.0f ),
                SpriteEffects.None,
                0 );
            
            mSpriteBatch.End();
        }
    }
}
