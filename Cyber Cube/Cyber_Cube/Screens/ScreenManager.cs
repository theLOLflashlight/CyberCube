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

        private List< GameScreen > mPendingScreens;

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
            mPendingScreens = new List< GameScreen >();
        }

        /// <summary>
        /// Adds a screen to the top of the screen manager on the next call to update.
        /// </summary>
        /// <param name="screen">Screen to add.</param>
        public void PushScreen( GameScreen screen )
        {
            if ( screen == null )
                throw new ArgumentNullException( nameof( screen ) );

            mPendingScreens.Add( screen );
        }

        private void PushScreen( GameScreen screen, GameTime gameTime )
        {
            TopScreen?.Pause( gameTime );

            if ( Game.Initialized )
                screen.Initialize();

            screen.ScreenManager = this;
            mScreens.Push( screen );

            screen.Resume( gameTime );
        }

        /// <summary>
        /// Removes one screen from the top of the screen manager on the next update.
        /// This is effect is cumulative.
        /// </summary>
        public void PopScreen()
        {
            mPendingScreens.Add( null );
        }

        private void PopScreen( GameTime gameTime )
        {
            TopScreen.Destroy( gameTime );
            mScreens.Pop().ScreenManager = null;

            TopScreen?.Resume( gameTime );
        }

        /// <summary>
        /// Removes a number of screens from the top of the screen manager on the next update.
        /// </summary>
        /// <param name="n">Number of screens to remove.</param>
        public void PopScreens( int n )
        {
            if ( n < 1 )
                throw new ArgumentOutOfRangeException( nameof( n ), "Value must be greater than 0." );

            while ( n-- > 0 )
                PopScreen();
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

            foreach ( GameScreen screen in mPendingScreens )
            {
                if ( screen == null )
                    PopScreen( gameTime );
                else
                    PushScreen( screen, gameTime );
            }

            mPendingScreens.Clear();

            TopScreen?.Update( gameTime );
        }

        public override void Draw( GameTime gameTime )
        {
            TopScreen?.Draw( gameTime );
        }
    }
}
