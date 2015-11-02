using CyberCube.IO;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Screens
{
    /// <summary>
    /// Abstract base class for all game screens.
    /// </summary>
    public abstract class GameScreen : DrawableCubeGameComponent
    {
        /// <summary>
        /// The screen manager this screen belongs to.
        /// </summary>
        public ScreenManager ScreenManager
        {
            get; internal set;
        }

        /// <summary>
        /// Collection of GameComponents owned by the screen.
        /// </summary>
        protected readonly GameComponentCollection Components = new GameComponentCollection();
            
        /// <summary>
        /// Creates a new instance of the GameScreen class.
        /// </summary>
        /// <param name="game">Game the GameScreen should be </param>
        public GameScreen( CubeGame game )
            : base( game )
        {
        }

        /// <summary>
        /// Pops the screen from its screen manager, returning to the previous screen.
        /// </summary>
        public void Back()
        {
            ScreenManager.PopScreen();
        }

        public virtual ConsoleMessage RunCommand( string command )
        {
            return GameConsole.MakeDefaultErrorMessage( command );
        }

        public override void Initialize()
        {
            base.Initialize();

            foreach ( IGameComponent gc in Components )
                gc.Initialize();
        }

        public override void Update( GameTime gameTime )
        {
            var comps = from c in Components
                        where (c as IUpdateable)?.Enabled == true
                        let uc = (IUpdateable) c
                        orderby uc.UpdateOrder ascending
                        select uc;
            
            foreach ( IUpdateable ugc in comps )
                ugc.Update( gameTime );
        }

        public override void Draw( GameTime gameTime )
        {
            var comps = from c in Components
                        where (c as IDrawable)?.Visible == true
                        let dc = (IDrawable) c
                        orderby dc.DrawOrder ascending
                        select dc;
            
            foreach ( IDrawable dgc in comps )
                dgc.Draw( gameTime );
        }

    }
}
