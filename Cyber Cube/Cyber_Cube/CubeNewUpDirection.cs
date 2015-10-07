using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyber_Cube
{
    public partial class Cube
    {
        private CompassDirections NewUpDirection( CompassDirections rotation, CompassDirections backTrack )
        {
            switch ( Up )
            {
            case CompassDirections.North:
                switch ( rotation )
                {
                case CompassDirections.North:
                    switch ( backTrack )
                    {
                    case CompassDirections.North:
                        return CompassDirections.South;
                    case CompassDirections.East:
                        return CompassDirections.West;
                    case CompassDirections.South:
                        return CompassDirections.North;
                    case CompassDirections.West:
                        return CompassDirections.East;
                    }
                    break;
                case CompassDirections.East:
                    switch ( backTrack )
                    {
                    case CompassDirections.North:
                        return CompassDirections.East;
                    case CompassDirections.East:
                        return CompassDirections.South;
                    case CompassDirections.South:
                        return CompassDirections.West;
                    case CompassDirections.West:
                        return CompassDirections.East;
                    }
                    break;
                case CompassDirections.South:
                    switch ( backTrack )
                    {
                    case CompassDirections.North:
                        return CompassDirections.North;
                    case CompassDirections.East:
                        return CompassDirections.East;
                    case CompassDirections.South:
                        return CompassDirections.South;
                    case CompassDirections.West:
                        return CompassDirections.West;
                    }
                    break;
                case CompassDirections.West:
                    switch ( backTrack )
                    {
                    case CompassDirections.North:
                        return CompassDirections.West;
                    case CompassDirections.East:
                        return CompassDirections.North;
                    case CompassDirections.South:
                        return CompassDirections.East;
                    case CompassDirections.West:
                        return CompassDirections.South;
                    }
                    break;
                }
                break;


            case CompassDirections.East:
                switch ( rotation )
                {
                case CompassDirections.North:
                    switch ( backTrack )
                    {
                    case CompassDirections.North:
                        return CompassDirections.West;
                    case CompassDirections.East:
                        return CompassDirections.North;
                    case CompassDirections.South:
                        return CompassDirections.East;
                    case CompassDirections.West:
                        return CompassDirections.South;
                    }
                    break;
                case CompassDirections.East:
                    switch ( backTrack )
                    {
                    case CompassDirections.North:
                        return CompassDirections.South;
                    case CompassDirections.East:
                        return CompassDirections.West;
                    case CompassDirections.South:
                        return CompassDirections.North;
                    case CompassDirections.West:
                        return CompassDirections.East;
                    }
                    break;
                case CompassDirections.South:
                    switch ( backTrack )
                    {
                    case CompassDirections.North:
                        return CompassDirections.East;
                    case CompassDirections.East:
                        return CompassDirections.South;
                    case CompassDirections.South:
                        return CompassDirections.West;
                    case CompassDirections.West:
                        return CompassDirections.North;
                    }
                    break;
                case CompassDirections.West:
                    switch ( backTrack )
                    {
                    case CompassDirections.North:
                        return CompassDirections.North;
                    case CompassDirections.East:
                        return CompassDirections.East;
                    case CompassDirections.South:
                        return CompassDirections.South;
                    case CompassDirections.West:
                        return CompassDirections.West;
                    }
                    break;
                }
                break;


            case CompassDirections.South:
                switch ( rotation )
                {
                case CompassDirections.North:
                    switch ( backTrack )
                    {
                    case CompassDirections.North:
                        return CompassDirections.North;
                    case CompassDirections.East:
                        return CompassDirections.East;
                    case CompassDirections.South:
                        return CompassDirections.South;
                    case CompassDirections.West:
                        return CompassDirections.West;
                    }
                    break;
                case CompassDirections.East:
                    switch ( backTrack )
                    {
                    case CompassDirections.North:
                        return CompassDirections.West;
                    case CompassDirections.East:
                        return CompassDirections.North;
                    case CompassDirections.South:
                        return CompassDirections.East;
                    case CompassDirections.West:
                        return CompassDirections.South;
                    }
                    break;
                case CompassDirections.South:
                    switch ( backTrack )
                    {
                    case CompassDirections.North:
                        return CompassDirections.South;
                    case CompassDirections.East:
                        return CompassDirections.West;
                    case CompassDirections.South:
                        return CompassDirections.North;
                    case CompassDirections.West:
                        return CompassDirections.East;
                    }
                    break;
                case CompassDirections.West:
                    switch ( backTrack )
                    {
                    case CompassDirections.North:
                        return CompassDirections.East;
                    case CompassDirections.East:
                        return CompassDirections.South;
                    case CompassDirections.South:
                        return CompassDirections.West;
                    case CompassDirections.West:
                        return CompassDirections.North;
                    }
                    break;
                }
                break;


            case CompassDirections.West:
                switch ( rotation )
                {
                case CompassDirections.North:
                    switch ( backTrack )
                    {
                    case CompassDirections.North:
                        return CompassDirections.East;
                    case CompassDirections.East:
                        return CompassDirections.South;
                    case CompassDirections.South:
                        return CompassDirections.West;
                    case CompassDirections.West:
                        return CompassDirections.North;
                    }
                    break;
                case CompassDirections.East:
                    switch ( backTrack )
                    {
                    case CompassDirections.North:
                        return CompassDirections.North;
                    case CompassDirections.East:
                        return CompassDirections.East;
                    case CompassDirections.South:
                        return CompassDirections.South;
                    case CompassDirections.West:
                        return CompassDirections.West;
                    }
                    break;
                case CompassDirections.South:
                    switch ( backTrack )
                    {
                    case CompassDirections.North:
                        return CompassDirections.West;
                    case CompassDirections.East:
                        return CompassDirections.North;
                    case CompassDirections.South:
                        return CompassDirections.East;
                    case CompassDirections.West:
                        return CompassDirections.South;
                    }
                    break;
                case CompassDirections.West:
                    switch ( backTrack )
                    {
                    case CompassDirections.North:
                        return CompassDirections.South;
                    case CompassDirections.East:
                        return CompassDirections.West;
                    case CompassDirections.South:
                        return CompassDirections.North;
                    case CompassDirections.West:
                        return CompassDirections.East;
                    }
                    break;
                }
                break;
            }

            throw new Exception( "WTF" ); // What a Terrible Failure.
        }
    }
}
