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
    public enum AnimationState
    {
        Idle = 0,
        MovementRight = 1,
        MovementLeft = 2,
        Mask_Movement = 3,

        Still = 0,
        Walking = 4,
        Running = 8,
        Sprinting = Walking | Running,
        Mask_Speed = 15 ^ Mask_Movement,

        Standing = 0,
        Falling = 16,
        Jumping = 32,// | Falling,
        Mask_Aerial = 63 ^ (Mask_Speed | Mask_Movement),
    }

    public partial class Player : Actor
    {
        public const float JUMP_VELOCITY = -6f;
        public const float JUMP_STOP_FACTOR = 0.6f;
        public const float MAX_RUN_SPEED = 3.5f;
        public const float AIR_MOVEMENT_SCALE = 10;
        public const float GROUND_MOVEMENT_SCALE = 20;
        public const float PLAYER_GRAVITY = 15;
        public const float PLAYER_FRICTION = 1;
        
        public const float MODEL_SCALE = 0.061f;
        public const float RUN_ANIM_FACTOR = 0.04f / MODEL_SCALE;

        public readonly float PLAYER_WIDTH = 15.ToUnits();
        public readonly float PLAYER_HEIGHT = 60.ToUnits();

        public readonly Color PLAYER_COLOR = Color.OrangeRed;

        private Model model3D;

        private Fixture mTorso;
        private Fixture mFeet;
        private AnimatedVariable<float, float> mModelRotation;

        private static Texture2D sTexture;

        public AnimationState AnimState
        {
            get; private set;
        }

        public AnimationState AnimMovementState
        {
            get {
                return AnimState & AnimationState.Mask_Movement;
            }
            private set {
                AnimState = (AnimState & ~AnimationState.Mask_Movement)
                           | value & AnimationState.Mask_Movement;
            }
        }

        public AnimationState AnimSpeedState
        {
            get {
                return AnimState & AnimationState.Mask_Speed;
            }
            private set {
                AnimState = (AnimState & ~AnimationState.Mask_Speed)
                           | value & AnimationState.Mask_Speed;
            }
        }

        public AnimationState AnimAerialState
        {
            get {
                return AnimState & AnimationState.Mask_Aerial;
            }
            private set {
                AnimState = (AnimState & ~AnimationState.Mask_Aerial)
                            | value & AnimationState.Mask_Aerial;
            }
        }

        private float MovementRotation
        {
            get {
                switch ( AnimMovementState )
                {
                case AnimationState.MovementRight:
                    return MathHelper.PiOver2;

                case AnimationState.MovementLeft:
                    return -MathHelper.PiOver2;

                default:
                    return 0;
                }
            }
        }

        private AnimationPlayer PlayerAnimation
        {
            get {
                return AnimSpeedState != AnimationState.Still
                    && AnimAerialState == AnimationState.Standing
                    ? mRunPlayer : mIdlePlayer;
            }
        }

        private AnimationPlayer mIdlePlayer;
        private AnimationPlayer mRunPlayer;

        private AnimationClip mIdleClip;
        private AnimationClip mRunClip;

        private SkinningData mSkinData;

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
            : base( cube.Game, screen, cube, worldPos, (Direction) rotation )
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
            model3D = Game.Content.Load<Model>( "Models\\playerBeta" );

            mSkinData = model3D.Tag as SkinningData;
            if ( mSkinData == null )
                throw new InvalidOperationException(
                    "This model does not contain a SkinningData tag." );

            // Create an animation player, and start decoding an animation clip.
            mIdlePlayer = new AnimationPlayer( mSkinData );
            mRunPlayer = new AnimationPlayer( mSkinData );

            mIdleClip = mSkinData.AnimationClips[ "idle" ];
            mRunClip = mSkinData.AnimationClips[ "run" ];

            mRunPlayer.StartClip( mRunClip );
            mIdlePlayer.StartClip( mIdleClip );

            sfxJump = Game.Content.Load<SoundEffect>( "Audio\\jump" );
            sfxLand = Game.Content.Load<SoundEffect>( "Audio\\land" );

            sTexture = new Texture2D( GraphicsDevice, 1, 1 );
            sTexture.SetData( new Color[] { PLAYER_COLOR } );
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
                PLAYER_WIDTH,
                PLAYER_HEIGHT,
                5.ToUnits(),
                5.ToUnits(),
                0 );
            var torso = FixtureFactory.AttachPolygon( verts, 1, body, "torso" );
            torso.Friction = PLAYER_FRICTION;

            var feet = FixtureFactory.AttachRectangle(
                PLAYER_WIDTH * 0.9f,
                10.ToUnits(),
                1,
                new Vector2( 0, PLAYER_HEIGHT / 2 ),
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

            mNumFootContacts = 0;
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

        public bool IsRunning
        {
            get {
                return AnimMovementState != AnimationState.Idle
                    && AnimSpeedState != AnimationState.Still;
            }
        }

        //public bool IsJumping
        //{
        //    get {
        //        return (AnimState & AnimationState.Jumping) == AnimationState.Jumping;
        //    }
        //}

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

            AnimAerialState |= AnimationState.Jumping;
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

            mIdlePlayer.Update( gameTime.ElapsedGameTime, true, Matrix.Identity );
            //mRunPlayer.Update( gameTime.ElapsedGameTime, true, Matrix.Identity );

            #region Rotation
            if ( mNumFootContacts == 1 && !IsJumping || FreeFall && !IsJumping )
                Rotation = UpDir.Angle;

            if ( Game.GameProperties.AllowManualGravity )
            {
                if ( GetPlayerAction( Action.RotateClockwise ) )
                    UpDir = (Direction) (Rotation - (MathHelper.PiOver4 + 0.0001f));

                if ( GetPlayerAction( Action.RotateAntiClockwise ) )
                    UpDir = (Direction) (Rotation + (MathHelper.PiOver4 + 0.0001f));
            }
            #endregion

            // Rotation-normalized velocity proxy
            Vector2 velocity = Velocity.Rotate( -Rotation );

            #region Running
            float movementScale = FreeFall ? AIR_MOVEMENT_SCALE : GROUND_MOVEMENT_SCALE;
            var actionLeft = GetPlayerAction( Action.MoveLeft );
            var actionRight = GetPlayerAction( Action.MoveRight );

            //IsRunning = (bool) actionLeft != actionRight;

            AnimMovementState = (actionRight ? AnimationState.MovementRight : AnimationState.Idle)
                                | (actionLeft ? AnimationState.MovementLeft : AnimationState.Idle);

            if ( !IsRunning )
                velocity.X = velocity.X.Lerp( 0, movementScale * seconds );

            velocity.X -= actionLeft * (movementScale + 1) * seconds;
            velocity.X += actionRight * (movementScale + 1) * seconds;
            velocity.X = MathHelper.Clamp( velocity.X, -MAX_RUN_SPEED, +MAX_RUN_SPEED );

            float speed = Math.Abs( velocity.X );

            AnimSpeedState = speed >= MAX_RUN_SPEED ? AnimationState.Sprinting
                : speed > MAX_RUN_SPEED / 2 ? AnimationState.Running
                    : speed > 0 ? AnimationState.Walking
                        : AnimationState.Still;

            long ticks = gameTime.ElapsedGameTime.Ticks;
            TimeSpan t = new TimeSpan( (long) (ticks * RUN_ANIM_FACTOR * speed) );
            mRunPlayer.Update( t, true, Matrix.Identity );

            float runFactor = velocity.X * MathHelper.PiOver2 / 8 / MAX_RUN_SPEED;
            #endregion

            #region Jumping
            if ( GetPlayerAction( Action.Jump ) )
                Jump( ref velocity );

            if ( GetPlayerAction( Action.JumpStop ) )
                JumpStop( ref velocity );

            if ( velocity.Y >= 0 && !FreeFall )
            {
                IsJumping = false;
                AnimAerialState &= ~AnimationState.Jumping;
            }

            if ( FreeFall )
                AnimAerialState |= AnimationState.Falling;
            else
                AnimAerialState = AnimationState.Standing;
            #endregion

            #region Landing
            if ( velocity.Y == 0 && !hasLanded )
            {
                sfxLand.Play();
                hasLanded = true;
            }
            #endregion

            Velocity = velocity.Rotate( Rotation );

            // ACTOR UPDATE \\
            base.Update( gameTime );

            mModelRotation.AnimateValue( Rotation + runFactor );
            mModelRotation.Step( MathHelper.TwoPi * seconds );

            if ( GetPlayerAction( Action.PlaceClone ) )
                ClonePlayer();

            if ( GetPlayerAction( Action.CycleClone ) )
                Screen.NextClone();

            if ( GetPlayerAction( Action.DeleteClone ) )
                KillPlayer();
        }

        public override void Draw( GameTime gameTime )
        {
            // Below are codes for render the 3d model, didn't quite working bug-free so commented out for 
            //Matrix[] transforms = new Matrix[ model3D.Bones.Count ];
            //model3D.CopyAbsoluteBoneTransformsTo( transforms );

            Matrix[] transforms = PlayerAnimation.GetSkinTransforms();

            Matrix worldTransformation = Matrix.CreateTranslation( 0, -1, 0 )
                * Matrix.CreateScale( MODEL_SCALE )
                * Matrix.CreateFromAxisAngle( Vector3.UnitY, MovementRotation )
                * Vector3.UnitZ.RotateOnto_M( CubeFace.Normal )
                * Matrix.CreateFromAxisAngle( CubeFace.Normal, CubeFace.Rotation - mModelRotation )
                * Matrix.CreateTranslation( WorldPosition );

            // Draw the model. A model can have multiple meshes, so loop.
            foreach ( ModelMesh mesh in model3D.Meshes )
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach ( SkinnedEffect effect in mesh.Effects )
                {
                    effect.EnableDefaultLighting();
                    effect.Parameters[ "Bones" ].SetValue( transforms );
                    effect.World = transforms[ mesh.ParentBone.Index ] * worldTransformation;
                    //effect.DiffuseColor = PLAYER_COLOR.ToVector3();
                    effect.AmbientLightColor = PLAYER_COLOR.ToVector3();
                    effect.Texture = sTexture;

                    Screen.Camera.Apply( effect );
                }
                mesh.Draw();
            }
        }

    }
}
