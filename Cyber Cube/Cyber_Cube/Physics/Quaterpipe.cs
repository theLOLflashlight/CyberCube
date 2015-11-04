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
            arc.Add( new Vector2( arc.First().X, arc.First().Y + 0.0001f ) );
            //arc.Add( arc.First() );
            arc.Rotate( angle );
            arc.Translate( new Vector2( -radius / 2 ) );

            return arc;
        }

        private float     mRadius;
        private Texture2D mQuarterpipeTex;

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

            Body = BodyFactory.CreateChainShape(
                world,
                MakeArc( 100, radius.ToUnits(), 0 ),
                position.ToUnits(),
                new Concave() );

            Body.BodyType = bodyType;
            Body.Mass = mass;
            Body.CollisionCategories = categories;
            Body.Rotation = AngleFromType( type );

            var fixtureList = FixtureFactory.AttachCompoundPolygon(
                Triangulate.ConvexPartition(
                    MakeArc( 10, radius.ToUnits(), 0 ),
                    TriangulationAlgorithm.Earclip ),
                1,
                Body );

            foreach ( var f in fixtureList )
                f.CollidesWith = Category.None;
        }

        private Texture2D CreateCircleTexture( float radius )
        {
            return CreateQuarterpipeTexture( GraphicsDevice, radius, Color.White );
        }

        public static Texture2D CreateQuarterpipeTexture( GraphicsDevice graphicsDevice, float radiusf, Color color )
        {
            int radius = (int) radiusf;
            int diam = radius * 2;
            float radiussq = radiusf * radiusf;

            Texture2D texture = new Texture2D( graphicsDevice, radius, radius );
            Color[] colorData = new Color[ radius * radius ];

            for ( int x = radius ; x < diam ; x++ )
            {
                for ( int y = radius ; y < diam ; y++ )
                {
                    int index = (x - radius) * radius + (y - radius);
                    Vector2 pos = new Vector2( x - radius, y - radius );

                    colorData[ index ] = pos.LengthSquared() <= radiussq
                                         ? Color.Transparent
                                         : color;
                }
            }

            texture.SetData( colorData );
            return texture;
        }

        public override void Initialize()
        {
            base.Initialize();

            mQuarterpipeTex = CreateCircleTexture( (int) mRadius );
        }

        public override void Draw( GameTime gameTime )
        {
            Vector2 position = Body.Position.ToPixels();
            Vector2 origin = new Vector2(
                mQuarterpipeTex.Width,
                mQuarterpipeTex.Height ) / 2;

            mSpriteBatch.Begin();

            mSpriteBatch.Draw(
                mQuarterpipeTex,
                position,
                null,
                BodyType == BodyType.Static ? Color.Black : Color.White,
                Body.Rotation,
                origin,
                Vector2.One,
                SpriteEffects.None,
                0 );
            
            mSpriteBatch.End();
        }
    }
}
