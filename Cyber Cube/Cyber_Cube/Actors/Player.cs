using CyberCube.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        public const float JUMP_VELOCITY = -6f;
        public const float JUMP_STOP_FACTOR = 0.6f;
        public const float MAX_RUN_SPEED = 3.5f;
        public const float AIR_MOVEMENT_SCALE = 10;
        public const float GROUND_MOVEMENT_SCALE = 20;
        public const float PLAYER_GRAVITY = 15f;

        public readonly float PLAYER_WIDTH = 15.ToUnits();//0.4f;
        public readonly float PLAYER_HEIGHT = 50.ToUnits();//1.7f;

        private Model model3D;

        private Fixture mTorso;
        private Fixture mFeet;
        private AnimatedVariable<float, float> mModelRotation;

        private AnimationPlayer mIdlePlayer;
        private AnimationPlayer mRunPlayer;
        private AnimationClip mIdleClip;
        private AnimationClip mRunClip;
        private SkinningData mSkinData;

        private bool mShowRunClip;

        private SoundEffect sfxJump;
        private SoundEffect sfxLand;

        private bool hasLanded;

        public new PlayScreen Screen
        {
            get {
                return (PlayScreen) base.Screen;
            }
            set {
                base.Screen = value;
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
            //model3D = Game.Content.Load<Model>( "Models\\playerAlpha3D" );
            model3D = Game.Content.Load<Model>("Models\\playerBeta");

            mSkinData = model3D.Tag as SkinningData;

            if (mSkinData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");

            // Create an animation player, and start decoding an animation clip.
            mIdlePlayer = new AnimationPlayer(mSkinData);
            mRunPlayer = new AnimationPlayer(mSkinData);

            mIdleClip = mSkinData.AnimationClips["idle"];
            mRunClip = mSkinData.AnimationClips["run"];

            mRunPlayer.StartClip(mRunClip);
            mIdlePlayer.StartClip(mIdleClip);

            mShowRunClip = false;

            sfxJump = Game.Content.Load<SoundEffect>("Audio\\jump");
            sfxLand = Game.Content.Load<SoundEffect>("Audio\\land");
        }

        protected override void ApplyRotation( CompassDirection dir )
        {
            base.ApplyRotation( dir );
            mModelRotation.Value = Rotation;
            mNumFootContacts = 0;
            Cube.Rotate( dir );
        }

        protected override Body CreateBody( World world )
        {
            Body body = base.CreateBody( world );

            Vertices verts = PolygonTools.CreateRoundedRectangle(
                PLAYER_WIDTH,
                PLAYER_HEIGHT,
                5.ToUnits(),
                5.ToUnits(),
                0 );
            FixtureFactory.AttachPolygon( verts, 1, body, "torso" );

            var feet = FixtureFactory.AttachRectangle(
                PLAYER_WIDTH * 0.9f,
                10.ToUnits(),
                1,
                new Vector2( 0, PLAYER_HEIGHT / 2 ),//.ToUnits(),
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
            mTorso.OnCollision += Torso_OnHazardCollision;
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
            Body.GravityScale = PLAYER_GRAVITY;
            Body.UseAdHocGravity = true;
            Body.AdHocGravity = Vector2.UnitY;
            Body.CollisionCategories = Constants.Categories.PLAYER;
            Body.CollidesWith = Category.All ^ Constants.Categories.PLAYER;
            Body.Mass = 68;

            hasLanded = true;
        }

        protected override void Dispose( bool disposing )
        {
            base.Dispose( disposing );
            if ( disposing )
            {
                foreach ( Cube.Face face in Cube.Faces )
                {
                    face.World.ContactManager.BeginContact -= BeginFeetContact;
                    face.World.ContactManager.EndContact -= EndFeetContact;
                }
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

        public void KillPlayer()
        {
            Screen.RemovePlayer( this );
        }

        public void ClonePlayer()
        {
            Screen.AddPlayer( WorldPosition, Rotation );
        }

        private void Jump( ref Vector2 velocity )
        {
            if ( FreeFall && !Game.GameProperties.AllowMultipleJumping )
                return;

            IsJumping = true;
            hasLanded = false;

            velocity.Y = JUMP_VELOCITY;
            sfxJump.Play();

            AchievementManager.Instance[ Stat.Jump ]++;
        }

        private void JumpStop( ref Vector2 velocity )
        {
            if ( velocity.Y < 0 )
                velocity.Y *= JUMP_STOP_FACTOR;
        }

        private InputState<Action>.ActionState GetPlayerAction( Action action )
        {
            return Screen.Player == this
                ? Game.Input.GetAction( action, this )
                : default( InputState<Action>.ActionState );
        }

        public override void Update( GameTime gameTime )
        {
            //var input = Game.Input;
            float seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;

            #region Rotation
            if ( mNumFootContacts == 1 && !IsJumping || FreeFall && !IsJumping )
                Rotation = UpDir.Angle;

            if ( Game.GameProperties.AllowManualGravity )
            {
                if ( GetPlayerAction( Action.RotateClockwise ) )
                    UpDir = Direction.FromAngle( Rotation - (MathHelper.PiOver4 + 0.0001f) );

                if ( GetPlayerAction( Action.RotateAntiClockwise ) )
                    UpDir = Direction.FromAngle( Rotation + (MathHelper.PiOver4 + 0.0001f) );
            }
            #endregion

            // Rotation-normalized velocity proxy
            Vector2 velocity = Velocity.Rotate( -Rotation );

            #region Running
            float movementScale = FreeFall ? AIR_MOVEMENT_SCALE : GROUND_MOVEMENT_SCALE;
            var actionRight = GetPlayerAction( Action.MoveRight );
            var actionLeft = GetPlayerAction( Action.MoveLeft );

            if ( !( actionRight || actionLeft ) )
            {
                velocity.X = velocity.X.Lerp(0, movementScale * seconds);
                mShowRunClip = false;
            }
            else
            {
                mShowRunClip = true;
            }

            velocity.X += actionRight * (movementScale + 1) * seconds;
            velocity.X -= actionLeft * (movementScale + 1) * seconds;
            velocity.X = MathHelper.Clamp( velocity.X, -MAX_RUN_SPEED, +MAX_RUN_SPEED );
            #endregion

            #region Jumping
            if ( GetPlayerAction( Action.Jump ) )
                Jump( ref velocity );

            if ( GetPlayerAction( Action.JumpStop ) )
                JumpStop( ref velocity );

            if ( velocity.Y >= 0 && !FreeFall )
                IsJumping = false;
            #endregion

            #region Landing
            if (velocity.Y == 0 && !hasLanded)
            {
                sfxLand.Play();
                hasLanded = true;
            }
            #endregion

            Velocity = velocity.Rotate( Rotation );

            // ACTOR UPDATE \\
            base.Update( gameTime );

            mModelRotation.AnimateValue( Rotation );
            mModelRotation.Update( MathHelper.TwoPi * seconds );

            mIdlePlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            mRunPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);

            if ( GetPlayerAction( Action.PlaceClone ) )
                ClonePlayer();

            if ( GetPlayerAction( Action.CycleClone ) )
                Screen.NextClone();

            if ( GetPlayerAction( Action.DeleteClone ) )
                KillPlayer();
        }

        public override void Draw( GameTime gameTime )
        {
			// Below are codes for render the 3d model, didn't quite working bug-free so commented out for now

			//Matrix[] transforms = new Matrix[ model3D.Bones.Count ];
			//model3D.CopyAbsoluteBoneTransformsTo( transforms );

            Matrix[] transforms;

            if (mShowRunClip)
            {
                transforms = mRunPlayer.GetSkinTransforms();
            }
            else
            {
                transforms = mIdlePlayer.GetSkinTransforms();
            }

            // Draw the model. A model can have multiple meshes, so loop.
            foreach ( ModelMesh mesh in model3D.Meshes )
			{
				// This is where the mesh orientation is set, as well 
				// as our camera and projection.
				//foreach ( SkinnedEffect effect in mesh.Effects )
                foreach (SkinnedEffect effect in mesh.Effects)
                {
					effect.EnableDefaultLighting();
                    effect.Parameters["Bones"].SetValue(transforms);
					effect.World = transforms[ mesh.ParentBone.Index ]
                        * Matrix.CreateScale( 0.06f )
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
