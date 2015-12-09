using CyberCube.Actors;
using CyberCube.IO;
using CyberCube.Levels;
using CyberCube.Physics;
using CyberCube.Screens;
using CyberCube.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using FarseerPhysics.Factories;

namespace CyberCube.Actors
{
	public partial class Enemy : Actor
    {
        public const float MOVEMENT_SCALE = 30;

        public const float MODEL_SCALE = 0.04f;
        
        public readonly float ENEMY_WIDTH = 34.ToUnits();
        public readonly float ENEMY_HEIGHT = 34.ToUnits();

        private const float VISION_LENGTH = 200;
        private const float VISION_WIDTH = 60;

        public readonly Color ENEMY_COLOR = Color.White;

        private static Texture2D sTexture;

        private Direction mMovementDirection;
        private float mMoveTimeDelay;
        private float mDirectionTimeDelay;
        private float mAttackTimeDelay;
        private bool mPlayerSpotted;

        private VertexPositionColor[] mVisionVertices;
        private BasicEffect mEffect;

        public new PlayScreen Screen
        {
            get {
                return (PlayScreen) base.Screen;
            }
            set {
                base.Screen = value;
            }
        }

        public Enemy( PlayScreen screen, PlayableCube cube, Vector3 worldPos, float rotation ) 
            : base ( cube.Game, screen, cube, worldPos, (Direction) rotation )
        {
            this.Visible = true;
            this.DrawOrder = 1;

            mModelRotation = new AnimatedVariable<float>( rotation,
                (f0, f1, step) => {
                    var diff = MathHelper.WrapAngle( f1 - f0 );
                    return f0.Tween( f0 + diff, step );
                } );
            
            mMovementDirection = (Direction) rotation;
            mMovementDirection--;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            LoadModels();
            LoadAnimations();

            sTexture = new Texture2D( GraphicsDevice, 1, 1 );
            sTexture.SetData( new Color[] { ENEMY_COLOR } );

            mEffect = new BasicEffect(GraphicsDevice);
            mEffect.Alpha = 0.4f;
            mEffect.VertexColorEnabled = true;
            mEffect.LightingEnabled = false;

        }

        protected override Body CreateBody( World world )
        {
            Body body = base.CreateBody( world );

            Vertices verts = PolygonTools.CreateRoundedRectangle(
                ENEMY_WIDTH,
                ENEMY_HEIGHT,
                5.ToUnits(),
                5.ToUnits(),
                0 );
            var head = FixtureFactory.AttachPolygon( verts, 1, body, "enemy" );
            
            return body;
        }

        protected override void ReconstructBody()
        {
            base.ReconstructBody();
        }

        public override void Initialize()
        {
            base.Initialize();
            
            Body.FixedRotation = true;
            Body.GravityScale = 0;
            Body.UseAdHocGravity = true;
            Body.AdHocGravity = Vector2.Zero;

            Body.CollisionCategories = Constants.Categories.ENEMY;
            Body.CollidesWith = Category.All ^ Constants.Categories.ENEMY;
            Body.Mass = 1000000;

            mVisionVertices = new VertexPositionColor[3];
            mVisionVertices[0] = new VertexPositionColor(CubePosition, Color.LightGoldenrodYellow);
            Vector2 vec2d = Body.Position.ToPixels() + new Vector2( -VISION_LENGTH, VISION_WIDTH );
            float adjustingFactor = Cube.Face.SIZE / 2;
            vec2d -= new Vector2( adjustingFactor );
            vec2d /= adjustingFactor;
            mVisionVertices[ 2 ] =  new VertexPositionColor(Transform2dTo3d( vec2d ), Color.LightGoldenrodYellow);

            vec2d = Body.Position.ToPixels() + new Vector2( -VISION_LENGTH, -VISION_WIDTH );
            adjustingFactor = Cube.Face.SIZE / 2;
            vec2d -= new Vector2( adjustingFactor );
            vec2d /= adjustingFactor;
            mVisionVertices[ 1 ] =  new VertexPositionColor(Transform2dTo3d( vec2d ), Color.LightGoldenrodYellow);
        }

