using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Editor
{
    public class EditableCube : Cube
    {
        public new CubeEditorGame Game
        {
            get {
                return (CubeEditorGame) base.Game;
            }
        }


        public EditableCube( CubeEditorGame game )
            : base( game )
        {
        }

    }
}
