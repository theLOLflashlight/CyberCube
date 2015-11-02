using CyberCube.Levels;
using CyberCube.Physics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Screens.Brushes
{
    public class HandBrush : IEditBrush
    {
        private EditableCube.Face mFace;

        private Solid mFocusSolid;
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
            Solid focusSolid = face.FindSolidAt( mousePos.ToUnits() );

            if ( focusSolid != mFocusSolid )
                Cancel();

            if ( focusSolid != null )
            {
                mFace = face;
                Started = true;
                mStartPos = mousePos;
                mFocusSolid = focusSolid;
                mOriginalPos = mFocusSolid.Position;
                Game.Input.Focus = mFocusSolid;
            }
        }

        public void Update( EditableCube.Face face, Vector2 mousePos, GameTime gameTime )
        {
            if ( face != mFace )
                return;

            var input = Game.Input;

            if ( mFocusSolid != null )
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

                mFocusSolid.Position += delta.Rotate( -face.Cube.UpDir.Angle );

                if ( input.Keyboard_WasAnyKeyPressed( Keys.Delete, Keys.Back ) )
                {
                    face.RemoveSolid( mFocusSolid );
                    Cancel();
                }
            }

            if ( !Started )
                return;

            mFocusSolid.Position = mOriginalPos + (mousePos.ToUnits() - mStartPos.ToUnits());

            if ( input.IsShiftDown() )
            {
                Vector2 snappedPos = mFocusSolid.Position.ToPixels();
                snappedPos /= 25;
                snappedPos = snappedPos.Rounded();
                snappedPos *= 25;

                mFocusSolid.Position = snappedPos.ToUnits();
            }
            else
            {
                mFocusSolid.Position = mFocusSolid.Position.ToPixels().Rounded().ToUnits();
            }
        }

        public void End( EditableCube.Face face, Vector2? mousePos, GameTime gameTime )
        {
            if ( face != mFace )
                return;

            if ( !Started )
                Cancel();

            Started = false;
            mStartPos = Vector2.Zero;
        }

        public void Cancel()
        {
            mFace = null;
            Started = false;

            if ( Game.Input.CheckFocus( mFocusSolid ) )
                Game.Input.Focus = null;

            mFocusSolid = null;
            mStartPos = Vector2.Zero;
        }

        public void Draw( EditableCube.Face face, SpriteBatch spriteBatch, GameTime gameTime )
        {
            if ( face != mFace )
                return;

#if WINDOWS
            if ( mFocusSolid != null )
            {
                Matrix proj = Matrix.CreateOrthographicOffCenter(
                        0,
                        Cube.Face.WIDTH.ToUnits(),
                        Cube.Face.HEIGHT.ToUnits(),
                        0,
                        0,
                        1 );

                face.DebugView.BeginCustomDraw( proj, Matrix.Identity );

                double t = (gameTime.TotalGameTime.TotalSeconds % 4 / 2) - 1;
                byte val = (byte) (150 + Math.Abs( t * 105 ));
                Color color = new Color( val, val, val, 255 );

                Body b = mFocusSolid.Body;
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
