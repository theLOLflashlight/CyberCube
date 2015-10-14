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
    public partial class Cube : DrawableGameComponent
    {
        public new Game1 Game
        {
            get {
                return base.Game as Game1;
            }
        }

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

        private SpriteFont mFont;
        private SpriteBatch mSpriteBatch;

        public BasicEffect Effect { get; private set; }

        private SpriteFont mFontLarge;

        Vector3 mPosition = Vector3.Zero;
        Vector3 mRotation = Vector3.Zero;
        Vector3 mScale = Vector3.One;

        public Camera mCamera;

        public float CameraDistance { get; private set; }

        public enum CubeMode { Edit, Play }

        public CubeMode Mode { get; set; }

        public Face CurrentFace { get; private set; }

        public Direction UpDir { get; private set; }

        public Vector3 ComputeUpVector()
        {
            return Vector3.Transform( CurrentFace.UpVec,
                Matrix.CreateFromAxisAngle( CurrentFace.Normal, UpDir.ToRadians() ) );

            /*Vector3 up = Vector3.Transform( CurrentFace.Up,
                Matrix.CreateFromAxisAngle( CurrentFace.Normal, Up.ToRadians() ) );

            up.X = (float) Math.Round( up.X );
            up.Y = (float) Math.Round( up.Y );
            up.Z = (float) Math.Round( up.Z );

            return up;*/
        }

        public Player mPlayer;

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

        public Cube( Game game )
            : base( game )
        {
            mFrontFace = new Face( this, "Front", Vector3.UnitZ, Vector3.UnitY, Direction.North );
            mBackFace = new Face( this, "Back", -Vector3.UnitZ, -Vector3.UnitY, Direction.East );
            mTopFace = new Face( this, "Top", Vector3.UnitY, -Vector3.UnitZ, Direction.North );
            mBottomFace = new Face( this, "Bottom", -Vector3.UnitY, Vector3.UnitZ, Direction.North );
            mLeftFace = new Face( this, "Left", -Vector3.UnitX, Vector3.UnitZ, Direction.East );
            mRightFace = new Face( this, "Right", Vector3.UnitX, Vector3.UnitZ, Direction.West );

            SetUpFaces();

            CameraDistance = 7;

            CurrentFace = mFrontFace;
            UpDir = CompassDirections.North;

            foreach ( Face face in Faces )
                Game.Components.Add( face );

            mPlayer = new Player( this );
            Game.Components.Add( mPlayer );
        }

        private void SetUpFaces()
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
            Effect.Projection = Matrix.CreatePerspectiveFieldOfView( (float) Math.PI / 4.0f,
                                            (float) Game.Window.ClientBounds.Width / (float) Game.Window.ClientBounds.Height, 1f, 10f );

            mCamera = new Camera( Game,
                                  CameraDistance * CurrentFace.Normal,
                                  mPosition,
                                  ComputeUpVector() );

            mCamera.UsesSphereAnimation = true;

            Effect.View = mCamera.View;

            Color[] colors = {
                Color.Red,
                Color.Orange,
                Color.Blue,
                Color.Green,
                Color.White,
                Color.Yellow
            };

            foreach ( var pair in Enumerable.Zip( Faces, colors,
                      (f, c) => { return new Tuple<Face, Color>( f, c ); } ) )
                pair.Item1.BackgroundColor = pair.Item2;
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch( GraphicsDevice );
            mFontLarge = Game.Content.Load<SpriteFont>( "MessageFontLarge" );
            mFont = Game.Content.Load<SpriteFont>( "MessageFont" );

            base.LoadContent();
        }

        public override void Update( GameTime gameTime )
        {
            var input = Game.mInput;

            if ( Mode == CubeMode.Edit )
            {
                if ( input.Keyboard_WasAnyKeyPressed( new Keys[] { Keys.Right, Keys.Left, Keys.Up, Keys.Down, Keys.RightShift, Keys.LeftShift } ) )
                {
                    if ( input.Keyboard_WasKeyPressed( Keys.Right ) )
                        this.RotateRight();

                    if ( input.Keyboard_WasKeyPressed( Keys.Left ) )
                        this.RotateLeft();

                    if ( input.Keyboard_WasKeyPressed( Keys.Up ) )
                        this.RotateUp();

                    if ( input.Keyboard_WasKeyPressed( Keys.Down ) )
                        this.RotateDown();

                    if ( input.Keyboard_WasKeyPressed( Keys.RightShift ) )
                        this.RotateClockwise();

                    if ( input.Keyboard_WasKeyPressed( Keys.LeftShift ) )
                        this.RotateAntiClockwise();
                }
            }
            else
            {
                if ( input.Keyboard_WasKeyPressed( Keys.RightShift ) )
                    this.RotateClockwise();

                if ( input.Keyboard_WasKeyPressed( Keys.LeftShift ) )
                    this.RotateAntiClockwise();
            }

            if ( input.Keyboard_WasKeyPressed( Keys.Space ) )
            {
                //CurrentFace = mFrontFace;
                //Up = CompassDirections.North;
                //
                //mCamera.AnimatePosition( 7 * CurrentFace.Normal, 7 );
                //mCamera.AnimateTarget( mPosition, 10 );
                //mCamera.AnimateUpVector( Vector3.Up, 1 );

                Mode = Mode == CubeMode.Edit ? CubeMode.Play : CubeMode.Edit;
            }

            mCamera.Update( gameTime );

            Matrix R = Matrix.CreateFromYawPitchRoll( mRotation.Y, mRotation.X, mRotation.Z );
            Matrix T = Matrix.CreateTranslation( mPosition );
            Matrix S = Matrix.CreateScale( mScale );
            Effect.World = S * R * T;

            Effect.View = mCamera.View;

            base.Update( gameTime );
        }

        public override void Draw( GameTime gameTime )
        {
            GraphicsDevice.Clear( Color.CornflowerBlue );

            foreach ( Face face in Faces )
                face.Render2D( gameTime );

            Effect.TextureEnabled = true;

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullClockwiseFace;
            GraphicsDevice.RasterizerState = rs;

            foreach ( Face face in Faces )
                face.Render3D( Effect );


            string output = CurrentFace.Name;

            var pos = mFont.MeasureString( output );

            mSpriteBatch.Begin();
            mSpriteBatch.DrawString( mFont,
                                    output,
                                    new Vector2( Game.Window.ClientBounds.Width - pos.X, pos.Y ),
                                    Color.White,
                                    UpDir.ToRadians(),
                                    mFont.MeasureString( output ) / 2,
                                    1,
                                    SpriteEffects.None,
                                    0 );

            mSpriteBatch.DrawString( mFont, mCamera.Position.ToString(), Vector2.Zero, Color.White );
            mSpriteBatch.DrawString( mFont, mCamera.UpVector.ToString(), new Vector2( 0, 30 ), Color.White );
            mSpriteBatch.End();

            base.Draw( gameTime );
        }

    }
}
