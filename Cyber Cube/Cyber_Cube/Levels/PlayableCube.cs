using CyberCube.Physics;
using CyberCube.Screens;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Levels
{
    public class PlayableCube : Cube
    {
        public static void LoadContent( ContentManager content )
        {
            Face.LoadContent( content );
        }

        public new PlayScreen Screen
        {
            get {
                return (PlayScreen) base.Screen;
            }
            set {
                base.Screen = value;
            }
        }

        public PlayableCube( CubeGame game, PlayScreen screen = null )
            : base( game, screen )
        {
        }

        protected override Cube.Face NewFace( string name, Vector3 normal, Vector3 up, Direction rotation )
        {
            return new Face( this, name, normal, up, rotation );
        }

        public void CenterOnPlayer( Player player )
        {
            CurrentFace = player.CubeFace;
            UpDir = player.UpDir;

            Screen.Camera.AnimatePosition( CameraDistance * CurrentFace.Normal, CameraDistance );
            Screen.Camera.AnimateUpVector( ComputeUpVector(), 1 );
            //Screen.Camera.SkipAnimation();
        }

        public override void Update( GameTime gameTime )
        {
            var input = Game.Input;

            if ( !input.HasFocus )
            {
                if ( input.GetAction( Action.RotateClockwise ) )
                    RotateClockwise();

                if ( input.GetAction( Action.RotateAntiClockwise ) )
                    RotateAntiClockwise();
            }

            Screen.Camera.Target = Screen.Player.WorldPosition;

            base.Update( gameTime );
        }

        public new partial class Face : Cube.Face
        {
            internal static void LoadContent( ContentManager content )
            {
            }

            public new PlayableCube Cube
            {
                get {
                    return (PlayableCube) base.Cube;
                }
            }

            public Face( PlayableCube cube, string name, Vector3 normal, Vector3 up, Direction orientation )
                : base( cube, name, normal, up, orientation )
            {
            }

            public override void Update( GameTime gameTime )
            {
                base.Update( gameTime );

                World.Step( (float) gameTime.ElapsedGameTime.TotalSeconds );
            }

        }

    }
}
