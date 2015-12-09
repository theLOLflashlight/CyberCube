using CyberCube.Actors;
using CyberCube.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System;
using CyberCube.Tools;
using Microsoft.Xna.Framework.Audio;
using System.Threading;

namespace CyberCube.Screens
{
    /// <summary>
    /// Screen which contains a cube which can be played, but not edited.
    /// </summary>
    public class PlayScreen : CubeScreen
    {
        private static SpriteFont sFont;

        private static Song sSong;
        private static SoundEffectInstance sSfxAlarm;
        private float mVolume = 0.2f;


        public static void LoadContent( ContentManager content )
        {
            sFont = content.Load<SpriteFont>( "MessageFont" );

            sSong = content.Load<Song>("Audio\\GameplayTrack");

            SoundEffect alarm = content.Load<SoundEffect>("Audio\\alarm");
            sSfxAlarm = alarm.CreateInstance();
            sSfxAlarm.Volume = 0.1f;

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

        public void AddEnemy( Vector3 pos, float rotation )
        {
            Enemy enemy = new Enemy( this, Cube, pos, rotation );
            Enemies.Add( enemy );
            Components.Add( enemy );

            enemy.Initialize();
        }

        public void AddProjectile( Vector3 pos, Vector3 direction, float rotation )
        {
            Projectile projectile = new Projectile( this, Cube, pos, direction, rotation );
            Projectiles.Add( projectile );
            Components.Add( projectile );

            projectile.Initialize();
        }

        public void RemoveProjectile( Projectile projectile )
        {
            Components.Remove( projectile );
            Projectiles.Remove( projectile );

            projectile.Dispose();
        }

        public void PlayAlarm()
        {
            sSfxAlarm.Play();
        }

        public void ResetLevel()
        {
            mPlayerClones.Clear();
            mActivePlayerIndex = 0;
            CubePosition start = Cube.StartPosition;
            
            AddPlayer( start.Position, start.Rotation );
            Cube.CenterOnPlayer(mPlayerClones[0]);
        }

        private void OnCloneChanged( Player player )
        {
            AchievementManager.Instance[ Stat.Swap ]++;

            float dist = Vector3.Distance( player.WorldPosition, Player.WorldPosition );

            Camera.AnimateTarget( player.WorldPosition, dist * 4 );
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
            get; set;
        }
        
        public List<Projectile> Projectiles
        {
            get; set;
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
            MediaPlayer.Volume = mVolume;
            //MediaPlayer.Play(mSong);
            MediaPlayer.IsRepeating = true;

            /*Random rand = new Random();

            for ( int i = 0 ; i < 10 ; ++i )
            {
                PlayableCube cube = new PlayableCube( Game, this );
                cube.Position = new Vector3(
                    (float) (rand.NextDouble() - 0.5) * 10,
                    (float) (rand.NextDouble() - 0.5) * 10,
                    (float) (rand.NextDouble() - 0.5) * 10 );

                cube.Enabled = false;
                Components.Add( cube );
            }*/
        }
        #endregion

        private bool mEndLevel = false;

        public void EndLevel()
        {
            if ( !mEndLevel )
            {
                //MediaPlayer.Stop();
                this.Back();
            }
            mEndLevel = true;
        }

        private List<double> mTimes = new List<double>();

        public double PlayTimeSeconds
        {
            get {
                double totalTime = 0;

                for ( int i = 1; i < mTimes.Count; i += 2 )
                    totalTime += mTimes[ i ] - mTimes[ i - 1 ];

                return totalTime;
            }
        }

        public override void Resume( GameTime gameTime )
        {
            base.Resume( gameTime );
            mTimes.Add( gameTime.TotalGameTime.TotalSeconds );
        }

        public override void Pause( GameTime gameTime )
        {
            base.Pause( gameTime );
            mTimes.Add( gameTime.TotalGameTime.TotalSeconds );
        }

        public override void Destroy( GameTime gameTime )
        {
            base.Destroy( gameTime );
            mTimes.Add( gameTime.TotalGameTime.TotalSeconds );
        }

        internal PlayScreen mNextPlayScreen = null;

        internal Thread mLoadThread;

        private void LoadNextLevelAsync()
        {
#if XBOX
            mLoadThread.SetProcessorAffinity( 3 );
#endif
            PlayableCube playCube = new PlayableCube( Game );
            playCube.Load( Cube.NextLevel );
            mNextPlayScreen = new PlayScreen( Game, playCube );
            /* your code goes here */
        }

        public void NextLevel( string filename = null )
        {
            EndLevel();

            //PlayableCube playCube = new PlayableCube( Game );
            //playCube.Load( filename ?? Cube.NextLevel );
            //PlayScreen playScreen = new PlayScreen( Game, playCube );

            //mLoadThread.Join();
            //ScreenManager.PushScreen( mNextPlayScreen );
            EndLevelScreen endLevelScreen = new EndLevelScreen(
                Game, Cube.Name, this );

            // Pass information?
            ScreenManager.PushScreen( endLevelScreen );
        }

        public override void Initialize()
        {
            base.Initialize();

            mLoadThread = new Thread(
            new ThreadStart( LoadNextLevelAsync ) );
            mLoadThread.Start();

            CubePosition start = Cube.StartPosition;
            List<CubePosition> enemyPositions = Cube.EnemyPositions;
            
            Enemies = new List<Enemy>();
            Projectiles = new List<Projectile>();
            foreach ( CubePosition pos in enemyPositions )
            {
                AddEnemy( pos.Position, pos.Rotation );
            }

            AddPlayer( start.Position, start.Rotation );

            Player = PendingPlayer;

            Camera.Target = Player.WorldPosition;
            Camera.Position = ComputeCameraPosition( Player ) + Cube.Position;
            Camera.UpVector = Player.CubeFace.UpVec.Rotate( Player.Normal, -Player.Rotation );
        }


        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );
            Player = PendingPlayer;

