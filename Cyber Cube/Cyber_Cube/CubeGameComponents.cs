using CyberCube.Screens;
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

    public abstract class SimpleDrawableCubeGameComponent : DrawableGameComponent
    {
        public new CubeGame Game
        {
            get {
                return (CubeGame) base.Game;
            }
        }

        public SimpleDrawableCubeGameComponent( CubeGame game )
            : base( game )
        {
        }

        public override abstract void Draw( GameTime gameTime );
    }

    public abstract class DrawableCubeGameComponent : SimpleDrawableCubeGameComponent
    {
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
    }

    public abstract class DrawableCubeScreenGameComponent : DrawableCubeGameComponent
    {
        public CubeScreen Screen
        {
            get; set;
        }

        public DrawableCubeScreenGameComponent( CubeGame game, CubeScreen screen )
            : base( game )
        {
            Screen = screen;
        }

    }

    public abstract class SimpleDrawableCubeScreenGameComponent : SimpleDrawableCubeGameComponent
    {
        public CubeScreen Screen
        {
            get; set;
        }

        public SimpleDrawableCubeScreenGameComponent( CubeGame game, CubeScreen screen )
            : base( game )
        {
            Screen = screen;
        }

    }
}
