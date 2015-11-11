using CyberCube.Levels;
using CyberCube.Physics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Brushes
{
    public class BodyTypeBrush : IEditBrush
    {
        private EditableCube.Face mFace;

        //private Solid mFocusSolid;
        //private Vector2 mStartPos;

        public bool Started
        {
            get; private set;
        }

        public void Start( EditableCube.Face face, Vector2 mousePos, GameTime gameTime )
        {
            Solid solid = face.FindSolidAt( mousePos.ToUnits() );
            if ( solid != null )
            {
                switch ( solid.BodyType )
                {
                case BodyType.Static:
                    solid.BodyType = BodyType.Dynamic;
                    solid.Body.CollisionCategories = Category.Cat2;
                    break;
                case BodyType.Dynamic:
                    solid.BodyType = BodyType.Static;
                    solid.Body.CollisionCategories = Category.Cat1;
                    break;
                }
            }
        }

        public void Update( EditableCube.Face face, Vector2 mousePos, GameTime gameTime )
        {
            if ( !Started || face != mFace )
                return;
        }

        public void End( EditableCube.Face face, Vector2? mousePos, GameTime gameTime )
        {
            if ( !Started || face != mFace )
                return;

            Cancel();
        }

        public void Cancel()
        {
            mFace = null;
            Started = false;
        }

        public void Draw( EditableCube.Face face, SpriteBatch spriteBatch, GameTime gameTime )
        {
            if ( !Started || face != mFace )
                return;

            return;
        }
    }
}
