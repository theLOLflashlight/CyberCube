using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Graphics
{
    public static class Utils
    {


        public static void DrawBody( this Body body, GraphicsDevice device, Texture2D texture, GameTime gameTime )
        {
            using ( BasicEffect effect = new BasicEffect( device ) )
            {
                effect.Texture = texture;
                effect.TextureEnabled = true;
                effect.Projection = Matrix.CreateOrthographicOffCenter(
                0, Cube.Face.WIDTH, Cube.Face.HEIGHT, 0, 0, 1 );

                Transform transform;
                body.GetTransform( out transform );

                Matrix t = Matrix.CreateTranslation( new Vector3( body.Position, 0 ) );
                Matrix r = Matrix.CreateRotationZ( transform.q.GetAngle() );

                effect.View = r * t;

                foreach ( Fixture f in body.FixtureList )
                {
                    PolygonShape pgon = f.Shape as PolygonShape;
                    if ( pgon != null )
                    {
                        VertexPositionTexture[] verts = new VertexPositionTexture[ pgon.Vertices.Count ];

                        int i = 0;
                        foreach ( Vector2 vtex in pgon.Vertices )
                            verts[ i++ ] = new VertexPositionTexture(
                                new Vector3( vtex, 0 ) * Physics.Constants.UNIT_TO_PIXEL,
                                Vector2.Zero );

                        foreach ( var pass in effect.CurrentTechnique.Passes )
                        {
                            pass.Apply();
                            device.DrawUserIndexedPrimitives( PrimitiveType.TriangleStrip, verts, 0, verts.Length,
                                new short[] { 0, 1, 2, 3, 0 }, 0, 3 );
                        }
                    }
                }
            }
        }


        public static void DrawLine( this SpriteBatch batch, Vector2 p0, Vector2 p1, Texture2D texture, float thickness = 1 )
        {
            Vector2 edge = p1 - p0;
            float angle = (float) Math.Atan2( edge.Y, edge.X );

            batch.Draw(
                texture,
                new Rectangle(
                    (int) p0.X,
                    (int) p0.Y,
                    (int) edge.Length(),
                    (int) thickness ),
                null,
                Color.White,
                angle,
                Vector2.Zero,
                SpriteEffects.None,
                0 );
        }

        public static void DrawRect( this SpriteBatch batch, Rectangle rect, Color color )
        {
            Texture2D texture = new Texture2D( batch.GraphicsDevice, 1, 1 );
            texture.SetData( new Color[] { Color.White } );
            batch.Draw( texture, rect, color );
        }

    }
}
