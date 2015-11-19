using CyberCube.Actors;
using CyberCube.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System;

namespace CyberCube.Screens
{
    /// <summary>
    /// Screen which contains a cube which can be played, but not edited.
    /// </summary>
    public class PlayScreen : CubeScreen
    {
        private static SpriteFont sFont;

        private static Song mSong;
        private float mVolume = 0.2f;


        public static void LoadContent( ContentManager content )
        {
            sFont = content.Load<SpriteFont>( "MessageFont" );

            mSong = content.Load<Song>("Audio\\GameplayTrack");
        }

        public delegate void CloneChangedHandler( Player player );

        public event CloneChangedHandler CloneChanged;

        public Player Player
        {
            get; private set;
        }

        public Player PendingPlayer
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
            {
                ResetLevel();
            }
            else
            {
                mActivePlayerIndex %= mPlayerClones.Count;
                OnCloneChanged( PendingPlayer );
            }
        }

        public void ResetLevel()
        {
            mPlayerClones.Clear();
            mActivePlayerIndex = 0;
            CubePosition start = Cube.StartPosition;
            AddPlayer( start.Position, start.Rotation );
        }

        private void OnCloneChanged( Player player )
        {
            Camera.AnimateTarget( player.WorldPosition, 10 );
            CloneChanged?.Invoke( player );
        }

        public void NextClone()
        {
            if ( mPlayerClones.Count > 1 )
            {
                mActivePlayerIndex += 1;
                mActivePlayerIndex %= mPlayerClones.Count;
                OnCloneChanged( PendingPlayer );
            }
        }

        public void PreviousClone()
        {
            if ( mPlayerClones.Count > 1 )
            {
                mActivePlayerIndex += mPlayerClones.Count - 1;
                mActivePlayerIndex %= mPlayerClones.Count;
                OnCloneChanged( PendingPlayer );
            }
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
            MediaPlayer.Volume = mVolume;
            //MediaPlayer.Play(mSong);
            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// Creates a new PlayScreen.
        /// </summary>
        /// <param name="game">Game the PlayScreen should be associated with.</param>
        /// <param name="playCube">PlayableCube the screen should display.</param>
        public PlayScreen( CubeGame game, PlayableCube playCube )
            : base( game, playCube )
        {
            MediaPlayer.Volume = mVolume;
            //MediaPlayer.Play(mSong);
            MediaPlayer.IsRepeating = true;

            Enemies = new List<Enemy>();
            //Enemies.Add( new Enemy( this, Cube, Vector3.UnitZ, Direction.Up ) );
            //Enemies.Add( new Enemy( this, Cube, Vector3.UnitY, Direction.Up ) );

            //foreach( Enemy enemy in Enemies ) 
            //    Components.Add( enemy );
        }
        #endregion

        private bool mEndLevel = false;

        public void EndLevel()
        {
            if ( !mEndLevel )
            {
                MediaPlayer.Stop();
                this.Back();
            }
            mEndLevel = true;
        }

        public void NextLevel( string filename )
        {
            EndLevel();

            PlayableCube playCube = new PlayableCube( Game );
            playCube.Load( filename );
            PlayScreen playScreen = new PlayScreen( Game, playCube );
            ScreenManager.PushScreen( playScreen );
            EndLevelScreen endLevelScreen = new EndLevelScreen( Game );
            // Pass information?
            ScreenManager.PushScreen( endLevelScreen );
        }

        public override void Initialize()
        {
            base.Initialize();

            CubePosition start = Cube.StartPosition;
            AddPlayer( start.Position, start.Rotation );
        }

        public override void Update( GameTime gameTime )
        {
            Player = PendingPlayer;
            base.Update( gameTime );

            if ( mEndLevel )
                return;

            Player p = PendingPlayer;

            Vector3 pos = p.Normal * Cube.CameraDistance;
            pos += p.WorldPosition * 2;
            pos.Normalize();

            if ( Camera.IsTargetAnimating )
                Camera.AnimateTarget( p.WorldPosition );
            else
                Camera.Target = p.WorldPosition;

            Camera.AnimatePosition( pos * Cube.CameraDistance, Cube.CameraDistance * 2 );
            Camera.AnimateUpVector( p.CubeFace.UpVec.Rotate( p.Normal, -p.Rotation ) );
        }

        public override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );

            mSpriteBatch.Begin();
            
#if DEBUG
            //mSpriteBatch.DrawString( sFont, $"collisions: {Player?.NumFootContacts}", new Vector2( 0, 60 ), Color.White );
#endif
            mSpriteBatch.End();
        }
    }
}
