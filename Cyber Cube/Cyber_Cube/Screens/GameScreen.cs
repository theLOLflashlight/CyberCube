using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Screens
{
    public abstract class GameScreen : DrawableCubeGameComponent
    {

        public ScreenManager ScreenManager
        {
            get; internal set;
        }

        public GameScreen( CubeGame game )
            : base( game )
        {
        }

        public void Back()
        {
            ScreenManager.PopScreen();
        }


    }
}
