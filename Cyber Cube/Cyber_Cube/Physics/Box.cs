﻿using CyberCube.Tools;
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
    public class Box : Solid
    {
        protected float mWidth;
        protected float mHeight;

        public Box( CubeGame game,
                    World world,
                    RectangleF rec,
                    BodyType bodyType = BodyType.Static,
                    float mass = 1,
                    Category categories = Category.Cat1 )
            : base( game, world )
        {
            mWidth = rec.Width;
            mHeight = rec.Height;

            Body = BodyFactory.CreateBody( world, rec.Center.ToUnits() );

            FixtureFactory.AttachRectangle(
                    mWidth.ToUnits(),
                    mHeight.ToUnits(),
                    1,
                    Vector2.Zero,
                    Body,
                    new Flat() );

            Body.BodyType = bodyType;
            Body.Mass = mass;
            Body.CollisionCategories = categories;
        }

        public override void Draw( GameTime gameTime )
        {
            mSpriteBatch.Begin( SpriteSortMode.Immediate, BlendState.Opaque );

            Vector2 position = Body.Position.ToPixels();
            Vector2 scale = new Vector2(
                mWidth / Texture.Width,
                mHeight / Texture.Height );

            mSpriteBatch.Draw(
                Texture,
                position,
                null,
                BodyType == BodyType.Static ? Color.Black : Color.White,
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
