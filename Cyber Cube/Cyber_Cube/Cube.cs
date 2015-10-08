using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
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

        public class Face
        {
            public Face NorthFace { get; internal set; }
            public Face EastFace { get; internal set; }
            public Face SouthFace { get; internal set; }
            public Face WestFace { get; internal set; }

            public string Name { get; private set; }

            public Face( string name )
                : this( null, null, null, null )
            {
                Name = name;
            }

            public Face( Face northFace, Face eastFace, Face southFace, Face westFace )
            {
                NorthFace = northFace;
                EastFace = eastFace;
                SouthFace = southFace;
                WestFace = westFace;
            }
        }

        private Face mFrontFace = new Face( "Front" );
        private Face mBackFace = new Face( "Back" );
        private Face mLeftFace = new Face( "Left" );
        private Face mRightFace = new Face( "Right" );
        private Face mTopFace = new Face( "Top" );
        private Face mBottomFace = new Face( "Bottom" );

        private SpriteFont font;
        private SpriteBatch spriteBatch;

        public BasicEffect effect;
        public Vector3 size;
        public Vector3 position;
        public VertexPositionNormalTexture[] vertices;
        public int primitiveCount;

        Vector3 mRotation = Vector3.UnitZ;
        Vector3 mScale = Vector3.One;
        Vector3 mPosition = Vector3.Zero;

        Vector3 mRotationTarget = Vector3.UnitZ;

        private SpriteFont fontLarge;

        public Face CurrentFace { get; private set; }

        public Direction Up { get; private set; }

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

            BuildShape();

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
            Matrix projection = Matrix.CreatePerspectiveFieldOfView( (float) Math.PI / 4.0f,
                                            (float) Game.Window.ClientBounds.Width / (float) Game.Window.ClientBounds.Height, 1f, 10f );
            effect.Projection = projection;
            Matrix V = Matrix.CreateTranslation( 0f, 0f, -10f );
            effect.View = V;

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

            if ( input.Keyboard_WasKeyPressed( Keys.Right ) )
                this.RotateRight();

            if ( input.Keyboard_WasKeyPressed( Keys.Left ) )
                this.RotateLeft();

            if ( input.Keyboard_WasKeyPressed( Keys.Up ) )
                this.RotateUp();

            if ( input.Keyboard_WasKeyPressed( Keys.Down ) )
                this.RotateDown();


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

            bool moved = false;

            // Keyboard
            {
                if ( input.Keyboard.IsKeyDown( Keys.Right ) )
                {
                    mPosition.X += 0.015f;
                    moved = true;
                }

                if ( input.Keyboard.IsKeyDown( Keys.Left ) )
                {
                    mPosition.X -= 0.015f;
                    moved = true;
                }

                if ( input.Keyboard.IsKeyDown( Keys.Up ) )
                {
                    mPosition.Y += 0.015f;
                    moved = true;
                }

                if ( input.Keyboard.IsKeyDown( Keys.Down ) )
                {
                    mPosition.Y -= 0.015f;
                    moved = true;
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

            Vector3Approach( ref mScale, Vector3.One, new Vector3( 0.005f ) );
            if ( !moved )
            {
                FloatApproach( ref mPosition.X, 0, 0.01f );
                FloatApproach( ref mPosition.Y, 0, 0.01f );
            }

            if ( mScale.X < 0 )
                mScale.X = 0;
            if ( mScale.Y < 0 )
                mScale.Y = 0;
            if ( mScale.Z < 0 )
                mScale.Z = 0;

            Vector3Approach( ref mRotation, mRotationTarget, new Vector3( 0.025f ) );

            mPosition.Z = 0;
            mPosition.Z = 5f - mPosition.Length();

            Matrix R = Matrix.CreateFromYawPitchRoll( mRotation.Y * MathHelper.PiOver2,
                                                      mRotation.X * MathHelper.PiOver2,
                                                      mRotation.Z * MathHelper.PiOver2 );
            Matrix T = Matrix.CreateTranslation( mPosition );
            Matrix S = Matrix.CreateScale( mScale );
            effect.World = S * R * T;

            effect.DirectionalLight0.Direction = Vector3.Normalize( lightDir );

            base.Update( gameTime );
        }

        Matrix RotateVecToZAxis( Vector3 v )
        {
            v.Normalize();
            Vector3 axis = Vector3.Cross( v, Vector3.UnitZ );
            float partialAngle = (float) Math.Asin( axis.Length() );
            float finalAngle = v.Z < 0f
                               ? MathHelper.PiOver2 - partialAngle + MathHelper.PiOver2
                               : partialAngle;
            axis.Normalize();

            return Matrix.CreateFromAxisAngle( axis, finalAngle );
        }

        private static void FloatApproach( ref float variable, float target, float step )
        {
            if ( variable > target )
                variable -= Math.Min( variable - target, step );

            else if ( variable < target )
                variable += Math.Min( target - variable, step );
        }

        private static void Vector3Approach( ref Vector3 variable, Vector3 target, Vector3 step )
        {
            FloatApproach( ref variable.X, target.X, step.X );
            FloatApproach( ref variable.Y, target.Y, step.Y );
            FloatApproach( ref variable.Z, target.Z, step.Z );
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

            spriteBatch.DrawString( font, mRotationTarget.ToString(), Vector2.Zero, Color.White );
            spriteBatch.End();

            base.Draw( gameTime );
        }

    }
}
