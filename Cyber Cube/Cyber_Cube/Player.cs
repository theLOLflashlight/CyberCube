using CyberCube.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube
{
    public class Player : Actor
    {
        private Texture2D pixel;
        private Model model3D;
		private float aspectRatio;

        public Player( Cube cube, Vector3 worldPos, Direction upDir  )
            : base( cube.Game, cube, worldPos, upDir )
        {
            this.Visible = true;
            this.DrawOrder = 1;
        }

        public override void Initialize()
        {
            base.Initialize();

            pixel = new Texture2D( GraphicsDevice, 3, 3 );
            pixel.SetData( new[] { Color.White, Color.White, Color.White,
                                   Color.White, Color.White, Color.White,
                                   Color.White, Color.White, Color.White } );

            model3D = Game.Content.Load<Model>("Models\\playerAlpha3D");
			aspectRatio = GraphicsDevice.Viewport.AspectRatio;
        }

        protected override Vector3 TransformMovementTo3d( Vector2 vec2d )
        {
            var angle = UpDir.ToRadians() + CubeFace.Rotation;

            return new Vector3( vec2d, 0 )
                       .Transform( Utils.RotateOntoQ( Vector3.UnitZ, Normal ) )
                       .Rotate( Normal, angle );
        }

        public void Jump()
        {
            mVelocity2d.Y = FreeFall ? 1.5f : 2f;
        }

        public override void Update( GameTime gameTime )
        {
            var input = Game.Input;

            float timeDiff = (float) gameTime.ElapsedGameTime.TotalSeconds;

            mVelocity2d.Y -= 5f * timeDiff;
            var xScale = FreeFall ? 2 : 10;

            if ( !input.HasFocus )
            {
                //delta2d.X += input.GetAction( Action.MoveRight ).Value;
                //delta2d.X -= input.GetAction( Action.MoveLeft ).Value;
                //delta2d.Y += input.GetAction( Action.MoveUp ).Value;
                //delta2d.Y -= input.GetAction( Action.MoveDown ).Value;

                var actionRight = input.GetAction( Action.MoveRight );
                var actionLeft = input.GetAction( Action.MoveLeft );

                if ( !actionRight && !actionLeft )
                    Utils.FloatApproach( ref mVelocity2d.X, 0, xScale * timeDiff );

                mVelocity2d.X += actionRight.Value * (xScale + 1) * timeDiff;
                mVelocity2d.X -= actionLeft.Value * (xScale + 1) * timeDiff;

                if ( input.GetAction( Action.Jump ) )
                    this.Jump();

                //if ( delta2d != Vector2.Zero )
                //{
                //    var tmp = delta2d;
                //    delta2d.Normalize();
                //    delta2d.X *= Math.Abs( tmp.X );
                //    delta2d.Y *= Math.Abs( tmp.Y );
                //}
            }
            else
            {
                Utils.FloatApproach( ref mVelocity2d.X, 0, xScale * timeDiff );
            }

            mVelocity2d.X = MathHelper.Clamp( mVelocity2d.X, -1, +1 );

            base.Update( gameTime );

            Game.Camera.Target = WorldPosition;
        }

        protected override void ApplyRotation( CompassDirection dir )
        {
            base.ApplyRotation( dir );
            Cube.Rotate( dir );
        }

        public override void Draw( GameTime gameTime )
        {

            // Find screen equivalent of 3D location in world
            Vector3 screenLocation = GraphicsDevice.Viewport.Project(
                this.WorldPosition,
                Cube.Effect.Projection,
                Cube.Effect.View,
                Cube.Effect.World );

            /*
			// Below are codes for render the 3d model, didn't quite working bug-free so commented out for now
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
						Matrix.CreateTranslation( WorldPosition.X, WorldPosition.Y, 0 ) *
						Matrix.CreateScale( 0.0008f );
					if (Cube.CurrentFace.Normal == Vector3.UnitZ)
					{
						effect.World *= Matrix.CreateRotationY( MathHelper.PiOver4 );
					} 
					effect.View = Matrix.CreateLookAt( Game.Camera.Position,
						Vector3.Zero, Game.Camera.UpVector );
					effect.Projection = Matrix.CreatePerspectiveFieldOfView(
						MathHelper.ToRadians( 45.0f ), aspectRatio,
						1.0f, 10000.0f );
				}
				// Draw the mesh, using the effects set above.
				mesh.Draw();
			}

            */

			// Draw our pixel texture there
			mSpriteBatch.Begin();
			mSpriteBatch.Draw( pixel,
							   new Vector2(
								   screenLocation.X - 1,
								   screenLocation.Y - 1 ),
							   Color.Black );
			mSpriteBatch.End();

			base.Draw( gameTime );
        }

    }
}
