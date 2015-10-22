using CyberCube.Physics;
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
        public partial class Face : DrawableCubeGameComponent
        {
            public const int SIZE = 100;
            public const int WIDTH = SIZE;
            public const int HEIGHT = SIZE;

            /// <summary>
            /// The face's VertexPositionNormalTexture
            /// </summary>
            private VertexPositionNormalTexture[] mVertexData = new VertexPositionNormalTexture[ 6 ];

            private Texture2D pixel;

            private SpriteFont mFont;

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

            public Texture2D Texture
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

            public List< Solid2 > Solid2s
            {
                get; private set;
            }

            public Face( Cube cube, string name, Vector3 normal, Vector3 up, Direction orientation )
                : base( cube.Game )
            {
                Cube = cube;
                Name = name;
                Normal = Vector3.Normalize( normal );
                UpVec = Vector3.Normalize( up );
                Rotation = orientation.ToRadians();
                BackgroundColor = Color.Transparent;

                Solid2s = new List<Solid2>();

                SetUpVertices();
                //SetUpWorld();

                Game.Components.ComponentAdded += ( s, e ) => {
                    if ( ReferenceEquals( this, e.GameComponent ) )
                        foreach ( Solid solid in mSolids )
                            Game.Components.Add( solid );
                };

                Game.Components.ComponentRemoved += ( s, e ) => {
                    if ( ReferenceEquals( this, e.GameComponent ) )
                        foreach ( Solid solid in mSolids )
                            Game.Components.Remove( solid );
                };
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

                Texture = new Texture2D( GraphicsDevice, WIDTH, HEIGHT );
                pixel = new Texture2D( GraphicsDevice, 1, 1 );
                pixel.SetData( new[] { Color.White } );

                this.Visible = false;

                Solid2s.Clear();
                Solid2s.Add( new RecSolid2( new Rectangle( 0, HEIGHT - 10, WIDTH, 10 ) ) );
                Solid2s.Add( new RecSolid2( new Rectangle( 10, 70, 30, 10 ) ) );
                Solid2s.Add( new RecSolid2( new Rectangle( WIDTH - 15, 15, 10, 30 ) ) );

                Solid2s.Add( new OneWayLine( new Line2( 10, 20, 40, 20 ) ) );
                Solid2s.Add( new OneWayLine( new Line2( 10, 40, 40, 40 ) ) );

                Solid2s.Add( new OneWayLine( new Line2( 70, 50, 70, 20 ) ) );

                Solid2s.Add( new OneWayLine( new Line2( 38, 91, 38, 79 ) ) );
                Solid2s.Add( new OneWayLine( new Line2( 24, 91, 24, 79 ) ) );
                Solid2s.Add( new OneWayLine( new Line2( 10, 91, 10, 79 ) ) );
            }

            protected override void LoadContent()
            {
                base.LoadContent();

                mFont = Game.Content.Load< SpriteFont >( "MessageFont" );
                //LoadPhysics();
            }

            public override void Draw( GameTime gameTime )
            {
                mSpriteBatch.Begin();
                mSpriteBatch.DrawString( mFont,
                                        Name,
                                        new Vector2( WIDTH, HEIGHT ) / 2,
                                        Color.Black,
                                        0, // Rotation is handled via texture orientation.
                                        mFont.MeasureString( Name ) / 2,
                                        1,
                                        SpriteEffects.None,
                                        0 );

                foreach ( Solid2 solid in Solid2s )
                    solid.Draw( mSpriteBatch, pixel );

                //Vector2 vec2d = Game.Player.ComputeFacePosition();
                //vec2d = vec2d.Rounded();
                //vec2d /= 10;
                //
                //mSpriteBatch.Draw( pixel, new Rectangle( 10 * (int) vec2d.X, 10 * (int) vec2d.Y, 10, 10 ), new Color( 0, 0, 0, 128 ) );

                mSpriteBatch.End();

                //DrawBodies( gameTime );
            }

            public void Render2D( GameTime gameTime )
            {
                // Huge memory performance improvement gained by disposing of render target.
                using ( var renderTarget = new RenderTarget2D( GraphicsDevice, WIDTH, HEIGHT ) )
                {
                    var tmp = GraphicsDevice.GetRenderTargets();

                    // Set the current graphics device to the render target and clear it
                    GraphicsDevice.SetRenderTarget( renderTarget );
                    GraphicsDevice.Clear( BackgroundColor );

                    this.Draw( gameTime );

                    // Now switch back to the default target (i.e., the primary display) and set it up
                    GraphicsDevice.SetRenderTargets( tmp );
                    GraphicsDevice.Clear( Game.BackgroundColor );

                    int[] data = new int[ WIDTH * HEIGHT ];
                    renderTarget.GetData( data );
                    Texture.SetData( data );
                }
            }

            public void Render3D( Effect effect )
            {
                effect.Parameters[ "Texture" ].SetValue( Texture );

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
