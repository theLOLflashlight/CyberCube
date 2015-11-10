﻿using CyberCube.Tools;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Physics
{
    public interface ISolidMaker
    {
        Solid MakeSolid( CubeGame game, World world, Body body );
    }

    public abstract class Solid : DrawableCubeGameObject
    {
        public abstract override void Draw( GameTime gameTime );

        private World mWorld;

        public Texture2D Texture;

        protected Solid( CubeGame game,
                         World world,
                         Vector2 position = default( Vector2 ),
                         float rotation = 0,
                         ISolidMaker solidMaker = null )
            : base( game )
        {
            mWorld = world;
            Body = BodyFactory.CreateBody( World, position, rotation, solidMaker );
            Initialize();
        }

        public Solid( CubeGame game, World world, Body body )
            : base( game )
        {
            mWorld = world;
            Body = body;
            Initialize();
        }

        public sealed override void Update( GameTime gameTime )
        {
        }

        public Body Body
        {
            get; private set;
        }

        public BodyType BodyType
        {
            get {
                return Body.BodyType;
            }
            set {
                Body.BodyType = value;
            }
        }

        public Vector2 Position
        {
            get {
                return Body.Position;
            }
            set {
                Body.Position = value;
            }
        }

        public float Rotation
        {
            get {
                return Body.Rotation;
            }
            set {
                Body.Rotation = value;
            }
        }

        private void Initialize()
        {
            Texture = new Texture2D( GraphicsDevice, 1, 1 );
            Texture.SetData( new[] { Color.White } );
        }

        public Solid Clone( World world )
        {
            Solid clone = (Solid) MemberwiseClone();
            clone.mWorld = world;
            clone.Body = clone.Body.DeepClone( world );

            clone.PostClone();

            return clone;
        }

        protected virtual void PostClone()
        {
            Initialize();
        }

        public World World
        {
            get {
                return mWorld;
            }
            set {
                if ( value == null )
                    throw new NullReferenceException( "The value of World cannot be null" );

                World newWorld = value;

                if ( newWorld != mWorld )
                {
                    Body body = Body.DeepClone( newWorld );
                    mWorld?.RemoveBody( Body );
                    Body = body;
                    PostClone();
                }
                mWorld = newWorld;
            }
        }
    }
    
}
