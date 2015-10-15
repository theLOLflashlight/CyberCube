using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Cyber_Cube
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch mSpriteBatch;

        public readonly InputState Input = new InputState();
        public readonly Camera Camera;

        private Cube mCube;
        private SpriteFont mFont;
        public Player Player { get; private set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager( this );
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Camera = new Camera( this );
            mCube = new Cube( this );
            Player = new Player( mCube );

            Components.Add( Player );
            Components.Add( mCube );
            Components.Add( Camera );
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content. Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            mSpriteBatch = new SpriteBatch( GraphicsDevice );

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            mFont = Content.Load<SpriteFont>( "MessageFont" );

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update( GameTime gameTime )
        {
            Input.Update( gameTime );

            // Allows the game to exit
            if ( GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed )
                this.Exit();

            if ( mCube.Mode == Cube.CubeMode.Edit )
            {
                if ( Input.Keyboard_WasKeyPressed( Keys.Right ) )
                    mCube.RotateRight();

                if ( Input.Keyboard_WasKeyPressed( Keys.Left ) )
                    mCube.RotateLeft();

                if ( Input.Keyboard_WasKeyPressed( Keys.Up ) )
                    mCube.RotateUp();

                if ( Input.Keyboard_WasKeyPressed( Keys.Down ) )
                    mCube.RotateDown();
            }

            if ( Input.Keyboard_WasKeyPressed( Keys.RightShift ) )
                mCube.RotateClockwise();

            if ( Input.Keyboard_WasKeyPressed( Keys.LeftShift ) )
                mCube.RotateAntiClockwise();

            if ( Input.Keyboard_WasKeyPressed( Keys.Space )
                 || Input.GamePad_WasButtonPressed( Buttons.Start ) )
            {
                mCube.Mode = mCube.Mode == Cube.CubeMode.Edit
                             ? Cube.CubeMode.Play
                             : Cube.CubeMode.Edit;

                Player.Enabled = mCube.Mode == Cube.CubeMode.Play;
            }

            base.Update( gameTime );
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            GraphicsDevice.Clear( Color.CornflowerBlue );

            // TODO: Add your drawing code here

            base.Draw( gameTime );

            mSpriteBatch.Begin();

            string output = mCube.CurrentFace.Name;
            var pos = mFont.MeasureString( output );

            mSpriteBatch.DrawString( mFont,
                                    output,
                                    new Vector2( Window.ClientBounds.Width - pos.X, pos.Y ),
                                    Color.White,
                                    mCube.UpDir.ToRadians(),
                                    mFont.MeasureString( output ) / 2,
                                    1,
                                    SpriteEffects.None,
                                    0 );

            mSpriteBatch.DrawString( mFont, Camera.Position.ToString(), Vector2.Zero, Color.White );
            mSpriteBatch.DrawString( mFont, Camera.UpVector.ToString(), new Vector2( 0, 30 ), Color.White );
            mSpriteBatch.DrawString( mFont, "X: " + Player.WorldPosition.X, new Vector2( 0, 60 ), Color.White );
            mSpriteBatch.DrawString( mFont, "Y: " + Player.WorldPosition.Y, new Vector2( 0, 90 ), Color.White );
            mSpriteBatch.DrawString( mFont, "Z: " + Player.WorldPosition.Z, new Vector2( 0, 120 ), Color.White );
            mSpriteBatch.End();
        }
    }
}
