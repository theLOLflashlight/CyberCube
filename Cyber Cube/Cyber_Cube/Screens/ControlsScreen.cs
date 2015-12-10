using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CyberCube.IO;

using System.IO;
using System.Threading;
using Microsoft.Xna.Framework.Audio;

namespace CyberCube.Screens
{
    public class ControlsScreen : GameScreen
    {
        private static Texture2D sControllerMap;
        private static Texture2D sKeyboardMap;
        private static Texture2D sBack;

        private static SoundEffect sfxConfirm;

        KeyboardState newKeyState, oldKeyState;
        GamePadState newPadState, oldPadState;

        bool isIn;

        private Color cSelected;
        private bool isFading;

        public static void LoadContent(ContentManager content)
        {
            sControllerMap = content.Load<Texture2D>("NavigationItems\\360_controller");
            sKeyboardMap = content.Load<Texture2D>("NavigationItems\\keyboardControls");
            sBack = content.Load<Texture2D>("NavigationItems\\button_Back");


            sfxConfirm = content.Load<SoundEffect>("Audio\\buttonPressed2");
        }

        public ControlsScreen(CubeGame game)
            : base( game )
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            cSelected = new Color(0, 240, 255, 255);
            isIn = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

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

            newKeyState = Keyboard.GetState();
            newPadState = GamePad.GetState(PlayerIndex.One);

            if((newKeyState.IsKeyUp(Keys.Enter) && oldKeyState.IsKeyDown(Keys.Enter)) || (newPadState.IsButtonUp(Buttons.A) && oldPadState.IsButtonDown(Buttons.A)))
            {
                
                if (isIn)
                {
                    sfxConfirm.Play();
                    Back();
                }
                else
                {
                    isIn = true;
                }
            }

            oldKeyState = newKeyState;
            oldPadState = newPadState;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.White);

            mSpriteBatch.Begin();
            mSpriteBatch.Draw(sControllerMap, new Vector2(GraphicsDevice.Viewport.Width / 2 - sControllerMap.Width * 5 / 6, GraphicsDevice.Viewport.Height / 2 - 250), Color.White);
            mSpriteBatch.Draw(sKeyboardMap, new Vector2(GraphicsDevice.Viewport.Width / 2 + 150, GraphicsDevice.Viewport.Height / 2 - 250), Color.White);
            mSpriteBatch.Draw(sBack, new Vector2(GraphicsDevice.Viewport.Width / 2 - sBack.Width / 2, GraphicsDevice.Viewport.Height - 100), cSelected);
            mSpriteBatch.End();
        }
    }
}
