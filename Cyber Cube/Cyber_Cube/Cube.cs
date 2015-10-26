using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using CyberCube.Physics;
using FarseerPhysics.Dynamics;

namespace CyberCube
{
    /// <summary>
    /// Represents an entire level. Contains the 6 faces of the cube.
    /// </summary>
    public partial class Cube : DrawableCubeGameComponent
    {
        public readonly BoundingBox BoundingBox = new BoundingBox( -Vector3.One, Vector3.One );

        private Face mFrontFace;
        private Face mBackFace;
        private Face mTopFace;
        private Face mBottomFace;
        private Face mLeftFace;
        private Face mRightFace;

        /// <summary>
        /// Gets each face of the cube.
        /// </summary>
        public IEnumerable< Face > Faces
        {
            get {
                yield return mFrontFace;
                yield return mBackFace;
                yield return mTopFace;
                yield return mBottomFace;
                yield return mLeftFace;
                yield return mRightFace;
            }
        }

        public BasicEffect Effect { get; private set; }

        private Vector3 mPosition = Vector3.Zero;
        private Vector3 mRotation = Vector3.Zero;
        private Vector3 mScale = Vector3.One;

        public float CameraDistance { get; private set; }

        public enum CubeMode { Play, Edit }

        public CubeMode Mode { get; set; }

        public Face CurrentFace { get; private set; }

        public Direction UpDir { get; private set; }

        public Vector3 ComputeUpVector()
        {
            return Utils.RoundVector(
                       CurrentFace.UpVec.Rotate(
                           CurrentFace.Normal,
                           UpDir.Angle ) );
        }

        public void CenterOnPlayer( Player player )
        {
            CurrentFace = player.CubeFace;
            UpDir = player.UpDir;

            Game.Camera.AnimatePosition( CameraDistance * CurrentFace.Normal, CameraDistance );
            Game.Camera.AnimateUpVector( ComputeUpVector(), 1 );
            //Game.Camera.SkipAnimation();
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

        public Cube( CubeGame game )
            : base( game )
        {
            mFrontFace = new Face( this, "Front", Vector3.UnitZ, Vector3.UnitY, Direction.Up );
            mBackFace = new Face( this, "Back", -Vector3.UnitZ, -Vector3.UnitY, Direction.Right );
            mTopFace = new Face( this, "Top", Vector3.UnitY, -Vector3.UnitZ, Direction.Up );
            mBottomFace = new Face( this, "Bottom", -Vector3.UnitY, Vector3.UnitZ, Direction.Up );
            mLeftFace = new Face( this, "Left", -Vector3.UnitX, Vector3.UnitZ, Direction.Right );
            mRightFace = new Face( this, "Right", Vector3.UnitX, Vector3.UnitZ, Direction.Left );

            mFrontFace.BackgroundColor = Color.Red;
            mBackFace.BackgroundColor = Color.Orange;
            mTopFace.BackgroundColor = Color.Blue;
            mBottomFace.BackgroundColor = Color.Green;
            mLeftFace.BackgroundColor = Color.White;
            mRightFace.BackgroundColor = Color.Yellow;

            ConnectFaces();

            CameraDistance = 6;
            CurrentFace = mFrontFace;
            UpDir = CompassDirection.North;

            Game.Components.ComponentAdded += ( s, e ) => {
                if ( ReferenceEquals( this, e.GameComponent ) )
                    foreach ( Face face in Faces )
                        Game.Components.Add( face );
            };

            Game.Components.ComponentRemoved += ( s, e ) => {
                if ( ReferenceEquals( this, e.GameComponent ) )
                    foreach ( Face face in Faces )
                        Game.Components.Remove( face );
            };
        }

        private void ConnectFaces()
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

        public const float NEAR_PLANE = 1f;
        public const float FAR_PLANE = 10f;

        public override void Initialize()
        {
            base.Initialize();

            Effect = new BasicEffect( GraphicsDevice );
            Effect.AmbientLightColor = new Vector3( 1.0f, 1.0f, 1.0f );
            Effect.DirectionalLight0.Enabled = true;
            Effect.DirectionalLight0.DiffuseColor = Vector3.One;
            Effect.DirectionalLight0.Direction = Vector3.Normalize( Vector3.One );
            Effect.LightingEnabled = true;

            //COMP7051
            Effect.Projection = Matrix.CreatePerspectiveFieldOfView( MathHelper.Pi / 4.0f,
                (float) Game.Window.ClientBounds.Width / (float) Game.Window.ClientBounds.Height, NEAR_PLANE, FAR_PLANE );

            Game.Camera.Position = CameraDistance * CurrentFace.Normal;
            Game.Camera.Target = mPosition;
            Game.Camera.UpVector = ComputeUpVector();

            Effect.View = Game.Camera.View;
        }

        public void Reset()
        {
            foreach ( var face in Faces )
                face.ResetWorld();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update( GameTime gameTime )
        {
            var input = Game.Input;

            if ( Mode == CubeMode.Edit )
            {
                Game.Camera.Target = mPosition;
                EditPass( gameTime );
            }

            Matrix R = Matrix.CreateFromYawPitchRoll( mRotation.Y, mRotation.X, mRotation.Z );
            Matrix T = Matrix.CreateTranslation( mPosition );
            Matrix S = Matrix.CreateScale( mScale );
            Effect.World = S * R * T;

            Effect.View = Game.Camera.View;

            base.Update( gameTime );
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

        public Face GetFaceFromPosition( Vector3 vec )
        {
            var x = vec.X;
            var y = vec.Y;
            var z = vec.Z;

            var l = -1;
            var t = 1;
            var r = 1;
            var e = -1;
            var f = 1;
            var b = -1;

            x = MathHelper.Clamp( x, l, r );
            y = MathHelper.Clamp( y, e, t );
            z = MathHelper.Clamp( z, b, f );

            var dl = Math.Abs( x - l );
            var dr = Math.Abs( x - r );
            var dt = Math.Abs( y - t );
            var de = Math.Abs( y - e );
            var df = Math.Abs( z - f );
            var db = Math.Abs( z - b );

            var m = Math.Min( Math.Min( df, Math.Min( dl, dr ) ),
                              Math.Min( db, Math.Min( dt, de ) ) );

            if ( m == df )
                return mFrontFace;

            if ( m == db )
                return mBackFace;

            if ( m == dt )
                return mTopFace;

            if ( m == de )
                return mBottomFace;

            if ( m == dl )
                return mLeftFace;

            if ( m == dr )
                return mRightFace;

            throw new Tools.WtfException();
        }

		public Vector2 ComputeFacePosition( Vector3 worldPosition, Face cubeFace )
		{
			float adjustingFactor = Cube.Face.SIZE / 2;
			return Transform3dTo2d( worldPosition, cubeFace )
				   * adjustingFactor
				   + new Vector2( adjustingFactor );
		}

		private Vector2 Transform3dTo2d( Vector3 vec3d, Face cubeFace )
		{
			vec3d = vec3d.Rotate( cubeFace.Normal, -cubeFace.Rotation )
						 .Transform( Utils.RotateOntoQ( cubeFace.Normal, Vector3.UnitZ ) );
			return new Vector2( vec3d.X, -vec3d.Y );
		}

    }
}
