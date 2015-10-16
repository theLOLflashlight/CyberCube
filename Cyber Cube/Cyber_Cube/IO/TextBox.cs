using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Cyber_Cube.IO
{
    public partial class TextBox : DrawableGameComponent
    {
        public new Game1 Game
        {
            get {
                return base.Game as Game1;
            }
        }

        private SpriteBatch mSpriteBatch;


        public SpriteFont Font { get; set; }

        private string mText = "";

        public string Text
        {
            get {
                return mText;
            }
            set {
                mText = EnableMultiLine ? value : value.Replace( "\n", "" );
                InputPos = InputPos;
            }
        }

        private bool mIsSelecting = false;

        private int mInputPos = 0;

        public int InputPos
        {
            get {
                return mInputPos;
            }
            set {
                mInputPos = Math.Min( Math.Max( value, 0 ), Text.Length );

                if ( !mIsSelecting )
                    mSelectionAnchor = mInputPos;
            }
        }

        private int mSelectionAnchor = 0;

        public int SelectionAnchor
        {
            get {
                return mSelectionAnchor;
            }
            set {
                mSelectionAnchor = Math.Min( Math.Max( value, 0 ), Text.Length );
            }
        }

        public bool HasSelection
        {
            get {
                return mInputPos != mSelectionAnchor;
            }
        }

        public int SelectionStart
        {
            get {
                return mInputPos < mSelectionAnchor ? mInputPos : mSelectionAnchor;
            }
        }

        public int SelectionEnd
        {
            get {
                return mInputPos > mSelectionAnchor ? mInputPos : mSelectionAnchor;
            }
        }

        public string Selection
        {
            get {
                int start = SelectionStart;
                return Text.Substring( start, SelectionEnd - start );
            }
        }

        private void InsertText( string text )
        {
            text = EnableMultiLine ? text : text.Replace( "\n", "" );

            int start = SelectionStart;
            int end = SelectionEnd;

            mText = Text.Remove( start, end - start );
            mText = Text.Insert( start, text );

            mIsSelecting = false;
            InputPos = start + text.Length;
        }

        private void InsertText( char glyph )
        {
            InsertText( glyph.ToString() );
        }

        public Vector2 Position { get; set; }

        public int RepeatDelay = 500;
        public int RepeatFrequency = 30;
        public int CursorBlinkFrequency = 700;
        public bool EnableMultiLine = false;

        public TextBox( Game1 game )
            : base( game )
        {
        }

        public TextBox( Game1 game, Vector2 position )
            : this( game )
        {
            Position = position;
        }

        public override void Initialize()
        {
            base.Initialize();

            mSpriteBatch = new SpriteBatch( GraphicsDevice );
        }

        private double mLastCursorBlink;
        private bool mCursorVisible = true;

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );

            mSpriteBatch.Begin();

            if ( HasSelection )
            {
                Vector2 selectionDimen = Font.MeasureString( Selection );
                float selectionOffset = Font.MeasureString( Text.Substring( 0, SelectionStart ) ).X;

                Rectangle box;
                box.X      = (int) (Position.X + selectionOffset);
                box.Y      = (int) Position.Y;
                box.Width  = (int) selectionDimen.X;
                box.Height = (int) selectionDimen.Y;

                Color color = Color.Blue;
                color.A = 127;

                mSpriteBatch.DrawRect( box, color );
            }

            if ( mCursorVisible )
            {
                int newlines = 0;
                int lineStart = 0;

                if ( Text.Length > 0 )
                    for ( int i = InputPos - 1; i >= 0; --i )
                        if ( Text.ElementAt( i ) == '\n' )
                            if ( newlines++ == 0 )
                                lineStart = i + 1;

                float caretOffsetX = Font.MeasureString( Text.Substring( lineStart, InputPos - lineStart ) ).X;
                float caretOffsetY = newlines * Font.MeasureString( " " ).Y;

                Rectangle caret;
                caret.X      = (int) (Position.X + caretOffsetX);
                caret.Y      = (int) (Position.Y + caretOffsetY);
                caret.Width  = 1;
                caret.Height = (int) Font.MeasureString( " " ).Y;
            
                mSpriteBatch.DrawRect( caret, Color.White );
            }

            mSpriteBatch.DrawString( Font, Text, Position, Color.White );
            mSpriteBatch.End();
        }

        

    }
}
