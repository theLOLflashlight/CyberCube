using CyberCube.Physics;
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

namespace CyberCube.Levels
{
    public class EndDoor : Solid
    {
        private static Texture2D sDoorTexture;

        public struct EndDoorMaker : ISolidMaker
        {
            public Solid MakeSolid( CubeGame game, World world, Body body )
            {
                return new EndDoor( game, world, body );
            }
        }

        public static void LoadContent(ContentManager content)
        {
            sDoorTexture = content.Load<Texture2D>("Textures\\door");
        }

        public EndDoor( CubeGame game, World world, Body body )
            : base( game, world, body )
        {
        }

        public const float WIDTH = 70f;
        public const float HEIGHT = 100f;

        public string NextLevel
        {
            get; set;
        }

        public EndDoor( CubeGame game,
                        World world,
                        Vector2 pos,
                        string nextLevel = null,
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
                    new Door( nextLevel ) );

            door.IsSensor = true;

            Body.BodyType = bodyType;
            Body.CollisionCategories = categories;
        }

        public override void Draw( GameTime gameTime, SpriteBatch batch )
        {
            //mSpriteBatch.Begin( SpriteSortMode.Immediate, BlendState.Opaque );

            Vector2 position = Body.Position.ToPixels();
            Vector2 scale = new Vector2(
                WIDTH / sDoorTexture.Width,
                HEIGHT / sDoorTexture.Height );

            batch.Draw(
                sDoorTexture,
                position,
                null,
                Color.White,
                Body.Rotation,
                new Vector2(
                    sDoorTexture.Width / 2.0f,
                    sDoorTexture.Height / 2.0f ),
                scale,
                SpriteEffects.None,
                0 );

            //mSpriteBatch.End();
        }

    }
}
