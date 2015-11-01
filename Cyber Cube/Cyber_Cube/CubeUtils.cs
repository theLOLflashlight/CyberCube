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
        /// <summary>
        /// Gets each face of the cube.
        /// </summary>
        public IEnumerable< Face > Faces
        {
            get {
                yield return mFrontFace;
                yield return mBackFace;
                yield return mTopFace;
                yield return mBottomFace;
                yield return mLeftFace;
                yield return mRightFace;
            }
        }

        public partial class Face
        {
            internal Solid FindSolidAt( Vector2 point )
            {
                return FindSolidAt( ref point );
            }

            internal Solid FindSolidAt( ref Vector2 point )
            {
                foreach ( Solid solid in mSolids )
                    foreach ( Fixture f in solid.Body.FixtureList )
                        if ( f.TestPoint( ref point ) )
                            return solid;

                return null;
            }

            protected Vector2? MouseFacePosition()
            {
                Vector3? mouseWorldPos = Cube.GetMouseWorldPosition();
                if ( mouseWorldPos != null
                     && Cube.GetFaceFromPosition( mouseWorldPos.Value ) == this )
                        return ConvertWorldToFace( mouseWorldPos.Value );

                return null;
            }

            public Vector2 Transform3dTo2d( Vector3 vec3d )
            {
                vec3d = vec3d.Rotate( Normal, -Rotation )
                             .Transform( Utils.RotateOntoQ( Normal, Vector3.UnitZ ) );
                return new Vector2( vec3d.X, -vec3d.Y );
            }

            public Vector2 ConvertWorldToFace( Vector3 vec3d )
            {
                return Transform3dTo2d( vec3d )
                       * (SIZE / 2f)
                       + new Vector2( SIZE / 2f );
            }

        }

        public Face GetFaceFromPosition( Vector3 vec )
        {
            var x = vec.X;
            var y = vec.Y;
            var z = vec.Z;

            var l = -1;
            var t = 1;
            var r = 1;
            var e = -1;
            var f = 1;
            var b = -1;

            x = MathHelper.Clamp( x, l, r );
            y = MathHelper.Clamp( y, e, t );
            z = MathHelper.Clamp( z, b, f );

            var dl = Math.Abs( x - l );
            var dr = Math.Abs( x - r );
            var dt = Math.Abs( y - t );
            var de = Math.Abs( y - e );
            var df = Math.Abs( z - f );
            var db = Math.Abs( z - b );

            var m = Math.Min( Math.Min( df, Math.Min( dl, dr ) ),
                              Math.Min( db, Math.Min( dt, de ) ) );

            if ( m == df )
                return mFrontFace;

            if ( m == db )
                return mBackFace;

            if ( m == dt )
                return mTopFace;

            if ( m == de )
                return mBottomFace;

            if ( m == dl )
                return mLeftFace;

            if ( m == dr )
                return mRightFace;

            throw new Tools.WtfException();
        }

		public Vector2 ComputeFacePosition( Vector3 worldPosition, Face cubeFace )
		{
			float adjustingFactor = Cube.Face.SIZE / 2;
			return Transform3dTo2d( worldPosition, cubeFace )
				   * adjustingFactor
				   + new Vector2( adjustingFactor );
		}

		private Vector2 Transform3dTo2d( Vector3 vec3d, Face cubeFace )
		{
			vec3d = vec3d.Rotate( cubeFace.Normal, -cubeFace.Rotation )
						 .Transform( Utils.RotateOntoQ( cubeFace.Normal, Vector3.UnitZ ) );
			return new Vector2( vec3d.X, -vec3d.Y );
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
