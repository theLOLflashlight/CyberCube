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
using CyberCube.IO;
using System.Reflection;

namespace CyberCube
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CubeGame
        : Microsoft.Xna.Framework.Game
        , IInputProvider< Action >
    {
        private InputState< Action > mInput;

        public InputState< Action > Input
        {
            get {
                return mInput;
            }
        }

        InputState IInputProvider.Input
        {
            get {
                return mInput;
            }
        }

        public Camera Camera { get; private set; }

        public Player Player { get; private set; }

        public Color BackgroundColor { get; private set; }

        private GraphicsDeviceManager mGraphicsDeviceManager;

        private SpriteBatch mSpriteBatch;
        private SpriteFont mFont;

        private Cube mCube;
        public readonly GameConsole Console;

        public CubeGame()
        {
            mInput = new InputState< Action >();
            mGraphicsDeviceManager = new GraphicsDeviceManager( this );
            Content.RootDirectory = "Content";

            Console = new GameConsole( this, this );
            Camera = new Camera( this );
            mCube = new Cube( this );
            Player = new Player( mCube );

            Console.CommandExecuted += RunCommand;
            Console.Close();

            Components.Add( Console );
            Components.Add( Player );
			//Components.Add( new Enemy(mCube, mCube.mTopFace, new Vector2(-.5f, -.5f)) );
            Components.Add( mCube );
            Components.Add( Camera );

            Components.Add( new GamerServicesComponent( this ) );
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content. Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
			base.Initialize();
            IsMouseVisible = true;
            BackgroundColor = Color.CornflowerBlue;
            mSpriteBatch = new SpriteBatch( GraphicsDevice );


            Input.AddBinding( Action.MoveLeft, Keys.Left );
            Input.AddBinding( Action.MoveRight, Keys.Right );
            Input.AddBinding( Action.MoveUp, Keys.Up );
            Input.AddBinding( Action.MoveDown, Keys.Down );
            Input.AddPressedBinding( Action.Jump, Keys.Up );

            Input.AddPressedBinding( Action.RotateLeft, Keys.Left );
            Input.AddPressedBinding( Action.RotateRight, Keys.Right );
            Input.AddPressedBinding( Action.RotateUp, Keys.Up );
            Input.AddPressedBinding( Action.RotateDown, Keys.Down );
            Input.AddPressedBinding( Action.RotateClockwise, Keys.RightShift );
            Input.AddPressedBinding( Action.RotateAntiClockwise, Keys.RightControl );

            Input.AddPressedBinding( Action.ToggleCubeMode, Keys.Space );


            Input.AddBinding( Action.MoveLeft, Buttons.DPadLeft );
            Input.AddBinding( Action.MoveRight, Buttons.DPadRight );
            Input.AddBinding( Action.MoveUp, Buttons.DPadUp );
            Input.AddBinding( Action.MoveDown, Buttons.DPadDown );
            Input.AddPressedBinding( Action.Jump, Buttons.A );

            Input.AddPressedBinding( Action.RotateLeft, Buttons.DPadLeft );
            Input.AddPressedBinding( Action.RotateRight, Buttons.DPadRight );
            Input.AddPressedBinding( Action.RotateUp, Buttons.DPadUp );
            Input.AddPressedBinding( Action.RotateDown, Buttons.DPadDown );
            Input.AddPressedBinding( Action.RotateClockwise, Buttons.RightShoulder );
            Input.AddPressedBinding( Action.RotateAntiClockwise, Buttons.LeftShoulder );

            Input.AddPressedBinding( Action.ToggleCubeMode, Buttons.Start );


            Input.AddBinding( Action.MoveLeft, i => -i.GamePad.ThumbSticks.Left.X );
            Input.AddBinding( Action.MoveRight, i => i.GamePad.ThumbSticks.Left.X );
            Input.AddBinding( Action.MoveUp, i => i.GamePad.ThumbSticks.Left.Y );
            Input.AddBinding( Action.MoveDown, i => -i.GamePad.ThumbSticks.Left.Y );

            base.Initialize();

            StorageManager.Instance.Initialize();
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
            Input.Refresh();

            // Allows the game to exit
            if ( GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed )
                this.Exit();

            if ( !Input.HasFocus )
            {
            if ( mCube.Mode == Cube.CubeMode.Edit )
            {
                    if ( Input.GetAction( Action.RotateRight ) )
                    mCube.RotateRight();

                    if ( Input.GetAction( Action.RotateLeft ) )
                    mCube.RotateLeft();

                    if ( Input.GetAction( Action.RotateUp ) )
                        mCube.RotateTop();

                    if ( Input.GetAction( Action.RotateDown ) )
                        mCube.RotateBottom();
            }

                if ( Input.GetAction( Action.RotateClockwise ) )
                mCube.RotateClockwise();

                if ( Input.GetAction( Action.RotateAntiClockwise ) )
                mCube.RotateAntiClockwise();

                if ( Input.GetAction( Action.ToggleCubeMode ) )
                mCube.Mode = mCube.Mode == Cube.CubeMode.Edit
                             ? Cube.CubeMode.Play
                             : Cube.CubeMode.Edit;
            }

            Player.Enabled = mCube.Mode == Cube.CubeMode.Play;

            base.Update( gameTime );

            if ( Input.Keyboard_WasKeyPressed( Keys.OemTilde ) )
                Console.Open();

            if ( Input.Keyboard_WasKeyReleased( Keys.Escape ) )
                Console.Close();
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
                Console.AddMessage( @"Valid commands are:
[help]
[background <xna color>]
[exit]
[clear]" );
                return true;

            case "exit":
                this.Exit();
                return true;

            case "clear":
                Console.ClearHistory();
                return true;
            }
            return false;
        }
    }
}
