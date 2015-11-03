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
            public World World
            {
                get; private set;
            }

            protected readonly List< Solid > mSolids = new List<Solid>();

            public void CopySolids( Face from )
            {
                mSolids.Clear();
                World.Clear();
                foreach ( Solid solid in from.mSolids )
                {
                    mSolids.Add( solid.Clone( World ) );
                }
            }

            public void ResetWorld()
            {
                mSolids.Clear();
                World.Clear();
                SetUpWorld();
            }

            private void SetUpWorld()
            {
                World = new World( Vector2.Zero );

                var box = new Box(
                        Game,
                        World,
                        new Rectangle( 360, 0, 100, 100 ),
                        BodyType.Dynamic, 30, Category.Cat2 );

                box.Body.UseAdHocGravity = true;
                box.Body.AdHocGravity = new Vector2( 0, 9.8f );

                AddSolid( box );

                AddSolid(
                    new Quarterpipe(
                        Game,
                        World,
                        100,
                        new Vector2( 850, 850 ),
                        Quarterpipe.Type.SE ) );

                AddSolid(
                    new Corner(
                        Game,
                        World,
                        100,
                        new Vector2( 450, 750 ),
                        Corner.Type.NE ) );

                AddSolid(
                    new Box(
                        Game,
                        World,
                        new Rectangle( 900, 600, 100, 400 ) ) );

                AddSolid(
                    new Box(
                        Game,
                        World,
                        new Rectangle( 0, HEIGHT - 100, WIDTH, 100 ) ) );

                AddSolid(
                    new Box(
                        Game,
                        World,
                        new Rectangle( 100, 700, 300, 100 ) ) );

                AddSolid(
                    new Box(
                        Game,
                        World,
                        new Rectangle( WIDTH - 150, 150, 100, 300 ) ) );

                AddSolid( new OneWayPlatform( Game, World, new Line2( 400, 200, 100, 200 ) ) );
                AddSolid( new OneWayPlatform( Game, World, new Line2( 100, 400, 400, 400 ) ) );
                AddSolid( new OneWayPlatform( Game, World, new Line2( 700, 500, 700, 200 ) ) );
                //AddSolid( new OneWayPlatform( Game, World, new Line2( 400, 200, 550, 300 ) ) );
                //AddSolid( new OneWayPlatform( Game, World, new Line2( 380, 910, 380, 790 ) ) );
                //AddSolid( new OneWayPlatform( Game, World, new Line2( 240, 910, 240, 790 ) ) );
                //AddSolid( new OneWayPlatform( Game, World, new Line2( 100, 910, 100, 790 ) ) );
            }

            public void AddSolid( Solid solid )
            {
                mSolids.Add( solid );
                if ( Game.Initialized )
                    solid.Initialize();
            }

            public void RemoveSolid( Solid solid )
            {
                mSolids.Remove( solid );
                World.RemoveBody( solid.Body );
            }

            private IEnumerable<CompassDirection> ClampSolid( Solid solid )
            {
                Vector2 pos = solid.Body.Position.ToPixels();

                if ( pos.X > WIDTH )
                {
                    solid.Body.Position = new Vector2( WIDTH, pos.Y ).ToUnits();

                    var dir = VectorToDirection( Vector3.UnitX );
                    if ( dir != null )
                        yield return dir.Value;
                }
                else if ( pos.X < 0 )
                {
                    solid.Body.Position = new Vector2( 0, pos.Y ).ToUnits();

                    var dir = VectorToDirection( -Vector3.UnitX );
                    if ( dir != null )
                        yield return dir.Value;
                }

                if ( pos.Y > HEIGHT )
                {
                    solid.Body.Position = new Vector2( pos.X, HEIGHT ).ToUnits();

                    var dir = VectorToDirection( Vector3.UnitY );
                    if ( dir != null )
                        yield return dir.Value;
                }
                else if ( pos.Y < 0 )
                {
                    solid.Body.Position = new Vector2( pos.X, 0 ).ToUnits();

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
