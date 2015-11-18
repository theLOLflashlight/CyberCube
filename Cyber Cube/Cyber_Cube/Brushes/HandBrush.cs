using CyberCube.Levels;
using CyberCube.Physics;
using CyberCube.Screens;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Brushes
{
    public class HandBrush : IEditBrush
    {
        private EditableCube.Face mFace;

        private Solid mSelectedSolid;
        private Vector2 mStartPos;
        private Vector2 mOriginalPos;

        protected CubeGame Game
        {
            get; set;
        }

        public HandBrush( CubeGame game )
        {
            Game = game;
        }

        public bool Started
        {
            get; private set;
        }

        public void Start( EditableCube.Face face, Vector2 mousePos, GameTime gameTime )
        {
            Solid selectedSolid = face.FindSolidAt( mousePos.ToUnits() );

            if ( selectedSolid != mSelectedSolid )
                Cancel();

            if ( selectedSolid != null )
            {
                mFace = face;
                Started = true;
                mStartPos = mousePos;
                mSelectedSolid = selectedSolid;
                mOriginalPos = mSelectedSolid.Position;
                Game.Input.Focus = mSelectedSolid;
            }
        }

        public void Update( EditableCube.Face face, Vector2 mousePos, GameTime gameTime )
        {
            if ( face != mFace )
                return;

            var input = Game.Input;

            if ( input.CheckFocus( mSelectedSolid ) )
            {
                Vector2 delta = Vector2.Zero;

                if ( input.Keyboard.IsKeyDown( Keys.Up ) )
                    delta.Y += -1.ToUnits();

                if ( input.Keyboard.IsKeyDown( Keys.Right ) )
                    delta.X += 1.ToUnits();

                if ( input.Keyboard.IsKeyDown( Keys.Down ) )
                    delta.Y += 1.ToUnits();

                if ( input.Keyboard.IsKeyDown( Keys.Left ) )
                    delta.X += -1.ToUnits();


                if ( input.GetAction( Action.RotateClockwise ) )
                    mSelectedSolid.Rotation += MathHelper.PiOver2;

                if ( input.GetAction( Action.RotateAntiClockwise ) )
                    mSelectedSolid.Rotation -= MathHelper.PiOver2;


                mSelectedSolid.Position += delta.Rotate( face.Cube.UpDir.Angle );

                if ( input.Keyboard_WasAnyKeyPressed( Keys.Delete, Keys.Back ) )
                {
                    face.RemoveSolid( mSelectedSolid );
                    Cancel();
                }
            }

            if ( !Started )
                return;

            mSelectedSolid.Position = mOriginalPos + (mousePos.ToUnits() - mStartPos.ToUnits());

            if ( !input.IsShiftDown() )
            {
                mSelectedSolid.Position = EditScreen.SnapVector( mSelectedSolid.Position.ToPixels(), EditScreen.SNAP_SIZE / 2 ).ToUnits();
            }
            else
            {
                mSelectedSolid.Position = mSelectedSolid.Position
                                                        .ToPixels()
                                                        .Rounded()
                                                        .ToUnits();
            }
        }

        public void End( EditableCube.Face face, Vector2? mousePos, GameTime gameTime )
        {
            if ( face != mFace )
                return;

            if ( !Started )
            {
                Cancel();
            }
            else
            {
                Started = false;
                mStartPos = Vector2.Zero;
            }
        }

        public void Cancel()
        {
            mFace = null;
            Started = false;

            if ( Game.Input.CheckFocus( mSelectedSolid ) )
                Game.Input.Focus = null;

            mSelectedSolid = null;
            mStartPos = Vector2.Zero;
        }

        public void Draw( EditableCube.Face face, SpriteBatch spriteBatch, GameTime gameTime )
        {
            if ( face != mFace )
                return;

#if WINDOWS
            if ( mSelectedSolid != null )
            {
                face.DebugView.BeginCustomDraw( Cube.Face.DEBUG_PROJECTION, Matrix.Identity );

                double t = (gameTime.TotalGameTime.TotalSeconds % 4 / 2) - 1;
                byte val = (byte) (150 + Math.Abs( t * 105 ));
                Color color = new Color( val, val, val, 255 );

                Body b = mSelectedSolid.Body;
                Transform trans;
                b.GetTransform( out trans );
                foreach ( Fixture f in b.FixtureList )
                    face.DebugView.DrawShape( f, trans, color );

                face.DebugView.EndCustomDraw();
            }
#endif
        }
    }
}
