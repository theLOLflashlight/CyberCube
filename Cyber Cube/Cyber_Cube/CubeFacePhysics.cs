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

            private readonly List< Solid > mSolids = new List<Solid>();

            public const float UNIT_TO_PIXEL = 10;
            public const float PIXEL_TO_UNIT = 1 / UNIT_TO_PIXEL;

            private void SetUpWorld()
            {
                mWorld = new World( new Vector2( 0, 9.8f ) );

                mSolids.Add(
                    new RecSolid(
                        Game,
                        mWorld,
                        new Rectangle( 36, 0, 10, 10 ),
                        BodyType.Dynamic ) );

                mSolids.Add(
                    new RecSolid(
                        Game,
                        mWorld,
                        new Rectangle( 0, HEIGHT - 10, WIDTH, 10 ) ) );

                mSolids.Add(
                    new RecSolid(
                        Game,
                        mWorld,
                        new Rectangle( 10, 70, 30, 10 ) ) );

                mSolids.Add(
                    new RecSolid(
                        Game,
                        mWorld,
                        new Rectangle( WIDTH - 15, 15, 10, 30 ) ) );
            }

            private void LoadPhysics()
            {
                
            }

            public override void Update( GameTime gameTime )
            {
                base.Update( gameTime );

                //mWorld.Step( (float) gameTime.ElapsedGameTime.TotalSeconds );
            }

            private void DrawBodies( GameTime gameTime )
            {
                foreach ( Solid solid in mSolids )
                    solid.Draw( gameTime );
            }

        }

    }
}
