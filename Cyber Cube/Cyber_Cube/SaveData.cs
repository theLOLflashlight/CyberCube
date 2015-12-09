using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CyberCube
{
    public struct Score : IComparable<Score>
    {
        public int score;
        public string name;

        public int CompareTo( Score other )
        {
            return score.CompareTo( other.score );
        }
    }

    public class SaveData
    {
        /// <summary>
        /// Name of the file that the highscore data is saved to.
        /// </summary>
        public static string SaveFileAppend = "save1.sav";

        /// <summary>
        /// Get a SaveData object from memory or create a new one with default scores.
        /// </summary>
        public static SaveData Load( string levelName )
        {
            SaveData saveData;
            XmlSerializer xmlSerializer = new XmlSerializer( typeof( SaveData ) );

            try
            {
                var stream = StorageManager.Instance.OpenReadFile( levelName + SaveData.SaveFileAppend );
                saveData = (SaveData)xmlSerializer.Deserialize( stream );
                StorageManager.Instance.Finish();
            }
            catch( Exception e )
            {
                saveData = new SaveData();
                saveData.Initialize();
                saveData.Save( levelName );
            }

            return saveData;
        }

        /// <summary>
        /// List of 10 ordered highscores.
        /// </summary>
        public List<Score> Scores { get; set; }

        public SaveData()
        {
        }

        public void Initialize()
        {
            Scores = new List<Score>( 10 );
        }

        public void AddScore( int newScore, string newName )
        {
            if( newScore > Scores.First().score )
            {
                // Remove the lowest score, add the new score, then sort.
                Scores.RemoveAt( 0 );
                Scores.Add( new Score { score = newScore, name = newName } );
                Scores.Sort();

                if( Scores.Count() > 10 )
                    throw new Exception( "Too many highscores recorded " );
            }
        }

        public void Save( string levelName )
        {
            var stream = StorageManager.Instance.OpenWriteFile( levelName + SaveData.SaveFileAppend );
            new XmlSerializer( typeof( SaveData ) ).Serialize( stream, this );
            StorageManager.Instance.Finish();
        }
    }
}
