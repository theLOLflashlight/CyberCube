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
using Cyber_Cube.IO;
using System.Reflection;

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

        private GameConsole mConsole;
        public Color BackgroundColor { get; private set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager( this );
            Content.RootDirectory = "Content";

            mConsole = new GameConsole( this );
            Camera = new Camera( this );
            mCube = new Cube( this );
            Player = new Player( mCube );

            mConsole.CommandExecuted += RunCommand;
            mConsole.Close();

            Components.Add( mConsole );
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
            IsMouseVisible = true;
            BackgroundColor = Color.CornflowerBlue;
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

            if ( Input.Keyboard_WasKeyPressed( Keys.RightControl ) )
                mCube.RotateAntiClockwise();

            if ( Input.Keyboard_WasKeyPressed( Keys.Space )
                 || Input.GamePad_WasButtonPressed( Buttons.Start ) )
            {
                mCube.Mode = mCube.Mode == Cube.CubeMode.Edit
                             ? Cube.CubeMode.Play
                             : Cube.CubeMode.Edit;
            }

            Player.Enabled = mCube.Mode == Cube.CubeMode.Play;

            base.Update( gameTime );

            if ( Input.Keyboard_WasKeyPressed( Keys.OemTilde ) )
                mConsole.Open();

            if ( Input.Keyboard_WasKeyReleased( Keys.Escape ) )
                mConsole.Close();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            GraphicsDevice.Clear( BackgroundColor );

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


            mSpriteBatch.DrawString( mFont, "X: " + Player.WorldPosition.X.ToString( "F6" ), new Vector2( 0, 60 ), Color.White );
            mSpriteBatch.DrawString( mFont, "Y: " + Player.WorldPosition.Y.ToString( "F6" ), new Vector2( 0, 90 ), Color.White );
            mSpriteBatch.DrawString( mFont, "Z: " + Player.WorldPosition.Z.ToString( "F6" ), new Vector2( 0, 120 ), Color.White );
            mSpriteBatch.End();
        }

        private bool RunCommand( string command )
        {
            switch ( command )
            {
            default:
                if ( command.StartsWith( "background " ) )
                {
                    string colorName = command.Substring( "background ".Length ).Trim();

                    var properties = typeof( Color ).GetProperties( BindingFlags.Public | BindingFlags.Static );

                    foreach ( var property in properties )
                    {
                        if ( property.Name.ToLower() == colorName.ToLower()
                             && property.PropertyType == typeof( Color ) )
                        {
                            BackgroundColor = (Color) property.GetValue( null, null );
                            return true;
                        }
                    }
                }
                break;

            case "help":
                mConsole.History.Add( @"Valid commands are:
[help]
[background <xna color>]
[exit]
[clear]" );
                return true;

            case "exit":
                this.Exit();
                return true;

            case "clear":
                mConsole.History.Clear();
                return true;
            }
            return false;
        }
    }
}
