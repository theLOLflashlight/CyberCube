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

        public virtual Solid Clone( World world )
        {
            Solid clone = (Solid) MemberwiseClone();
            clone.mWorld = world;
            clone.Body = clone.Body.DeepClone( world );

            return clone;
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
                         float mass = 1,
                         Category categories = Category.Cat1 )
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
            Body.CollisionCategories = categories;
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
                BodyType == BodyType.Static ? Color.Black : Color.White,
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

    public class OneWayPlatform : Solid
    {
        protected readonly Line2 mLine;

        protected readonly Fixture mEdge;
        protected readonly Fixture mExclusionRec;

        const float SENSOR_RANGE = 100f;
        const float EDGE_THICKNESS = 1f;

        private int mExclusionCount = 0;

        public OneWayPlatform( CubeGame game,
                               World world,
                               Line2 line,
                               BodyType bodyType = BodyType.Static,
                               float mass = 1 )
            : base( game, world )
        {
            if ( line.X0 != line.X1 && line.Y0 != line.Y1 )
                throw new ArgumentException( "Only vertical and horizontal edges are supported." );

            Vector2 center, offset;
            float edgeWidth, edgeHeight, sensorWidth, sensorHeight;

            #region Variable setup
            {
                bool horizontal = line.Y0 == line.Y1;
                bool inverted = line.X1 < line.X0 || line.Y1 > line.Y0;
#if XBOX
                center = Vector2.Zero;
#endif
                center.X = (line.X0 + line.X1) / 2;
                center.Y = (line.Y0 + line.Y1) / 2;

                edgeWidth    = horizontal
                               ? line.Length
                               : EDGE_THICKNESS;
                sensorWidth  = horizontal
                               ? edgeWidth
                               : SENSOR_RANGE;
                edgeHeight   = horizontal
                               ? EDGE_THICKNESS
                               : line.Length;
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

            mLine = line - center;

            Body = BodyFactory.CreateBody( world, center * Constants.PIXEL_TO_UNIT );
            Body.BodyType = bodyType;
            Body.Mass = mass;

            // Not really an Edge, just a thin rectangle. This is necessary for accurate 
            // collision and exclusion detection.
            mEdge = FixtureFactory.AttachRectangle(
                edgeWidth * Constants.PIXEL_TO_UNIT,
                edgeHeight * Constants.PIXEL_TO_UNIT,
                1,
                Vector2.Zero,//(offset / 2) * Constants.PIXEL_TO_UNIT,
                Body );
            mEdge.CollidesWith = Category.Cat2;

            Fixture edge = FixtureFactory.AttachEdge(
                mLine.P0 * Constants.PIXEL_TO_UNIT,
                mLine.P1 * Constants.PIXEL_TO_UNIT,
                Body );
            edge.CollidesWith = Category.All ^ Category.Cat2;

            mExclusionRec = FixtureFactory.AttachRectangle(
                sensorWidth * Constants.PIXEL_TO_UNIT,
                sensorHeight * Constants.PIXEL_TO_UNIT,
                1,
                offset * Constants.PIXEL_TO_UNIT,
                Body );

            mExclusionRec.IsSensor = true;

            mExclusionRec.OnCollision = ( a, b, c ) => {
                if ( !b.IsSensor )
                {
                    mEdge.IgnoreCollisionWith( b );
                    ++mExclusionCount;
                }
                return !b.IsSensor;
            };
            
            mExclusionRec.OnSeparation = ( a, b ) => {
                mEdge.RestoreCollisionWith( b );
                --mExclusionCount;
            };
        }

        public override void Draw( GameTime gameTime )
        {
            //mExclusionRec.Body.DrawBody( GraphicsDevice, Texture, gameTime );

            mSpriteBatch.Begin( /*SpriteSortMode.Deferred, BlendState.NonPremultiplied*/ );

            Color shadowColor = /*mExclusionCount > 0
                                ? new Color( 255, 255, 255, 64 )
                                : */new Color( 0, 0, 0, 64 );

            Line2 line = mLine.Rotate( Body.Rotation );
            line += Body.Position * Constants.UNIT_TO_PIXEL;

            mSpriteBatch.DrawLine( line, Texture, shadowColor, 30 );
            mSpriteBatch.DrawLine( line, Texture, shadowColor, 20 );
            mSpriteBatch.DrawLine( line, Texture, BodyType == BodyType.Static ? Color.Black : Color.White, 10 );

            mSpriteBatch.End();
        }

    }
}
