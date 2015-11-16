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
using SkinnedModel;

namespace CyberCube.Actors
{
    public partial class Player : Actor
    {
        public const float JUMP_VELOCITY = -10f;
        public const float JUMP_STOP_FACTOR = 0.6f;
        public const float MAX_RUN_SPEED = 5;
        public const float AIR_MOVEMENT_SCALE = 10;
        public const float GROUND_MOVEMENT_SCALE = 20;

        private Model model3D;

        private Fixture mTorso;
        private Fixture mFeet;
        private AnimatedVariable<float, float> mModelRotation;

        private AnimationPlayer mAnimationPlayer;
        private AnimationClip mCurrentClip;
        private SkinningData mSkinData;

        public new PlayScreen Screen
        {
            get {
                return (PlayScreen) base.Screen;
            }
            set {
                base.Screen = (PlayScreen) value;
            }
        }

        public Player( PlayScreen screen, PlayableCube cube, Vector3 worldPos, float rotation )
            : base( cube.Game, screen, cube, worldPos, Direction.FromAngle( rotation ) )
        {
            this.Visible = true;
            this.DrawOrder = 1;

            mModelRotation = new AnimatedVariable<float, float>( rotation,
                (f0, f1, step) => {
                    var diff = MathHelper.WrapAngle( f1 - f0 );
                    return f0.Lerp( f0 + diff, step );
                } );
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            model3D = Game.Content.Load<Model>( "Models\\playerAlpha3D" );


            //model3D = Game.Content.Load<Model>("Models\\playerBeta");

            mSkinData = model3D.Tag as SkinningData;

            if (mSkinData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");

            // Create an animation player, and start decoding an animation clip.
            mAnimationPlayer = new AnimationPlayer(mSkinData);

            // Default animation clip
            mCurrentClip = mSkinData.AnimationClips["Armature|idle.001"];

            mAnimationPlayer.StartClip(mCurrentClip);
        }

        protected override void ApplyRotation( CompassDirection dir )
        {
            base.ApplyRotation( dir );
            mModelRotation.Value = Rotation;
            Cube.Rotate( dir );
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

            var feet = FixtureFactory.AttachRectangle(
                20.ToUnits(),
                10.ToUnits(),
                1,
                new Vector2( 0, 25 ).ToUnits(),
                body,
                "feet" );
            feet.IsSensor = true;

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
            mFeet.OnCollision += Feet_OnCollision;
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
            Body.CollidesWith = Category.All ^ Category.Cat2;
            Body.Mass = 68;
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
            velocity.Y = JUMP_VELOCITY;
        }

        private void JumpStop( ref Vector2 velocity )
        {
            if ( velocity.Y < 0 )
                velocity.Y *= JUMP_STOP_FACTOR;
        }

        public override void Update( GameTime gameTime )
        {
            var input = Game.Input;
            float seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;

            #region Rotation
            if ( mNumFootContacts == 1 && !IsJumping || FreeFall && !IsJumping )
                Rotation = UpDir.Angle;

            if ( Game.GameProperties.AllowManualGravity )
            {
                if ( input.GetAction( Action.RotateClockwise, this ) )
                    UpDir = Direction.FromAngle( Rotation - (MathHelper.PiOver4 + 0.0001f) );

                if ( input.GetAction( Action.RotateAntiClockwise, this ) )
                    UpDir = Direction.FromAngle( Rotation + (MathHelper.PiOver4 + 0.0001f) );
            }
            #endregion

            // Rotation-normalized velocity proxy
            Vector2 velocity = Velocity.Rotate( -Rotation );

            #region Running
            float movementScale = FreeFall ? AIR_MOVEMENT_SCALE : GROUND_MOVEMENT_SCALE;
            var actionRight = input.GetAction( Action.MoveRight, this );
            var actionLeft = input.GetAction( Action.MoveLeft, this );

            if ( !(actionRight || actionLeft) )
                velocity.X = velocity.X.Lerp( 0, movementScale * seconds );

            velocity.X += actionRight * (movementScale + 1) * seconds;
            velocity.X -= actionLeft * (movementScale + 1) * seconds;
            velocity.X = MathHelper.Clamp( velocity.X, -MAX_RUN_SPEED, +MAX_RUN_SPEED );
            #endregion

            #region Jumping
            if ( input.GetAction( Action.Jump, this ) )
                Jump( ref velocity );

            if ( input.GetAction( Action.JumpStop, this ) )
                JumpStop( ref velocity );

            if ( velocity.Y >= 0 && !FreeFall )
                IsJumping = false;
            #endregion

            Velocity = velocity.Rotate( Rotation );

            // ACTOR UPDATE \\
            base.Update( gameTime );

            mModelRotation.AnimateValue( Rotation );
            mModelRotation.Update( MathHelper.TwoPi * seconds );

            mAnimationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);

            if ( input.GetAction( Action.PlaceClone, this ) )
                Screen.AddPlayer( WorldPosition, Rotation );

            if ( input.GetAction( Action.CycleClone, this ) )
                Screen.NextClone();

            if ( input.GetAction( Action.DeleteClone, this ) )
                Screen.RemovePlayer( this );
        }

        public override void Draw( GameTime gameTime )
        {
			// Below are codes for render the 3d model, didn't quite working bug-free so commented out for now
			//Matrix[] transforms = new Matrix[ model3D.Bones.Count ];
			//model3D.CopyAbsoluteBoneTransformsTo( transforms );

            Matrix[] transforms = mAnimationPlayer.GetSkinTransforms();

            // Draw the model. A model can have multiple meshes, so loop.
            foreach ( ModelMesh mesh in model3D.Meshes )
			{
				// This is where the mesh orientation is set, as well 
				// as our camera and projection.
				foreach ( SkinnedEffect effect in mesh.Effects )
				{
					effect.EnableDefaultLighting();
                    effect.Parameters["Bones"].SetValue(transforms);
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
