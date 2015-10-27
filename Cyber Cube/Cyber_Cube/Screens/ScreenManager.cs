using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CyberCube.Screens
{
    public class ScreenManager : SimpleDrawableCubeGameComponent
    {
        private Stack< GameScreen > mScreens;

        public GameScreen TopScreen
        {
            get {
                return mScreens.Count > 0 ? mScreens.Peek() : null;
            }
        }

        public int Count
        {
            get {
                return mScreens.Count;
            }
        }

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

        public void PushScreen( GameScreen screen )
        {
            if ( Game.Initialized )
                screen.Initialize();
            screen.ScreenManager = this;
            mScreens.Push( screen );
        }

        public GameScreen PopScreen()
        {
            GameScreen screen = mScreens.Pop();
            screen.ScreenManager = null;
            return screen;
        }

        public IEnumerable< GameScreen > PopScreens( int n )
        {
            for ( int i = 0; i < n; ++i )
                yield return PopScreen();
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
