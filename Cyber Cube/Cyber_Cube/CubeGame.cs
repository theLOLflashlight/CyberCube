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
        public readonly GameHud Hud;

        private Menu mMenu;
        private string version;

        private PauseMenu mPauseMenu;

        public CubeGame()
        {
            mInput = new InputState< Action >();

            SetUpBinds();

            mGraphicsDeviceManager = new GraphicsDeviceManager( this );
            Content.RootDirectory = "Content";

            Console = new GameConsole( this, this );
            Hud = new GameHud( this );
            Camera = new Camera( this );
            mCube = new Cube( this );
            Player = new Player( mCube, Vector3.UnitZ, Direction.North );
			Enemy = new Enemy( mCube, new Vector3(-0.5f, 0.7f, 1.0f), Direction.North );

            Console.CommandExecuted += RunCommand;
            Console.Close();

            Components.Add( Console );
            Components.Add( Hud );
            Components.Add( Player );
			Components.Add( Enemy );
            Components.Add( mCube );
            Components.Add( Camera );

            Components.Add( new GamerServicesComponent( this ) );

            version = "v0.1 alpha";
        }

        private void SetUpBinds()
        {
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
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content. Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            IsMouseVisible = true;
            BackgroundColor = Color.CornflowerBlue;
            mSpriteBatch = new SpriteBatch( GraphicsDevice );

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
            mMenu = new Menu(this, version);
            mPauseMenu = new PauseMenu(this);
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

            if (mMenu.CurrentMenuState != GameState.PlayingGame)
            {
                mMenu.Update();

                switch(mMenu.CurrentMenuState)
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
            else if (mMenu.CurrentMenuState == GameState.PauseGame)
            {
                mPauseMenu.Update();

                switch(mPauseMenu.Status)
                {
                    case 1:
                        mMenu.CurrentMenuState = GameState.PlayingGame;
                        break;
                    case 4:
                        mMenu.CurrentMenuState = GameState.MainMenu;
                        break;
                    default:
                        break;
                }
            }
            else if (mMenu.CurrentMenuState == GameState.PlayingGame)
            {

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
                    mPauseMenu.EnterPauseMenu();
                    mMenu.CurrentMenuState = GameState.PauseGame;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            if (mMenu.CurrentMenuState != GameState.PlayingGame)
            {
                mSpriteBatch.Begin();
                GraphicsDevice.Clear(Color.White);
                mMenu.Draw(mSpriteBatch);
                mSpriteBatch.End();
            }
            else if (mMenu.CurrentMenuState == GameState.PauseGame)
            {
                mSpriteBatch.Begin();
                mPauseMenu.Draw(mSpriteBatch);
                mSpriteBatch.End();
            }
            else if (mMenu.CurrentMenuState == GameState.PlayingGame)
            {
                GraphicsDevice.Clear( BackgroundColor );

                base.Draw( gameTime );

                /*mSpriteBatch.Begin();

                string output = mCube.CurrentFace.Name;
                var pos = mFont.MeasureString( output );

                mSpriteBatch.DrawString( mFont,
                                         output,
                                         new Vector2( GraphicsDevice.Viewport.Width - pos.X, pos.Y ),
                                         Color.White,
                                         mCube.UpDir.ToRadians(),
                                         mFont.MeasureString( output ) / 2,
                                         1,
                                         SpriteEffects.None,
                                         0 );
                mSpriteBatch.End();*/
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
