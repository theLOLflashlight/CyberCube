using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{
    public class Player : DrawableGameComponent
    {
        private SpriteFont mFont;

        public Vector3 WorldPosition = Vector3.UnitZ;
        public Vector3 Velocity = Vector3.Zero;

        private Cube mCube;

        private Texture2D pixel;

        public Vector3 Normal
        {
            get {
                return mCube.CurrentFace.Normal;
            }
        }

        public Player( Cube cube )
            : base( cube.Game )
        {
            mCube = cube;
        }

        public override void Initialize()
        {
            base.Initialize();

            pixel = new Texture2D( GraphicsDevice, 2, 2 );
            pixel.SetData( new[] { Color.White, Color.White, Color.White, Color.White } );

            this.Visible = true;
            this.DrawOrder = 1;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            mFont = Game.Content.Load<SpriteFont>( "MessageFont" );
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            if ( mCube.Mode == Cube.CubeMode.Edit )
                return;

            var input = (Game as Game1).mInput;

            var deltaPos = Vector3.Zero;

            if ( input.Keyboard.IsKeyDown( Keys.Right ) )
                deltaPos.X++;

            if ( input.Keyboard.IsKeyDown( Keys.Left ) )
                deltaPos.X--;

            if ( input.Keyboard.IsKeyDown( Keys.Up ) )
                deltaPos.Y++;

            if ( input.Keyboard.IsKeyDown( Keys.Down ) )
                deltaPos.Y--;


            //deltaPos.Y -= gameTime.ElapsedGameTime.Milliseconds / 1000f;

            deltaPos = Vector3.Transform( deltaPos, Utils.RotateVecToVec( Vector3.UnitZ, Normal ) );

            var angle = mCube.Up.ToRadians() + mCube.CurrentFace.Dir.ToRadians();
            deltaPos = Vector3.Transform( deltaPos, Matrix.CreateFromAxisAngle( Normal, angle ) );

            WorldPosition += deltaPos / 100;

            bool moved = false;

            if ( WorldPosition.X > 1 )
            {
                moved = true;
                WorldPosition.X = 1;
            }
            if ( WorldPosition.X < -1 )
            {
                moved = true;
                WorldPosition.X = -1;
            }
            if ( WorldPosition.Y > 1 )
            {
                moved = true;
                WorldPosition.Y = 1;
            }
            if ( WorldPosition.Y < -1 )
            {
                moved = true;
                WorldPosition.Y = -1;
            }
            if ( WorldPosition.Z > 1 )
            {
                moved = true;
                WorldPosition.Z = 1;
            }
            if ( WorldPosition.Z < -1 )
            {
                moved = true;
                WorldPosition.Z = -1;
            }

            if ( moved )
            {
                if ( input.Keyboard.IsKeyDown( Keys.Right ) )
                    mCube.RotateRight();

                if ( input.Keyboard.IsKeyDown( Keys.Left ) )
                    mCube.RotateLeft();

                if ( input.Keyboard.IsKeyDown( Keys.Up ) )
                    mCube.RotateUp();

                if ( input.Keyboard.IsKeyDown( Keys.Down ) )
                    mCube.RotateDown();
            }


        }

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );

            SpriteBatch spriteBatch = new SpriteBatch( GraphicsDevice );

            // Find screen equivalent of 3D location in world
            Vector3 worldLocation = WorldPosition;
            Vector3 screenLocation = GraphicsDevice.Viewport.Project(
                worldLocation,
                mCube.effect.Projection,
                mCube.effect.View,
                mCube.effect.World );

            // Draw our pixel texture there
            spriteBatch.Begin();
            spriteBatch.Draw( pixel,
                              new Vector2(
                                  screenLocation.X,
                                  screenLocation.Y ),
                              Color.Black );

            spriteBatch.End();

            if ( this.Visible )
                return;

            var pos = Vector3.Transform( WorldPosition, Utils.RotateVecToVec( mCube.CurrentFace.Normal, Vector3.UnitZ ) );

            spriteBatch.Begin();
            spriteBatch.DrawString( mFont,
                                    "A",
                                    new Vector2( Cube.Face.SIZE / 2 ) + (new Vector2( pos.X, pos.Y ) * 50),
                                    Color.Black,
                                    -mCube.Up.ToRadians(),// 0,//Up.ToRadians(),
                                    mFont.MeasureString( "A" ) / 2,
                                    1f,
                                    SpriteEffects.None,
                                    0 );
            spriteBatch.End();
        }

    }
}
