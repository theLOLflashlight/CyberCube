﻿using CyberCube.Tools;
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

        private AnimatedVariable<float> mFov;
        private AnimatedVariable<float> mAspectRatio;
        private AnimatedVariable<float> mNearPlaneDist;
        private AnimatedVariable<float> mFarPlaneDist;

        public float FovSpeed = 1;
        public float AspectRatioSpeed = 1;
        public float NearPlaneDistSpeed = 1;
        public float FarPlaneDistSpeed = 1;


        private Matrix mViewMatrix = default( Matrix );

        private AnimatedVariable<Vector3> mPosition;
        private AnimatedVariable<Vector3> mTarget;
        private AnimatedVariable<Vector3> mUpVector;

        public float PositionSpeed = 1;
        public float TargetSpeed = 1;
        public float UpVectorSpeed = 1;

        public Camera( CubeGame game )
            : base( game )
        {
            #region Init Projection
            mFov = new AnimatedVariable<float>( Utils.Tween );
            mAspectRatio = new AnimatedVariable<float>( Utils.Tween );
            mNearPlaneDist = new AnimatedVariable<float>( Utils.Tween );
            mFarPlaneDist = new AnimatedVariable<float>( Utils.Tween );

            mFov.ValueChanged += OnProjectionValueChanged;
            mAspectRatio.ValueChanged += OnProjectionValueChanged;
            mNearPlaneDist.ValueChanged += OnProjectionValueChanged;
            mFarPlaneDist.ValueChanged += OnProjectionValueChanged;
            #endregion

            #region Init View
            mPosition = new AnimatedVariable<Vector3>( (v0, v1, d)
                => Vector3.Distance( v0, v1 ) > d / 10 ? v0.Slerp( v1, d ) : v1 );

            mTarget = new AnimatedVariable<Vector3>( (v0, v1, d)
                => Vector3.Distance( v0, v1 ) > d ? v0.Slerp( v1, d ) : v1 );

            mUpVector = new AnimatedVariable<Vector3>( Vector3.Up, (v0, v1, d)
                => Vector3.Distance( v0, v1 ) > d ? v0.Slerp( v1, d * 10 ) : v1 );

            mPosition.ValueChanged += OnViewValueChanged;
            mTarget.ValueChanged += OnViewValueChanged;
            mUpVector.ValueChanged += OnViewValueChanged;

            mPosition.ValueChanged += FixNaN;
            mTarget.ValueChanged += FixNaN;
            mUpVector.ValueChanged += FixNaN;
            #endregion
        }

        private void FixNaN( AnimatedVariable<Vector3> sender, Vector3 value )
        {
            if ( float.IsNaN( value.X ) || float.IsNaN( value.Y ) || float.IsNaN( value.Z ) )
                sender.EndAnimation();
        }

        private static AnimatedVariable<Vector3>.ValueInterpolator Orbit( Vector3 origin, float nearFactor = 1 )
        {
            return (v0, v1, d) => {
                return Vector3.Distance( v0, v1 ) > d * nearFactor
                    ? (v0 - origin).Slerp( v1 - origin, d ) + origin
                    : v1;
                //return (v0 - origin).Slerp( v1 - origin, Utils.Cubic( 0, 1, d ) ) + origin;
            };
        }

        public override void Update( GameTime gameTime )
        {
            float seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;

            mFov.Step( FovSpeed * seconds );
            mAspectRatio.Step( AspectRatioSpeed * seconds );
            mNearPlaneDist.Step( NearPlaneDistSpeed * seconds );
            mFarPlaneDist.Step( FarPlaneDistSpeed * seconds );

            mPosition.Step( PositionSpeed * seconds );
            mTarget.Step( TargetSpeed * seconds );
            mUpVector.Step( UpVectorSpeed * seconds );
        }

        public void Apply( Effect effect )
        {
            if ( effect is BasicEffect )
            {
                var basicEffect = (BasicEffect) effect;

                basicEffect.Projection = Projection;
                basicEffect.View = View;
            }
            else if ( effect is SkinnedEffect )
            {
                var skinnedEffect = (SkinnedEffect) effect;

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

        private void OnProjectionValueChanged( AnimatedVariable<float> sender, float value )
        {
            if ( !sender.IsAnimating )
                mProjectionMatrix = default( Matrix );
        }

        private void OnViewValueChanged( AnimatedVariable<Vector3> sender, Vector3 value )
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

            mPosition.Interpolator = VectorUtils.Lerp;
            mPosition.AnimateValue( position );
        }

        public void OrbitPosition( Vector3 position, Vector3 origin, float? speed = null )
        {
            if ( speed != null )
                PositionSpeed = speed.Value;

            mPosition.Interpolator = Orbit( origin, 0.1f );
            mPosition.AnimateValue( position );
        }

        public void AnimateTarget( Vector3 target, float? speed = null )
        {
            if ( speed != null )
                TargetSpeed = speed.Value;

            mTarget.Interpolator = VectorUtils.Lerp;
            mTarget.AnimateValue( target );
        }

        public void OrbitTarget( Vector3 target, Vector3 origin, float? speed = null )
        {
            if ( speed != null )
                TargetSpeed = speed.Value;

            mTarget.Interpolator = Orbit( origin );
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
            mFov.EndAnimation();
            mAspectRatio.EndAnimation();
            mNearPlaneDist.EndAnimation();
            mFarPlaneDist.EndAnimation();
        }

        public void SkipViewAnimation()
        {
            mPosition.EndAnimation();
            mTarget.EndAnimation();
            mUpVector.EndAnimation();
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
