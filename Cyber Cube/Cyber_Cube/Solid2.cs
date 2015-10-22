using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube
{
    public abstract class Solid2
    {
        public abstract Vector2? Collide( Vector2 pos, Vector2 oldPos, ref Vector2 normal );

        public abstract void Draw( SpriteBatch batch, Texture2D pixel );

    }

    public class OneWayLine : Solid2
    {
        public Line2 Line;

        public OneWayLine( Line2 line )
        {
            Line = line;
        }

        public bool Vertical
        {
            get {
                return Line.P0.Y == Line.P1.Y;
            }
        }

        public bool Horizontal
        {
            get {
                return Line.P0.X == Line.P1.X;
            }
        }

        public bool Positive
        {
            get {
                return Horizontal ? Line.P0.Y < Line.P1.Y : Line.P0.X < Line.P1.X;
            }
        }

        public override Vector2? Collide( Vector2 pos, Vector2 oldPos, ref Vector2 normal )
        {
            float Ax = Line.P0.X;
            float Ay = Line.P0.Y;
            float Bx = Line.P1.X;
            float By = Line.P1.Y;

            float rx = Math.Min( Ax, Bx );
            float ry = Math.Min( Ay, By );

            Rectangle rec = new Rectangle( (int) rx, (int) ry,
                                           (int) (Math.Max( Ax, Bx ) - rx),
                                           (int) (Math.Max( Ay, By ) - ry) );

            Vector2? newPos = null;

            float oldRel = (Bx - Ax) * (oldPos.Y - Ay) - (By - Ay) * (oldPos.X - Ax);
            float rel = (Bx - Ax) * (pos.Y - Ay) - (By - Ay) * (pos.X - Ax);

            oldRel = (float) Math.Round( oldRel, 3 );
            rel = (float) Math.Round( rel, 3 );

            if ( oldRel <= 0 && rel > 0 )
            {
                newPos = Line.Intersection( oldPos, pos );
            }
            else if ( oldRel == 0 && rel == 0
                      && rec.Contains( pos, 0.001f ) )
            {
                newPos = pos;
            }

            if ( newPos != null )
            {
                Vector2 vec = Line.P1 - Line.P0;
                vec.Normalize();
                normal = new Vector2(
                    Line.P1.X > Line.P0.X ? -vec.Y : vec.Y,
                    Line.P1.Y > Line.P0.Y ? -vec.X : vec.X );
            }

            return newPos;
        }

        public override void Draw( SpriteBatch batch, Texture2D pixel )
        {
            Vector2 edge = Line.P1 - Line.P0;
            // calculate angle to rotate line
            float angle = (float) Math.Atan2( edge.Y, edge.X );

            batch.Draw(pixel,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)Line.P0.X,
                    (int)Line.P0.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    3), //width of line, change this to make thicker line
                null,
                new Color( 0, 0, 0, 64 ), //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

            batch.Draw(pixel,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)Line.P0.X,
                    (int)Line.P0.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    2), //width of line, change this to make thicker line
                null,
                new Color( 0, 0, 0, 64 ), //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

            batch.Draw(pixel,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)Line.P0.X,
                    (int)Line.P0.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                Color.Black, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);
        }
    }

    public class RecSolid2 : Solid2
    {
        public Rectangle Rec;

        public RecSolid2( Rectangle rec )
        {
            Rec = rec;
        }

        public override Vector2? Collide( Vector2 pos, Vector2 oldPos, ref Vector2 normal )
        {
            Vector2? newPos = null;

            if ( Rec.Contains( pos, 1 ) )
            {
                Line2 top = new Line2(
                    Rec.Left, Rec.Top,
                    Rec.Right, Rec.Top );
                Line2 right = new Line2(
                    Rec.Right, Rec.Top,
                    Rec.Right, Rec.Bottom );
                Line2 bottom = new Line2(
                    Rec.Right, Rec.Bottom,
                    Rec.Left, Rec.Bottom );
                Line2 left = new Line2(
                    Rec.Left, Rec.Bottom,
                    Rec.Left, Rec.Top );

                newPos = LineCollide( top, pos, oldPos, ref normal )
                         ?? LineCollide( right, pos, oldPos, ref normal )
                         ?? LineCollide( bottom, pos, oldPos, ref normal )
                         ?? LineCollide( left, pos, oldPos, ref normal );

                if ( newPos == null && Rec.Contains( pos ) )
                {
                    newPos = pos.NearestPointOn( Rec );

                    Vector2 groundNormal = pos - newPos.Value;
                    if ( groundNormal != Vector2.Zero )
                        normal = Vector2.Normalize( groundNormal );
                }
            }

            return newPos;
        }

        private Vector2? LineCollide( Line2 line, Vector2 pos, Vector2 oldPos, ref Vector2 normal )
        {
            float Ax = line.P0.X;
            float Ay = line.P0.Y;
            float Bx = line.P1.X;
            float By = line.P1.Y;

            Vector2? newPos = null;

            float oldRel = (Bx - Ax) * (oldPos.Y - Ay) - (By - Ay) * (oldPos.X - Ax);
            float rel = (Bx - Ax) * (pos.Y - Ay) - (By - Ay) * (pos.X - Ax);

            oldRel = (float) Math.Round( oldRel, 3 );
            rel = (float) Math.Round( rel, 3 );

            if ( oldRel <= 0 && rel > 0 )
            {
                newPos = line.Intersection( oldPos, pos );
            }
            else if ( oldRel == 0 && rel == 0 )
            {
                newPos = pos;
            }

            if ( newPos != null )
            {
                Vector2 vec = line.P1 - line.P0;
                vec.Normalize();
                normal = new Vector2(
                    line.P1.X > line.P0.X ? -vec.Y : vec.Y,
                    line.P1.Y > line.P0.Y ? -vec.X : vec.X );
            }

            return newPos;
        }

        public override void Draw( SpriteBatch batch, Texture2D pixel )
        {
            batch.Draw( pixel, Rec, Color.Black );
        }
        
    }
}
