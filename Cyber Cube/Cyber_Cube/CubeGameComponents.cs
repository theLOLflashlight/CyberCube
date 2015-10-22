using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube
{
    public abstract class CubeGameComponent : GameComponent
    {
        public new CubeGame Game
        {
            get {
                return (CubeGame) base.Game;
            }
        }

        public CubeGameComponent( CubeGame game )
            : base( game )
        {
        }

    }

    public abstract class DrawableCubeGameComponent : DrawableGameComponent
    {
        public new CubeGame Game
        {
            get {
                return (CubeGame) base.Game;
            }
        }

        protected SpriteBatch mSpriteBatch
        {
            get; private set;
        }

        public DrawableCubeGameComponent( CubeGame game )
            : base( game )
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            mSpriteBatch = new SpriteBatch( GraphicsDevice );
        }

        public override abstract void Draw( GameTime gameTime );
    }
}
