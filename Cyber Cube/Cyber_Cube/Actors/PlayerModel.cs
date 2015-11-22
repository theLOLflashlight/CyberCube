using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Actors
{
    public partial class Player
    {
        private Model model3D;
        private SkinningData mSkinData;

        private void LoadModels()
        {
            //model3D = Game.Content.Load<Model>( @"Models\playerAlpha3D" );
            model3D = Game.Content.Load<Model>( @"Models\playerBeta" );

            mSkinData = model3D.Tag as SkinningData;
            if ( mSkinData == null )
                throw new InvalidOperationException(
                    "This model does not contain a SkinningData tag." );

        }

    }
}
