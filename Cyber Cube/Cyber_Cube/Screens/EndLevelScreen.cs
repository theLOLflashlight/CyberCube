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

namespace CyberCube.Screens
{
    public class EndLevelScreen : GameScreen
    {
        private static SpriteFont sFont;
        private static Texture2D sLevelClear;
        private static Texture2D sScoreTitle;
        private static Texture2D sHighScores;

        private string pLevelName;
        private SaveData pSaveData;
        private List<Achievement> pAchievements;
        private int pScore;

        int asyncState = 0;
        private IAsyncResult result;

        public static void LoadContent(ContentManager content)
        {
            sFont = content.Load<SpriteFont>("EndLevelScreenFont");
            sLevelClear = content.Load <Texture2D>("NavigationItems\\levelClear");
            sScoreTitle = content.Load<Texture2D>("NavigationItems\\scoreBreakdown");
            sHighScores = content.Load<Texture2D>("NavigationItems\\levelScores");
        }

        public EndLevelScreen(CubeGame game, List<Achievement> achieved, string levelName )
            : base(game)
        {
            pLevelName = levelName;
            pSaveData = SaveData.Load( pLevelName );

            pAchievements = achieved;
            pScore = 0;

            foreach( Achievement a in pAchievements )
                pScore += a.Value;

            // TODO: Replace Tester with user's name
            pSaveData.AddScore( pScore, "The World's #1" );
            pSaveData.Save( pLevelName );

#if XBOX
            this.Components.Add(new GamerServicesComponent(Game));
#endif

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Y))
            {
#if XBOX

                if (asyncState == 0) asyncState = 1;
                switch (asyncState) {
                    case 1:
                        result = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Player Name", "Enter your name:", "", null, null); 
                        asyncState = 2; 
                        break; 
                    
                    case 2:
                        if (result.IsCompleted) 
                        { 
                            //PlayerName = Guide.EndShowKeyboardInput(result); 

                            pSaveData.AddScore( pScore, Guide.EndShowKeyboardInput(result));
                            pSaveData.Save( pLevelName );
                            asyncState = 0;
                        }

                        break;
                    }

                GamerServicesDispatcher.Update();

#endif

                if ((Keyboard.GetState().IsKeyDown(Keys.Enter)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A)))
                    this.Back();
                }
        }

        public override void Draw(GameTime gameTime)
        {
            int delta = 0;

            GraphicsDevice.Clear(Color.Black);

            mSpriteBatch.Begin();

            mSpriteBatch.Draw( sLevelClear, 
                               new Vector2( GraphicsDevice.Viewport.Width / 2 - ( sLevelClear.Width / 2 ), 50 ), 
                               Color.White );

            mSpriteBatch.Draw( sScoreTitle,
                               new Vector2( GraphicsDevice.Viewport.Width / 4 - ( sScoreTitle.Width / 2 ), 150 ),
                               Color.White );

            mSpriteBatch.Draw( sHighScores,
                               new Vector2( GraphicsDevice.Viewport.Width * 3 / 4 - ( sHighScores.Width / 2 ), 150 ),
                               Color.White );


            // Level Scoring segment
            foreach (Achievement a in pAchievements)
            {
                mSpriteBatch.DrawString(sFont,
                                    a.Title,
                                    new Vector2(GraphicsDevice.Viewport.Width / 7, 225 + delta),
                                    Color.White);

                mSpriteBatch.DrawString(sFont,
                                    a.Value.ToString(),
                                    new Vector2(GraphicsDevice.Viewport.Width / 3, 225 + delta),
                                    Color.White);

                delta += 25;
            }

            mSpriteBatch.DrawString(sFont,
                                    "Total Score:",
                                    new Vector2(GraphicsDevice.Viewport.Width / 7, 225 + delta),
                                    Color.White);

            mSpriteBatch.DrawString(sFont,
                                    pScore.ToString(),
                                    new Vector2(GraphicsDevice.Viewport.Width / 3, 225 + delta),
                                    Color.White);

            // High score segment
            delta = 0;

            foreach(Score s in pSaveData.Scores.OrderBy( s => -1* s.score ).OrderBy( s => s.name ))
            {
                mSpriteBatch.DrawString(sFont,
                                    s.name,
                                    new Vector2(GraphicsDevice.Viewport.Width / 7 + GraphicsDevice.Viewport.Width / 2, 225 + delta),
                                    Color.White);

                mSpriteBatch.DrawString(sFont,
                                    s.score.ToString(),
                                    new Vector2(GraphicsDevice.Viewport.Width / 3 + GraphicsDevice.Viewport.Width / 2, 225 + delta),
                                    Color.White);

                delta += 25;
            }

            mSpriteBatch.DrawString( sFont,
                                    "A (GamePad) or Enter (Keyboard) to Continue",
                                    new Vector2( GraphicsDevice.Viewport.Width * 5 / 9, GraphicsDevice.Viewport.Height - 50 ),
                                    Color.White );

            mSpriteBatch.End();
        }
    }
}
