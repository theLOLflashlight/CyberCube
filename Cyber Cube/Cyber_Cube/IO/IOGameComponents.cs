using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.IO
{
    public abstract class IOGameComponent : GameComponent
    {
        private IInputProvider mInputProvider;

        protected InputState Input
        {
            get {
                return mInputProvider.Input;
            }
        }

        public IOGameComponent( Game game, IInputProvider inputProvider )
            : base( game )
        {
            mInputProvider = inputProvider;
        }
    }

    public abstract class IODrawableGameComponent : DrawableGameComponent
    {
        private IInputProvider mInputProvider;

        protected InputState Input
        {
            get {
                return mInputProvider.Input;
            }
        }

        protected SpriteBatch mSpriteBatch
        {
            get; private set;
        }

        public IODrawableGameComponent( Game game, IInputProvider inputProvider )
            : base( game )
        {
            mInputProvider = inputProvider;
        }

        public override void Initialize()
        {
            base.Initialize();
            mSpriteBatch = new SpriteBatch( GraphicsDevice );
        }
    }
}
