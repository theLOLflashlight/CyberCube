using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CyberCube.Levels;

namespace CyberCube.Screens
{
    /// <summary>
    /// Screen which contains a cube which can be edited, but not played.
    /// </summary>
    public class EditScreen : CubeScreen
    {
        /// <summary>
        /// Hides the base Cube property, exposing the methods of the EditableCube class.
        /// </summary>
        public new EditableCube Cube
        {
            get {
                return (EditableCube) base.Cube;
            }
            set {
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
