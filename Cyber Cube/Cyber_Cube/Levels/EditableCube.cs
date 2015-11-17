using CyberCube.Brushes;
using CyberCube.Physics;
using CyberCube.Screens;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Levels
{
    public class EditableCube : Cube
    {
        private static Model sModel3D;

        public static void LoadContent( ContentManager content )
        {
            Face.LoadContent( content );
            sModel3D = content.Load<Model>( "Models\\playerAlpha3D" );
        }

        public new EditScreen Screen
        {
            get {
                return (EditScreen) base.Screen;
            }
            set {
                base.Screen = value;
            }
        }

        public EditableCube( CubeGame game, EditScreen screen = null )
            : base( game, screen )
        {
        }

        protected override Cube.Face NewFace( CubeFaceType type, Vector3 normal, Vector3 up, Direction rotation )
        {
            return new Face( this, type, normal, up, rotation );
        }

        /// <summary>
        /// Generates a PlayableCube object from the current EditableCube's state.
        /// </summary>
        /// <returns>A playable copy of the cube.</returns>
        internal PlayableCube GeneratePlayableCube()
        {
            PlayableCube cube = new PlayableCube( Game );

            var editFaces = Faces.GetEnumerator();
            var playFaces = cube.Faces.GetEnumerator();

            while ( editFaces.MoveNext() && playFaces.MoveNext() )
                playFaces.Current.CopySolids( editFaces.Current );

            cube.StartPosition = this.StartPosition;
            return cube;
        }

        public override void Update( GameTime gameTime )
        {
            var input = Game.Input;

            Screen.Camera.Target = mPosition;

            if ( !input.HasFocus )
            {
                if ( input.GetAction( Action.RotateRight ) )
                    RotateRight();

                if ( input.GetAction( Action.RotateLeft ) )
                    RotateLeft();

                if ( input.GetAction( Action.RotateUp ) )
                    RotateTop();

                if ( input.GetAction( Action.RotateDown ) )
                    RotateBottom();

                if ( input.GetAction( Action.RotateClockwise ) )
                    RotateClockwise();

                if ( input.GetAction( Action.RotateAntiClockwise ) )
                    RotateAntiClockwise();
            }

            base.Update( gameTime );
        }

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );

            Matrix[] transforms = new Matrix[ sModel3D.Bones.Count ];
            sModel3D.CopyAbsoluteBoneTransformsTo( transforms );

            Cube.Face face = GetFaceFromPosition( StartPosition.Position );

            // Draw the model. A model can have multiple meshes, so loop.
            foreach ( ModelMesh mesh in sModel3D.Meshes )
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach ( BasicEffect effect in mesh.Effects )
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[ mesh.ParentBone.Index ]
                        * Matrix.CreateScale( 0.0006f )
                        * Matrix.CreateTranslation( 0, -5.ToUnits(), 0 )
                        * Vector3.UnitY.RotateOntoM( face.UpVec )
                        * Matrix.CreateFromAxisAngle( face.Normal, -StartPosition.Rotation )
                        * Matrix.CreateTranslation( StartPosition.Position );

                    Screen.Camera.Apply( effect );
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

        public new partial class Face : Cube.Face
        {
            private static SpriteFont sFont;

            internal static void LoadContent( ContentManager content )
            {
                sFont = content.Load< SpriteFont >( "MessageFontLarge" );
            }

#if WINDOWS
            public FarseerPhysics.DebugView.DebugViewXNA DebugView
            {
                get {
                    return mDebugView;
                }
            }
#endif

            private IEditBrush LeftBrush
            {
                get {
                    return Cube.Screen.LeftBrush;
                }
            }

            private IEditBrush RightBrush
            {
                get {
                    return Cube.Screen.RightBrush;
                }
            }

            private IEditBrush MiddleBrush
            {
                get {
                    return Cube.Screen.MiddleBrush;
                }
            }

            public new EditableCube Cube
            {
                get {
                    return (EditableCube) base.Cube;
                }
            }

            public Face( EditableCube cube, CubeFaceType type, Vector3 normal, Vector3 up, Direction orientation )
                : base( cube, type, normal, up, orientation )
            {
            }

            public override void Draw( GameTime gameTime )
            {
                base.Draw( gameTime );

                mSpriteBatch.Begin();

                LeftBrush?.Draw( this, mSpriteBatch, gameTime );
                RightBrush?.Draw( this, mSpriteBatch, gameTime );
                MiddleBrush?.Draw( this, mSpriteBatch, gameTime );

                mSpriteBatch.DrawString( sFont,
                                         Name,
                                         new Vector2( WIDTH, HEIGHT ) / 2,
                                         new Color( 0, 0, 0, 128 ),
                                         0,
                                         sFont.MeasureString( Name ) / 2,
                                         5,
                                         SpriteEffects.None,
                                         0 );

                mSpriteBatch.End();
            }

            public override void Update( GameTime gameTime )
            {
                base.Update( gameTime );

                World.Step( 0 );

                var input = Game.Input;

                Vector2? mouseFacePos = GetMouseFacePosition();
                if ( mouseFacePos != null )
                {
                    if ( input.Mouse_WasButtonPressed( IO.MouseButtons.Left ) )
                        LeftBrush?.Start( this, mouseFacePos.Value, gameTime );

                    if ( input.Mouse_WasButtonPressed( IO.MouseButtons.Right ) )
                        RightBrush?.Start( this, mouseFacePos.Value, gameTime );

                    if ( input.Mouse_WasButtonPressed( IO.MouseButtons.Middle ) )
                        MiddleBrush?.Start( this, mouseFacePos.Value, gameTime );
                }

                Vector2? mousePlanePos = mouseFacePos ?? GetMousePlanePosition();
                if ( mousePlanePos != null )
                {
                    LeftBrush?.Update( this, mousePlanePos.Value, gameTime );
                    RightBrush?.Update( this, mousePlanePos.Value, gameTime );
                    MiddleBrush?.Update( this, mousePlanePos.Value, gameTime );
                }

                if ( input.Mouse_WasButtonReleased( IO.MouseButtons.Left ) )
                    LeftBrush?.End( this, mousePlanePos, gameTime );

                if ( input.Mouse_WasButtonReleased( IO.MouseButtons.Right ) )
                    RightBrush?.End( this, mousePlanePos, gameTime );

                if ( input.Mouse_WasButtonReleased( IO.MouseButtons.Middle ) )
                    MiddleBrush?.End( this, mousePlanePos, gameTime );
            }

        }
    }
}
