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
    public class EndLevelScreen : GameScreen
    {
        private static SpriteFont sFont;

        public static void LoadContent(ContentManager content)
        {

        }

        public EndLevelScreen( CubeGame game )
            :base (game)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
        }
    }
}
