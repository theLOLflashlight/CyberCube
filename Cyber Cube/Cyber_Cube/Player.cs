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
        public new Game1 Game
        {
            get {
                return base.Game as Game1;
            }
        }

        private SpriteFont mFont;

        public Vector3 WorldPosition = Vector3.UnitZ;
        public Vector3 Velocity = Vector3.Zero;

        private Cube mCube;

        private Texture2D pixel;

        private Cube.Face CurrentFace
        {
            get {
                return mCube.CurrentFace;
            }
        }

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

        public Vector3 Transform2dTo3d( Vector2 vec2d )
        {
            Vector3 vec3d = new Vector3( vec2d, 0 );

            vec3d = Vector3.Transform( vec3d, Utils.RotateVecToVec( Vector3.UnitZ, Normal ) );
            var angle = mCube.UpDir.ToRadians() + mCube.CurrentFace.Dir.ToRadians();

            return Vector3.Transform( vec3d, Matrix.CreateFromAxisAngle( Normal, angle ) );
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            if ( mCube.Mode == Cube.CubeMode.Edit )
                return;

            var input = Game.mInput;

            var delta2d = Vector2.Zero;

            if ( input.Keyboard.IsKeyDown( Keys.Right ) )
                delta2d.X++;

            if ( input.Keyboard.IsKeyDown( Keys.Left ) )
                delta2d.X--;

            if ( input.Keyboard.IsKeyDown( Keys.Up ) )
                delta2d.Y++;

            if ( input.Keyboard.IsKeyDown( Keys.Down ) )
                delta2d.Y--;

            Vector3 delta3d = Transform2dTo3d( delta2d );

            WorldPosition += (delta3d / 100) * (gameTime.ElapsedGameTime.Milliseconds / 10f);

            if ( WorldPosition.X > 1 )
            {
                WorldPosition.X = 1;
                mCube.Rotate( CurrentFace.VectorToDirection( Vector3.UnitX ) );
            }
            else if ( WorldPosition.X < -1 )
            {
                WorldPosition.X = -1;
                mCube.Rotate( CurrentFace.VectorToDirection( -Vector3.UnitX ) );
            }

            if ( WorldPosition.Y > 1 )
            {
                WorldPosition.Y = 1;
                mCube.Rotate( CurrentFace.VectorToDirection( Vector3.UnitY ) );
            }
            else if ( WorldPosition.Y < -1 )
            {
                WorldPosition.Y = -1;
                mCube.Rotate( CurrentFace.VectorToDirection( -Vector3.UnitY ) );
            }

            if ( WorldPosition.Z > 1 )
            {
                WorldPosition.Z = 1;
                mCube.Rotate( CurrentFace.VectorToDirection( Vector3.UnitZ ) );
            }
            else if ( WorldPosition.Z < -1 )
            {
                WorldPosition.Z = -1;
                mCube.Rotate( CurrentFace.VectorToDirection( -Vector3.UnitZ ) );
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
                mCube.Effect.Projection,
                mCube.Effect.View,
                mCube.Effect.World );

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
                                    -mCube.UpDir.ToRadians(),// 0,//Up.ToRadians(),
                                    mFont.MeasureString( "A" ) / 2,
                                    1f,
                                    SpriteEffects.None,
                                    0 );
            spriteBatch.End();
        }

    }
}
