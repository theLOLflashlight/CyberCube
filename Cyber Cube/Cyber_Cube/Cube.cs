using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Cyber_Cube
{
    /// <summary>
    /// Represents an entire level. Contains the 6 faces of the cube.
    /// </summary>
    public partial class Cube : DrawableCubeGameComponent
    {
        private Face mFrontFace;
        private Face mBackFace;
        private Face mTopFace;
        private Face mBottomFace;
        private Face mLeftFace;
        private Face mRightFace;

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

        public enum CubeMode { Edit, Play }

        public CubeMode Mode { get; set; }

        public Face CurrentFace { get; private set; }

        public Direction UpDir { get; private set; }

        public Vector3 ComputeUpVector()
        {
            return Utils.RoundVector(
                       CurrentFace.UpVec.Rotate(
                           CurrentFace.Normal,
                           UpDir.ToRadians() ) );
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
            mFrontFace = new Face( this, "Front", Vector3.UnitZ, Vector3.UnitY, Direction.North );
            mBackFace = new Face( this, "Back", -Vector3.UnitZ, -Vector3.UnitY, Direction.East );
            mTopFace = new Face( this, "Top", Vector3.UnitY, -Vector3.UnitZ, Direction.North );
            mBottomFace = new Face( this, "Bottom", -Vector3.UnitY, Vector3.UnitZ, Direction.North );
            mLeftFace = new Face( this, "Left", -Vector3.UnitX, Vector3.UnitZ, Direction.East );
            mRightFace = new Face( this, "Right", Vector3.UnitX, Vector3.UnitZ, Direction.West );

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

            mBackFace.NorthFace = mBottomFace;
            mBackFace.EastFace = mRightFace;
            mBackFace.SouthFace = mTopFace;
            mBackFace.WestFace = mLeftFace;

            mTopFace.NorthFace = mBackFace;
            mTopFace.EastFace = mRightFace;
            mTopFace.SouthFace = mFrontFace;
            mTopFace.WestFace = mLeftFace;

            mBottomFace.NorthFace = mFrontFace;
            mBottomFace.EastFace = mRightFace;
            mBottomFace.SouthFace = mBackFace;
            mBottomFace.WestFace = mLeftFace;

            mLeftFace.NorthFace = mFrontFace;
            mLeftFace.EastFace = mBottomFace;
            mLeftFace.SouthFace = mBackFace;
            mLeftFace.WestFace = mTopFace;

            mRightFace.NorthFace = mFrontFace;
            mRightFace.EastFace = mTopFace;
            mRightFace.SouthFace = mBackFace;
            mRightFace.WestFace = mBottomFace;
        }

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
                (float) Game.Window.ClientBounds.Width / (float) Game.Window.ClientBounds.Height, 1f, 10f );

            Game.Camera.Position = CameraDistance * CurrentFace.Normal;
            Game.Camera.Target = mPosition;
            Game.Camera.UpVector = ComputeUpVector();

            Effect.View = Game.Camera.View;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update( GameTime gameTime )
        {
            var input = Game.Input;

            if ( Mode == CubeMode.Edit )
                Game.Camera.Target = mPosition;

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

            base.Draw( gameTime );
        }

    }
}
