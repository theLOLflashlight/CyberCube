using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CyberCube.Levels;
using Microsoft.Xna.Framework.Input;

namespace CyberCube.Screens
{
    public class CubeScreen : GameScreen
    {
        public Cube Cube
        {
            get; protected set;
        }

        public Camera Camera
        {
            get; protected set;
        }

        public GameHud Hud
        {
            get; protected set;
        }

        public CubeScreen( CubeGame game )
            : base( game )
        {
            Hud = new GameHud( this );
            Camera = new Camera();
        }

        public override void Initialize()
        {
            base.Initialize();
            Cube.Initialize();
            Hud.Initialize();
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            if ( Game.Input.Keyboard_WasKeyPressed( Keys.Escape ) )
                Back();

            Hud.Update( gameTime );
            Cube.Update( gameTime );
            Camera.Update( gameTime );
        }

        public override void Draw( GameTime gameTime )
        {
            Cube.Draw( gameTime );
            Hud.Draw( gameTime );
        }
    }
}
