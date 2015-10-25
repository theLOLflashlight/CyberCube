using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CyberCube.Physics;

namespace CyberCube
{
    public partial class Cube
    {
        public partial class Face
        {
            private World mWorld;

            public World World
            {
                get {
                    return mWorld;
                }
            }

            private readonly List< Solid > mSolids = new List<Solid>();

            private void SetUpWorld()
            {
                mWorld = new World( Vector2.Zero );

                var box = new RecSolid(
                        Game,
                        mWorld,
                        new Rectangle( 360, 0, 100, 100 ),
                        BodyType.Dynamic, 30 );

                box.Body.UseAdHocGravity = true;
                box.Body.AdHocGravity = new Vector2( 0, 9.8f );

                mSolids.Add( box );

                mSolids.Add(
                    new RecSolid(
                        Game,
                        mWorld,
                        new Rectangle( 0, HEIGHT - 100, WIDTH, 100 ) ) );

                mSolids.Add(
                    new RecSolid(
                        Game,
                        mWorld,
                        new Rectangle( 100, 700, 300, 100 ) ) );

                mSolids.Add(
                    new RecSolid(
                        Game,
                        mWorld,
                        new Rectangle( WIDTH - 150, 150, 100, 300 ) ) );

                mSolids.Add( new EdgeSolid( Game, mWorld, new Line2( 400, 200, 100, 200 ) ) );
                mSolids.Add( new EdgeSolid( Game, mWorld, new Line2( 100, 400, 400, 400 ) ) );
                mSolids.Add( new EdgeSolid( Game, mWorld, new Line2( 700, 500, 700, 200 ) ) );
                //mSolids.Add( new EdgeSolid( Game, mWorld, new Line2( 400, 200, 550, 300 ) ) );
                //mSolids.Add( new EdgeSolid( Game, mWorld, new Line2( 380, 910, 380, 790 ) ) );
                //mSolids.Add( new EdgeSolid( Game, mWorld, new Line2( 240, 910, 240, 790 ) ) );
                //mSolids.Add( new EdgeSolid( Game, mWorld, new Line2( 100, 910, 100, 790 ) ) );
            }

            public void AddSolid( Solid solid )
            {
                mSolids.Add( solid );
                if ( Game.Initialized )
                    solid.Initialize();
            }

            private void LoadPhysics()
            {
                
            }

            public override void Update( GameTime gameTime )
            {
                base.Update( gameTime );

                //foreach ( Solid solid in mSolids.ToList() )
                //    foreach ( var dir in ClampSolid( solid ) )
                //        MoveSolid( solid, dir );

                if ( Cube.Mode == CubeMode.Play )
                    mWorld.Step( (float) gameTime.ElapsedGameTime.TotalSeconds );
            }

            private void DrawBodies( GameTime gameTime )
            {
                foreach ( Solid solid in mSolids )
                    solid.Draw( gameTime );
            }

            private IEnumerable<CompassDirection> ClampSolid( Solid solid )
            {
                Vector2 pos = solid.Body.Position * Physics.Constants.UNIT_TO_PIXEL;

                if ( pos.X > WIDTH )
                {
                    solid.Body.Position = new Vector2( WIDTH, pos.Y ) * Physics.Constants.PIXEL_TO_UNIT;

                    var dir = VectorToDirection( Vector3.UnitX );
                    if ( dir != null )
                        yield return dir.Value;
                }
                else if ( pos.X < 0 )
                {
                    solid.Body.Position = new Vector2( 0, pos.Y ) * Physics.Constants.PIXEL_TO_UNIT;

                    var dir = VectorToDirection( -Vector3.UnitX );
                    if ( dir != null )
                        yield return dir.Value;
                }

                if ( pos.Y > HEIGHT )
                {
                    solid.Body.Position = new Vector2( pos.X, HEIGHT ) * Physics.Constants.PIXEL_TO_UNIT;

                    var dir = VectorToDirection( Vector3.UnitY );
                    if ( dir != null )
                        yield return dir.Value;
                }
                else if ( pos.Y < 0 )
                {
                    solid.Body.Position = new Vector2( pos.X, 0 ) * Physics.Constants.PIXEL_TO_UNIT;

                    var dir = VectorToDirection( -Vector3.UnitY );
                    if ( dir != null )
                        yield return dir.Value;
                }
            }

            protected void MoveSolid( Solid solid, CompassDirection dir )
            {
                Face nextFace = AdjacentFace( dir );
                var backDir = nextFace.BackwardsDirectionFrom( this );

                mSolids.Remove( solid );
                solid.World = nextFace.World;
                nextFace.mSolids.Add( solid );

                Body body = solid.Body;

                float pastRotation = body.Rotation;

                body.Rotation += nextFace.Rotation;
                body.AdHocGravity = Vector2.UnitY.Rotate( -body.Rotation ).Rounded();
                body.Awake = true;

                body.Position = body.Position.Rotate( pastRotation - body.Rotation );
                body.LinearVelocity = body.LinearVelocity.Rotate( pastRotation - body.Rotation );
            }

        }

    }
}
