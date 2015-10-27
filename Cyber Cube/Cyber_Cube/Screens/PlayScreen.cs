using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CyberCube.Levels;
using Microsoft.Xna.Framework.Input;

namespace CyberCube.Screens
{
    public class PlayScreen : CubeScreen
    {
        public new PlayableCube Cube
        {
            get {
                return (PlayableCube) base.Cube;
            }
            set {
                base.Cube = value;
            }
        }

        public Player Player
        {
            get; private set;
        }

        public PlayScreen( CubeGame game )
            : base( game )
        {
            Cube = new PlayableCube( game, this );
        }

        public PlayScreen( CubeGame game, PlayableCube playCube )
            : base( game )
        {
            Cube = playCube;
            Cube.Screen = this;
        }

        public override void Initialize()
        {
            base.Initialize();
            Player = new Player( this, Cube, Vector3.UnitZ, Direction.Up );
            Player.Initialize();
        }

        public override void Update( GameTime gameTime )
        {
            Player.Update( gameTime );
            base.Update( gameTime );
        }

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );
            Player.Draw( gameTime );
        }
    }
}
