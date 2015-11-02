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
        }
        #endregion


        public override ConsoleMessage RunCommand( string command )
        {
            switch ( command.ToLower() )
            {
            case "set brush hand":
                LeftBrush = new HandBrush( Game );
                return null;

            case "set brush rectangle":
                Texture2D pixel = new Texture2D( GraphicsDevice, 1, 1 );
                pixel.SetData( new[] { Color.White } );

                LeftBrush = new RectangleBrush( pixel );
                return null;

            case "set brush bodytype":
                LeftBrush = new BodyTypeBrush();
                return null;
            }

            return base.RunCommand( command );
        }


        public override void Initialize()
        {
            base.Initialize();
            Texture2D pixel = new Texture2D( GraphicsDevice, 1, 1 );
            pixel.SetData( new[] { Color.White } );

            LeftBrush = new HandBrush( Game );
        }

        public override void Update( GameTime gameTime )
        {
            var input = Game.Input;

            if ( !input.HasFocus && input.GetAction( Action.ToggleCubeMode ) )
                TestLevel();

            base.Update( gameTime );
        }

        private void TestLevel()
        {
            PlayableCube playCube = Cube.GeneratePlayableCube();
            ScreenManager.PushScreen( new PlayScreen( Game, playCube ) );
        }

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );
        }
    }
}
