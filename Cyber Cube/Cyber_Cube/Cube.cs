using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Cyber_Cube
{
    /// <summary>
    /// Represents an entire level. Contains the 6 faces of the cube.
    /// </summary>
    public partial class Cube
    {

        public class Face
        {
            public Face NorthFace { get; internal set; }
            public Face EastFace { get; internal set; }
            public Face SouthFace { get; internal set; }
            public Face WestFace { get; internal set; }

            public string Name { get; private set; }

            public Face( string name )
                : this( null, null, null, null )
            {
                Name = name;
            }

            public Face( Face northFace, Face eastFace, Face southFace, Face westFace )
            {
                NorthFace = northFace;
                EastFace = eastFace;
                SouthFace = southFace;
                WestFace = westFace;
            }
        }

        private Face mFrontFace = new Face( "Front" );
        private Face mBackFace = new Face( "Back" );
        private Face mLeftFace = new Face( "Left" );
        private Face mRightFace = new Face( "Right" );
        private Face mTopFace = new Face( "Top" );
        private Face mBottomFace = new Face( "Bottom" );

        public Face CurrentFace { get; private set; }

        public CompassDirections Up { get; private set; }

        /*
        Picture the cube laid out as:
            T
            F
          L X R
            B
        where:
            T = top
            F = front
            L = left
            X = bottom
            R = right
            B = back
        the adjacent faces will be labeled with respect to this diagram.
        */

        public Cube()
        {
            CurrentFace = mFrontFace;
            Up = CompassDirections.North;

            mFrontFace.NorthFace = mTopFace;
            mFrontFace.EastFace = mRightFace;
            mFrontFace.SouthFace = mBottomFace;
            mFrontFace.WestFace = mLeftFace;

            mBackFace.NorthFace = mBottomFace;
            mBackFace.EastFace = mRightFace;
            mBackFace.SouthFace = mTopFace;
            mBackFace.WestFace = mLeftFace;

            mLeftFace.NorthFace = mFrontFace;
            mLeftFace.EastFace = mBottomFace;
            mLeftFace.SouthFace = mBackFace;
            mLeftFace.WestFace = mTopFace;

            mRightFace.NorthFace = mFrontFace;
            mRightFace.EastFace = mTopFace;
            mRightFace.SouthFace = mBackFace;
            mRightFace.WestFace = mBottomFace;

            mTopFace.NorthFace = mBackFace;
            mTopFace.EastFace = mRightFace;
            mTopFace.SouthFace = mFrontFace;
            mTopFace.WestFace = mLeftFace;

            mBottomFace.NorthFace = mFrontFace;
            mBottomFace.EastFace = mRightFace;
            mBottomFace.SouthFace = mBackFace;
            mBottomFace.WestFace = mLeftFace;
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

        public void RotateRight()
        {
            switch ( Up )
            {
            case CompassDirections.North:
                RotateEast();
                break;
            case CompassDirections.East:
                RotateSouth();
                break;
            case CompassDirections.South:
                RotateWest();
                break;
            case CompassDirections.West:
                RotateNorth();
                break;
            }
        }

        public void RotateLeft()
        {
            switch ( Up )
            {
            case CompassDirections.North:
                RotateWest();
                break;
            case CompassDirections.East:
                RotateNorth();
                break;
            case CompassDirections.South:
                RotateEast();
                break;
            case CompassDirections.West:
                RotateSouth();
                break;
            }
        }

        public void RotateUp()
        {
            switch ( Up )
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
        }

        public void RotateDown()
        {
            switch ( Up )
            {
            case CompassDirections.North:
                RotateSouth();
                break;
            case CompassDirections.East:
                RotateWest();
                break;
            case CompassDirections.South:
                RotateNorth();
                break;
            case CompassDirections.West:
                RotateEast();
                break;
            }
        }


        private void RotateNorth()
        {
            Face nextFace = CurrentFace.NorthFace;
            var backTrack = FaceAdjacency( nextFace, CurrentFace );

            Up = NewUpDirection( CompassDirections.North, backTrack );
            CurrentFace = nextFace;
        }

        private void RotateEast()
        {
            Face nextFace = CurrentFace.EastFace;
            var backTrack = FaceAdjacency( nextFace, CurrentFace );

            Up = NewUpDirection( CompassDirections.East, backTrack );
            CurrentFace = nextFace;
        }

        private void RotateSouth()
        {
            Face nextFace = CurrentFace.SouthFace;
            var backTrack = FaceAdjacency( nextFace, CurrentFace );

            Up = NewUpDirection( CompassDirections.South, backTrack );
            CurrentFace = nextFace;
        }

        private void RotateWest()
        {
            Face nextFace = CurrentFace.WestFace;
            var backTrack = FaceAdjacency( nextFace, CurrentFace );

            Up = NewUpDirection( CompassDirections.West, backTrack );
            CurrentFace = nextFace;
        }

    }
}
