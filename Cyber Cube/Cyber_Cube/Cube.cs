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

        private Face mFrontFace = new Face( "Front", Vector3.UnitZ, Vector3.UnitY );
        private Face mBackFace = new Face( "Back", -Vector3.UnitZ, -Vector3.UnitY );
        private Face mLeftFace = new Face( "Left", -Vector3.UnitX, Vector3.UnitZ );
        private Face mRightFace = new Face( "Right", Vector3.UnitX, Vector3.UnitZ );
        private Face mTopFace = new Face( "Top", Vector3.UnitY, -Vector3.UnitZ );
        private Face mBottomFace = new Face( "Bottom", -Vector3.UnitY, Vector3.UnitZ );

        private SpriteFont font;
        private SpriteBatch spriteBatch;

        public BasicEffect effect;
        public Vector3 size;
        public Vector3 position;
        public VertexPositionNormalTexture[] vertices;
        public int primitiveCount;

        private SpriteFont fontLarge;

        Vector3 mPosition = Vector3.Zero;
        Vector3 mRotation = Vector3.Zero;
        Vector3 mScale = Vector3.One;

        private Camera mCamera;

        public Face CurrentFace { get; private set; }

        public Direction Up { get; private set; }

        public Vector3 GetUpVector()
        {
            return Vector3.Transform( CurrentFace.Up,
                Matrix.CreateFromAxisAngle( CurrentFace.Normal, Up.ToRadians() ) );
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

        public Cube( Game game )
            : base( game )
        {
            this.size = new Vector3( 0, 0, 0 );
            this.position = new Vector3( 0, 0, 0 );
            this.vertices = null;

            CurrentFace = mFrontFace;
            Up = CompassDirections.North;

            mFrontFace.NorthFace = mTopFace;
            mFrontFace.EastFace = mRightFace;
            mFrontFace.SouthFace = mBottomFace;
            mFrontFace.WestFace = mLeftFace;

            mBackFace.NorthFace = mBottomFace;
            mBackFace.EastFace = mRightFace;
            mBackFace.SouthFace = mTopFace;
            mBackFace.WestFace = mLeftFace;

            mLeftFace.NorthFace = mFrontFace;
            mLeftFace.EastFace = mBottomFace;
            mLeftFace.SouthFace = mBackFace;
            mLeftFace.WestFace = mTopFace;

            mRightFace.NorthFace = mFrontFace;
            mRightFace.EastFace = mTopFace;
            mRightFace.SouthFace = mBackFace;
            mRightFace.WestFace = mBottomFace;

            mTopFace.NorthFace = mBackFace;
            mTopFace.EastFace = mRightFace;
            mTopFace.SouthFace = mFrontFace;
            mTopFace.WestFace = mLeftFace;

            mBottomFace.NorthFace = mFrontFace;
            mBottomFace.EastFace = mRightFace;
            mBottomFace.SouthFace = mBackFace;
            mBottomFace.WestFace = mLeftFace;
        }


        public override void Initialize()
        {
            base.Initialize();

            effect = new BasicEffect( GraphicsDevice );
            effect.AmbientLightColor = new Vector3( 0.0f, 1.0f, 0.0f );
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.DiffuseColor = Vector3.One;
            effect.DirectionalLight0.Direction = Vector3.Normalize( Vector3.One );
            effect.LightingEnabled = true;

            //COMP7051
            effect.Projection = Matrix.CreatePerspectiveFieldOfView( (float) Math.PI / 4.0f,
                                            (float) Game.Window.ClientBounds.Width / (float) Game.Window.ClientBounds.Height, 1f, 10f );

            mCamera = new Camera( Game,
                                  7 * CurrentFace.Normal,
                                  mPosition,
                                  GetUpVector() );

            effect.View = mCamera.View;

            //COMP7051
            size = new Vector3( 1, 1, 1 );
            position = new Vector3( 0, 0, 0 );
            BuildShape();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch( GraphicsDevice );
            fontLarge = Game.Content.Load<SpriteFont>( "MessageFontLarge" );
            font = Game.Content.Load<SpriteFont>( "MessageFont" );

            base.LoadContent();
        }

        public override void Update( GameTime gameTime )
        {
            var input = (Game as Game1).mInput;

            if ( input.Keyboard_WasAnyKeyPressed( new Keys[] { Keys.Right, Keys.Left, Keys.Up, Keys.Down } ) )
            {
                if ( input.Keyboard_WasKeyPressed( Keys.Right ) )
                    this.RotateRight();

                if ( input.Keyboard_WasKeyPressed( Keys.Left ) )
                    this.RotateLeft();

                if ( input.Keyboard_WasKeyPressed( Keys.Up ) )
                    this.RotateUp();

                if ( input.Keyboard_WasKeyPressed( Keys.Down ) )
                    this.RotateDown();


                mCamera.AnimatePosition( 7 * CurrentFace.Normal, 7 );
                mCamera.AnimateUpVector( GetUpVector(), 1 );
            }

            if ( input.Keyboard_WasKeyPressed( Keys.Space ) )
            {
                CurrentFace = mFrontFace;
                Up = CompassDirections.North;

                mCamera.AnimatePosition( 7 * CurrentFace.Normal, 7 );
                mCamera.AnimateTarget( mPosition, 10 );
                mCamera.AnimateUpVector( Vector3.Up, 1 );
            }

            mCamera.Update( gameTime );

            Vector3 lightDir = Vector3.Zero;

            // Game Pad
            {
                if ( input.GamePad.IsConnected )
                {
                    if ( input.GamePad.DPad.Left == ButtonState.Pressed )
                        mRotation.Y -= 0.005f;

                    if ( input.GamePad.DPad.Right == ButtonState.Pressed )
                        mRotation.Y += 0.005f;

                    float stickRY = input.GamePad.ThumbSticks.Right.Y;

                    if ( stickRY != input.OldGamePad.ThumbSticks.Right.Y )
                    {
                        mScale = new Vector3( (stickRY + 1.0f) / 2.0f + .5f );
                    }

                    lightDir += new Vector3( input.GamePad.ThumbSticks.Left.X,
                                             input.GamePad.ThumbSticks.Left.Y,
                                             1.0f );
                    lightDir.Normalize();
                }
                else
                {
                    //angle += 0.0003f * gameTime.ElapsedGameTime.Milliseconds;
                }
            }

            // Mouse
            {
                int scrollWheel = input.Mouse.ScrollWheelValue - input.OldMouse.ScrollWheelValue;

                if ( scrollWheel != input.OldMouse.ScrollWheelValue )
                {
                    mScale += new Vector3( scrollWheel / 2000f );
                }

                lightDir += new Vector3( -input.Mouse.X + Game.Window.ClientBounds.Width / 2,
                                         input.Mouse.Y - Game.Window.ClientBounds.Height / 2,
                                         1.0f );
                lightDir.Normalize();
            }

            Utils.Vector3Approach( ref mScale, Vector3.One, 0.005f );
            Utils.Vector3Approach( ref mPosition, Vector3.Zero, 0.01f );

            if ( mScale.X < 0 )
                mScale.X = 0;
            if ( mScale.Y < 0 )
                mScale.Y = 0;
            if ( mScale.Z < 0 )
                mScale.Z = 0;

            Matrix R = Matrix.CreateFromYawPitchRoll( mRotation.Y, mRotation.X, mRotation.Z );
            Matrix P = Matrix.CreateTranslation( mPosition );
            Matrix S = Matrix.CreateScale( mScale );
            effect.World = S * R * P;

            effect.View = mCamera.View;

            effect.DirectionalLight0.Direction = Vector3.Normalize( lightDir );

            base.Update( gameTime );
        }

        public override void Draw( GameTime gameTime )
        {

            if ( vertices != null )
            {
                RasterizerState rs = new RasterizerState();
                rs.CullMode = CullMode.CullClockwiseFace;
                GraphicsDevice.RasterizerState = rs;
                foreach ( EffectPass pass in effect.CurrentTechnique.Passes )
                {
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>( PrimitiveType.TriangleList, vertices, 0, primitiveCount );
                }
            }

            spriteBatch.Begin();
            spriteBatch.DrawString( fontLarge,
                                    CurrentFace.Name,
                                    new Vector2( Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2 ),
                                    Color.White,
                                    Up.ToRadians(),
                                    fontLarge.MeasureString( CurrentFace.Name ) / 2,
                                    1,
                                    SpriteEffects.None,
                                    0 );

            //spriteBatch.DrawString( font, mRotationTarget.ToString(), Vector2.Zero, Color.White );
            spriteBatch.End();

            base.Draw( gameTime );
        }

    }
}
