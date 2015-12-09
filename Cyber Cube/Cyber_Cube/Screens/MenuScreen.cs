using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using System.IO;
using CyberCube.Levels;

namespace CyberCube.Screens
{
    public class MenuScreen : GameScreen
    {
        private static Texture2D sTitleCard;
        private static Texture2D sNewGame;
        private static Texture2D sLoadGame;
        //private static Texture2D sLevelEdit;
        private static Texture2D sControls;
        private static Texture2D sExit;
        private static SpriteFont sVersionFont;

        private string verString;

        private static Song mSong;

        enum Highlight
        {
            NewGame,
            LoadGame,
            //LevelEditor,
            Controls,
            Credits,
            Exit,
        }

        private Highlight currentHighlight;

        private Color cSelected;
        private Color cNewGame, cLoadGame, cControls, cExit;
        //private Color cLevelEdit;
        private Boolean isFading;

        private GamePadState OldPadState;
        private KeyboardState OldKeyState;

        public static void LoadContent(ContentManager content)
        {
            sTitleCard = content.Load<Texture2D>("NavigationItems\\logo");

            sNewGame = content.Load<Texture2D>("NavigationItems\\button_NewGame");
            sLoadGame = content.Load<Texture2D>("NavigationItems\\button_Continue");
            //sLevelEdit = content.Load<Texture2D>("NavigationItems\\menuLevelEditor");
            sControls = content.Load<Texture2D>("NavigationItems\\button_Controls");
            sExit = content.Load<Texture2D>("NavigationItems\\button_Exit");

            sVersionFont = content.Load<SpriteFont>("ConsoleFont");

            mSong = content.Load<Song>("Audio\\GameplayTrack");

            MediaPlayer.Volume = 0.2f;
            MediaPlayer.Play(mSong);
            MediaPlayer.IsRepeating = true;
        }

        public MenuScreen( CubeGame game )
            : base( game )
        {
            verString = "v1.0";

            currentHighlight = Highlight.NewGame;

            cSelected = new Color(0, 240, 255, 255);
            isFading = true;
            cNewGame = Color.White;
            cLoadGame = Color.White;
            //cLevelEdit = Color.White;
            cControls = Color.White;
            cExit = Color.White;
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            // Fade effect for highlighed menu button
            if (isFading)
            {
                cSelected.A -= 3;
                if (cSelected.A == 0) isFading = false;
            }
            else
            {
                cSelected.A += 3;
                if (cSelected.A == 255) isFading = true;
            }

            GamePadState NewPadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState NewKeyState = Keyboard.GetState();

            // Change highlighted button on "up"
            if ((NewKeyState.IsKeyDown(Keys.Up) && OldKeyState.IsKeyUp(Keys.Up))
                || (NewPadState.IsButtonDown(Buttons.DPadUp) && OldPadState.IsButtonUp(Buttons.DPadUp)))
            {
                switch (currentHighlight)
                {
                    case Highlight.NewGame:
                        break;
                    case Highlight.LoadGame:
                        currentHighlight = Highlight.NewGame;
                        break;
                    //case Highlight.LevelEditor:
                    //    currentHighlight = Highlight.LoadGame;
                    //    break;
                    case Highlight.Controls:
                        //currentHighlight = Highlight.LevelEditor;
                        currentHighlight = Highlight.LoadGame;
                        break;
                    case Highlight.Exit:
                        currentHighlight = Highlight.Controls;
                        break;
                    default:
                        break;
                }
            }

            // Level editor is not accessible by normal means:
            // L + E on keyboard, or left and right bumper on the gamepad
            if ((NewKeyState.IsKeyDown(Keys.L) && NewKeyState.IsKeyDown(Keys.E)) || (NewPadState.IsButtonDown(Buttons.LeftShoulder) && NewPadState.IsButtonDown(Buttons.RightShoulder)))
            {
                ScreenManager.PushScreen(new EditScreen(Game));
            }

            // Change highlighted button on "down"
            if ((NewKeyState.IsKeyDown(Keys.Down) && OldKeyState.IsKeyUp(Keys.Down))
                || (NewPadState.IsButtonDown(Buttons.DPadDown) && OldPadState.IsButtonUp(Buttons.DPadDown)))
            {
                switch (currentHighlight)
                {
                    case Highlight.NewGame:
                        currentHighlight = Highlight.LoadGame;
                        break;
                    case Highlight.LoadGame:
                        //currentHighlight = Highlight.LevelEditor;
                        currentHighlight = Highlight.Controls;
                        break;
                    //case Highlight.LevelEditor:
                    //    currentHighlight = Highlight.Controls;
                    //    break;
                    case Highlight.Controls:
                        currentHighlight = Highlight.Exit;
                        break;
                    case Highlight.Exit:
                        break;
                    default:
                        break;
                }
            }

            if ((NewKeyState.IsKeyDown(Keys.Enter) && OldKeyState.IsKeyUp(Keys.Enter))
                || (NewPadState.IsButtonDown(Buttons.A) && OldPadState.IsButtonUp(Buttons.A)))
            {
                switch (currentHighlight)
                {
                    case Highlight.NewGame:
                        PlayableCube playCube = new PlayableCube( Game );
                        playCube.Load("_level0");
                        //playCube.Load( "enemy" );
                        PlayScreen playScreen = new PlayScreen( Game, playCube );
                        ScreenManager.PushScreen( playScreen );
                        break;
                    case Highlight.LoadGame:
                        Stream saveStream = StorageManager.Instance.OpenWriteFile("CyberCube.sav");
                        // Saving functionality here.

                        Stream loadStream = StorageManager.Instance.OpenReadFile("CyberCube.sav");
                        // Loading functionality here.

                        StorageManager.Instance.Finish();

                        break;
                    //case Highlight.LevelEditor:
                    //    ScreenManager.PushScreen(new EditScreen(Game));
                    //    break;
                    case Highlight.Controls:

                        break;
                    case Highlight.Exit:
                        Game.Exit();
                        break;
                    default:
                        break;
                }
            }

            OldPadState = NewPadState;
            OldKeyState = NewKeyState;
        }

