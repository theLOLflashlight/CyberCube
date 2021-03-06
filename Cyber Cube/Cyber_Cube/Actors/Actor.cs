﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using CyberCube.Screens;
using CyberCube.Physics;
using CyberCube.Tools;

namespace CyberCube.Actors
{
    public abstract class Actor : DrawableCubeScreenGameComponent
    {
        protected Vector3 mCubePosition;

        protected bool mRotated;

        public Vector3 CubePosition
        {
            get {
                return mCubePosition;
            }
        }

        public Vector3 WorldPosition
        {
            get {
                return mCubePosition + Cube.Position;
            }
        }

        public Body Body
        {
            get; private set;
        }

        public Vector2 Position
        {
            get {
                return Body.Position.ToPixels();
            }
            set {
                Body.Position = value.ToUnits();
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


        public virtual bool FreeFall
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

        public Direction UpDir
        {
            get {
                return (Direction) Rotation;
            }
            set {
                Rotation = value.Angle;
            }
        }

        private float mRotation;

        public float Rotation
        {
            get {
                return Body?.Rotation ?? mRotation;
            }
            set {
                mRotation = value;
                if ( Body != null )
                {
                    Body.Rotation = mRotation;
                    Body.AdHocGravity = Vector2.UnitY.Rotate( mRotation );
                    Body.Awake = true;
                }
            }
        }

        public Vector2 UpVector
        {
            get {
                return Vector2.UnitY.Rotate( Rotation );
            }
        }

        public Actor( CubeGame game, CubeScreen screen )
            : base( game, screen )
        {
        }

        public Actor( CubeGame game, CubeScreen screen, Cube cube, Vector3 worldPos, Direction upDir )
            : this( game, screen )
        {
            Cube = cube;
            mCubePosition = worldPos;
            UpDir = upDir;

            CubeFace = Cube.GetFaceFromPosition( mCubePosition );
        }

        protected virtual Body CreateBody( World world )
        {
            Body body = BodyFactory.CreateBody(
                CubeFace.World,
                ComputeFacePosition().ToUnits(),
                Rotation );

            body.BodyType = BodyType.Dynamic;

            return body;
        }

        protected override void Dispose( bool disposing )
        {
            base.Dispose( disposing );
            if ( disposing )
            {
                Body.Dispose();
            }
        }

        protected virtual void ReconstructBody()
        {
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
            ReconstructBody();
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            mRotation = Body.Rotation;

            SetFacePosition( Body.Position.ToPixels() );
        }

        protected virtual void ApplyRotation( CompassDirection dir )
        {
            mRotated = true;

            Cube.Face nextFace = CubeFace.AdjacentFace( dir );
            var backDir = nextFace.BackwardsDirectionFrom( CubeFace );

            float mass = Body.Mass;
            Body body = Body.DeepClone( nextFace.World );
            CubeFace.World.RemoveBody( Body );
            Body = body;
            Body.Mass = mass;

            float pastUpDirAngle = UpDir.Angle;
            float rotDif = UpDir.Angle - Rotation;

            UpDir = Cube.GetNextUpDirection( UpDir, dir, backDir );
            CubeFace = nextFace;
            Rotation -= rotDif;

            Body.Position = ComputeFacePosition().ToUnits();

            Velocity = Velocity.Rotate( UpDir.Angle - pastUpDirAngle );

            ReconstructBody();
        }


        protected Vector2 ComputeFacePosition()
        {
            float adjustingFactor = Cube.Face.SIZE / 2;
            return Transform3dTo2d( CubePosition )
                   * adjustingFactor
                   + new Vector2( adjustingFactor );
        }

        private void SetFacePosition( Vector2 vec2d )
        {
            float adjustingFactor = Cube.Face.SIZE / 2;
            vec2d -= new Vector2( adjustingFactor );
            vec2d /= adjustingFactor;

            mCubePosition = Transform2dTo3d( vec2d );

            foreach ( var dir in ClampWorldPosition() )
                ApplyRotation( dir );
        }

        public Vector3 Transform2dTo3d( Vector2 vec2d )
        {
            return new Vector3( vec2d.X, -vec2d.Y, 0 )
                       .Transform( VectorUtils.RotateOnto_Q( Vector3.UnitZ, Normal ) )
                       .Rotate( Normal, CubeFace.Rotation )
                       + Normal;
        }

        public Vector2 Transform3dTo2d( Vector3 vec3d )
        {
            vec3d = vec3d.Rotate( Normal, -CubeFace.Rotation )
                         .Transform( VectorUtils.RotateOnto_Q( Normal, Vector3.UnitZ ) );
            return new Vector2( vec3d.X, -vec3d.Y );
        }

        private void WrapCubePosition( float diff )
        {
            if ( Normal == Vector3.UnitX || Normal == -Vector3.UnitX )
                mCubePosition.X -= Math.Abs( diff ) * Normal.X;

            else if ( Normal == Vector3.UnitY || Normal == -Vector3.UnitY )
                mCubePosition.Y -= Math.Abs( diff ) * Normal.Y;

            else if ( Normal == Vector3.UnitZ || Normal == -Vector3.UnitZ )
                mCubePosition.Z -= Math.Abs( diff ) * Normal.Z;
        }

        private IEnumerable< CompassDirection > ClampWorldPosition()
        {
            if ( mCubePosition.X > 1 )
            {
                WrapCubePosition( mCubePosition.X - 1 );
                mCubePosition.X = 1;

                var dir = CubeFace.VectorToDirection( Vector3.UnitX );
                if ( dir != null )
                    yield return dir.Value;
            }
            else if ( mCubePosition.X < -1 )
            {
                WrapCubePosition( mCubePosition.X + 1 );
                mCubePosition.X = -1;

                var dir = CubeFace.VectorToDirection( -Vector3.UnitX );
                if ( dir != null )
                    yield return dir.Value;
            }

            if ( mCubePosition.Y > 1 )
            {
                WrapCubePosition( mCubePosition.Y - 1 );
                mCubePosition.Y = 1;

                var dir = CubeFace.VectorToDirection( Vector3.UnitY );
                if ( dir != null )
                    yield return dir.Value;
            }
            else if ( mCubePosition.Y < -1 )
            {
                WrapCubePosition( mCubePosition.Y + 1 );
                mCubePosition.Y = -1;

                var dir = CubeFace.VectorToDirection( -Vector3.UnitY );
                if ( dir != null )
                    yield return dir.Value;
            }

            if ( mCubePosition.Z > 1 )
            {
                WrapCubePosition( mCubePosition.Z - 1 );
                mCubePosition.Z = 1;

                var dir = CubeFace.VectorToDirection( Vector3.UnitZ );
                if ( dir != null )
                    yield return dir.Value;
            }
            else if ( mCubePosition.Z < -1 )
            {
                WrapCubePosition( mCubePosition.Z + 1 );
                mCubePosition.Z = -1;

                var dir = CubeFace.VectorToDirection( -Vector3.UnitZ );
                if ( dir != null )
                    yield return dir.Value;
            }
        }

    }
}
