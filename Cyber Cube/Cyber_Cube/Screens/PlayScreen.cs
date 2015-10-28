using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CyberCube.Levels;
using Microsoft.Xna.Framework.Input;

namespace CyberCube.Screens
{
    /// <summary>
    /// Screen which contains a cube which can be played, but not edited.
    /// </summary>
    public class PlayScreen : CubeScreen
    {
        /// <summary>
        /// Hides the base Cube property, exposing the methods of the PlayableCube class.
        /// </summary>
        public new PlayableCube Cube
        {
            get {
                return (PlayableCube) base.Cube;
            }
            set {
                base.Cube = value;
            }
        }

        public Player Player
        {
            get; private set;
        }

        /// <summary>
        /// Creates a new PlayScreen.
        /// </summary>
        /// <param name="game">Game the PlayScreen should be associated with.</param>
        public PlayScreen( CubeGame game )
            : this( game, new PlayableCube( game ) )
        {
        }

        /// <summary>
        /// Creates a new PlayScreen.
        /// </summary>
        /// <param name="game">Game the PlayScreen should be associated with.</param>
        /// <param name="playCube">PlayableCube the screen should display.</param>
        public PlayScreen( CubeGame game, PlayableCube playCube )
            : base( game, playCube )
        {
            Player = new Player( this, Cube, Vector3.UnitZ, Direction.Up );
            Components.Add( Player );
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );
        }

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );
        }
    }
}
