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
using CyberCube.Levels;
using CyberCube.Screens;
using System.ComponentModel;

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

        public class GameProperties
        {
#if WINDOWS
            public bool DebugView { get; set; } = false;
#endif

            public Color Background { get; set; } = Color.CornflowerBlue;


            public static object Parse( string value, Type type )
            {
                if ( type == typeof( string ) )
                    return value;

                if ( type == typeof( bool ) )
                    return bool.Parse( value );

                if ( type == typeof( int ) )
                    return int.Parse( value );

                if ( type == typeof( float ) )
                    return float.Parse( value );

                if ( type == typeof( Color ) )
                    return Color_Parse( value );

                return null;
            }
            
            private static Color Color_Parse( string value )
            {
                var properties = typeof( Color ).GetProperties( BindingFlags.Public | BindingFlags.Static );

                foreach ( var property in properties )
                    if ( property.Name.ToLower() == value.ToLower()
                         && property.PropertyType == typeof( Color ) )
                        return (Color) property.GetValue( null, null );

                throw new FormatException( $"{value} is not a color." );
            }
        }

        public readonly GameProperties RuntimeProperties = new GameProperties();


        public Color BackgroundColor
        {
            get {
                return RuntimeProperties.Background;
            }
            private set {
                RuntimeProperties.Background = value;
            }
        }

        protected GraphicsDeviceManager mGraphicsDeviceManager;

        private SpriteBatch mSpriteBatch;
        private SpriteFont mFont;

        public readonly GameConsole Console;

        private ScreenManager mScreenManager;

        public bool Initialized
        {
            get; private set;
        }

        public CubeGame()
        {
            Initialized = false;
            mInput = new InputState< Action >();

            SetUpBinds();

            mGraphicsDeviceManager = new GraphicsDeviceManager( this );

            mGraphicsDeviceManager.IsFullScreen = false;
            mGraphicsDeviceManager.PreferredBackBufferHeight = 720;
            mGraphicsDeviceManager.PreferredBackBufferWidth = 1280;

            Content.RootDirectory = "Content";

            mScreenManager = new ScreenManager( this );

            Console = new GameConsole( this, this );

            Console.CommandExecuted += RunCommand;
            Console.Close();

            Components.Add( Console );
            Components.Add( mScreenManager );

            Components.Add( new GamerServicesComponent( this ) );
        }

        private void SetUpBinds()
        {
            Input.AddBinding( Action.MoveLeft, Keys.Left );
            Input.AddBinding( Action.MoveRight, Keys.Right );
            Input.AddBinding( Action.MoveUp, Keys.Up );
            Input.AddBinding( Action.MoveDown, Keys.Down );
            Input.AddPressedBinding( Action.Jump, Keys.Up );
            Input.AddReleasedBinding( Action.JumpEnd, Keys.Up );

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
            Input.AddReleasedBinding( Action.JumpEnd, Buttons.A );

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
            Initialized = true;

            IsMouseVisible = true;
            BackgroundColor = Color.CornflowerBlue;
            mSpriteBatch = new SpriteBatch( GraphicsDevice );

            StorageManager.Instance.Initialize();
            mScreenManager.PushScreen( new MenuScreen( this ) );
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            mFont = Content.Load<SpriteFont>( "MessageFont" );

            PlayableCube.LoadContent( Content );
            EditableCube.LoadContent( Content );
            Menu.LoadContent( Content );
            PauseMenu.LoadContent( Content );

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

            base.Update( gameTime );

            if ( Input.Keyboard_WasKeyPressed( Keys.OemTilde ) )
                Console.Open();

            if ( Input.Keyboard_WasKeyReleased( Keys.Escape ) )
                Console.Close();

            //if ( Input.GetAction( Action.PauseGame ))
            //{
            //    mPauseMenu.EnterPauseMenu();
            //    mMenu.CurrentMenuState = GameState.PauseGame;
            //}
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            GraphicsDevice.Clear( BackgroundColor );

            base.Draw( gameTime );
        }

        private bool RunCommand( string command )
        {
            if ( Console.Closed )
                Console.Open();

            switch ( command )
            {
            default:
                command = command.Trim();
                string[] tokens = command.Split( ' ' );

                if ( tokens.Length == 0 )
                    break;

                var properties = typeof( GameProperties ).GetProperties();

                foreach ( var property in properties )
                {
                    if ( tokens[ 0 ].ToLower() == property.Name.ToLower() )
                    {
                        if ( tokens.Length == 1 )
                        {
                            Console.AddMessage( property.GetValue( RuntimeProperties, null ).ToString() );
                            return true;
                        }
                        else if ( tokens.Length == 3 && tokens[ 1 ] == "=" )
                        {
                            try {
                                property.SetValue(
                                    RuntimeProperties,
                                    GameProperties.Parse( tokens[ 2 ], property.PropertyType ),
                                    null );
                            }
                            catch
                            {
                                Console.AddMessage( new ConsoleErrorMessage(
                                    $"{property.Name} must be of type {property.PropertyType.Name}." ) );
                            }
                            return true;
                        }
                        break;
                    }
                }
                break;

            case "help":
                Console.AddMessage(
@"Valid commands are:
[help]
[reset]
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
