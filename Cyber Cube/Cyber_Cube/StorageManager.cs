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
    /// Holds the data of an individual save.
    /// 
    /// Developed by Morgan Wynne.
    /// </summary>
    public struct GameDataStorage
    {
        //TODO: Data goes here
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
        /// 
        /// TODO: Change this to private and make accessors for it once testing is completed.
        /// </summary>
        public GameDataStorage data;

        /// <summary>
        /// Represents a storage device for user data, such as a memory unit or hard drive.
        /// </summary>
        private StorageDevice storageDevice;

        /// <summary>
        /// Serializer for the 'data' variable into XML if on a Windows machine.
        /// </summary>
        private XmlSerializer serializer;

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
            this.data = new GameDataStorage();
            serializer = new XmlSerializer( typeof( GameDataStorage ) );
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
        public void Save()
        {
            if( !Guide.IsVisible )
            {
                storageDevice = null;
                var result = StorageDevice.BeginShowSelector( PlayerIndex.One, null, null );

                StorageContainer container = this.InitializeStorageContainer( result );

                // If the file exists, delete it and re-create it.
                if( container.FileExists( fileName ) )
                    container.DeleteFile( fileName );

                // Open the file, stream the data, and close the file.
                using( Stream stream = container.CreateFile( fileName ) )
                {
                    serializer.Serialize( stream, data );
                }

                result.AsyncWaitHandle.Close();
                container.Dispose();
            }
        }

        /// <summary>
        /// Begins the loading of data from a save file into the 'data' variable.
        /// </summary>
        public void Load()
        {
            if( !Guide.IsVisible )
            {
                storageDevice = null;
                var result = StorageDevice.BeginShowSelector( PlayerIndex.One, null, null );

                StorageContainer container = this.InitializeStorageContainer( result );

                if( container.FileExists( fileName ) )
                {
                    using( Stream stream = container.OpenFile( fileName, FileMode.Open ) )
                    {
                        data = (GameDataStorage)serializer.Deserialize( stream );
                    }
                }

                result.AsyncWaitHandle.Close();
                container.Dispose();
            }
        }
    }
}
