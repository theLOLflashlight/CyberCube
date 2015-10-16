using Cyber_Cube.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{
    public class Player : DrawableGameComponent
    {
        public new Game1 Game
        {
            get {
                return base.Game as Game1;
            }
        }

        public Vector3 WorldPosition = Vector3.UnitZ;
        public Vector3 Velocity = Vector3.Zero;

        private Cube mCube;

        private Texture2D pixel;
        private SpriteBatch mSpriteBatch;

        private Cube.Face CurrentFace
        {
            get {
                return mCube.CurrentFace;
            }
        }

        public Vector3 Normal
        {
            get {
                return mCube.CurrentFace.Normal;
            }
        }

        public Player( Cube cube )
            : base( cube.Game )
        {
            mCube = cube;
            this.Visible = true;
            this.DrawOrder = 1;
        }

        public override void Initialize()
        {
            base.Initialize();

            pixel = new Texture2D( GraphicsDevice, 2, 2 );
            pixel.SetData( new[] { Color.White, Color.White, Color.White, Color.White } );

            mSpriteBatch = new SpriteBatch( GraphicsDevice );
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public Vector3 Transform2dTo3d( Vector2 vec2d )
        {
            var angle = mCube.UpDir.ToRadians() + CurrentFace.Orientation.ToRadians();

            return new Vector3( vec2d, 0 )
                       .Transform( Utils.RotateOntoQ( Vector3.UnitZ, Normal ) )
                       .Rotate( Normal, angle );
        }

        public Vector2 Transform3dTo2d( Vector3 vec3d )
        {
            var angle = CurrentFace.Orientation.ToRadians();

            vec3d = vec3d.Rotate( Normal, -angle )
                         .Transform( Utils.RotateOntoQ( Normal, Vector3.UnitZ ) );
            return new Vector2( vec3d.X, -vec3d.Y );
        }

        public Vector2 ComputeFacePosition()
        {
            var adjustingFactor = Cube.Face.SIZE / 2;
            return Transform3dTo2d( WorldPosition ) * adjustingFactor + new Vector2( adjustingFactor );
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            var input = Game.Input;

            var delta2d = Vector2.Zero;

            delta2d.X += input[ IO.Action.MoveRight ].Value;
            delta2d.X -= input[ IO.Action.MoveLeft ].Value;
            delta2d.Y += input[ IO.Action.MoveUp ].Value;
            delta2d.Y -= input[ IO.Action.MoveDown ].Value;

            Vector3 delta3d = Transform2dTo3d( delta2d );
            WorldPosition += (delta3d / Cube.Face.SIZE) * ((float) gameTime.ElapsedGameTime.TotalMilliseconds / 10f);

            if ( WorldPosition.X > 1 )
            {
                WorldPosition.X = 1;
                mCube.Rotate( CurrentFace.VectorToDirection( Vector3.UnitX ) );
            }
            else if ( WorldPosition.X < -1 )
            {
                WorldPosition.X = -1;
                mCube.Rotate( CurrentFace.VectorToDirection( -Vector3.UnitX ) );
            }

            if ( WorldPosition.Y > 1 )
            {
                WorldPosition.Y = 1;
                mCube.Rotate( CurrentFace.VectorToDirection( Vector3.UnitY ) );
            }
            else if ( WorldPosition.Y < -1 )
            {
                WorldPosition.Y = -1;
                mCube.Rotate( CurrentFace.VectorToDirection( -Vector3.UnitY ) );
            }

            if ( WorldPosition.Z > 1 )
            {
                WorldPosition.Z = 1;
                mCube.Rotate( CurrentFace.VectorToDirection( Vector3.UnitZ ) );
            }
            else if ( WorldPosition.Z < -1 )
            {
                WorldPosition.Z = -1;
                mCube.Rotate( CurrentFace.VectorToDirection( -Vector3.UnitZ ) );
            }

            Game.Camera.Target = WorldPosition;
        }

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );

            // Find screen equivalent of 3D location in world
            Vector3 screenLocation = GraphicsDevice.Viewport.Project(
                this.WorldPosition,
                mCube.Effect.Projection,
                mCube.Effect.View,
                mCube.Effect.World );

            // Draw our pixel texture there
            mSpriteBatch.Begin();
            
            mSpriteBatch.Draw( pixel,
                               new Vector2(
                                   screenLocation.X,
                                   screenLocation.Y ),
                               Color.Black );

            mSpriteBatch.End();
        }

    }
}
