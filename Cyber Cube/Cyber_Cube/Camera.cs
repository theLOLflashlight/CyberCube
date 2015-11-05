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

        private float mFovSpeed = 1;
        private float mAspectRatioSpeed = 1;
        private float mNearPlaneDistSpeed = 1;
        private float mFarPlaneDistSpeed = 1;


        private Matrix mViewMatrix = default( Matrix );

        private AnimatedVariable<Vector3, float> mPosition;
        private AnimatedVariable<Vector3, float> mTarget;
        private AnimatedVariable<Vector3, float> mUpVector;

        private float mPositionSpeed = 1;
        private float mTargetSpeed = 1;
        private float mUpVectorSpeed = 1;


        /*private Matrix mWorldMatrix = default( Matrix );

        private AnimatedVariable<Vector3, float> mScale;
        private AnimatedVariable<Vector3, float> mRotation;
        private AnimatedVariable<Vector3, float> mTranslation;

        private float mScaleSpeed = 1;
        private float mRotationSpeed = 1;
        private float mTranslationSpeed = 1;*/


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

            #region Init World
            /*mScale = new AnimatedVariable<Vector3, float>( Vector3.One, Utils.Lerp );
            mRotation = new AnimatedVariable<Vector3, float>( Vector3.Zero, Utils.Lerp );
            mTranslation = new AnimatedVariable<Vector3, float>( Vector3.Zero, Utils.Lerp );

            mScale.ValueChanged += OnWorldValueChanged;
            mRotation.ValueChanged += OnWorldValueChanged;
            mTranslation.ValueChanged += OnWorldValueChanged;*/
            #endregion
        }

        public void Apply( Effect effect )
        {
            if ( effect is BasicEffect )
            {
                var basicEffect = (BasicEffect) effect;

                basicEffect.Projection = Projection;
                basicEffect.View = View;
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

        /*public Matrix World
        {
            get {
                if ( IsWorldAnimating || mWorldMatrix == default( Matrix ) )
                {
                    Matrix S = Matrix.CreateScale( Scale );
                    Matrix R = Matrix.CreateFromYawPitchRoll( Rotation.Y, Rotation.X, Rotation.Z );
                    Matrix T = Matrix.CreateTranslation( Position );
                    mWorldMatrix = S * R * T;
                }

                return mWorldMatrix;
            }
        }*/

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

        /*private void OnWorldValueChanged( AnimatedVariable<Vector3, float> sender, Vector3 value )
        {
            if ( !sender.IsAnimating )
                mWorldMatrix = default( Matrix );
        }*/


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

        #region World Params
        /*public Vector3 Scale
        {
            get {
                return mScale.Value;
            }
            set {
                mScale.Value = value;
            }
        }

        public Vector3 Rotation
        {
            get {
                return mRotation.Value;
            }
            set {
                mRotation.Value = value;
            }
        }

        public Vector3 Translation
        {
            get {
                return mTranslation.Value;
            }
            set {
                mTranslation.Value = value;
            }
        }*/
        #endregion


        public override void Update( GameTime gameTime )
        {
            float seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;

            mFov.Update( mFovSpeed * seconds );
            mAspectRatio.Update( mAspectRatioSpeed * seconds );
            mNearPlaneDist.Update( mNearPlaneDistSpeed * seconds );
            mFarPlaneDist.Update( mFarPlaneDistSpeed * seconds );

            mPosition.Update( mPositionSpeed * seconds );
            mTarget.Update( mTargetSpeed * seconds );
            mUpVector.Update( mUpVectorSpeed * seconds );

            //mScale.Update( mScaleSpeed * seconds );
            //mRotation.Update( mRotationSpeed * seconds );
            //mTranslation.Update( mTranslationSpeed * seconds );
        }


        #region Animate Projection
        public void AnimateFov( float fov, float speed )
        {
            mFovSpeed = speed;
            mFov.AnimateValue( fov );
        }

        public void AnimateAspectRatio( float aspectRatio, float speed )
        {
            mAspectRatioSpeed = speed;
            mAspectRatio.AnimateValue( aspectRatio );
        }

        public void AnimateNearPlaneDist( float nearPlaneDist, float speed )
        {
            mNearPlaneDistSpeed = speed;
            mNearPlaneDist.AnimateValue( nearPlaneDist );
        }

        public void AnimateFarPlaneDist( float farPlaneDist, float speed )
        {
            mFarPlaneDistSpeed = speed;
            mFarPlaneDist.AnimateValue( farPlaneDist );
        }
        #endregion

        #region Animate View
        public void AnimatePosition( Vector3 position, float speed )
        {
            mPositionSpeed = speed;
            mPosition.AnimateValue( position );
        }

        public void AnimateTarget( Vector3 target, float speed )
        {
            mTargetSpeed = speed;
            mTarget.AnimateValue( target );
        }

        public void AnimateUpVector( Vector3 upVector, float speed )
        {
            mUpVectorSpeed = speed;
            mUpVector.AnimateValue( upVector );
        }
        #endregion

        #region Animate World
        /*public void AnimateScale( Vector3 scale, float speed )
        {
            mScaleSpeed = speed;
            mScale.AnimateValue( scale );
        }

        public void AnimateRotation( Vector3 rotation, float speed )
        {
            mRotationSpeed = speed;
            mRotation.AnimateValue( rotation );
        }

        public void AnimateTranslation( Vector3 translation, float speed )
        {
            mTranslationSpeed = speed;
            mTranslation.AnimateValue( translation );
        }*/
        #endregion

        #region Skip Animation
        public void SkipAnimation()
        {
            SkipProjectionAnimation();
            SkipViewAnimation();
            //SkipWorldAnimation();
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

        /*public void SkipWorldAnimation()
        {
            mScale.SkipAnimation();
            mRotation.SkipAnimation();
            mTranslation.SkipAnimation();
        }*/
        #endregion

        #region Is Animating
        public bool IsAnimating()
        {
            return IsProjectionAnimating
                   || IsViewAnimating;
                   //|| IsWorldAnimating;
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

        /*public bool IsWorldAnimating
        {
            get {
                return mScale.IsAnimating
                       || mRotation.IsAnimating
                       || mTranslation.IsAnimating;
            }
        }*/
        #endregion
    }
}
