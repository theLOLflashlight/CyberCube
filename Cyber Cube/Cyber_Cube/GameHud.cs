using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CyberCube.Screens;

namespace CyberCube
{
    public class GameHud : DrawableCubeGameComponent
    {

        private SpriteFont mFont;

        public CubeScreen Screen
        {
            get; private set;
        }

        public GameHud( CubeScreen screen )
            : base( screen.Game )
        {
            Screen = screen;
            this.DrawOrder = 2;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            mFont = Game.Content.Load<SpriteFont>( "MessageFont" );
        }

        private double mUpdateRate = 0;

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            mUpdateRate = 1 / gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw( GameTime gameTime )
        {
            double drawRate = 1 / gameTime.ElapsedGameTime.TotalSeconds; 

            mSpriteBatch.Begin();

            string strDrawRate = drawRate.ToString( "0.0" ) + " FPS";
            var posDrawRate = mFont.MeasureString( strDrawRate );

            string strUpdateRate = mUpdateRate.ToString( "0.0" ) + " UPS";
            var posUpdateRate = mFont.MeasureString( strUpdateRate );

            mSpriteBatch.DrawString( mFont, strDrawRate,
                new Vector2( GraphicsDevice.Viewport.Width - posDrawRate.X, 0 ), Color.White );

            mSpriteBatch.DrawString( mFont, strUpdateRate,
                new Vector2( GraphicsDevice.Viewport.Width - posUpdateRate.X, posDrawRate.Y ), Color.White );

            mSpriteBatch.DrawString( mFont, Screen.Camera.Position.ToString(), Vector2.Zero, Color.White );
            mSpriteBatch.DrawString( mFont, Screen.Camera.UpVector.ToString(), new Vector2( 0, 30 ), Color.White );

            //mSpriteBatch.DrawString( mFont, "X: " + Screen.Player.WorldPosition.X.ToString( "F6" ), new Vector2( 0, 60 ), Color.White );
            //mSpriteBatch.DrawString( mFont, "Y: " + Screen.Player.WorldPosition.Y.ToString( "F6" ), new Vector2( 0, 90 ), Color.White );
            //mSpriteBatch.DrawString( mFont, "Z: " + Screen.Player.WorldPosition.Z.ToString( "F6" ), new Vector2( 0, 120 ), Color.White );

            mSpriteBatch.End();
        }

    }
}
