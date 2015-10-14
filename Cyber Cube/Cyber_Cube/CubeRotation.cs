using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{
    public partial class Cube
    {
        public void RotateRight()
        {
            Rotate( +UpDir );
        }

        public void RotateLeft()
        {
            Rotate( -UpDir );
        }

        public void RotateUp()
        {
            Rotate( UpDir );
        }

        public void RotateDown()
        {
            Rotate( ~UpDir );
        }

        public void RotateClockwise()
        {
            --UpDir;
            mCamera.AnimatePosition( CameraDistance * CurrentFace.Normal, CameraDistance );
            mCamera.AnimateUpVector( ComputeUpVector(), 1 );
        }

        public void RotateAntiClockwise()
        {
            ++UpDir;
            mCamera.AnimatePosition( CameraDistance * CurrentFace.Normal, CameraDistance );
            mCamera.AnimateUpVector( ComputeUpVector(), 1 );
        }

        public void Rotate( CompassDirections direction )
        {
            Face nextFace = CurrentFace.AdjacentFace( direction );
            var backTrack = FaceAdjacency( nextFace, CurrentFace );

            UpDir = GetNewUpDirection( direction, backTrack );
            CurrentFace = nextFace;

            mCamera.AnimatePosition( CameraDistance * CurrentFace.Normal, CameraDistance );
            mCamera.AnimateUpVector( ComputeUpVector(), 1 );
        }

        private static CompassDirections FaceAdjacency( Face source, Face target )
        {
            if ( source.NorthFace == target )
                return CompassDirections.North;

            if ( source.EastFace == target )
                return CompassDirections.East;

            if ( source.SouthFace == target )
                return CompassDirections.South;

            if ( source.WestFace == target )
                return CompassDirections.West;

            throw new Exception( "Faces are not connected." );
        }

        private Direction GetNewUpDirection( Direction rotation, Direction backTrack )
        {
            if ( UpDir == rotation )
                return ~backTrack;

            if ( UpDir == +rotation )
                return -backTrack;

            if ( UpDir == ~rotation )
                return backTrack;

            if ( UpDir == -rotation )
                return +backTrack;

            throw new Exception( "WTF" ); // What a Terrible Failure.
        }
    }
}
