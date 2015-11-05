using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube
{
    public partial class Cube
    {
        /// Note: the term "face-direction" is used to mean a direction relative to a face. If text 
        /// were to be rendered on a face, the North face-direction would always pass through the 
        /// bottom then the top of the text and the East face-direction would always point in the 
        /// direction that the text is to be read (assuming a ltr language).

        /// <summary>
        /// Rotates the camera about the cube so that the face adjacent to the current right edge 
        /// becomes the current face.
        /// </summary>
        public void RotateRight()
        {
            Rotate( +UpDir );
        }

        /// <summary>
        /// Rotates the camera about the cube so that the face adjacent to the current left edge 
        /// becomes the current face.
        /// </summary>
        public void RotateLeft()
        {
            Rotate( -UpDir );
        }

        /// <summary>
        /// Rotates the camera about the cube so that the face adjacent to the current top edge 
        /// becomes the current face.
        /// </summary>
        public void RotateTop()
        {
            Rotate( UpDir );
        }

        /// <summary>
        /// Rotates the camera about the cube so that the face adjacent to the current bottom edge 
        /// becomes the current face.
        /// </summary>
        public void RotateBottom()
        {
            Rotate( ~UpDir );
        }

        /// <summary>
        /// Rotates the camera about the cube so that the current top edge becomes the right edge.
        /// </summary>
        public void RotateClockwise()
        {
            --UpDir;
            //Screen.Camera.AnimatePosition( CameraDistance * CurrentFace.Normal, CameraDistance );
            //Screen.Camera.AnimateUpVector( ComputeUpVector(), 1 );
        }

        /// <summary>
        /// Rotates the camera about the cube so that the current top edge becomes the left edge.
        /// </summary>
        public void RotateAntiClockwise()
        {
            ++UpDir;
            //Screen.Camera.AnimatePosition( CameraDistance * CurrentFace.Normal, CameraDistance );
            //Screen.Camera.AnimateUpVector( ComputeUpVector(), 1 );
        }

        /// <summary>
        /// Rotates the camera about the cube so that the face adjacent to the current edge in the 
        /// specified face-direction becomes the current face. The input direction is cube 
        /// orientation-agnostic.
        /// </summary>
        /// <param name="direction">The direction to rotate towards, relative to the current 
        /// face.</param>
        public void Rotate( CompassDirection direction )
        {
            Face nextFace = CurrentFace.AdjacentFace( direction );
            CompassDirection backDir = nextFace.BackwardsDirectionFrom( CurrentFace );

            UpDir = GetNextUpDirection( direction, backDir );
            CurrentFace = nextFace;

            //Screen.Camera.AnimatePosition( CurrentFace.Normal * CameraDistance, CameraDistance );
            //Screen.Camera.AnimateUpVector( ComputeUpVector(), 1 );
        }

        /// <summary>
        /// Rotates the camera about the cube so that the face adjacent to the current edge in the 
        /// specified face-direction becomes the current face. The input direction is cube 
        /// orientation-agnostic. This is a convenience overload accepting a 
        /// Nullable&lt;CompassDirection&gt; as a parameter.
        /// </summary>
        /// <param name="direction">The direction to rotate towards, relative to the current 
        /// face, or null for no rotation.</param>
        public void Rotate( CompassDirection? direction )
        {
            if ( direction.HasValue )
                Rotate( direction.Value );
        }

        /// <summary>
        /// Gets the face-direction which would point towards the top of the screen after a 
        /// rotation in a specified direction, given that a second rotation in another specified 
        /// face-direction will undo the first rotation.
        /// </summary>
        /// <param name="rotation">The direction the cube will be rotated.</param>
        /// <param name="backwards">A direction which would undo the first rotation.</param>
        /// <returns>The face-direction which would point towards the top of the screen.</returns>
        private Direction GetNextUpDirection( Direction rotation, Direction backwards )
        {
            return Cube.GetNextUpDirection( UpDir, rotation, backwards );
        }


        public static Direction GetNextUpDirection( Direction up, Direction rotation, Direction backwards )
        {
            // This method was derived by generalizing a triply nested switch statement which 
            // contained [4^3 = 64] cases.

            if ( up == rotation )
                return ~backwards;

            if ( up == +rotation )
                return -backwards;

            if ( up == ~rotation )
                return backwards;

            if ( up == -rotation )
                return +backwards;

            // There are only 4 possible values for directions.
            throw new Tools.WtfException();
        }
    }
}
