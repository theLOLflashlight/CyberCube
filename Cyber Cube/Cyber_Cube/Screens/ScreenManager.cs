using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;

namespace CyberCube.Screens
{
    /// <summary>
    /// Manages a set of screens.
    /// </summary>
    public class ScreenManager : SimpleDrawableCubeGameComponent
    {
        /// <summary>
        /// A stack of screens owned by the screen manager.
        /// </summary>
        private Stack< GameScreen > mScreens;

        /// <summary>
        /// A list of screens which will be pushed (in order) onto the stack of 
        /// screens owned by the screen manager on the next call to Update. A 
        /// null element in this list will pop the current top screen instead 
        /// (still in order).
        /// </summary>
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
        /// Gets the screens currently owned by the screen manager in the 
        /// reverse order in which they were added (top screen first).
        /// </summary>
        public IEnumerable< GameScreen > Screens
        {
            get {
                foreach ( GameScreen screen in mScreens.Reverse() )
                    yield return screen;
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

        /// <summary>
        /// Immediately pauses the current top screen, pushes a different 
        /// screen on top of it and resumes the new top screen.
        /// </summary>
        /// <param name="screen">Screen to add.</param>
        /// <param name="gameTime">Time at which the screen was pushed.</param>
        private void PushScreen( GameScreen screen, GameTime gameTime )
        {
            TopScreen?.Pause( gameTime );

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

        /// <summary>
        /// Immediately destroys the current top screen, pops it off the top 
        /// and resumes the new top screen.
        /// </summary>
        /// <param name="gameTime">Time at which the screen was popped.</param>
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
        /// Removes all screens from the screen manager on the next update. 
        /// Screens added before this call will be added before they are 
        /// removed and screens added after this call will not be removed.
        /// </summary>
        public void ClearScreens()
        {
            int count = Count;
            foreach ( var screen in mPendingScreens )
                count += screen != null ? 1 : -1;

            for ( int i = 0; i < count; ++i )
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

            var pendingScreens = mPendingScreens.ToList();
            mPendingScreens.Clear();

            foreach ( GameScreen screen in pendingScreens )
                if ( screen == null )
                    PopScreen( gameTime );
                else
                    PushScreen( screen, gameTime );

            TopScreen?.Update( gameTime );
        }

        public override void Draw( GameTime gameTime )
        {
            TopScreen?.Draw( gameTime );
        }
    }
}
