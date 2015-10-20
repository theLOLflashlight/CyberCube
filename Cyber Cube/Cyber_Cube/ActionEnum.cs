using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube
{
    public enum Action
    {
        MoveLeft, MoveRight, MoveUp, MoveDown, Jump,
        RotateLeft, RotateRight, RotateUp, RotateDown, RotateClockwise, RotateAntiClockwise,
        ToggleCubeMode
    }
}
