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
        public const float MOVEMENT_SCALE = 20;

        public const float MODEL_SCALE = 0.04f;
        
        public readonly float ENEMY_WIDTH = 60.ToUnits();
        public readonly float ENEMY_HEIGHT = 60.ToUnits();

        public readonly Color ENEMY_COLOR = Color.White;

        private static Texture2D sTexture;

        private Direction mMovementDirection;
        private float mMoveTimeDelay;

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
            Body.CollidesWith = Category.All;
            Body.Mass = 1000;
        }

        public override void Update( GameTime gameTime )
        {
            float seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;
            mMoveTimeDelay -= seconds;
            if (mMoveTimeDelay < 0)
            {
                float movementScale = MOVEMENT_SCALE;
                Vector2 velocityCopy = Velocity;
                bool fallOff = PreviewFallOffCubeFace(movementScale);
            
                if ( fallOff )
                {
                    Velocity = Vector2.Zero;
                    mMoveTimeDelay = 3;
                    mMovementDirection = ~mMovementDirection;
                }
                else
                {
                    Vector2 velocity = velocityCopy.Rotate( -Rotation );
                    if (mMovementDirection == Direction.Left)
                    { 
                        velocity.X = -movementScale * seconds;
                    }
                    else if (mMovementDirection == Direction.Right)
                    {
                        velocity.X = movementScale * seconds;
                    }
                    else if (mMovementDirection == Direction.Up)
                    {
                        velocity.Y = movementScale * seconds;
                    }
                    else if (mMovementDirection == Direction.Down)
                    {
                        velocity.Y = -movementScale * seconds;
                    }

                    Velocity = velocity.Rotate( Rotation );
                }
            }

            base.Update( gameTime );
            UpdateAnimations( gameTime );
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
            Matrix[] bones = EnemyAnimation.GetSkinTransforms();

            Matrix worldTransformation = Matrix.Identity
                * Matrix.CreateScale( MODEL_SCALE )
                * Matrix.CreateTranslation( WorldPosition );

            // Draw the model. A model can have multiple meshes, so loop.
            foreach ( ModelMesh mesh in model3D.Meshes )
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach ( SkinnedEffect effect in mesh.Effects )
                {
                    effect.EnableDefaultLighting();
                    effect.AmbientLightColor = Color.White.ToVector3();
                    effect.Texture = sTexture;

                    effect.SetBoneTransforms(bones);
                    effect.World = bones[ mesh.ParentBone.Index ] * worldTransformation;

                    Screen.Camera.Apply( effect );
                }
                mesh.Draw();
            }
        }
    }   
}
