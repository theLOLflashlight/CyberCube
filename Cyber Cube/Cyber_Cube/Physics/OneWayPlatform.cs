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
        public struct OneWayPlatformMaker : ISolidMaker
        {
            public Line2 Line;

            public OneWayPlatformMaker( Line2 line )
            {
                Line = line;
            }

            public Solid MakeSolid( CubeGame game, World world, Body body )
            {
                return new OneWayPlatform( game, world, body, Line );
            }
        }

        public OneWayPlatform( CubeGame game, World world, Body body, Line2 line )
            : base( game, world, body )
        {
            mLine = line - line.Center;

            mPlatform = Body.FindFixture( new SolidDescriptor( "platform" ) );
            mExclusionRec = Body.FindFixture( new SolidDescriptor( "exclusion" ) );
        }

        private Line2 mLine;

        private Fixture mPlatform;
        private Fixture mExclusionRec;

        public const float SENSOR_RANGE = 100f;
        public const float EDGE_THICKNESS = 1f;

        private int mExclusionCount = 0;

        public OneWayPlatform( CubeGame game,
                               World world,
                               Line2 line,
                               BodyType bodyType = BodyType.Static,
                               float density = 1,
                               Category categories = Category.Cat1,
                               Category oneWayCategories = Category.Cat2 )
            : base( game, world, line.Center.ToUnits(), 0, new OneWayPlatformMaker( line ) )
        {
            if ( line.X0 != line.X1 && line.Y0 != line.Y1 )
                throw new ArgumentException( "Only vertical and horizontal edges are supported." );

            Vector2 offset;
            float edgeWidth, edgeHeight, sensorWidth, sensorHeight;

            #region Variable setup
            {
                bool horizontal = line.Y0 == line.Y1;
                bool inverted = line.X1 < line.X0 || line.Y1 > line.Y0;

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

            mLine = line - line.Center;

            // Not really an Edge, just a thin rectangle. This is necessary for accurate 
            // collision and exclusion detection.
            mPlatform = FixtureFactory.AttachRectangle(
                edgeWidth.ToUnits(),
                edgeHeight.ToUnits(),
                density,
                Vector2.Zero,
                Body,
                new Flat( "platform" ) );

            mExclusionRec = FixtureFactory.AttachRectangle(
                sensorWidth.ToUnits(),
                sensorHeight.ToUnits(),
                0,
                offset.ToUnits(),
                Body,
                new SolidDescriptor( "exclusion" ) );
            mExclusionRec.IsSensor = true;

            // This edge allows the solid to collide with all bodies which can't pass through it.
            Fixture edge = FixtureFactory.AttachEdge(
                mLine.P0.ToUnits(),
                mLine.P1.ToUnits(),
                Body,
                new Flat() );

            Body.BodyType = bodyType;
            Body.CollisionCategories = categories;

            mPlatform.CollidesWith = oneWayCategories;
            edge.CollidesWith &= ~oneWayCategories;
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

        protected override void PostClone()
        {
            base.PostClone();

            mPlatform = Body.FindFixture( new SolidDescriptor( "platform" ) );
            mExclusionRec = Body.FindFixture( new SolidDescriptor( "exclusion" ) );
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