        public override void Draw( GameTime gameTime )
        {
            Vector2 buttonPos = new Vector2(GraphicsDevice.Viewport.Width / 2 - (sNewGame.Width / 2), 250);

            mSpriteBatch.Begin();

            GraphicsDevice.Clear( Color.White );

            mSpriteBatch.Draw(sTitleCard, new Vector2(GraphicsDevice.Viewport.Width / 2 - (sTitleCard.Width / 2), 70), Color.White);

            switch (currentHighlight)
            {
                case Highlight.NewGame:
                    cNewGame = cSelected;
                    cLoadGame = Color.White;
                    //cLevelEdit = Color.White;
                    cControls = Color.White;
                    cExit = Color.White;
                    break;
                case Highlight.LoadGame:
                    cNewGame = Color.White;
                    cLoadGame = cSelected;
                    //cLevelEdit = Color.White;
                    cControls = Color.White;
                    cExit = Color.White;
                    break;
                //case Highlight.LevelEditor:
                //    cNewGame = Color.White;
                //    cLoadGame = Color.White;
                //    cLevelEdit = cSelected;
                //    cControls = Color.White;
                //    cExit = Color.White;
                //    break;
                case Highlight.Controls:
                    cNewGame = Color.White;
                    cLoadGame = Color.White;
                    //cLevelEdit = Color.White;
                    cControls = cSelected;
                    cExit = Color.White;
                    break;
                case Highlight.Exit:
                    cNewGame = Color.White;
                    cLoadGame = Color.White;
                    //cLevelEdit = Color.White;
                    cControls = Color.White;
                    cExit = cSelected;
                    break;
                default:
                    break;
            }

            mSpriteBatch.Draw(sNewGame, buttonPos, cNewGame);
            buttonPos.Y += 60;
            mSpriteBatch.Draw(sLoadGame, buttonPos, cLoadGame);
            buttonPos.Y += 60;
            //mSpriteBatch.Draw(sLevelEdit, buttonPos, cLevelEdit);
            //buttonPos.Y += 45;
            mSpriteBatch.Draw(sControls, buttonPos, cControls);
            buttonPos.Y += 60;
            mSpriteBatch.Draw(sExit, buttonPos, cExit);

            mSpriteBatch.DrawString(sVersionFont, verString, new Vector2(10, GraphicsDevice.Viewport.Height - 20), Color.Black);

            mSpriteBatch.End();
        }
    }
}
