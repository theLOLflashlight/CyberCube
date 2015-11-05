using CyberCube.Physics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube
{

    public partial class Cube
    {
        public abstract partial class Face : DrawableCubeGameComponent
        {
#if WINDOWS
            protected FarseerPhysics.DebugView.DebugViewXNA mDebugView;
            public static readonly Matrix DEBUG_PROJECTION = Matrix.CreateOrthographicOffCenter(
                        0, WIDTH.ToUnits(), HEIGHT.ToUnits(), 0, 0, 1 );
#endif

            public const int SIZE = 1000;
            public const int WIDTH = SIZE;
            public const int HEIGHT = SIZE;

            /// <summary>
            /// The face's VertexPositionNormalTexture
            /// </summary>
            private VertexPositionNormalTexture[] mVertexData = new VertexPositionNormalTexture[ 6 ];

            protected Texture2D pixel;

            public Cube Cube
            {
                get; private set;
            }

            public string Name
            {
                get; set;
            }

            public Vector3 Normal
            {
                get; private set;
            }

            public Vector3 UpVec
            {
                get; private set;
            }

            public RenderTarget2D RenderTarget
            {
                get; private set;
            }

            public Color BackgroundColor
            {
                get; set;
            }

            public float Rotation
            {
                get; private set;
            }

            public Plane Plane
            {
                get {
                    return new Plane( Normal, -1 );
                }
            }

            public Face( Cube cube, string name, Vector3 normal, Vector3 up, Direction rotation )
                : base( cube.Game )
            {
                Cube = cube;
                Name = name;
                Normal = Vector3.Normalize( normal );
                UpVec = Vector3.Normalize( up );
                Rotation = rotation.Angle;
                BackgroundColor = Color.Transparent;

                SetUpVertices();
                SetUpWorld();
                this.Visible = false;
            }

            private void SetUpVertices()
            {
                Vector3[] face = new Vector3[ 6 ];
                // top left
                face[ 0 ] = new Vector3( -1, 1, 0 );
                // bottom left
                face[ 1 ] = new Vector3( -1, -1, 0 );
                // top right
                face[ 2 ] = new Vector3( 1, 1, 0 );
                // bottom left
                face[ 3 ] = new Vector3( -1, -1, 0 );
                // bottom right
                face[ 4 ] = new Vector3( 1, -1, 0 );
                // top right
                face[ 5 ] = new Vector3( 1, 1, 0 );

                Vector2 textureTopLeft = Vector2.Zero;
                Vector2 textureTopRight = Vector2.UnitX;
                Vector2 textureBottomLeft = Vector2.UnitY;
                Vector2 textureBottomRight = Vector2.One;

                Matrix rotation = Vector3.UnitZ.RotateOntoM( Normal )
                                  * Matrix.CreateFromAxisAngle( Normal, Rotation );

                mVertexData[ 0 ] = new VertexPositionNormalTexture(
                    face[ 0 ].Transform( rotation ) + Normal,
                    Normal, textureTopLeft );

                mVertexData[ 1 ] = new VertexPositionNormalTexture(
                    face[ 1 ].Transform( rotation ) + Normal,
                    Normal, textureBottomLeft );

                mVertexData[ 2 ] = new VertexPositionNormalTexture(
                    face[ 2 ].Transform( rotation ) + Normal,
                    Normal, textureTopRight );

                mVertexData[ 3 ] = new VertexPositionNormalTexture(
                    face[ 3 ].Transform( rotation ) + Normal,
                    Normal, textureBottomLeft );

                mVertexData[ 4 ] = new VertexPositionNormalTexture(
                    face[ 4 ].Transform( rotation ) + Normal,
                    Normal, textureBottomRight );

                mVertexData[ 5 ] = new VertexPositionNormalTexture(
                    face[ 5 ].Transform( rotation ) + Normal,
                    Normal, textureTopRight );
            }

            public override void Initialize()
            {
                base.Initialize();

                RenderTarget = new RenderTarget2D( GraphicsDevice, WIDTH, HEIGHT );
                pixel = new Texture2D( GraphicsDevice, 1, 1 );
                pixel.SetData( new[] { Color.White } );

#if WINDOWS
                mDebugView = new FarseerPhysics.DebugView.DebugViewXNA( World );
                mDebugView.LoadContent( GraphicsDevice, Game.Content );
#endif

                World.Step( 0 );
            }

            public override void Update( GameTime gameTime )
            {
                base.Update( gameTime );

                foreach ( Solid solid in mSolids )
                    solid.Update( gameTime );
            }

            public override void Draw( GameTime gameTime )
            {
                foreach ( Solid solid in mSolids )
                    solid.Draw( gameTime );

#if WINDOWS
                if ( Game.GameProperties.DebugView )
                {
                    mDebugView.BeginCustomDraw( DEBUG_PROJECTION, Matrix.Identity );
                    foreach ( Body b in World.BodyList )
                    {
                        Transform trans;
                        b.GetTransform( out trans );
                        foreach ( Fixture f in b.FixtureList )
                            mDebugView.DrawShape( f, trans, Color.White );
                    }
                    mDebugView.EndCustomDraw();
                }
#endif
            }

            public void Render2D( GameTime gameTime )
            {
                var tmp = GraphicsDevice.GetRenderTargets();

                // Set the current graphics device to the render target and clear it
                GraphicsDevice.SetRenderTarget( RenderTarget );
                GraphicsDevice.Clear( BackgroundColor );

                // Draw our face on the RenderTarget
                this.Draw( gameTime );

                // Now switch back to the default target (i.e., the primary display) and set it up
                GraphicsDevice.SetRenderTargets( tmp );
                GraphicsDevice.Clear( Game.BackgroundColor );
            }

            public void Render3D( Effect effect )
            {
                effect.Parameters[ "Texture" ].SetValue( RenderTarget );

                foreach ( EffectPass pass in effect.CurrentTechnique.Passes )
                {
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives( PrimitiveType.TriangleList, mVertexData, 0, 2 );
                }
            }

            public CompassDirection? VectorToDirection( Vector3 vec )
            {
                vec.Normalize();
                vec = vec.Rounded();

                if ( UpVec == vec )
                    return CompassDirection.North;
                
                vec = vec.Rotate( Normal, MathHelper.PiOver2 ).Rounded();

                if ( UpVec == vec )
                    return CompassDirection.East;

                vec = vec.Rotate( Normal, MathHelper.PiOver2 ).Rounded();

                if ( UpVec == vec )
                    return CompassDirection.South;

                vec = vec.Rotate( Normal, MathHelper.PiOver2 ).Rounded();

                if ( UpVec == vec )
                    return CompassDirection.West;

                return null;
            }

            public Face AdjacentFace( CompassDirection direction )
            {
                switch ( direction )
                {
                case CompassDirection.North:
                    return NorthFace;

                case CompassDirection.East:
                    return EastFace;

                case CompassDirection.South:
                    return SouthFace;

                case CompassDirection.West:
                    return WestFace;
                }

                throw new Tools.WtfException();
            }

            public CompassDirection BackwardsDirectionFrom( Face target )
            {
                if ( NorthFace == target )
                    return CompassDirection.North;

                if ( EastFace == target )
                    return CompassDirection.East;

                if ( SouthFace == target )
                    return CompassDirection.South;

                if ( WestFace == target )
                    return CompassDirection.West;

                throw new Exception( "Faces are not connected." );
            }

            public Face NorthFace
            {
                get; internal set;
            }
            public Face EastFace
            {
                get; internal set;
            }
            public Face SouthFace
            {
                get; internal set;
            }
            public Face WestFace
            {
                get; internal set;
            }
			public Face OppositeFace
            {
                get; internal set;
            }
        }
    }
}
