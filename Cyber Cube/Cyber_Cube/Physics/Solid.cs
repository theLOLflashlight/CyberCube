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

        const float SENSOR_RANGE = 100f;
        const float EDGE_THICKNESS = 1f;

        public EdgeSolid( CubeGame game,
                          World world,
                          Line2 line,
                          BodyType bodyType = BodyType.Static,
                          float mass = 1 )
            : base( game, world )
        {
            if ( line.X0 != line.X1 && line.Y0 != line.Y1 )
                throw new ArgumentException( "Only vertical and horizontal edges are supported." );

            mLine = line;

            Vector2 center, offset;
            float edgeWidth, edgeHeight, sensorWidth, sensorHeight;

            #region Variable setup
            {
                bool horizontal = mLine.Y0 == mLine.Y1;
                bool inverted = mLine.X1 < mLine.X0 || mLine.Y1 > mLine.Y0;

                center.X = (mLine.X0 + mLine.X1) / 2;
                center.Y = (mLine.Y0 + mLine.Y1) / 2;

                edgeWidth    = horizontal
                               ? mLine.Length
                               : EDGE_THICKNESS;
                sensorWidth  = horizontal
                               ? edgeWidth
                               : SENSOR_RANGE;
                edgeHeight   = horizontal
                               ? EDGE_THICKNESS
                               : mLine.Length;
                sensorHeight = horizontal
                               ? SENSOR_RANGE
                               : edgeHeight;

                offset = horizontal
                         ? Vector2.UnitY * ((sensorHeight / 2) + edgeHeight)
                         : Vector2.UnitX * ((sensorWidth / 2) + edgeWidth);
                if ( inverted )
                    offset = -offset;
            }
            #endregion

            Body = BodyFactory.CreateBody( world );
            Body.BodyType = bodyType;
            Body.Mass = mass;


            mEdge = FixtureFactory.AttachRectangle(
                edgeWidth * Constants.PIXEL_TO_UNIT,
                edgeHeight * Constants.PIXEL_TO_UNIT,
                1,
                center * Constants.PIXEL_TO_UNIT,
                Body );


            mExclusionRec = FixtureFactory.AttachRectangle(
                sensorWidth * Constants.PIXEL_TO_UNIT,
                sensorHeight * Constants.PIXEL_TO_UNIT,
                1,
                (center + offset) * Constants.PIXEL_TO_UNIT,
                Body );

            mExclusionRec.IsSensor = true;

            mExclusionRec.OnCollision += ( a, b, c ) => {
                if ( !b.IsSensor )
                    mEdge.IgnoreCollisionWith( b );
                return !b.IsSensor;
            };
            
            mExclusionRec.OnSeparation += ( a, b ) => {
                mEdge.RestoreCollisionWith( b );
            };
        }

        public override void Draw( GameTime gameTime )
        {
            //mExclusionRec.Body.DrawBody( GraphicsDevice, Texture, gameTime );

            mSpriteBatch.Begin();

            mSpriteBatch.DrawLine( mLine, Texture, new Color( 0, 0, 0, 64 ), 30 );
            mSpriteBatch.DrawLine( mLine, Texture, new Color( 0, 0, 0, 64 ), 20 );
            mSpriteBatch.DrawLine( mLine, Texture, Color.Black, 10 );

            mSpriteBatch.End();
        }

    }
}
