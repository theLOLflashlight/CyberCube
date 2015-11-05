using CyberCube.Graphics;
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
    public class PlatformBrush : IEditBrush
    {
        private EditableCube.Face mFace;

        private Line2 mLine;

        private bool ValidLine
        {
            get {
                return mLine.Length >= 1.ToPixels()
                       && (mLine.IsHorizontal || mLine.IsVertical);
            }
        }

        private Texture2D mTexture;

        protected CubeGame Game
        {
            get; set;
        }

        public PlatformBrush( CubeGame game )
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
            mLine.P0 = EditScreen.SnapVector( mousePos, 25 );
        }

        public void Update( EditableCube.Face face, Vector2 mousePos, GameTime gameTime )
        {
            if ( !Started || face != mFace )
                return;

            mLine.P1 = EditScreen.SnapVector( mousePos, 25 );

            if ( !ValidLine )
                mLine.P1 = mousePos;
        }

        public void End( EditableCube.Face face, Vector2? mousePos, GameTime gameTime )
        {
            if ( !Started || face != mFace )
                return;

            if ( ValidLine )
            {
                OneWayPlatform platform = new OneWayPlatform(
                    face.Game,
                    face.World,
                    mLine );

                platform.Body.UseAdHocGravity = true;
                platform.Body.AdHocGravity =
                    Vector2.UnitY.Rotate( face.Cube.UpDir.Angle ).Rounded()
                    * 9.8f;

                face.AddSolid( platform );
            }

            Cancel();
        }

        public void Cancel()
        {
            mFace = null;
            Started = false;
            mLine = default( Line2 );
            mTexture = null;
        }

        public void Draw( EditableCube.Face face, SpriteBatch spriteBatch, GameTime gameTime )
        {
            if ( !Started || face != mFace )
                return;

            Color color = ValidLine
                          ? new Color( 0, 0, 0, 255 )
                          : new Color( 0, 0, 0, 128 );

            spriteBatch.DrawLine( mLine, mTexture, color, 10 );
        }
    }
}
