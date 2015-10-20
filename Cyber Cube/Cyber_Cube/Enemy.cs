using Cyber_Cube.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube {
	class Enemy : DrawableGameComponent {

		public Vector3 WorldPosition = Vector3.UnitZ;
		private Cube mCube;
		protected Cube.Face CurrentFace {
			get {
				return mCube.CurrentFace;
			}
		}

		protected Texture2D pixel;
		protected SpriteBatch mSpriteBatch;


		private Cyber_Cube.Cube.Face face;
		private Vector2 position;
		private int movementIndex = 0;
		private Cyber_Cube.Action[] movementPatterns = new[] { 
			Action.MoveDown, Action.MoveDown, Action.MoveDown, Action.MoveDown, 
			Action.MoveLeft, Action.MoveLeft, Action.MoveLeft, Action.MoveLeft, 
			Action.MoveUp, Action.MoveUp, Action.MoveUp, Action.MoveUp, 
			Action.MoveRight, Action.MoveRight, Action.MoveRight, Action.MoveRight };

		public Enemy(Cube cube, Cyber_Cube.Cube.Face face, Vector2 position)
			: base(cube.Game) {
				mCube = cube;
				this.face = face;
				this.position = position;
				this.Visible = true;
				this.DrawOrder = 1;
		}

		public override void Initialize() {
			base.Initialize();

			pixel = new Texture2D(GraphicsDevice, 2, 2);
			pixel.SetData(new[] { Color.White, Color.White, Color.White, Color.White });

			mSpriteBatch = new SpriteBatch(GraphicsDevice);
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			var delta2d = Vector2.Zero;
			switch (movementPatterns[movementIndex]) {
				case Action.MoveRight:
					delta2d.X += 1.0f;
					break;
				case Action.MoveLeft:
					delta2d.X -= 1.0f;
					break;
				case Action.MoveUp:
					delta2d.Y += 1.0f;
					break;
				case Action.MoveDown:
					delta2d.Y -= 1.0f;
					break;
			}

			movementIndex++;
			movementIndex = movementIndex % movementPatterns.Length;

			Vector3 delta3d = Transform2dTo3d(delta2d);
			WorldPosition += (delta3d / Cube.Face.SIZE) * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 10f);
		}

		public Vector3 Transform2dTo3d(Vector2 vec2d) {
			var angle = mCube.UpDir.ToRadians() + face.Rotation;

			return new Vector3(vec2d, 0)
					   .Transform(Utils.RotateOntoQ(Vector3.UnitZ, face.Normal))
					   .Rotate(face.Normal, angle);
		}

		public override void Draw(GameTime gameTime) {
			base.Draw(gameTime);

			if (CurrentFace != face) {
				return;
			}

			// Find screen equivalent of 3D location in world
			Vector3 screenLocation = GraphicsDevice.Viewport.Project(
				this.WorldPosition,
				mCube.Effect.Projection,
				mCube.Effect.View,
				mCube.Effect.World);

			// Draw our pixel texture there
			mSpriteBatch.Begin();

			mSpriteBatch.Draw(pixel,
							   new Vector2(
								   screenLocation.X,
								   screenLocation.Y),
							   Color.Black);

			mSpriteBatch.End();
		}
	}
}
