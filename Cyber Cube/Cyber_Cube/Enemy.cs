using CyberCube.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube {
	public class Enemy : Actor {

		private Texture2D pixel;
        private VertexPositionNormalTexture[] vertices;
		private int tickCounter;
        private Vector2 facingDirection;

		public Enemy( Cube cube, Vector3 worldPos, Direction upDir )
			: base(cube.Game, cube, worldPos, upDir) 
		{
			this.Visible = true;
			this.DrawOrder = 2;
		}

		public override void Initialize() {
			base.Initialize();

			pixel = new Texture2D( GraphicsDevice, 3, 3 );
			pixel.SetData( new[] { Color.White, Color.White, Color.White,
                                   Color.White, Color.White, Color.White,
                                   Color.White, Color.White, Color.White } );

            vertices = new VertexPositionNormalTexture[3];
            vertices[0] = new VertexPositionNormalTexture(new Vector3(0, 0, 1), Vector3.UnitZ, new Vector2(0, 0));
            vertices[1] = new VertexPositionNormalTexture(new Vector3(0.5f, 0, 1), Vector3.UnitZ, new Vector2(0.5f, 0));
            vertices[2] = new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 1), Vector3.UnitZ, new Vector2(0.5f, 0.5f));
        }

		protected override Vector3 TransformMovementTo3d( Vector2 vec2d )
		{
			var angle = UpDir.ToRadians() + CubeFace.Rotation;

			return new Vector3( vec2d, 0 )
					   .Transform( Utils.RotateOntoQ( Vector3.UnitZ, Normal ) )
					   .Rotate( Normal, angle );
		}

		public override void Update(GameTime gameTime) 
		{
			float timeDiff = (float)gameTime.ElapsedGameTime.TotalSeconds;

			mVelocity2d.Y -= 5f * timeDiff;
			var xScale = FreeFall ? 0f : 8f;
			Utils.FloatApproach( ref mVelocity2d.X, 0, xScale * timeDiff );

			if (tickCounter < 160)
			{
				mVelocity2d.X += xScale * timeDiff;
                facingDirection = Vector2.UnitX;
			}
			else if (tickCounter >= 240 && tickCounter < 400)
			{
				mVelocity2d.X -= xScale * timeDiff;
                facingDirection = -Vector2.UnitX;
			}
			tickCounter++;
			tickCounter %= 480;

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime) {
            // Find screen equivalent of 3D location in world
			Vector3 screenLocation = GraphicsDevice.Viewport.Project(
				this.WorldPosition,
				Cube.Effect.Projection,
				Cube.Effect.View,
				Cube.Effect.World );

            //RasterizerState rasterizerState = new RasterizerState();
            //rasterizerState.CullMode = CullMode.None;
            //GraphicsDevice.RasterizerState = rasterizerState;

            //foreach (EffectPass pass in Cube.Effect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 1);
            //}

			mSpriteBatch.Begin();
            mSpriteBatch.Draw(pixel, new Vector2(screenLocation.X - 1, screenLocation.Y - 1), Color.DeepSkyBlue);
		    mSpriteBatch.End();

            base.Draw(gameTime);
		}
	}
}
