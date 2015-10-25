using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CyberCube.IO;
using CyberCube.Graphics;

namespace CyberCube.MenuFiles
{
    class PauseMenu
    {
        enum Highlight
        {
            ResumeGame,
            SaveGame,
            Controls,
            QuitToMenu,
        }

        private Highlight currentHighlight;

        private Texture2D bResumeGame;
        private Texture2D bSaveGame;
        private Texture2D bControls;
        private Texture2D bQuitMenu;

        private Rectangle bg;
        private Vector2 bStart;

        private Color selected;
        private Color cResumeGame, cSaveGame, cControls, cQuit;

        private Boolean fade;

        private GamePadState OldPadState;
        private KeyboardState OldKeyState;

        public int Status { get; set; }

        public PauseMenu(Game game)
        {
            bResumeGame = game.Content.Load<Texture2D>("PauseItems\\pauseResume");
            bSaveGame = game.Content.Load<Texture2D>("PauseItems\\pauseSaveGame");
            bControls = game.Content.Load<Texture2D>("MenuItems\\menuControls");
            bQuitMenu = game.Content.Load<Texture2D>("PauseItems\\pauseQuitMenu");

            currentHighlight = Highlight.ResumeGame;

            selected = new Color(255, 255, 255, 255);
            fade = true;

            cResumeGame = Color.White;
            cSaveGame = Color.White;
            cControls = Color.White;
            cQuit = Color.White;

            bg = new Rectangle(game.GraphicsDevice.Viewport.Width / 4,
                               game.GraphicsDevice.Viewport.Height / 4,
                               game.GraphicsDevice.Viewport.Width / 2,
                               game.GraphicsDevice.Viewport.Height / 2);

            bStart = new Vector2(game.GraphicsDevice.Viewport.Width / 2 - bResumeGame.Width / 2, 150);
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
                    case Highlight.ResumeGame:
                        break;
                    case Highlight.SaveGame:
                        currentHighlight = Highlight.ResumeGame;
                        break;
                    case Highlight.Controls:
                        currentHighlight = Highlight.SaveGame;
                        break;
                    case Highlight.QuitToMenu:
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
                    case Highlight.ResumeGame:
                        currentHighlight = Highlight.SaveGame;
                        break;
                    case Highlight.SaveGame:
                        currentHighlight = Highlight.Controls;
                        break;
                    case Highlight.Controls:
                        currentHighlight = Highlight.QuitToMenu;
                        break;
                    case Highlight.QuitToMenu:
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
                    case Highlight.ResumeGame:
                        Status = 1;
                        break;
                    case Highlight.SaveGame:
                        break;
                    case Highlight.Controls:
                        break;
                    case Highlight.QuitToMenu:
                        Status = 4;
                        break;
                    default:
                        break;
                }
            }

            if ((NewKeyState.IsKeyDown(Keys.O) && OldKeyState.IsKeyUp(Keys.O)))
                Status = 1;

            OldPadState = NewPadState;
            OldKeyState = NewKeyState;
        }

        public void Draw(SpriteBatch sBatch)
        {
            sBatch.DrawRect(bg, Color.White);

            switch (currentHighlight)
            {
                case Highlight.ResumeGame:
                    cResumeGame = selected;
                    cSaveGame = Color.White;
                    cControls = Color.White;
                    cQuit = Color.White;
                    break;
                case Highlight.SaveGame:
                    cResumeGame = Color.White;
                    cSaveGame = selected;
                    cControls = Color.White;
                    cQuit = Color.White;
                    break;
                case Highlight.Controls:
                    cResumeGame = Color.White;
                    cSaveGame = Color.White;
                    cControls = selected;
                    cQuit = Color.White;
                    break;
                case Highlight.QuitToMenu:
                    cResumeGame = Color.White;
                    cSaveGame = Color.White;
                    cControls = Color.White;
                    cQuit = selected;
                    break;
                default:
                    break;
            }

            sBatch.Draw(bResumeGame, bStart, cResumeGame);
            bStart.Y += 45;
            sBatch.Draw(bSaveGame, bStart, cSaveGame);
            bStart.Y += 45;
            sBatch.Draw(bControls, bStart, cControls);
            bStart.Y += 45;
            sBatch.Draw(bQuitMenu, bStart, cQuit);
            bStart.Y -= 135;
        }

        public void EnterPauseMenu()
        {
            currentHighlight = Highlight.ResumeGame;
            Status = 0;
        }
    }
}
