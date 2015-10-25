using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace CyberCube
{
    public abstract class Actor : DrawableCubeGameComponent
    {
        protected Vector3 mWorldPosition;

        public Vector3 WorldPosition
        {
            get {
                return mWorldPosition;
            }
        }

        protected Body Body
        {
            get; private set;
        }

        public Vector2 Position
        {
            get {
                return Body.Position * Physics.Constants.UNIT_TO_PIXEL;
            }
            set {
                Body.Position = value * Physics.Constants.PIXEL_TO_UNIT;
                SetFacePosition( value );
            }
        }

        public Vector2 Velocity
        {
            get {
                return Body.LinearVelocity;
            }
            set {
                Body.LinearVelocity = value;
            }
        }

        public float VelocityX
        {
            get {
                return Body.LinearVelocity.X;
            }
            set {
                Body.LinearVelocity = new Vector2( value, VelocityY );
            }
        }

        public float VelocityY
        {
            get {
                return Body.LinearVelocity.Y;
            }
            set {
                Body.LinearVelocity = new Vector2( VelocityX, value );
            }
        }


        public bool FreeFall
        {
            get {
                return true;
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

        private Direction mUpDir;

        public Direction UpDir
        {
            get {
                return mUpDir;
            }
            set {
                mUpDir = value;
                Body.Rotation = mUpDir.Angle;
                Body.AdHocGravity = Vector2.UnitY.Rotate( -Body.Rotation ).Rounded();
                Body.Awake = true;
            }
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
            mUpDir = upDir;

            CubeFace = Cube.GetFaceFromPosition( mWorldPosition );
        }

        protected virtual Body CreateBody( World world )
        {
            Body body = BodyFactory.CreateCircle(
                CubeFace.World,
                3 * Physics.Constants.PIXEL_TO_UNIT,
                1,
                ComputeFacePosition() * Physics.Constants.PIXEL_TO_UNIT );

            body.BodyType = BodyType.Dynamic;
            body.Rotation = UpDir.Angle;

            return body;
        }

        public Vector2 Gravity
        {
            get {
                return Body.GravityScale *
                       (Body.UseAdHocGravity
                       ? Body.AdHocGravity
                       : CubeFace.World.Gravity);
            }
            set {
                Body.UseAdHocGravity = true;
                Body.GravityScale = value.Length();
                Body.AdHocGravity = Vector2.Normalize( value );
            }
        }

        public void ResetGravity()
        {
            Body.UseAdHocGravity = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            Body = CreateBody( CubeFace.World );

            Body.FixedRotation = true;
            Body.GravityScale = 20f;
            Body.UseAdHocGravity = true;
            Body.AdHocGravity = Vector2.UnitY;
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            SetFacePosition( Body.Position * Physics.Constants.UNIT_TO_PIXEL );
        }

        protected virtual void ApplyRotation( CompassDirection dir )
        {
            Cube.Face nextFace = CubeFace.AdjacentFace( dir );
            var backDir = nextFace.BackwardsDirectionFrom( CubeFace );

            float mass = Body.Mass;
            Body body = Body.DeepClone( nextFace.World );
            CubeFace.World.RemoveBody( Body );
            Body = body;
            Body.Mass = mass;

            float pastRotation = Body.Rotation;

            this.UpDir = Cube.GetNextUpDirection( UpDir, dir, backDir );
            this.CubeFace = nextFace;

            Body.Position = ComputeFacePosition() * Physics.Constants.PIXEL_TO_UNIT;

            Velocity = Velocity.Rotate( pastRotation - Body.Rotation );
        }


        protected Vector2 ComputeFacePosition()
        {
            float adjustingFactor = Cube.Face.SIZE / 2;
            return Transform3dTo2d( WorldPosition )
                   * adjustingFactor
                   + new Vector2( adjustingFactor );
        }

        private void SetFacePosition( Vector2 vec2d )
        {
            float adjustingFactor = Cube.Face.SIZE / 2;
            vec2d -= new Vector2( adjustingFactor );
            vec2d /= adjustingFactor;

            mWorldPosition = Transform2dTo3d( vec2d );

            foreach ( var dir in ClampWorldPosition() )
                ApplyRotation( dir );
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
                if ( dir != null )
                    yield return dir.Value;
            }
            else if ( mWorldPosition.X < -1 )
            {
                mWorldPosition.X = -1;

                var dir = CubeFace.VectorToDirection( -Vector3.UnitX );
                if ( dir != null )
                    yield return dir.Value;
            }

            if ( mWorldPosition.Y > 1 )
            {
                mWorldPosition.Y = 1;

                var dir = CubeFace.VectorToDirection( Vector3.UnitY );
                if ( dir != null )
                    yield return dir.Value;
            }
            else if ( mWorldPosition.Y < -1 )
            {
                mWorldPosition.Y = -1;

                var dir = CubeFace.VectorToDirection( -Vector3.UnitY );
                if ( dir != null )
                    yield return dir.Value;
            }

            if ( mWorldPosition.Z > 1 )
            {
                mWorldPosition.Z = 1;

                var dir = CubeFace.VectorToDirection( Vector3.UnitZ );
                if ( dir != null )
                    yield return dir.Value;
            }
            else if ( mWorldPosition.Z < -1 )
            {
                mWorldPosition.Z = -1;

                var dir = CubeFace.VectorToDirection( -Vector3.UnitZ );
                if ( dir != null )
                    yield return dir.Value;
            }
        }

    }
}
