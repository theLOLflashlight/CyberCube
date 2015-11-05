using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.IO;

namespace CyberCube.Screens
{
    public class PauseScreen : GameScreen
    {
        private static Texture2D sResumeGame;
        private static Texture2D sSaveGame;
        private static Texture2D sControls;
        private static Texture2D sQuitMenu;

        enum Highlight
        {
            ResumeGame,
            SaveGame,
            Controls,
            QuitToMenu,
        }

        private Highlight currentHighlight;

        private Rectangle bg;
        private Vector2 bStart;

        private Color cSelected;
        private Color cResumeGame, cSaveGame, cControls, cQuit;

        private Boolean isFading;

        private GamePadState OldPadState;
        private KeyboardState OldKeyState;

        public static void LoadContent(ContentManager content)
        {
            sResumeGame = content.Load<Texture2D>("NavigationItems\\pauseResume");
            sSaveGame = content.Load<Texture2D>("NavigationItems\\pauseSaveGame");
            sControls = content.Load<Texture2D>("NavigationItems\\menuControls");
            sQuitMenu = content.Load<Texture2D>("NavigationItems\\pauseQuitMenu");
        }

        public PauseScreen( CubeGame game )
            : base( game )
        {
            currentHighlight = Highlight.ResumeGame;

            cSelected = new Color(255, 255, 255, 255);
            isFading = true;

            cResumeGame = Color.White;
            cSaveGame = Color.White;
            cControls = Color.White;
            cQuit = Color.White;

            bg = new Rectangle(game.GraphicsDevice.Viewport.Width / 4,
                               game.GraphicsDevice.Viewport.Height / 4,
                               game.GraphicsDevice.Viewport.Width / 2,
                               game.GraphicsDevice.Viewport.Height / 2);

            bStart = new Vector2(game.GraphicsDevice.Viewport.Width / 2 - sResumeGame.Width / 2, 150);
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            // Fade effect for highlighted menu button
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

            // Change highlighted button for "up"
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

            // Change highlighted button for "down"
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
                        Back();
                        break;
                    case Highlight.SaveGame:
                        break;
                    case Highlight.Controls:
                        break;
                    case Highlight.QuitToMenu:
                        ScreenManager.PauseMenuToMainMenu(2);
                        break;
                    default:
                        break;
                }
            }

            if ((NewKeyState.IsKeyDown(Keys.Escape) && OldKeyState.IsKeyUp(Keys.Escape)))
                Back();

            OldPadState = NewPadState;
            OldKeyState = NewKeyState;
        }

        public override void Draw( GameTime gameTime )
        {
            mSpriteBatch.Begin();

            GraphicsDevice.Clear(Color.White);

            mSpriteBatch.Draw(new Texture2D(GraphicsDevice, bg.Width, bg.Height), bg, Color.White);

            switch (currentHighlight)
            {
                case Highlight.ResumeGame:
                    cResumeGame = cSelected;
                    cSaveGame = Color.White;
                    cControls = Color.White;
                    cQuit = Color.White;
                    break;
                case Highlight.SaveGame:
                    cResumeGame = Color.White;
                    cSaveGame = cSelected;
                    cControls = Color.White;
                    cQuit = Color.White;
                    break;
                case Highlight.Controls:
                    cResumeGame = Color.White;
                    cSaveGame = Color.White;
                    cControls = cSelected;
                    cQuit = Color.White;
                    break;
                case Highlight.QuitToMenu:
                    cResumeGame = Color.White;
                    cSaveGame = Color.White;
                    cControls = Color.White;
                    cQuit = cSelected;
                    break;
                default:
                    break;
            }

            mSpriteBatch.Draw(sResumeGame, bStart, cResumeGame);
            bStart.Y += 45;
            mSpriteBatch.Draw(sSaveGame, bStart, cSaveGame);
            bStart.Y += 45;
            mSpriteBatch.Draw(sControls, bStart, cControls);
            bStart.Y += 45;
            mSpriteBatch.Draw(sQuitMenu, bStart, cQuit);
            bStart.Y -= 135;

            mSpriteBatch.End();
        }
    }
}
