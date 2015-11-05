using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CyberCube.Screens
{
    /// <summary>
    /// Manages a set of screens.
    /// </summary>
    public class ScreenManager : SimpleDrawableCubeGameComponent
    {
        /// <summary>
        /// The screens owned by the screen manager.
        /// </summary>
        private Stack< GameScreen > mScreens;

        /// <summary>
        /// Gets the screen that is currently being updated/drawn.
        /// </summary>
        public GameScreen TopScreen
        {
            get {
                return mScreens.Count > 0 ? mScreens.Peek() : null;
            }
        }

        /// <summary>
        /// Gets the number of screens in the screen manager.
        /// </summary>
        public int Count
        {
            get {
                return mScreens.Count;
            }
        }

        /// <summary>
        /// Creates a new ScreenManager.
        /// </summary>
        /// <param name="game">Game the ScreenManager should be associated with.</param>
        public ScreenManager( CubeGame game )
            : base( game )
        {
            mScreens = new Stack< GameScreen >();
        }

        public override void Initialize()
        {
            base.Initialize();

            foreach ( GameScreen screen in mScreens )
                screen.Initialize();
        }

        /// <summary>
        /// Adds a screen to the top of the screen manager and initializes it.
        /// </summary>
        /// <param name="screen">Screen to add.</param>
        public void PushScreen( GameScreen screen )
        {
            if ( Game.Initialized )
                screen.Initialize();
            screen.ScreenManager = this;
            mScreens.Push( screen );
        }

        /// <summary>
        /// Removes and returns the top screen in the screen manager.
        /// </summary>
        public GameScreen PopScreen()
        {
            GameScreen screen = mScreens.Pop();
            screen.ScreenManager = null;
            return screen;
        }

        /// <summary>
        /// Removes and returns a number of screens from the top of the screen manager. Screens 
        /// are returned in the order they are removed.
        /// </summary>
        /// <param name="n">Number of screens to remove.</param>
        /// <returns></returns>
        public IEnumerable< GameScreen > PopScreens( int n )
        {
            for ( int i = 0; i < n; ++i )
                yield return PopScreen();
        }

        /// <summary>
        ///  Temporary fix for removing screens for the screenmanager
        /// </summary>
        /// <param name="n"></param>
        public void PauseMenuToMainMenu(int n)
        {
            for (int i = 0; i < n; ++i)
            {
                mScreens.Pop();
            }
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            TopScreen?.Update( gameTime );
        }

        public override void Draw( GameTime gameTime )
        {
            TopScreen?.Draw( gameTime );
        }
    }
}
