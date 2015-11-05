using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using CyberCube.Physics;
using FarseerPhysics.Dynamics;
using CyberCube.Screens;
using FarseerPhysics.Common;

namespace CyberCube
{
    /// <summary>
    /// Represents an entire level. Contains the 6 faces of the cube.
    /// </summary>
    public abstract partial class Cube : DrawableCubeScreenGameComponent
    {
        public readonly BoundingBox BoundingBox = new BoundingBox( -Vector3.One, Vector3.One );

        protected Face mFrontFace;
        protected Face mBackFace;
        protected Face mTopFace;
        protected Face mBottomFace;
        protected Face mLeftFace;
        protected Face mRightFace;

        public BasicEffect Effect
        {
            get; protected set;
        }

        protected Vector3 mPosition = Vector3.Zero;
        protected Vector3 mRotation = Vector3.Zero;
        protected Vector3 mScale = Vector3.One;

        public float CameraDistance
        {
            get; protected set;
        }

        public Face CurrentFace
        {
            get; protected set;
        }

        public Direction UpDir
        {
            get; protected set;
        }

        public Vector3 ComputeUpVector()
        {
            return Utils.RoundVector(
                       CurrentFace.UpVec.Rotate(
                           CurrentFace.Normal,
                           UpDir.Angle ) );
        }

        /*
        Picture the unfolded cube:
            T
            F
          L X R
            B
        Where:
            T = top
            F = front
            L = left
            X = bottom
            R = right
            B = back
        The adjacent faces are labeled with respect to this diagram, noting the orientation of each 
        letter (face) when folded into a cube.
        */

        public Cube( CubeGame game, CubeScreen screen )
            : base( game, screen )
        {
            Screen = screen;

            mFrontFace = NewFace( "Front", Vector3.UnitZ, Vector3.UnitY, Direction.Up );
            mBackFace = NewFace( "Back", -Vector3.UnitZ, -Vector3.UnitY, Direction.Right );
            mTopFace = NewFace( "Top", Vector3.UnitY, -Vector3.UnitZ, Direction.Up );
            mBottomFace = NewFace( "Bottom", -Vector3.UnitY, Vector3.UnitZ, Direction.Up );
            mLeftFace = NewFace( "Left", -Vector3.UnitX, Vector3.UnitZ, Direction.Right );
            mRightFace = NewFace( "Right", Vector3.UnitX, Vector3.UnitZ, Direction.Left );

            mFrontFace.BackgroundColor = Color.Red;
            mBackFace.BackgroundColor = Color.Orange;
            mTopFace.BackgroundColor = Color.Blue;
            mBottomFace.BackgroundColor = Color.Green;
            mLeftFace.BackgroundColor = Color.Purple;
            mRightFace.BackgroundColor = Color.Yellow;

            //ConnectFaces();

            CameraDistance = 6;
            CurrentFace = mFrontFace;
            UpDir = CompassDirection.North;
        }

        protected abstract Face NewFace( string name, Vector3 normal, Vector3 up, Direction rotation );

        protected void ConnectFaces()
        {
            mFrontFace.NorthFace = mTopFace;
            mFrontFace.EastFace = mRightFace;
            mFrontFace.SouthFace = mBottomFace;
            mFrontFace.WestFace = mLeftFace;
			mFrontFace.OppositeFace = mBackFace;

            mBackFace.NorthFace = mBottomFace;
            mBackFace.EastFace = mRightFace;
            mBackFace.SouthFace = mTopFace;
            mBackFace.WestFace = mLeftFace;
			mBackFace.OppositeFace = mFrontFace;

            mTopFace.NorthFace = mBackFace;
            mTopFace.EastFace = mRightFace;
            mTopFace.SouthFace = mFrontFace;
            mTopFace.WestFace = mLeftFace;
			mTopFace.OppositeFace = mBottomFace;

            mBottomFace.NorthFace = mFrontFace;
            mBottomFace.EastFace = mRightFace;
            mBottomFace.SouthFace = mBackFace;
            mBottomFace.WestFace = mLeftFace;
			mBottomFace.OppositeFace = mTopFace;

            mLeftFace.NorthFace = mFrontFace;
            mLeftFace.EastFace = mBottomFace;
            mLeftFace.SouthFace = mBackFace;
            mLeftFace.WestFace = mTopFace;
			mLeftFace.OppositeFace = mRightFace;

            mRightFace.NorthFace = mFrontFace;
            mRightFace.EastFace = mTopFace;
            mRightFace.SouthFace = mBackFace;
            mRightFace.WestFace = mBottomFace;
			mRightFace.OppositeFace = mLeftFace;
        }

        internal void Save( string name )
        {
            foreach ( Face face in Faces )
                WorldSerializer.Serialize( face.World, $"{name}_{face.Name.ToLower()}.ccf" );
        }

        internal void Load( string name )
        {
            foreach ( Face face in Faces )
                face.World = WorldSerializer.Deserialize( $"{name}_{face.Name.ToLower()}.ccf" );
        }

        public const float NEAR_PLANE = 1f;
        public const float FAR_PLANE = 10f;

        public override void Initialize()
        {
            base.Initialize();

            ConnectFaces();

            Effect = new BasicEffect( GraphicsDevice );
            Effect.AmbientLightColor = new Vector3( 1.0f, 1.0f, 1.0f );
            Effect.DirectionalLight0.Enabled = true;
            Effect.DirectionalLight0.DiffuseColor = Vector3.One;
            Effect.DirectionalLight0.Direction = Vector3.Normalize( Vector3.One );
            Effect.LightingEnabled = true;

            //COMP7051
            Effect.Projection = Matrix.CreatePerspectiveFieldOfView( MathHelper.Pi / 4.0f,
                (float) Game.Window.ClientBounds.Width / (float) Game.Window.ClientBounds.Height, NEAR_PLANE, FAR_PLANE );

            Screen.Camera.Position = CameraDistance * CurrentFace.Normal;
            Screen.Camera.Target = mPosition;
            Screen.Camera.UpVector = ComputeUpVector();

            Effect.View = Screen.Camera.View;

            foreach ( Face face in Faces )
                face.Initialize();
        }

        public override void Update( GameTime gameTime )
        {
            Matrix R = Matrix.CreateFromYawPitchRoll( mRotation.Y, mRotation.X, mRotation.Z );
            Matrix T = Matrix.CreateTranslation( mPosition );
            Matrix S = Matrix.CreateScale( mScale );
            Effect.World = S * R * T;

            Effect.View = Screen.Camera.View;

            base.Update( gameTime );

            foreach ( Face face in Faces )
                face.Update( gameTime );
        }


        public override void Draw( GameTime gameTime )
        {
            foreach ( Face face in Faces )
                face.Render2D( gameTime );

            Effect.TextureEnabled = true;

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullClockwiseFace;
            GraphicsDevice.RasterizerState = rs;

            foreach ( Face face in Faces )
                face.Render3D( Effect );
        }

    }
}
