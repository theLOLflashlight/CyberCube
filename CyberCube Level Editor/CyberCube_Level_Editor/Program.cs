using System;

namespace CyberCube.Editor
{
#if WINDOWS
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CubeEditorGame game = new CubeEditorGame())
            {
                game.Run();
            }
        }
    }
#endif
}

