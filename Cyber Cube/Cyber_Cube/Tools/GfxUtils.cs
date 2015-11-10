using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Tools
{
    public static class GfxUtils
    {

        public static void DrawLine( this SpriteBatch batch, Line2 line, Texture2D texture, Color color, float thickness = 1 )
        {
            batch.DrawLine( line.P0, line.P1, texture, color, thickness );
        }

        public static void DrawLine( this SpriteBatch batch, Vector2 p0, Vector2 p1, Texture2D texture, Color color, float thickness = 1 )
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
                color,
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