        public override void Update( GameTime gameTime )
        {
            float seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;
            mMoveTimeDelay -= seconds;
            mDirectionTimeDelay -= seconds;
            mAttackTimeDelay -= seconds;


            if ( SeePlayer( Screen.Player ) )
            {
                Screen.PlayAlarm();
                mPlayerSpotted = true;
                mVisionVertices[0].Color = Color.Red;
                mVisionVertices[1].Color = Color.Red;
                mVisionVertices[2].Color = Color.Red;
            }
            else
            {
                mPlayerSpotted = false;
                mVisionVertices[0].Color = Color.LightGoldenrodYellow;
                mVisionVertices[1].Color = Color.LightGoldenrodYellow;
                mVisionVertices[2].Color = Color.LightGoldenrodYellow;
            }


            if (mMoveTimeDelay < 0)
            {
                if (mDirectionTimeDelay < 0)
                {
                    mDirectionTimeDelay = 10;
                    mMovementDirection = ~mMovementDirection;
                }

                float movementScale = MOVEMENT_SCALE;
                Vector2 velocityCopy = Velocity;
                bool fallOff = PreviewFallOffCubeFace(movementScale);
            
                if ( fallOff )
                {
                    mMoveTimeDelay = 3;
                    mDirectionTimeDelay = 10;
                    Velocity = Vector2.Zero;
                    mMovementDirection = ~mMovementDirection;
                }
                else if ( mPlayerSpotted )
                {
                    if ( mAttackTimeDelay < 0 )
                    {
                        mAttackTimeDelay = 3;
                        mMoveTimeDelay = 3;
                        Velocity = Vector2.Zero;
                        ShootProjectile( Screen.Player );
                    }
                }
                else
                {

                    mVisionVertices[0].Position = CubePosition;

                    Vector2 velocity = velocityCopy.Rotate( -Rotation );
                    if (mMovementDirection == Direction.Left)
                    { 
                        velocity.X = -movementScale * seconds;

                        // update vision
                        Vector2 vec2d = Body.Position.ToPixels() + new Vector2( -VISION_LENGTH, VISION_WIDTH );
                        float adjustingFactor = Cube.Face.SIZE / 2;
                        vec2d -= new Vector2( adjustingFactor );
                        vec2d /= adjustingFactor;
                        mVisionVertices[ 2 ].Position = Transform2dTo3d( vec2d );

                        vec2d = Body.Position.ToPixels() + new Vector2( -VISION_LENGTH, -VISION_WIDTH );
                        adjustingFactor = Cube.Face.SIZE / 2;
                        vec2d -= new Vector2( adjustingFactor );
                        vec2d /= adjustingFactor;
                        mVisionVertices[ 1 ].Position = Transform2dTo3d( vec2d );
                    }
                    else if (mMovementDirection == Direction.Right)
                    {
                        velocity.X = movementScale * seconds;

                        // update vision
                        Vector2 vec2d = Body.Position.ToPixels() + new Vector2( VISION_LENGTH, -VISION_WIDTH );
                        float adjustingFactor = Cube.Face.SIZE / 2;
                        vec2d -= new Vector2( adjustingFactor );
                        vec2d /= adjustingFactor;
                        mVisionVertices[ 2 ].Position = Transform2dTo3d( vec2d );

                        vec2d = Body.Position.ToPixels() + new Vector2( VISION_LENGTH, VISION_WIDTH );
                        adjustingFactor = Cube.Face.SIZE / 2;
                        vec2d -= new Vector2( adjustingFactor );
                        vec2d /= adjustingFactor;
                        mVisionVertices[ 1 ].Position = Transform2dTo3d( vec2d );
                    }
                    else if (mMovementDirection == Direction.Up)
                    {
                        velocity.Y = movementScale * seconds;

                        // update vision
                        Vector2 vec2d = Body.Position.ToPixels() + new Vector2( -VISION_WIDTH, -VISION_LENGTH );
                        float adjustingFactor = Cube.Face.SIZE / 2;
                        vec2d -= new Vector2( adjustingFactor );
                        vec2d /= adjustingFactor;
                        mVisionVertices[ 2 ].Position = Transform2dTo3d( vec2d );

                        vec2d = Body.Position.ToPixels() + new Vector2( VISION_WIDTH, -VISION_LENGTH );
                        adjustingFactor = Cube.Face.SIZE / 2;
                        vec2d -= new Vector2( adjustingFactor );
                        vec2d /= adjustingFactor;
                        mVisionVertices[ 1 ].Position = Transform2dTo3d( vec2d );
                    }
                    else if (mMovementDirection == Direction.Down)
                    {
                        velocity.Y = -movementScale * seconds;

                        // update vision
                        Vector2 vec2d = Body.Position.ToPixels() + new Vector2( VISION_WIDTH, VISION_LENGTH );
                        float adjustingFactor = Cube.Face.SIZE / 2;
                        vec2d -= new Vector2( adjustingFactor );
                        vec2d /= adjustingFactor;
                        mVisionVertices[ 2 ].Position = Transform2dTo3d( vec2d );

                        vec2d = Body.Position.ToPixels() + new Vector2( VISION_WIDTH, VISION_LENGTH );
                        adjustingFactor = Cube.Face.SIZE / 2;
                        vec2d -= new Vector2( adjustingFactor );
                        vec2d /= adjustingFactor;
                        mVisionVertices[ 1 ].Position = Transform2dTo3d( vec2d );
                    }

                    Velocity = velocity.Rotate( Rotation );
                    UpdateMovingAnimations( gameTime, velocity );
                }
            }

            base.Update( gameTime );
            UpdateAnimations( gameTime );

        
        }

