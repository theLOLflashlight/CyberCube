using CyberCube.Physics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube
{
    public partial class Cube
    {

        public partial class Face
        {

            private Vector2? mMouseDragStart;
            private Rectangle? mTentativeRec;

            private void EditPass( GameTime gameTime )
            {
                var input = Game.Input;

                if ( mMouseDragStart == null )
                    mTentativeRec = null;


                Vector2? mouseFacePos = MouseFacePosition();
                if ( mouseFacePos != null )
                    UpdateMouse( gameTime, mouseFacePos.Value );


                if ( mMouseDragStart != null && input.Mouse_WasRightReleased() )
                {
                    if ( mTentativeRec != null )
                        AddRectangle( mTentativeRec.Value );
                    mMouseDragStart = null;
                }

                if ( mFocusSolid != null && input.Mouse_WasLeftReleased() )
                {
                    mFocusSolid.Body.Enabled = true;
                    mFocusSolid = null;
                }
            }



            private void UpdateMouse( GameTime gameTime, Vector2 mouseFacePos )
            {
                var input = Game.Input;

                if ( mMouseDragStart != null )
                {
                    Vector2 pos = mMouseDragStart.Value;

                    int x = (int) Math.Min( pos.X, mouseFacePos.X );
                    int y = (int) Math.Min( pos.Y, mouseFacePos.Y );

                    int w = (int) Math.Abs( mouseFacePos.X - pos.X );
                    int h = (int) Math.Abs( mouseFacePos.Y - pos.Y );

                    if ( w > 0.5 * Physics.Constants.UNIT_TO_PIXEL
                            && h > 0.5 * Physics.Constants.UNIT_TO_PIXEL )
                        mTentativeRec = new Rectangle( x, y, w, h );
                    else
                        mTentativeRec = null;
                }

                if ( input.Mouse_WasRightPressed() )
                {
                    mMouseDragStart = mouseFacePos;
                }


                if ( mFocusSolid != null )
                {
                    Vector2 worldPos = mouseFacePos * Physics.Constants.PIXEL_TO_UNIT;
                    Vector2 delta = worldPos - mFocusClickPosition;

                    mFocusSolid.Body.Position += delta;
                    mFocusClickPosition = worldPos;
                }

                if ( input.Mouse_WasLeftPressed() )
                {
                    Vector2 clickPos = mouseFacePos * Physics.Constants.PIXEL_TO_UNIT;

                    mFocusSolid = FindSolidAt( ref clickPos );
                    if ( mFocusSolid != null )
                    {
                        mFocusLocalPoint = mFocusSolid.Body.GetLocalPoint( ref clickPos );
                        mFocusClickPosition = clickPos;
                        mFocusSolid.Body.Enabled = false;
                    }
                }

                if ( input.Mouse_WasMiddlePressed() )
                {
                    Solid solid = FindSolidAt( mouseFacePos * Physics.Constants.PIXEL_TO_UNIT );
                    if ( solid != null )
                    {
                        switch ( solid.BodyType )
                        {
                        case BodyType.Static:
                            solid.BodyType = BodyType.Dynamic;
                            break;
                        case BodyType.Dynamic:
                            solid.BodyType = BodyType.Static;
                            break;
                        }
                    }
                }
            }


            private Solid mFocusSolid;
            private Vector2 mFocusLocalPoint;
            private Vector2 mFocusClickPosition;

            private Solid FindSolidAt( Vector2 point )
            {
                return FindSolidAt( ref point );
            }

            private Solid FindSolidAt( ref Vector2 point )
            {
                foreach ( Solid solid in mSolids )
                    foreach ( Fixture f in solid.Body.FixtureList )
                        if ( f.TestPoint( ref point ) )
                            return solid;

                return null;
            }

            private Vector2? MouseFacePosition()
            {
                Vector3? mouseWorldPos = Cube.GetMouseWorldPosition();
                if ( mouseWorldPos != null
                     && Cube.GetFaceFromPosition( mouseWorldPos.Value ) == this )
                        return ConvertWorldToFace( mouseWorldPos.Value );

                return null;
            }

            private void AddRectangle( Rectangle rec )
            {
                var box = new RecSolid(
                    Game,
                    World,
                    rec,
                    BodyType.Dynamic,
                    rec.Width * rec.Height * 0.003f,
                    Category.Cat2 );

                box.Body.UseAdHocGravity = true;
                box.Body.AdHocGravity = Game.Player.Gravity;

                AddSolid( box );
            }

            private void EditDraw( GameTime gameTime )
            {
                mSpriteBatch.Begin();

                if ( mTentativeRec != null )
                    mSpriteBatch.Draw( pixel, mTentativeRec.Value, new Color( 0, 0, 0, 128 ) );

                mSpriteBatch.DrawString( mFont,
                                         Name,
                                         new Vector2( WIDTH, HEIGHT ) / 2,
                                         new Color( 0, 0, 0, 128 ),
                                         0, // Rotation is handled via texture orientation.
                                         mFont.MeasureString( Name ) / 2,
                                         5,
                                         SpriteEffects.None,
                                         0 );

                mSpriteBatch.End();
            }

        }
        

        private void EditPass( GameTime gameTime )
        {
        }

        public Vector3? GetMouseWorldPosition()
        {
            var input = Game.Input;

            Line3 line;
            line.P0 = GraphicsDevice.Viewport.Unproject(
                new Vector3( input.Mouse.X, input.Mouse.Y, 0 ),
                Effect.Projection, Effect.View, Effect.World );
            line.P1 = GraphicsDevice.Viewport.Unproject(
                new Vector3( input.Mouse.X, input.Mouse.Y, 1 ),
                Effect.Projection, Effect.View, Effect.World );

            float? dist = line.Intersects( BoundingBox );
            if ( dist != null )
            {
                float t = dist.Value / line.Length;
                if ( t < 0 || 1 < t )
                    return null;

                Vector3 worldMouse = line[ t ];

                // Due to imprecision, sometimes the resulting coordinate doesn't 
                // lie exactly on the cube.

                Vector3 delta = new Vector3(
                    worldMouse.X < 0 ? 1 + worldMouse.X : 1 - worldMouse.X,
                    worldMouse.Y < 0 ? 1 + worldMouse.Y : 1 - worldMouse.Y,
                    worldMouse.Z < 0 ? 1 + worldMouse.Z : 1 - worldMouse.Z );

                float min = Math.Min( delta.X, Math.Min( delta.Y, delta.Z ) );

                if ( delta.X == min )
                    worldMouse.X = worldMouse.X < 0 ? -1 : 1;

                if ( delta.Y == min )
                    worldMouse.Y = worldMouse.Y < 0 ? -1 : 1;

                if ( delta.Z == min )
                    worldMouse.Z = worldMouse.Z < 0 ? -1 : 1;

                return worldMouse;
            }

            return null;
        } 

    }
}
