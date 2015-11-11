using CyberCube.Physics;
using CyberCube.Tools;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Levels
{
    public class EndDoor : Solid
    {
        public struct EndDoorMaker : ISolidMaker
        {
            public Solid MakeSolid( CubeGame game, World world, Body body )
            {
                return new EndDoor( game, world, body );
            }
        }

        public EndDoor( CubeGame game, World world, Body body )
            : base( game, world, body )
        {
        }

        public const float WIDTH = 70f;
        public const float HEIGHT = 100f;

        public EndDoor( CubeGame game,
                        World world,
                        Vector2 pos,
                        BodyType bodyType = BodyType.Static,
                        float density = 1,
                        Category categories = Category.Cat1 )
            : base( game, world, pos.ToUnits(), 0, new EndDoorMaker() )
        {
            var door = FixtureFactory.AttachRectangle(
                    WIDTH.ToUnits(),
                    HEIGHT.ToUnits(),
                    density,
                    Vector2.Zero,
                    Body,
                    new SolidDescriptor( "end_door" ) );

            door.IsSensor = true;

            Body.BodyType = bodyType;
            Body.CollisionCategories = categories;
        }

        public override void Draw( GameTime gameTime )
        {
            mSpriteBatch.Begin( SpriteSortMode.Immediate, BlendState.Opaque );

            Vector2 position = Body.Position.ToPixels();
            Vector2 scale = new Vector2(
                WIDTH / Texture.Width,
                HEIGHT / Texture.Height );

            mSpriteBatch.Draw(
                Texture,
                position,
                null,
                Color.White,
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
