using CyberCube.Tools;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Physics
{
    public class Box : Solid
    {
        private static Texture2D mRedBox;
        private static Texture2D mWhiteBox;
        private Rectangle destRect;

        public struct BoxMaker : ISolidMaker
        {
            public float Width;
            public float Height;

            public BoxMaker(float width, float height)
            {
                Width = width;
                Height = height;
            }

            public Solid MakeSolid(CubeGame game, World world, Body body)
            {
                return new Box(game, world, body, Width, Height);
            }
        }

        public static void LoadContent(ContentManager content)
        {
            mWhiteBox = content.Load<Texture2D>("Textures\\whiteBox");
            mRedBox = content.Load<Texture2D>("Textures\\RedBox");
        }

        public Box(CubeGame game, World world, Body body, float width, float height)
            : base(game, world, body)
        {
            mWidth = width;
            mHeight = height;
        }

        private float mWidth;
        private float mHeight;

        public Box(CubeGame game,
                    World world,
                    RectangleF rec,
                    BodyType bodyType = BodyType.Static,
                    float density = 1,
                    Category categories = Constants.Categories.DEFAULT)
            : base(game, world, rec.Center.ToUnits(), 0, new BoxMaker(rec.Width, rec.Height))
        {
            mWidth = rec.Width;
            mHeight = rec.Height;

            destRect = new Rectangle((int)rec.Left, (int)rec.Bottom, (int)rec.Width, (int)rec.Height);

            FixtureFactory.AttachRectangle(
                    mWidth.ToUnits(),
                    mHeight.ToUnits(),
                    density,
                    Vector2.Zero,
                    Body,
                    new Flat());

            Body.BodyType = bodyType;
            Body.CollisionCategories = categories;
        }

        public override void Draw( GameTime gameTime, SpriteBatch batch )
        {
            Vector2 position = Body.Position.ToPixels();

            Vector2 scale = new Vector2(
                mWidth / Texture.Width,
                mHeight / Texture.Height);

            batch.Draw(
                Texture,
                position,
                null,
                SOLID_COLOR,
                Body.Rotation,
                new Vector2(
                    Texture.Width / 2.0f,
                    Texture.Height / 2.0f),
                scale,
                SpriteEffects.None,
                0);
        }

    }
}
