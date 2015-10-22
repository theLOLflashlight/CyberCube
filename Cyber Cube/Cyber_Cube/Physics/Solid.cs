using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Physics
{
    public abstract class Solid : DrawableCubeGameComponent
    {

        public Body Body
        {
            get; protected set;
        }

        private World mWorld;

        public World World
        {
            get {
                return mWorld;
            }
            set {
                if ( value == null )
                    throw new NullReferenceException( "The value of World cannot be null" );

                World newWorld = value;

                if ( newWorld != mWorld )
                {
                    Body body = Body.DeepClone( newWorld );
                    mWorld.RemoveBody( Body );
                    Body = body;
                }
                mWorld = newWorld;
            }
        }

        public BodyType BodyType
        {
            get {
                return Body.BodyType;
            }
            set {
                Body.BodyType = value;
            }
        }

        public Solid( CubeGame game, World world )
            : base( game )
        {
            mWorld = world;
            this.Visible = false;
        }

        public abstract override void Draw( GameTime gameTime );

    }

    public class RecSolid : Solid
    {
        protected readonly Rectangle mRec;

        private Texture2D mTexture;

        public RecSolid( CubeGame game,
                         World world,
                         Rectangle rec,
                         BodyType bodyType = BodyType.Static,
                         float mass = 1)
            : base( game, world )
        {
            mRec = rec;

            Body = BodyFactory.CreateRectangle(
                    world,
                    mRec.Width * Constants.PIXEL_TO_UNIT,
                    mRec.Height * Constants.PIXEL_TO_UNIT,
                    1,
                    Constants.PIXEL_TO_UNIT * new Vector2(
                        mRec.X + mRec.Width / 2,
                        mRec.Y + mRec.Height / 2 ) );

            Body.BodyType = bodyType;
            Body.Mass = mass;
        }

        public override void Initialize()
        {
            base.Initialize();

            mTexture = new Texture2D( GraphicsDevice, 1, 1 );
            mTexture.SetData( new[] { Color.White } );
        }

        public override void Draw( GameTime gameTime )
        {
            mSpriteBatch.Begin( SpriteSortMode.Immediate, BlendState.Opaque );

            Vector2 position = Body.Position * Constants.UNIT_TO_PIXEL;
            Vector2 scale = new Vector2(
                mRec.Width / (float) mTexture.Width,
                mRec.Height / (float) mTexture.Height );

            mSpriteBatch.Draw(
                mTexture,
                position,
                null,
                Color.Black,
                Body.Rotation,
                new Vector2(
                    mTexture.Width / 2.0f,
                    mTexture.Height / 2.0f ),
                scale,
                SpriteEffects.None,
                0 );

            mSpriteBatch.End();
        }

    }

    public class EdgeSolid : Solid
    {
        protected readonly Line2 mLine;

        private Texture2D mTexture;

        public EdgeSolid( CubeGame game,
                          World world,
                          Line2 line,
                          BodyType bodyType = BodyType.Static,
                          float mass = 1 )
            : base( game, world )
        {
            mLine = line;

            Body = BodyFactory.CreateEdge(
                    world,
                    mLine.P0 * Constants.PIXEL_TO_UNIT,
                    mLine.P1 * Constants.PIXEL_TO_UNIT );

            Body.BodyType = bodyType;
            Body.Mass = mass;
        }

        public override void Initialize()
        {
            base.Initialize();

            mTexture = new Texture2D( GraphicsDevice, 1, 1 );
            mTexture.SetData( new[] { Color.White } );
        }

        public override void Draw( GameTime gameTime )
        {
            mSpriteBatch.Begin();

            Vector2 edge = mLine.P1 - mLine.P0;
            float angle = (float) Math.Atan2( edge.Y, edge.X );

            mSpriteBatch.Draw(
                mTexture,
                new Rectangle(
                    (int) mLine.P0.X,
                    (int) mLine.P0.Y,
                    (int) edge.Length(),
                    30 ),
                null,
                new Color( 0, 0, 0, 64 ),
                angle,
                Vector2.Zero,
                SpriteEffects.None,
                0 );

            mSpriteBatch.Draw(
                mTexture,
                new Rectangle(
                    (int) mLine.P0.X,
                    (int) mLine.P0.Y,
                    (int) edge.Length(),
                    20 ),
                null,
                new Color( 0, 0, 0, 64 ),
                angle,
                Vector2.Zero,
                SpriteEffects.None,
                0 );

            mSpriteBatch.Draw(
                mTexture,
                new Rectangle(
                    (int) mLine.P0.X,
                    (int) mLine.P0.Y,
                    (int) edge.Length(),
                    10 ),
                null,
                Color.Black,
                angle,
                Vector2.Zero,
                SpriteEffects.None,
                0 );

            mSpriteBatch.End();
        }

    }
}
