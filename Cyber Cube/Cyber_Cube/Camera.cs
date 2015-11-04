using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube
{
    public class Camera : CubeGameComponent
    {
        private Tools.AnimatedVariable< Vector3, float > mPosition;
        private Tools.AnimatedVariable< Vector3, float > mTarget;
        private Tools.AnimatedVariable< Vector3, float > mUpVector;

        private Matrix mViewMatrix = default( Matrix );

        private float mPositionSpeed = 1;
        private float mTargetSpeed = 1;
        private float mUpVectorSpeed = 1;

        public Camera( CubeGame game )
            : this( game, Vector3.Zero, Vector3.Zero, Vector3.Zero )
        {
        }

        public Camera( CubeGame game, Vector3 position, Vector3 target, Vector3 upVector )
            : base( game )
        {
            mPosition = new Tools.AnimatedVariable<Vector3, float>( position,
                ( v0, v1, amount ) => Vector3.Distance( v0, v1 ) > amount
                                      ? v0.Slerp( v1, amount )
                                      : v1 );

            mTarget = new Tools.AnimatedVariable<Vector3, float>( target,
                ( v0, v1, amount ) => Vector3.Distance( v0, v1 ) > amount
                                      ? v0.Slerp( v1, amount )
                                      : v1 );

            mUpVector = new Tools.AnimatedVariable<Vector3, float>( upVector,
                ( v0, v1, amount ) => Vector3.Distance( v0, v1 ) > amount
                                      ? v0.Slerp( v1, amount * 10 )
                                      : v1 );

            mPosition.OnValueChanged += OnViewValueChanged;
            mTarget.OnValueChanged += OnViewValueChanged;
            mUpVector.OnValueChanged += OnViewValueChanged;
        }

        private void OnViewValueChanged( Tools.AnimatedVariable<Vector3, float> sender, Vector3 value )
        {
            if ( !sender.IsAnimating )
                mViewMatrix = default( Matrix );
        }

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

        public override void Update( GameTime gameTime )
        {
            float seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;

            mPosition.Update( mPositionSpeed * seconds );
            mTarget.Update( mTargetSpeed * seconds );
            mUpVector.Update( mUpVectorSpeed * seconds );
        }

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

        public void SkipAnimation()
        {
            mPosition.SkipAnimation();
            mTarget.SkipAnimation();
            mUpVector.SkipAnimation();
        }

        public bool IsAnimating
        {
            get {
                return mPosition.IsAnimating || mTarget.IsAnimating || mUpVector.IsAnimating;
            }
        }

        public Matrix View
        {
            get {
                if ( IsAnimating || mViewMatrix == default( Matrix ) )
                    mViewMatrix = Matrix.CreateLookAt( Position, Target, UpVector );

                return mViewMatrix;
            }
        }

    }
}
