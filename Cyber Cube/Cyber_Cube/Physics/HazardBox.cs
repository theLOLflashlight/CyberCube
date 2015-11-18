using CyberCube.Tools;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Physics
{
    public class HazardBox : Solid
    {
        public struct HazardBoxMaker : ISolidMaker
        {
            public float Width;
            public float Height;

            public HazardBoxMaker( float width, float height )
            {
                Width = width;
                Height = height;
            }

            public Solid MakeSolid( CubeGame game, World world, Body body )
            {
                return new HazardBox( game, world, body, Width, Height );
            }
        }

        public HazardBox( CubeGame game, World world, Body body, float width, float height )
            : base( game, world, body )
        {
            mWidth = width;
            mHeight = height;
        }

        private float mWidth;
        private float mHeight;

        public HazardBox( CubeGame game,
                          World world,
                          RectangleF rec,
                          BodyType bodyType = BodyType.Static,
                          float density = 1,
                          Category categories = Constants.Categories.DEFAULT,
                          Category killCategories = Constants.Categories.ACTORS )
            : base( game, world, rec.Center.ToUnits(), 0, new HazardBoxMaker( rec.Width, rec.Height ) )
        {
            mWidth = rec.Width;
            mHeight = rec.Height;

            Fixture box = FixtureFactory.AttachRectangle(
                    mWidth.ToUnits(),
                    mHeight.ToUnits(),
                    density,
                    Vector2.Zero,
                    Body,
                    new Flat() );

            Fixture killBox = box.CloneOnto( Body );
            killBox.IsSensor = true;
            killBox.UserData = new Hazard( "killbox" );

            Body.BodyType = bodyType;
            Body.CollisionCategories = categories;

            killBox.CollidesWith = killCategories;
            box.CollidesWith ^= killCategories;
        }

        public override void Draw( GameTime gameTime )
        {
            Vector2 position = Body.Position.ToPixels();
            Vector2 scale = new Vector2(
                mWidth / Texture.Width,
                mHeight / Texture.Height );

            mSpriteBatch.Begin( SpriteSortMode.Immediate, BlendState.AlphaBlend );

            mSpriteBatch.Draw(
                Texture,
                position,
                null,
                new Color( 255, 0, 0, 128 ),
                Body.Rotation,
                new Vector2(
                    Texture.Width / 2.0f,
                    Texture.Height / 2.0f ),
                scale,
                SpriteEffects.None,
                0 );

            mSpriteBatch.End();
        }

    }
}
