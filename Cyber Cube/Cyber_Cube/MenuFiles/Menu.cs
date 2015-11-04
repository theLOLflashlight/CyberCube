using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//using CyberCube.IO;

namespace CyberCube.MenuFiles
{
    public class Menu
    {
        private static Texture2D sTitleCard;
        private static Texture2D sNewGame;
        private static Texture2D sLoadGame;
        private static Texture2D sLevelEdit;
        private static Texture2D sControls;
        private static Texture2D sExit;
        private static SpriteFont sString;

        public static void LoadContent( ContentManager content )
        {
            sTitleCard = content.Load<Texture2D>("MenuItems\\TitleCard");

            sNewGame = content.Load<Texture2D>("MenuItems\\menuNewGame");
            sLoadGame = content.Load<Texture2D>("MenuItems\\menuLoadGame");
            sLevelEdit = content.Load<Texture2D>("MenuItems\\menuLevelEditor");
            sControls = content.Load<Texture2D>("MenuItems\\menuControls");
            sExit = content.Load<Texture2D>("MenuItems\\menuExit");

            sString = content.Load<SpriteFont>("ConsoleFont");
        }

        enum Highlight
        {
            NewGame,
            LoadGame,
            LevelEditor,
            Controls,
            Credits,
            Exit,
        }

        public GameState CurrentMenuState { get; set; }
        private Highlight currentHighlight;

        private float winWidth;
        private float winHeight;

        private Texture2D titleCard;
        private Texture2D bNewGame;
        private Texture2D bLoadGame;
        private Texture2D bLevelEdit;
        private Texture2D bControls;
        private Texture2D bExit;
        private Texture2D bCredits;

        private SpriteFont vString;
        private string verString;

        private Color selected;
        private Color cNewGame, cLoadGame, cLevelEdit, cControls, cExit;
        private Boolean fade;

        private GamePadState OldPadState;
        private KeyboardState OldKeyState;

        public Menu(Game game, string s)
        {
            titleCard = sTitleCard;

            bNewGame = sNewGame;
            bLoadGame = sLoadGame;
            bLevelEdit = sLevelEdit;
            bControls = sControls;
            bExit = sExit;

            vString = sString;
            verString = s;

            winWidth = game.GraphicsDevice.Viewport.Width;
            winHeight = game.GraphicsDevice.Viewport.Height;

            CurrentMenuState = GameState.MainMenu;
            currentHighlight = Highlight.NewGame;

            selected = new Color(255, 255, 255, 255);
            fade = true;
            cNewGame = Color.White;
            cLoadGame = Color.White;
            cLevelEdit = Color.White;
            cControls = Color.White;
            cExit = Color.White;
        }

        public void Update()
        {
            if (fade)
            {
                selected.A -= 3;
                if (selected.A == 0) fade = false;
            }
            else
            {
                selected.A += 3;
                if (selected.A == 255) fade = true;
            }

            GamePadState NewPadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState NewKeyState = Keyboard.GetState();

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
                    case Highlight.LevelEditor:
                        currentHighlight = Highlight.LoadGame;
                        break;
                    case Highlight.Controls:
                        currentHighlight = Highlight.LevelEditor;
                        break;
                    case Highlight.Exit:
                        currentHighlight = Highlight.Controls;
                        break;
                    default:
                        break;
                }
            }

            if ((NewKeyState.IsKeyDown(Keys.Down) && OldKeyState.IsKeyUp(Keys.Down))
                || (NewPadState.IsButtonDown(Buttons.DPadDown) && OldPadState.IsButtonUp(Buttons.DPadDown)))
            {
                switch (currentHighlight)
                {
                    case Highlight.NewGame:
                        currentHighlight = Highlight.LoadGame;
                        break;
                    case Highlight.LoadGame:
                        currentHighlight = Highlight.LevelEditor;
                        break;
                    case Highlight.LevelEditor:
                        currentHighlight = Highlight.Controls;
                        break;
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
                        CurrentMenuState = GameState.PlayingGame;
                        break;
                    case Highlight.LoadGame:
                        CurrentMenuState = GameState.LoadGame;
                        break;
                    case Highlight.LevelEditor:
                        CurrentMenuState = GameState.LevelEditor;
                        break;
                    case Highlight.Controls:

                        break;
                    case Highlight.Exit:
                        CurrentMenuState = GameState.ExitGame;
                        break;
                    default:
                        break;
                }
            }

            OldPadState = NewPadState;
            OldKeyState = NewKeyState;
        }

        public void Draw(SpriteBatch sBatch)
        {
            Vector2 buttonPos = new Vector2(winWidth / 2 - (bNewGame.Width / 2), 200);

            sBatch.Draw(titleCard, new Vector2(winWidth / 2 - (titleCard.Width / 2), 50), Color.White);

            switch (currentHighlight)
            {
                case Highlight.NewGame:
                    cNewGame = selected;
                    cLoadGame = Color.White;
                    cLevelEdit = Color.White;
                    cControls = Color.White;
                    cExit = Color.White;
                    break;
                case Highlight.LoadGame:
                    cNewGame = Color.White;
                    cLoadGame = selected;
                    cLevelEdit = Color.White;
                    cControls = Color.White;
                    cExit = Color.White;
                    break;
                case Highlight.LevelEditor:
                    cNewGame = Color.White;
                    cLoadGame = Color.White;
                    cLevelEdit = selected;
                    cControls = Color.White;
                    cExit = Color.White;
                    break;
                case Highlight.Controls:
                    cNewGame = Color.White;
                    cLoadGame = Color.White;
                    cLevelEdit = Color.White;
                    cControls = selected;
                    cExit = Color.White;
                    break;
                case Highlight.Exit:
                    cNewGame = Color.White;
                    cLoadGame = Color.White;
                    cLevelEdit = Color.White;
                    cControls = Color.White;
                    cExit = selected;
                    break;
                default:
                    break;
            }

            sBatch.Draw(bNewGame, buttonPos, cNewGame);
            buttonPos.Y += 45;
            sBatch.Draw(bLoadGame, buttonPos, cLoadGame);
            buttonPos.Y += 45;
            sBatch.Draw(bLevelEdit, buttonPos, cLevelEdit);
            buttonPos.Y += 45;
            sBatch.Draw(bControls, buttonPos, cControls);
            buttonPos.Y += 45;
            sBatch.Draw(bExit, buttonPos, cExit);

            sBatch.DrawString(vString, verString, new Vector2(10, winHeight - 20), Color.Black);

        }

    }
}
