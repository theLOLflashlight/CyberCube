using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{
    public class Camera : GameComponent
    {
        public new Game1 Game
        {
            get {
                return base.Game as Game1;
            }
        }

        private Vector3 mPosition0;
        private Vector3 mPosition1;

        private Vector3 mTarget0;
        private Vector3 mTarget1;

        private Vector3 mUpVector0;
        private Vector3 mUpVector1;

        private Matrix mViewMatrix = Matrix.Identity;
        private float mPositionSpeed = 1;
        private float mTargetSpeed = 1;
        private float mUpVectorSpeed = 1;

        public bool UsesSphereAnimation { get; set; }

        public Vector3 Position
        {
            get {
                return mPosition0;
            }
            set {
                mPosition0 = mPosition1 = value;
            }
        }

        public Vector3 Target
        {
            get
            {
                return mTarget0;
            }
            set
            {
                mTarget0 = mTarget1 = value;
            }
        }

        public Vector3 UpVector
        {
            get
            {
                return mUpVector0;
            }
            set
            {
                mUpVector0 = mUpVector1 = value;
            }
        }


        public Camera( Game game )
            : base( game )
        {
        }

        public Camera( Game game, Vector3 position, Vector3 target, Vector3 upVector )
            : base( game )
        {
            mPosition0 = mPosition1 = position;
            mTarget0 = mTarget1 = target;
            mUpVector0 = mUpVector1 = upVector;
        }

        public override void Update( GameTime gameTime )
        {
            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000f;

            if ( Vector3.Distance( mPosition0, mPosition1 ) < seconds * mPositionSpeed )
                mPosition0 = mPosition1;

            if ( Vector3.Distance( mTarget0, mTarget1 ) < seconds * mTargetSpeed )
                mTarget0 = mTarget1;

            if ( Vector3.Distance( mUpVector0, mUpVector1 ) < seconds * mUpVectorSpeed )
                mUpVector0 = mUpVector1;


            Utils.Vector3InterpolatorRef interpolator = UsesSphereAnimation
                ? (Utils.Vector3InterpolatorRef) Utils.InterpolateVectorSlerp
                : (Utils.Vector3InterpolatorRef) Utils.InterpolateVectorLerp;

            interpolator( ref mPosition0, mPosition1, seconds * mPositionSpeed );
            interpolator( ref mTarget0, mTarget1, seconds * mTargetSpeed );
            interpolator( ref mUpVector0, mUpVector1, seconds * mUpVectorSpeed * 10 );

        }

        public void AnimatePosition( Vector3 position, float speed )
        {
            mPosition1 = position;
            mPositionSpeed = speed;
        }

        public void AnimateTarget( Vector3 target, float speed )
        {
            mTarget1 = target;
            mTargetSpeed = speed;
        }

        public void AnimateUpVector( Vector3 upVector, float speed )
        {
            mUpVector1 = upVector;
            mUpVectorSpeed = speed;
        }

        public void SkipAnimation()
        {
            mPosition0 = mPosition1;
            mTarget0 = mTarget1;
            mUpVector0 = mUpVector1;
        }

        public bool IsAnimating()
        {
            return mPosition0 != mPosition1 || mTarget0 != mTarget1 || mUpVector0 != mUpVector1;
        }

        public Matrix View
        {
            get
            {
                if ( IsAnimating() || mViewMatrix == Matrix.Identity )
                    Matrix.CreateLookAt( ref mPosition0,
                                         ref mTarget0,
                                         ref mUpVector0,
                                         out mViewMatrix );
                return mViewMatrix;
            }
        }

    }
}
