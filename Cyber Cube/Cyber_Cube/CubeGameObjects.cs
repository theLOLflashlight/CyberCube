using CyberCube.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube
{
    public abstract class CubeGameObject
    {
        public CubeGame Game
        {
            get; private set;
        }

        public CubeGameObject( CubeGame game )
        {
            Game = game;
        }

        public abstract void Update( GameTime gameTime );

    }


    public abstract class DrawableCubeGameObject : CubeGameObject
    {
        protected SpriteBatch mSpriteBatch
        {
            get; private set;
        }

        public GraphicsDevice GraphicsDevice
        {
            get {
                return mSpriteBatch.GraphicsDevice;
            }
        }

        public DrawableCubeGameObject( CubeGame game )
            : base( game )
        {
            mSpriteBatch = new SpriteBatch( Game.GraphicsDevice );
        }

        public abstract void Draw( GameTime gameTime );

    }
}
