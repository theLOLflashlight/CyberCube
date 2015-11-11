using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube
{
    public enum Action
    {
        None = 0,
        MoveLeft, MoveRight, MoveUp, MoveDown, Jump, JumpStop,
        PlaceClone, CycleClone, DeleteClone,
        RotateLeft, RotateRight, RotateUp, RotateDown, RotateClockwise, RotateAntiClockwise,
        ToggleCubeMode, PauseGame
    }
}
