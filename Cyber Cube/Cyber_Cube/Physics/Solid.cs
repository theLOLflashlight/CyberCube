using CyberCube.Graphics;
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
    public abstract class Solid : DrawableCubeGameComponent
    {

        public Body Body
        {
            get; protected set;
        }

        private World mWorld;

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
                    mWorld.RemoveBody( Body );
                    Body = body;
                }
                mWorld = newWorld;
            }
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

        public Texture2D Texture;

        public Solid( CubeGame game, World world )
            : base( game )
        {
            mWorld = world;
            this.Visible = false;
        }

        public override void Initialize()
        {
            base.Initialize();

            Texture = new Texture2D( GraphicsDevice, 1, 1 );
            Texture.SetData( new[] { Color.White } );
        }

        public virtual Solid Clone( World world )
        {
            Solid clone = (Solid) MemberwiseClone();
            clone.mWorld = world;
            clone.Body = clone.Body.Clone( world );

            return clone;
        }

        protected Solid DefaultDeepClone( Solid clone )
        {
            foreach ( Fixture f in Body.FixtureList )
                f.CloneOnto( clone.Body );
            return clone;
        }

        public abstract override void Draw( GameTime gameTime );

    }
    
}
