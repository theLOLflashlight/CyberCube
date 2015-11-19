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

namespace CyberCube.Brushes
{
    public class EndDoorBrush : IEditBrush
    {
        private EditableCube.Face mFace;

        private Vector2 mStartPos;
        private Vector2 mCurrentPos;

        private Texture2D mTexture;
        private string mNextLevel;

        protected CubeGame Game
        {
            get; set;
        }

        public EndDoorBrush( CubeGame game, string nextLevel = null )
        {
            Game = game;
            mNextLevel = nextLevel;
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

            mCurrentPos = mousePos;
        }

        public void End( EditableCube.Face face, Vector2? mousePos, GameTime gameTime )
        {
            if ( !Started || face != mFace )
                return;

            if ( mousePos != null )
            {
                EndDoor door = new EndDoor(
                    face.Game,
                    face.World,
                    mousePos.Value,
                    mNextLevel );

                door.Body.UseAdHocGravity = true;
                door.Body.AdHocGravity =
                    Vector2.UnitY.Rotate( face.Cube.UpDir.Angle ).Rounded()
                    * 9.8f;

                face.AddSolid( door );
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
                0,//Body.Rotation,
                new Vector2(
                    mTexture.Width,
                    mTexture.Height ) / 2,
                new Vector2( EndDoor.WIDTH, EndDoor.HEIGHT ),
                SpriteEffects.None,
                0 );
        }
    }
}
