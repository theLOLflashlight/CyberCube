using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube
{
    public abstract class Actor : DrawableCubeGameComponent
    {

        protected abstract Vector3 TransformMovementTo3d( Vector2 vec2d );

        protected Vector3 mWorldPosition;

        public Vector3 WorldPosition
        {
            get {
                return mWorldPosition;
            }
        }

        protected Vector2 mVelocity2d = Vector2.Zero;

        public Vector2 Velocity2d
        {
            get {
                return mVelocity2d;
            }
        }

        private bool mCollided = false;

        private Vector2 mGroundNormal;

        public bool FreeFall
        {
            get {
                return mGroundNormal == Vector2.Zero;
            }
        }

        public Cube Cube
        {
            get; protected set;
        }

        public Cube.Face CubeFace
        {
            get; protected set;
        }

        public Vector3 Normal
        {
            get {
                return CubeFace.Normal;
            }
        }

        public Direction UpDir
        {
            get; set;
        }

        public Actor( CubeGame game )
            : base( game )
        {
        }

        public Actor( CubeGame game, Cube cube, Vector3 worldPos, Direction upDir )
            : this( game )
        {
            Cube = cube;
            mWorldPosition = worldPos;
            UpDir = upDir;

            CubeFace = Cube.GetFaceFromPosition( mWorldPosition );
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            float timeDiff = (float) gameTime.ElapsedGameTime.TotalSeconds;


            if ( mGroundNormal.X > 0 )
                mVelocity2d.X = Math.Max( 0, mVelocity2d.X );

            else if ( mGroundNormal.X < 0 )
                mVelocity2d.X = Math.Min( 0, mVelocity2d.X );

            if ( mGroundNormal.Y > 0 )
                mVelocity2d.Y = Math.Max( 0, mVelocity2d.Y );

            else if ( mGroundNormal.Y < 0 )
                mVelocity2d.Y = Math.Min( 0, mVelocity2d.Y );


            mWorldPosition += TransformMovementTo3d( mVelocity2d ) * timeDiff;

            foreach ( var dir in ClampWorldPosition() )
                ApplyRotation( dir );


            if ( !mCollided )
                mGroundNormal = Vector2.Zero;
            mCollided = false;
        }

        protected virtual void ApplyRotation( CompassDirection dir )
        {
            Cube.Face nextFace = CubeFace.AdjacentFace( dir );
            var backDir = nextFace.BackwardsDirectionFrom( CubeFace );

            this.UpDir = Cube.GetNextUpDirection( UpDir, dir, backDir );
            this.CubeFace = nextFace;
        }


        public Vector2 ComputeFacePosition()
        {
            float adjustingFactor = Cube.Face.SIZE / 2;
            return Transform3dTo2d( WorldPosition )
                   * adjustingFactor
                   + new Vector2( adjustingFactor );
        }

        public void SetFacePosition( Vector2 vec2d )
        {
            float adjustingFactor = Cube.Face.SIZE / 2;
            vec2d -= new Vector2( adjustingFactor );
            vec2d /= adjustingFactor;

            mWorldPosition = Transform2dTo3d( vec2d );

            foreach ( var dir in ClampWorldPosition() )
                ApplyRotation( dir );
        }

        public void Collide( Rectangle rec )
        {
            Collide( rec, ComputeFacePosition() );
        }

        public void Collide( Rectangle rec, Vector2 facePos )
        {
            Vector2 perimiterPos = facePos.NearestPointOn( rec );

            Vector2 groundNormal = facePos - perimiterPos;
            if ( groundNormal != Vector2.Zero )
            {
                mGroundNormal = Vector2.Normalize( groundNormal );
                mGroundNormal = mGroundNormal.Rotate( UpDir.ToRadians() ).Rounded();
                mCollided = true;
            }

            SetFacePosition( perimiterPos );
        }

        public Vector3 Transform2dTo3d( Vector2 vec2d )
        {
            return new Vector3( vec2d.X, -vec2d.Y, 0 )
                       .Transform( Utils.RotateOntoQ( Vector3.UnitZ, Normal ) )
                       .Rotate( Normal, CubeFace.Rotation )
                       + Normal;
        }

        public Vector2 Transform3dTo2d( Vector3 vec3d )
        {
            vec3d = vec3d.Rotate( Normal, -CubeFace.Rotation )
                         .Transform( Utils.RotateOntoQ( Normal, Vector3.UnitZ ) );
            return new Vector2( vec3d.X, -vec3d.Y );
        }

        private IEnumerable< CompassDirection > ClampWorldPosition()
        {
            if ( mWorldPosition.X > 1 )
            {
                mWorldPosition.X = 1;

                var dir = CubeFace.VectorToDirection( Vector3.UnitX );
                if ( dir.HasValue )
                    yield return dir.Value;
            }
            else if ( mWorldPosition.X < -1 )
            {
                mWorldPosition.X = -1;

                var dir = CubeFace.VectorToDirection( -Vector3.UnitX );
                if ( dir.HasValue )
                    yield return dir.Value;
            }

            if ( mWorldPosition.Y > 1 )
            {
                mWorldPosition.Y = 1;

                var dir = CubeFace.VectorToDirection( Vector3.UnitY );
                if ( dir.HasValue )
                    yield return dir.Value;
            }
            else if ( mWorldPosition.Y < -1 )
            {
                mWorldPosition.Y = -1;

                var dir = CubeFace.VectorToDirection( -Vector3.UnitY );
                if ( dir.HasValue )
                    yield return dir.Value;
            }

            if ( mWorldPosition.Z > 1 )
            {
                mWorldPosition.Z = 1;

                var dir = CubeFace.VectorToDirection( Vector3.UnitZ );
                if ( dir.HasValue )
                    yield return dir.Value;
            }
            else if ( mWorldPosition.Z < -1 )
            {
                mWorldPosition.Z = -1;

                var dir = CubeFace.VectorToDirection( -Vector3.UnitZ );
                if ( dir.HasValue )
                    yield return dir.Value;
            }
        }

    }
}
