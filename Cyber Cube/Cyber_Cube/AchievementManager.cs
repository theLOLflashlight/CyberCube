﻿using System;
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
    public enum Stat { Jump }

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
            public int Goal { get; set; }
        }

        public bool Achieved( Dictionary< Stat, int > stats )
        {
            // TODO: Implement this
            return true;
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
        private AchievementManager()
        {
            foreach( Stat stat in Enum.GetValues( typeof( Stat ) ) )
            {
                this[ stat ] = 0;
            }
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
                return stats[ index ];
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

        /// <summary>
        /// Gets the achievements that the player accomplished and reset the stats.
        /// Meant to be called at the end of a level.
        /// </summary>
        /// <returns></returns>
        public List<Achievement> GetAchieved()
        {
            List<Achievement> achieved = (List<Achievement>)Achievements.Where( a => a.Achieved( stats ) );
            this.ResetStats();
            return achieved;
        }
    }
}
