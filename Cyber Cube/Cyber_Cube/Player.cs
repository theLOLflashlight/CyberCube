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
    public class Player : DrawableCubeGameComponent
    {
        public Vector3 WorldPosition = Vector3.UnitZ;
        public Vector3 Velocity = Vector3.Zero;

        public Cube Cube { get; private set; }

        private Texture2D pixel;

        private Cube.Face CurrentFace
        {
            get {
                return Cube.CurrentFace;
            }
        }

        public Vector3 Normal
        {
            get {
                return Cube.CurrentFace.Normal;
            }
        }

        public Player( Cube cube )
            : base( cube.Game )
        {
            Cube = cube;
            this.Visible = true;
            this.DrawOrder = 1;
        }

        public override void Initialize()
        {
            base.Initialize();

            pixel = new Texture2D( GraphicsDevice, 2, 2 );
            pixel.SetData( new[] { Color.White, Color.White, Color.White, Color.White } );
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public Vector3 TransformMovementTo3d( Vector2 vec2d )
        {
            var angle = Cube.UpDir.ToRadians() + CurrentFace.Rotation;
            
            return new Vector3( vec2d, 0 )
                       .Transform( Utils.RotateOntoQ( Vector3.UnitZ, Normal ) )
                       .Rotate( Normal, angle );
        }

        public Vector3 Transform2dTo3d( Vector2 vec2d )
        {
            return new Vector3( vec2d.X, -vec2d.Y, 0 )
                       .Transform( Utils.RotateOntoQ( Vector3.UnitZ, Normal ) )
                       .Rotate( Normal, CurrentFace.Rotation );
        }

        public Vector2 Transform3dTo2d( Vector3 vec3d )
        {
            vec3d = vec3d.Rotate( Normal, -CurrentFace.Rotation )
                         .Transform( Utils.RotateOntoQ( Normal, Vector3.UnitZ ) );
            return new Vector2( vec3d.X, -vec3d.Y );
        }

        public Vector2 ComputeFacePosition()
        {
            float adjustingFactor = Cube.Face.SIZE / 2;
            return Transform3dTo2d( WorldPosition ) * adjustingFactor + new Vector2( adjustingFactor );
        }

        private Vector2 mGroundNormal;

        public void Collide( Rectangle rec )
        {
            Collide( rec, ComputeFacePosition() );
        }

        public void Collide( Rectangle rec, Vector2 facePos )
        {
            Vector2 perimiterPos = facePos.NearestPointOn( rec );

            Vector2 groundNormal = facePos - perimiterPos;
            if ( groundNormal != Vector2.Zero )
            {
                mGroundNormal = Vector2.Normalize( groundNormal );
                mGroundNormal = mGroundNormal.Rotate( Cube.UpDir.ToRadians() ).Rounded();
                mCollided = true;
            }

            Set2dPosition( perimiterPos );
        }

        public void Set2dPosition( Vector2 vec )
        {
            float adjustingFactor = Cube.Face.SIZE / 2;
            vec -= new Vector2( adjustingFactor );
            vec /= adjustingFactor;

            WorldPosition = Transform2dTo3d( vec );
            WorldPosition += Normal;
            //Game.Camera.Target = WorldPosition;
        }

        private Vector2 mVelocity2D = Vector2.Zero;

        private bool mCollided = false;

        public bool FreeFall
        {
            get {
                return mGroundNormal == Vector2.Zero;
            }
        }

        public void Jump()
        {
            mVelocity2D.Y = 2f;
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            var input = Game.Input;

            float timeDiff = (float) gameTime.ElapsedGameTime.TotalSeconds;

            var delta2d = Vector2.Zero;
            if ( !input.HasFocus )
            {
                delta2d.X += input.GetAction( Action.MoveRight ).Value;
                delta2d.X -= input.GetAction( Action.MoveLeft ).Value;
                delta2d.Y += input.GetAction( Action.MoveUp ).Value;
                delta2d.Y -= input.GetAction( Action.MoveDown ).Value;

                if ( input.GetAction( Action.Jump ) )
                    this.Jump();

                if ( delta2d != Vector2.Zero )
                {
                    var tmp = delta2d;
                    delta2d.Normalize();
                    delta2d.X *= Math.Abs( tmp.X );
                    delta2d.Y *= Math.Abs( tmp.Y );
                }
            }

            mVelocity2D.Y -= 5f * timeDiff;

            if ( mGroundNormal.Y > 0 )
                mVelocity2D.Y = Math.Max( 0, mVelocity2D.Y );

            else if ( mGroundNormal.Y < 0 )
                mVelocity2D.Y = Math.Min( 0, mVelocity2D.Y );


            if ( !mCollided )
                mGroundNormal = Vector2.Zero;
            mCollided = false;

            delta2d += mVelocity2D;

            Vector3 delta3d = TransformMovementTo3d( delta2d );
            WorldPosition += delta3d * timeDiff;

            if ( WorldPosition.X > 1 )
            {
                WorldPosition.X = 1;
                Cube.Rotate( CurrentFace.VectorToDirection( Vector3.UnitX ) );
            }
            else if ( WorldPosition.X < -1 )
            {
                WorldPosition.X = -1;
                Cube.Rotate( CurrentFace.VectorToDirection( -Vector3.UnitX ) );
            }

            if ( WorldPosition.Y > 1 )
            {
                WorldPosition.Y = 1;
                Cube.Rotate( CurrentFace.VectorToDirection( Vector3.UnitY ) );
            }
            else if ( WorldPosition.Y < -1 )
            {
                WorldPosition.Y = -1;
                Cube.Rotate( CurrentFace.VectorToDirection( -Vector3.UnitY ) );
            }

            if ( WorldPosition.Z > 1 )
            {
                WorldPosition.Z = 1;
                Cube.Rotate( CurrentFace.VectorToDirection( Vector3.UnitZ ) );
            }
            else if ( WorldPosition.Z < -1 )
            {
                WorldPosition.Z = -1;
                Cube.Rotate( CurrentFace.VectorToDirection( -Vector3.UnitZ ) );
            }

            Game.Camera.Target = WorldPosition;
        }

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );

            // Find screen equivalent of 3D location in world
            Vector3 screenLocation = GraphicsDevice.Viewport.Project(
                this.WorldPosition,
                Cube.Effect.Projection,
                Cube.Effect.View,
                Cube.Effect.World );

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
