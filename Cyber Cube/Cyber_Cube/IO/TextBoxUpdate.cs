using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CyberCube.IO
{
    public partial class TextBox
    {

        private Dictionary<Keys, double> mKeyHistory = new Dictionary<Keys, double>();
        private Keys? mRepeatKey;
        private double mLastRepeat;

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            double currMillis = gameTime.TotalGameTime.TotalMilliseconds;

            if ( currMillis - mLastCursorBlink >= CursorBlinkFrequency )
            {
                mCursorVisible = !mCursorVisible;
                mLastCursorBlink = currMillis;
            }

            if ( !Input.CheckFocus( this ) )
                return;

            string oldText = Text;
            int oldInputPos = InputPos;

            Func<Keys, bool> notModKey = k => { return k < Keys.LeftShift || k > Keys.RightAlt; };

            var currentKeys = Input.Keyboard.GetPressedKeys().Where( notModKey );
            var prevKeys = Input.OldKeyboard.GetPressedKeys().Where( notModKey );

            var newKeys = currentKeys.Where( ck => !prevKeys.Any( pk => pk == ck ) ).ToList();
            var heldKeys = currentKeys.Intersect( prevKeys );

            if ( mRepeatKey.HasValue && !heldKeys.Contains( mRepeatKey.Value ) )
                mRepeatKey = null;

            var tmpKey = mRepeatKey;

            foreach ( var pair in mKeyHistory.ToArray() )
            {
                if ( !heldKeys.Contains( pair.Key ) )
                {
                    mKeyHistory.Remove( pair.Key );
                    continue;
                }

                if ( pair.Key != mRepeatKey )
                {
                    mRepeatKey = null;
                    if ( currMillis - pair.Value >= this.RepeatDelay )
                        mRepeatKey = pair.Key;
                }
            }

            if ( mRepeatKey != tmpKey )
                mLastRepeat = currMillis;


            foreach ( Keys key in newKeys )
                mKeyHistory[ key ] = currMillis;

            if ( mRepeatKey.HasValue && currMillis - mLastRepeat >= this.RepeatFrequency )
            {
                newKeys.Add( mRepeatKey.Value );
                mLastRepeat = currMillis;
            }

            foreach ( Keys key in newKeys )
            {
                switch ( key )
                {
                case Keys.Home:
                    mIsSelecting = Input.IsShiftDown();
                    InputPos = 0;
                    break;

                case Keys.End:
                    mIsSelecting = Input.IsShiftDown();
                    InputPos = Text.Length;
                    break;

                case Keys.Left:
                    mIsSelecting = Input.IsShiftDown();

                    if ( Input.IsCtrlDown() )
                        InputPos = SkipLeft();

                    else if ( HasSelection && !mIsSelecting )
                        mSelectionAnchor = mInputPos = SelectionStart;

                    else
                        --InputPos;

                    break;

                case Keys.Right:
                    mIsSelecting = Input.IsShiftDown();

                    if ( Input.IsCtrlDown() )
                        InputPos = SkipRight();

                    else if ( HasSelection && !mIsSelecting )
                        mSelectionAnchor = mInputPos = SelectionEnd;

                    else
                        ++InputPos;

                    break;

                case Keys.Back:
                    if ( Text.Length > 0 )
                    {
                        int pos = HasSelection ? InputPos : InputPos - 1;

                        if ( Input.IsCtrlDown() )
                            pos = SkipLeft();

                        if ( pos < 0 )
                            pos = 0;

                        if ( InputPos > SelectionAnchor )
                            pos = SelectionStart;

                        mIsSelecting = true;
                        SelectionAnchor = Math.Max( InputPos, SelectionEnd );
                        InputPos = Math.Min( pos, SelectionStart );

                        InsertText( "" );
                    }
                    break;

                case Keys.Delete:
                    if ( Text.Length > 0 )
                    {
                        int pos = HasSelection ? InputPos : InputPos + 1;

                        if ( Input.IsCtrlDown() )
                            pos = SkipRight();

                        if ( pos > Text.Length )
                            pos = Text.Length;

                        if ( InputPos < SelectionAnchor )
                            pos = SelectionEnd;

                        mIsSelecting = true;
                        SelectionAnchor = Math.Min( InputPos, SelectionStart );
                        InputPos = Math.Max( pos, SelectionEnd );

                        InsertText( "" );
                    }
                    break;

                case Keys.Space:
                    InsertText( " " );
                    break;

                case Keys.Enter:
                    if ( EnableMultiLine )
                        InsertText( "\n" );
                    break;

                default:
                    char glyph = (char) key;

                    if ( char.IsLetter( glyph ) )
                    {
                        if ( Input.IsCtrlDown() )
                        {
                            switch ( glyph )
                            {
                            case 'A':
                                InputPos = Text.Length;
                                SelectionAnchor = 0;
                                break;

                            case 'X':
                                this.Cut();
                                break;

                            case 'C':
                                this.Copy();
                                break;

                            case 'V':
                                this.Paste();
                                break;
                            }
                        }
                        else
                        {
                            InsertText( !Input.IsShiftDown()
                                        ? char.ToLower( glyph )
                                        : glyph );
                        }
                    }
                    else if ( char.IsDigit( glyph ) )
                    {
                        if ( !Input.IsShiftDown() )
                            InsertText( glyph );
                        else
                            InsertText( ")!@#$%^&*(".ElementAt( glyph - '0' ) );
                    }
                    break;

                case Keys.OemTilde:
                    InsertText( !Input.IsShiftDown() ? "`" : "~" );
                    break;

                case Keys.OemMinus:
                    InsertText( !Input.IsShiftDown() ? "-" : "_" );
                    break;

                case Keys.OemPlus:
                    InsertText( !Input.IsShiftDown() ? "=" : "+" );
                    break;

                case Keys.OemOpenBrackets:
                    InsertText( !Input.IsShiftDown() ? "[" : "{" );
                    break;

                case Keys.OemCloseBrackets:
                    InsertText( !Input.IsShiftDown() ? "]" : "}" );
                    break;

                case Keys.OemPipe:
                    InsertText( !Input.IsShiftDown() ? "\\" : "|" );
                    break;

                case Keys.OemSemicolon:
                    InsertText( !Input.IsShiftDown() ? ";" : ":" );
                    break;

                case Keys.OemQuotes:
                    InsertText( !Input.IsShiftDown() ? "'" : "\"" );
                    break;

                case Keys.OemComma:
                    InsertText( !Input.IsShiftDown() ? "," : "<" );
                    break;

                case Keys.OemPeriod:
                    InsertText( !Input.IsShiftDown() ? "." : ">" );
                    break;

                case Keys.OemQuestion:
                    InsertText( !Input.IsShiftDown() ? "/" : "?" );
                    break;
                }

                mIsSelecting = false;
            }

            if ( oldText != Text || oldInputPos != InputPos )
            {
                mCursorVisible = true;
                mLastCursorBlink = currMillis;
            }
        }

        private int SkipLeft()
        {
            bool foundWhiteSpace = false;
            bool foundWord = false;
            bool foundSymbol = false;
            int i = InputPos;
            while ( i-- > 0 )
            {
                char element = Text.ElementAt( i );

                if ( char.IsWhiteSpace( element ) )
                    foundWhiteSpace = true;

                if ( !char.IsLetterOrDigit( element ) && !char.IsWhiteSpace( element ) )
                    foundSymbol = true;

                if ( char.IsLetterOrDigit( element ) )
                    foundWord = true;

                if ( foundWord && char.IsWhiteSpace( element ) )
                    return i + 1;

                if ( foundSymbol )
                    return foundWord ? i + 1 : i;
            }
            return i;
        }

        private int SkipRight()
        {
            bool foundWhiteSpace = false;
            bool foundWord = false;
            bool foundSymbol = false;
            int i;
            for ( i = InputPos ; i < Text.Length ; ++i )
            {
                char element = Text.ElementAt( i );

                if ( char.IsWhiteSpace( element ) )
                    foundWhiteSpace = true;

                if ( !char.IsLetterOrDigit( element ) && !char.IsWhiteSpace( element ) )
                {
                    if ( foundSymbol )
                        break;
                    foundSymbol = true;
                }

                if ( char.IsLetterOrDigit( element ) )
                    foundWord = true;

                if ( foundWhiteSpace && char.IsLetterOrDigit( element ) )
                    break;

                if ( (foundWord || foundWhiteSpace) && !char.IsLetterOrDigit( element ) && !char.IsWhiteSpace( element ) )
                    break;

                if ( foundSymbol && foundWord )
                    break;
            }
            return i;
        }


#if XBOX
        private static string sClipboard = "";
#endif

        private void Cut()
        {
            if ( HasSelection )
            {
#if WINDOWS
                Thread t = new Thread( () => {
                    System.Windows.Forms.Clipboard.SetText( Selection );
                } );
                t.SetApartmentState( ApartmentState.STA );
                t.Start();
                t.Join();
#elif XBOX
                sClipboard = Selection;
#endif
                InsertText( "" );
            }
        }

        private void Copy()
        {
            if ( HasSelection )
            {
#if WINDOWS
                Thread t = new Thread( () => {
                    System.Windows.Forms.Clipboard.SetText( Selection );
                } );
                t.SetApartmentState( ApartmentState.STA );
                t.Start();
                t.Join();
#elif XBOX
                sClipboard = Selection;
#endif
            }
        }

        private void Paste()
        {
            string clipboard = "";
#if WINDOWS
            Thread t = new Thread( () => {
                if ( System.Windows.Forms.Clipboard.ContainsText() )
                    clipboard = System.Windows.Forms.Clipboard.GetText( System.Windows.Forms.TextDataFormat.UnicodeText );
            } );
            t.SetApartmentState( ApartmentState.STA );
            t.Start();
            t.Join();
#elif XBOX
            clipboard = sClipboard;
#endif
            InsertText( clipboard );
        }

    }
}
