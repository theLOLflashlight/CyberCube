using CyberCube.Actors;
using CyberCube.Levels;
using CyberCube.Physics;
using CyberCube.Screens;
using CyberCube.Tools;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Brushes
{
    public class EnemyBrush : IEditBrush
    {
        private EditableCube.Face mFace;

        private Vector2 mStartPos;
        private Vector2 mCurrentPos;

        private Texture2D mTexture;

        protected CubeGame Game
        {
            get; set;
        }

        public EnemyBrush( CubeGame game )
        {
            Game = game;
        }

        public bool Started
        {
            get; private set;
        }

        public void Start( EditableCube.Face face, Vector2 mousePos, GameTime gameTime )
        {
            mTexture = new Texture2D( Game.GraphicsDevice, 1, 1 );
            mTexture.SetData( new[] { Color.White } );

            mFace = face;
            Started = true;
            mStartPos = mousePos;
        }

        public void Update( EditableCube.Face face, Vector2 mousePos, GameTime gameTime )
        {
            if ( !Started || face != mFace )
                return;

            mCurrentPos = EditScreen.SnapVector( mousePos, EditScreen.SNAP_SIZE / 2 );
        }

        public void End( EditableCube.Face face, Vector2? mousePos, GameTime gameTime )
        {
            if ( !Started || face != mFace )
                return;

            if ( mousePos != null )
            {
                Vector2 pos = EditScreen.SnapVector( mousePos.Value, EditScreen.SNAP_SIZE / 2 );
                face.Cube.EnemyPositions.Add( new CubePosition(
                    face.ConvertFaceToCube( pos ), 
                    face.Cube.UpDir.Angle) );
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
                Color.White,
                face.Cube.UpDir.Angle,
                new Vector2(
                    mTexture.Width,
                    mTexture.Height ) / 2,
                new Vector2( EndDoor.WIDTH, EndDoor.HEIGHT ),
                SpriteEffects.None,
                0 );
        }
    }
}
