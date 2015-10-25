using CyberCube.Graphics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
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

        public Texture2D Texture;

        public Solid( CubeGame game, World world )
            : base( game )
        {
            mWorld = world;
            this.Visible = false;
        }

        public override void Initialize()
        {
            base.Initialize();

            Texture = new Texture2D( GraphicsDevice, 1, 1 );
            Texture.SetData( new[] { Color.White } );
        }

        public abstract override void Draw( GameTime gameTime );

    }

    public class RecSolid : Solid
    {
        protected readonly Rectangle mRec;

        public RecSolid( CubeGame game,
                         World world,
                         Rectangle rec,
                         BodyType bodyType = BodyType.Static,
                         float mass = 1 )
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

        public override void Draw( GameTime gameTime )
        {
            //Body.DrawBody( GraphicsDevice, mTexture, gameTime );

            mSpriteBatch.Begin( SpriteSortMode.Immediate, BlendState.Opaque );

            Vector2 position = Body.Position * Constants.UNIT_TO_PIXEL;
            Vector2 scale = new Vector2(
                mRec.Width / (float) Texture.Width,
                mRec.Height / (float) Texture.Height );

            mSpriteBatch.Draw(
                Texture,
                position,
                null,
                Color.Black,
                Body.Rotation,
                new Vector2(
                    Texture.Width / 2.0f,
                    Texture.Height / 2.0f ),
                scale,
                SpriteEffects.None,
                0 );

            mSpriteBatch.End();
        }

    }

    public class EdgeSolid : Solid
    {
        protected readonly Line2 mLine;

        protected readonly Fixture mEdge;
        protected readonly Fixture mExclusionRec;

        public EdgeSolid( CubeGame game,
                          World world,
                          Line2 line,
                          BodyType bodyType = BodyType.Static,
                          float mass = 1 )
            : base( game, world )
        {
            mLine = line;

            Vector2 center;
            center.X = (mLine.X0 + mLine.X1) / 2;
            center.Y = (mLine.Y0 + mLine.Y1) / 2;

            Body = BodyFactory.CreateBody( world );

            Body.BodyType = bodyType;
            Body.Mass = mass;

            mEdge = FixtureFactory.AttachEdge(
                    mLine.P0 * Constants.PIXEL_TO_UNIT,
                    mLine.P1 * Constants.PIXEL_TO_UNIT,
                    Body );

            float width = mLine.Length;
            float height = 100;
            if ( mLine.Y0 == mLine.Y1 )
            {
                width = 100;
                height = mLine.Length;
            }

            Vector2 off = mLine.Y0 == mLine.Y1
                          ? Vector2.UnitY * ((height / 2) + 2)
                          : Vector2.UnitX * ((width / 2) + 2);

            if ( mLine.X1 < mLine.X0 || mLine.Y1 > mLine.Y0 )
                off = -off;

            mExclusionRec = FixtureFactory.AttachRectangle(
                width * Constants.PIXEL_TO_UNIT,
                height * Constants.PIXEL_TO_UNIT,
                1,
                (center + off) * Constants.PIXEL_TO_UNIT,
                Body );

            mExclusionRec.IsSensor = true;

            mExclusionRec.OnCollision += ( a, b, c ) => {
                mEdge.IgnoreCollisionWith( b );
                return true;
            };
            
            mExclusionRec.OnSeparation += ( a, b ) => {
                mEdge.RestoreCollisionWith( b );
            };
        }

        public override void Draw( GameTime gameTime )
        {
            //mExclusionRec.Body.DrawBody( GraphicsDevice, Texture, gameTime );

            mSpriteBatch.Begin();

            Vector2 edge = mLine.P1 - mLine.P0;
            float angle = (float) Math.Atan2( edge.Y, edge.X );

            mSpriteBatch.Draw(
                Texture,
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
                Texture,
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
                Texture,
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
