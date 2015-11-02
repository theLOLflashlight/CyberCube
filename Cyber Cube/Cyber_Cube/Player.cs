using CyberCube.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using CyberCube.Screens;
using CyberCube.Levels;
using CyberCube.Graphics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Collision;
using CyberCube.Physics;

namespace CyberCube
{
    public class Player : Actor
    {
        private Texture2D pixel;
        private Model model3D;
		private float aspectRatio;

        public Player( PlayScreen screen, PlayableCube cube, Vector3 worldPos, Direction upDir )
            : base( cube.Game, screen, cube, worldPos, upDir )
        {
            this.Visible = true;
            this.DrawOrder = 1;
        }

        public override void Reset( Vector3 worldPos, Direction upDir )
        {
            base.Reset( worldPos, upDir );
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            model3D = Game.Content.Load<Model>( "Models\\playerAlpha3D" );
        }

        private Fixture mFeet;

        protected override Body CreateBody( World world )
        {
            Body body = BodyFactory.CreateRectangle(
                CubeFace.World,
                25.ToUnits(),
                50.ToUnits(),
                1,
                ComputeFacePosition().ToUnits() );

            mFeet = FixtureFactory.AttachRectangle(
                20.ToUnits(),
                10.ToUnits(),
                1,
                new Vector2( 0, 25 ).ToUnits(),
                body );
            mFeet.IsSensor = true;

            body.BodyType = BodyType.Dynamic;
            body.Rotation = Rotation;

            return body;
        }

        public override void Initialize()
        {
            base.Initialize();

            Body.FixedRotation = true;
            Body.GravityScale = 20f;
            Body.UseAdHocGravity = true;
            Body.AdHocGravity = Vector2.UnitY;
            Body.CollisionCategories = Category.Cat2;
            Body.Mass = 68;

            Body.OnCollision += Feet_OnCollision;

            pixel = new Texture2D( GraphicsDevice, 3, 3 );
            pixel.SetData( new[] { Color.White, Color.White, Color.White,
                                   Color.White, Color.White, Color.White,
                                   Color.White, Color.White, Color.White } );

			aspectRatio = GraphicsDevice.Viewport.AspectRatio;
        }

        private bool Feet_OnCollision( Fixture fixtureA, Fixture fixtureB, Contact contact )
        {
            if ( contact.IsTouching && !fixtureB.IsSensor
                 && contact.Manifold.Type == ManifoldType.FaceA
                 && contact.Manifold.PointCount >= 1 )
            {
                Vector2 groundNormal = contact.Manifold.LocalNormal;
                float rotation = (float) Math.Atan2( groundNormal.Y, groundNormal.X ) + MathHelper.PiOver2;

                float wrappedRotation = MathHelper.WrapAngle( rotation );
                float wrappedBodyRotation = MathHelper.WrapAngle( Body.Rotation );

                if ( Math.Abs( wrappedBodyRotation - wrappedRotation ) <= MathHelper.PiOver4 )
                    Rotation = rotation;

                return true;
            }
            return contact.IsTouching;
        }

        public void Jump( ref Vector2 velocity )
        {
            velocity.Y = FreeFall ? -10f : -20f;
        }

        public void JumpEnd( ref Vector2 velocity )
        {
            if ( FreeFall && velocity.Y < 0 )
                velocity.Y *= 0.6f;
        }

        public override void Update( GameTime gameTime )
        {
            var input = Game.Input;

            if ( !input.HasFocus )
            {
                if ( input.GetAction( Action.RotateClockwise ) )
                    UpDir = Direction.FromAngle( -Rotation + (MathHelper.PiOver4 + 0.0001f) );

                if ( input.GetAction( Action.RotateAntiClockwise ) )
                    UpDir = Direction.FromAngle( -Rotation - (MathHelper.PiOver4 + 0.0001f) );
            }

            float timeDiff = (float) gameTime.ElapsedGameTime.TotalSeconds;

            var xScale = (FreeFall ? 20 : 100);

            Vector2 velocity = Velocity.Rotate( -Body.Rotation );

            if ( !input.HasFocus )
            {
                var actionRight = input.GetAction( Action.MoveRight );
                var actionLeft = input.GetAction( Action.MoveLeft );

                if ( !actionRight && !actionLeft )
                    Utils.Lerp( ref velocity.X, 0, xScale * timeDiff );

                velocity.X += actionRight.Value * (xScale + 1) * timeDiff;
                velocity.X -= actionLeft.Value * (xScale + 1) * timeDiff;

                if ( input.GetAction( Action.Jump ) )
                    this.Jump( ref velocity );

                if ( input.GetAction( Action.JumpEnd ) )
                    this.JumpEnd( ref velocity );
            }
            else
            {
                Utils.Lerp( ref velocity.X, 0, xScale * timeDiff );
            }

            velocity.X = MathHelper.Clamp( velocity.X, -5, +5 );

            Velocity = velocity.Rotate( Body.Rotation );

            base.Update( gameTime );

            Screen.Camera.Target = WorldPosition;
            Screen.Camera.AnimateUpVector( CubeFace.UpVec.Rotate( Normal, -Body.Rotation ), 1 );
        }

        protected override void ApplyRotation( CompassDirection dir )
        {
            base.ApplyRotation( dir );
            Body.OnCollision += Feet_OnCollision;
            Cube.Rotate( dir );
        }

        public override void Draw( GameTime gameTime )
        {
            //if ( Cube.Mode == Cube.CubeMode.Play )
            //{
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
						    Matrix.CreateTranslation( WorldPosition.X, WorldPosition.Y, WorldPosition.Z ) *
						    Matrix.CreateScale( 0.0008f );

                        Matrix m = Vector3.UnitY.RotateOntoM( CubeFace.UpVec )
                                   * Matrix.CreateFromAxisAngle( CubeFace.Normal, -Body.Rotation );

                        effect.World *= m;

					    effect.View = Matrix.CreateLookAt( Screen.Camera.Position,
						    Vector3.Zero, Screen.Camera.UpVector );
					    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
						    MathHelper.ToRadians( 45.0f ), aspectRatio,
						    1.0f, 10000.0f );
				    }
				    // Draw the mesh, using the effects set above.
				    mesh.Draw();
			    }
            //}

            // Find screen equivalent of 3D location in world
            /*Vector3 screenLocation = GraphicsDevice.Viewport.Project(
                this.WorldPosition,
                Cube.Effect.Projection,
                Cube.Effect.View,
                Cube.Effect.World );

            // Draw our pixel texture there
            mSpriteBatch.Begin();
            mSpriteBatch.Draw( pixel,
                               new Vector2(
                                   screenLocation.X - 1,
                                   screenLocation.Y - 1 ),
                               FreeFall ? Color.Black : Color.White );

            mSpriteBatch.End();*/
        }

    }
}
