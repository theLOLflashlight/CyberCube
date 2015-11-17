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
            get {
                return mPlayerClones.Count > 0
                    ? mPlayerClones[ mActivePlayerIndex ]
                    : null;
            }
        }

        private int mActivePlayerIndex = 0;

        private List< Player > mPlayerClones = new List<Player>();

        public void AddPlayer( Vector3 pos, float rotation )
        {
            Player clone = new Player( this, Cube, pos, rotation );

            mPlayerClones.Add( clone );
            Components.Add( clone );

            clone.Initialize();
        }

        public void RemovePlayer( Player player )
        {
            Components.Remove( player );
            mPlayerClones.Remove( player );

            player.Dispose();

            if ( mPlayerClones.Count == 0 )
                EndLevel();
            else
                mActivePlayerIndex %= mPlayerClones.Count;
        }

        public void NextClone()
        {
            mActivePlayerIndex += 1;
            mActivePlayerIndex %= mPlayerClones.Count;
        }

        public void PreviousClone()
        {
            mActivePlayerIndex += mPlayerClones.Count - 1;
            mActivePlayerIndex %= mPlayerClones.Count;
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
            Enemies = new List<Enemy>();
            Enemies.Add( new Enemy( this, Cube, Vector3.UnitZ, Direction.Up ) );
            Enemies.Add( new Enemy( this, Cube, Vector3.UnitY, Direction.Up ) );

            //Components.Add( Player );
            foreach( Enemy enemy in Enemies ) 
            {
                Components.Add( enemy );
            }
        }
        #endregion

        private bool mEndLevel = false;

        public void EndLevel()
        {
            if ( !mEndLevel )
                this.Back();
            mEndLevel = true;
        }

        public override void Initialize()
        {
            base.Initialize();

            CubePosition start = Cube.StartPosition;
            AddPlayer( start.Position, start.Rotation );
        }

        public override void Update( GameTime gameTime )
        {
            var input = Game.Input;

            if ( !input.HasFocus || input.CheckFocusType<Player>() )
                input.Focus = Player;

            base.Update( gameTime );

            if ( mEndLevel )
                return;

            Player p = Player;

            Vector3 pos = p.Normal * Cube.CameraDistance;
            pos += p.WorldPosition * 2;
            pos.Normalize();

            Camera.Target = p.WorldPosition;
            Camera.AnimatePosition( pos * Cube.CameraDistance, Cube.CameraDistance * 2 );
            Camera.AnimateUpVector( p.CubeFace.UpVec.Rotate( p.Normal, -p.Rotation ) );
        }

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );

            mSpriteBatch.Begin();
#if DEBUG
            mSpriteBatch.DrawString( sFont, $"collisions: {Player?.NumFootContacts}", new Vector2( 0, 60 ), Color.White );
#endif
            mSpriteBatch.End();
        }
    }
}
