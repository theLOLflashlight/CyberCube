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
    public class QuarterpipeBrush : IEditBrush
    {
        private EditableCube.Face mFace;

        private Vector2 mStartPos;
        private Vector2 mCurrentPos;

        private Texture2D mTexture;

        protected CubeGame Game
        {
            get; set;
        }

        public QuarterpipeBrush( CubeGame game )
        {
            Game = game;
        }

        public bool Started
        {
            get; private set;
        }

        public void Start( EditableCube.Face face, Vector2 mousePos, GameTime gameTime )
        {
            mTexture = Quarterpipe.CreateQuarterpipeTexture(
                Game.GraphicsDevice,
                100,
                new Color( 255, 255, 255, 128 ) );

            mFace = face;
            Started = true;
            mStartPos = mousePos;
        }

        public void Update( EditableCube.Face face, Vector2 mousePos, GameTime gameTime )
        {
            if ( !Started || face != mFace )
                return;

            mCurrentPos = mousePos;
        }

        public void End( EditableCube.Face face, Vector2? mousePos, GameTime gameTime )
        {
            if ( !Started || face != mFace )
                return;

            if ( mousePos != null )
            {
                Quarterpipe qpipe = new Quarterpipe(
                    face.Game,
                    face.World,
                    100,
                    mousePos.Value,
                    Quarterpipe.Type.SE,
                    BodyType.Static,
                    ((100 * 100) - ((MathHelper.Pi * 100 * 100) / 4)) * 0.01f,
                    Category.Cat2 );

                qpipe.Body.UseAdHocGravity = true;
                qpipe.Body.AdHocGravity =
                    Vector2.UnitY.Rotate( face.Cube.UpDir.Angle ).Rounded()
                    * 9.8f;

                face.AddSolid( qpipe );
            }

            Cancel();
        }

        public void Cancel()
        {
            mFace = null;
            Started = false;
            mStartPos = Vector2.Zero;
            mCurrentPos = Vector2.Zero;
            mTexture = null;
        }

        public void Draw( EditableCube.Face face, SpriteBatch spriteBatch, GameTime gameTime )
        {
            if ( !Started || face != mFace )
                return;

            spriteBatch.Draw(
                mTexture,
                mCurrentPos,
                null,
                Color.Black,
                0,//Body.Rotation,
                new Vector2(
                    mTexture.Width,
                    mTexture.Height ) / 2,
                Vector2.One,
                SpriteEffects.None,
                0 );
        }
    }
}
