using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CyberCube.Levels;
using Microsoft.Xna.Framework.Graphics;
using CyberCube.Physics;
using FarseerPhysics.Dynamics;
using CyberCube.Screens.Brushes;
using CyberCube.IO;

namespace CyberCube.Screens
{
    /// <summary>
    /// Screen which contains a cube which can be edited, but not played.
    /// </summary>
    public class EditScreen : CubeScreen
    {

        public static Vector2 SnapVector( Vector2 vec, float snapSize )
        {
            vec /= snapSize;
            vec = vec.Rounded();
            vec *= snapSize;
            return vec;
        }

        private IEditBrush mLeftBrush;
        private IEditBrush mRightBrush;
        private IEditBrush mMiddleBrush;

        public IEditBrush LeftBrush
        {
            get {
                return mLeftBrush;
            }
            set {
                mLeftBrush?.Cancel();
                mLeftBrush = value;
            }
        }

        public IEditBrush RightBrush
        {
            get {
                return mRightBrush;
            }
            set {
                mRightBrush?.Cancel();
                mRightBrush = value;
            }
        }

        public IEditBrush MiddleBrush
        {
            get {
                return mMiddleBrush;
            }
            set {
                mMiddleBrush?.Cancel();
                mMiddleBrush = value;
            }
        }

        private readonly EditScreenProperties ScreenProperties;

        #region Boilerplate
        /// <summary>
        /// Hides the base Cube property, exposing the methods of the EditableCube class.
        /// </summary>
        public new EditableCube Cube
        {
            get {
                return (EditableCube) base.Cube;
            }
            protected set {
                base.Cube = value;
            }
        }

        /// <summary>
        /// Creates a new EditScreen.
        /// </summary>
        /// <param name="game">Game the EditScreen should be associated with.</param>
        public EditScreen( CubeGame game )
            : this( game, new EditableCube( game ) )
        {
        }

        /// <summary>
        /// Creates a new EditScreen.
        /// </summary>
        /// <param name="game">Game the EditScreen should be associated with.</param>
        /// <param name="editCube">EditableCube the screen should display.</param>
        public EditScreen( CubeGame game, EditableCube editCube )
            : base( game, editCube )
        {
            ScreenProperties = new EditScreenProperties( this );
        }
        #endregion

        public class EditScreenProperties : RuntimeProperties
        {
            public IEditBrush Brush
            {
                get {
                    return mScreen.LeftBrush;
                }
                set {
                    mScreen.LeftBrush = value;
                }
            }

            private EditScreen mScreen;

            public EditScreenProperties( EditScreen screen )
            {
                mScreen = screen;
            }

            protected override object Parse( string value, Type type )
            {
                object obj = base.Parse( value, type );

                if ( obj == null )
                {
                    if ( type == typeof( IEditBrush ) )
                        obj = EditBrush_Parse( value );
                }

                return obj;
            }

            private IEditBrush EditBrush_Parse( string value )
            {
                switch ( value.ToLower() )
                {
                case "hand":
                    return new HandBrush( mScreen.Game );

                case "box":
                    return new BoxBrush( mScreen.Game );

                case "type":
                    return new BodyTypeBrush();

                case "corner":
                    return new CornerBrush( mScreen.Game );

                case "qpipe":
                case "quarterpipe":
                    return new QuarterpipeBrush( mScreen.Game );
                }
                throw new FormatException();
            }
        }

        public override ConsoleMessage RunCommand( string command )
        {
            switch ( command.ToLower() )
            {
            default:
                try {
                    var ret = ScreenProperties.Evaluate( command );

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
            }

            return base.RunCommand( command );
        }


        public override void Initialize()
        {
            base.Initialize();
            LeftBrush = new HandBrush( Game );
        }

        private void TestLevel()
        {
            PlayableCube playCube = Cube.GeneratePlayableCube();
            ScreenManager.PushScreen( new PlayScreen( Game, playCube ) );
        }

        public override void Update( GameTime gameTime )
        {
            var input = Game.Input;

            if ( !input.HasFocus && input.GetAction( Action.ToggleCubeMode ) )
                TestLevel();

            base.Update( gameTime );
        }

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );
        }
    }
}
