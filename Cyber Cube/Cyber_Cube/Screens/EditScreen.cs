using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CyberCube.Levels;
using Microsoft.Xna.Framework.Graphics;
using CyberCube.Physics;
using FarseerPhysics.Dynamics;

namespace CyberCube.Screens
{
    /// <summary>
    /// Screen which contains a cube which can be edited, but not played.
    /// </summary>
    public class EditScreen : CubeScreen
    {

        public class HandBrush : EditableCube.IBrush
        {
            private EditableCube.Face mFace;
            private Solid mFocusSolid;
            private Vector2 mMouseUnitPos;

            public bool Started
            {
                get; private set;
            }

            public void Start( EditableCube.Face face, Vector2 mousePos, GameTime gameTime )
            {
                Vector2 clickPos = mousePos * Physics.Constants.PIXEL_TO_UNIT;

                mFocusSolid = face.FindSolidAt( ref clickPos );
                if ( mFocusSolid != null )
                {
                    mFace = face;
                    Started = true;
                    mMouseUnitPos = clickPos;
                    mFocusSolid.Body.Enabled = false;
                }
            }

            public void Update( EditableCube.Face face, Vector2 mousePos, GameTime gameTime )
            {
                if ( face != mFace )
                    return;

                Vector2 worldPos = mousePos * Physics.Constants.PIXEL_TO_UNIT;
                Vector2 delta = worldPos - mMouseUnitPos;

                mFocusSolid.Body.Position += delta;
                mMouseUnitPos = worldPos;
            }

            public void End( EditableCube.Face face, Vector2? mousePos, GameTime gameTime )
            {
                if ( face != mFace )
                    return;

                Cancel();
            }

            public void Cancel()
            {
                Started = false;

                if ( mFocusSolid != null )
                    mFocusSolid.Body.Enabled = true;
                mFocusSolid = null;
            }

            public void Draw( EditableCube.Face face, SpriteBatch spriteBatch, GameTime gameTime )
            {
                if ( face != mFace )
                    return;

                return;
            }
        }

        public class RectangleBrush : EditableCube.IBrush
        {
            private EditableCube.Face mFace;

            private Rectangle? mTentativeRec;
            private Vector2 mMousePos;

            private Texture2D mTexture;

            public RectangleBrush( Texture2D texture )
            {
                mTexture = texture;
            }

            public bool Started
            {
                get; private set;
            }

            public void Start( EditableCube.Face face, Vector2 mousePos, GameTime gameTime )
            {
                mFace = face;
                Started = true;
                mMousePos = mousePos;
            }

            public void Update( EditableCube.Face face, Vector2 mousePos, GameTime gameTime )
            {
                if ( face != mFace )
                    return;

                Vector2 pos = mMousePos;

                int x = (int) Math.Min( pos.X, mousePos.X );
                int y = (int) Math.Min( pos.Y, mousePos.Y );

                int w = (int) Math.Abs( mousePos.X - pos.X );
                int h = (int) Math.Abs( mousePos.Y - pos.Y );

                if ( w > 0.5 * Physics.Constants.UNIT_TO_PIXEL
                     && h > 0.5 * Physics.Constants.UNIT_TO_PIXEL )
                    mTentativeRec = new Rectangle( x, y, w, h );
                else
                    mTentativeRec = null;
            }

            public void End( EditableCube.Face face, Vector2? mousePos, GameTime gameTime )
            {
                if ( face != mFace )
                    return;

                if ( mTentativeRec != null )
                {
                    var rec = mTentativeRec.Value;

                    Box box = new Box(
                        face.Game,
                        face.World,
                        rec,
                        BodyType.Static,
                        rec.Width * rec.Height * 0.003f,
                        Category.Cat2 );

                    box.Body.UseAdHocGravity = true;
                    box.Body.AdHocGravity =
                        Vector2.UnitY.Rotate( face.Cube.UpDir.Angle ).Rounded()
                        * 9.8f;

                    face.AddSolid( box );
                }

                Started = false;
                mTentativeRec = null;
            }

            public void Cancel()
            {
                Started = false;
                mTentativeRec = null;
            }

            public void Draw( EditableCube.Face face, SpriteBatch spriteBatch, GameTime gameTime )
            {
                if ( face != mFace )
                    return;

                if ( mTentativeRec != null )
                    spriteBatch.Draw( mTexture, mTentativeRec.Value, new Color( 0, 0, 0, 128 ) );
            }
        }

        #region Boilerplate
        /// <summary>
        /// Hides the base Cube property, exposing the methods of the EditableCube class.
        /// </summary>
        public new EditableCube Cube
        {
            get {
                return (EditableCube) base.Cube;
            }
            protected set {
                base.Cube = value;
            }
        }

        /// <summary>
        /// Creates a new EditScreen.
        /// </summary>
        /// <param name="game">Game the EditScreen should be associated with.</param>
        public EditScreen( CubeGame game )
            : this( game, new EditableCube( game ) )
        {
        }

        /// <summary>
        /// Creates a new EditScreen.
        /// </summary>
        /// <param name="game">Game the EditScreen should be associated with.</param>
        /// <param name="editCube">EditableCube the screen should display.</param>
        public EditScreen( CubeGame game, EditableCube editCube )
            : base( game, editCube )
        {
        }
        #endregion

        public override void Initialize()
        {
            base.Initialize();
            Texture2D pixel = new Texture2D( GraphicsDevice, 1, 1 );
            pixel.SetData( new[] { Color.White } );

            Cube.Brush = new HandBrush();
            //Cube.Brush = new RectangleBrush( pixel );
        }

        public override void Update( GameTime gameTime )
        {
            var input = Game.Input;

            if ( !input.HasFocus && input.GetAction( Action.ToggleCubeMode ) )
                TestLevel();

            base.Update( gameTime );
        }

        private void TestLevel()
        {
            PlayableCube playCube = Cube.GeneratePlayableCube();
            ScreenManager.PushScreen( new PlayScreen( Game, playCube ) );
        }

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );
        }
    }
}
