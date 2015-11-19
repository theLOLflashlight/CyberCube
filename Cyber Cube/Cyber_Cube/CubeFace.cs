﻿using CyberCube.Physics;
using CyberCube.Tools;
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

    public enum CubeFaceType
    {
        Front, Back, Top, Bottom, Left, Right
    }

    public partial class Cube
    {
        public abstract partial class Face : DrawableCubeGameObject
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
            private VertexPositionNormalTextureColor[] mVertexData = new VertexPositionNormalTextureColor[ 6 ];

            protected Texture2D pixel;

            public Cube Cube
            {
                get; private set;
            }

            public CubeFaceType Type
            {
                get; set;
            }

            public string Name
            {
                get {
                    return Type.ToString();
                }
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

            public Face( Cube cube, CubeFaceType type, Vector3 normal, Vector3 up, Direction rotation )
                : base( cube.Game )
            {
                Cube = cube;
                Type = type;
                Normal = Vector3.Normalize( normal );
                UpVec = Vector3.Normalize( up );
                Rotation = -rotation.Angle;
                BackgroundColor = Color.Gray;

                SetUpVertices();
                SetUpWorld();
            }

            private void SetUpVertices()
            {
                Vector3[] vertex = new Vector3[ 6 ];
                // top left
                vertex[ 0 ] = new Vector3( -1, 1, 0 );
                // bottom left
                vertex[ 1 ] = new Vector3( -1, -1, 0 );
                // top right
                vertex[ 2 ] = new Vector3( 1, 1, 0 );
                // bottom left
                vertex[ 3 ] = new Vector3( -1, -1, 0 );
                // bottom right
                vertex[ 4 ] = new Vector3( 1, -1, 0 );
                // top right
                vertex[ 5 ] = new Vector3( 1, 1, 0 );

                Vector2 textureTopLeft = Vector2.Zero;
                Vector2 textureTopRight = Vector2.UnitX;
                Vector2 textureBottomLeft = Vector2.UnitY;
                Vector2 textureBottomRight = Vector2.One;

                Matrix rotation = Vector3.UnitZ.RotateOntoM( Normal )
                                  * Matrix.CreateFromAxisAngle( Normal, Rotation );

                mVertexData[ 0 ] = new VertexPositionNormalTextureColor(
                    vertex[ 0 ].Transform( rotation ) + Normal,
                    Normal, textureTopLeft );

                mVertexData[ 1 ] = new VertexPositionNormalTextureColor(
                    vertex[ 1 ].Transform( rotation ) + Normal,
                    Normal, textureBottomLeft );

                mVertexData[ 2 ] = new VertexPositionNormalTextureColor(
                    vertex[ 2 ].Transform( rotation ) + Normal,
                    Normal, textureTopRight );

                mVertexData[ 3 ] = new VertexPositionNormalTextureColor(
                    vertex[ 3 ].Transform( rotation ) + Normal,
                    Normal, textureBottomLeft );

                mVertexData[ 4 ] = new VertexPositionNormalTextureColor(
                    vertex[ 4 ].Transform( rotation ) + Normal,
                    Normal, textureBottomRight );

                mVertexData[ 5 ] = new VertexPositionNormalTextureColor(
                    vertex[ 5 ].Transform( rotation ) + Normal,
                    Normal, textureTopRight );
            }

            public void Initialize()
            {
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

            public void Render2D( GameTime gameTime, byte backgroundAlpha = 255 )
            {
                var tmp = GraphicsDevice.GetRenderTargets();

                // Set the current graphics device to the render target and clear it
                GraphicsDevice.SetRenderTarget( RenderTarget );
                Color color = BackgroundColor;
                color.A = backgroundAlpha;
                GraphicsDevice.Clear( color );

                // Draw our face on the RenderTarget
                this.Draw( gameTime );

                // Now switch back to the default target (i.e., the primary display) and set it up
                GraphicsDevice.SetRenderTargets( tmp );
                GraphicsDevice.Clear( Game.BackgroundColor );
            }

            public void Render3D( CubeEffect effect )
            {
                effect.Texture = RenderTarget;

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
