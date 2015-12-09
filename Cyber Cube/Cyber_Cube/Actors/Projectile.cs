using CyberCube.Actors;
using CyberCube.IO;
using CyberCube.Levels;
using CyberCube.Physics;
using CyberCube.Screens;
using CyberCube.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using FarseerPhysics.Factories;

namespace CyberCube.Actors
{
	public partial class Projectile : Actor
    {
        public const float MOVEMENT_SCALE = 120;
        private const int BULLET_SIZE = 10;
        
        private static Texture2D sTexture;

        private Vector3 mDirection;

        public new PlayScreen Screen
        {
            get {
                return (PlayScreen) base.Screen;
            }
            set {
                base.Screen = value;
            }
        }

        public Projectile( PlayScreen screen, PlayableCube cube, Vector3 worldPos, Vector3 direction, float rotation ) 
            : base ( cube.Game, screen, cube, worldPos, (Direction) rotation )
        {
            this.Visible = true;
            this.DrawOrder = 1;
            mDirection = direction;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            sTexture = new Texture2D( GraphicsDevice, BULLET_SIZE, BULLET_SIZE );
            Color[] data = new Color[BULLET_SIZE * BULLET_SIZE];
            for(int i = 0; i < data.Length; i++) {
                data[i] = Color.White;
            }
            sTexture.SetData( data );

        }

        protected override Body CreateBody( World world )
        {
            Body body = base.CreateBody( world );

            Vertices verts = PolygonTools.CreateRectangle(5.ToUnits(), 5.ToUnits());
            var bullet = FixtureFactory.AttachPolygon( verts, 1, body, "bullet" );
            
            return body;
        }

        public override void Initialize()
        {
            base.Initialize();

            Body.FixedRotation = true;
            Body.GravityScale = 0;
            Body.UseAdHocGravity = true;
            Body.AdHocGravity = Vector2.Zero;

            Body.CollisionCategories = Constants.Categories.ENEMY;
            Body.CollidesWith = Category.All ^ Constants.Categories.ENEMY;
            Body.Mass = 1000000;
        }

        public override void Update( GameTime gameTime )
        {
            float seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 velocity = Velocity.Rotate( -Rotation );

            velocity.X = mDirection.X * MOVEMENT_SCALE * seconds;
            velocity.Y = mDirection.Y * MOVEMENT_SCALE * seconds;

            Velocity = velocity.Rotate( Rotation );
            
            base.Update( gameTime );

            if (mRotated) Screen.RemoveProjectile( this );
        }
        



        public override void Draw( GameTime gameTime )
        {
            if (Cube.CurrentFace == CubeFace)
            {
                Vector3 screenLocation = GraphicsDevice.Viewport.Project(
                    CubePosition,
                    Cube.Effect.Projection,
                    Cube.Effect.View,
                    Cube.Effect.World );
                mSpriteBatch.Begin();
                mSpriteBatch.Draw(sTexture, new Vector2(
                    screenLocation.X - BULLET_SIZE/2,
                    screenLocation.Y - BULLET_SIZE/2 ), Color.White);
                mSpriteBatch.End();
            }
        }
    }   
}
