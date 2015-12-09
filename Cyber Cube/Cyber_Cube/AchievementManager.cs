using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace CyberCube
{
    /// <summary>
    /// All of the actions 
    /// </summary>
    public enum Stat { Jump, Second, Clone, Die, Swap }

    public class Achievement
    {
        //public delegate bool Requirement2( int value );

        /// <summary>
        /// Value, in score, that's gained when achieving this.
        /// </summary>
        public int Value { get; set; }
        
        /// <summary>
        /// Name of the achievement.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// List of requirements needed to achieve this achievement.
        /// </summary>
        public List<Requirement> Requirements;

        public class Requirement
        {
            public Stat AssociatedStat { get; set; }
            public string Goal { get; set; }
        }

        public bool Achieved( Dictionary< Stat, int > stats )
        {
            foreach( Requirement requirement in Requirements )
            {
                if( !stats.ContainsKey( requirement.AssociatedStat ) )
                    stats[ requirement.AssociatedStat ] = 0;

                char comparison = requirement.Goal.ToCharArray()[ 0 ];
                int goal = int.Parse( requirement.Goal.Remove( 0, 1 ) );
                int stat = stats[ requirement.AssociatedStat ];

                switch( comparison )
                {
                    case '>':
                    if( stat >= goal )
                        return true;
                    break;
                    case '<':
                    if( stat < goal )
                        return true;
                    break;
                    default:
                    if( stat == goal )
                        return true;
                    break;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Developed by Morgan Wynne.
    /// </summary>
    public class AchievementManager
    {
        private static readonly AchievementManager instance = new AchievementManager();
        static AchievementManager()
        {
            try
            {
                instance = (AchievementManager)new XmlSerializer( typeof( AchievementManager ) ).Deserialize( TitleContainer.OpenStream( "Achievements.xml" ) );
            }
            catch( Exception e ) {
            }
        }
        public AchievementManager()
        {
        }

        /// <summary>
        /// Usable instance of the AchievementManager class.
        /// </summary>
        public static AchievementManager Instance
        { get { return instance; } }

        public void Initialize()
        {
        }

        /// <summary>
        /// Skips having to reference the Stats dictionary.
        /// </summary>
        public int this[ Stat index ]
        {
            get
            {
                return ( stats.ContainsKey( index ) ) ? stats[ index ] : stats[ index ] = 0;
            }
            set
            {
                stats[ index ] = value;
            }
        }

        /// <summary>
        /// Loaded list of achievements the user can get.
        /// </summary>
        public List<Achievement> Achievements = new List<Achievement>();

        /// <summary>
        /// Current statistics being tracked.
        /// </summary>
        private Dictionary< Stat, int > stats = new Dictionary<Stat, int>();

        /// <summary>
        /// Resets all the stacks tracked in the level.
        /// </summary>
        public void ResetStats()
        {
            this.stats = new Dictionary<Stat, int>();
        }

        public int GetCurrentScore()
        {
            int score = 0;

            var achieved = Achievements.Where( a => a.Achieved( stats ) );
            foreach( Achievement achievement in achieved )
                score += achievement.Value;
            return score;
        }

        /// <summary>
        /// Gets the achievements that the player accomplished and reset the stats.
        /// Meant to be called at the end of a level.
        /// </summary>
        /// <returns></returns>
        public List<Achievement> GetAchieved()
        {
            List<Achievement> achieved = Achievements.Where( a => a.Achieved( stats ) ).ToList();

            this.ResetStats();
            return achieved;
        }
    }
}