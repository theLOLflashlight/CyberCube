using CyberCube.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Physics
{
    public class OneWayPlatform : Solid
    {
        protected Line2 mLine;

        protected Fixture mEdge;
        protected Fixture mPlatform;
        protected Fixture mExclusionRec;

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

            Body = BodyFactory.CreateBody( world, center.ToUnits() );
            Body.BodyType = bodyType;
            Body.Mass = mass;

            mEdge = FixtureFactory.AttachEdge(
                mLine.P0.ToUnits(),
                mLine.P1.ToUnits(),
                Body );
            mEdge.CollidesWith = Category.All ^ Category.Cat2;

            // Not really an Edge, just a thin rectangle. This is necessary for accurate 
            // collision and exclusion detection.
            mPlatform = FixtureFactory.AttachRectangle(
                edgeWidth.ToUnits(),
                edgeHeight.ToUnits(),
                1,
                Vector2.Zero.ToUnits(),
                Body,
                new Flat() );
            mPlatform.CollidesWith = Category.Cat2;

            mExclusionRec = FixtureFactory.AttachRectangle(
                sensorWidth.ToUnits(),
                sensorHeight.ToUnits(),
                1,
                offset.ToUnits(),
                Body );

            mExclusionRec.IsSensor = true;
        }

        public override void Initialize()
        {
            base.Initialize();
            mExclusionRec.OnCollision = ( a, b, c ) => {
                if ( !b.IsSensor )
                {
                    mPlatform.IgnoreCollisionWith( b );
                    ++mExclusionCount;
                }
                return !b.IsSensor;
            };
            
            mExclusionRec.OnSeparation = ( a, b ) => {
                mPlatform.RestoreCollisionWith( b );
                --mExclusionCount;
            };
        }

        public override Solid Clone( World world )
        {
            OneWayPlatform clone = (OneWayPlatform) base.Clone( world );
            clone.mEdge = mEdge.CloneOnto( clone.Body );
            clone.mPlatform = mPlatform.CloneOnto( clone.Body );
            clone.mExclusionRec = mExclusionRec.CloneOnto( clone.Body );

            return clone;
        }

        public override void Draw( GameTime gameTime )
        {
            mSpriteBatch.Begin( /*SpriteSortMode.Deferred, BlendState.NonPremultiplied*/ );

            Color shadowColor = /*mExclusionCount > 0
                                ? new Color( 255, 255, 255, 64 )
                                : */new Color( 0, 0, 0, 64 );

            Line2 line = mLine.Rotate( Body.Rotation );
            line += Body.Position.ToPixels();

            mSpriteBatch.DrawLine( line, Texture, shadowColor, 30 );
            mSpriteBatch.DrawLine( line, Texture, shadowColor, 20 );
            mSpriteBatch.DrawLine( line, Texture, BodyType == BodyType.Static ? Color.Black : Color.White, 10 );

            mSpriteBatch.End();
        }

    }
}
