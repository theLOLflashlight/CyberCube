using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CyberCube.MenuFiles;
using System.IO;

namespace CyberCube.Screens
{
    public class MenuScreen : GameScreen
    {

        private Menu mMenu;

        public MenuScreen( CubeGame game )
            : base( game )
        {
            mMenu = new Menu( Game, "v0.1 alpha" );
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            mMenu.Update();

            switch( mMenu.CurrentMenuState )
            {
            case GameState.PlayingGame:
                ScreenManager.PushScreen( new PlayScreen( Game ) );
                break;
            case GameState.LoadGame:

                Stream saveStream = StorageManager.Instance.OpenWriteFile( "CyberCube.sav" );
                // Saving functionality here.

                Stream loadStream = StorageManager.Instance.OpenReadFile( "CyberCube.sav" );
                // Loading functionality here.

                StorageManager.Instance.Finish();

                break;
            case GameState.LevelEditor:
                ScreenManager.PushScreen( new EditScreen( Game ) );
                break;
            case GameState.ExitGame:
                Game.Exit();
                break;
            }
            mMenu.CurrentMenuState = GameState.MainMenu;
        }

        public override void Draw( GameTime gameTime )
        {
            mSpriteBatch.Begin();

            GraphicsDevice.Clear( Color.White );
            mMenu.Draw( mSpriteBatch );

            mSpriteBatch.End();
        }
    }
}
