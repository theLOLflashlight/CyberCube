using CyberCube.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using CyberCube.Screens;
using CyberCube.Levels;
using CyberCube.Tools;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Collision;
using CyberCube.Physics;
using FarseerPhysics.Common;
using SkinnedModel;

namespace CyberCube.Actors
{
    public partial class Player : Actor
    {
        public const float JUMP_VELOCITY = -6f;
        public const float JUMP_STOP_FACTOR = 0.6f;
        public const float MAX_RUN_SPEED = 3.5f;
        public const float AIR_MOVEMENT_SCALE = 10;
        public const float GROUND_MOVEMENT_SCALE = 20;
        public const float PLAYER_GRAVITY = 15;
        public const float PLAYER_FRICTION = 0;
        
        public const float MODEL_SCALE = 0.061f;
        public const float RUN_ANIM_FACTOR = 0.035f / MODEL_SCALE;

        public readonly float PLAYER_WIDTH = 15.ToUnits();
        public readonly float PLAYER_HEIGHT = 60.ToUnits();

        public readonly Color PLAYER_COLOR = Color.OrangeRed;

        private Fixture mTorso;
        private Fixture mFeet;

        private static Texture2D sTexture;

        private SoundEffect sfxJump;
        private SoundEffect sfxLand;

        public new PlayScreen Screen
        {
            get {
                return (PlayScreen) base.Screen;
            }
            set {
                base.Screen = value;
            }
        }

        public Player( PlayScreen screen, PlayableCube cube, Vector3 worldPos, float rotation )
            : base( cube.Game, screen, cube, worldPos, (Direction) rotation )
        {
            this.Visible = true;
            this.DrawOrder = 1;

            mModelRotation = new AnimatedVariable<float>( rotation,
                (f0, f1, step) => {
                    var diff = MathHelper.WrapAngle( f1 - f0 );
                    return f0.Tween( f0 + diff, step );
                } );
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            LoadModels();
            LoadAnimations();

            sfxJump = Game.Content.Load<SoundEffect>( @"Audio\jump" );
            sfxLand = Game.Content.Load<SoundEffect>( @"Audio\land" );

            sTexture = new Texture2D( GraphicsDevice, 1, 1 );
            sTexture.SetData( new Color[] { PLAYER_COLOR } );
        }

        public bool IsActive
        {
            get {
                return Screen.Player == this;
            }
        }

        private InputState<Action>.ActionState GetPlayerAction( Action action )
        {
            return IsActive ? Game.Input.GetAction( action, this )
                : default( InputState<Action>.ActionState );
        }

        protected override void ApplyRotation( CompassDirection dir )
        {
            base.ApplyRotation( dir );
            mModelRotation.Value = Rotation;
            if ( IsActive )
                Cube.Rotate( dir );
        }

        protected override Body CreateBody( World world )
        {
            Body body = base.CreateBody( world );

            Vertices verts = PolygonTools.CreateRoundedRectangle(
                PLAYER_WIDTH,
                PLAYER_HEIGHT,
                5.ToUnits(),
                5.ToUnits(),
                0 );
            var torso = FixtureFactory.AttachPolygon( verts, 1, body, "torso" );
            torso.Friction = PLAYER_FRICTION;

            var feet = FixtureFactory.AttachRectangle(
                PLAYER_WIDTH * 0.9f,
                10.ToUnits(),
                1,
                new Vector2( 0, PLAYER_HEIGHT / 2 ),
                body,
                "feet" );
            feet.IsSensor = true;

            return body;
        }

        protected override void ReconstructBody()
        {
            base.ReconstructBody();

            mTorso = Body.FindFixture( "torso" );
            mFeet = Body.FindFixture( "feet" );

            mTorso.OnCollision += Torso_OnCollision;
            mTorso.OnCollision += Torso_OnHazardCollision;
            mTorso.OnCollision += Torso_OnDoorCollision;
            mTorso.OnCollision += Torso_OnEnemyCollision;
            mTorso.OnCollision += Torso_OnProjectileCollision;

            mFeet.OnSeparation += Feet_OnSeparation;
            mFeet.OnCollision += Feet_OnCollision;

            mNumFootContacts = 0;
        }

        public override void Initialize()
        {
            base.Initialize();

            foreach ( Cube.Face face in Cube.Faces )
            {
                face.World.ContactManager.BeginContact += BeginFeetContact;
                face.World.ContactManager.EndContact += EndFeetContact;
            }

            Body.FixedRotation = true;
            Body.GravityScale = PLAYER_GRAVITY;
            Body.UseAdHocGravity = true;
            Body.AdHocGravity = Vector2.UnitY.Rotate( Rotation );

            Body.CollisionCategories = Constants.Categories.PLAYER;
            Body.CollidesWith = Category.All ^ Constants.Categories.PLAYER;
            Body.Mass = 68;
        }

        protected override void Dispose( bool disposing )
        {
            base.Dispose( disposing );
            if ( disposing )
            {
                foreach ( Cube.Face face in Cube.Faces )
                {
                    face.World.ContactManager.BeginContact -= BeginFeetContact;
                    face.World.ContactManager.EndContact -= EndFeetContact;
                }
            }
        }

