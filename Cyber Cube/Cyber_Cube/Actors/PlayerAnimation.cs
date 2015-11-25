using CyberCube.Tools;
using Microsoft.Xna.Framework;
using SkinnedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Actors
{
    public partial class Player
    {
        #region AnimationState
        #region AnimationState Enums
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
            Jumping = 32,
            Mask_Aerial = 63 ^ (Mask_Speed | Mask_Movement),
        }

        public enum AnimationMovementState
        {
            Mask = AnimationState.Mask_Movement,
            Idle = AnimationState.Idle,
            Right = AnimationState.MovementRight,
            Left = AnimationState.MovementLeft
        }

        public enum AnimationSpeedState
        {
            Mask = AnimationState.Mask_Speed,
            Still = AnimationState.Still,
            Walking = AnimationState.Walking,
            Running = AnimationState.Running,
            Sprinting = AnimationState.Sprinting
        }

        public enum AnimationAerialState
        {
            Mask = AnimationState.Mask_Aerial,
            Standing = AnimationState.Standing,
            Falling = AnimationState.Falling,
            Jumping = AnimationState.Jumping
        }
        #endregion

        #region AnimationState Properties
        public AnimationState AnimState
        {
            get; private set;
        }

        public AnimationMovementState AnimMovementState
        {
            get {
                return (AnimationMovementState) AnimState & AnimationMovementState.Mask;
            }
            private set {
                AnimState = (AnimState & ~AnimationState.Mask_Movement)
                    | (AnimationState) (value & AnimationMovementState.Mask);
            }
        }

        public AnimationSpeedState AnimSpeedState
        {
            get {
                return (AnimationSpeedState) AnimState & AnimationSpeedState.Mask;
            }
            private set {
                AnimState = (AnimState & ~AnimationState.Mask_Speed)
                    | (AnimationState) (value & AnimationSpeedState.Mask);
            }
        }

        public AnimationAerialState AnimAerialState
        {
            get {
                return (AnimationAerialState) AnimState & AnimationAerialState.Mask;
            }
            private set {
                AnimState = (AnimState & ~AnimationState.Mask_Aerial)
                    | (AnimationState) (value & AnimationAerialState.Mask);
            }
        }
        #endregion
        #endregion

        public bool IsRunning
        {
            get {
                return AnimMovementState != AnimationMovementState.Idle
                    && AnimSpeedState != AnimationSpeedState.Still;
            }
        }

        public bool IsJumping
        {
            get {
                return (AnimState & AnimationState.Jumping) == AnimationState.Jumping;
            }
        }

        private AnimationPlayer mIdlePlayer;
        private AnimationPlayer mRunPlayer;

        private AnimationClip mIdleClip;
        private AnimationClip mRunClip;

        private AnimatedVariable<float> mModelRotation;

        private void LoadAnimations()
        {
            // Create an animation player, and start decoding an animation clip.
            mIdlePlayer = new AnimationPlayer( mSkinData );
            mRunPlayer = new AnimationPlayer( mSkinData );

            mIdleClip = mSkinData.AnimationClips[ "idle" ];
            mRunClip = mSkinData.AnimationClips[ "run" ];

            mRunPlayer.StartClip( mRunClip );
            mIdlePlayer.StartClip( mIdleClip );
        }

        private void UpdateAnimations( GameTime gameTime )
        {
            mIdlePlayer.Update( gameTime.ElapsedGameTime, true, Matrix.Identity );
        }

        private void UpdateRunningAnimations( GameTime gameTime, Vector2 velocity )
        {
            float seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;

            AnimMovementState = velocity.X > 0 ? AnimationMovementState.Right
                : velocity.X < 0 ? AnimationMovementState.Left
                    : AnimationMovementState.Idle;

            float speed = Math.Abs( velocity.X );

            AnimSpeedState = speed >= MAX_RUN_SPEED ? AnimationSpeedState.Sprinting
                : speed > MAX_RUN_SPEED / 2 ? AnimationSpeedState.Running
                    : speed > 0 ? AnimationSpeedState.Walking
                        : AnimationSpeedState.Still;

            long ticks = gameTime.ElapsedGameTime.Ticks;
            TimeSpan t = new TimeSpan( (long) (ticks * RUN_ANIM_FACTOR * speed) );
            mRunPlayer.Update( t, true, Matrix.Identity );

            float runFactor = velocity.X * MathHelper.PiOver2 / 8 / MAX_RUN_SPEED;
            mModelRotation.AnimateValue( Rotation + runFactor );
            mModelRotation.Step( MathHelper.TwoPi * seconds );
        }

        private void UpdateJumpingAnimations( GameTime gameTime, Vector2 velocity )
        {
            var tmp = AnimAerialState;

            if ( velocity.Y >= 0 )
                AnimAerialState &= ~AnimationAerialState.Jumping;

            if ( FreeFall )
                AnimAerialState |= AnimationAerialState.Falling;
            else
                AnimAerialState &= ~AnimationAerialState.Falling;

            if ( tmp != AnimationAerialState.Standing
                 && AnimAerialState == AnimationAerialState.Standing )
            {
                sfxLand.Play();
            }
        }


        private float MovementRotation
        {
            get {
                switch ( AnimMovementState )
                {
                case AnimationMovementState.Right:
                    return MathHelper.PiOver2;

                case AnimationMovementState.Left:
                    return -MathHelper.PiOver2;

                default:
                    return 0;
                }
            }
        }

        private AnimationPlayer PlayerAnimation
        {
            get {
                return AnimSpeedState != AnimationSpeedState.Still
                    && AnimAerialState == AnimationAerialState.Standing
                    ? mRunPlayer : mIdlePlayer;
            }
        }
    }
}
