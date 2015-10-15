﻿using Microsoft.Xna.Framework;
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
            Vector3 vec3d = new Vector3( vec2d, 0 );

            vec3d = Vector3.Transform( vec3d, Vector3.UnitZ.RotateOnto( Normal ) );
            var angle = mCube.UpDir.ToRadians() + mCube.CurrentFace.Dir.ToRadians();

            return vec3d.Rotate( Normal, angle );
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            var input = Game.Input;

            var delta2d = Vector2.Zero;

            delta2d.X += input[ Actions.MoveRight ].Value;
            delta2d.X -= input[ Actions.MoveLeft ].Value;
            delta2d.Y += input[ Actions.MoveUp ].Value;
            delta2d.Y -= input[ Actions.MoveDown ].Value;

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