        public void ApplyAngularImpulse( float impulse )
        {
            Body.ApplyAngularImpulse( impulse );
        }

        public void ApplyLinearImpulse( Vector2 impulse )
        {
            Body.ApplyLinearImpulse( impulse );
        }

        public void ApplyRelativeLinearImpulse( Vector2 impulse )
        {
            Body.ApplyLinearImpulse( impulse.Rotate( Rotation ) );
        }

        public void KillPlayer()
        {
            Screen.RemovePlayer( this );
        }

        public void ClonePlayer()
        {
            Screen.AddPlayer( CubePosition, Rotation );
        }

        private void Jump( ref Vector2 velocity )
        {
            if ( FreeFall && !Game.GameProperties.AllowMultipleJumping )
                return;

            AnimAerialState |= AnimationAerialState.Jumping;

            velocity.Y = JUMP_VELOCITY;
            sfxJump.Play();

            AchievementManager.Instance[ Stat.Jump ]++;
        }

        private void JumpStop( ref Vector2 velocity )
        {
            if ( velocity.Y < 0 )
                velocity.Y *= JUMP_STOP_FACTOR;
        }

        public override void Update( GameTime gameTime )
        {
            //var input = Game.Input;
            float seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;

            #region Rotation
            if ( mNumFootContacts == 1 && !IsJumping )
                Rotation = UpDir.Angle;

            if ( Game.GameProperties.AllowManualGravity )
            {
                if ( GetPlayerAction( Action.RotateClockwise ) )
                    UpDir = (Direction) (Rotation - (MathHelper.PiOver4 + 0.0001f));

                if ( GetPlayerAction( Action.RotateAntiClockwise ) )
                    UpDir = (Direction) (Rotation + (MathHelper.PiOver4 + 0.0001f));
            }
            #endregion

            // Rotation-normalized velocity proxy
            Vector2 velocity = Velocity.Rotate( -Rotation );

            #region Running
            float movementScale = FreeFall ? AIR_MOVEMENT_SCALE : GROUND_MOVEMENT_SCALE;
            var actionLeft = GetPlayerAction( Action.MoveLeft );
            var actionRight = GetPlayerAction( Action.MoveRight );

            if ( (bool) actionRight == actionLeft )
            {
                velocity.X = velocity.X.Tween( 0, movementScale * seconds );
            }
            else
            {
                float maxSpeed = Math.Max( actionLeft, actionRight ) * MAX_RUN_SPEED;

                velocity.X -= actionLeft * (movementScale + 1) * seconds;
                velocity.X += actionRight * (movementScale + 1) * seconds;
                velocity.X = MathHelper.Clamp( velocity.X, -maxSpeed, +maxSpeed );
                //velocity.X = MathHelper.Clamp( velocity.X, -MAX_RUN_SPEED, +MAX_RUN_SPEED );
            }
            #endregion
            UpdateRunningAnimations( gameTime, velocity );

            #region Jumping
            if ( GetPlayerAction( Action.Jump ) )
                Jump( ref velocity );

            if ( GetPlayerAction( Action.JumpStop ) )
                JumpStop( ref velocity );
            #endregion
            UpdateJumpingAnimations( gameTime, velocity );

            Velocity = velocity.Rotate( Rotation );

            // ACTOR UPDATE \\
            base.Update( gameTime );
            UpdateAnimations( gameTime );

            if ( GetPlayerAction( Action.PlaceClone ) )
                ClonePlayer();

            if ( GetPlayerAction( Action.CycleClone ) )
                Screen.NextClone();

            if ( GetPlayerAction( Action.DeleteClone ) )
                KillPlayer();
        }

        public override void Draw( GameTime gameTime )
        {
            // Below are codes for render the 3d model, didn't quite working bug-free so commented out for 
            //Matrix[] transforms = new Matrix[ model3D.Bones.Count ];
            //model3D.CopyAbsoluteBoneTransformsTo( transforms );

            Matrix[] transforms = PlayerAnimation.GetSkinTransforms();

            Matrix worldTransformation = Matrix.CreateTranslation( 0, -1, 0 )
                * Matrix.CreateScale( MODEL_SCALE )
                * Matrix.CreateFromAxisAngle( Vector3.UnitY, MovementRotation )
                * Vector3.UnitZ.RotateOnto_M( CubeFace.Normal )
                * Matrix.CreateFromAxisAngle( CubeFace.Normal, CubeFace.Rotation - mModelRotation )
                * Matrix.CreateTranslation( WorldPosition );

            // Draw the model. A model can have multiple meshes, so loop.
            foreach ( ModelMesh mesh in model3D.Meshes )
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach ( SkinnedEffect effect in mesh.Effects )
                {
                    effect.EnableDefaultLighting();
                    effect.Parameters[ "Bones" ].SetValue( transforms );
                    effect.World = transforms[ mesh.ParentBone.Index ] * worldTransformation;
                    //effect.DiffuseColor = PLAYER_COLOR.ToVector3();
                    effect.AmbientLightColor = PLAYER_COLOR.ToVector3();
                    effect.Texture = sTexture;

                    Screen.Camera.Apply( effect );
                }
                mesh.Draw();
            }
        }

    }
}
