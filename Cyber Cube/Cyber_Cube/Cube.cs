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
    public class Cube
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

        private Dictionary< Complex, Face > mFaces = new Dictionary< Complex, Face >( 6 );

        private Complex z;

        public Complex Z
        {
            get
            {
                return z;
            }

            private set
            {
                z = value != value // Check for NaN as a result from division by zero.
                    ? double.PositiveInfinity
                    : value;
            }
        }

        public Face CurrentFace
        {
            get
            {
                return mFaces[ Z ];
            }
        }

        public void RotateRight()
        {
            Z = -Complex.ImaginaryOne * Z;
        }

        public void RotateLeft()
        {
            Z = Complex.ImaginaryOne * Z;
        }

        public void RotateUp()
        {
            Z = (Z + 1) / (Z - 1);
        }

        public void RotateDown()
        {
            Z = -((Z - 1) / (Z + 1));
        }

        public Cube()
        {
            mFaces[ double.PositiveInfinity ] = mTopFace;
            mFaces[ 0 ]                       = mBottomFace;
            mFaces[ 1 ]                       = mFrontFace;
            mFaces[ Complex.ImaginaryOne ]    = mLeftFace;
            mFaces[ -1 ]                      = mBackFace;
            mFaces[ -Complex.ImaginaryOne ]   = mRightFace;

            Z = 1;

            /*mFrontFace.NorthFace = mTopFace;
            mFrontFace.EastFace = mRightFace;
            mFrontFace.SouthFace = mBottomFace;
            mFrontFace.WestFace = mLeftFace;

            mBackFace.NorthFace = mTopFace;
            mBackFace.EastFace = mLeftFace;
            mBackFace.SouthFace = mBottomFace;
            mBackFace.WestFace = mRightFace;

            mLeftFace.NorthFace = mTopFace;
            mLeftFace.EastFace = mFrontFace;
            mLeftFace.SouthFace = mBottomFace;
            mLeftFace.WestFace = mBackFace;

            mRightFace.NorthFace = mTopFace;
            mRightFace.EastFace = mBackFace;
            mRightFace.SouthFace = mBottomFace;
            mRightFace.WestFace = mFrontFace;

            //mFace.NorthFace = mFace;
            //mFace.EastFace = mFace;
            //mFace.SouthFace = mFace;
            //mFace.WestFace = mFace;*/
        }

    }
}
