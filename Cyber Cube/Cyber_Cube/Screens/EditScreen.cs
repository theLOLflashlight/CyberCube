using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CyberCube.Levels;

namespace CyberCube.Screens
{
    public class EditScreen : CubeScreen
    {
        public new EditableCube Cube
        {
            get {
                return (EditableCube) base.Cube;
            }
            set {
                base.Cube = value;
            }
        }

        public EditScreen( CubeGame game )
            : base( game )
        {
            Cube = new EditableCube( game, this );
        }

        public EditScreen( CubeGame game, EditableCube editCube )
            : base( game )
        {
            Cube = editCube;
            Cube.Screen = this;
        }

        public override void Update( GameTime gameTime )
        {
            var input = Game.Input;

            if ( !input.HasFocus )
            {
                if ( input.GetAction( Action.ToggleCubeMode ) )
                {
                    PlayableCube playCube = Cube.GeneratePlayableCube();
                    ScreenManager.PushScreen( new PlayScreen( Game, playCube ) );
                }
            }

            base.Update( gameTime );
        }

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );
        }
    }
}
