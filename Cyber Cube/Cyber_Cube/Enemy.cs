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
        private Model model3D;
        private float aspectRatio;

		private VertexPositionColorTexture[] visionVertices;
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

            model3D = Game.Content.Load<Model>("Models\\enemyAlpha3D");
            aspectRatio = GraphicsDevice.Viewport.AspectRatio;

			pixel = new Texture2D( GraphicsDevice, 3, 3 );
			pixel.SetData( new[] { Color.White, Color.White, Color.White,
                                   Color.White, Color.White, Color.White,
                                   Color.White, Color.White, Color.White } );

			visionVertices = new VertexPositionColorTexture[3];
			visionVertices[0] = new VertexPositionColorTexture( new Vector3( 0, -0.01f, 0 ), Color.LightGoldenrodYellow, new Vector2( 0, 0 ) );
			visionVertices[1] = new VertexPositionColorTexture( new Vector3( 0.5f, -0.01f, 0 ), Color.LightGoldenrodYellow, new Vector2( 0.5f, 0 ) );
			visionVertices[2] = new VertexPositionColorTexture( new Vector3( 0.5f, 0.5f, 0 ), Color.LightGoldenrodYellow, new Vector2( 0.5f, 0.5f ) );
        }


		public override void Update(GameTime gameTime) 
		{
			float timeDiff = (float)gameTime.ElapsedGameTime.TotalSeconds;

			//mVelocity2d.Y -= 5f * timeDiff;
			var xScale = FreeFall ? 0f : 8f;

			//Utils.FloatApproach( ref mVelocity2d.X, 0, xScale * timeDiff );

			if (SeePlayer( Game.Player ))
			{
				xScale *= 1f;
				visionVertices[0].Color = Color.DarkRed;
				visionVertices[1].Color = Color.DarkRed;
				visionVertices[2].Color = Color.DarkRed;

				//Vector2 playerDirection = Game.Player.ComputeFacePosition() - ComputeFacePosition();
				//playerDirection.Normalize();
				//playerDirection.Y = 0;
				//facingDirection = playerDirection;
				//tickCounter = 0;
			}
			else
			{
				visionVertices[0].Color = Color.LightGoldenrodYellow;
				visionVertices[1].Color = Color.LightGoldenrodYellow;
				visionVertices[2].Color = Color.LightGoldenrodYellow;
			}

			if (tickCounter < 160)
			{
				facingDirection = Vector2.UnitX;
			}
			else if (tickCounter >= 240 && tickCounter < 400)
			{
				facingDirection = -Vector2.UnitX;
			}
			else
			{
				xScale = 0;
			}
			

			visionVertices[0].Position = WorldPosition;
			if (facingDirection == Vector2.UnitX)
			{
				visionVertices[1].Position = WorldPosition + (new Vector3( 0.5f, -0.01f, 0 ));
				visionVertices[2].Position = WorldPosition + (new Vector3( 0.5f, 0.25f, 0 ));
			}
			else
			{
				xScale *= -1;
				visionVertices[1].Position = WorldPosition + (new Vector3( -0.5f, -0.01f, 0 ));
				visionVertices[2].Position = WorldPosition + (new Vector3( -0.5f, 0.25f, 0 ));
			}
			//mVelocity2d.X += xScale * timeDiff;

			tickCounter++;
			tickCounter %= 480;


			base.Update(gameTime);
		}

		// we determine if we see the player by using the triangle vision and see if it contains the player 
		private bool SeePlayer( Player player )
		{
			Vector2 playerPosition = player.Position;
			Vector2 pointA = Cube.ComputeFacePosition( visionVertices[0].Position, CubeFace );
			Vector2 pointB = Cube.ComputeFacePosition( visionVertices[1].Position, CubeFace );
			Vector2 pointC = Cube.ComputeFacePosition( visionVertices[2].Position, CubeFace );

			// Alternative: Barycentric Technique


			return PointInTriangle(playerPosition, pointA, pointB, pointC);
		}

		private bool SameSide(Vector2 point1, Vector2 point2, Vector2 pointA, Vector2 pointB)
		{
			
			Vector3 cp1 = Vector3.Cross(new Vector3(pointB - pointA, 0), new Vector3(point1 - pointA, 0));
			Vector3 cp2 = Vector3.Cross( new Vector3( pointB - pointA, 0 ), new Vector3( point2 - pointA, 0 ) );
			return Vector3.Dot(cp1, cp2) >= 0;
		}

		private bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
		{
			return SameSide( p, a, b, c ) && SameSide( p, b, c, a ) && SameSide( p, c, a, b );
		}

		public override void Draw(GameTime gameTime) {
            // Find screen equivalent of 3D location in world
			Vector3 screenLocation = GraphicsDevice.Viewport.Project(
				this.WorldPosition,
				Cube.Effect.Projection,
				Cube.Effect.View,
				Cube.Effect.World );

			Cube.Effect.Alpha = 0.6f;
			Cube.Effect.VertexColorEnabled = true;
			Cube.Effect.LightingEnabled = false;

			RasterizerState rasterizerState = new RasterizerState();
			rasterizerState.CullMode = CullMode.None;
			GraphicsDevice.RasterizerState = rasterizerState;
            foreach (EffectPass pass in Cube.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
				GraphicsDevice.DrawUserPrimitives( PrimitiveType.TriangleList, visionVertices, 0, 1 );
            }

			Cube.Effect.LightingEnabled = true;
			Cube.Effect.VertexColorEnabled = false;
			Cube.Effect.Alpha = 1f;

			mSpriteBatch.Begin();
            mSpriteBatch.Draw(pixel, new Vector2(screenLocation.X - 1, screenLocation.Y - 1), Color.DeepSkyBlue);
		    mSpriteBatch.End();
		}
	}
}