            if ( mEndLevel )
                return;

            Player p = Player;

            foreach ( Enemy enemy in Enemies )
            {
                
            }

            if ( Camera.IsTargetAnimating )
                Camera.AnimateTarget( p.WorldPosition );
            else
                Camera.Target = p.WorldPosition;

            //Camera.Position = ComputeCameraPosition( Player ) + Cube.Position;
            Camera.OrbitPosition( ComputeCameraPosition( Player ) + Cube.Position,
                Cube.Position, Cube.CameraDistance * 4 );
            //Camera.AnimatePosition( ComputeCameraPosition( p ), Cube.CameraDistance * 4 );
            Camera.AnimateUpVector( p.CubeFace.UpVec.Rotate( p.Normal, -p.Rotation ), 1.4f );
        }

        private Vector3 ComputeCameraPosition( Player p )
        {
            Vector3 playerPos = p.CubePosition;
            Vector3 normal = p.Normal;

            HyperVector3 cubePos = new HyperVector3( playerPos );
            cubePos.Cascade( normal );

            const float sqrt2 = 1.41421356237f;
            Vector3 bias = cubePos.Coalesce(
                v => MathTools.TransformRange( (v / Cube.Scale).Length(), .9f * sqrt2, sqrt2, 0, 1, true ) );

            Vector3 defaultPos = normal * (Cube.CameraDistance - 1) + playerPos;
            //defaultPos = defaultPos.ChangeLength( Cube.CameraDistance );

            HyperVector3 projPos = cubePos.ChangeLength( Cube.CameraDistance );
            projPos.Cascade( defaultPos );

            Vector3 projPosA, projPosB;
            float biasA, biasB;

            #region Project and bias setup
            switch ( normal.LargestComponent() )
            {
            case Vector3Component.X:
                projPosA = projPos.Y;
                projPosB = projPos.Z;
                biasA = bias.Y;
                biasB = bias.Z;
                break;

            case Vector3Component.Y:
                projPosA = projPos.X;
                projPosB = projPos.Z;
                biasA = bias.X;
                biasB = bias.Z;
                break;

            case Vector3Component.Z:
                projPosA = projPos.Y;
                projPosB = projPos.X;
                biasA = bias.Y;
                biasB = bias.X;
                break;

            default:
                throw new EnumException<Vector3Component>( "switch statement" );
            }
            #endregion

            return VectorUtils.Slerp(
                defaultPos.Slerp( projPosA, biasA ),
                defaultPos.Slerp( projPosB, biasB ),
                MathTools.TransformRange( biasB - biasA, -1, 1, 0, 1 ) );
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
