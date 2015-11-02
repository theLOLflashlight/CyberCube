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
    /// Abstract base class for all screens which involve a cube on the screen.
    /// </summary>
    public abstract class CubeScreen : GameScreen
    {
        /// <summary>
        /// The cube present on this screen.
        /// </summary>
        public Cube Cube
        {
            get; protected set;
        }

        /// <summary>
        /// The camera used to render the cube.
        /// </summary>
        public Camera Camera
        {
            get; protected set;
        }

        public GameHud Hud
        {
            get; protected set;
        }

        /// <summary>
        /// Creates a new CubeScreen.
        /// </summary>
        /// <param name="game">Game the CubeScreen should be associated with.</param>
        /// <param name="cube">Cube the screen should display.</param>
        public CubeScreen( CubeGame game, Cube cube )
            : base( game )
        {
            Cube = cube;
            Cube.Screen = this;
            Hud = new GameHud( this );
            Camera = new Camera( Game );

            Cube.UpdateOrder = 2;

            Hud.UpdateOrder = 2;
            Hud.DrawOrder = 2;

            Camera.UpdateOrder = 2;

            Components.Add( Hud );
            Components.Add( Cube );
            Components.Add( Camera );
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            if ( !Game.Input.HasFocus
                 && Game.Input.Keyboard_WasKeyPressed( Keys.Escape ) )
                Back();
        }

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );
        }
    }
}
