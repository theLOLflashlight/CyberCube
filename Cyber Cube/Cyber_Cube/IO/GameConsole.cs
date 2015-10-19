using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Cyber_Cube.IO
{
    public class GameConsole : IODrawableGameComponent
    {
        public const string DEFAULT_INPUT_PROMPT = "> ";

        public delegate bool ExecuteCommand( string command );

        public event ExecuteCommand CommandExecuted;

        public ReadOnlyCollection< ConsoleMessage > History
        {
            get {
                return mHistory.AsReadOnly();
            }
        }

        public int Height
        {
            get {
                return mHeight;
            }
            set {
                mHeight = value > 0 ? value : 0;
            }
        }

        public string InputPrompt { get; set; }

        public bool Opened
        {
            get {
                return this.Enabled && this.Visible;
            }
        }

        public bool Closed
        {
            get {
                return !this.Enabled && !this.Visible;
            }
        }

        private SpriteFont mFont;

        private TextBox mTextBox;
        private int mHeight;
        private int mHistoryIndex = 0;
        private List<ConsoleMessage> mHistory = new List<ConsoleMessage>();

        public GameConsole( Game game, IInputProvider inputProvider )
            : base( game, inputProvider )
        {
            mTextBox = new TextBox( game, inputProvider );
            InputPrompt = DEFAULT_INPUT_PROMPT;
        }

        public void Open()
        {
            this.Enabled = true;
            this.Visible = true;
            Input.Focus = mTextBox;
        }

        public void Close()
        {
            this.Enabled = false;
            this.Visible = false;
            mTextBox.Text = "";
            Input.Focus = null;
        }

        public override void Initialize()
        {
            mTextBox.Initialize();
            base.Initialize();

            Height = GraphicsDevice.Viewport.Height / 2;
            this.DrawOrder = 10;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            mFont = Game.Content.Load<SpriteFont>( "ConsoleFont" );
            mTextBox.Font = mFont;
        }

        public void AddMessage( ConsoleMessage item )
        {
            mHistory.Add( item );
        }

        public void ClearHistory()
        {
            mHistoryIndex = 0;
            mHistory.Clear();
        }

        private void RunCommand( string command )
        {
            mHistory.Add( new ConsoleInputMessage( command, InputPrompt ) );
            mHistoryIndex = 0;

            if ( CommandExecuted != null && !this.CommandExecuted( command ) )
                mHistory.Add( new ConsoleErrorMessage( "unrecognized command '" + command + "'" ) );
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            if ( History.Count > 0 )
            {
                if ( Input.Keyboard_WasKeyPressed( Keys.Up ) )
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
                    {
                        mTextBox.Text = "";
                    }
                    else
                    {
                        mTextBox.Text = History[ History.Count - mHistoryIndex ].Message;
                        mTextBox.InputPos = int.MaxValue;
                    }
                }

                if ( Input.Keyboard_WasKeyPressed( Keys.Down ) )
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
                    {
                        mTextBox.Text = "";
                    }
                    else
                    {
                        mTextBox.Text = History[ History.Count - mHistoryIndex ].Message;
                        mTextBox.InputPos = int.MaxValue;
                    }
                }
            }


            if ( Input.Keyboard_WasKeyPressed( Keys.Enter ) && !Input.IsShiftDown() )
            {
                RunCommand( mTextBox.Text.TrimEnd( '\n' ) );
                mTextBox.Text = "";
            }
            // We need to prevent the textbox from receiving the enter key when 
            // it was pressed to run a command.
            else if ( mTextBox.Enabled )
            {
                mTextBox.Update( gameTime );
            }
        }

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );

            mSpriteBatch.Begin();

            Rectangle consoleRect = new Rectangle( 0, 0, GraphicsDevice.Viewport.Width, Height );
            Rectangle line = consoleRect;
            line.Height = 1;

            Vector2 pos = mFont.MeasureString( InputPrompt );
            line.Y = consoleRect.Bottom - (int) pos.Y;

            mSpriteBatch.DrawRect( consoleRect, new Color( 0, 0, 0, 128 ) );
            mSpriteBatch.DrawRect( line, new Color( 0, 0, 0, 96 ) );
            mSpriteBatch.DrawString( mFont, InputPrompt, new Vector2( 0, line.Y ), Color.White );

            float yOffset = line.Y;
            mHistory.Reverse();
            foreach ( ConsoleMessage msg in mHistory )
            {
                yOffset -= mFont.MeasureString( msg.ToString() ).Y;
                mSpriteBatch.DrawString( mFont, msg.ToString(), new Vector2( 0, yOffset ), msg.TextColor );
            }
            mHistory.Reverse();

            mSpriteBatch.End();

            mTextBox.Position = new Vector2( pos.X, line.Y );

            if ( mTextBox.Visible )
                mTextBox.Draw( gameTime );
        }

    }

    public class ConsoleMessage
    {
        public string Message { get; private set; }
        public Color TextColor { get; private set; }

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
        private string mInputPrompt;

        public ConsoleInputMessage( string message, string inputPrompt )
            : base( message, Color.White )
        {
            mInputPrompt = inputPrompt;
        }

        public override string ToString()
        {
            return mInputPrompt + Message;
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
