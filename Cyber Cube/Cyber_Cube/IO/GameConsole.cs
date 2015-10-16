using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube.IO
{
    public class GameConsole : DrawableGameComponent
    {
        public new Game1 Game
        {
            get {
                return base.Game as Game1;
            }
        }

        private TextBox mTextBox;
        private int mHistoryIndex = 0;

        public readonly List<ConsoleMessage> History = new List<ConsoleMessage>();

        public delegate bool ExecuteCommand( string command );

        public event ExecuteCommand CommandExecuted;

        private SpriteFont mFont;
        private SpriteBatch mSpriteBatch;

        public GameConsole( Game1 game )
            : base( game )
        {
            mTextBox = new TextBox( game );

            //Game.Components.ComponentAdded += ( s, e ) => {
            //    if ( ReferenceEquals( e.GameComponent, this ) )
            //        Game.Components.Add( mTextBox );
            //};
            //
            //Game.Components.ComponentRemoved += ( s, e ) => {
            //    if ( ReferenceEquals( e.GameComponent, this ) )
            //        Game.Components.Remove( mTextBox );
            //};

            //this.EnabledChanged += ( s, e ) => { mTextBox.Enabled = this.Enabled; };
            //this.VisibleChanged += ( s, e ) => { mTextBox.Visible = this.Visible; };
        }

        public void Open()
        {
            this.Enabled = true;
            this.Visible = true;
        }

        public void Close()
        {
            this.Enabled = false;
            this.Visible = false;
            mTextBox.Text = "";
        }

        public override void Initialize()
        {
            mTextBox.Initialize();
            base.Initialize();

            mSpriteBatch = new SpriteBatch( GraphicsDevice );

            this.DrawOrder = 10;
            //mTextBox.DrawOrder = this.DrawOrder + 1;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            mFont = Game.Content.Load<SpriteFont>( "ConsoleFont" );
            mTextBox.Font = mFont;
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );
            InputState input = Game.Input;

            if ( History.Count > 0 )
            {
                if ( input.Keyboard_WasKeyPressed( Keys.Up ) )
                {
                    for ( int i = mHistoryIndex + 1 ; i <= History.Count ; ++i )
                    {
                        if ( History[ History.Count - i ] is ConsoleInputMessage )
                        {
                            mHistoryIndex = i;
                            break;
                        }
                    }

                    if ( mHistoryIndex == 0 )
                        mTextBox.Text = "";
                    else
                    {
                        mTextBox.Text = History[ History.Count - mHistoryIndex ].Message;
                        mTextBox.InputPos = int.MaxValue;
                    }
                }

                if ( input.Keyboard_WasKeyPressed( Keys.Down ) )
                {
                    for ( int i = mHistoryIndex - 1 ; i >= 0 ; --i )
                    {
                        if ( i == 0 || History[ History.Count - i ] is ConsoleInputMessage )
                        {
                            mHistoryIndex = i;
                            break;
                        }
                    }

                    if ( mHistoryIndex == 0 )
                        mTextBox.Text = "";
                    else
                    {
                        mTextBox.Text = History[ History.Count - mHistoryIndex ].Message;
                        mTextBox.InputPos = int.MaxValue;
                    }
                }
            }


            if ( input.Keyboard_WasKeyPressed( Keys.Enter ) && !input.IsShiftDown() )
            {
                RunCommand( mTextBox.Text.TrimEnd( '\n' ) );
                mTextBox.Text = "";
            }
            else if ( mTextBox.Enabled )
            {
                mTextBox.Update( gameTime );
            }
        }

        private void RunCommand( string command )
        {
            History.Add( new ConsoleInputMessage( command ) );
            mHistoryIndex = 0;

            if ( CommandExecuted != null && !this.CommandExecuted( command ) )
                History.Add( new ConsoleErrorMessage( "unrecognized command '" + command + "'" ) );
        }

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );

            mSpriteBatch.Begin();

            Rectangle consoleRect = new Rectangle( 0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 2 );

            mSpriteBatch.DrawRect( consoleRect, new Color( 0, 0, 0, 128 ) );

            var pos = mFont.MeasureString( "> " );

            Rectangle line = consoleRect;
            line.Height = 1;
            line.Y = consoleRect.Bottom - (int) pos.Y;

            mSpriteBatch.DrawRect( line, new Color( 0, 0, 0, 96 ) );

            mTextBox.Position = new Vector2( pos.X, line.Y );

            mSpriteBatch.DrawString( mFont, "> ", new Vector2( 0, line.Y ), Color.White );

            History.Reverse();
            float yOffset = line.Y;
            foreach ( ConsoleMessage msg in History )
            {
                yOffset -= mFont.MeasureString( msg.ToString() ).Y;
                mSpriteBatch.DrawString( mFont, msg.ToString(), new Vector2( 0, yOffset ), msg.TextColor );
            }
            History.Reverse();

            mSpriteBatch.End();

            if ( mTextBox.Visible )
                mTextBox.Draw( gameTime );
        }

    }

    public class ConsoleMessage
    {
        public string Message
        { get; private set; }
        public Color TextColor
        { get; private set; }

        public ConsoleMessage( string message, Color color )
        {
            this.Message = message;
            this.TextColor = color;
        }

        public static implicit operator ConsoleMessage( string text )
        {
            return new ConsoleMessage( text, Color.White );
        }

        public override string ToString()
        {
            return Message;
        }
    }

    public class ConsoleInputMessage : ConsoleMessage
    {

        public ConsoleInputMessage( string message )
            : base( message, Color.White )
        {
        }

        public override string ToString()
        {
            return "> " + Message;
        }

    }

    public class ConsoleErrorMessage : ConsoleMessage
    {

        public ConsoleErrorMessage( string message )
            : base( message, Color.Red )
        {
        }

        public override string ToString()
        {
            return "ERROR: " + Message;
        }

    }
}
