using CyberCube.IO;
using CyberCube.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube {
	public class Enemy : Actor {

        private Model model3D;
        private Vector2 movingDirection = Vector2.UnitY;
        

		public Enemy( CubeScreen screen, Cube cube, Vector3 worldPos, Direction upDir )
			: base(cube.Game, screen, cube, worldPos, upDir) 
		{
			this.Visible = true;
			this.DrawOrder = 2;
		}

		public override void Initialize() {
			base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            model3D = Game.Content.Load<Model>( "Models\\enemyAlpha3D" );
        }


        public override void Update(GameTime gameTime) 
		{
			base.Update(gameTime);

            float moveScale = 0.2f;
            float stepAhead = 60f;
            float timeDiff = (float) gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 position = Transform3dTo2d(mWorldPosition);
            Vector2 newPosition = position - Vector2.Multiply(movingDirection, moveScale * timeDiff);
            
            Vector2 futurePosition = position - Vector2.Multiply(movingDirection, moveScale * timeDiff * stepAhead);

            if (futurePosition.X < -1 || futurePosition.X > 1) {
                newPosition = position;
                movingDirection *= -Vector2.UnitX;
            }

            if (futurePosition.Y < -1 || futurePosition.Y > 1) {
                newPosition = position;
                movingDirection *= -Vector2.UnitY;
            }

            mWorldPosition = Transform2dTo3d(newPosition);

            // Need to reference Player class to set collision


        }

		public override void Draw(GameTime gameTime) {

            Matrix[] transforms = (new Matrix[model3D.Bones.Count]);
			model3D.CopyAbsoluteBoneTransformsTo( transforms );

			// Draw the model. A model can have multiple meshes, so loop.
			foreach (ModelMesh mesh in model3D.Meshes)
			{
				// This is where the mesh orientation is set, as well 
				// as our camera and projection.
				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.EnableDefaultLighting();
					effect.World = transforms[mesh.ParentBone.Index] * 
                        Matrix.CreateScale( 0.0008f );

                    Matrix m = Vector3.UnitY.RotateOntoM( CubeFace.UpVec )
                                   * Matrix.CreateFromAxisAngle( CubeFace.Normal, -Rotation );
                    effect.World *= m;

                    effect.World *= Matrix.CreateTranslation( WorldPosition );
                    
					effect.View = Screen.Camera.View;
					effect.Projection = Cube.Effect.Projection;
				}
				// Draw the mesh, using the effects set above.
				mesh.Draw();
			}
		}
	}
}