        // we determine if we see the player by using the triangle vision and see if it contains the player 
        private bool SeePlayer(Player player)
        {
            Vector2 playerPosition = Transform3dTo2d(player.CubePosition);
            Vector2 pointA = Transform3dTo2d(mVisionVertices[0].Position);
            Vector2 pointB = Transform3dTo2d(mVisionVertices[1].Position);
            Vector2 pointC = Transform3dTo2d(mVisionVertices[2].Position);
            
            return PointInTriangle(playerPosition, pointA, pointB, pointC);
        }

        private bool SameSide(Vector2 point1, Vector2 point2, Vector2 pointA, Vector2 pointB)
        {

            Vector3 cp1 = Vector3.Cross(new Vector3(pointB - pointA, 0), new Vector3(point1 - pointA, 0));
            Vector3 cp2 = Vector3.Cross(new Vector3(pointB - pointA, 0), new Vector3(point2 - pointA, 0));
            return Vector3.Dot(cp1, cp2) >= 0;
        }

        private bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            return SameSide(p, a, b, c) && SameSide(p, b, c, a) && SameSide(p, c, a, b);
        }

        private void ShootProjectile(Player player)
        {
            Vector2 playerPosition = Transform3dTo2d( player.CubePosition );
            Vector3 direction = Vector3.Normalize( player.CubePosition - CubePosition );
            direction.Y = -direction.Y;
            Screen.AddProjectile( CubePosition, direction, Rotation );
        }

        private bool PreviewFallOffCubeFace(float movementScale)
        {
            Vector2 previewVelocity = Velocity.Rotate( -Rotation );
            
            // preview 3 seconds ahead
            if (mMovementDirection == Direction.Left)
            { 
                previewVelocity.X = -movementScale * 3;
            }
            else
            {
                previewVelocity.X = movementScale * 3;
            }

            Velocity = previewVelocity.Rotate( Rotation );

            // check if will move off the edge of the cube
            Vector2 vec2d = Body.Position.ToPixels() + Velocity;
            float adjustingFactor = Cube.Face.SIZE / 2;
            vec2d -= new Vector2( adjustingFactor );
            vec2d /= adjustingFactor;
            Vector3 previewWorldPosition = Transform2dTo3d( vec2d );
            
            if ( CubeFace.Normal == Vector3.UnitZ || CubeFace.Normal == -Vector3.UnitZ )
            {
                if ( previewWorldPosition.X > 1 || previewWorldPosition.X < -1 ||
                     previewWorldPosition.Y > 1 || previewWorldPosition.Y < -1 )
                {
                    return true;
                }
            }
            else if ( CubeFace.Normal == Vector3.UnitX || CubeFace.Normal == -Vector3.UnitX )
            {
                if ( previewWorldPosition.Z > 1 || previewWorldPosition.Z < -1 ||
                     previewWorldPosition.Y > 1 || previewWorldPosition.Y < -1 )
                {
                    return true;
                }
            }
            else
            {
                if ( previewWorldPosition.X > 1 || previewWorldPosition.X < -1 ||
                     previewWorldPosition.Z > 1 || previewWorldPosition.Z < -1 )
                {
                    return true;
                }
            }

            return false;
        }



        public override void Draw( GameTime gameTime )
        {
            if (Cube.CurrentFace == CubeFace)
            {
                mEffect.View = Cube.Effect.View;
                mEffect.Projection = Cube.Effect.Projection;
                foreach (EffectPass pass in mEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, mVisionVertices, 0, 1);
                }
                
                Matrix[] bones = EnemyAnimation.GetSkinTransforms();

                Matrix worldTransformation = Matrix.Identity
                    * Matrix.CreateScale(MODEL_SCALE)
                    * Matrix.CreateFromAxisAngle(Vector3.UnitY, MovementRotation)
                    * Matrix.CreateTranslation(CubePosition);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in model3D.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (SkinnedEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.AmbientLightColor = Color.White.ToVector3();
                        effect.Texture = sTexture;

                        effect.SetBoneTransforms(bones);
                        effect.World = bones[mesh.ParentBone.Index] * worldTransformation;

                        Screen.Camera.Apply(effect);
                    }
                    mesh.Draw();
                }
            }
        }
    }   
}
