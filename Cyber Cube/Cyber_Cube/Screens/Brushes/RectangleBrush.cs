using CyberCube.Levels;
using CyberCube.Physics;
using CyberCube.Tools;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Screens.Brushes
{
    public class RectangleBrush : IEditBrush
    {
        private EditableCube.Face mFace;

        private RectangleF mRec;
        private Vector2 mStartPos;

        private bool ValidRec
        {
            get {
                return mRec.Width > 0.5.ToPixels()
                       && mRec.Height > 0.5.ToPixels();
            }
        }

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
            mStartPos = mousePos;
        }

        public void Update( EditableCube.Face face, Vector2 mousePos, GameTime gameTime )
        {
            if ( !Started || face != mFace )
                return;

            mRec.X = Math.Min( mStartPos.X, mousePos.X );
            mRec.Y = Math.Min( mStartPos.Y, mousePos.Y );

            mRec.Width = Math.Abs( mStartPos.X - mousePos.X );
            mRec.Height = Math.Abs( mStartPos.Y - mousePos.Y );
        }

        public void End( EditableCube.Face face, Vector2? mousePos, GameTime gameTime )
        {
            if ( !Started || face != mFace )
                return;

            if ( ValidRec )
            {
                Box box = new Box(
                    face.Game,
                    face.World,
                    mRec,
                    BodyType.Static,
                    mRec.Width * mRec.Height * 0.01f,
                    Category.Cat2 );

                box.Body.UseAdHocGravity = true;
                box.Body.AdHocGravity =
                    Vector2.UnitY.Rotate( face.Cube.UpDir.Angle ).Rounded()
                    * 9.8f;

                face.AddSolid( box );
            }

            Cancel();
        }

        public void Cancel()
        {
            mFace = null;
            Started = false;
            mRec = Rectangle.Empty;
            mStartPos = Vector2.Zero;
        }

        public void Draw( EditableCube.Face face, SpriteBatch spriteBatch, GameTime gameTime )
        {
            if ( !Started || face != mFace )
                return;

            Color color = ValidRec
                          ? new Color( 0, 0, 0, 255 )
                          : new Color( 0, 0, 0, 128 );

            spriteBatch.Draw( mTexture, (Rectangle) mRec, color );
        }
    }
}
