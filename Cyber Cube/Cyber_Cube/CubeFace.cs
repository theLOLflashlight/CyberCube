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
        public class Face : DrawableGameComponent
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
            public Vector3 Up { get; private set; }

            private SpriteFont mFont;

            public Texture2D Texture { get; private set; }

            public Color BackgroundColor { get; set; }

            /// <summary>
            /// The face's VertexPositionNormalTexture
            /// </summary>
            private VertexPositionNormalTexture[] VPNT = new VertexPositionNormalTexture[ 6 ];

            public readonly Matrix RotatePlane;

            public Direction Dir;

            public Face( Cube cube, string name, Vector3 normal, Vector3 up, Direction dir )
                : base( cube.Game )
            {
                Cube = cube;
                Name = name;
                Normal = Vector3.Normalize( normal );
                Up = up;

                Dir = dir;

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

                RotatePlane = Utils.RotateVecToVec( Vector3.UnitZ, Normal )
                              * Matrix.CreateFromAxisAngle( Normal, dir.ToRadians() );

                VPNT[ 0 ] = new VertexPositionNormalTexture(
                    Vector3.Transform( face[ 0 ], RotatePlane ) + Normal,
                    Normal, textureTopLeft );

                VPNT[ 1 ] = new VertexPositionNormalTexture(
                    Vector3.Transform( face[ 1 ], RotatePlane ) + Normal,
                    Normal, textureBottomLeft );

                VPNT[ 2 ] = new VertexPositionNormalTexture(
                    Vector3.Transform( face[ 2 ], RotatePlane ) + Normal,
                    Normal, textureTopRight );

                VPNT[ 3 ] = new VertexPositionNormalTexture(
                    Vector3.Transform( face[ 3 ], RotatePlane ) + Normal,
                    Normal, textureBottomLeft );

                VPNT[ 4 ] = new VertexPositionNormalTexture(
                    Vector3.Transform( face[ 4 ], RotatePlane ) + Normal,
                    Normal, textureBottomRight );

                VPNT[ 5 ] = new VertexPositionNormalTexture(
                    Vector3.Transform( face[ 5 ], RotatePlane ) + Normal,
                    Normal, textureTopRight );
            }

            public override void Initialize()
            {
                base.Initialize();

                this.Visible = false;
                BackgroundColor = Color.Transparent;
                //this.Render2D( new GameTime() );
            }

            protected override void LoadContent()
            {
                base.LoadContent();

                mFont = Game.Content.Load< SpriteFont >( "MessageFont" );
            }
            

            public override void Update( GameTime gameTime )
            {
                base.Update( gameTime );

            }

            public override void Draw( GameTime gameTime )
            {
                base.Draw( gameTime );

                SpriteBatch spriteBatch = new SpriteBatch( GraphicsDevice );

                spriteBatch.Begin();
                spriteBatch.DrawString( mFont,
                                        Name,
                                        new Vector2( WIDTH, HEIGHT ) / 2,
                                        Color.Black,
                                        0,//Up.ToRadians(),
                                        mFont.MeasureString( Name ) / 2,
                                        1f,
                                        SpriteEffects.None,
                                        0 );

                if ( Cube.CurrentFace == this )
                {
                    //Cube.mPlayer.Draw( gameTime );
                }

                spriteBatch.End();
            }

            public void Render2D( GameTime gameTime )
            {
                RenderTarget2D renderTarget = new RenderTarget2D( GraphicsDevice, WIDTH, HEIGHT );
                {
                    var renderTargets = GraphicsDevice.GetRenderTargets();
                    
                    // Set the current graphics device to the render target and clear it
                    GraphicsDevice.SetRenderTarget( renderTarget );
                    GraphicsDevice.Clear( BackgroundColor );

                    this.Draw( gameTime );
                    Texture = renderTarget;

                    // Now switch back to the default target (i.e., the primary display) and set it up
                    GraphicsDevice.SetRenderTargets( renderTargets );
                    GraphicsDevice.Clear( Color.CornflowerBlue );
                }
            }

            public void Render3D( Effect effect )
            {
                effect.Parameters[ "Texture" ].SetValue( Texture );

                foreach ( EffectPass pass in effect.CurrentTechnique.Passes )
                {
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives( PrimitiveType.TriangleList, VPNT, 0, 2 );
                }
            }

        }
    }
}
