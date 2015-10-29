using CyberCube.Physics;
using CyberCube.Screens;
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
        public static void LoadContent( ContentManager content )
        {
            Face.LoadContent( content );
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

        protected override Cube.Face NewFace( string name, Vector3 normal, Vector3 up, Direction rotation )
        {
            return new Face( this, name, normal, up, rotation );
        }

        /// <summary>
        /// Not yet implemented.
        /// </summary>
        /// <returns>A playable copy of the cube.</returns>
        internal PlayableCube GeneratePlayableCube()
        {
            PlayableCube cube = new PlayableCube( Game );

            var e = cube.Faces.GetEnumerator();
            foreach ( Face editFace in Faces )
            {
                e.MoveNext();
                e.Current.CopySolids( editFace );
            }

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

        public new partial class Face : Cube.Face
        {
            private static SpriteFont sFont;

            internal static void LoadContent( ContentManager content )
            {
                sFont = content.Load< SpriteFont >( "MessageFontLarge" );
            }

            public new EditableCube Cube
            {
                get {
                    return (EditableCube) base.Cube;
                }
            }

            private Solid mFocusSolid;
            private Vector2 mFocusLocalPoint;
            private Vector2 mFocusClickPosition;

            private Vector2? mMouseDragStart;
            private Rectangle? mTentativeRec;

            public Face( EditableCube cube, string name, Vector3 normal, Vector3 up, Direction orientation )
                : base( cube, name, normal, up, orientation )
            {
            }

            public override void Draw( GameTime gameTime )
            {
                base.Draw( gameTime );

                mSpriteBatch.Begin();

                if ( mTentativeRec != null )
                    mSpriteBatch.Draw( pixel, mTentativeRec.Value, new Color( 0, 0, 0, 128 ) );

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

                var input = Game.Input;

                if ( mMouseDragStart == null )
                    mTentativeRec = null;


                Vector2? mouseFacePos = MouseFacePosition();
                if ( mouseFacePos != null )
                    UpdateMouse( gameTime, mouseFacePos.Value );


                if ( mMouseDragStart != null && input.Mouse_WasRightReleased() )
                {
                    if ( mTentativeRec != null )
                        AddRectangle( mTentativeRec.Value );
                    mMouseDragStart = null;
                }

                if ( mFocusSolid != null && input.Mouse_WasLeftReleased() )
                {
                    mFocusSolid.Body.Enabled = true;
                    mFocusSolid = null;
                }
            }

            private void UpdateMouse( GameTime gameTime, Vector2 mouseFacePos )
            {
                var input = Game.Input;

                if ( mMouseDragStart != null )
                {
                    Vector2 pos = mMouseDragStart.Value;

                    int x = (int) Math.Min( pos.X, mouseFacePos.X );
                    int y = (int) Math.Min( pos.Y, mouseFacePos.Y );

                    int w = (int) Math.Abs( mouseFacePos.X - pos.X );
                    int h = (int) Math.Abs( mouseFacePos.Y - pos.Y );

                    if ( w > 0.5 * Physics.Constants.UNIT_TO_PIXEL
                         && h > 0.5 * Physics.Constants.UNIT_TO_PIXEL )
                        mTentativeRec = new Rectangle( x, y, w, h );
                    else
                        mTentativeRec = null;
                }

                if ( input.Mouse_WasRightPressed() )
                {
                    mMouseDragStart = mouseFacePos;
                }


                if ( mFocusSolid != null )
                {
                    Vector2 worldPos = mouseFacePos * Physics.Constants.PIXEL_TO_UNIT;
                    Vector2 delta = worldPos - mFocusClickPosition;

                    mFocusSolid.Body.Position += delta;
                    mFocusClickPosition = worldPos;
                }

                if ( input.Mouse_WasLeftPressed() )
                {
                    Vector2 clickPos = mouseFacePos * Physics.Constants.PIXEL_TO_UNIT;

                    mFocusSolid = FindSolidAt( ref clickPos );
                    if ( mFocusSolid != null )
                    {
                        mFocusLocalPoint = mFocusSolid.Body.GetLocalPoint( ref clickPos );
                        mFocusClickPosition = clickPos;
                        mFocusSolid.Body.Enabled = false;
                    }
                }

                if ( input.Mouse_WasMiddlePressed() )
                {
                    Solid solid = FindSolidAt( mouseFacePos * Physics.Constants.PIXEL_TO_UNIT );
                    if ( solid != null )
                    {
                        switch ( solid.BodyType )
                        {
                        case BodyType.Static:
                            solid.BodyType = BodyType.Dynamic;
                            break;
                        case BodyType.Dynamic:
                            solid.BodyType = BodyType.Static;
                            break;
                        }
                    }
                }
            }

            private void AddRectangle( Rectangle rec )
            {
                var box = new Box(
                    Game,
                    World,
                    rec,
                    BodyType.Static,
                    rec.Width * rec.Height * 0.003f,
                    Category.Cat2 );

                box.Body.UseAdHocGravity = true;
                box.Body.AdHocGravity =
                    Vector2.UnitY.Rotate( Cube.UpDir.Angle ).Rounded()
                    * 9.8f;

                AddSolid( box );
            }
            
        }
    }
}
