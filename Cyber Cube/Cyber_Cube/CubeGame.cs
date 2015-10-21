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
using CyberCube.MenuFiles;
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

		public Enemy Enemy { get; private set; }

        public Color BackgroundColor { get; private set; }

        private GraphicsDeviceManager mGraphicsDeviceManager;

        private SpriteBatch mSpriteBatch;
        private SpriteFont mFont;

        private Cube mCube;
        public readonly GameConsole Console;

        private Menu theMenu;
        private string version;

        private PauseMenu thePauseMenu;

        public CubeGame()
        {
            mInput = new InputState< Action >();
            mGraphicsDeviceManager = new GraphicsDeviceManager( this );
            Content.RootDirectory = "Content";

            Console = new GameConsole( this, this );
            Camera = new Camera( this );
            mCube = new Cube( this );
            Player = new Player( mCube, Vector3.UnitZ, Direction.North );
			Enemy = new Enemy( mCube, new Vector3(-0.5f, 0.7f, 1.0f), Direction.North );

            Console.CommandExecuted += RunCommand;
            Console.Close();

            Components.Add( Console );
            Components.Add( Player );
			Components.Add( Enemy );
            Components.Add( mCube );
            Components.Add( Camera );

            Components.Add( new GamerServicesComponent( this ) );

            version = "v0.1 alpha";
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

            Input.AddPressedBinding( Action.PauseGame, Buttons.Y );
            Input.AddPressedBinding( Action.PauseGame, Keys.P);


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
            theMenu = new Menu(this, version);
            thePauseMenu = new PauseMenu(this);
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
            if (theMenu.CurrentMenuState == GameState.MainMenu)
            {
                theMenu.Update();

                switch(theMenu.CurrentMenuState)
                {
                    case GameState.LoadGame:
                        break;
                    case GameState.LevelEditor:
                        break;
                    case GameState.ExitGame:
                        this.Exit();
                        break;
                    default:
                        break;
                }
            }
            else if (theMenu.CurrentMenuState == GameState.PauseGame)
            {
                thePauseMenu.Update();

                switch(thePauseMenu.Status)
                {
                    case 1:
                        theMenu.CurrentMenuState = GameState.PlayingGame;
                        break;
                    case 4:
                        theMenu.CurrentMenuState = GameState.MainMenu;
                        break;
                    default:
                        break;
                }
            }
            else if (theMenu.CurrentMenuState == GameState.PlayingGame)
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
                    {
                        mCube.RotateClockwise();
                        --Player.UpDir;
                    }

                    if ( Input.GetAction( Action.RotateAntiClockwise ) )
                    {
                        mCube.RotateAntiClockwise();
                        ++Player.UpDir;
                    }

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

                if ( Input.GetAction( Action.PauseGame ))
                {
                    thePauseMenu.EnterPauseMenu();
                    theMenu.CurrentMenuState = GameState.PauseGame;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            if (theMenu.CurrentMenuState == GameState.MainMenu)
            {
                mSpriteBatch.Begin();
                GraphicsDevice.Clear(Color.White);
                theMenu.Draw(mSpriteBatch);
                mSpriteBatch.End();
            }
            else if (theMenu.CurrentMenuState == GameState.PauseGame)
            {
                mSpriteBatch.Begin();
                thePauseMenu.Draw(mSpriteBatch);
                mSpriteBatch.End();
            }
            else if (theMenu.CurrentMenuState == GameState.PlayingGame)
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
