using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CyberCube.IO;

using System.IO;

namespace CyberCube.Screens
{
    public class EndLevelScreen : GameScreen
    {
        private static SpriteFont sFont;

        public static void LoadContent(ContentManager content)
        {
            sFont = content.Load<SpriteFont>("MessageFontLarge");
        }

        public EndLevelScreen(CubeGame game)
            : base(game)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                this.Back();
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            mSpriteBatch.Begin();
            mSpriteBatch.DrawString(sFont,
                                    "Level Clear, woohoo",
                                    new Vector2(GraphicsDevice.Viewport.Width / 3,
                                                GraphicsDevice.Viewport.Height / 3),
                                    Color.White);

            mSpriteBatch.DrawString(sFont,
                                    "Press Enter to continue",
                                    new Vector2(GraphicsDevice.Viewport.Width / 3,
                                                GraphicsDevice.Viewport.Height * 2 / 3),
                                    Color.White);
            mSpriteBatch.End();
        }
    }
}
