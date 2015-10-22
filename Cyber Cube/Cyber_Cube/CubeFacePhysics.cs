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

                mSolids.Add( new EdgeSolid( Game, mWorld, new Line2( 100, 200, 400, 200 ) ) );
                //mSolids.Add( new EdgeSolid( Game, mWorld, new Line2( 400, 200, 550, 300 ) ) );
                mSolids.Add( new EdgeSolid( Game, mWorld, new Line2( 100, 400, 400, 400 ) ) );
                mSolids.Add( new EdgeSolid( Game, mWorld, new Line2( 700, 500, 700, 200 ) ) );
                //mSolids.Add( new EdgeSolid( Game, mWorld, new Line2( 380, 910, 380, 790 ) ) );
                //mSolids.Add( new EdgeSolid( Game, mWorld, new Line2( 240, 910, 240, 790 ) ) );
                //mSolids.Add( new EdgeSolid( Game, mWorld, new Line2( 100, 910, 100, 790 ) ) );
            }

            private void LoadPhysics()
            {
                
            }

            public override void Update( GameTime gameTime )
            {
                base.Update( gameTime );

                mWorld.Step( (float) gameTime.ElapsedGameTime.TotalSeconds );
            }

            private void DrawBodies( GameTime gameTime )
            {
                foreach ( Solid solid in mSolids )
                    solid.Draw( gameTime );
            }

        }

    }
}
