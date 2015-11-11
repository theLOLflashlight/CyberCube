using CyberCube.Actors;
using CyberCube.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CyberCube.Screens
{
    /// <summary>
    /// Screen which contains a cube which can be played, but not edited.
    /// </summary>
    public class PlayScreen : CubeScreen
    {
        private static SpriteFont sFont;

        public static void LoadContent( ContentManager content )
        {
            sFont = content.Load<SpriteFont>( "MessageFont" );
        }

        public Player Player
        {
            get; private set;
        }

        public List<Enemy> Enemies
        {
            get; private set;
        }

        #region Boilerplate
        /// <summary>
        /// Hides the base Cube property, exposing the methods of the PlayableCube class.
        /// </summary>
        public new PlayableCube Cube
        {
            get {
                return (PlayableCube) base.Cube;
            }
            protected set {
                base.Cube = value;
            }
        }

        /// <summary>
        /// Creates a new PlayScreen.
        /// </summary>
        /// <param name="game">Game the PlayScreen should be associated with.</param>
        public PlayScreen( CubeGame game )
            : this( game, new PlayableCube( game ) )
        {
        }

        /// <summary>
        /// Creates a new PlayScreen.
        /// </summary>
        /// <param name="game">Game the PlayScreen should be associated with.</param>
        /// <param name="playCube">PlayableCube the screen should display.</param>
        public PlayScreen( CubeGame game, PlayableCube playCube )
            : base( game, playCube )
        {
            Player = new Player( this, Cube, Vector3.UnitZ, Direction.Up );
            Enemies = new List<Enemy>();
            Enemies.Add( new Enemy( this, Cube, Vector3.UnitZ, Direction.Up ) );
            Enemies.Add( new Enemy( this, Cube, Vector3.UnitY, Direction.Up ) );

            Components.Add( Player );
            foreach( Enemy enemy in Enemies ) 
            {
                Components.Add( enemy );
            }
        }
        #endregion

        private bool mEndLevel = false;

        public void EndLevel()
        {
            mEndLevel = true;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            if ( mEndLevel )
                this.Back();
        }

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );

            mSpriteBatch.Begin();
#if DEBUG
            mSpriteBatch.DrawString( sFont, $"collisions: {Player.NumFootContacts}", new Vector2( 0, 60 ), Color.White );
#endif
            mSpriteBatch.End();
        }
    }
}
