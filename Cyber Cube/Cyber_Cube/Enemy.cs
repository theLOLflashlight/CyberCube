using Cyber_Cube.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube {
	class Enemy : DrawableGameComponent {

		private Texture2D enemyTexture;
		private SpriteBatch mSpriteBatch;

		private Cube mCube;
		private Cyber_Cube.Cube.Face face;
		private Vector2 position;
		private Vector3 WorldPosition = Vector3.UnitZ;
		private Direction movingDirection;

		public Enemy(Cube cube, Cyber_Cube.Cube.Face face, Vector2 position)
			: base(cube.Game) {
				mCube = cube;
				this.face = face;
				this.position = position;
				this.Visible = true;
			this.DrawOrder = 2;
			movingDirection = Direction.East;
		}

		public override void Initialize() {
			base.Initialize();
			mSpriteBatch = new SpriteBatch(GraphicsDevice);

			enemyTexture = new Texture2D(GraphicsDevice, 10, 10);
			Color[] data = new Color[enemyTexture.Width * enemyTexture.Height];
			for (int i = 0; i < data.Length; i++) {
				data[i] = Color.White;
			}
			enemyTexture.SetData(data);

			WorldPosition.X = 0.5f;
			WorldPosition.Y = -0.5f;
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			var delta2d = Vector2.Zero;
			if (movingDirection == Direction.East) {
				delta2d.X += 0.5f;
			} else {
				delta2d.X -= 0.5f;
			}


			var angle = mCube.UpDir.ToRadians() + face.Orientation.ToRadians();
			var delta3d = new Vector3(delta2d, 0)
					.Transform(Utils.RotateOntoQ(Vector3.UnitZ, face.Normal))
					.Rotate(mCube.CurrentFace.Normal, angle);
			WorldPosition += (delta3d / Cube.Face.SIZE) * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 10f);

			if (mCube.CurrentFace != face.OppositeFace) {
				if (WorldPosition.X + ((float)enemyTexture.Width / Cube.Face.SIZE / 2) * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 10f) > 1) {
					movingDirection = Direction.West;
				} else if (WorldPosition.X < -1) {
					movingDirection = Direction.East;
				}
			} else {
				if (WorldPosition.X < -1) {
					movingDirection = Direction.West;
				} else if (WorldPosition.X + (enemyTexture.Width / Cube.Face.SIZE / 2) * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 10f) > 1) {
					movingDirection = Direction.East;
				}
			}
		}

		public override void Draw(GameTime gameTime) {
			base.Draw(gameTime);

			// Find screen equivalent of 3D location in world
			Vector3 screenLocation = GraphicsDevice.Viewport.Project(
				this.WorldPosition,
				mCube.Effect.Projection,
				mCube.Effect.View,
				mCube.Effect.World);

			// Draw our pixel texture there
			mSpriteBatch.Begin();
			mSpriteBatch.Draw(enemyTexture, new Vector2(screenLocation.X, screenLocation.Y), Color.Gray);
			mSpriteBatch.End();
		}
	}
}
