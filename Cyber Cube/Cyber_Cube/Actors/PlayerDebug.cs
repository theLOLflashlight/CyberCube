using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Actors
{
    public partial class Player
    {
#if DEBUG

        public int NumFootContacts
        {
            get {
                return mNumFootContacts;
            }
        }

#endif
    }
}
