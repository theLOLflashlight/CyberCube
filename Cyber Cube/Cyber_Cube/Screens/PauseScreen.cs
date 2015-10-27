using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CyberCube.MenuFiles;

namespace CyberCube.Screens
{
    public class PauseScreen : GameScreen
    {
        private PauseMenu mPauseMenu;

        public PauseScreen( CubeGame game, PauseMenu pauseMenu )
            : base( game )
        {
            mPauseMenu = pauseMenu;
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            mPauseMenu.Update();

            switch( mPauseMenu.Status )
            {
            case 1:
                Back();
                break;
            case 4:
                ScreenManager.PopScreens( 2 );
                break;
            default:
                break;
            }
        }

        public override void Draw( GameTime gameTime )
        {
            mSpriteBatch.Begin();

            mPauseMenu.Draw( mSpriteBatch );

            mSpriteBatch.End();
        }
    }
}
