using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{

    public partial class Cube
    {
        public class Face : DrawableCubeGameComponent
        {
            public const int SIZE = 100;
            public const int WIDTH = SIZE;
            public const int HEIGHT = SIZE;

            public Cube Cube { get; private set; }

            public Face NorthFace { get; internal set; }
            public Face EastFace { get; internal set; }
            public Face SouthFace { get; internal set; }
            public Face WestFace { get; internal set; }

            public string Name { get; private set; }

            public Vector3 Normal { get; private set; }
            public Vector3 UpVec { get; private set; }

            private SpriteFont mFont;

            public Texture2D Texture { get; private set; }

            public Color BackgroundColor { get; set; }

            /// <summary>
            /// The face's VertexPositionNormalTexture
            /// </summary>
            private VertexPositionNormalTexture[] mVertexData = new VertexPositionNormalTexture[ 6 ];

            public float Rotation { get; private set; }

            private Texture2D pixel;

            public Face( Cube cube, string name, Vector3 normal, Vector3 up, Direction orientation )
                : base( cube.Game )
            {
                Cube = cube;
                Name = name;
                Normal = Vector3.Normalize( normal );
                UpVec = Vector3.Normalize( up );
                Rotation = orientation.ToRadians();
                BackgroundColor = Color.Transparent;

                Vector3[] face = new Vector3[ 6 ];
                // TopLeft
                face[ 0 ] = new Vector3( -1, 1, 0.0f );
                // BottomLeft
                face[ 1 ] = new Vector3( -1, -1, 0.0f );
                // TopRight
                face[ 2 ] = new Vector3( 1, 1, 0.0f );
                // BottomLeft
                face[ 3 ] = new Vector3( -1, -1, 0.0f );
                // BottomRight
                face[ 4 ] = new Vector3( 1, -1, 0.0f );
                // TopRight
                face[ 5 ] = new Vector3( 1, 1, 0.0f );

                Vector2 textureTopLeft = Vector2.Zero;
                Vector2 textureTopRight = Vector2.UnitX;
                Vector2 textureBottomLeft = Vector2.UnitY;
                Vector2 textureBottomRight = Vector2.One;

                Matrix rotation = Vector3.UnitZ.RotateOntoM( Normal )
                                  * Matrix.CreateFromAxisAngle( Normal, Rotation );

                mVertexData[ 0 ] = new VertexPositionNormalTexture(
                    Vector3.Transform( face[ 0 ], rotation ) + Normal,
                    Normal, textureTopLeft );

                mVertexData[ 1 ] = new VertexPositionNormalTexture(
                    Vector3.Transform( face[ 1 ], rotation ) + Normal,
                    Normal, textureBottomLeft );

                mVertexData[ 2 ] = new VertexPositionNormalTexture(
                    Vector3.Transform( face[ 2 ], rotation ) + Normal,
                    Normal, textureTopRight );

                mVertexData[ 3 ] = new VertexPositionNormalTexture(
                    Vector3.Transform( face[ 3 ], rotation ) + Normal,
                    Normal, textureBottomLeft );

                mVertexData[ 4 ] = new VertexPositionNormalTexture(
                    Vector3.Transform( face[ 4 ], rotation ) + Normal,
                    Normal, textureBottomRight );

                mVertexData[ 5 ] = new VertexPositionNormalTexture(
                    Vector3.Transform( face[ 5 ], rotation ) + Normal,
                    Normal, textureTopRight );
            }

            public override void Initialize()
            {
                base.Initialize();

                this.Visible = false;

                Texture = new Texture2D( GraphicsDevice, WIDTH, HEIGHT );
                pixel = new Texture2D( GraphicsDevice, 1, 1 );
                pixel.SetData( new[] { Color.White } );

                mSolids.Add( new Solid( new Rectangle( 0, HEIGHT - 10, WIDTH, 10 ) ) );
                mSolids.Add( new Solid( new Rectangle( WIDTH / 10, 2 * HEIGHT / 3, WIDTH / 3, 10 ) ) );
            }

            protected override void LoadContent()
            {
                base.LoadContent();

                mFont = Game.Content.Load< SpriteFont >( "MessageFont" );
            }

            private List< Solid > mSolids = new List<Solid>();

            public override void Update( GameTime gameTime )
            {
                base.Update( gameTime );

                foreach ( Solid solid in mSolids )
                    solid.Color = Color.Black;

                if ( Game.Player.Normal == this.Normal )
                {
                    Vector2 vec2d = Game.Player.ComputeFacePosition();

                    foreach ( Solid solid in mSolids )
                        if ( solid.Rectangle.Contains( vec2d ) )
                        {
                            solid.Color = Color.White;
                            Game.Player.Collide( solid.Rectangle, vec2d );
                        }
                }
            }

            public override void Draw( GameTime gameTime )
            {
                base.Draw( gameTime );

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

                foreach ( Solid solid in mSolids )
                    mSpriteBatch.Draw( pixel, solid.Rectangle, solid.Color );

                //Vector2 vec2d = Game.Player.ComputeFacePosition();
                //vec2d = vec2d.Rounded();
                //vec2d /= 10;
                //
                //mSpriteBatch.Draw( pixel, new Rectangle( 10 * (int) vec2d.X, 10 * (int) vec2d.Y, 10, 10 ), new Color( 0, 0, 0, 128 ) );

                mSpriteBatch.End();
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
                return this;
            }
        }
    }
}
