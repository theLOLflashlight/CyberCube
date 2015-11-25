﻿using CyberCube.Actors;
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
        public const float MAX_RUN_SPEED = 3.5f;

        public const float MODEL_SCALE = 0.061f;
        public const float RUN_ANIM_FACTOR = 0.04f / MODEL_SCALE;
        
        public readonly float ENEMY_WIDTH = 15.ToUnits();
        public readonly float ENEMY_HEIGHT = 60.ToUnits();

        public readonly Color ENEMY_COLOR = Color.BlueViolet;

        private static Texture2D sTexture;

        public Enemy( PlayScreen screen, PlayableCube cube, Vector3 worldPos, float rotation ) 
            : base ( cube.Game, screen, cube, worldPos, (Direction)  rotation )
        {
            this.Visible = true;
            this.DrawOrder = 1;

            mModelRotation = new AnimatedVariable<float>( rotation,
                (f0, f1, step) => {
                    var diff = MathHelper.WrapAngle( f1 - f0 );
                    return f0.Tween( f0 + diff, step );
                } );

            // hard code value for now
            mWorldPosition = new Vector3(0, -0.20020324f, 1);
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
            var head = FixtureFactory.AttachPolygon( verts, 1, body, "head" );
            
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
            Body.CollidesWith = FarseerPhysics.Dynamics.Category.All;
            Body.Mass = 1000;
        }

        public override void Update( GameTime gameTime )
        {
            float seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;


            base.Update( gameTime );
            UpdateAnimations( gameTime );
        }



        public override void Draw( GameTime gameTime )
        {
            Matrix[] transforms = EnemyAnimation.GetSkinTransforms();

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
                    effect.AmbientLightColor = ENEMY_COLOR.ToVector3();
                    effect.Texture = sTexture;

                    Screen.Camera.Apply( effect );
                }
                mesh.Draw();
            }
        }
    }   
}
