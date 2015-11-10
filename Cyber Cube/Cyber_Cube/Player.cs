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
using CyberCube.Tools;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Collision;
using CyberCube.Physics;
using FarseerPhysics.Common;
using CyberCube.Tools;

namespace CyberCube
{
    public class Player : Actor
    {
        private Texture2D pixel;
        private Model model3D;
		private float aspectRatio;

        private AnimatedVariable<float, float> mModelRotation;

        public Player( PlayScreen screen, PlayableCube cube, Vector3 worldPos, Direction upDir )
            : base( cube.Game, screen, cube, worldPos, upDir )
        {
            this.Visible = true;
            this.DrawOrder = 1;

            mModelRotation = new AnimatedVariable<float, float>(
                (f0, f1, step) => {
                    var diff = MathHelper.WrapAngle( f1 - f0 );
                    return f0.Lerp( f0 + diff, step );
                } );
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

        private Fixture mTorso;
        private Fixture mFeet;

        public int mNumFootContacts;

        public override bool FreeFall
        {
            get {
                return mNumFootContacts == 0;
            }
        }

        protected override Body CreateBody( World world )
        {
            Body body = base.CreateBody( world );

            Vertices verts = PolygonTools.CreateRoundedRectangle(
                25.ToUnits(),
                50.ToUnits(),
                5.ToUnits(),
                5.ToUnits(),
                0 );

            FixtureFactory.AttachPolygon( verts, 1, body, "torso" );

            //mTorso = FixtureFactory.AttachRectangle(
            //    25.ToUnits(),
            //    50.ToUnits(),
            //    1,
            //    Vector2.Zero,
            //    body,
            //    "torso" );

            mFeet = FixtureFactory.AttachRectangle(
                20.ToUnits(),
                10.ToUnits(),
                1,
                new Vector2( 0, 25 ).ToUnits(),
                body,
                "feet" );
            mFeet.IsSensor = true;

            return body;
        }

        protected override void ReconstructBody()
        {
            base.ReconstructBody();

            mTorso = Body.FindFixture( "torso" );
            mFeet = Body.FindFixture( "feet" );

            mTorso.OnCollision += Torso_OnCollision;

            mTorso.OnCollision += Torso_OnDoorCollision;

            mFeet.OnSeparation += Feet_OnSeparation;

            mFeet.OnCollision += ( a, b, c ) => {
                if ( !b.IsSensor && b.UserData is Flat )
                {
                    var diff = MathHelper.WrapAngle( UpDir.Angle - Rotation );
                    Rotation = Rotation + diff;
                }
                return true;
            };
        }

        private bool BeginFeetContact( Contact contact )
        {
            if ( contact.FixtureA.UserData as string == "feet"
                 || contact.FixtureB.UserData as string == "feet" )
            {
                ++mNumFootContacts;
            }
            return true;
        }

        private void EndFeetContact( Contact contact )
        {
            if ( contact.FixtureA.UserData as string == "feet"
                 || contact.FixtureB.UserData as string == "feet" )
            {
                --mNumFootContacts;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            foreach ( Cube.Face face in Cube.Faces )
            {
                face.World.ContactManager.BeginContact += BeginFeetContact;
                face.World.ContactManager.EndContact += EndFeetContact;
            }

            Body.FixedRotation = true;
            Body.GravityScale = 20f;
            Body.UseAdHocGravity = true;
            Body.AdHocGravity = Vector2.UnitY;
            Body.CollisionCategories = Category.Cat2;
            Body.Mass = 68;

            pixel = new Texture2D( GraphicsDevice, 3, 3 );
            pixel.SetData( new[] { Color.White, Color.White, Color.White,
                                   Color.White, Color.White, Color.White,
                                   Color.White, Color.White, Color.White } );

			aspectRatio = GraphicsDevice.Viewport.AspectRatio;
        }

        private bool Torso_OnCollision( Fixture fixtureA, Fixture fixtureB, Contact contact )
        {
            if ( contact.IsTouching && !fixtureB.IsSensor
                 && contact.Manifold.Type == ManifoldType.FaceA
                 && contact.Manifold.PointCount >= 1 )
            {
                Vector2 groundNormal = contact.Manifold.LocalNormal.Rotate( fixtureB.Body.Rotation );
                float groundNormalAngle = (float) Math.Atan2( groundNormal.Y, groundNormal.X ) + MathHelper.PiOver2;

                if ( MathTools.AnglesWithinRange( Rotation, groundNormalAngle, MathHelper.PiOver4 ) )
                    Rotation = groundNormalAngle;

                return true;
            }
            return contact.IsTouching;
        }

        private bool Torso_OnDoorCollision( Fixture fixtureA, Fixture fixtureB, Contact contact )
        {
            if ( fixtureB.UserData as SolidDescriptor == new SolidDescriptor( "end_door" )
                 && MathTools.AnglesWithinRange( Rotation, fixtureB.Body.Rotation, 0 ) )
            {
                this.Screen.Back();
            }

            return contact.IsTouching;
        }

        private void Feet_OnSeparation( Fixture fixtureA, Fixture fixtureB )
        {
            if ( fixtureB.Body.BodyType != BodyType.Dynamic
                 && fixtureB.UserData is Convex
                 && !IsJumping )
            {
                ApplyRelativeLinearImpulse( Vector2.UnitY * Velocity.Length() * 2 );
            }
        }

        public void ApplyAngularImpulse( float impulse )
        {
            Body.ApplyAngularImpulse( impulse );
        }

        public void ApplyLinearImpulse( Vector2 impulse )
        {
            Body.ApplyLinearImpulse( impulse );
        }

        public void ApplyRelativeLinearImpulse( Vector2 impulse )
        {
            Body.ApplyLinearImpulse( impulse.Rotate( Rotation ) );
        }

        public bool IsJumping
        {
            get; private set;
        }

        private void Jump( ref Vector2 velocity )
        {
            if ( FreeFall && !Game.GameProperties.AllowMultipleJumping )
                return;

            IsJumping = true;
            velocity.Y = -10f;
        }

        private void JumpStop( ref Vector2 velocity )
        {
            if ( velocity.Y < 0 )
                velocity.Y *= 0.6f;
        }

        public override void Update( GameTime gameTime )
        {
            var input = Game.Input;
            float seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;

            if ( mNumFootContacts == 1 && !IsJumping || FreeFall && !IsJumping )
                Rotation = UpDir.Angle;

            if ( Game.GameProperties.AllowManualGravity )
            {
                if ( input.GetAction( Action.RotateClockwise, this ) )
                    UpDir = Direction.FromAngle( -Rotation + (MathHelper.PiOver4 + 0.0001f) );

                if ( input.GetAction( Action.RotateAntiClockwise, this ) )
                    UpDir = Direction.FromAngle( -Rotation - (MathHelper.PiOver4 + 0.0001f) );
            }

            var actionRight = input.GetAction( Action.MoveRight, this );
            var actionLeft = input.GetAction( Action.MoveLeft, this );

            Vector2 velocity = Velocity.Rotate( -Rotation );

            float movementScale = FreeFall ? 10 : 20;

            if ( !(actionRight || actionLeft) )
                velocity.X = velocity.X.Lerp( 0, movementScale * seconds );

            velocity.X += actionRight * (movementScale + 1) * seconds;
            velocity.X -= actionLeft * (movementScale + 1) * seconds;

            if ( input.GetAction( Action.Jump, this ) )
                this.Jump( ref velocity );

            if ( input.GetAction( Action.JumpStop, this ) )
                this.JumpStop( ref velocity );

            velocity.X = MathHelper.Clamp( velocity.X, -5, +5 );
            if ( velocity.Y >= 0 && !FreeFall )
                IsJumping = false;

            Velocity = velocity.Rotate( Rotation );

            // ACTOR UPDATE \\
            base.Update( gameTime );


            mModelRotation.AnimateValue( Rotation );
            mModelRotation.Update( MathHelper.TwoPi * seconds );


            Vector3 pos = Normal * Cube.CameraDistance;
            pos += WorldPosition * 2f;
            pos.Normalize();

            Screen.Camera.AnimatePosition( pos * Cube.CameraDistance, Cube.CameraDistance * 2 );
            Screen.Camera.Target = WorldPosition;
            Screen.Camera.AnimateUpVector( CubeFace.UpVec.Rotate( Normal, -Rotation ) );
        }

        protected override void ApplyRotation( CompassDirection dir )
        {
            base.ApplyRotation( dir );
            mModelRotation.Value = Rotation;
            Cube.Rotate( dir );
        }

        public override void Draw( GameTime gameTime )
        {
			// Below are codes for render the 3d model, didn't quite working bug-free so commented out for now
			Matrix[] transforms = new Matrix[ model3D.Bones.Count ];
			model3D.CopyAbsoluteBoneTransformsTo( transforms );

			// Draw the model. A model can have multiple meshes, so loop.
			foreach ( ModelMesh mesh in model3D.Meshes )
			{
				// This is where the mesh orientation is set, as well 
				// as our camera and projection.
				foreach ( BasicEffect effect in mesh.Effects )
				{
					effect.EnableDefaultLighting();
					effect.World = transforms[ mesh.ParentBone.Index ]
                        * Matrix.CreateScale( 0.0006f )
                        * Matrix.CreateTranslation( 0, -5.ToUnits(), 0 )
                        * Vector3.UnitY.RotateOntoM( CubeFace.UpVec )
                        * Matrix.CreateFromAxisAngle( CubeFace.Normal, -mModelRotation )
                        * Matrix.CreateTranslation( WorldPosition );

                    Screen.Camera.Apply( effect );
				}
				// Draw the mesh, using the effects set above.
				mesh.Draw();
			}
        }

    }
}
