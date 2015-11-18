using CyberCube.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube
{
    public class Camera : CubeGameComponent
    {
        private Matrix mProjectionMatrix = default( Matrix );

        private AnimatedVariable<float, float> mFov;
        private AnimatedVariable<float, float> mAspectRatio;
        private AnimatedVariable<float, float> mNearPlaneDist;
        private AnimatedVariable<float, float> mFarPlaneDist;

        public float FovSpeed { get; set; } = 1;
        public float AspectRatioSpeed { get; set; } = 1;
        public float NearPlaneDistSpeed { get; set; } = 1;
        public float FarPlaneDistSpeed { get; set; } = 1;


        private Matrix mViewMatrix = default( Matrix );

        private AnimatedVariable<Vector3, float> mPosition;
        private AnimatedVariable<Vector3, float> mTarget;
        private AnimatedVariable<Vector3, float> mUpVector;

        public float PositionSpeed { get; set; } = 1;
        public float TargetSpeed { get; set; } = 1;
        public float UpVectorSpeed { get; set; } = 1;


        public Camera( CubeGame game )
            : base( game )
        {
            #region Init Projection
            mFov = new AnimatedVariable<float, float>( Utils.Lerp );
            mAspectRatio = new AnimatedVariable<float, float>( Utils.Lerp );
            mNearPlaneDist = new AnimatedVariable<float, float>( Utils.Lerp );
            mFarPlaneDist = new AnimatedVariable<float, float>( Utils.Lerp );

            mFov.ValueChanged += OnProjectionValueChanged;
            mAspectRatio.ValueChanged += OnProjectionValueChanged;
            mNearPlaneDist.ValueChanged += OnProjectionValueChanged;
            mFarPlaneDist.ValueChanged += OnProjectionValueChanged;
            #endregion

            #region Init View
            mPosition = new AnimatedVariable<Vector3, float>( (v0, v1, d)
                => Vector3.Distance( v0, v1 ) > d / 10 ? v0.Slerp( v1, d ) : v1 );

            mTarget = new AnimatedVariable<Vector3, float>( (v0, v1, d)
                => Vector3.Distance( v0, v1 ) > d ? v0.Slerp( v1, d ) : v1 );

            mUpVector = new AnimatedVariable<Vector3, float>( Vector3.Up, (v0, v1, d)
                => Vector3.Distance( v0, v1 ) > d ? v0.Slerp( v1, d * 10 ) : v1 );

            mPosition.ValueChanged += OnViewValueChanged;
            mTarget.ValueChanged += OnViewValueChanged;
            mUpVector.ValueChanged += OnViewValueChanged;
            #endregion
        }

        public override void Update( GameTime gameTime )
        {
            float seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;

            mFov.Update( FovSpeed * seconds );
            mAspectRatio.Update( AspectRatioSpeed * seconds );
            mNearPlaneDist.Update( NearPlaneDistSpeed * seconds );
            mFarPlaneDist.Update( FarPlaneDistSpeed * seconds );

            mPosition.Update( PositionSpeed * seconds );
            mTarget.Update( TargetSpeed * seconds );
            mUpVector.Update( UpVectorSpeed * seconds );
        }

        public void Apply( Effect effect )
        {
            if ( effect is BasicEffect )
            {
                var basicEffect = (BasicEffect) effect;

                basicEffect.Projection = Projection;
                basicEffect.View = View;
            }
            else if(effect is SkinnedEffect)
            {
                var skinnedEffect = (SkinnedEffect)effect;

                skinnedEffect.Projection = Projection;
                skinnedEffect.View = View;
            }
            else
            {
                effect.Parameters[ "Projection" ].SetValue( Projection );
                effect.Parameters[ "View" ].SetValue( View );
            }
        }

        public Matrix Projection
        {
            get {
                if ( IsProjectionAnimating || mProjectionMatrix == default( Matrix ) )
                    mProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                        Fov, AspectRatio, NearPlaneDistance, FarPlaneDistance );

                return mProjectionMatrix;
            }
        }

        public Matrix View
        {
            get {
                if ( IsViewAnimating || mViewMatrix == default( Matrix ) )
                    mViewMatrix = Matrix.CreateLookAt( Position, Target, UpVector );

                return mViewMatrix;
            }
        }

        private void OnProjectionValueChanged( AnimatedVariable<float, float> sender, float value )
        {
            if ( !sender.IsAnimating )
                mProjectionMatrix = default( Matrix );
        }

        private void OnViewValueChanged( AnimatedVariable<Vector3, float> sender, Vector3 value )
        {
            if ( !sender.IsAnimating )
                mViewMatrix = default( Matrix );
        }

        #region Projection Params
        public float Fov
        {
            get {
                return mFov.Value;
            }
            set {
                mFov.Value = value;
            }
        }

        public float AspectRatio
        {
            get {
                return mAspectRatio.Value;
            }
            set {
                mAspectRatio.Value = value;
            }
        }

        public float NearPlaneDistance
        {
            get {
                return mNearPlaneDist.Value;
            }
            set {
                mNearPlaneDist.Value = value;
            }
        }

        public float FarPlaneDistance
        {
            get {
                return mFarPlaneDist.Value;
            }
            set {
                mFarPlaneDist.Value = value;
            }
        }
        #endregion

        #region View Params
        public Vector3 Position
        {
            get {
                return mPosition.Value;
            }
            set {
                mPosition.Value = value;
            }
        }

        public Vector3 Target
        {
            get {
                return mTarget.Value;
            }
            set {
                mTarget.Value = value;
            }
        }

        public Vector3 UpVector
        {
            get {
                return mUpVector.Value;
            }
            set {
                mUpVector.Value = value;
            }
        }
        #endregion

        #region Animate Projection
        public void AnimateFov( float fov, float? speed = null )
        {
            if ( speed != null )
                FovSpeed = speed.Value;
            mFov.AnimateValue( fov );
        }

        public void AnimateAspectRatio( float aspectRatio, float? speed = null )
        {
            if ( speed != null )
                AspectRatioSpeed = speed.Value;
            mAspectRatio.AnimateValue( aspectRatio );
        }

        public void AnimateNearPlaneDist( float nearPlaneDist, float? speed = null )
        {
            if ( speed != null )
                NearPlaneDistSpeed = speed.Value;
            mNearPlaneDist.AnimateValue( nearPlaneDist );
        }

        public void AnimateFarPlaneDist( float farPlaneDist, float? speed = null )
        {
            if ( speed != null )
                FarPlaneDistSpeed = speed.Value;
            mFarPlaneDist.AnimateValue( farPlaneDist );
        }
        #endregion

        #region Animate View
        public void AnimatePosition( Vector3 position, float? speed = null )
        {
            if ( speed != null )
                PositionSpeed = speed.Value;
            mPosition.AnimateValue( position );
        }

        public void AnimateTarget( Vector3 target, float? speed = null )
        {
            if ( speed != null )
                TargetSpeed = speed.Value;
            mTarget.AnimateValue( target );
        }

        public void AnimateUpVector( Vector3 upVector, float? speed = null )
        {
            if ( speed != null )
                UpVectorSpeed = speed.Value;
            mUpVector.AnimateValue( upVector );
        }
        #endregion

        #region Skip Animation
        public void SkipAnimation()
        {
            SkipProjectionAnimation();
            SkipViewAnimation();
        }

        public void SkipProjectionAnimation()
        {
            mFov.SkipAnimation();
            mAspectRatio.SkipAnimation();
            mNearPlaneDist.SkipAnimation();
            mFarPlaneDist.SkipAnimation();
        }

        public void SkipViewAnimation()
        {
            mPosition.SkipAnimation();
            mTarget.SkipAnimation();
            mUpVector.SkipAnimation();
        }
        #endregion

        #region Is Animating
        public bool IsAnimating
        {
            get {
                return IsProjectionAnimating || IsViewAnimating;
            }
        }

        public bool IsProjectionAnimating
        {
            get {
                return mFov.IsAnimating
                       || mAspectRatio.IsAnimating
                       || mNearPlaneDist.IsAnimating
                       || mFarPlaneDist.IsAnimating;
            }
        }

        public bool IsViewAnimating
        {
            get {
                return mPosition.IsAnimating
                       || mTarget.IsAnimating
                       || mUpVector.IsAnimating;
            }
        }

        public bool IsTargetAnimating
        {
            get {
                return mTarget.IsAnimating;
            }
        }
        #endregion
    }
}
