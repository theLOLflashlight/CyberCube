using CyberCube.Physics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube
{
    public partial class Cube
    {

        private Vector2? mMouseDragStart;

        private void EditPass( GameTime gameTime )
        {
            var input = Game.Input;

            if ( input.Mouse_WasLeftPressed() )
            {
                Vector3? mouseWorldPos = GetMouseWorldPosition();
                if ( mouseWorldPos != null )
                {
                    Vector3 worldPos = mouseWorldPos.Value;

                    Face face = GetFaceFromPosition( worldPos );
                    Vector2 facePos = face.ConvertWorldToFace( worldPos );

                    mMouseDragStart = facePos;
                }
            }

            if ( input.Mouse_WasLeftReleased() && mMouseDragStart != null )
            {
                Vector3? mouseWorldPos = GetMouseWorldPosition();
                if ( mouseWorldPos != null )
                {
                    Vector3 worldPos = mouseWorldPos.Value;

                    Face face = GetFaceFromPosition( worldPos );
                    Vector2 facePos = face.ConvertWorldToFace( worldPos );

                    Vector2 pos = mMouseDragStart.Value;

                    int x = (int) Math.Min( pos.X, facePos.X );
                    int y = (int) Math.Min( pos.Y, facePos.Y );

                    int w = (int) Math.Abs( facePos.X - pos.X );
                    int h = (int) Math.Abs( facePos.Y - pos.Y );

                    var box = new RecSolid(
                        Game,
                        face.World,
                        new Rectangle( x, y, w, h ),
                        BodyType.Dynamic, w * h * 0.003f );

                    box.Body.UseAdHocGravity = true;
                    box.Body.AdHocGravity = Game.Player.Gravity;

                    face.AddSolid( box );
                }
                mMouseDragStart = null;
            }
        }

        public Vector3? GetMouseWorldPosition()
        {
            var input = Game.Input;

            Line3 line;

            line.P0 = GraphicsDevice.Viewport.Unproject(
                new Vector3( input.Mouse.X, input.Mouse.Y, 0 ),
                Effect.Projection,
                Effect.View,
                Effect.World );

            line.P1 = GraphicsDevice.Viewport.Unproject(
                new Vector3( input.Mouse.X, input.Mouse.Y, 1 ),
                Effect.Projection,
                Effect.View,
                Effect.World );

            float? dist = line.Intersects( BoundingBox );
            if ( dist != null )
            {
                float t = dist.Value / line.Length;
                if ( t < 0 || 1 < t )
                    return null;

                Vector3 worldMouse = line[ t ];

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
