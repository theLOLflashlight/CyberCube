using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{
    public partial class Cube
    {
        public void Rotate( CompassDirections direction )
        {
            switch ( direction )
            {
            case CompassDirections.North:
                RotateNorth();
                break;
            case CompassDirections.East:
                RotateEast();
                break;
            case CompassDirections.South:
                RotateSouth();
                break;
            case CompassDirections.West:
                RotateWest();
                break;
            }
            mCamera.AnimatePosition( 7 * CurrentFace.Normal, 7 );
            mCamera.AnimateUpVector( GetUpVector(), 1 );
        }

        public void RotateRight()
        {
            Rotate( +Up );
        }

        public void RotateLeft()
        {
            Rotate( -Up );
        }

        public void RotateUp()
        {
            Rotate( Up );
        }

        public void RotateDown()
        {
            Rotate( ~Up );
        }

        public void RotateClockwise()
        {
            --Up;
            mCamera.AnimatePosition( 7 * CurrentFace.Normal, 7 );
            mCamera.AnimateUpVector( GetUpVector(), 1 );
        }

        public void RotateAntiClockwise()
        {
            ++Up;
            mCamera.AnimatePosition( 7 * CurrentFace.Normal, 7 );
            mCamera.AnimateUpVector( GetUpVector(), 1 );
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
            if ( Up == rotation )
                return ~backTrack;

            if ( Up == +rotation )
                return -backTrack;

            if ( Up == ~rotation )
                return backTrack;

            if ( Up == -rotation )
                return +backTrack;

            throw new Exception( "WTF" ); // What a Terrible Failure.
        }

        private void RotateNorth()
        {
            Face nextFace = CurrentFace.NorthFace;
            var backTrack = FaceAdjacency( nextFace, CurrentFace );

            Up = GetNewUpDirection( CompassDirections.North, backTrack );
            CurrentFace = nextFace;
        }

        private void RotateEast()
        {
            Face nextFace = CurrentFace.EastFace;
            var backTrack = FaceAdjacency( nextFace, CurrentFace );

            Up = GetNewUpDirection( CompassDirections.East, backTrack );
            CurrentFace = nextFace;
        }

        private void RotateSouth()
        {
            Face nextFace = CurrentFace.SouthFace;
            var backTrack = FaceAdjacency( nextFace, CurrentFace );

            Up = GetNewUpDirection( CompassDirections.South, backTrack );
            CurrentFace = nextFace;
        }

        private void RotateWest()
        {
            Face nextFace = CurrentFace.WestFace;
            var backTrack = FaceAdjacency( nextFace, CurrentFace );

            Up = GetNewUpDirection( CompassDirections.West, backTrack );
            CurrentFace = nextFace;
        }

    }
}
