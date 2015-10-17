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
#if WINDOWS
using System.Xml.Serialization;
#endif

namespace Cyber_Cube
{
    /// <summary>
    /// Holds an array of saves.
    /// 
    /// Developed by Morgan Wynne.
    /// </summary>
    public struct GameDataStorage
    {
        /// <summary>
        /// Array of individual player data.
        /// </summary>
        public PlayerDataSave[] PlayerDataSaves;

        /// <summary>
        /// Public constructor, used purely to set how many players the struct has to keep track of. 
        /// </summary>
        /// <param name="count">The maximum number of players that the save file will keep track of at any point in time.</param>
        public GameDataStorage( int count )
        {
            this.PlayerDataSaves = new PlayerDataSave[ count ];
        }
    }

    /// <summary>
    /// Holds the data of an individual save.
    /// 
    /// Developed by Morgan Wynne.
    /// </summary>
    public struct PlayerDataSave
    {
        public Vector3 PlayerWorldPosition;
    }

    /// <summary>
    /// Manager singleton for storing and retreiving high scores and current progress.
    /// Created from tutorial at http://www.pacificsimplicity.ca/blog/loading-and-save-highscore-xmlisolatedstorage-tutorial.
    /// 
    /// Developed by Morgan Wynne.
    /// </summary>
    /// <example>
    /// This sample shows how to access the instance of this style of singleton from elsewhere in the program.
    /// <code>
    /// StorageManager.Instance;
    /// </code>
    /// </example>
    public class StorageManager
    {
        /// <summary>
        /// The name of the save file that the data is streamed to.
        /// </summary>
        public string fileName = "cybercube.sav";

        /// <summary>
        /// The name of the container to load/save from within the save file.
        /// </summary>
        public string containerName = "cybercubesave";

        /// <summary>
        /// Singleton instance of the StorageManager class.
        /// </summary>
        private static readonly StorageManager instance = new StorageManager();

        /// <summary>
        /// Currently loaded player data.
        /// </summary>
        private GameDataStorage data;

        /// <summary>
        /// TODO: Remove this, this is just for convenience.
        /// </summary>
        public Vector3 PlayersWorldPosition { get { return data.PlayerDataSaves[ 0 ].PlayerWorldPosition; } }

        /// <summary>
        /// Represents a storage device for user data, such as a memory unit or hard drive.
        /// </summary>
        private StorageDevice storageDevice;

#if WINDOWS
        /// <summary>
        /// Serializer for the 'data' variable into XML if on a Windows machine.
        /// </summary>
        private XmlSerializer serializer;
#endif

        static StorageManager()
        {
        }
        private StorageManager()
        {
        }

        /// <summary>
        /// Singleton instance of the StorageManager class.
        /// </summary>
        public static StorageManager Instance
        { get { return instance; } }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            // TODO: Define how many saves there will be.
            this.data = new GameDataStorage( 1 );
#if WINDOWS
            serializer = new XmlSerializer( typeof( GameDataStorage ) );
#endif
        }

        private StorageContainer InitializeStorageContainer( IAsyncResult result )
        {
            storageDevice = StorageDevice.EndShowSelector( result );
            if( storageDevice == null || !( storageDevice.IsConnected ) )
                throw new Exception( "StorageDevice was null or not connected." );

            IAsyncResult r = storageDevice.BeginOpenContainer( containerName, null, null );
            result.AsyncWaitHandle.WaitOne();
            return storageDevice.EndOpenContainer( r );
        }

        /// <summary>
        /// Begins the saving of data from the 'data' variable into the save file.
        /// </summary>
        public void Save( Player player )
        {
            data.PlayerDataSaves[ 0 ].PlayerWorldPosition = player.WorldPosition;

            if( !Guide.IsVisible )
            {
                storageDevice = null;
                StorageDevice.BeginShowSelector( PlayerIndex.One, this.SaveGameDataStorage, null );
            }
        }

        /// <summary>
        /// AsyncCallback for StorageDevice.BeginShowSelector. Holds actual saving functionality.
        /// </summary>
        private void SaveGameDataStorage( IAsyncResult result )
        {
            StorageContainer container = this.InitializeStorageContainer( result );
            
            // If the file exists, delete it and re-create it.
            if( container.FileExists( fileName ) )
                container.DeleteFile( fileName );

            // Open the file, stream the data, and close the file.
            using( Stream stream = container.CreateFile( fileName ) )
            {
#if WINDOWS
                serializer.Serialize( stream, data );
#else
                using( StreamWriter sw = new StreamWriter( stream ) )
                {
                    // TODO: Implement Xbox serialization to stream here.
                    sw.Close();
                }
#endif
            }

            // Ends the saving operation.
            this.EndOperation( result, container );
        }

        /// <summary>
        /// Begins the loading of data from a save file into the 'data' variable.
        /// </summary>
        public void Load()
        {
            if( !Guide.IsVisible )
            {
                storageDevice = null;
                StorageDevice.BeginShowSelector( PlayerIndex.One, this.LoadGameDataStorage, null );
            }
        }

        /// <summary>
        /// AsyncCallback for StorageDevice.BeginShowSelector. Holds actual loading functionality.
        /// </summary>
        private void LoadGameDataStorage( IAsyncResult result ) 
        {
            StorageContainer container = this.InitializeStorageContainer( result );

            if( container.FileExists( fileName ) )
            {
                using( Stream stream = container.OpenFile( fileName, FileMode.Open ) )
                {
#if WINDOWS
                    data = (GameDataStorage)serializer.Deserialize( stream );
#else
                    using( StreamReader sr = new StreamReader( stream ) )
                    {
                        // TODO: Implement Xbox deserialization from stream here
                        sr.Close();
                    }
#endif
                }
            }

            // Ends the loading operation.
            this.EndOperation( result, container );
        }

        /// <summary>
        /// Disposes and closes all objects required for accessing saved data.
        /// </summary>
        private void EndOperation( IAsyncResult result, StorageContainer container )
        {
            // Disposes of the storage container to finalize changes to save file.
            container.Dispose();
            // Releases the Async Handle.
            result.AsyncWaitHandle.Close();
        }

    }
}
