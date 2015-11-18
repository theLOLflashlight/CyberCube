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

        public class CubeGameProperties : RuntimeProperties
        {
#if WINDOWS
            public bool DebugView { get; set; } = false;
#endif
            public bool AllowMultipleJumping { get; set; } = false;

            public bool AllowManualGravity { get; set; } = false;

            public Color Background { get; set; } = Color.CornflowerBlue;

            public CubeGameProperties()
            {
#if DEBUG
                AllowMultipleJumping = true;
                AllowManualGravity = true;
#endif
            }

            protected override object Parse( string value, Type type )
            {
                object obj = base.Parse( value, type );

                if ( obj == null )
                {
                    if ( type == typeof( Color ) )
                        obj = Color_Parse( value );
                }

                return obj;
            }

            private static Color Color_Parse( string value )
            {
                var properties = typeof( Color ).GetProperties( BindingFlags.Public | BindingFlags.Static );

                foreach ( var property in properties )
                    if ( property.Name.ToLower() == value.ToLower()
                         && property.PropertyType == typeof( Color ) )
                        return (Color) property.GetValue( null, null );

                throw new FormatException( $"'{value}' is not the name of a color." );
            }
        }

        public readonly CubeGameProperties GameProperties = new CubeGameProperties();


        public Color BackgroundColor
        {
            get {
                return GameProperties.Background;
            }
            private set {
                GameProperties.Background = value;
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

            //mGraphicsDeviceManager.PreferMultiSampling = true;
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
            Input.AddReleasedBinding( Action.JumpStop, Keys.Up );

            Input.AddPressedBinding( Action.PlaceClone, Keys.C );
            Input.AddPressedBinding( Action.CycleClone, Keys.V );
            Input.AddPressedBinding( Action.DeleteClone, Keys.X );

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
            Input.AddReleasedBinding( Action.JumpStop, Buttons.A );

            Input.AddPressedBinding( Action.RotateLeft, Buttons.DPadLeft );
            Input.AddPressedBinding( Action.RotateRight, Buttons.DPadRight );
            Input.AddPressedBinding( Action.RotateUp, Buttons.DPadUp );
            Input.AddPressedBinding( Action.RotateDown, Buttons.DPadDown );
            Input.AddPressedBinding( Action.RotateClockwise, Buttons.RightShoulder );
            Input.AddPressedBinding( Action.RotateAntiClockwise, Buttons.LeftShoulder );

            Input.AddPressedBinding( Action.ToggleCubeMode, Buttons.Start );

            Input.AddPressedBinding( Action.PauseGame, Buttons.Y );
            //Input.AddPressedBinding( Action.PauseGame, Keys.P);


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
            PlayScreen.LoadContent( Content );
            MenuScreen.LoadContent( Content );
            PauseScreen.LoadContent( Content );
            PlayScreen.LoadContent( Content );

            Cube.LoadContent( Content );


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

            if ( Input.GetAction( Action.PauseGame ) )
            {
                mScreenManager.PushScreen( new PauseScreen( this ) );
            }
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

        private ConsoleMessage RunCommand( string command )
        {
            if ( Console.Closed )
                Console.Open();

            switch ( command.ToLower() )
            {
            default:
                try {
                    var ret = GameProperties.Evaluate( command );

                    if ( ret.Success )
                    {
                        if ( ret.Result != null )
                            return ret.Result.ToString();

                        return null;
                    }
                }
                catch ( Exception ex )
                {
                    return new ConsoleErrorMessage( ex.Message );
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
                return null;

            case "cheats on":
                GameProperties.AllowManualGravity = true;
                GameProperties.AllowMultipleJumping = true;
                return null;

            case "cheats off":
                GameProperties.AllowManualGravity = false;
                GameProperties.AllowMultipleJumping = false;
                return null;

            case "exit":
                this.Exit();
                return null;

            case "clear":
                Console.ClearHistory();
                return null;
            }

            return mScreenManager.TopScreen?.RunCommand( command );
        }

    }
}
